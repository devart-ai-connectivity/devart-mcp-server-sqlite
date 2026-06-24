// --------------------------------------------------------------------------
// <copyright file="AdoNetIndexesTool.cs" company="Devart">
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

namespace Devart.AI.McpServer.AdoNet.Tools
{
  internal sealed class AdoNetIndexesTool(McpConfiguration serverConfiguration) : IndexesTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var resultTable = await base.GetMetadataTable(connection, schema, tableName, services, cancellationToken).ConfigureAwait(false);

      resultTable.Columns.Add(AdoNetConstants.IndexName, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.IndexType, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.IndexColumns, typeof(string));

      var metadata = services.GetService<IMetadata>();
      var indexes = await connection.GetSchemaAsync(
        metadata.IndexesCollectionName,
        [metadata.SchemaName(schema), tableName],
        cancellationToken
      ).ConfigureAwait(false);

      foreach (DataRow index in indexes.Rows)
      {
        var indexName = index[AdoNetConstants.IndexName]?.ToString();
        var indexUnique = (bool)index[AdoNetConstants.IndexIsUnique];
        var indexPrimary = (bool)index[AdoNetConstants.IndexIsPrimary];

        var indexColumnIndexes = string.Join(", ", (short[])index[AdoNetConstants.IndexColumns]);
        var indexColumnNames = await this.GetColumnNames(
          indexColumnIndexes,
          connection,
          schema, tableName,
          services,
          cancellationToken
        ).ConfigureAwait(false);

        resultTable.Rows.Add(indexName, GetIndexType(indexUnique, indexPrimary), indexColumnNames);
      }

      return resultTable;
    }

    private static string GetIndexType(bool unique, bool primary)
      => primary ? "PRIMARY KEY" : unique ? "UNIQUE" : "NON UNIQUE";
  }
}
