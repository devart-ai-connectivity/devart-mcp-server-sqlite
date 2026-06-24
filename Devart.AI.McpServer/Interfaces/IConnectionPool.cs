// --------------------------------------------------------------------------
// <copyright file="IConnectionPool.cs" company="Devart">
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
  public interface IConnectionPool
  {
    Task<DbConnection> OpenConnectionAsync(McpConfiguration configuration, IServiceProvider services, CancellationToken cancellationToken);
  }
}
