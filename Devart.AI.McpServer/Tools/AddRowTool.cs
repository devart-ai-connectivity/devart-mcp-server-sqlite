// --------------------------------------------------------------------------
// <copyright file="AddRowTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.Tools
{
  public class AddRowTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected override string Name => "add_row";

    protected override string Description => string.Format(McpResources.AddRowTool_Description, ServerConfiguration.SourceDisplayName);

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      [Description("Name of the schema.")]
      string schema,
      [Description("Name of the table.")]
      string tableName,
      [Description("Dictionary of column names and their values.")]
      Dictionary<string, object> parameters,
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(schema, tableName, parameters, services, cancellationToken), services);

    protected virtual async Task<string> ExecuteAsync(
      string schema,
      string tableName,
      Dictionary<string, object> parameters,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();
      var formatter = services.GetRequiredService<ISqlFormatter>();
      var commandHelper = services.GetRequiredService<ICommandHelper>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      await using var command = connection.CreateCommand();

      var tableFullName = formatter.FormatName(schema, tableName, configuration, connection);
      var orderedParams = parameters.OrderBy(p => p.Key).ToArray();
      var columns = string.Join(", ", orderedParams.Select(p => formatter.FormatName(null, p.Key, configuration, connection)));
      var placeholders = string.Join(", ", Enumerable.Range(0, orderedParams.Length).Select(formatter.FormatParameterPlaceholder));

      command.CommandText =
$"""
INSERT INTO {tableFullName} ({columns})
VALUES ({placeholders})
""";

      foreach (var param in orderedParams)
      {
        var value = database.NormalizeParameterValue(param.Value);
        commandHelper.AddParameter(command, value);
      }

      var rowsAffected = await database.ExecuteOnConnectionAsync(
        connection,
        () => command.ExecuteNonQueryAsync(cancellationToken)
      ).ConfigureAwait(false);

      return string.Format(McpResources.AddRowTool_SuccessMessage, rowsAffected);
    }
  }
}