// --------------------------------------------------------------------------
// <copyright file="McpLifetimeLogger.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Devart.AI.McpServer.Hosting
{
  internal sealed class McpLifetimeLogger(ILogger<McpLifetimeLogger> logger, McpRunContext context) : IHostedService
  {
    private static readonly Action<ILogger, string, Exception> LogStarted =
      LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(1001, nameof(LogStarted)),
        McpResources.CommandLine_LogStartWithConfig);

    private static readonly Action<ILogger, Exception> LogStopped =
      LoggerMessage.Define(
        LogLevel.Information,
        new EventId(1002, nameof(LogStopped)),
        McpResources.CommandLine_LogStopped);

    public Task StartAsync(CancellationToken cancellationToken)
    {
      LogStarted(logger, context.ConfigName, null);
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      LogStopped(logger, null);
      return Task.CompletedTask;
    }
  }
}