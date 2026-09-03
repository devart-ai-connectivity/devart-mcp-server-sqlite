// --------------------------------------------------------------------------
// <copyright file="McpAppLocation.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;

namespace Devart.AI.McpServer
{
  public static class McpAppLocation
  {
    private const string DotnetHostName = "dotnet";

    public static string AppDirectory { get; } = ResolveAppDirectory();

    private static string ResolveAppDirectory()
    {
      var processPath = Environment.ProcessPath;

      if (!IsDotnetHost(processPath))
      {
        return Path.GetDirectoryName(processPath);
      }

      var entryAssemblyPath = Assembly.GetEntryAssembly()?.Location;

      return string.IsNullOrEmpty(entryAssemblyPath)
        ? Path.GetDirectoryName(processPath)
        : Path.GetDirectoryName(entryAssemblyPath);
    }

    private static bool IsDotnetHost(string processPath) =>
      string.Equals(Path.GetFileNameWithoutExtension(processPath), DotnetHostName, StringComparison.OrdinalIgnoreCase);
  }
}