// --------------------------------------------------------------------------
// <copyright file="McpLoggingFormatter.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Devart.AI.McpServer.Hosting
{
  internal class McpLoggingFormatter : ConsoleFormatter
  {
    public const string FormatterName = "McpNoStackTrace";

    private const string LogLevelPadding = ": ";
    private static readonly string MessagePadding = new(' ', GetLogLevelString(LogLevel.Information).Length + LogLevelPadding.Length);
    private static readonly string NewLineWithMessagePadding = Environment.NewLine + MessagePadding;

    public McpLoggingFormatter() : base(FormatterName) { }

    public override void Write<TState>(
      in LogEntry<TState> logEntry,
      IExternalScopeProvider scopeProvider,
      TextWriter textWriter)
    {
      string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
      if (string.IsNullOrEmpty(message) && logEntry.Exception is null)
      {
        return;
      }

      string logLevelString = GetLogLevelString(logEntry.LogLevel);
      var (foreground, background) = GetLogLevelColors(logEntry.LogLevel);

      WriteColored(textWriter, logLevelString, foreground, background);
      textWriter.Write(LogLevelPadding);
      textWriter.Write(logEntry.Category);
      textWriter.Write('[');
      textWriter.Write(logEntry.EventId.Id);
      textWriter.Write(']');
      textWriter.WriteLine();

      if (!string.IsNullOrEmpty(message))
      {
        textWriter.Write(MessagePadding);
        WriteIndented(textWriter, message);
        textWriter.WriteLine();
      }

      if (logEntry.Exception is not null)
      {
        textWriter.Write(MessagePadding);
        textWriter.Write(logEntry.Exception.GetType().FullName);
        textWriter.Write(": ");
        WriteIndented(textWriter, logEntry.Exception.Message);
        textWriter.WriteLine();
      }
    }

    private static void WriteIndented(TextWriter writer, string message)
    {
      writer.Write(message.Replace(Environment.NewLine, NewLineWithMessagePadding));
    }

    private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
    {
      LogLevel.Trace => "trce",
      LogLevel.Debug => "dbug",
      LogLevel.Information => "info",
      LogLevel.Warning => "warn",
      LogLevel.Error => "fail",
      LogLevel.Critical => "crit",
      _ => "none",
    };

    private static (string Foreground, string Background) GetLogLevelColors(LogLevel logLevel) => logLevel switch
    {
      LogLevel.Trace => ("\x1b[37m", string.Empty),
      LogLevel.Debug => ("\x1b[37m", string.Empty),
      LogLevel.Information => ("\x1b[32m", string.Empty),
      LogLevel.Warning => ("\x1b[33m", string.Empty),
      LogLevel.Error => ("\x1b[30m", "\x1b[41m"),
      LogLevel.Critical => ("\x1b[37m", "\x1b[41m"),
      _ => (string.Empty, string.Empty),
    };

    private static void WriteColored(TextWriter writer, string text, string foreground, string background)
    {
      bool hasBackground = !string.IsNullOrEmpty(background);
      bool hasForeground = !string.IsNullOrEmpty(foreground);

      if (hasBackground)
      {
        writer.Write(background);
      }
      if (hasForeground)
      {
        writer.Write(foreground);
      }
      writer.Write(text);
      if (hasForeground)
      {
        writer.Write("\x1b[39m\x1b[22m");
      }
      if (hasBackground)
      {
        writer.Write("\x1b[49m");
      }
    }
  }
}
