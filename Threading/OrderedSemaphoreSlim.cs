﻿// copyright: https://stackoverflow.com/a/23415880/1003464

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ahmadalli.DotNetUtils.Threading
{
    public sealed class OrderedSemaphoreSlim : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<TaskCompletionSource<bool>> _queue = new ConcurrentQueue<TaskCompletionSource<bool>>();

        public OrderedSemaphoreSlim(int initialCount)
        {
            _semaphore = new SemaphoreSlim(initialCount);
        }

        public OrderedSemaphoreSlim(int initialCount, int maxCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, maxCount);
        }

        public void Wait()
        {
            WaitAsync().Wait();
        }

        public Task WaitAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>();
            _queue.Enqueue(tcs);
            _semaphore.WaitAsync(cancellationToken).ContinueWith(t =>
            {
                if (_queue.TryDequeue(out var popped))
                    popped.SetResult(true);
            }, cancellationToken);
            return tcs.Task;
        }

        public void Release()
        {
            _semaphore.Release();
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }
}
