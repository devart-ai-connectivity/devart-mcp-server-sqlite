// --------------------------------------------------------------------------
// <copyright file="McpRootCommand.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.CommandLine;
using System.Linq;

namespace Devart.AI.McpServer.CommandLine
{
  internal sealed class McpRootCommand : RootCommand
  {
    public static readonly string VerbosityOption = "--verbosity";

    public McpRootCommand() : base(McpResources.CommandLine_RootCommand)
    {
      Option<string> verbosityOption = new(VerbosityOption, "-v")
      {
        Description = McpResources.CommandLine_VerbosityOption,
        Recursive = true,
        Arity = ArgumentArity.ZeroOrOne,
        DefaultValueFactory = result => ""
      };

      verbosityOption.Validators.Add(result =>
      {
        if (result.Tokens.Count == 0)
        {
          return;
        }

        string value = result.Tokens.Single().Value.ToLowerInvariant();
        string[] validValues = ["quiet", "q", "minimal", "m", "normal", "n", "detailed", "d", "diagnostic", "diag"];

        if (!validValues.Contains(value))
        {
          result.AddError($"Argument '{value}' not recognized. Must be one of: 'q[uiet]', 'm[inimal]', 'n[ormal]', 'd[etailed]', 'diag[nostic]'");
        }
      });
      Options.Add(verbosityOption);
    }
  }
}
