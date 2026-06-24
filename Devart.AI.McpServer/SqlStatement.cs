// --------------------------------------------------------------------------
// <copyright file="SqlStatement.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer
{
  public sealed class SqlStatement
  {
    public StatementType Type { get; init; }

    public string Text { get; init; }
  }
}
