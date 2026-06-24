// --------------------------------------------------------------------------
// <copyright file="McpTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Devart.AI.McpServer
{
  public abstract class McpTool : McpServerTool
  {
    private readonly Lazy<McpServerTool> nativeTool;

    protected McpTool(McpConfiguration serverConfiguration)
    {
      ServerConfiguration = serverConfiguration;
      nativeTool = new(() => CreateMcpTool());
    }

    public override Tool ProtocolTool => ServerTool.ProtocolTool;

    public override IReadOnlyList<object> Metadata => ServerTool.Metadata;

    public virtual bool IsApplicable(McpConfiguration configuration) => true;

    protected abstract string Name { get; }

    protected abstract string Description { get; }

    protected abstract Delegate ExecuteDefinition { get; }

    protected McpConfiguration ServerConfiguration { get; }

    private McpServerTool ServerTool => nativeTool.Value;

    public override ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
      => ServerTool.InvokeAsync(request, cancellationToken);

    protected static async Task<string> DoActionAsync(Func<Task<string>> action)
    {
      try
      {
        return await action();
      }
      catch (Exception ex) when (ex is not McpException)
      {
        throw new McpException(ex.Message);
      }
    }

    protected bool IsIgnoredSchema(string schemaName)
      => ServerConfiguration.IgnoreSchemas?.Contains(schemaName?.Trim(), StringComparer.OrdinalIgnoreCase) == true;

    protected virtual async Task<DataTable> GetMetadataTable(
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken
    ) => new();

    private McpServerTool CreateMcpTool()
      => McpServerTool.Create(
        method: ExecuteDefinition,
        options: new McpServerToolCreateOptions
        {
          Name = $"{ServerConfiguration.ToolPrefix}_{Name}",
          Description = Description
        }
      );
  }
}
