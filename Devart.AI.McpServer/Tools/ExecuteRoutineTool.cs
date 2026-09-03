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
using Devart.AI.McpServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;

namespace Devart.AI.McpServer.Tools
{
  public class ExecuteRoutineTool(McpConfiguration serverConfiguration) : McpTool(serverConfiguration)
  {
    private readonly HashSet<string> allowedRoutineTypes = [.. serverConfiguration.SupportedRoutines];

    private string AllowedRoutineTypesFormatted => $"'{string.Join("' or '", this.allowedRoutineTypes)}'";

    protected override string Name => "execute_routine";

    public override bool IsApplicable(McpConfiguration configuration)
      => configuration.SupportedRoutines is { Count: > 0 };

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
      CancellationToken cancellationToken) => DoActionAsync(() => ExecuteAsync(schema, routineName, routineType, parameters, services, cancellationToken), services);

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

      AddParameters(command, commandHelper, parameters, database);

      return normalizedRoutineType switch
      {
        McpConstants.ProcedureRoutine => await ExecuteProcedure(database, connection, command, formatter, routineFullName, cancellationToken),
        McpConstants.FunctionRoutine => await ExecuteFunction(database, connection, command, commandHelper, formatter, routineFullName, cancellationToken),
        _ => throw new NotImplementedException(string.Format(McpResources.ExecuteRoutineTool_RoutineTypeNotSupported, normalizedRoutineType))
      };
    }

    private string ValidateRoutineType(string routineType)
      => this.allowedRoutineTypes.FirstOrDefault(t => t.Equals(routineType?.Trim(), StringComparison.OrdinalIgnoreCase))
        ?? throw new McpProtocolException(
          string.Format(
            McpResources.ExecuteRoutineTool_InvalidRoutineTypeError,
            routineType,
            AllowedRoutineTypesFormatted
          ),
          McpErrorCode.InvalidParams
        );

    private static void AddParameters(
      DbCommand command,
      ICommandHelper commandHelper,
      Dictionary<int, object> parameters,
      IDatabase database)
    {
      foreach (var parameter in parameters.OrderBy(p => p.Key))
      {
        var value = database.NormalizeParameterValue(parameter.Value);
        commandHelper.AddParameter(command, value);
      }
    }

    private static Task PrepareAsync(
      IDatabase database,
      DbConnection connection,
      DbCommand command,
      CancellationToken cancellationToken)
      => database.ExecuteOnConnectionAsync(
        connection,
        () => command.PrepareAsync(cancellationToken)
      );

    private static async Task<string> ExecuteProcedure(
      IDatabase database,
      DbConnection connection,
      DbCommand command,
      ISqlFormatter formatter,
      string routineFullName,
      CancellationToken cancellationToken)
    {
      command.CommandText = formatter.FormatCallProcedure(routineFullName, command.Parameters.Count);
      await PrepareAsync(database, connection, command, cancellationToken).ConfigureAwait(false);

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
      await PrepareAsync(database, connection, command, cancellationToken).ConfigureAwait(false);

      object returnValue;
      if (resultParameter is null)
      {
        returnValue = await database.ExecuteOnConnectionAsync(
          connection,
          () => command.ExecuteScalarAsync(cancellationToken)
        ).ConfigureAwait(false);
      }
      else
      {
        await database.ExecuteOnConnectionAsync(
          connection,
          () => command.ExecuteNonQueryAsync(cancellationToken)
        ).ConfigureAwait(false);
        returnValue = resultParameter.Value;
      }

      var returnValueString = MarkdownTableFormatter.FormatTableValue(returnValue);
      return string.Format(McpResources.ExecuteRoutineTool_FunctionSuccessMessage, returnValueString);
    }
  }
}