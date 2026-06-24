// --------------------------------------------------------------------------
// <copyright file="SingleInstanceAlreadyAcquiredException.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;

namespace Devart.AI.McpServer.Hosting
{
  internal sealed class SingleInstanceAlreadyAcquiredException : Exception
  {
    public SingleInstanceAlreadyAcquiredException() : base("Already running") { }
  }
}
