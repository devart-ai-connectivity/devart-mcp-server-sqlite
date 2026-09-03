// --------------------------------------------------------------------------
// <copyright file="OdbcMetadata.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Odbc;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Odbc
{
  public class OdbcMetadata : IMetadata
  {
    public virtual string DatabaseName(string database) => database;

    public virtual string SchemaName(string schema) => schema;

    public string TablesCollectionName => OdbcMetaDataCollectionNames.Tables;

    public string TablesSchemaName => OdbcConstants.TableSchema;

    public MetadataColumn[] TablesColumnsMapping
      => [(OdbcConstants.TableSchema, McpResources.TablesTool_TableSchema),
          (OdbcConstants.TableName, McpResources.TablesTool_TableName),
          (OdbcConstants.Remarks, McpResources.TablesTool_Remarks, false)];

    public virtual string[] TablesRestrictions(string database) => null;

    public virtual string[] ColumnsRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), SchemaName(schema), tableName];

    public virtual string[] IndexesRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), SchemaName(schema), tableName];

    public virtual string[] ForeignKeysRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), SchemaName(schema), tableName];

    public virtual string[] PrimaryKeysRestrictions(string database, string schema, string tableName)
      => [DatabaseName(database), SchemaName(schema), tableName];

    public string ColumnsCollectionName => OdbcMetaDataCollectionNames.Columns;

    public MetadataColumn[] ColumnsColumnsMapping
      => [(OdbcConstants.ColumnName, McpResources.ColumnsTool_ColumnNameHeader),
          (OdbcConstants.TypeName, McpResources.ColumnsTool_DataTypeHeader),
          (OdbcConstants.ColumnSize, McpResources.ColumnsTool_SizeHeader),
          (OdbcConstants.Nullable, McpResources.ColumnsTool_NullableHeader),
          (OdbcConstants.ColumnDefinition, McpResources.ColumnsTool_DefaultValueHeader, false),
          (OdbcConstants.Remarks, McpResources.ColumnsTool_DescriptionHeader, false)];

    public string IndexesCollectionName => OdbcMetaDataCollectionNames.Indexes;

    public MetadataColumn[] IndexesColumnsMapping
      => [(OdbcConstants.IndexName, McpResources.IndexesTool_IndexName),
          (OdbcConstants.IndexType, McpResources.IndexesTool_IndexType),
          (OdbcConstants.ColumnName, McpResources.IndexesTool_ColumnName),
          (OdbcConstants.OrdinalPosition, McpResources.IndexesTool_OrdinalPosition)];

    public string ForeignKeysCollectionName => OdbcConstants.ForeignKeysCollectionName;

    public MetadataColumn[] ForeignKeysColumnsMapping
      => [(OdbcConstants.ForeignKeyFkName, McpResources.ForeignKeysTool_FkNameHeader),
          (OdbcConstants.ForeignKeyFkColumn, McpResources.ForeignKeysTool_FkColumnHeader),
          (OdbcConstants.ForeignKeyPkSchema, McpResources.ForeignKeysTool_PkSchemaHeader),
          (OdbcConstants.ForeignKeyPkTable, McpResources.ForeignKeysTool_PkTableHeader),
          (OdbcConstants.ForeignKeyPkColumn, McpResources.ForeignKeysTool_PkColumnHeader),
          (OdbcConstants.ForeignKeyUpdateRule, McpResources.ForeignKeysTool_UpdateRuleHeader),
          (OdbcConstants.ForeignKeyDeleteRule, McpResources.ForeignKeysTool_DeleteRuleHeader)];

    public string PrimaryKeysCollectionName => OdbcConstants.PrimaryKeysCollectionName;

    public MetadataColumn[] PrimaryKeysColumnsMapping
      => [(OdbcConstants.PrimaryKeyName, McpResources.PrimaryKeysTool_PkNameHeader),
          (OdbcConstants.ColumnName, McpResources.PrimaryKeysTool_PkColumnHeader)];

    public string RoutinesCollectionName => OdbcMetaDataCollectionNames.Procedures;

    public MetadataColumn[] RoutinesColumnsMapping
      => [(OdbcConstants.ProcedureSchema, McpResources.RoutinesTool_ProcedureSchema),
          (OdbcConstants.ProcedureName, McpResources.RoutinesTool_ProcedureName),
          (OdbcConstants.ProcedureType, McpResources.RoutinesTool_ProcedureType),
          (OdbcConstants.ProcedureParameters, McpResources.RoutinesTool_ProcedureParameters),
          (OdbcConstants.Remarks, McpResources.RoutinesTool_Remarks, false)];
  }
}