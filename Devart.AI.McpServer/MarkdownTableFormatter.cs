// --------------------------------------------------------------------------
// <copyright file="MarkdownTableFormatter.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.AI.McpServer
{
  internal static class MarkdownTableFormatter
  {

    private static readonly string TableLeftBorder = "| ",
      TableCenterBorder = " | ",
      TableRightBorder = " |",
      TableBorder = "|",
      TableHeader = "---",
      NullValue = "";

    public static string FormatDataTable(DataTable table,
      (string name, string alias)[] columnsMapping = null,
      Predicate<DataRow> skipPredicate = null)
    {
      if (table.Columns.Count == 0)
      {
        return string.Empty;
      }
      if (table.Rows.Count == 0)
      {
        return McpResources.Common_NoDataAvailable;
      }

      var result = new StringBuilder(table.Columns.Count * 10 * table.Rows.Count + 1);

      columnsMapping ??= [.. table.Columns.Cast<DataColumn>().Select(c => (c.ColumnName, c.ColumnName))];

      foreach (DataRow row in table.Rows)
      {
        if (skipPredicate?.Invoke(row) == true)
        {
          continue;
        }

        if (result.Length == 0)
        {
          FormatTableHeader(columnsMapping.Select(c => c.alias), result);
        }

        result.AppendLine();
        FormatTableRow(columnsMapping.Select(column => FormatTableValue(row[column.name])), result);
      }

      return result.Length == 0 ? McpResources.Common_NoDataAvailable : result.ToString();
    }

    public static async Task<string> FormatDataReaderAsync(
      DbDataReader reader,
      (string name, string alias)[] columnsMapping = null,
      Predicate<object[]> skipPredicate = null,
      CancellationToken cancellationToken = default)
    {
      if (!reader.HasRows)
      {
        return McpResources.Common_NoDataAvailable;
      }
      var result = new StringBuilder(reader.FieldCount * 10 * 5);
      columnsMapping ??= [.. Enumerable.Range(0, reader.FieldCount).Select(i => (reader.GetName(i), reader.GetName(i)))];

      FormatTableHeader(columnsMapping.Select(c => c.alias), result);

      object[] row = new object[columnsMapping.Length];
      while (await reader.ReadAsync(cancellationToken))
      {
        for (int i = 0; i < row.Length; i++)
        {
          row[i] = reader[columnsMapping[i].name];
        }
        if (skipPredicate?.Invoke(row) == true)
        {
          continue;
        }

        result.AppendLine();
        FormatTableRow(row, result);
      }
      return result.ToString();
    }

    public static string FormatTableValue(object value) => value == null || value == DBNull.Value
      ? NullValue
      : Convert.ToString(value, CultureInfo.InvariantCulture);

    private static void FormatTableHeader(IEnumerable<string> columns, StringBuilder builder)
    {
      var headerBorder = new StringBuilder();
      builder.Append(TableLeftBorder);
      headerBorder.Append(TableBorder);
      foreach (var column in columns)
      {
        if (headerBorder.Length > 1)
        {
          builder.Append(TableCenterBorder);
          headerBorder.Append(TableBorder);
        }
        builder.Append(column);
        headerBorder.Append(TableHeader);
      }
      builder.AppendLine(TableRightBorder);
      headerBorder.Append(TableBorder);
      builder.Append(headerBorder);
    }

    private static void FormatTableRow(IEnumerable<object> values, StringBuilder builder)
    {
      builder.Append(TableLeftBorder);
      int count = 0;
      foreach (var value in values)
      {
        if (count > 0)
        {
          builder.Append(TableCenterBorder);
        }
        builder.Append(FormatTableValue(value));
        count++;
      }
      builder.Append(TableRightBorder);
    }
  }
}
