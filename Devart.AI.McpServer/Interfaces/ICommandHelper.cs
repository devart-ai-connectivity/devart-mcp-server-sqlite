// --------------------------------------------------------------------------
// <copyright file="ICommandHelper.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System.Data.Common;

namespace Devart.AI.McpServer.Interfaces
{
  public interface ICommandHelper
  {
    public void AddParameter(DbCommand command, object value);

    public DbParameter AddResultParameter(DbCommand command);
  }
}
