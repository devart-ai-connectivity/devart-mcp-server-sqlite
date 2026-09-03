// --------------------------------------------------------------------------
// <copyright file="AdoNetConfigCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.CommandLine;

namespace Devart.AI.McpServer.AdoNet.CommandLine
{
  internal sealed class AdoNetConfigCommand(string productFullName) : McpConfigCommand(productFullName)
  {
    protected override int ExecuteConfig()
    {
      AdoNetConfig config = new();

      return config.Execute();
    }
  }
}