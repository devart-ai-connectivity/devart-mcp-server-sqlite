// --------------------------------------------------------------------------
// <copyright file="OdbcTools.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.Tools;
using Devart.AI.McpServer.Odbc.Tools;

namespace Devart.AI.McpServer.Odbc
{
  public static class OdbcTools
  {
    public static McpToolSetBuilder CreateBuilder(McpConfiguration configuration)
      => new McpToolSetBuilder(configuration)
        .Add(new ConfigTool(configuration))
        .Add(new OdbcInstructionsTool(configuration))
        .Add(new DbServerVersionTool(configuration))
        .Add(new TablesTool(configuration))
        .Add(new ColumnsTool(configuration))
        .Add(new OdbcIndexesTool(configuration))
        .Add(new OdbcRoutinesTool(configuration))
        .Add(new CountRowsTool(configuration))
        .Add(new ExecuteReaderTool(configuration))
        .Add(new ExecuteNonQueryTool(configuration))
        .Add(new ExecuteRoutineTool(configuration))
        .Add(new AddRowTool(configuration));
  }
}
