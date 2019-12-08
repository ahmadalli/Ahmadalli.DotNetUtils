using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ahmadalli.DotNetUtils.Collections
{
    public class SemaphoreSlimStorage<TEntity>
    {
        private readonly int _semaphoreInitCount;
        private readonly int _semaphoreMaxCount;

        private readonly Dictionary<TEntity, SemaphoreSlim> _semaphoresDictionary = new Dictionary<TEntity, SemaphoreSlim>();

        private readonly SemaphoreSlim _semaphoreDictionarySemaphore = new SemaphoreSlim(1, 1);

        public SemaphoreSlimStorage(int semaphoreInitCount, int semaphoreMaxCount = int.MaxValue)
        {
            _semaphoreInitCount = semaphoreInitCount;
            _semaphoreMaxCount = semaphoreMaxCount;
        }

        public async Task<SemaphoreSlim> GetSemaphore(TEntity key)
        {
            await _semaphoreDictionarySemaphore.WaitAsync();
            try
            {
                if (!_semaphoresDictionary.ContainsKey(key))
                    _semaphoresDictionary.Add(key, new SemaphoreSlim(_semaphoreInitCount, _semaphoreMaxCount));
                return _semaphoresDictionary[key];
            }
            finally
            {
                _semaphoreDictionarySemaphore.Release();
            }
        }
    }
}
