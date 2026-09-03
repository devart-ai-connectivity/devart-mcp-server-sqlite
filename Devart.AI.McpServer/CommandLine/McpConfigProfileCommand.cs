// --------------------------------------------------------------------------
// <copyright file="McpConfigProfileCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;

namespace Devart.AI.McpServer.CommandLine
{
  internal abstract class McpConfigProfileCommand : McpConfigStoreCommand
  {
    private const string ProfileArgument = "profile";

    protected McpConfigProfileCommand(string name, string description, string productFullName)
      : base(name, description, productFullName)
    {
      Arguments.Add(new Argument<string>(ProfileArgument)
      {
        Description = McpResources.CommandLine_ParamProfileName
      });
    }

    protected abstract int Execute(McpConfigStore store, string profileName, ParseResult parseResult);

    protected sealed override int Execute(McpConfigStore store, ParseResult parseResult)
    {
      var profileName = parseResult.GetRequiredValue<string>(ProfileArgument);
      if (string.IsNullOrWhiteSpace(profileName))
      {
        Console.Error.WriteLine(McpResources.CommandLine_ConfigProfileNameRequired);

        return ExitError;
      }

      return Execute(store, profileName, parseResult);
    }
  }
}