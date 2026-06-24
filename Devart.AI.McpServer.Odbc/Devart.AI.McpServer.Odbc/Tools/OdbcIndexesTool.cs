// --------------------------------------------------------------------------
// <copyright file="OdbcIndexesTool.cs" company="Devart">
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

namespace Devart.AI.McpServer.Odbc.Tools
{
  internal sealed class OdbcIndexesTool(McpConfiguration serverConfiguration) : IndexesTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var metadata = services.GetRequiredService<IMetadata>();

      var resultTable = await base.GetMetadataTable(connection, schema, tableName, services, cancellationToken).ConfigureAwait(false);

      resultTable.Columns.Add(OdbcConstants.IndexName, typeof(string));
      resultTable.Columns.Add(OdbcConstants.IndexType, typeof(string));
      resultTable.Columns.Add(OdbcConstants.IndexColumn, typeof(string));
      resultTable.Columns.Add(OdbcConstants.OrdinalPosition, typeof(int));

      using var indexes = await connection.GetSchemaAsync(
        metadata.IndexesCollectionName,
        [metadata.DatabaseName(connection.Database), metadata.SchemaName(schema), tableName],
        cancellationToken
      ).ConfigureAwait(false);

      foreach (DataRow index in indexes.Rows)
      {
        var indexName = index[OdbcConstants.IndexName]?.ToString();
        if (indexName == null)
          continue;

        var indexColumn = index[OdbcConstants.IndexColumn]?.ToString();
        var indexColumnOrdinal = index[OdbcConstants.OrdinalPosition];
        var indexUnique = (short)index[OdbcConstants.IndexIsNonUnique] == 0;

        resultTable.Rows.Add(indexName, GetIndexType(indexUnique), indexColumn, indexColumnOrdinal);
      }

      return resultTable;
    }

    private static string GetIndexType(bool unique)
      => unique ? "UNIQUE" : "NON UNIQUE";
  }
}
