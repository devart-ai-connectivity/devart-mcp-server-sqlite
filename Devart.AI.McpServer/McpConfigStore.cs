// --------------------------------------------------------------------------
// <copyright file="McpConfigStore.cs" company="Devart">
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
using System.Text.Json.Nodes;

namespace Devart.AI.McpServer
{
  internal sealed class McpConfigStore
  {
    private const string ConnectionsProperty = "Connections";
    private const string IdProperty = "Id";
    private const string NameProperty = "Name";
    private const string ProtocolTypeProperty = "ProtocolType";
    private const string HttpAddressProperty = "HttpAddress";
    private const string HttpPortProperty = "HttpPort";
    private const string HttpRoutePrefixProperty = "HttpRoutePrefix";
    private const string ConnectionStringProperty = "ConnectionString";

    private const int DefaultHttpPort = 8080;

    private static readonly JsonNodeOptions NodeOptions = new()
    {
      PropertyNameCaseInsensitive = true
    };

    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
      CommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
      WriteIndented = true
    };

    private readonly JsonObject root;
    private readonly JsonArray connections;

    private McpConfigStore(string filePath, JsonObject root, JsonArray connections)
    {
      FilePath = filePath;
      this.root = root;
      this.connections = connections;
    }

    public string FilePath { get; }

    public static McpConfigStore Open(string filePath)
    {
      var root = Parse(filePath);

      if (!root.TryGetPropertyValue(ConnectionsProperty, out var node))
      {
        var created = new JsonArray();
        root[ConnectionsProperty] = created;

        return new McpConfigStore(filePath, root, created);
      }

      return node is JsonArray connections
        ? new McpConfigStore(filePath, root, connections)
        : throw new ArgumentException(McpResources.Common_ConfigFileInvalid);
    }

    public bool Contains(string profileName) => Find(profileName) is not null;

    public IReadOnlyList<McpConfigProfile> List()
      => [.. this.connections
        .OfType<JsonObject>()
        .Select(connection => new McpConfigProfile(
          GetString(connection, NameProperty),
          GetString(connection, ProtocolTypeProperty),
          GetString(connection, HttpAddressProperty),
          GetInt32(connection, HttpPortProperty),
          GetString(connection, HttpRoutePrefixProperty)))];

    public void Add(string profileName, McpConfigValues values)
    {
      var profile = new JsonObject(NodeOptions)
      {
        [IdProperty] = Guid.NewGuid().ToString(),
        [NameProperty] = profileName,
        [ProtocolTypeProperty] = (values.ProtocolType ?? McpProtocolType.Stdio).ToString(),
      };

      if (values.HttpAddress is not null)
      {
        profile[HttpAddressProperty] = values.HttpAddress;
      }

      profile[HttpPortProperty] = values.HttpPort ?? DefaultHttpPort;

      if (values.HttpRoutePrefix is not null)
      {
        profile[HttpRoutePrefixProperty] = values.HttpRoutePrefix;
      }

      profile[ConnectionStringProperty] = values.ConnectionString;
      this.connections.Add(profile);
    }

    public Guid Set(string profileName, McpConfigValues values)
    {
      if (!Update(profileName, values))
      {
        Add(profileName, values);
      }

      return Guid.TryParse(GetString(Find(profileName), IdProperty), out var id) ? id : Guid.NewGuid();
    }

    public bool Update(string profileName, McpConfigValues values)
    {
      var profile = Find(profileName);
      if (profile is null)
      {
        return false;
      }

      Apply(profile, values);

      return true;
    }

    public bool Remove(string profileName)
    {
      var profile = Find(profileName);

      return profile is not null && this.connections.Remove(profile);
    }

    public void Save() => File.WriteAllText(FilePath, this.root.ToJsonString(WriteOptions));

    private static void Apply(JsonObject profile, McpConfigValues values)
    {
      if (values.ProtocolType is not null)
      {
        profile[ProtocolTypeProperty] = values.ProtocolType.Value.ToString();
      }

      if (values.HttpAddress is not null)
      {
        profile[HttpAddressProperty] = values.HttpAddress;
      }

      if (values.HttpPort is not null)
      {
        profile[HttpPortProperty] = values.HttpPort.Value;
      }

      if (values.HttpRoutePrefix is not null)
      {
        profile[HttpRoutePrefixProperty] = values.HttpRoutePrefix;
      }

      if (values.ConnectionString is not null)
      {
        profile[ConnectionStringProperty] = values.ConnectionString;
      }
    }

    private JsonObject Find(string profileName)
      => this.connections
        .OfType<JsonObject>()
        .FirstOrDefault(connection => string.Equals(GetString(connection, NameProperty), profileName, StringComparison.OrdinalIgnoreCase));

    private static JsonObject Parse(string filePath)
    {
      if (!File.Exists(filePath))
      {
        return new JsonObject(NodeOptions);
      }

      var text = File.ReadAllText(filePath);
      if (string.IsNullOrWhiteSpace(text))
      {
        return new JsonObject(NodeOptions);
      }

      try
      {
        return JsonNode.Parse(text, NodeOptions, DocumentOptions) as JsonObject
          ?? throw new ArgumentException(McpResources.Common_ConfigFileInvalid);
      }
      catch (JsonException)
      {
        throw new ArgumentException(McpResources.Common_ConfigFileInvalid);
      }
    }

    private static string GetString(JsonObject connection, string propertyName)
      => connection is not null
        && connection.TryGetPropertyValue(propertyName, out var node)
        && node is JsonValue value
        && value.TryGetValue<string>(out var text)
          ? text
          : null;

    private static int? GetInt32(JsonObject connection, string propertyName)
      => connection.TryGetPropertyValue(propertyName, out var node)
        && node is JsonValue value
        && value.TryGetValue<int>(out var number)
          ? number
          : null;
  }
}