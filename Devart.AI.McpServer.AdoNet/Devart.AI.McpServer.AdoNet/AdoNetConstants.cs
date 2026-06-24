// --------------------------------------------------------------------------
// <copyright file="AdoNetConstants.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.AdoNet
{
  internal static class AdoNetConstants
  {
    public const string Tables = "Tables";
    public const string Columns = "Columns";
    public const string Indexes = "Indexes";
    public const string ForeignKeys = "ForeignKeys";
    public const string PrimaryKeys = "PrimaryKeys";
    public const string Functions = "Functions";
    public const string Procedures = "Procedures";

    public const string TableSchema = "schema";
    public const string TableName = "name";
    public const string TableRemarks = "comment";

    public const string ColumnName = "name";
    public const string ColumnTypeName = "typename";
    public const string ColumnSize = "length";
    public const string ColumnNullable = "nullable";
    public const string ColumnRemarks = "comment";

    public const string IndexName = "indexname";
    public const string IndexType = "type";
    public const string IndexColumns = "columns";

    public const string IndexIsUnique = "isunique";
    public const string IndexIsPrimary = "isprimary";

    public const string OrdinalPosition = "position";

    public const string ProcedureSchema = "schema";
    public const string ProcedureName = "name";
    public const string ProcedureType = "type";
    public const string ProcedureParameters = "argumenttypes";

    public const string ForeignKeyFkName = "name";
    public const string ForeignKeyFkColumn = "fkcolumn";
    public const string ForeignKeyPkSchema = "referencedschema";
    public const string ForeignKeyPkTable = "referencedtable";
    public const string ForeignKeyPkColumn = "pkcolumn";
    public const string ForeignKeyUpdateRule = "updaterule";
    public const string ForeignKeyDeleteRule = "deleterule";

    public const string ForeignKeyUpdateAction = "updateactioncode";
    public const string ForeignKeyDeleteAction = "deleteactioncode";
    public const string ForeignKeyFkColumnIndexes = "columnindexes";
    public const string ForeignKeyPkColumnIndexes = "foreigncolumnindexes";

    public const string PrimaryKeyName = "name";
    public const string PrimaryKeyColumn = "pkcolumn";
  }
}
