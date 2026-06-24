// --------------------------------------------------------------------------
// <copyright file="OdbcCommandHelper.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.Odbc
{
  internal sealed class OdbcCommandHelper : ICommandHelper
  {
    public void AddParameter(DbCommand command, object value) => ((OdbcCommand)command).Parameters.AddWithValue("?", value);

    public DbParameter AddResultParameter(DbCommand command)
      => ((OdbcCommand)command).Parameters.Add(
        new OdbcParameter
        {
          ParameterName = "?",
          OdbcType = OdbcType.NVarChar,
          Size = 4000,
          Direction = ParameterDirection.ReturnValue
        }
      );
  }
}
