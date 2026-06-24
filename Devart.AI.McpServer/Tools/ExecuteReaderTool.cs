// --------------------------------------------------------------------------
// <copyright file="ExecuteReaderTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public class ExecuteReaderTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    private const int CommandTimeoutSeconds = 60 * 60;

    protected override string Name => "execute_reader";

    protected override string Description => string.Format(McpResources.ExecuteReaderTool_Description, ServerConfiguration.SourceDisplayName);

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      [Description("One or more SQL-92 SELECT statements separated by semicolons.")]
      string sql,
      IServiceProvider services,
      IProgress<ProgressNotificationValue> progress,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(sql, services, progress, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      string sql,
      IServiceProvider services,
      IProgress<ProgressNotificationValue> progress,
      CancellationToken cancellationToken)
    {
      ReportProgress(progress, McpResources.ExecuteReaderTool_ConnectingMessage, 10.0f);

      var sqlStatementsParser = services.GetRequiredService<ISqlStatementsParser>();
      var database = services.GetRequiredService<IDatabase>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var statements = GetStatements(sql, sqlStatementsParser);

      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      using var heartbeatTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      var heartbeatTask = CreateHeartbeatTask(progress, heartbeatTokenSource.Token);
      try
      {
        var result = new StringBuilder(100);
        for (int i = 0; i < statements.Length; i++)
        {
          await using var reader = await database.ExecuteReaderAsync(
            connection,
            statements[i].Text,
            cmd => cmd.CommandTimeout = CommandTimeoutSeconds,
            cancellationToken
          ).ConfigureAwait(false);

          var currentResult = await database.ExecuteOnConnectionAsync(
            connection,
            () => reader.ToMarkdownAsync(cancellationToken)
          ).ConfigureAwait(false);

          if (statements.Length > 1)
          {
            result.AppendLine(string.Format(McpResources.ExecuteReaderTool_ResultSetHeader, i + 1));
          }
          result.AppendLine(currentResult);
        }

        ReportProgress(progress, McpResources.ExecuteReaderTool_CompletedMessage, 100.0f);
        return result.ToString();
      }
      finally
      {
        heartbeatTokenSource.Cancel();
        try
        {
          await heartbeatTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException) { }
      }
    }

    private static Task CreateHeartbeatTask(IProgress<ProgressNotificationValue> progress, CancellationToken cancellationToken)
      => Task.Run(async () =>
        {
          float progressPercent = 20.0f;
          float GetIncrementedProgress() => progressPercent += (99.0f - progressPercent) * 0.1f;

          const int secondsDelay = 30;
          var secondsElapsed = 0;
          while (!cancellationToken.IsCancellationRequested)
          {
            ReportProgress(
              progress,
              secondsElapsed == 0
                ? McpResources.ExecuteReaderTool_ExecutingMessage
                : string.Format(McpResources.ExecuteReaderTool_InProgressMessage, secondsElapsed),
              secondsElapsed == 0
                ? progressPercent
                : GetIncrementedProgress()
            );

            try
            {
              await Task.Delay(TimeSpan.FromSeconds(secondsDelay), cancellationToken);
            }
            catch (OperationCanceledException) { }

            secondsElapsed += secondsDelay;
          }
        }, CancellationToken.None);

    private static void ReportProgress(IProgress<ProgressNotificationValue> progress, string message, float progressValue)
    {
      progress?.Report(new ProgressNotificationValue
      {
        Message = message,
        Progress = progressValue
      });
    }

    private static SqlStatement[] GetStatements(string sql, ISqlStatementsParser parser)
    {
      var statements = parser.Parse(sql);

      if (statements.Length == 0)
      {
        throw new McpProtocolException(McpResources.Common_NoSqlStatementsProvided, McpErrorCode.InvalidParams);
      }

      for (int i = 0; i < statements.Length; i++)
      {
        if (statements[i].Type != StatementType.Select)
        {
          throw new McpProtocolException(
            string.Format(
              McpResources.ExecuteReaderTool_InvalidStatementTypeError,
              i + 1,
              statements[i].Type.ToString().ToUpper()
            ),
            McpErrorCode.InvalidParams
          );
        }
      }

      return statements;
    }
  }
}
