// --------------------------------------------------------------------------
// <copyright file="OdbcConfigCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.CommandLine;

namespace Devart.AI.McpServer.Odbc.CommandLine
{
  internal sealed class OdbcConfigCommand : McpConfigCommand
  {
    protected override int ExecuteConfig()
    {
      OdbcConfig config = new();

      return config.Execute();
    }
  }
}
