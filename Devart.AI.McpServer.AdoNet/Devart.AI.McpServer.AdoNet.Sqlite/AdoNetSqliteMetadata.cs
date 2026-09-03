// --------------------------------------------------------------------------
// <copyright file="AdoNetSqliteMetadata.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.AdoNet.Sqlite
{
  public class AdoNetSqliteMetadata : AdoNetMetadata
  {
    public const string PrimaryKeyName = "PkName";

    public const string PrimaryKeyColumn = "PkColumn";

    private const string Tables = "Tables";
    private const string Columns = "Columns";
    private const string IndexColumns = "IndexColumns";
    private const string ForeignKeyColumns = "ForeignKeyColumns";
    private const string PrimaryKeys = "PrimaryKeys";

    private const string Catalog = "Catalog";
    private const string Name = "Name";
    private const string TypeName = "TypeName";
    private const string MaxLength = "CharacterMaximumLength";
    private const string Nullable = "Nullable";
    private const string DefaultValue = "DefaultValue";
    private const string Index = "Index";
    private const string Position = "Position";
    private const string ForeignKey = "ForeignKey";
    private const string ReferencedTable = "ReferencedTable";
    private const string ReferencedColumn = "ReferencedColumn";

    public override string SchemaName(string schema) => "";

    public override string TablesCollectionName => Tables;

    public override string TablesSchemaName => "";

    public override MetadataColumn[] TablesColumnsMapping
      => [(Catalog, McpResources.TablesTool_TableSchema),
          (Name, McpResources.TablesTool_TableName)];

    public override string ColumnsCollectionName => Columns;

    public override MetadataColumn[] ColumnsColumnsMapping
      => [(Name, McpResources.ColumnsTool_ColumnNameHeader),
          (TypeName, McpResources.ColumnsTool_DataTypeHeader),
          (MaxLength, McpResources.ColumnsTool_SizeHeader),
          (Nullable, McpResources.ColumnsTool_NullableHeader),
          (DefaultValue, McpResources.ColumnsTool_DefaultValueHeader, false)];

    public override string[] ColumnsRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), tableName];

    public override string IndexesCollectionName => IndexColumns;

    public override MetadataColumn[] IndexesColumnsMapping
      => [(Index, McpResources.IndexesTool_IndexName),
          (Name, McpResources.IndexesTool_ColumnName),
          (Position, McpResources.IndexesTool_OrdinalPosition)];

    public override string[] IndexesRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), tableName];

    public override string PrimaryKeysCollectionName => PrimaryKeys;

    public override MetadataColumn[] PrimaryKeysColumnsMapping
      => [(PrimaryKeyName, McpResources.PrimaryKeysTool_PkNameHeader),
          (PrimaryKeyColumn, McpResources.PrimaryKeysTool_PkColumnHeader)];

    public override string[] PrimaryKeysRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), tableName];

    public override string ForeignKeysCollectionName => ForeignKeyColumns;

    public override MetadataColumn[] ForeignKeysColumnsMapping
      => [(ForeignKey, McpResources.ForeignKeysTool_FkNameHeader),
          (Name, McpResources.ForeignKeysTool_FkColumnHeader),
          (ReferencedTable, McpResources.ForeignKeysTool_PkTableHeader),
          (ReferencedColumn, McpResources.ForeignKeysTool_PkColumnHeader)];

    public override string[] ForeignKeysRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), tableName];
  }
}