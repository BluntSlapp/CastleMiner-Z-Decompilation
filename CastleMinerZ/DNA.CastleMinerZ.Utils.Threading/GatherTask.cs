namespace DNA.CastleMinerZ.Utils.Threading
{
	public sealed class GatherTask : BaseTask
	{
		private CountdownLatch _count = default(CountdownLatch);

		private TaskThreadEnum _thread = TaskThreadEnum.THREAD_ANY;

		private bool _rush;

		private SimpleQueue<BaseTask> _children = new SimpleQueue<BaseTask>();

		private static ObjectCache<GatherTask> _cache = new ObjectCache<GatherTask>();

		public bool Rush
		{
			get
			{
				return _rush;
			}
			set
			{
				_rush = value;
			}
		}

		public TaskThreadEnum Thread
		{
			get
			{
				return _thread;
			}
			set
			{
				_thread = value;
			}
		}

		public override void DoWork(TaskThreadEnum threadIndex)
		{
			base.DoWork(threadIndex);
			Release();
		}

		private void InsertTask(BaseTask task)
		{
			_children.Queue(task);
		}

		public void SetCount(int c)
		{
			_count.Value = c;
		}

		public void AddTask(TaskDelegate work, object context)
		{
			Task task = Task.Alloc();
			task.Init(work, context, this);
			InsertTask(task);
		}

		public WaitableTask AddWaitableTask(TaskDelegate work, object context)
		{
			WaitableTask waitableTask = WaitableTask.Alloc();
			waitableTask.Init(work, context, this);
			InsertTask(waitableTask);
			return waitableTask;
		}

		public void Start()
		{
			_count.Value = _children.Count;
			while (!_children.Empty)
			{
				BaseTask task = _children.Dequeue();
				if (_rush)
				{
					TaskDispatcher.Instance.AddRushTask(task);
				}
				else
				{
					TaskDispatcher.Instance.AddTask(task);
				}
			}
		}

		public void StartNow()
		{
			_rush = true;
			Start();
		}

		public void ChildFinished()
		{
			if (_count.Decrement() == 0)
			{
				if (_rush)
				{
					TaskDispatcher.Instance.AddRushTask(this);
				}
				else
				{
					TaskDispatcher.Instance.AddTask(_thread, this);
				}
			}
		}

		public static GatherTask Alloc()
		{
			GatherTask gatherTask = _cache.Get();
			gatherTask._thread = TaskThreadEnum.THREAD_ANY;
			gatherTask._count.Value = 0;
			gatherTask._rush = false;
			return gatherTask;
		}

		public override void Release()
		{
			base.Release();
			_cache.Put(this);
		}
	}
}
