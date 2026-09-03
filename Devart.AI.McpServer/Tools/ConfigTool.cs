// --------------------------------------------------------------------------
// <copyright file="ConfigTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.Tools
{
  public class ConfigTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "config";

    protected override string Description => string.Format(McpResources.ConfigTool_Description, ServerConfiguration.ServerName);

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(services, cancellationToken), services);

    protected virtual async Task<string> ExecuteAsync(
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var config = services.GetService<IConfig>();
      config?.Execute();
      return await Task.FromResult("");
    }
  }
}