using System.Threading;

namespace DNA.CastleMinerZ.Utils.Threading
{
	public struct SpinLock
	{
		private int _lock;

		public bool Locked
		{
			get
			{
				return _lock != 0;
			}
		}

		public void Lock()
		{
			while (Interlocked.CompareExchange(ref _lock, 1, 0) != 0)
			{
			}
		}

		public void Unlock()
		{
			_lock = 0;
		}

		public bool TryLock()
		{
			return Interlocked.CompareExchange(ref _lock, 1, 0) == 0;
		}
	}
}
