// --------------------------------------------------------------------------
// <copyright file="IndexesTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public class IndexesTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "get_indexes";

    protected override string Description => string.Format(McpResources.IndexesTool_Description, ServerConfiguration.SourceDisplayName);

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      [Description("Name of the schema.")]
      string schema,
      [Description("Name of the table.")]
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(schema, tableName, services, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();
      var metadata = services.GetRequiredService<IMetadata>();
      var formatter = services.GetRequiredService<ISqlFormatter>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      using var table = await database.ExecuteOnConnectionAsync(
        connection,
        () => GetMetadataTable(connection, schema, tableName, services, cancellationToken)
      ).ConfigureAwait(false);

      var markdownTable = table?.ToMarkdown(metadata.IndexesColumnsMapping);
      var quotedFullName = formatter.FormatName(schema, tableName, configuration, connection);
      var header = string.Format(McpResources.IndexesTool_OutputHeader, quotedFullName);

      return $"{header}{Environment.NewLine}{markdownTable}";
    }
  }
}