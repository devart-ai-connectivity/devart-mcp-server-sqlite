// --------------------------------------------------------------------------
// <copyright file="AdoNetTools.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Collections.Generic;
using ModelContextProtocol.Server;
using Devart.AI.McpServer.Tools;
using Devart.AI.McpServer.AdoNet.Tools;

namespace Devart.AI.McpServer.AdoNet
{
  public static class AdoNetTools
  {
    public static List<McpServerTool> CreateTools(McpConfiguration configuration)
      => new McpToolSetBuilder(configuration)
        .Add(new ConfigTool(configuration))
        .Add(new AdoNetInstructionsTool(configuration))
        .Add(new TablesTool(configuration))
        .Add(new AdoNetColumnsTool(configuration))
        .Add(new AdoNetIndexesTool(configuration))
        .Add(new AdoNetForeignKeysTool(configuration))
        .Add(new AdoNetRoutinesTool(configuration))
        .Add(new CountRowsTool(configuration))
        .Add(new ExecuteReaderTool(configuration))
        .Add(new ExecuteNonQueryTool(configuration))
        .Build();
  }
}
