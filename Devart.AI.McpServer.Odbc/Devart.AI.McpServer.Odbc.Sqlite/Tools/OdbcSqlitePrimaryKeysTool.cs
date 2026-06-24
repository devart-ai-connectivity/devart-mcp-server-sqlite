// --------------------------------------------------------------------------
// <copyright file="OdbcSqlitePrimaryKeysTool.cs" company="Devart">
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
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.Odbc.Sqlite.Tools
{
  internal sealed class OdbcSqlitePrimaryKeysTool(McpConfiguration serverConfiguration) : PrimaryKeysTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();

      DataTable resultTable = new(OdbcConstants.PrimaryKeysCollectionName)
      {
        Locale = System.Globalization.CultureInfo.InvariantCulture
      };
      resultTable.Columns.Add(OdbcConstants.PrimaryKeyName, typeof(string));
      resultTable.Columns.Add(OdbcConstants.PrimaryKeyColumn, typeof(string));

      var pkName = $"sqlite_autoindex_{tableName}_1";

      await using var reader = await database.ExecuteReaderAsync(
        connection,
        $"PRAGMA {connection.Database}.table_info({tableName})",
        null,
        cancellationToken
      ).ConfigureAwait(false);

      while (await reader.ReadAsync(cancellationToken))
      {
        if (reader.GetInt32(5) > 0)
        {
          resultTable.Rows.Add(pkName, reader.GetString(1));
        }
      }

      return resultTable;
    }
  }
}
