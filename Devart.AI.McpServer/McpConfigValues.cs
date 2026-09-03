// --------------------------------------------------------------------------
// <copyright file="McpConfigValues.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer
{
  internal sealed record McpConfigValues(
    McpProtocolType? ProtocolType,
    string HttpAddress,
    int? HttpPort,
    string HttpRoutePrefix,
    string ConnectionString)
  {
    public bool IsEmpty
      => ProtocolType is null
        && HttpAddress is null
        && HttpPort is null
        && HttpRoutePrefix is null
        && ConnectionString is null;
  }
}