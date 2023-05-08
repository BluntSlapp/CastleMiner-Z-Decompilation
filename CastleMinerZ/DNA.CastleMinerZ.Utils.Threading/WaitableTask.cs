using System.Threading;

namespace DNA.CastleMinerZ.Utils.Threading
{
	public sealed class WaitableTask : BaseTask
	{
		private ManualResetEvent _finished = new ManualResetEvent(true);

		public WaitableTask _nextWaitableTask;

		private volatile bool _interrupted;

		private static ObjectCache<WaitableTask> _cache = new ObjectCache<WaitableTask>();

		private static WaitableTask _waitables = null;

		public bool Done
		{
			get
			{
				return _finished.WaitOne(0);
			}
		}

		public WaitableTask()
		{
			WaitableTask waitableTask = null;
			do
			{
				waitableTask = (_nextWaitableTask = _waitables);
			}
			while (waitableTask != Interlocked.CompareExchange(ref _waitables, this, waitableTask));
		}

		public override void Init(TaskDelegate work, object context)
		{
			base.Init(work, context);
			_interrupted = false;
			_finished.Reset();
		}

		public override void DoWork(TaskThreadEnum threadIndex)
		{
			base.DoWork(threadIndex);
			_finished.Set();
		}

		public override void Interrupt()
		{
			base.Interrupt();
			_finished.Set();
		}

		public bool Wait()
		{
			_finished.WaitOne();
			return !_interrupted;
		}

		public static void WakeAll()
		{
			for (WaitableTask waitableTask = _waitables; waitableTask != null; waitableTask = waitableTask._nextWaitableTask)
			{
				waitableTask.Interrupt();
			}
		}

		public static WaitableTask Alloc()
		{
			return _cache.Get();
		}

		public override void Release()
		{
			base.Release();
			_cache.Put(this);
		}
	}
}
