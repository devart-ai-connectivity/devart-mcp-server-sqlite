// --------------------------------------------------------------------------
// <copyright file="McpConfigAddCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;

namespace Devart.AI.McpServer.CommandLine
{
  internal sealed class McpConfigAddCommand(string productFullName)
    : McpConfigValuesCommand("add", McpResources.CommandLine_CommandConfigAdd, productFullName)
  {
    protected override int Execute(McpConfigStore store, string profileName, ParseResult parseResult)
    {
      if (store.Contains(profileName))
      {
        Console.Error.WriteLine(string.Format(McpResources.CommandLine_ConfigProfileExists, profileName, store.FilePath));

        return ExitError;
      }

      store.Add(profileName, GetValues(parseResult));
      store.Save();

      Console.Out.WriteLine(string.Format(McpResources.CommandLine_ConfigProfileAdded, profileName, store.FilePath));

      return ExitSuccess;
    }
  }
}