// --------------------------------------------------------------------------
// <copyright file="SingleInstanceGuard.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Threading;

namespace Devart.AI.McpServer.Hosting
{
  internal readonly struct SingleInstanceGuard : IDisposable
  {
    private readonly Mutex _mutex;

    public bool Acquired { get; }

    private SingleInstanceGuard(Mutex mutex, bool acquired)
    {
      _mutex = mutex;
      Acquired = acquired;
    }

    public static SingleInstanceGuard TryAcquire(Guid id)
    {
      if (id == Guid.Empty)
      {
        return new SingleInstanceGuard(null, acquired: true);
      }

      var mutex = new Mutex(initiallyOwned: false, $@"Global\{id}", out var createdNew);
      if (!createdNew)
      {
        mutex.Dispose();
        return new SingleInstanceGuard(null, acquired: false);
      }

      return new SingleInstanceGuard(mutex, acquired: true);
    }

    public void Dispose() => _mutex?.Dispose();
  }
}
