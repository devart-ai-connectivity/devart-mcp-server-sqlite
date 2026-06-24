// --------------------------------------------------------------------------
// <copyright file="ISqlStatementsParser.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.Interfaces
{
  public interface ISqlStatementsParser
  {
    SqlStatement[] Parse(string scriptText);
  }
}
