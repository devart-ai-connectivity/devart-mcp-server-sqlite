// --------------------------------------------------------------------------
// <copyright file="InstructionsTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public abstract class InstructionsTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    protected abstract string InstructionsResourceName{ get; }

    protected override string Name => "instructions";

    protected override string Description => string.Format(McpResources.InstructionsTool_Description, ServerConfiguration.ServerName);

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(services, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var tools = services.GetServices<McpServerTool>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var assembly = GetType().Assembly;
      using var stream = assembly.GetManifestResourceStream(InstructionsResourceName)
        ?? throw new InvalidOperationException(string.Format(McpResources.InstructionsTool_ResourceNotFound, InstructionsResourceName));
      using var reader = new StreamReader(stream, Encoding.UTF8);
      string commonInstructions = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);

      var result = new StringBuilder(commonInstructions);
      foreach (var tool in tools)
      {
        result.AppendLine($"* `{tool.ProtocolTool.Name}` - {tool.ProtocolTool.Description}");
      }

      var formatter = services.GetService<ISqlFormatter>();
      result
        .AppendLine()
        .AppendLine(McpResources.InstructionsTool_SqlEscapingInstructions)
        .Append($"INSERT INTO {formatter.FormatName(null, "table name", configuration, null)}")
        .Append($"({formatter.FormatName(null, "column name", configuration, null)})")
        .AppendLine(" VALUES ('test value');");
      return result.ToString();
    }
  }
}
