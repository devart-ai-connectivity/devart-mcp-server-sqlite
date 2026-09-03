// --------------------------------------------------------------------------
// <copyright file="DataRowExtensions.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Globalization;

namespace Devart.AI.McpServer.Extensions
{
  public static class DataRowExtensions
  {
    public static object TryGet(this DataRow row, string columnName)
      => row != null
        && !string.IsNullOrEmpty(columnName)
        && row.Table.Columns.Contains(columnName)
        && row[columnName] != DBNull.Value
        ? row[columnName]
        : null;

    public static string TryGetString(this DataRow row, string columnName)
    {
      var value = row.TryGet(columnName);
      return value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    public static T TryGetValue<T>(this DataRow row, string columnName, T defaultValue = default)
      => row.TryGet(columnName) is T value ? value : defaultValue;
  }
}