// --------------------------------------------------------------------------
// <copyright file="AdoNetLauncher.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.CommandLine;
using System.Linq;
using Devart.AI.McpServer.AdoNet.CommandLine;
using Devart.AI.McpServer.CommandLine;

namespace Devart.AI.McpServer.AdoNet
{
  public static class AdoNetLauncher
  {
    public static McpLauncher Create(params Command[] productCommands)
    {
      var commands = new Command[productCommands.Length + 1];
      productCommands.CopyTo(commands, 0);
      commands[productCommands.Length] = new AdoNetConfigCommand(GetProductFullName(productCommands));
      return new McpLauncher(AdoNetResources.CommandLine_RootCommand, commands);
    }

    private static string GetProductFullName(Command[] productCommands)
      => productCommands.OfType<McpRunCommand>().FirstOrDefault()?.ProductFullName;
  }
}