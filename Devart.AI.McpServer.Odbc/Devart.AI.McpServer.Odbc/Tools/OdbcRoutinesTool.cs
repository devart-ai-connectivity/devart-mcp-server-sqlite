// --------------------------------------------------------------------------
// <copyright file="OdbcRoutinesTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.Odbc.Tools
{
  internal sealed class OdbcRoutinesTool(McpConfiguration serverConfiguration) : RoutinesTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken)
    {
      var metadata = services.GetService<IMetadata>();
      var routines = await connection.GetSchemaAsync(
        OdbcMetaDataCollectionNames.Procedures,
        cancellationToken
      ).ConfigureAwait(false);

      var resultTable = await base.GetMetadataTable(connection, schema, tableName, services, cancellationToken).ConfigureAwait(false);

      resultTable.Columns.Add(OdbcConstants.ProcedureSchema, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ProcedureName, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ProcedureType, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ProcedureParameters, typeof(string));
      resultTable.Columns.Add(OdbcConstants.Remarks, typeof(string));

      foreach (DataRow routine in routines.Rows)
      {
        var routineSchema = routine[OdbcConstants.ProcedureSchema]?.ToString();
        var routineName = routine[OdbcConstants.ProcedureName]?.ToString();
        var routineTypeRaw = routine[OdbcConstants.ProcedureType]?.ToString();
        var remarks = routine[OdbcConstants.Remarks]?.ToString();

        if (IsIgnoredSchema(routineSchema))
        {
          continue;
        }

        var routineType = GetRoutineType(routineTypeRaw);
        if (routineType == null)
        {
          continue;
        }

        var routineParameters = await connection.GetSchemaAsync(
          OdbcMetaDataCollectionNames.ProcedureParameters,
          [metadata.DatabaseName(connection.Database), metadata.SchemaName(routineSchema), routineName],
          cancellationToken
        ).ConfigureAwait(false);

        var parametersText = FormatParameters(routineParameters);

        resultTable.Rows.Add(routineSchema, routineName, routineType, parametersText, remarks);
      }

      return resultTable;
    }

    private static string GetRoutineType(string routineTypeRaw) => routineTypeRaw switch
    {
      "1" => McpConstants.ProcedureRoutine,
      "2" => McpConstants.FunctionRoutine,
      _ => null
    };

    private static string FormatParameters(DataTable routineParameters)
    {
      var parametersBuilder = new StringBuilder();
      var parameterRows = routineParameters.AsEnumerable()
        .OrderBy(row => Convert.ToInt32(row[OdbcConstants.OrdinalPosition] ?? 0))
        .ToArray();

      foreach (var parameter in parameterRows)
      {
        var parameterName = parameter[OdbcConstants.ColumnName]?.ToString();
        var parameterType = parameter[OdbcConstants.TypeName]?.ToString();
        var parameterSize = parameter[OdbcConstants.ColumnSize]?.ToString();
        var columnType = parameter[OdbcConstants.ColumnType]?.ToString();
        var nullable = parameter[OdbcConstants.Nullable]?.ToString();
        var ordinalPosition = parameter[OdbcConstants.OrdinalPosition]?.ToString();

        if (columnType == "3")
        {
          continue;
        }

        if (parametersBuilder.Length > 0)
        {
          parametersBuilder.Append(", ");
        }

        var direction = GetParameterDirection(columnType);

        var displayName = string.IsNullOrWhiteSpace(parameterName) ? $"param{ordinalPosition}" : parameterName;

        parametersBuilder.Append($"{direction} {displayName} {parameterType}");

        if (!string.IsNullOrWhiteSpace(parameterSize) && parameterSize != "0")
        {
          parametersBuilder.Append($"({parameterSize})");
        }

        var nullableText = IsNullable(nullable);
        if (!string.IsNullOrEmpty(nullableText))
        {
          parametersBuilder.Append(nullableText);
        }
      }
      return parametersBuilder.ToString();
    }

    private static string GetParameterDirection(string columnType) => columnType switch
    {
      "1" => "IN",
      "2" => "INOUT",
      "4" => "OUT",
      "5" => "RETURN",
      _ => "IN"
    };

    private static string IsNullable(string nullable) => nullable switch
    {
      "0" => " NOT NULL",
      "1" => " NULL",
      _ => string.Empty
    };
  }
}
