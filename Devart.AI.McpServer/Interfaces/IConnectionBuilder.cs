// --------------------------------------------------------------------------
// <copyright file="IConnectionBuilder.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Devart.AI.McpServer.Interfaces
{
  public interface IConnectionBuilder
  {
    Task<DbConnection> CreateConnectionAsync(McpConfiguration configuration, CancellationToken cancellationToken);
  }
}
