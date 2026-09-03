// --------------------------------------------------------------------------
// <copyright file="RoutinesTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.Tools
{
  public class RoutinesTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "get_routines";

    public override bool IsApplicable(McpConfiguration configuration)
      => configuration.SupportedRoutines is { Count: > 0 };

    protected override string Description => string.Format(McpResources.RoutinesTool_Description, ServerConfiguration.SourceDisplayName);

    protected override Delegate ExecuteDefinition => Execute;

    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var metadata = services.GetRequiredService<IMetadata>();
      return await connection
        .GetSchemaAsync(metadata.RoutinesCollectionName, cancellationToken)
        .ConfigureAwait(false);
    }

    public Task<string> Execute(
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(services, cancellationToken), services);

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

      var markdownTable = RequireMetadataTable(table, () => metadata.RoutinesCollectionName).ToMarkdown(metadata.RoutinesColumnsMapping);

      return $"{McpResources.RoutinesTool_OutputHeader}{Environment.NewLine}{markdownTable}";
    }
  }
}