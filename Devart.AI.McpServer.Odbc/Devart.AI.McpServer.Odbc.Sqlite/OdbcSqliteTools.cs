// --------------------------------------------------------------------------
// <copyright file="OdbcSqliteTools.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Collections.Generic;
using ModelContextProtocol.Server;
using Devart.AI.McpServer.Odbc.Sqlite.Tools;

namespace Devart.AI.McpServer.Odbc.Sqlite
{
  internal static class OdbcSqliteTools
  {
    public static List<McpServerTool> CreateTools(McpConfiguration configuration)
      => OdbcTools.CreateBuilder(configuration)
        .Add(new OdbcSqlitePrimaryKeysTool(configuration))
        .Add(new OdbcSqliteForeignKeysTool(configuration))
        .Build();
  }
}
