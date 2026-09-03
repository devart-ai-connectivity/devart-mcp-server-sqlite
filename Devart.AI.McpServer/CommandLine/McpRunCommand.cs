// --------------------------------------------------------------------------
// <copyright file="McpRunCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Net.Sockets;
using System.Security;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Hosting;
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Devart.AI.McpServer.CommandLine
{
  public abstract class McpRunCommand : McpCommand
  {
    private const string ConfigArgument = "config";
    private const string FileOption = "--file";

    private const string ConnectionVariablePrefix = "DEVART_MCP_";
    private const string ConnectionVariableSuffix = "_CONNECTION";
    private const string VariablePlaceholderStart = "${";
    private const char VariablePlaceholderEnd = '}';

    private const int ExitSuccess = 0;
    private const int ExitGeneralError = 1;
    private const int ExitConfigurationError = 2;
    private const int ExitAlreadyRunning = 3;

    private static readonly Dictionary<McpProtocolType, IMcpHostRunner> Runners = new()
    {
      [McpProtocolType.Stdio] = new StdioMcpHostRunner(),
      [McpProtocolType.Http] = new HttpMcpHostRunner(),
    };

    protected McpRunCommand() : base("run", "-r", McpResources.CommandLine_CommandRunMcp)
    {
      Arguments.Add(new Argument<string>(ConfigArgument)
      {
        Description = string.Format(McpResources.CommandLine_ParamConfigName, ConnectionVariableName),
        Arity = ArgumentArity.ZeroOrOne
      });
      Option<string> fileOption = new(FileOption, "-f")
      {
        Description = McpResources.CommandLine_OptionFile,
        Arity = ArgumentArity.ExactlyOne
      };
      Options.Add(fileOption);
    }

    protected abstract void ConfigureServices(IHostApplicationBuilder builder, McpConfiguration configuration);

    protected virtual IMcpServerBuilder SetupApplicationBuilder(IHostApplicationBuilder builder, McpConfiguration configuration)
    {
      builder.Services.AddSingleton(configuration);
      ConfigureServices(builder, configuration);
      var serverBuilder = builder.Services.AddMcpServer();
      RegisterTools(serverBuilder, configuration);
      return serverBuilder;
    }

    protected virtual void RegisterTools(IMcpServerBuilder serverBuilder, McpConfiguration configuration)
    {
    }

    public abstract string ProductFullName { get; }

    public abstract string ProductId { get; }

    protected virtual string TechnologyId => null;

    public string ConnectionVariableName
      => string.IsNullOrEmpty(TechnologyId)
        ? $"{ConnectionVariablePrefix}{ProductId.ToUpperInvariant()}{ConnectionVariableSuffix}"
        : $"{ConnectionVariablePrefix}{TechnologyId.ToUpperInvariant()}_{ProductId.ToUpperInvariant()}{ConnectionVariableSuffix}";

    protected abstract McpAppSettings CreateAppSettings();

    protected virtual McpConfiguration CreateConfiguration() => new();

    protected virtual McpConfiguration LoadConfiguration(string configName, string configFile)
    {
      var configPath = McpConfigFile.Resolve(configFile, ProductFullName);
      return CreateConfiguration().Load(configPath, configName, CreateAppSettings());
    }

    protected virtual McpConfiguration LoadEnvironmentConfiguration(string variableName, string configFile)
    {
      var connectionString = ReadConnectionVariable(variableName) ?? throw new ArgumentException(string.Format(McpResources.CommandLine_ConnectionVariableNotSet, variableName));

      var appSettings = CreateAppSettings();
      var profile = JsonSerializer.SerializeToElement(new
      {
        Id = SaveEnvironmentProfile(variableName, connectionString, configFile),
        Name = variableName,
        ConnectionString = connectionString,
        ProtocolType = McpProtocolType.Stdio,
      });

      var config = CreateConfiguration().Create(profile, appSettings);

      return string.IsNullOrEmpty(appSettings.ToolPrefix)
        ? config with { ToolPrefix = ProductId.ToLowerInvariant() }
        : config;
    }

    private static string ReadConnectionVariable(string variableName)
    {
      var value = Environment.GetEnvironmentVariable(variableName);
      if (string.IsNullOrWhiteSpace(value))
      {
        return null;
      }

      var trimmed = value.Trim();

      return trimmed.StartsWith(VariablePlaceholderStart, StringComparison.Ordinal) && trimmed.EndsWith(VariablePlaceholderEnd)
        ? null
        : value;
    }

    private Guid SaveEnvironmentProfile(string profileName, string connectionString, string configFile)
    {
      try
      {
        var path = string.IsNullOrEmpty(configFile) ? McpConfigFile.ResolveForUpdate(ProductFullName) : configFile;
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
          Directory.CreateDirectory(directory);
        }

        var store = McpConfigStore.Open(path);
        var id = store.Set(profileName, new McpConfigValues(McpProtocolType.Stdio, null, null, null, connectionString));
        store.Save();

        return id;
      }
      catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or SecurityException or NotSupportedException or ArgumentException)
      {
        return Guid.NewGuid();
      }
    }

    protected override async Task<int> DoActionAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
      var explicitLogLevel = TryParseVerbosity(parseResult);

      McpConfiguration config;
      string configName;
      try
      {
        configName = parseResult.GetValue<string>(ConfigArgument);
        var configFile = parseResult.GetValue<string>(FileOption);
        if (string.IsNullOrEmpty(configName))
        {
          configName = ConnectionVariableName;
          config = LoadEnvironmentConfiguration(configName, configFile);
        }
        else
        {
          config = LoadConfiguration(configName, configFile);
        }
      }
      catch (OperationCanceledException)
      {
        return ExitSuccess;
      }
      catch (ArgumentException ex)
      {
        LogMcpError(ex.Message);
        return ExitConfigurationError;
      }
      catch (Exception ex)
      {
        LogMcpError(ex.Message);
        return ExitGeneralError;
      }

      var defaultLogLevel = Runners.TryGetValue(config.ProtocolType, out var runner)
        ? runner.DefaultLogLevel
        : LogLevel.None;
      var logLevel = explicitLogLevel ?? defaultLogLevel;

      try
      {
        return await RunMcpServerAsync(config, configName, logLevel, cancellationToken).ConfigureAwait(false);
      }
      catch (OperationCanceledException)
      {
        return ExitSuccess;
      }
      catch (SocketException) when (cancellationToken.IsCancellationRequested)
      {
        return ExitSuccess;
      }
      catch (SingleInstanceAlreadyAcquiredException ex)
      {
        ReportRunError(config.ProtocolType, ex.Message, null);
        return ExitAlreadyRunning;
      }
      catch (ArgumentException ex)
      {
        ReportRunError(config.ProtocolType, ex.Message, null);
        return ExitConfigurationError;
      }
      catch (Exception ex)
      {
        ReportRunError(config.ProtocolType, McpResources.CommandLine_LogUnhandledError, ex);
        return ExitGeneralError;
      }
    }

    private static void ReportRunError(McpProtocolType protocol, string message, Exception ex)
    {
      if (protocol == McpProtocolType.Stdio)
      {
        LogMcpError(message);
        return;
      }

      Console.Error.WriteLine(message);
      if (ex is not null)
      {
        Console.Error.WriteLine(ex);
      }
    }

    private static void LogMcpError(string message)
    {
      var result = new
      {
        jsonrpc = "2.0",
        id = 0,
        result = new
        {
          content = new[]
          {
            new { type = "text", text = message }
          },
          isError = true
        }
      };

      Console.Out.WriteLine(JsonSerializer.Serialize(result));
    }

    private Task<int> RunMcpServerAsync(McpConfiguration config, string configName, LogLevel logLevel, CancellationToken cancellationToken)
      => !Runners.TryGetValue(config.ProtocolType, out var runner)
        ? throw new ArgumentException(McpResources.Common_ConfigFileInvalidProtocolType)
        : runner.RunAsync(config, (builder, c) =>
          {
            builder.Services.AddSingleton(new McpRunContext(configName));
            builder.Services.AddHostedService<McpLifetimeLogger>();
            return SetupApplicationBuilder(builder, c);
          }, logLevel, cancellationToken);
  }
}