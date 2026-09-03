// --------------------------------------------------------------------------
// <copyright file="MetadataColumn.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer
{
  public readonly record struct MetadataColumn(string Name, string Alias, bool Required = true)
  {
    public static implicit operator MetadataColumn((string name, string alias) column)
      => new(column.name, column.alias);

    public static implicit operator MetadataColumn((string name, string alias, bool required) column)
      => new(column.name, column.alias, column.required);
  }
}