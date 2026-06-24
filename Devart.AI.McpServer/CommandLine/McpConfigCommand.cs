// --------------------------------------------------------------------------
// <copyright file="McpConfigCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.AI.McpServer.CommandLine
{
  public abstract class McpConfigCommand : McpCommand
  {
    public McpConfigCommand() : base("config", "-c", McpResources.CommandLine_CommandConfigMcp)
    {
    }

    protected abstract int ExecuteConfig();

    protected override Task<int> DoActionAsync(ParseResult parseResult, CancellationToken cancellationToken)
      => Task.FromResult(ExecuteConfig());
  }
}
