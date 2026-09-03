// --------------------------------------------------------------------------
// <copyright file="OdbcSqliteAppSettings.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer.Odbc.Sqlite
{
  internal sealed class OdbcSqliteAppSettings : McpAppSettings
  {
    public override string ServerName => $"Devart {Properties.ProductInfo.ProductFullName}";

    public override string SourceName => "SQLite";

    public override bool OnPremise => true;

    public override string[] Routines => [];

    public override string Driver => Properties.ProductInfo.Driver;

    public override string DriverUrl => Properties.ProductInfo.DriverUrl;


    public override string DriverDll => Properties.ProductInfo.DriverDll;
  }
}