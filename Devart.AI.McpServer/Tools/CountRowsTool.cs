// --------------------------------------------------------------------------
// <copyright file="CountRowsTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public class CountRowsTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "count_rows";

    protected override string Description => string.Format(McpResources.CountRowsTool_Description, ServerConfiguration.SourceDisplayName);

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
      var formatter = services.GetRequiredService<ISqlFormatter>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      var quotedFullName = formatter.FormatName(schema, tableName, configuration, connection);
      var sql = $"SELECT '{quotedFullName}' AS NAME, COUNT(*) AS COUNT FROM {quotedFullName}";

      await using var reader = await database.ExecuteReaderAsync(connection, sql, cancellationToken: cancellationToken).ConfigureAwait(false);

      (string name, string alias)[] columns =
      [
        ("NAME", McpResources.CountRowsTool_TableName),
        ("COUNT", McpResources.CountRowsTool_RowsCount)
      ];

      return await database.ExecuteOnConnectionAsync(
        connection,
        () => reader.ToMarkdownAsync(columns, cancellationToken: cancellationToken)
      ).ConfigureAwait(false);
    }
  }
}
