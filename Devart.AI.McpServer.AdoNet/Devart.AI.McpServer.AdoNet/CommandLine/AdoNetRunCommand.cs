// --------------------------------------------------------------------------
// <copyright file="AdoNetRunCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.CommandLine;
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Devart.AI.McpServer.AdoNet.CommandLine
{
  public abstract class AdoNetRunCommand : McpRunCommand
  {
    protected abstract void SetupConnectionBuilder(IHostApplicationBuilder builder);

    protected virtual void SetupMetadata(IHostApplicationBuilder builder) => builder.Services.AddSingleton<IMetadata, AdoNetMetadata>();

    protected virtual void SetupSqlFormatter(IHostApplicationBuilder builder) => builder.Services.AddSingleton<ISqlFormatter, McpSqlFormatter>();

    protected sealed override void ConfigureServices(IHostApplicationBuilder builder, McpConfiguration configuration)
    {
      builder.Services
        .AddSingleton<IDatabase, McpDatabase>()
        .AddSingleton<IConfig, AdoNetConfig>()
        .AddSingleton<ICommandHelper, AdoNetCommandHelper>()
        .AddSingleton<ISqlStatementsParser, McpSqlStatementsParser>();

      SetupSqlFormatter(builder);
      SetupMetadata(builder);
      SetupConnectionBuilder(builder);
      ContributeServices(builder, configuration);
    }

    protected virtual void ContributeServices(IHostApplicationBuilder builder, McpConfiguration configuration) { }
  }
}