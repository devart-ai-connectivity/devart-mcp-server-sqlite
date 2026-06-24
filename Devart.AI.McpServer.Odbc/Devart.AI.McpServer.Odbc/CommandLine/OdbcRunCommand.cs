// --------------------------------------------------------------------------
// <copyright file="OdbcRunCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Devart.AI.McpServer.CommandLine;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Odbc.CommandLine
{
  public abstract class OdbcRunCommand : McpRunCommand
  {
    protected override McpConfiguration CreateConfiguration() => new OdbcConfiguration();

    protected virtual void SetupConnectionBuilder(IHostApplicationBuilder builder)
    {
      builder.Services.AddSingleton<IConnectionBuilder, OdbcConnectionBuilder>();
    }

    protected virtual void SetupMetadata(IHostApplicationBuilder builder)
    {
      builder.Services.AddSingleton<IMetadata, OdbcMetadata>();
    }

    protected sealed override void ConfigureServices(IHostApplicationBuilder builder, McpConfiguration configuration)
    {
      builder.Services
        .AddSingleton<IDatabase, McpDatabase>()
        .AddSingleton<IConfig, OdbcConfig>()
        .AddSingleton<ICommandHelper, OdbcCommandHelper>()
        .AddSingleton<ISqlFormatter, OdbcSqlFormatter>()
        .AddSingleton<ISqlStatementsParser, McpSqlStatementsParser>();

      SetupMetadata(builder);
      SetupConnectionBuilder(builder);
      ContributeServices(builder, configuration);
    }

    protected virtual void ContributeServices(IHostApplicationBuilder builder, McpConfiguration configuration)
    {
    }
  }
}