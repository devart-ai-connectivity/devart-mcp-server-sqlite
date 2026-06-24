// --------------------------------------------------------------------------
// <copyright file="McpCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Devart.AI.McpServer.Hosting;

namespace Devart.AI.McpServer.CommandLine
{
  public abstract class McpCommand : Command
  {
    public McpCommand(string name, string alias, string description) : base(name, description)
    {
      Aliases.Add(alias);
      SetAction((result, cancellation) => DoActionAsync(result, cancellation));
    }

    protected virtual Task<int> DoActionAsync(ILogger logger, LogLevel logLevel, ParseResult parseResult, CancellationToken cancellationToken)
      => Task.FromResult(0);

    protected virtual async Task<int> DoActionAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
      var logLevel = TryParseVerbosity(parseResult) ?? LogLevel.None;
      using var loggerFactory = LoggerFactory.Create(builder => McpLoggingSetup.Configure(builder, logLevel));
      var logger = loggerFactory.CreateLogger(nameof(McpLauncher));
      return await DoActionAsync(logger, logLevel, parseResult, cancellationToken);
    }

    protected static LogLevel? TryParseVerbosity(ParseResult parseResult)
    {
      string verbosityString = parseResult.GetValue<string>(McpRootCommand.VerbosityOption);
      return verbosityString switch
      {
        "quiet" or "q" => LogLevel.None,
        "minimal" or "m" => LogLevel.Error,
        "normal" or "n" => LogLevel.Information,
        "detailed" or "d" => LogLevel.Debug,
        "diagnostic" or "diag" => LogLevel.Trace,
        _ => null
      };
    }
  }
}
