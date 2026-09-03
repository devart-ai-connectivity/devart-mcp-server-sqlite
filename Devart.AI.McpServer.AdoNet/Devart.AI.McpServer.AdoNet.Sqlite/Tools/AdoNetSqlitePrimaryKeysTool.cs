// --------------------------------------------------------------------------
// <copyright file="AdoNetSqlitePrimaryKeysTool.cs" company="Devart">
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

namespace Devart.AI.McpServer.AdoNet.Sqlite.Tools
{
  internal sealed class AdoNetSqlitePrimaryKeysTool(McpConfiguration serverConfiguration) : PrimaryKeysTool(serverConfiguration)
  {
    private const string IndexName = "Name";
    private const string ColumnName = "Name";

    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var resultTable = CreateAnswerTable(
        AdoNetSqliteMetadata.PrimaryKeyName,
        AdoNetSqliteMetadata.PrimaryKeyColumn);

      var metadata = services.GetRequiredService<IMetadata>();
      var database = metadata.DatabaseName(connection.Database);

      using var keys = await connection.GetSchemaAsync(
        metadata.PrimaryKeysCollectionName,
        metadata.PrimaryKeysRestrictions(connection.Database, schema, tableName),
        cancellationToken
      ).ConfigureAwait(false);

      foreach (DataRow key in keys.Rows)
      {
        var keyName = key.TryGetString(IndexName);

        using var columns = await connection.GetSchemaAsync(
          metadata.IndexesCollectionName,
          [database, tableName, keyName],
          cancellationToken
        ).ConfigureAwait(false);

        foreach (DataRow column in columns.Rows)
        {
          resultTable.Rows.Add(keyName, column.TryGetString(ColumnName));
        }
      }

      return resultTable;
    }
  }
}