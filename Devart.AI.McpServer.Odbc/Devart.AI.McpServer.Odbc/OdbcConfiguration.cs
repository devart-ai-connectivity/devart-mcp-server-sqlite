// --------------------------------------------------------------------------
// <copyright file="OdbcConfiguration.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Odbc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Devart.AI.McpServer.Odbc
{
  internal sealed record OdbcConfiguration : McpConfiguration
  {
    [JsonPropertyName("DsnName")]
    public string DsnName { get; init; }

    [JsonIgnore]
    public string DriverUrl { get; init; }

    [JsonIgnore]
    public string DriverDll { get; init; }

    public override string CompleteConnectionString
      => string.IsNullOrWhiteSpace(DsnName)
        ? ConnectionString
        : new OdbcConnectionStringBuilder { Dsn = DsnName }.ConnectionString;

    protected override McpConfiguration Configure(McpAppSettings appSettings)
    {
      var config = base.Configure(appSettings);

      return config is OdbcConfiguration odbcConfig
        ? odbcConfig with
        {
          Driver = string.IsNullOrEmpty(appSettings.Driver) ? odbcConfig.Driver : appSettings.Driver,
          DriverUrl = appSettings.DriverUrl,
          DriverDll = appSettings.DriverDll,
        }
        : config;
    }

    protected override McpConfiguration FromJson(JsonElement json)
      => JsonSerializer.Deserialize<OdbcConfiguration>(json.GetRawText(), JsonOptions);
  }
}