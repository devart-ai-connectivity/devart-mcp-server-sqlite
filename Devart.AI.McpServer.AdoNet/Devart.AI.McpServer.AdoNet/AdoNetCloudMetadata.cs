// --------------------------------------------------------------------------
// <copyright file="AdoNetCloudMetadata.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.AdoNet
{
  public class AdoNetCloudMetadata : AdoNetMetadata
  {
    public override string DatabaseName(string database) => "";

    public override string SchemaName(string schema) => "";

    public override string TablesCollectionName => AdoNetCloudConstants.Tables;

    public override string TablesSchemaName => "";

    public override MetadataColumn[] TablesColumnsMapping
      => [(AdoNetCloudConstants.Name, McpResources.TablesTool_TableName)];

    public override string ColumnsCollectionName => AdoNetCloudConstants.Columns;

    public override MetadataColumn[] ColumnsColumnsMapping
      => [(AdoNetCloudConstants.Name, McpResources.ColumnsTool_ColumnNameHeader),
          (AdoNetCloudConstants.DataType, McpResources.ColumnsTool_DataTypeHeader),
          (AdoNetCloudConstants.Length, McpResources.ColumnsTool_SizeHeader),
          (AdoNetCloudConstants.Nullable, McpResources.ColumnsTool_NullableHeader),
          (AdoNetCloudConstants.DefaultValue, McpResources.ColumnsTool_DefaultValueHeader, false),
          (AdoNetCloudConstants.Label, McpResources.ColumnsTool_DescriptionHeader, false)];

    public override string[] ColumnsRestrictions(string database, string schema, string tableName)
      => [tableName];

    public override string IndexesCollectionName => AdoNetCloudConstants.IndexColumns;

    public override MetadataColumn[] IndexesColumnsMapping
      => [(AdoNetCloudConstants.Constraint, McpResources.IndexesTool_IndexName),
          (AdoNetCloudConstants.Name, McpResources.IndexesTool_ColumnName),
          (AdoNetCloudConstants.Position, McpResources.IndexesTool_OrdinalPosition)];

    public override string[] IndexesRestrictions(string database, string schema, string tableName)
      => [tableName];

    public override string ForeignKeysCollectionName => AdoNetCloudConstants.ForeignKeyColumns;

    public override MetadataColumn[] ForeignKeysColumnsMapping
      => [(AdoNetCloudConstants.Constraint, McpResources.ForeignKeysTool_FkNameHeader),
          (AdoNetCloudConstants.Name, McpResources.ForeignKeysTool_FkColumnHeader),
          (AdoNetCloudConstants.ReferencedTable, McpResources.ForeignKeysTool_PkTableHeader),
          (AdoNetCloudConstants.ReferencedColumn, McpResources.ForeignKeysTool_PkColumnHeader),
          (AdoNetCloudConstants.CascadeDelete, McpResources.ForeignKeysTool_DeleteRuleHeader)];

    public override string[] ForeignKeysRestrictions(string database, string schema, string tableName)
      => [tableName];

    public override string PrimaryKeysCollectionName => AdoNetCloudConstants.PrimaryKeyColumns;

    public override MetadataColumn[] PrimaryKeysColumnsMapping
      => [(AdoNetCloudConstants.Constraint, McpResources.PrimaryKeysTool_PkNameHeader),
          (AdoNetCloudConstants.Name, McpResources.PrimaryKeysTool_PkColumnHeader)];

    public override string[] PrimaryKeysRestrictions(string database, string schema, string tableName)
      => [tableName];
  }
}