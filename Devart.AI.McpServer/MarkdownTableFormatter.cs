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
      MetadataColumn[] columnsMapping = null,
      Predicate<DataRow> skipPredicate = null)
    {
      if (table is null)
      {
        return string.Empty;
      }
      if (table.Columns.Count == 0)
      {
        return string.Empty;
      }
      if (table.Rows.Count == 0)
      {
        return McpResources.Common_NoDataAvailable;
      }

      columnsMapping ??= [.. table.Columns.Cast<DataColumn>().Select(c => new MetadataColumn(c.ColumnName, c.ColumnName))];

      var columns = ResolveColumns(columnsMapping, table.Columns.Contains, out var missingColumns);
      if (missingColumns.Length > 0)
      {
        throw new InvalidOperationException(string.Format(
          McpResources.Common_MetadataColumnsMissing,
          string.Join(", ", missingColumns),
          table.TableName));
      }
      if (columns.Length == 0)
      {
        return McpResources.Common_NoDataAvailable;
      }

      var result = new StringBuilder(columns.Length * 10 * table.Rows.Count + 1);

      foreach (DataRow row in table.Rows)
      {
        if (skipPredicate?.Invoke(row) == true)
        {
          continue;
        }

        if (result.Length == 0)
        {
          FormatTableHeader(columns.Select(c => c.Alias), result);
        }

        result.AppendLine();
        FormatTableRow(columns.Select(column => FormatTableValue(row[column.Name])), result);
      }

      return result.Length == 0 ? McpResources.Common_NoDataAvailable : result.ToString();
    }

    public static async Task<string> FormatDataReaderAsync(
      DbDataReader reader,
      MetadataColumn[] columnsMapping = null,
      Predicate<object[]> skipPredicate = null,
      CancellationToken cancellationToken = default)
    {
      if (!reader.HasRows)
      {
        return McpResources.Common_NoDataAvailable;
      }
      columnsMapping ??= [.. Enumerable.Range(0, reader.FieldCount).Select(i => new MetadataColumn(reader.GetName(i), reader.GetName(i)))];

      var fieldNames = new HashSet<string>(
        Enumerable.Range(0, reader.FieldCount).Select(reader.GetName),
        StringComparer.OrdinalIgnoreCase);

      var columns = ResolveColumns(columnsMapping, fieldNames.Contains, out var missingColumns);
      if (missingColumns.Length > 0)
      {
        throw new InvalidOperationException(string.Format(
          McpResources.Common_ResultColumnsMissing,
          string.Join(", ", missingColumns)));
      }
      if (columns.Length == 0)
      {
        return McpResources.Common_NoDataAvailable;
      }

      var result = new StringBuilder(columns.Length * 10 * 5);

      FormatTableHeader(columns.Select(c => c.Alias), result);

      object[] row = new object[columns.Length];
      while (await reader.ReadAsync(cancellationToken))
      {
        for (int i = 0; i < row.Length; i++)
        {
          row[i] = reader[columns[i].Name];
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

    private static MetadataColumn[] ResolveColumns(
      MetadataColumn[] columnsMapping,
      Func<string, bool> columnExists,
      out string[] missingColumns)
    {
      missingColumns = [.. columnsMapping
        .Where(column => column.Required && !columnExists(column.Name))
        .Select(column => column.Name)];

      return [.. columnsMapping.Where(column => columnExists(column.Name))];
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