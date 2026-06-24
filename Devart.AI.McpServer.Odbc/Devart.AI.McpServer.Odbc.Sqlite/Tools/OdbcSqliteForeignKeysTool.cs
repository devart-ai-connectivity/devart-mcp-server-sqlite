// --------------------------------------------------------------------------
// <copyright file="OdbcSqliteForeignKeysTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.AI.McpServer.Odbc.Sqlite.Tools
{
  internal sealed class OdbcSqliteForeignKeysTool(McpConfiguration serverConfiguration) : ForeignKeysTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();
      var commandHelper = services.GetRequiredService<ICommandHelper>();

      DataTable resultTable = new(tableName) {
        Locale = System.Globalization.CultureInfo.InvariantCulture
      };

      resultTable.Columns.Add(OdbcConstants.ForeignKeyFkName, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ForeignKeyFkColumn, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ForeignKeyPkSchema, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ForeignKeyPkTable, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ForeignKeyPkColumn, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ForeignKeyUpdateRule, typeof(string));
      resultTable.Columns.Add(OdbcConstants.ForeignKeyDeleteRule, typeof(string));

      await using var reader = await database.ExecuteReaderAsync(
        connection,
        $"PRAGMA {connection.Database}.foreign_key_list({tableName})",
        null,
        cancellationToken
      ).ConfigureAwait(false);

      while (await reader.ReadAsync(cancellationToken))
      {
        resultTable.Rows.Add(
          $"FK_{tableName}_{reader.GetString(2)}",
          reader.GetString(3),
          connection.Database,
          reader.GetString(2),
          reader.GetString(4),
          reader.GetString(5),
          reader.GetString(6));
      }

      return resultTable;
    }
  }
}
