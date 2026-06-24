// --------------------------------------------------------------------------
// <copyright file="McpConfiguration.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Devart.AI.McpServer
{
  public record McpConfiguration
  {
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
      PropertyNameCaseInsensitive = true,
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
    };

    private static readonly JsonDocumentOptions JsonDocOptions = new()
    {
      CommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
    };

    [JsonPropertyName("Id")]
    public Guid Id { get; init; }

    [JsonPropertyName("Name")]
    public string Name { get; init; }

    [JsonPropertyName("Driver")]
    public string Driver { get; init; }

    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; init; }

    [JsonPropertyName("ProtocolType")]
    public McpProtocolType ProtocolType { get; init; }

    [JsonPropertyName("HttpPort")]
    public int HttpPort { get; init; }

    [JsonIgnore]
    public string ToolPrefix { get; init; }

    [JsonIgnore]
    public string ServerName { get; init; }

    [JsonIgnore]
    public string SourceName { get; init; }

    [JsonIgnore]
    public string SourceDisplayName { get; init; }

    [JsonIgnore]
    public bool OnPremise { get; init; }

    [JsonIgnore]
    public HashSet<StatementType> NonQueryOperations { get; init; }

    [JsonIgnore]
    public HashSet<string> SupportedRoutines { get; init; }

    [JsonIgnore]
    public HashSet<string> IgnoreSchemas { get; init; }

    [JsonIgnore]
    public string OpenQuote { get; init; }

    [JsonIgnore]
    public string CloseQuote { get; init; }

    public virtual string CompleteConnectionString => ConnectionString;

    protected virtual McpConfiguration FromJson(JsonElement json)
      => JsonSerializer.Deserialize<McpConfiguration>(json.GetRawText(), JsonOptions);

    public virtual McpConfiguration Load(string configPath, string configName, McpAppSettings appSettings)
    {
      if (configPath is null || !Path.Exists(configPath))
      {
        throw new ArgumentException(string.Format(McpResources.CommandLine_ConfigFileNotFound, configPath));
      }

      var doc = JsonDocument.Parse(File.ReadAllText(configPath), JsonDocOptions);
      if (!doc.RootElement.TryGetProperty("Connections", out var connections))
      {
        throw new ArgumentException(McpResources.Common_ConfigFileInvalid);
      }

      var connection = connections.EnumerateArray()
        .FirstOrDefault(e => e.TryGetProperty(nameof(Name), out var n) && string.Equals(n.GetString(), configName, StringComparison.OrdinalIgnoreCase));
      if (connection.ValueKind == JsonValueKind.Undefined)
      {
        throw new ArgumentException(string.Format(McpResources.Common_ConfigInvalid, configName));
      }

      var config = FromJson(connection);

      return config with
      {
        ToolPrefix = string.IsNullOrEmpty(appSettings.ToolPrefix) ? config.Name.Replace(" ", "_") : appSettings.ToolPrefix,
        ServerName = appSettings.ServerName,
        SourceName = appSettings.SourceName,
        SourceDisplayName = appSettings.OnPremise
          ? $"the {appSettings.SourceName} database"
          : appSettings.SourceName,
        OnPremise = appSettings.OnPremise,
        NonQueryOperations = appSettings.NonQueryOperations != null
          ? [.. appSettings.NonQueryOperations.Select(s => Enum.Parse<StatementType>(s, ignoreCase: true))]
          : [],
        SupportedRoutines = appSettings.Routines != null
          ? [.. appSettings.Routines]
          : [],
        IgnoreSchemas = appSettings.IgnoreSchemas != null
          ? [.. appSettings.IgnoreSchemas]
          : null,
        OpenQuote = appSettings.SqlFormatter?.OpenQuote ?? "\"",
        CloseQuote = appSettings.SqlFormatter?.CloseQuote ?? "\"",
      };
    }
  }
}
