// --------------------------------------------------------------------------
// <copyright file="HttpMcpHostRunner.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Shutdown;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Devart.AI.McpServer.Hosting
{
  internal sealed class HttpMcpHostRunner : IMcpHostRunner
  {
    public McpProtocolType ProtocolType => McpProtocolType.Http;

    public LogLevel DefaultLogLevel => LogLevel.Error;

    public async Task<int> RunAsync(
      McpConfiguration configuration,
      Func<IHostApplicationBuilder, McpConfiguration, IMcpServerBuilder> setupServer,
      LogLevel logLevel,
      CancellationToken cancellationToken)
    {
      using var guard = SingleInstanceGuard.TryAcquire(configuration.Id);
      if (!guard.Acquired)
      {
        throw new SingleInstanceAlreadyAcquiredException();
      }

      var builder = WebApplication.CreateSlimBuilder([]);
      McpLoggingSetup.Configure(builder.Logging, logLevel);
      builder.WebHost.UseUrls($"http://{configuration.HttpAddress}:{configuration.HttpPort}");

      setupServer(builder, configuration).WithHttpTransport();

      builder.Services.AddSingleton<McpShutdownService>();

      var app = builder.Build();
      app.MapMcp(configuration.HttpRoutePrefix ?? string.Empty);
      app.MapShutdownEndpoint();
      await app.RunAsync(cancellationToken).ConfigureAwait(false);
      return 0;
    }
  }
}