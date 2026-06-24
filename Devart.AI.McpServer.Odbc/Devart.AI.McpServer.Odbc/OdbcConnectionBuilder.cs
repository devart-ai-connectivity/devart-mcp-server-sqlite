// --------------------------------------------------------------------------
// <copyright file="OdbcConnectionBuilder.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Common;
using System.Data.Odbc;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Odbc
{
  internal sealed class OdbcConnectionBuilder : IConnectionBuilder
  {
    public async Task<DbConnection> CreateConnectionAsync(McpConfiguration configuration, CancellationToken cancellationToken)
    {
      var connection = new OdbcConnection(configuration.CompleteConnectionString);
      await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
      return connection;
    }
  }
}
