// --------------------------------------------------------------------------
// <copyright file="OdbcConfig.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Odbc
{
  internal sealed class OdbcConfig : IConfig
  {
    public int Execute()
    {
      var startInfo = new ProcessStartInfo
      {
        FileName = Path.Combine(McpAppLocation.AppDirectory, "Devart.AI.McpServer.ConnectionManager.exe"),
        UseShellExecute = false,
        CreateNoWindow = false
      };

      using var process = new Process { StartInfo = startInfo };

      return process.Start() ? 0 : -1;
    }
  }
}