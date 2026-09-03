// --------------------------------------------------------------------------
// <copyright file="McpConfigFile.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.IO;

namespace Devart.AI.McpServer
{
  public static class McpConfigFile
  {
    public const string FileName = "mcpserver.json";

    private const string VendorFolder = "Devart";

    public static string Resolve(string configFile, string productFullName)
    {
      if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
      {
        return configFile;
      }

      var localPath = GetLocalPath();

      return localPath is not null && File.Exists(localPath)
        ? localPath
        : GetCommonPath(productFullName);
    }

    public static string ResolveForUpdate(string productFullName)
    {
      var localPath = GetLocalPath();
      if (localPath is not null && File.Exists(localPath))
      {
        return localPath;
      }

      var commonPath = GetCommonPath(productFullName);
      Directory.CreateDirectory(Path.GetDirectoryName(commonPath));

      return commonPath;
    }

    private static string GetLocalPath()
    {
      var directory = McpAppLocation.AppDirectory;

      return string.IsNullOrEmpty(directory) ? null : Path.Combine(directory, FileName);
    }

    private static string GetCommonPath(string productFullName)
      => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        VendorFolder,
        productFullName,
        FileName);
  }
}