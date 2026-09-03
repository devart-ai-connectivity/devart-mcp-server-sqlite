// --------------------------------------------------------------------------
// <copyright file="AdoNetSqliteRunCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.AdoNet.CommandLine;
using Devart.AI.McpServer.AdoNet.Sqlite.Properties;
using Devart.AI.McpServer.AdoNet.Sqlite.Tools;
using Devart.AI.McpServer.AdoNet.Tools;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Devart.AI.McpServer.AdoNet.Sqlite.CommandLine
{
  internal sealed class AdoNetSqliteRunCommand : AdoNetRunCommand
  {
    protected override void SetupConnectionBuilder(IHostApplicationBuilder builder) => builder.Services.AddSingleton<IConnectionBuilder, AdoNetSqliteConnectionBuilder>();

    protected override void SetupMetadata(IHostApplicationBuilder builder) => builder.Services.AddSingleton<IMetadata, AdoNetSqliteMetadata>();

    protected override void RegisterTools(IMcpServerBuilder serverBuilder, McpConfiguration configuration)
      => serverBuilder.WithTools(
        AdoNetRelationalTools.CreateBuilder(configuration)
        .Add(new AdoNetSqlitePrimaryKeysTool(configuration))
          .Build());

    public override string ProductFullName => ProductInfo.ProductFullName;

    public override string ProductId => ProductInfo.ProductId;

    protected override McpAppSettings CreateAppSettings() => new AdoNetSqliteAppSettings();
  }
}