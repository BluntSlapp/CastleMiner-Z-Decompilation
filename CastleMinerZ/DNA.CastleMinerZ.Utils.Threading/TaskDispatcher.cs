using System.Threading;

namespace DNA.CastleMinerZ.Utils.Threading
{
	public class TaskDispatcher
	{
		private static string[] _threadNames = new string[4] { "TASK_THREAD_1", "TASK_THREAD_2", "TASK_THREAD_3", "TASK_THREAD_COMAIN" };

		private static int[] _coreTable = new int[4] { 4, 3, 5, 1 };

		private static TaskDispatcher _theInstance = null;

		private TaskThread[] _taskThreads;

		private Thread[] _systemThreads;

		private int _numThreads = 4;

		private volatile bool _stopped;

		private SynchronizedQueue<BaseTask> _tasks = new SynchronizedQueue<BaseTask>();

		public bool Stopped
		{
			get
			{
				return _stopped;
			}
		}

		public static TaskDispatcher Instance
		{
			get
			{
				return _theInstance;
			}
		}

		public bool CanUseMainCore
		{
			get
			{
				if (_numThreads <= 3)
				{
					return false;
				}
				return !_taskThreads[3].Dormant;
			}
			set
			{
				if (_numThreads > 3)
				{
					_taskThreads[3].Dormant = !value;
				}
			}
		}

		private static string TheadName(int i)
		{
			return _threadNames[i];
		}

		public static TaskDispatcher Create()
		{
			if (_theInstance == null)
			{
				_theInstance = new TaskDispatcher();
			}
			return _theInstance;
		}

		private TaskDispatcher()
		{
			_theInstance = this;
			_stopped = false;
			_taskThreads = new TaskThread[_numThreads];
			_systemThreads = new Thread[_numThreads];
			for (int i = 0; i < _numThreads; i++)
			{
				_taskThreads[i] = new TaskThread();
				_taskThreads[i]._threadIndex = (TaskThreadEnum)i;
			}
			for (int j = 0; j < _numThreads; j++)
			{
				int num = _coreTable[j];
				TaskThread thread = _taskThreads[j];
				_systemThreads[j] = new Thread((ThreadStart)delegate
				{
					Thread.CurrentThread.SetProcessorAffinity(new int[1] { _coreTable[(int)thread._threadIndex] });
					thread.ThreadStart();
				});
			}
			if (_numThreads > 3)
			{
				_taskThreads[3].Dormant = true;
			}
			for (int k = 0; k != _numThreads; k++)
			{
				_systemThreads[k].Name = _threadNames[k];
				_systemThreads[k].Start();
			}
		}

		public bool IsIdle(TaskThreadEnum skipIndex)
		{
			for (int i = 0; i < _numThreads; i++)
			{
				if (skipIndex != (TaskThreadEnum)i && !_taskThreads[i].Dormant && _taskThreads[i].Busy)
				{
					return false;
				}
			}
			return true;
		}

		public void Stop()
		{
			if (_stopped)
			{
				return;
			}
			_stopped = true;
			WaitableTask.WakeAll();
			for (int i = 0; i < _numThreads; i++)
			{
				_taskThreads[i].Abort();
			}
			for (int j = 0; j < _numThreads; j++)
			{
				if (!_systemThreads[j].Join(1000))
				{
					_systemThreads[j].Abort();
				}
			}
		}

		private void WakeThreads()
		{
			for (int i = 0; i < _numThreads; i++)
			{
				if (!_taskThreads[i].Dormant)
				{
					_taskThreads[i].Wake();
				}
			}
		}

		private void InsertTask(BaseTask task)
		{
			_tasks.Queue(task);
			WakeThreads();
		}

		private void AppendTask(BaseTask task)
		{
			_tasks.Undequeue(task);
			WakeThreads();
		}

		private void InsertTask(TaskThreadEnum thread, BaseTask task)
		{
			if (thread == TaskThreadEnum.THREAD_ANY)
			{
				InsertTask(task);
			}
			else
			{
				_taskThreads[(int)thread].AddTask(task);
			}
		}

		public void AddTask(BaseTask task)
		{
			if (_stopped)
			{
				task.Interrupt();
			}
			else
			{
				InsertTask(task);
			}
		}

		public void AddRushTask(BaseTask task)
		{
			if (_stopped)
			{
				task.Interrupt();
			}
			else
			{
				AppendTask(task);
			}
		}

		public void AddTask(TaskThreadEnum thread, BaseTask task)
		{
			if (thread == TaskThreadEnum.THREAD_ANY)
			{
				AddTask(task);
			}
			else if (_stopped)
			{
				task.Interrupt();
			}
			else
			{
				InsertTask(thread, task);
			}
		}

		public BaseTask GetTask()
		{
			return _tasks.Dequeue();
		}

		public void AddTask(TaskDelegate work, object context)
		{
			if (!_stopped)
			{
				Task task = Task.Alloc();
				task.Init(work, context);
				InsertTask(task);
			}
		}

		public void AddRushTask(TaskDelegate work, object context)
		{
			if (!_stopped)
			{
				Task task = Task.Alloc();
				task.Init(work, context);
				AppendTask(task);
			}
		}

		public void AddTask(TaskThreadEnum thread, TaskDelegate work, object context)
		{
			if (!_stopped)
			{
				if (thread == TaskThreadEnum.THREAD_ANY)
				{
					AddTask(work, context);
					return;
				}
				Task task = Task.Alloc();
				task.Init(work, context);
				InsertTask(thread, task);
			}
		}

		public WaitableTask AddWaitableTask(TaskThreadEnum thread, TaskDelegate work, object context)
		{
			WaitableTask waitableTask;
			if (thread == TaskThreadEnum.THREAD_ANY)
			{
				waitableTask = AddWaitableTask(work, context);
			}
			else
			{
				waitableTask = WaitableTask.Alloc();
				waitableTask.Init(work, context);
				if (_stopped)
				{
					waitableTask.Interrupt();
				}
				else
				{
					InsertTask(thread, waitableTask);
				}
			}
			return waitableTask;
		}

		public WaitableTask AddWaitableTask(TaskDelegate work, object context)
		{
			WaitableTask waitableTask = WaitableTask.Alloc();
			waitableTask.Init(work, context);
			if (_stopped)
			{
				waitableTask.Interrupt();
			}
			else
			{
				InsertTask(waitableTask);
			}
			return waitableTask;
		}

		public WaitableTask AddWaitableRushTask(TaskDelegate work, object context)
		{
			WaitableTask waitableTask = WaitableTask.Alloc();
			waitableTask.Init(work, context);
			if (_stopped)
			{
				waitableTask.Interrupt();
			}
			else
			{
				AppendTask(waitableTask);
			}
			return waitableTask;
		}

		public GatherTask AddGatherTask(TaskThreadEnum thread, TaskDelegate work, object context)
		{
			GatherTask gatherTask = AddGatherTask(work, context);
			gatherTask.Thread = thread;
			return gatherTask;
		}

		public GatherTask AddGatherTask(TaskDelegate work, object context)
		{
			GatherTask gatherTask = GatherTask.Alloc();
			gatherTask.Init(work, context);
			return gatherTask;
		}
	}
}
