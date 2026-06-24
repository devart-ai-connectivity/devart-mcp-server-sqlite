// --------------------------------------------------------------------------
// <copyright file="McpAppSettings.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

namespace Devart.AI.McpServer
{
  public abstract class McpAppSettings
  {
    public abstract string ServerName { get; }

    public abstract string SourceName { get; }

    public abstract bool OnPremise { get; }

    public virtual string ToolPrefix => null;

    public virtual string[] NonQueryOperations => [
      "Insert",
      "Update",
      "Delete",
      "Create",
      "Alter",
      "Drop",
      "Call"
    ];

    public virtual string[] Routines => [
      McpConstants.ProcedureRoutine,
      McpConstants.FunctionRoutine
    ];

    public virtual string[] IgnoreSchemas => [];

    public virtual SqlFormatterSettings SqlFormatter => null;
  }
}
