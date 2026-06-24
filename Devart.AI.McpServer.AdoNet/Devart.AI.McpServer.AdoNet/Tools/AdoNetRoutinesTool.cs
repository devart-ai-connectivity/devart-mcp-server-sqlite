// --------------------------------------------------------------------------
// <copyright file="AdoNetRoutinesTool.cs" company="Devart">
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
using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.AdoNet.Tools
{
  internal sealed class AdoNetRoutinesTool(McpConfiguration serverConfiguration) : RoutinesTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken)
    {
      var resultTable = await base.GetMetadataTable(connection, schema, tableName, services, cancellationToken).ConfigureAwait(false);

      resultTable.Columns.Add(AdoNetConstants.ProcedureSchema, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ProcedureName, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ProcedureType, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ProcedureParameters, typeof(string));

      var functions = await connection.GetSchemaAsync(
        AdoNetConstants.Functions, 
        [],
        cancellationToken
      ).ConfigureAwait(false);

      foreach (DataRow func in functions.Rows)
      {
        var functionSchema = func[AdoNetConstants.ProcedureSchema]?.ToString();
        var functionName = func[AdoNetConstants.ProcedureName]?.ToString();
        var functionParams = func[AdoNetConstants.ProcedureParameters]?.ToString();

        resultTable.Rows.Add(functionSchema, functionName, "FUNCTION", functionParams);
      }

      var procedures = await connection.GetSchemaAsync(
        AdoNetConstants.Procedures, 
        [],
        cancellationToken
      ).ConfigureAwait(false);

      foreach (DataRow proc in procedures.Rows)
      {
        var procSchema = proc[AdoNetConstants.ProcedureSchema]?.ToString();
        var procName = proc[AdoNetConstants.ProcedureName]?.ToString();
        var procParams = proc[AdoNetConstants.ProcedureParameters]?.ToString();

        resultTable.Rows.Add(procSchema, procName, "PROCEDURE", procParams);
      }

      return resultTable;
    }
  }
}
