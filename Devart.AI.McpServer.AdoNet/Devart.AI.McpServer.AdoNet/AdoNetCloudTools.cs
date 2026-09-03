// --------------------------------------------------------------------------
// <copyright file="AdoNetCloudTools.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Collections.Generic;
using Devart.AI.McpServer.AdoNet.Tools;
using Devart.AI.McpServer.Tools;
using ModelContextProtocol.Server;

namespace Devart.AI.McpServer.AdoNet
{
  public static class AdoNetCloudTools
  {
    public static List<McpServerTool> CreateTools(McpConfiguration configuration)
      => new McpToolSetBuilder(configuration)
        .Add(new ConfigTool(configuration))
        .Add(new AdoNetInstructionsTool(configuration))
        .Add(new TablesTool(configuration))
        .Add(new ColumnsTool(configuration))
        .Add(new IndexesTool(configuration))
        .Add(new PrimaryKeysTool(configuration))
        .Add(new ForeignKeysTool(configuration))
        .Add(new CountRowsTool(configuration))
        .Add(new ExecuteReaderTool(configuration))
        .Add(new ExecuteNonQueryTool(configuration))
        .Add(new AddRowTool(configuration))
        .Build();
  }
}