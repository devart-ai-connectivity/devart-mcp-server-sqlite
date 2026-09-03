// --------------------------------------------------------------------------
// <copyright file="McpConfigValuesCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.CommandLine;

namespace Devart.AI.McpServer.CommandLine
{
  internal abstract class McpConfigValuesCommand : McpConfigProfileCommand
  {
    private const string ProtocolTypeOption = "--protocol-type";
    private const string HttpAddressOption = "--http-address";
    private const string HttpPortOption = "--http-port";
    private const string HttpRoutePrefixOption = "--http-route-prefix";
    private const string ConnectionStringOption = "--connection-string";

    private const int MinHttpPort = 0;
    private const int MaxHttpPort = 65535;

    private readonly Option<string> protocolTypeOption;
    private readonly Option<string> httpAddressOption;
    private readonly Option<int?> httpPortOption;
    private readonly Option<string> httpRoutePrefixOption;
    private readonly Option<string> connectionStringOption;

    protected McpConfigValuesCommand(string name, string description, string productFullName)
      : base(name, description, productFullName)
    {
      this.protocolTypeOption = CreateProtocolTypeOption();
      this.httpAddressOption = CreateOption(HttpAddressOption, "-ha", McpResources.CommandLine_OptionHttpAddress);
      this.httpPortOption = CreateHttpPortOption();
      this.httpRoutePrefixOption = CreateOption(HttpRoutePrefixOption, "-hr", McpResources.CommandLine_OptionHttpRoutePrefix);
      this.connectionStringOption = CreateOption(ConnectionStringOption, "-c", McpResources.CommandLine_OptionConnectionString);

      Options.Add(this.protocolTypeOption);
      Options.Add(this.httpAddressOption);
      Options.Add(this.httpPortOption);
      Options.Add(this.httpRoutePrefixOption);
      Options.Add(this.connectionStringOption);
    }

    protected McpConfigValues GetValues(ParseResult parseResult)
      => new(
        ParseProtocolType(parseResult.GetValue(this.protocolTypeOption)),
        parseResult.GetValue(this.httpAddressOption),
        parseResult.GetValue(this.httpPortOption),
        parseResult.GetValue(this.httpRoutePrefixOption),
        parseResult.GetValue(this.connectionStringOption));

    private static Option<string> CreateOption(string name, string alias, string description)
      => new(name, alias)
      {
        Description = description,
        Arity = ArgumentArity.ExactlyOne
      };

    private static Option<string> CreateProtocolTypeOption()
    {
      var option = CreateOption(ProtocolTypeOption, "-p", McpResources.CommandLine_OptionProtocolType);

      option.Validators.Add(result =>
      {
        if (result.Tokens.Count == 0)
        {
          return;
        }

        var value = result.Tokens[^1].Value;
        if (ParseProtocolType(value) is null)
        {
          result.AddError(string.Format(McpResources.CommandLine_InvalidProtocolType, value));
        }
      });

      return option;
    }

    private static Option<int?> CreateHttpPortOption()
    {
      Option<int?> option = new(HttpPortOption, "-hp")
      {
        Description = McpResources.CommandLine_OptionHttpPort,
        Arity = ArgumentArity.ExactlyOne
      };

      option.Validators.Add(result =>
      {
        if (result.Tokens.Count == 0)
        {
          return;
        }

        var value = result.Tokens[^1].Value;
        if (!int.TryParse(value, out var port) || port is < MinHttpPort or > MaxHttpPort)
        {
          result.AddError(string.Format(McpResources.CommandLine_InvalidHttpPort, value, MinHttpPort, MaxHttpPort));
        }
      });

      return option;
    }

    private static McpProtocolType? ParseProtocolType(string value)
      => value?.Trim().ToLowerInvariant() switch
      {
        "stdio" => McpProtocolType.Stdio,
        "http" => McpProtocolType.Http,
        _ => null
      };
  }
}