// --------------------------------------------------------------------------
// <copyright file="AdoNetToolExtensions.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.AdoNet.Tools
{
  internal static class AdoNetToolExtensions
  {
    public static async Task<string> GetColumnNames(
      this McpTool _,
      string columnIndexes,
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken)
    {
      List<string> indexList = [.. columnIndexes
        .Trim('{', '}')
        .Split(',')
        .Select(x => x.Trim())];

      var metadata = services.GetService<IMetadata>();
      var columns = await connection.GetSchemaAsync(
        metadata.ColumnsCollectionName,
        [metadata.SchemaName(schema), tableName],
        cancellationToken
      ).ConfigureAwait(false);

      List<string> columnNames = [];
      foreach (DataRow column in columns.Rows)
      {
        var columnName = column[AdoNetConstants.OrdinalPosition]?.ToString();
        if (indexList.Contains(columnName))
        {
          columnNames.Add(column[AdoNetConstants.ColumnName]?.ToString());
        }
      }

      return string.Join(",", columnNames);
    }
  }
}
