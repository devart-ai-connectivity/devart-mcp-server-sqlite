// --------------------------------------------------------------------------
// <copyright file="AdoNetCommandHelper.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Common;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer.AdoNet
{
  internal sealed class AdoNetCommandHelper : ICommandHelper
  {
    public void AddParameter(DbCommand command, object value)
    {
    }

    public DbParameter AddResultParameter(DbCommand command)
    {
      return null;
    }
  }
}
