// --------------------------------------------------------------------------
// <copyright file="OdbcSqliteRunCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using Devart.AI.McpServer.Odbc.CommandLine;
using Devart.AI.McpServer.Odbc.Sqlite.Properties;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Devart.AI.McpServer.Odbc.Sqlite.CommandLine
{
  internal sealed class OdbcSqliteRunCommand : OdbcRunCommand
  {
    protected override void RegisterTools(IMcpServerBuilder serverBuilder, McpConfiguration configuration)
      => serverBuilder.WithTools(OdbcSqliteTools.CreateTools(configuration));

    public override string ProductFullName => ProductInfo.ProductFullName;

    public override string ProductId => ProductInfo.ProductId;

    protected override McpAppSettings CreateAppSettings() => new OdbcSqliteAppSettings();
  }
}