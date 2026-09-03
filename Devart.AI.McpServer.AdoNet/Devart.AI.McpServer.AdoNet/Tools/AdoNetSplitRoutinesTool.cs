// --------------------------------------------------------------------------
// <copyright file="AdoNetSplitRoutinesTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.AdoNet.Tools
{
  public abstract class AdoNetSplitRoutinesTool(McpConfiguration serverConfiguration)
    : RoutinesTool(serverConfiguration)
  {
    private const string FunctionsCollection = "Functions";
    private const string ProceduresCollection = "Procedures";

    private const string Function = "FUNCTION";
    private const string Procedure = "PROCEDURE";

    protected abstract string RoutineSchemaColumn { get; }

    protected virtual string RoutineNameColumn => "Name";

    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var metadata = services.GetRequiredService<IMetadata>();
      var mapping = metadata.RoutinesColumnsMapping;

      var resultTable = new DataTable(ProceduresCollection);
      foreach (var column in mapping)
      {
        resultTable.Columns.Add(column.Name, typeof(string));
      }

      await AppendRoutinesAsync(resultTable, mapping, connection, FunctionsCollection, Function, cancellationToken)
        .ConfigureAwait(false);
      await AppendRoutinesAsync(resultTable, mapping, connection, ProceduresCollection, Procedure, cancellationToken)
        .ConfigureAwait(false);

      return resultTable;
    }

    private async Task AppendRoutinesAsync(
      DataTable resultTable,
      MetadataColumn[] mapping,
      DbConnection connection,
      string collectionName,
      string routineType,
      CancellationToken cancellationToken)
    {
      var routines = await connection.GetSchemaAsync(collectionName, cancellationToken).ConfigureAwait(false);

      foreach (DataRow routine in routines.Rows)
      {
        var row = resultTable.NewRow();
        foreach (var column in mapping)
        {
          row[column.Name] = column.Name switch
          {
            AdoNetRoutineColumns.Type => routineType,
            AdoNetRoutineColumns.Schema => routine.TryGetString(RoutineSchemaColumn),
            AdoNetRoutineColumns.Name => routine.TryGetString(RoutineNameColumn),
            _ => routine.TryGetString(column.Name)
          };
        }
        resultTable.Rows.Add(row);
      }
    }
  }
}