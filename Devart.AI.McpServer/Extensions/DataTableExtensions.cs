// --------------------------------------------------------------------------
// <copyright file="DataTableExtensions.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Linq;

namespace Devart.AI.McpServer.Extensions
{
  internal static class DataTableExtensions
  {
    public static string ToMarkdown(this DataTable table,
      string[] columns = null,
      Predicate<DataRow> skipPredicate = null)
      => ToMarkdown(table, columns?.Select(c => (c, c)).ToArray(), skipPredicate);

    public static string ToMarkdown(this DataTable table,
      (string name, string alias)[] columnsMapping = null,
      Predicate<DataRow> skipPredicate = null)
      => MarkdownTableFormatter.FormatDataTable(table, columnsMapping, skipPredicate);
  }
}
