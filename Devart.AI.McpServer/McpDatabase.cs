// --------------------------------------------------------------------------
// <copyright file="McpDatabase.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data.Common;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Devart.AI.McpServer.Extensions;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer
{
  public sealed class McpDatabase : IDatabase
  {
    private readonly SemaphoreSlim connectionLock = new(1, 1);
    private DbConnection connection;
    private bool disposed;

    public McpDatabase()
    {
      AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }

    public async Task<DbConnection> OpenConnectionAsync(
      McpConfiguration configuration,
      IServiceProvider services,
      CancellationToken cancellationToken = default)
    {
      var pool = services.GetService<IConnectionPool>();
      if (pool is not null) {
        return await pool.OpenConnectionAsync(configuration, services, cancellationToken).ConfigureAwait(false);
      }

      var builder = services.GetService<IConnectionBuilder>() ?? throw new NotImplementedException("IConnectionBuilder not implemented");

      await connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
      try
      {
        if (!ConnectionIsAlive(connection))
        {
          if (connection is not null)
          {
            try
            {
              await connection.DisposeAsync().ConfigureAwait(false);
            }
            catch
            {
            }
            connection = null;
          }

          connection = await builder.CreateConnectionAsync(configuration, cancellationToken).ConfigureAwait(false);
        }
        return connection;
      }
      finally
      {
        connectionLock.Release();
      }
    }

    public void Dispose()
    {
      if (!disposed)
      {
        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        DisposeSync();
        connectionLock.Dispose();
      }
      GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
      if (!disposed)
      {
        AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        await DisposeAsyncCore().ConfigureAwait(false);
        connectionLock.Dispose();
        disposed = true;
      }
      GC.SuppressFinalize(this);
    }

    public static bool ConnectionIsAlive(DbConnection connection)
      => connection is not null &&
        (connection.State is System.Data.ConnectionState.Open
                          or System.Data.ConnectionState.Fetching
                          or System.Data.ConnectionState.Executing);

    public Task<DbDataReader> ExecuteReaderAsync(
      DbConnection connection,
      string commandText,
      Action<DbCommand> configureCommand = null,
      CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(connection, nameof(connection));
      ArgumentException.ThrowIfNullOrEmpty(commandText, nameof(commandText));

      return ExecuteOnConnectionAsync(connection, () =>
      {
        var command = connection.CreateCommand();
        command.CommandText = commandText;
        configureCommand?.Invoke(command);
        return command.ExecuteReaderAsync(cancellationToken);
      });
    }

    public async Task<T> ExecuteOnConnectionAsync<T>(DbConnection connection, Func<Task<T>> operation)
    {
      ArgumentNullException.ThrowIfNull(connection, nameof(connection));
      ArgumentNullException.ThrowIfNull(operation, nameof(operation));

      try
      {
        return await operation().ConfigureAwait(false);
      }
      catch (Exception)
      {
        await TryInvalidateOnFailureAsync(connection).ConfigureAwait(false);
        throw;
      }
    }

    public async Task ExecuteOnConnectionAsync(DbConnection connection, Func<Task> operation)
    {
      ArgumentNullException.ThrowIfNull(connection, nameof(connection));
      ArgumentNullException.ThrowIfNull(operation, nameof(operation));

      try
      {
        await operation().ConfigureAwait(false);
      }
      catch (Exception)
      {
        await TryInvalidateOnFailureAsync(connection).ConfigureAwait(false);
        throw;
      }
    }

    public Task InvalidateConnectionAsync(DbConnection connection)
    {
      ArgumentNullException.ThrowIfNull(connection, nameof(connection));
      return InvalidateCachedConnectionAsync(connection);
    }

    public object NormalizeParameterValue(object value)
      => value is null
        ? DBNull.Value
        : value is JsonElement jsonElement
          ? jsonElement.ConvertToClrType()
          : value is System.Text.Json.Nodes.JsonValue jsonValue && jsonValue.TryGetValue(out JsonElement nestedJsonElement)
            ? nestedJsonElement.ConvertToClrType()
            : value;

    private async Task TryInvalidateOnFailureAsync(DbConnection connection)
    {
      try
      {
        await InvalidateCachedConnectionAsync(connection).ConfigureAwait(false);
      }
      catch
      {
      }
    }

    private async Task InvalidateCachedConnectionAsync(DbConnection failedConnection)
    {
      await connectionLock.WaitAsync().ConfigureAwait(false);
      try
      {
        if (!ReferenceEquals(connection, failedConnection))
        {
          return;
        }

        var toDispose = connection;
        connection = null;
        try
        {
          await toDispose.DisposeAsync().ConfigureAwait(false);
        }
        catch
        {
        }
      }
      finally
      {
        connectionLock.Release();
      }
    }

    private void OnProcessExit(object sender, EventArgs e)
    {
      DisposeSync();
    }

    private void DisposeSync()
    {
      if (connection is not null)
      {
        try
        {
          connection.Close();
          connection.Dispose();
        }
        finally
        {
          connection = null;
          disposed = true;
        }
      }
    }

    private async ValueTask DisposeAsyncCore()
    {
      if (connection is not null)
      {
        try
        {
          connection.Close();
          await connection.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
          connection = null;
          disposed = true;
        }
      }
    }
  }
}
