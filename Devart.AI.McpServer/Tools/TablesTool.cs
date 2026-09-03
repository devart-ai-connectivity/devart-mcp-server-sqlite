// --------------------------------------------------------------------------
// <copyright file="TablesTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.Tools
{
  public class TablesTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "get_tables";

    protected override string Description => string.Format(McpResources.TablesTool_Description, ServerConfiguration.SourceDisplayName);

    protected override Delegate ExecuteDefinition => Execute;

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
      using var tables = await database.ExecuteOnConnectionAsync(
        connection,
        () => connection.GetSchemaAsync(
          metadata.TablesCollectionName,
          metadata.TablesRestrictions(connection.Database),
          cancellationToken)
      ).ConfigureAwait(false);

      var markdownTable = RequireMetadataTable(tables, () => metadata.TablesCollectionName).ToMarkdown(
        metadata.TablesColumnsMapping,
        CreateIgnoredSchemaPredicate(tables, metadata.TablesSchemaName)
      );

      return $"{McpResources.TablesTool_OutputHeader}{Environment.NewLine}{markdownTable}";
    }

    private Predicate<DataRow> CreateIgnoredSchemaPredicate(DataTable tables, string schemaColumnName)
      => ServerConfiguration.IgnoreSchemas is not { Count: > 0 }
        || string.IsNullOrEmpty(schemaColumnName)
        || !tables.Columns.Contains(schemaColumnName)
        ? null
        : (row => IsIgnoredSchema(row[schemaColumnName]?.ToString()));
  }
}