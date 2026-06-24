// --------------------------------------------------------------------------
// <copyright file="StdioMcpHostRunner.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Hosting
{
  internal sealed class StdioMcpHostRunner : IMcpHostRunner
  {
    public McpProtocolType ProtocolType => McpProtocolType.Stdio;

    public LogLevel DefaultLogLevel => LogLevel.None;

    public async Task<int> RunAsync(
      McpConfiguration configuration,
      Func<IHostApplicationBuilder, McpConfiguration, IMcpServerBuilder> setupServer,
      LogLevel logLevel,
      CancellationToken cancellationToken)
    {
      var builder = Host.CreateEmptyApplicationBuilder(settings: null);
      McpLoggingSetup.Configure(builder.Logging, logLevel, stderrThreshold: LogLevel.Trace);

      setupServer(builder, configuration).WithStdioServerTransport();
      await builder.Build().RunAsync(cancellationToken).ConfigureAwait(false);
      return 0;
    }
  }
}
