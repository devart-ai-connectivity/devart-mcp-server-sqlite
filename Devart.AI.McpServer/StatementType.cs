// --------------------------------------------------------------------------
// <copyright file="StatementType.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Reflection;

namespace Devart.AI.McpServer
{
  [Obfuscation]
  public enum StatementType
  {
    Select,

    Insert,

    Update,

    Delete,

    Create,

    Alter,

    Drop,

    Call,

    Unknown
  }
}