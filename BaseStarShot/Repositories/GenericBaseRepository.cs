using System;
using System.Threading;
using SQLite.Net;
using System.Threading.Tasks;

namespace BaseStarShot.Repositories
{
	public abstract class GenericBaseRepository : BaseRepository
	{
		readonly AutoResetEvent resetEvent = new AutoResetEvent(true);

		protected GenericBaseRepository(string dbPath, bool storeDateTimeAsTicks = false, IBlobSerializer serializer = null)
			: base(dbPath, storeDateTimeAsTicks, serializer)
		{

		}

		protected Task<T> InvokeThreadSafe<T>(Func<Task<T>> func)
		{
			if (resetEvent.WaitOne())
			{
				try
				{
					return func();
				}
				finally
				{
					resetEvent.Set();
				}
			}
			return Task.FromResult(default(T));
		}
	}
}

