// --------------------------------------------------------------------------
// <copyright file="IDatabase.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.AI.McpServer.Interfaces
{
  public interface IDatabase : IAsyncDisposable, IDisposable
  {
    Task<DbConnection> OpenConnectionAsync(
      McpConfiguration configuration,
      IServiceProvider services,
      CancellationToken cancellationToken = default);

    Task<DbDataReader> ExecuteReaderAsync(
      DbConnection connection,
      string commandText,
      Action<DbCommand> configureCommand = null,
      CancellationToken cancellationToken = default);

    Task<T> ExecuteOnConnectionAsync<T>(DbConnection connection, Func<Task<T>> operation);

    Task ExecuteOnConnectionAsync(DbConnection connection, Func<Task> operation);

    Task InvalidateConnectionAsync(DbConnection connection);

    object NormalizeParameterValue(object value);
  }
}
