using System.Threading;

namespace DNA.CastleMinerZ.Utils.Threading
{
	public struct CountdownLatch
	{
		private int _count;

		public int Value
		{
			get
			{
				return _count;
			}
			set
			{
				Interlocked.Exchange(ref _count, value);
			}
		}

		public static implicit operator bool(CountdownLatch latch)
		{
			return latch.Value != 0;
		}

		public int Increment()
		{
			return Interlocked.Increment(ref _count);
		}

		public int Decrement()
		{
			return Interlocked.Decrement(ref _count);
		}
	}
}
