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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Hosting;

namespace Devart.AI.McpServer.CommandLine
{
  public abstract class McpRunCommand : McpCommand
  {
    private const string ConfigArgument = "config";
    private const string FileOption = "--file";

    protected const string ConfigFileName = "mcpserver.json";

    private const int ExitSuccess = 0;
    private const int ExitGeneralError = 1;
    private const int ExitConfigurationError = 2;
    private const int ExitAlreadyRunning = 3;

    private static readonly Dictionary<McpProtocolType, IMcpHostRunner> Runners = new()
    {
      [McpProtocolType.Stdio] = new StdioMcpHostRunner(),
      [McpProtocolType.Http]  = new HttpMcpHostRunner(),
    };

    protected McpRunCommand() : base("run", "-r", McpResources.CommandLine_CommandRunMcp)
    {
      Arguments.Add(new Argument<string>(ConfigArgument)
      {
        Description = McpResources.CommandLine_ParamConfigName
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

    protected abstract string ProductFullName { get; }

    protected abstract McpAppSettings CreateAppSettings();

    protected virtual McpConfiguration CreateConfiguration() => new();

    protected virtual McpConfiguration LoadConfiguration(string configName, string configFile)
    {
      var configPath = ResolveConfigPath(configFile, ConfigFileName, ProductFullName);
      return CreateConfiguration().Load(configPath, configName, CreateAppSettings());
    }

    protected static string ResolveConfigPath(string configFile, string configFileName, string productFullName)
    {
      if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
      {
        return configFile;
      }

      var localPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), configFileName);
      if (File.Exists(localPath))
      {
        return localPath;
      }

      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Devart", productFullName, configFileName);
    }

    protected override async Task<int> DoActionAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
      var explicitLogLevel = TryParseVerbosity(parseResult);

      McpConfiguration config;
      string configName;
      try
      {
        configName = parseResult.GetRequiredValue<string>(ConfigArgument);
        var configFile = parseResult.GetValue<string>(FileOption);
        config = LoadConfiguration(configName, configFile);
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
    {
      if (!Runners.TryGetValue(config.ProtocolType, out var runner))
      {
        throw new ArgumentException(McpResources.Common_ConfigFileInvalidProtocolType);
      }

      return runner.RunAsync(config, (builder, c) =>
      {
        builder.Services.AddSingleton(new McpRunContext(configName));
        builder.Services.AddHostedService<McpLifetimeLogger>();
        return SetupApplicationBuilder(builder, c);
      }, logLevel, cancellationToken);
    }
  }
}
