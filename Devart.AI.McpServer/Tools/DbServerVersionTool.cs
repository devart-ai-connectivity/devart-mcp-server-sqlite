// --------------------------------------------------------------------------
// <copyright file="DbServerVersionTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public class DbServerVersionTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "server_version";

    protected override string Description => string.Format(McpResources.ServerVersionTool_Description, ServerConfiguration.SourceName);

    protected override Delegate ExecuteDefinition => Execute;

    public override bool IsApplicable(McpConfiguration configuration) => configuration.OnPremise;

    public Task<string> Execute(
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(services, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);

      return string.Format(McpResources.ServerVersionTool_OutputHeader, configuration.SourceName, connection.ServerVersion);
    }
  }
}
