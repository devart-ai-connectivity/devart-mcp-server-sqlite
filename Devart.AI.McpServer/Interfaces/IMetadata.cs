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

    MetadataColumn[] TablesColumnsMapping { get; }

    string[] TablesRestrictions(string database);

    string ColumnsCollectionName { get; }

    MetadataColumn[] ColumnsColumnsMapping { get; }

    string[] ColumnsRestrictions(string database, string schema, string tableName);

    string IndexesCollectionName { get; }

    MetadataColumn[] IndexesColumnsMapping { get; }

    string[] IndexesRestrictions(string database, string schema, string tableName);

    string ForeignKeysCollectionName { get; }
    MetadataColumn[] ForeignKeysColumnsMapping { get; }

    string[] ForeignKeysRestrictions(string database, string schema, string tableName);

    string PrimaryKeysCollectionName { get; }

    MetadataColumn[] PrimaryKeysColumnsMapping { get; }

    string[] PrimaryKeysRestrictions(string database, string schema, string tableName);

    string RoutinesCollectionName { get; }
    MetadataColumn[] RoutinesColumnsMapping { get; }
  }
}