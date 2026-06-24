// --------------------------------------------------------------------------
// <copyright file="McpShutdownEndpointExtensions.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Devart.AI.McpServer.Shutdown
{
  internal static class McpShutdownEndpointExtensions
  {
    public static IEndpointRouteBuilder MapShutdownEndpoint(this IEndpointRouteBuilder endpoints, string path = "/admin/shutdown")
    {
      endpoints.MapPost(path, async context =>
      {
        var shutdownService = context.RequestServices.GetRequiredService<McpShutdownService>();

        if (!context.Request.Headers.TryGetValue("X-Shutdown-Token", out var tokenValue))
        {
          context.Response.StatusCode = 401;
          return;
        }

        if (!shutdownService.ValidateToken(tokenValue.ToString()))
        {
          context.Response.StatusCode = 403;
          return;
        }

        ShutdownRequest request = null;
        if (context.Request.ContentLength > 0)
        {
          request = await context.Request.ReadFromJsonAsync<ShutdownRequest>();
        }

        var response = new ShutdownResponse
        {
          Status = "shutting_down",
          Message = "Server will stop accepting new connections",
          InitiatedAt = DateTimeOffset.UtcNow,
        };

        context.Response.StatusCode = 202;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
        await context.Response.CompleteAsync();

        shutdownService.Shutdown();
      });

      return endpoints;
    }
  }

  internal sealed class ShutdownRequest
  {
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
  }

  internal sealed class ShutdownResponse
  {
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("initiatedAt")]
    public DateTimeOffset InitiatedAt { get; set; }
  }
}
