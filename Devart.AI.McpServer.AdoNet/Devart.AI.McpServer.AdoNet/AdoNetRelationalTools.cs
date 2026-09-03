// --------------------------------------------------------------------------
// <copyright file="AdoNetRelationalTools.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.AdoNet.Tools;
using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.AdoNet
{
  public static class AdoNetRelationalTools
  {
    public static McpToolSetBuilder CreateBuilder(McpConfiguration configuration)
      => new McpToolSetBuilder(configuration)
        .Add(new ConfigTool(configuration))
        .Add(new AdoNetInstructionsTool(configuration))
        .Add(new DbServerVersionTool(configuration))
        .Add(new TablesTool(configuration))
        .Add(new ColumnsTool(configuration))
        .Add(new IndexesTool(configuration))
        .Add(new ForeignKeysTool(configuration))
        .Add(new CountRowsTool(configuration))
        .Add(new ExecuteReaderTool(configuration))
        .Add(new ExecuteNonQueryTool(configuration))
        .Add(new ExecuteRoutineTool(configuration))
        .Add(new AddRowTool(configuration));
  }
}