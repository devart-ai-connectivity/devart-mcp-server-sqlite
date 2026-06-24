// --------------------------------------------------------------------------
// <copyright file="McpSqlFormatter.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data.Common;
using System.Linq;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer
{
  public class McpSqlFormatter : ISqlFormatter
  {
    public virtual string FormatName(string schema, string name, McpConfiguration configuration, DbConnection connection)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException(McpResources.SqlFormatter_NameNullOrEmpty, nameof(name));
      }

      var fullName = $"{configuration.OpenQuote}{name}{configuration.CloseQuote}";
      if (!string.IsNullOrWhiteSpace(schema))
      {
        fullName = $"{configuration.OpenQuote}{schema}{configuration.CloseQuote}.{fullName}";
      }
      return fullName;
    }

    public string FormatCallProcedure(string procedureFullName, int parametersCount)
      => $"{{CALL {procedureFullName}({GetPlaceholders(parametersCount)})}}";

    public string FormatCallFunction(string functionFullName, int parametersCount)
      => $"{{? = CALL {functionFullName}({GetPlaceholders(parametersCount)})}}";

    private static string GetPlaceholders(int parametersCount)
      => string.Join(", ", Enumerable.Repeat("?", parametersCount));
  }
}
