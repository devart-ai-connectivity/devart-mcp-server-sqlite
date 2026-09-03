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
    public McpConfigCommand(string productFullName) : this(productFullName, McpResources.CommandLine_CommandConfigMcp)
    {
    }

    public McpConfigCommand(string productFullName, string description) : base("config", "-c", description)
    {
      Subcommands.Add(new McpConfigListCommand(productFullName));
      Subcommands.Add(new McpConfigAddCommand(productFullName));
      Subcommands.Add(new McpConfigEditCommand(productFullName));
      Subcommands.Add(new McpConfigDeleteCommand(productFullName));
    }

    protected abstract int ExecuteConfig();

    protected override Task<int> DoActionAsync(ParseResult parseResult, CancellationToken cancellationToken)
      => Task.FromResult(ExecuteConfig());
  }
}