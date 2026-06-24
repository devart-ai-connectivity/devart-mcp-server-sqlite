// --------------------------------------------------------------------------
// <copyright file="McpLoggingSetup.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Devart.AI.McpServer.Hosting
{
  internal static class McpLoggingSetup
  {
    public static void Configure(ILoggingBuilder logging, LogLevel logLevel, LogLevel stderrThreshold = LogLevel.Error)
    {
      logging.ClearProviders();
      logging.AddConsole(options =>
      {
        options.FormatterName = McpLoggingFormatter.FormatterName;
        options.LogToStandardErrorThreshold = stderrThreshold;
      });
      logging.AddConsoleFormatter<McpLoggingFormatter, ConsoleFormatterOptions>();
      logging.SetMinimumLevel(logLevel);
      logging.AddFilter("Default", logLevel);
    }
  }
}
