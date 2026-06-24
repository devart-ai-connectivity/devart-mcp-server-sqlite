// --------------------------------------------------------------------------
// <copyright file="ExecuteNonQueryTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public class ExecuteNonQueryTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    private readonly HashSet<StatementType> allowedOperations = [.. serverConfiguration.NonQueryOperations];

    protected override string Name => "execute_nonquery";

    protected override string Description
      => string.Format(
        McpResources.ExecuteNonQueryTool_Description,
        ServerConfiguration.SourceDisplayName,
        string.Join(", ", allowedOperations).ToUpper()
      );

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      [Description("One or more SQL-92 statements with supported operations.")]
      string sql,
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(sql, services, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      string sql,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var sqlStatementsParser = services.GetRequiredService<ISqlStatementsParser>();
      var statements = sqlStatementsParser.Parse(sql);

      if (statements.Length == 0)
      {
        throw new McpProtocolException(McpResources.Common_NoSqlStatementsProvided, McpErrorCode.InvalidParams);
      }

      for (int i = 0; i < statements.Length; i++)
      {
        if (!allowedOperations.Contains(statements[i].Type))
        {
          throw new McpProtocolException(
            string.Format(
              McpResources.ExecuteNonQueryTool_InvalidStatementTypeError,
              i + 1,
              statements[i].Type.ToString().ToUpper(),
              string.Join("', '", allowedOperations).ToUpper()
            ),
            McpErrorCode.InvalidParams
          );
        }
      }

      var database = services.GetRequiredService<IDatabase>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);

      await using var transaction = await database.ExecuteOnConnectionAsync(
        connection,
        () => connection.BeginTransactionAsync(cancellationToken).AsTask()
      ).ConfigureAwait(false);

      await using var command = connection.CreateCommand();
      command.Transaction = transaction;

      int rowsAffected = 0;
      try
      {
        foreach (var statement in statements)
        {
          command.CommandText = statement.Text;
          rowsAffected += await database.ExecuteOnConnectionAsync(
            connection,
            () => command.ExecuteNonQueryAsync(cancellationToken)
          ).ConfigureAwait(false);
        }
        await database.ExecuteOnConnectionAsync(
          connection,
          () => transaction.CommitAsync(cancellationToken)
        ).ConfigureAwait(false);
      }
      catch
      {
        try
        {
          await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
        }
        throw;
      }

      return string.Format(McpResources.ExecuteNonQueryTool_SuccessMessage, rowsAffected);
    }
  }
}
