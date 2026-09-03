// --------------------------------------------------------------------------
// <copyright file="AdoNetMetadata.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.AdoNet
{
  public class AdoNetMetadata : IMetadata
  {
    public virtual string DatabaseName(string database) => database;

    public virtual string SchemaName(string schema) => schema;

    public virtual string TablesCollectionName => throw NotConfigured();

    public virtual string TablesSchemaName => throw NotConfigured();

    public virtual MetadataColumn[] TablesColumnsMapping => throw NotConfigured();

    public virtual string[] TablesRestrictions(string database) => null;

    public virtual string ColumnsCollectionName => throw NotConfigured();

    public virtual MetadataColumn[] ColumnsColumnsMapping => throw NotConfigured();

    public virtual string[] ColumnsRestrictions(string database, string schema, string tableName)
      => throw NotConfigured();

    public virtual string IndexesCollectionName => throw NotConfigured();

    public virtual MetadataColumn[] IndexesColumnsMapping => throw NotConfigured();

    public virtual string[] IndexesRestrictions(string database, string schema, string tableName)
      => throw NotConfigured();

    public virtual string ForeignKeysCollectionName => throw NotConfigured();

    public virtual MetadataColumn[] ForeignKeysColumnsMapping => throw NotConfigured();

    public virtual string[] ForeignKeysRestrictions(string database, string schema, string tableName)
      => throw NotConfigured();

    public virtual string PrimaryKeysCollectionName => throw NotConfigured();

    public virtual MetadataColumn[] PrimaryKeysColumnsMapping => throw NotConfigured();

    public virtual string[] PrimaryKeysRestrictions(string database, string schema, string tableName)
      => throw NotConfigured();

    public virtual string RoutinesCollectionName => throw NotConfigured();

    public virtual MetadataColumn[] RoutinesColumnsMapping => throw NotConfigured();

    private static NotSupportedException NotConfigured()
      => new(AdoNetResources.AdoNetMetadata_NotConfigured);
  }
}