using System;
using System.Collections.Generic;
using System.Threading;

namespace DNA.Threading
{
	public class TaskScheduler
	{
		private class ScheduledTask : Task
		{
			private ParameterizedThreadStart _paramCallback;

			private ThreadStart _callback;

			private object _state;

			public ScheduledTask(ParameterizedThreadStart callback, object state)
			{
				_paramCallback = callback;
				_state = state;
			}

			public ScheduledTask(ThreadStart callback)
			{
				_callback = callback;
			}

			public bool DoWork()
			{
				base.Status = TaskStatus.InProcess;
				try
				{
					if (_paramCallback == null)
					{
						_callback();
					}
					else
					{
						_paramCallback(_state);
					}
				}
				catch (Exception ex)
				{
					Exception ex3 = (base.Exception = ex);
					base.Status = TaskStatus.Failed;
					return false;
				}
				base.Status = TaskStatus.Compelete;
				return true;
			}
		}

		public class ExceptionEventArgs : EventArgs
		{
			public Exception InnerException;

			public ExceptionEventArgs(Exception e)
			{
				InnerException = e;
			}
		}

		private Queue<ScheduledTask> _taskQueue = new Queue<ScheduledTask>();

		private Thread _queueWorkerThread;

		public int[] ProcessorThreads = new int[3] { 3, 4, 5 };

		public event EventHandler<ExceptionEventArgs> ThreadException;

		private void ExecutionThread(object state)
		{
			Thread.CurrentThread.SetProcessorAffinity(new int[1] { ProcessorThreads[0] });
			ScheduledTask scheduledTask = (ScheduledTask)state;
			if (!scheduledTask.DoWork() && this.ThreadException != null)
			{
				this.ThreadException(this, new ExceptionEventArgs(scheduledTask.Exception));
			}
		}

		private void StartWorkerQueue()
		{
			if (_queueWorkerThread == null)
			{
				_queueWorkerThread = new Thread(QueueWorker);
				_queueWorkerThread.Name = "TaskSchedulerQueueWorker";
				_queueWorkerThread.Start();
			}
		}

		private void QueueWorker()
		{
			while (true)
			{
				ScheduledTask state;
				lock (_taskQueue)
				{
					if (_taskQueue.Count == 0)
					{
						_queueWorkerThread = null;
						break;
					}
					state = _taskQueue.Dequeue();
				}
				ExecutionThread(state);
			}
		}

		public Task QueueUserWorkItem(ThreadStart callBack)
		{
			lock (_taskQueue)
			{
				ScheduledTask scheduledTask = new ScheduledTask(callBack);
				_taskQueue.Enqueue(scheduledTask);
				StartWorkerQueue();
				return scheduledTask;
			}
		}

		public Task QueueUserWorkItem(ParameterizedThreadStart callBack, object state)
		{
			lock (_taskQueue)
			{
				ScheduledTask scheduledTask = new ScheduledTask(callBack, state);
				_taskQueue.Enqueue(scheduledTask);
				StartWorkerQueue();
				return scheduledTask;
			}
		}

		public Task DoUserWorkItem(ThreadStart callBack)
		{
			ScheduledTask scheduledTask = new ScheduledTask(callBack);
			Thread thread = new Thread(ExecutionThread);
			thread.Name = "TaskSchedulerWorker";
			thread.Start(scheduledTask);
			return scheduledTask;
		}

		public Task DoUserWorkItem(ParameterizedThreadStart callBack, object state)
		{
			ScheduledTask scheduledTask = new ScheduledTask(callBack, state);
			Thread thread = new Thread(ExecutionThread);
			thread.Name = "TaskSchedulerWorker";
			thread.Start(scheduledTask);
			return scheduledTask;
		}
	}
}
