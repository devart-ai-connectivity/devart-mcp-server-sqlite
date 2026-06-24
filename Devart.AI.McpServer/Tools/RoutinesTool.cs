// --------------------------------------------------------------------------
// <copyright file="RoutinesTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public abstract class RoutinesTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "get_routines";

    protected override string Description => string.Format(McpResources.RoutinesTool_Description, ServerConfiguration.SourceDisplayName);

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(services, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();
      var metadata = services.GetRequiredService<IMetadata>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      using var table = await database.ExecuteOnConnectionAsync(
        connection,
        () => GetMetadataTable(connection, "", "", services, cancellationToken)
      ).ConfigureAwait(false);

      var markdownTable = table?.ToMarkdown(metadata.RoutinesColumnsMapping);

      return $"{McpResources.RoutinesTool_OutputHeader}{Environment.NewLine}{markdownTable}";
    }
  }
}
