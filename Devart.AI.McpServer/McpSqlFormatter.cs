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

    public virtual string FormatParameterName(int index) => $"p{index}";

    public virtual string FormatParameterPlaceholder(int index) => $":{FormatParameterName(index)}";

    public virtual string FormatCallProcedure(string procedureFullName, int parametersCount)
      => $"CALL {procedureFullName}({FormatPlaceholders(parametersCount)})";

    public virtual string FormatCallFunction(string functionFullName, int parametersCount)
      => $"SELECT {functionFullName}({FormatPlaceholders(parametersCount)})";

    protected string FormatPlaceholders(int parametersCount)
      => string.Join(", ", Enumerable.Range(0, parametersCount).Select(FormatParameterPlaceholder));
  }
}