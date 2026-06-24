// --------------------------------------------------------------------------
// <copyright file="ISqlFormatter.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Common;

namespace Devart.AI.McpServer.Interfaces
{
  public interface ISqlFormatter
  {
    string FormatName(string schema, string name, McpConfiguration configuration, DbConnection connection);

    string FormatCallProcedure(string procedureFullName, int parametersCount);

    string FormatCallFunction(string functionFullName, int parametersCount);
  }
}
