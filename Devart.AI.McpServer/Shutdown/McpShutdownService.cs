// --------------------------------------------------------------------------
// <copyright file="McpShutdownService.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Devart.AI.McpServer.Shutdown
{
  internal sealed class McpShutdownService(IHostApplicationLifetime lifetime, ILogger<McpShutdownService> logger, McpConfiguration config) {
    private static readonly Action<ILogger, Exception> LogShutdownInitiated =
      LoggerMessage.Define(
        LogLevel.Information,
        new EventId(1004, nameof(LogShutdownInitiated)),
        "Shutdown initiated via the 'admin/shutdown' endpoint");

    private readonly IHostApplicationLifetime _lifetime = lifetime;
    private readonly ILogger<McpShutdownService> _logger = logger;
    private readonly string _shutdownToken = config.Id.ToString();

    public bool ValidateToken(string token)
      => CryptographicOperations.FixedTimeEquals(
        Encoding.UTF8.GetBytes(token),
        Encoding.UTF8.GetBytes(_shutdownToken));

    public void Shutdown()
    {
      LogShutdownInitiated(_logger, null);
      _lifetime.StopApplication();
    }
  }
}
