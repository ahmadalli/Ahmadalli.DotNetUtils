using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ahmadalli.DotNetUtils.Collections
{
    public class AsyncConcurrentQueue<TEntity>
    {
        private readonly ConcurrentQueue<TEntity> _queue = new ConcurrentQueue<TEntity>();
        private readonly SemaphoreSlim _countSemaphoreSlim = new SemaphoreSlim(0);

        public void Enqueue(TEntity item)
        {
            _queue.Enqueue(item);
            _countSemaphoreSlim.Release();
        }

        public async Task<TEntity> TryDequeueAsync(CancellationToken cancellationToken = default)
        {
            await _countSemaphoreSlim.WaitAsync(cancellationToken);
            if (!_queue.TryDequeue(out var result))
                throw new InvalidOperationException("dequeuing failed");
            return result;
        }

        public bool TryPeek(out TEntity item) => _queue.TryPeek(out item);
    }
}
