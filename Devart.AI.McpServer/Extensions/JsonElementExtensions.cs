// --------------------------------------------------------------------------
// <copyright file="JsonElementExtensions.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.Json;
using ModelContextProtocol;

namespace Devart.AI.McpServer.Extensions
{
  internal static class JsonElementExtensions
  {
    public static object ConvertToClrType(this JsonElement element)
    {
      switch (element.ValueKind)
      {
        case JsonValueKind.Null:
        case JsonValueKind.Undefined:
          return DBNull.Value;

        case JsonValueKind.True:
          return true;

        case JsonValueKind.False:
          return false;

        case JsonValueKind.Number:
          if (element.TryGetInt32(out var int32Value))
          {
            return int32Value;
          }
          if (element.TryGetInt64(out var int64Value))
          {
            return int64Value;
          }
          if (element.TryGetDecimal(out var decimalValue))
          {
            return decimalValue;
          }
          return element.GetDouble();

        case JsonValueKind.String:
          var stringValue = element.GetString();
          if (string.IsNullOrEmpty(stringValue))
          {
            return stringValue ?? string.Empty;
          }
          if (DateTimeOffset.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTimeOffset))
          {
            return dateTimeOffset;
          }
          if (DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dataTime))
          {
            return dataTime;
          }
          if (TimeSpan.TryParse(stringValue, CultureInfo.InvariantCulture, out var timeSpan))
          {
            return timeSpan;
          }
          if (Guid.TryParse(stringValue, out var guid))
          {
            return guid;
          }
          return stringValue;

        default:
          throw new McpProtocolException(
            string.Format(McpResources.JsonElementExtensions_UnsupportedJsonTypeError, element.ValueKind),
            McpErrorCode.InvalidParams);
      }
    }
  }
}
