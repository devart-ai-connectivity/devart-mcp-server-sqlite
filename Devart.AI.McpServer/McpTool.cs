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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Devart.AI.McpServer
{
  public abstract class McpTool : McpServerTool
  {
    private const string ArgumentsParameterName = "arguments";

    private readonly Lazy<McpServerTool> nativeTool;

    protected McpTool(McpConfiguration serverConfiguration)
    {
      ServerConfiguration = serverConfiguration;
      this.nativeTool = new(() => CreateMcpTool());
    }

    public override Tool ProtocolTool => ServerTool.ProtocolTool;

    public override IReadOnlyList<object> Metadata => ServerTool.Metadata;

    public virtual bool IsApplicable(McpConfiguration configuration) => true;

    protected abstract string Name { get; }

    protected abstract string Description { get; }

    protected abstract Delegate ExecuteDefinition { get; }

    protected McpConfiguration ServerConfiguration { get; }

    private McpServerTool ServerTool => this.nativeTool.Value;

    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
      try
      {
        return await ServerTool.InvokeAsync(request, cancellationToken).ConfigureAwait(false);
      }
      catch (ArgumentException ex) when (ex.ParamName == ArgumentsParameterName)
      {
        return new CallToolResult
        {
          IsError = true,
          Content = [new TextContentBlock { Text = ex.Message }]
        };
      }
    }

    protected static async Task<string> DoActionAsync(Func<Task<string>> action, IServiceProvider services = null)
    {
      try
      {
        return await action();
      }
      catch (Exception ex) when (ex is not McpException)
      {
        services?.GetService<ILoggerFactory>()?.CreateLogger(typeof(McpTool))
          .LogError("{Exception}", ex.ToString());
        throw new McpException($"{ex.GetType().Name}: {ex.Message}", ex);
      }
    }

    protected static DataTable RequireMetadataTable(DataTable table, Func<string> collectionName)
      => table ?? throw new InvalidOperationException(
        string.Format(McpResources.Common_MetadataCollectionNotReturned, collectionName()));

    protected bool IsIgnoredSchema(string schemaName)
      => ServerConfiguration.IgnoreSchemas?.Contains(schemaName?.Trim(), StringComparer.OrdinalIgnoreCase) == true;

    protected virtual async Task<DataTable> GetMetadataTable(
      DbConnection connection,
      string schema,
      string tableName,
      IServiceProvider services,
      CancellationToken cancellationToken
    ) => new();

    protected static DataTable CreateAnswerTable(params string[] columns)
      => CreateAnswerTable([.. columns.Select(column => (column, typeof(string)))]);

    protected static DataTable CreateAnswerTable(params (string Name, Type Type)[] columns)
    {
      var table = new DataTable();
      foreach (var (name, type) in columns)
      {
        table.Columns.Add(name, type);
      }

      return table;
    }

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