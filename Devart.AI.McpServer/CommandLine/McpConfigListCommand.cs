// --------------------------------------------------------------------------
// <copyright file="McpConfigListCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;

namespace Devart.AI.McpServer.CommandLine
{
  internal sealed class McpConfigListCommand(string productFullName)
    : McpConfigStoreCommand("list", McpResources.CommandLine_CommandConfigList, productFullName)
  {
    private const string ColumnSeparator = "  ";
    private const string HttpProtocolType = "http";

    protected override string ResolveFilePath() => McpConfigFile.Resolve(null, ProductFullName);

    protected override int Execute(McpConfigStore store, ParseResult parseResult)
    {
      var profiles = store.List();
      if (profiles.Count == 0)
      {
        Console.Out.WriteLine(string.Format(McpResources.CommandLine_ConfigListEmpty, store.FilePath));

        return ExitSuccess;
      }

      List<string[]> rows =
      [
        [
          McpResources.CommandLine_ConfigListName,
          McpResources.CommandLine_ConfigListProtocolType,
          McpResources.CommandLine_ConfigListHttpAddress,
          McpResources.CommandLine_ConfigListHttpPort,
          McpResources.CommandLine_ConfigListHttpRoutePrefix,
        ],
        .. profiles.Select(ToRow),
      ];

      var widths = MeasureColumns(rows);

      Console.Out.WriteLine(store.FilePath);
      Console.Out.WriteLine();

      foreach (var row in rows)
      {
        Console.Out.WriteLine(FormatRow(row, widths));
      }

      return ExitSuccess;
    }

    private static string[] ToRow(McpConfigProfile profile)
    {
      var http = string.Equals(profile.ProtocolType, HttpProtocolType, StringComparison.OrdinalIgnoreCase);

      return
      [
        Text(profile.Name),
        Text(profile.ProtocolType),
        http ? Text(profile.HttpAddress) : string.Empty,
        http && profile.HttpPort is { } port ? port.ToString() : string.Empty,
        http ? Text(profile.HttpRoutePrefix) : string.Empty,
      ];
    }

    private static int[] MeasureColumns(List<string[]> rows)
    {
      var widths = new int[rows[0].Length];

      foreach (var row in rows)
      {
        for (var column = 0; column < widths.Length; column++)
        {
          widths[column] = Math.Max(widths[column], row[column].Length);
        }
      }

      return widths;
    }

    private static string FormatRow(string[] row, int[] widths)
    {
      var builder = new StringBuilder();

      for (var column = 0; column < row.Length; column++)
      {
        builder.Append(row[column].PadRight(widths[column])).Append(ColumnSeparator);
      }

      return builder.ToString().TrimEnd();
    }

    private static string Text(string value) => value ?? string.Empty;
  }
}