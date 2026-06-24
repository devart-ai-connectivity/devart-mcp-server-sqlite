// --------------------------------------------------------------------------
// <copyright file="McpProtocolType.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Devart.AI.McpServer
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum McpProtocolType
  {
    [JsonPropertyName("stdio")] Stdio,
    [JsonPropertyName("http")] Http
  }
}
