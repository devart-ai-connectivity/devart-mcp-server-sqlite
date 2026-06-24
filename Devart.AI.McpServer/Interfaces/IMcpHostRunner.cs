// --------------------------------------------------------------------------
// <copyright file="IMcpHostRunner.cs" company="Devart">
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

namespace Devart.AI.McpServer.Interfaces
{
  public interface IMcpHostRunner
  {
    McpProtocolType ProtocolType { get; }

    LogLevel DefaultLogLevel { get; }

    Task<int> RunAsync(
      McpConfiguration configuration,
      Func<IHostApplicationBuilder, McpConfiguration, IMcpServerBuilder> setupServer,
      LogLevel logLevel,
      CancellationToken cancellationToken);
  }
}
