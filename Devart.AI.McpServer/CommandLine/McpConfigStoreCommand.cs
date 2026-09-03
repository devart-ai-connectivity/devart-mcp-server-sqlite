// --------------------------------------------------------------------------
// <copyright file="McpConfigStoreCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.AI.McpServer.CommandLine
{
  internal abstract class McpConfigStoreCommand(string name, string description, string productFullName)
    : McpCommand(name, description)
  {
    protected const int ExitSuccess = 0;
    protected const int ExitError = 1;

    protected string ProductFullName { get; } = productFullName;

    protected abstract int Execute(McpConfigStore store, ParseResult parseResult);

    protected virtual string ResolveFilePath() => McpConfigFile.ResolveForUpdate(ProductFullName);

    protected sealed override Task<int> DoActionAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
      try
      {
        return Task.FromResult(Execute(McpConfigStore.Open(ResolveFilePath()), parseResult));
      }
      catch (Exception ex) when (ex is ArgumentException or IOException or JsonException or NotSupportedException or UnauthorizedAccessException)
      {
        Console.Error.WriteLine(ex.Message);

        return Task.FromResult(ExitError);
      }
    }
  }
}