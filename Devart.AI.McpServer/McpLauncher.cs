// --------------------------------------------------------------------------
// <copyright file="McpLauncher.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.CommandLine;

namespace Devart.AI.McpServer
{
  public sealed class McpLauncher(params Command[] commands)
  {
    private static Mutex _setupMutex;

    private readonly IReadOnlyList<Command> _commands = commands ?? [];

    public async Task<int> RunAsync(string[] args)
    {
      _setupMutex = new Mutex(false, @$"Global\{Assembly.GetEntryAssembly()?.GetName().Name ?? Assembly.GetExecutingAssembly().GetName().Name}");
      GC.KeepAlive(_setupMutex);

      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
      CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

      using var cancellationTokenSource = new CancellationTokenSource();
      using var sigint = PosixSignalRegistration.Create(PosixSignal.SIGINT, ctx =>
      {
        ctx.Cancel = true;
        cancellationTokenSource.Cancel();
      });
      using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx =>
      {
        ctx.Cancel = true;
        cancellationTokenSource.Cancel();
      });

      var rootCommand = new McpRootCommand();
      foreach (var command in _commands)
      {
        rootCommand.Subcommands.Add(command);
      }

      var result = rootCommand.Parse(args);
      return await result.InvokeAsync(new InvocationConfiguration(), cancellationTokenSource.Token);
    }
  }
}
