// --------------------------------------------------------------------------
// <copyright file="AdoNetCommandHelper.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Data.Common;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.AdoNet
{
  internal sealed class AdoNetCommandHelper(ISqlFormatter formatter) : ICommandHelper
  {
    public void AddParameter(DbCommand command, object value)
    {
      var parameter = FindUnsetParameter(command) ?? AddParameter(command);
      parameter.Value = value ?? DBNull.Value;
    }

    public DbParameter AddResultParameter(DbCommand command) => null;

    private static DbParameter FindUnsetParameter(DbCommand command)
    {
      foreach (DbParameter parameter in command.Parameters)
      {
        if (parameter.Value is null)
        {
          return parameter;
        }
      }

      return null;
    }

    private DbParameter AddParameter(DbCommand command)
    {
      var parameter = command.CreateParameter();
      parameter.ParameterName = formatter.FormatParameterName(command.Parameters.Count);
      command.Parameters.Add(parameter);
      return parameter;
    }
  }
}