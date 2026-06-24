// --------------------------------------------------------------------------
// <copyright file="AdoNetMetadata.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.AdoNet
{
  internal sealed class AdoNetMetadata : IMetadata
  {
    public string DatabaseName(string database) => database;

    public string SchemaName(string schema) => schema;

    public string TablesCollectionName => AdoNetConstants.Tables;

    public string TablesSchemaName => AdoNetConstants.TableSchema;

    public (string name, string alias)[] TablesColumnsMapping
      => [(AdoNetConstants.TableSchema, McpResources.TablesTool_TableSchema),
          (AdoNetConstants.TableName, McpResources.TablesTool_TableName),
          (AdoNetConstants.TableRemarks, McpResources.TablesTool_Remarks)];

    public string ColumnsCollectionName => AdoNetConstants.Columns;

    public (string name, string alias)[] ColumnsColumnsMapping
      => [(AdoNetConstants.ColumnName, McpResources.ColumnsTool_ColumnNameHeader),
          (AdoNetConstants.ColumnTypeName, McpResources.ColumnsTool_DataTypeHeader),
          (AdoNetConstants.ColumnSize, McpResources.ColumnsTool_SizeHeader),
          (AdoNetConstants.ColumnNullable, McpResources.ColumnsTool_NullableHeader),
          (AdoNetConstants.ColumnRemarks, McpResources.ColumnsTool_DescriptionHeader)];

    public string IndexesCollectionName => AdoNetConstants.Indexes;

    public (string name, string alias)[] IndexesColumnsMapping
      => [(AdoNetConstants.IndexName, McpResources.IndexesTool_IndexName),
          (AdoNetConstants.IndexType, McpResources.IndexesTool_IndexType),
          (AdoNetConstants.IndexColumns, McpResources.IndexesTool_Columns)];

    public string ForeignKeysCollectionName => AdoNetConstants.ForeignKeys;

    public (string name, string alias)[] ForeignKeysColumnsMapping
      => [(AdoNetConstants.ForeignKeyFkName, McpResources.ForeignKeysTool_FkNameHeader),
          (AdoNetConstants.ForeignKeyFkColumn, McpResources.ForeignKeysTool_FkColumnHeader),
          (AdoNetConstants.ForeignKeyPkSchema, McpResources.ForeignKeysTool_PkSchemaHeader),
          (AdoNetConstants.ForeignKeyPkTable, McpResources.ForeignKeysTool_PkTableHeader),
          (AdoNetConstants.ForeignKeyPkColumn, McpResources.ForeignKeysTool_PkColumnHeader),
          (AdoNetConstants.ForeignKeyUpdateRule, McpResources.ForeignKeysTool_UpdateRuleHeader),
          (AdoNetConstants.ForeignKeyDeleteRule, McpResources.ForeignKeysTool_DeleteRuleHeader)];

    public string PrimaryKeysCollectionName => AdoNetConstants.PrimaryKeys;

    public (string name, string alias)[] PrimaryKeysColumnsMapping
      => [(AdoNetConstants.PrimaryKeyName, McpResources.PrimaryKeysTool_PkNameHeader),
          (AdoNetConstants.PrimaryKeyColumn, McpResources.PrimaryKeysTool_PkColumnHeader)];

    public string RoutinesCollectionName => "";

    public (string name, string alias)[] RoutinesColumnsMapping
      => [(AdoNetConstants.ProcedureSchema, McpResources.RoutinesTool_ProcedureSchema),
          (AdoNetConstants.ProcedureName, McpResources.RoutinesTool_ProcedureName),
          (AdoNetConstants.ProcedureType, McpResources.RoutinesTool_ProcedureType),
          (AdoNetConstants.ProcedureParameters, McpResources.RoutinesTool_ProcedureParameters)];
  }
}
