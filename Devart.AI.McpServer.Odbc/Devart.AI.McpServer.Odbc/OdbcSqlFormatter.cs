// --------------------------------------------------------------------------
// <copyright file="OdbcSqlFormatter.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Common;
using System.Data.Odbc;

namespace Devart.AI.McpServer.Odbc
{
  internal sealed class OdbcSqlFormatter : McpSqlFormatter
  {
    public override string FormatName(string schema, string name, McpConfiguration configuration, DbConnection connection)
    {
      if (string.IsNullOrWhiteSpace(name) || connection is null)
      {
        return base.FormatName(schema, name, configuration, connection);
      }

      using var builder = new OdbcCommandBuilder();
      var fullName = builder.QuoteIdentifier(name, (OdbcConnection)connection);

      if (!string.IsNullOrWhiteSpace(schema))
      {
        fullName = $"{builder.QuoteIdentifier(schema, (OdbcConnection)connection)}.{fullName}";
      }
      return fullName;
    }
  }
}
