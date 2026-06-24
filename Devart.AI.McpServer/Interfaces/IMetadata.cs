// --------------------------------------------------------------------------
// <copyright file="IMetadata.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.Interfaces
{
  public interface IMetadata
  {
    string DatabaseName(string database);

    string SchemaName(string schema);
    
    string TablesCollectionName { get; }
    
    string TablesSchemaName { get; }
    
    (string name, string alias)[] TablesColumnsMapping { get; }
    
    string ColumnsCollectionName { get; }

    (string name, string alias)[] ColumnsColumnsMapping { get; }

    string IndexesCollectionName { get; }

    (string name, string alias)[] IndexesColumnsMapping { get; }

    string ForeignKeysCollectionName { get; }
    (string name, string alias)[] ForeignKeysColumnsMapping { get; }

    string PrimaryKeysCollectionName { get; }

    (string name, string alias)[] PrimaryKeysColumnsMapping { get; }

    string RoutinesCollectionName { get; }
    (string name, string alias)[] RoutinesColumnsMapping { get; }
  }
}
