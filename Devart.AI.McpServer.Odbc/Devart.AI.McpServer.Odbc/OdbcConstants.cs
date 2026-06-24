// --------------------------------------------------------------------------
// <copyright file="OdbcConstants.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.Odbc
{
  public static class OdbcConstants
  {
    public const string ForeignKeysCollectionName = "ForeignKeys";

    public const string PrimaryKeysCollectionName = "PrimaryKeys";

    public const string TableSchema = "TABLE_SCHEM";
    public const string TableName = "TABLE_NAME";
    public const string Remarks = "REMARKS";

    public const string ColumnName = "COLUMN_NAME";
    public const string TypeName = "TYPE_NAME";
    public const string ColumnSize = "COLUMN_SIZE";
    public const string Nullable = "NULLABLE";
    public const string ColumnType = "COLUMN_TYPE";
    public const string ColumnDefinition = "COLUMN_DEF";

    public const string IndexName = "INDEX_NAME";
    public const string IndexType = "INDEX_TYPE";
    public const string IndexColumn = "COLUMN_NAME";
    public const string OrdinalPosition = "ORDINAL_POSITION";
    public const string IndexIsNonUnique = "NON_UNIQUE";

    public const string ProcedureSchema = "PROCEDURE_SCHEM";
    public const string ProcedureName = "PROCEDURE_NAME";
    public const string ProcedureType = "PROCEDURE_TYPE";
    public const string ProcedureParameters = "PROCEDURE_PARAMETERS";

    public const string ForeignKeyFkName = "FK_NAME";
    public const string ForeignKeyFkColumn = "FKCOLUMN_NAME";
    public const string ForeignKeyPkSchema = "PKTABLE_SCHEM";
    public const string ForeignKeyPkTable = "PKTABLE_NAME";
    public const string ForeignKeyPkColumn = "PKCOLUMN_NAME";
    public const string ForeignKeyUpdateRule = "UPDATE_RULE";
    public const string ForeignKeyDeleteRule = "DELETE_RULE";

    public const string PrimaryKeyName = "PK_NAME";
    public const string PrimaryKeyColumn = "COLUMN_NAME";
  }
}
