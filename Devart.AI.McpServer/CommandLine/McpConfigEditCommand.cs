// --------------------------------------------------------------------------
// <copyright file="McpConfigEditCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;

namespace Devart.AI.McpServer.CommandLine
{
  internal sealed class McpConfigEditCommand(string productFullName)
    : McpConfigValuesCommand("edit", McpResources.CommandLine_CommandConfigEdit, productFullName)
  {
    protected override int Execute(McpConfigStore store, string profileName, ParseResult parseResult)
    {
      var values = GetValues(parseResult);
      if (values.IsEmpty)
      {
        Console.Error.WriteLine(McpResources.CommandLine_ConfigNothingToUpdate);

        return ExitError;
      }

      if (!store.Update(profileName, values))
      {
        Console.Error.WriteLine(string.Format(McpResources.CommandLine_ConfigProfileNotFound, profileName, store.FilePath));

        return ExitError;
      }

      store.Save();

      Console.Out.WriteLine(string.Format(McpResources.CommandLine_ConfigProfileUpdated, profileName, store.FilePath));

      return ExitSuccess;
    }
  }
}