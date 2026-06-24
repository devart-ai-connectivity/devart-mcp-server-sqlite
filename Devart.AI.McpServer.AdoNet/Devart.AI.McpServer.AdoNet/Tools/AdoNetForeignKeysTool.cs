// --------------------------------------------------------------------------
// <copyright file="AdoNetForeignKeysTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Interfaces;
using Devart.AI.McpServer.Tools;

namespace Devart.AI.McpServer.AdoNet.Tools
{
  internal sealed class AdoNetForeignKeysTool(McpConfiguration serverConfiguration) : ForeignKeysTool(serverConfiguration)
  {
    protected override async Task<DataTable> GetMetadataTable(
      DbConnection connection, 
      string schema, 
      string tableName, 
      IServiceProvider services, 
      CancellationToken cancellationToken)
    {
      var resultTable = await base.GetMetadataTable(connection, schema, tableName, services, cancellationToken).ConfigureAwait(false);

      resultTable.Columns.Add(AdoNetConstants.ForeignKeyFkName, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ForeignKeyFkColumn, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ForeignKeyPkSchema, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ForeignKeyPkTable, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ForeignKeyPkColumn, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ForeignKeyUpdateRule, typeof(string));
      resultTable.Columns.Add(AdoNetConstants.ForeignKeyDeleteRule, typeof(string));

      var metadata = services.GetService<IMetadata>();
      var foreign_keys = await connection.GetSchemaAsync(
        metadata.ForeignKeysCollectionName,
        [metadata.SchemaName(schema), tableName],
        cancellationToken
      ).ConfigureAwait(false);

      foreach (DataRow key in foreign_keys.Rows)
      {
        var keyName = key[AdoNetConstants.ForeignKeyFkName]?.ToString();
        var pkSchema = key[AdoNetConstants.ForeignKeyPkSchema]?.ToString();
        var pkTable = key[AdoNetConstants.ForeignKeyPkTable]?.ToString();
        var updateAction = GetActionName(key[AdoNetConstants.ForeignKeyUpdateAction]?.ToString());
        var deleteAction = GetActionName(key[AdoNetConstants.ForeignKeyDeleteAction]?.ToString());

        var keyColumnIndexes = key[AdoNetConstants.ForeignKeyFkColumnIndexes]?.ToString();
        var fkColumnNames = await this.GetColumnNames(
          keyColumnIndexes,
          connection,
          schema, tableName,
          services,
          cancellationToken
        ).ConfigureAwait(false);

        keyColumnIndexes = key[AdoNetConstants.ForeignKeyPkColumnIndexes]?.ToString();
        var pkColumnNames = await this.GetColumnNames(
          keyColumnIndexes,
          connection,
          pkSchema, pkTable,
          services,
          cancellationToken
        ).ConfigureAwait(false);

        resultTable.Rows.Add(keyName, fkColumnNames, pkSchema, pkTable, pkColumnNames, updateAction, deleteAction);
      }

      return resultTable;
    }

    private static string GetActionName(string actionCode) => actionCode switch
    {
      "a" => "NO ACTION",
      "r" => "RESTRICT",
      "c" => "CASCADE",
      "n" => "SET NULL",
      "d" => "SET DEFAULT",
      _ => "RESTRICT"
    };
  }
}
