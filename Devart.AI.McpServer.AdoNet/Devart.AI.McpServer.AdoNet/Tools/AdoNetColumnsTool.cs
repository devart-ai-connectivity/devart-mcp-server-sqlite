// --------------------------------------------------------------------------
// <copyright file="AdoNetColumnsTool.cs" company="Devart">
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
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.AdoNet.Tools
{
  internal sealed class AdoNetColumnsTool(McpConfiguration serverConfiguration) : ColumnsTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var metadata = services.GetService<IMetadata>();
      return await connection
        .GetSchemaAsync(
          metadata.ColumnsCollectionName,
          [metadata.SchemaName(schema), tableName],
          cancellationToken)
        .ConfigureAwait(false);
    }
  }
}
