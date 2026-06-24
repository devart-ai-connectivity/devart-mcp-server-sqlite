// --------------------------------------------------------------------------
// <copyright file="ExecuteRoutineTool.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Tools
{
  public class ExecuteRoutineTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    private readonly HashSet<string> allowedRoutineTypes = [.. serverConfiguration.SupportedRoutines];

    private string AllowedRoutineTypesFormatted => $"'{string.Join("' or '", allowedRoutineTypes)}'";

    protected override string Name => "execute_routine";

    protected override string Description
      => string.Format(
        McpResources.ExecuteRoutineTool_Description,
        ServerConfiguration.SourceDisplayName,
        AllowedRoutineTypesFormatted
      );

    protected override Delegate ExecuteDefinition => Execute;

    public Task<string> Execute(
      [Description("Name of the schema.")]
      string schema,
      [Description("Name of the routine.")]
      string routineName,
      [Description($"Type of the routine.")]
      string routineType,
      [Description("Dictionary of parameter ordinal positions and their values.")]
      Dictionary<int, object> parameters,
      IServiceProvider services,
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(schema, routineName, routineType, parameters, services, cancellationToken));

    protected virtual async Task<string> ExecuteAsync(
      string schema,
      string routineName,
      string routineType,
      Dictionary<int, object> parameters,
      IServiceProvider services,
      CancellationToken cancellationToken)
    {
      var database = services.GetRequiredService<IDatabase>();
      var formatter = services.GetRequiredService<ISqlFormatter>();
      var commandHelper = services.GetRequiredService<ICommandHelper>();
      var configuration = services.GetService<McpConfiguration>() ?? ServerConfiguration;

      var normalizedRoutineType = ValidateRoutineType(routineType);
      var connection = await database.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      var routineFullName = formatter.FormatName(schema, routineName, configuration, connection);

      await using var command = connection.CreateCommand();
      await InitializeParametersAsync(command, commandHelper, parameters, database, connection, cancellationToken).ConfigureAwait(false);

      return normalizedRoutineType switch
      {
        McpConstants.ProcedureRoutine => await ExecuteProcedure(database, connection, command, formatter, routineFullName, cancellationToken),
        McpConstants.FunctionRoutine => await ExecuteFunction(database, connection, command, commandHelper, formatter, routineFullName, cancellationToken),
        _ => throw new NotImplementedException(string.Format(McpResources.ExecuteRoutineTool_RoutineTypeNotSupported, normalizedRoutineType))
      };
    }

    private string ValidateRoutineType(string routineType)
      => allowedRoutineTypes.FirstOrDefault(t => t.Equals(routineType?.Trim(), StringComparison.OrdinalIgnoreCase))
        ?? throw new McpProtocolException(
          string.Format(
            McpResources.ExecuteRoutineTool_InvalidRoutineTypeError,
            routineType,
            AllowedRoutineTypesFormatted
          ),
          McpErrorCode.InvalidParams
        );

    private static async Task InitializeParametersAsync(
      DbCommand command,
      ICommandHelper commandHelper,
      Dictionary<int, object> parameters,
      IDatabase database,
      DbConnection connection,
      CancellationToken cancellationToken)
    {
      foreach (var parameter in parameters.OrderBy(p => p.Key))
      {
        var value = database.NormalizeParameterValue(parameter.Value);
        commandHelper.AddParameter(command, value);
      }
      await database.ExecuteOnConnectionAsync(
        connection,
        () => command.PrepareAsync(cancellationToken)
      ).ConfigureAwait(false);
    }

    private static async Task<string> ExecuteProcedure(
      IDatabase database,
      DbConnection connection,
      DbCommand command,
      ISqlFormatter formatter,
      string routineFullName,
      CancellationToken cancellationToken)
    {
      command.CommandText = formatter.FormatCallProcedure(routineFullName, command.Parameters.Count);
      await database.ExecuteOnConnectionAsync(
        connection,
        () => command.ExecuteNonQueryAsync(cancellationToken)
      ).ConfigureAwait(false);

      return McpResources.ExecuteRoutineTool_ProcedureSuccessMessage;
    }

    private static async Task<string> ExecuteFunction(
      IDatabase database,
      DbConnection connection,
      DbCommand command,
      ICommandHelper commandHelper,
      ISqlFormatter formatter,
      string routineFullName,
      CancellationToken cancellationToken)
    {
      command.CommandText = formatter.FormatCallFunction(routineFullName, command.Parameters.Count);
      var resultParameter = commandHelper.AddResultParameter(command);

      await database.ExecuteOnConnectionAsync(
        connection,
        () => command.ExecuteNonQueryAsync(cancellationToken)
      ).ConfigureAwait(false);

      var returnValueString = MarkdownTableFormatter.FormatTableValue(resultParameter.Value);
      return string.Format(McpResources.ExecuteRoutineTool_FunctionSuccessMessage, returnValueString);
    }
  }
}
