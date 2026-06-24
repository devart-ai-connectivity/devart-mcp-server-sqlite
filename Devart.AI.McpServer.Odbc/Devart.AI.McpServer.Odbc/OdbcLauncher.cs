// --------------------------------------------------------------------------
// <copyright file="OdbcLauncher.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.CommandLine;
using Devart.AI.McpServer.Odbc.CommandLine;

namespace Devart.AI.McpServer.Odbc
{
  public static class OdbcLauncher
  {
    public static McpLauncher Create(params Command[] productCommands)
    {
      var commands = new Command[productCommands.Length + 1];
      productCommands.CopyTo(commands, 0);
      commands[productCommands.Length] = new OdbcConfigCommand();
      return new McpLauncher(commands);
    }
  }
}
