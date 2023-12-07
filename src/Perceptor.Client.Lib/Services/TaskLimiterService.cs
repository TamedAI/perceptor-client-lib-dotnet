using System;
using System.Threading;
using System.Threading.Tasks;

namespace Perceptor.Client.Lib.Services
{
	internal class TaskLimiterService
	{
		private readonly SemaphoreSlim _mutex;

		public TaskLimiterService(int maximalNumberOfParallelRequests)
		{
			_mutex = new SemaphoreSlim(maximalNumberOfParallelRequests);
		}
		
		public async Task<T> Exec<T>(Func<Task<T>> func, CancellationToken cancellationToken)
		{
			await _mutex.WaitAsync(cancellationToken);
			try
			{
				return await func();
			}
			finally
			{
				_mutex.Release();
			}
		}
	}
}