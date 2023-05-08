using System;
using System.Threading;

namespace DNA.CastleMinerZ.Utils.Threading
{
	public class TaskThread
	{
		private SynchronizedQueue<BaseTask> _taskList = new SynchronizedQueue<BaseTask>();

		private AutoResetEvent _tasksWaiting = new AutoResetEvent(false);

		private volatile bool _timeToQuit;

		private volatile bool _busy;

		private volatile bool _dormant;

		public int _tasksPending;

		public TaskThreadEnum _threadIndex;

		public bool Busy
		{
			get
			{
				return _busy;
			}
		}

		public bool Dormant
		{
			get
			{
				return _dormant;
			}
			set
			{
				_dormant = value;
			}
		}

		public void Wake()
		{
			_tasksWaiting.Set();
		}

		public void AddTask(BaseTask task)
		{
			_taskList.Queue(task);
			Wake();
		}

		public void Abort()
		{
			_timeToQuit = true;
			_tasksWaiting.Set();
		}

		public BaseTask GetTask()
		{
			BaseTask baseTask = _taskList.Dequeue();
			if (baseTask == null && !_dormant)
			{
				baseTask = TaskDispatcher.Instance.GetTask();
			}
			return baseTask;
		}

		public void DrainTaskList()
		{
			BaseTask task = GetTask();
			while (!_timeToQuit && task != null)
			{
				try
				{
					task.DoWork(_threadIndex);
				}
				catch (Exception e)
				{
					CastleMinerZGame.Instance.CrashGame(e);
				}
				task = GetTask();
			}
		}

		public void ThreadStart()
		{
			while (!_timeToQuit)
			{
				DrainTaskList();
				if (!_timeToQuit)
				{
					_busy = false;
					_tasksWaiting.WaitOne();
					_busy = true;
				}
			}
		}
	}
}
