// --------------------------------------------------------------------------
// <copyright file="DbDataReaderExtensions.cs" company="Devart">
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

namespace Devart.AI.McpServer.Extensions
{
  public static class DbDataReaderExtensions
  {
    public static Task<string> ToMarkdownAsync(
      this DbDataReader reader,
      CancellationToken cancellationToken = default)
      => MarkdownTableFormatter.FormatDataReaderAsync(reader, null, null, cancellationToken);

    public static Task<string> ToMarkdownAsync(
      this DbDataReader reader,
      (string name, string alias)[] columnsMapping = null,
      Predicate<object[]> skipPredicate = null,
      CancellationToken cancellationToken = default)
      => MarkdownTableFormatter.FormatDataReaderAsync(reader, columnsMapping, skipPredicate, cancellationToken);

    public static async Task<DataTable> ToDataTableAsync(this DbDataReader reader, string tableName, CancellationToken cancellationToken)
    {
      DataTable resultTable = NewDataTableFromReader(reader, out object[] values, tableName);

      while (await reader.ReadAsync(cancellationToken))
      {
        reader.GetValues(values);
        resultTable.Rows.Add(values);
      }

      return resultTable;
    }

    private static DataTable NewDataTableFromReader(DbDataReader reader, out object[] values, string tableName)
    {
      DataTable resultTable = new(tableName) {
        Locale = System.Globalization.CultureInfo.InvariantCulture
      };
      DataTable schemaTable = reader.GetSchemaTable()!;
      foreach (DataRow row in schemaTable.Rows)
      {
        resultTable.Columns.Add(row["ColumnName"] as string, (Type)row["DataType"]);
      }

      values = new object[resultTable.Columns.Count];
      return resultTable;
    }
  }
}
