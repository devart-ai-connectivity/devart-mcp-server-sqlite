// --------------------------------------------------------------------------
// <copyright file="AdoNetInstructionsTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.AdoNet.Tools
{
  internal sealed class AdoNetInstructionsTool(McpConfiguration serverConfiguration) : InstructionsTool(serverConfiguration)
  {
    protected override string InstructionsResourceName => "Devart.AI.McpServer.AdoNet.Embedded.Instructions.md";
  }
}
