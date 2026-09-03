// --------------------------------------------------------------------------
// <copyright file="McpConfigDeleteCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;

namespace Devart.AI.McpServer.CommandLine
{
  internal sealed class McpConfigDeleteCommand(string productFullName)
    : McpConfigProfileCommand("delete", McpResources.CommandLine_CommandConfigDelete, productFullName)
  {
    protected override int Execute(McpConfigStore store, string profileName, ParseResult parseResult)
    {
      if (!store.Remove(profileName))
      {
        Console.Error.WriteLine(string.Format(McpResources.CommandLine_ConfigProfileNotFound, profileName, store.FilePath));

        return ExitError;
      }

      store.Save();

      Console.Out.WriteLine(string.Format(McpResources.CommandLine_ConfigProfileDeleted, profileName, store.FilePath));

      return ExitSuccess;
    }
  }
}