// --------------------------------------------------------------------------
// <copyright file="McpToolSetBuilder.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ModelContextProtocol.Server;

namespace Devart.AI.McpServer
{
  public sealed class McpToolSetBuilder(McpConfiguration config)
  {
    private readonly McpConfiguration configuration = config ?? throw new ArgumentNullException(nameof(config));

    private readonly List<McpServerTool> toolList = [];

    public McpToolSetBuilder Add(McpServerTool tool)
    {
      ArgumentNullException.ThrowIfNull(tool);
      this.toolList.Add(tool);
      return this;
    }

    public McpToolSetBuilder AddRange(IEnumerable<McpServerTool> tools)
    {
      ArgumentNullException.ThrowIfNull(tools);
      foreach (var tool in tools)
      {
        Add(tool);
      }
      return this;
    }

    public McpToolSetBuilder When(bool condition, Action<McpToolSetBuilder> configure)
    {
      ArgumentNullException.ThrowIfNull(configure);
      if (condition)
      {
        configure(this);
      }
      return this;
    }

    public List<McpServerTool> Build()
      => [.. this.toolList.Where(tool => tool is not McpTool mcpTool || mcpTool.IsApplicable(this.configuration))];
  }
}