// --------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Reflection;
using System.Resources;

[assembly: AssemblyProduct($"Devart {Devart.AI.McpServer.AdoNet.Properties.ProductInfo.ProductFullName}")]
[assembly: AssemblyVersion(Devart.AI.McpServer.AdoNet.Properties.ProductInfo.ProductVersion)]
[assembly: AssemblyFileVersion(Devart.AI.McpServer.AdoNet.Properties.ProductInfo.ProductFileVersion)]
[assembly: NeutralResourcesLanguage("en")]

namespace Devart.AI.McpServer.AdoNet.Properties
{
  internal static class ProductInfo
  {
    internal const string
      ProductVersion = "1.0.0",
      ProductFileVersion = $"{ProductVersion}.0",
      ProductFullName = "MCP Server AdoNet";
  }
}