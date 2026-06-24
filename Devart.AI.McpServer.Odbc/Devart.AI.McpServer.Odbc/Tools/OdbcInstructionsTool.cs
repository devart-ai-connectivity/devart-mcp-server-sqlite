// --------------------------------------------------------------------------
// <copyright file="OdbcInstructionsTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.Odbc.Tools
{
  internal sealed class OdbcInstructionsTool(McpConfiguration serverConfiguration) : InstructionsTool(serverConfiguration)
  {
    protected override string InstructionsResourceName => "Devart.AI.McpServer.Odbc.Embedded.Instructions.md";
  }
}
