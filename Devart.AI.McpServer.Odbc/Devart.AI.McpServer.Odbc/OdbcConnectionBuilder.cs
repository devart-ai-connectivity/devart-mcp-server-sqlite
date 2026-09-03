// --------------------------------------------------------------------------
// <copyright file="OdbcConnectionBuilder.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data.Common;
using System.Data.Odbc;
using System.Threading;
using System.Threading.Tasks;
using Devart.AI.McpServer.Interfaces;
using ModelContextProtocol;

namespace Devart.AI.McpServer.Odbc
{
  internal sealed class OdbcConnectionBuilder : IConnectionBuilder
  {
    private const string DataSourceNotFoundSqlState = "IM002";

    public async Task<DbConnection> CreateConnectionAsync(McpConfiguration configuration, CancellationToken cancellationToken)
    {
      var connection = new OdbcConnection(configuration.CompleteConnectionString);
      try
      {
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
      }
      catch (OdbcException ex) when (IsDataSourceNotFound(ex) && GetDriverNotInstalledHint(configuration) is { } hint)
      {
        var originalMessage = ex.Message.TrimEnd();
        var separator = originalMessage.EndsWith('.') ? " " : ". ";
        throw new McpException($"{originalMessage}{separator}{hint}", ex);
      }

      if (GetUnsupportedDriverError(connection, configuration) is { } error)
      {
        await connection.DisposeAsync().ConfigureAwait(false);
        throw new McpException(error);
      }

      return connection;
    }

    private static string GetUnsupportedDriverError(OdbcConnection connection, McpConfiguration configuration)
    {
      if (configuration is not OdbcConfiguration odbcConfiguration || string.IsNullOrEmpty(odbcConfiguration.DriverDll))
      {
        return null;
      }

      var actualDriverDll = connection.Driver;

      return string.Equals(actualDriverDll, odbcConfiguration.DriverDll, StringComparison.OrdinalIgnoreCase)
        ? null
        : string.Format(
            OdbcResources.Connection_UnsupportedDriver,
            odbcConfiguration.Driver,
            odbcConfiguration.DriverUrl);
    }

    private static bool IsDataSourceNotFound(OdbcException exception)
    {
      foreach (OdbcError error in exception.Errors)
      {
        if (string.Equals(error.SQLState, DataSourceNotFoundSqlState, StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }

      return false;
    }

    private static string GetDriverNotInstalledHint(McpConfiguration configuration)
      => configuration is OdbcConfiguration odbcConfiguration
        && !string.IsNullOrEmpty(odbcConfiguration.Driver)
        && !string.IsNullOrEmpty(odbcConfiguration.DriverUrl)
          ? string.Format(
              OdbcResources.Connection_DriverNotInstalled,
              odbcConfiguration.Driver,
              odbcConfiguration.DriverUrl)
          : null;
  }
}