// --------------------------------------------------------------------------
// <copyright file="McpConfigProfile.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer
{
  internal sealed record McpConfigProfile(
    string Name,
    string ProtocolType,
    string HttpAddress,
    int? HttpPort,
    string HttpRoutePrefix);
}