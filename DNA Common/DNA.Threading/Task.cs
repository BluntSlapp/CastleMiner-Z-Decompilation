using System;

namespace DNA.Threading
{
	public abstract class Task
	{
		private TaskStatus _status;

		private Exception _exception;

		public TaskStatus Status
		{
			get
			{
				return _status;
			}
			protected set
			{
				_status = value;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
			protected set
			{
				_exception = value;
			}
		}

		private bool Failed
		{
			get
			{
				return Status == TaskStatus.Failed;
			}
		}

		private bool Completed
		{
			get
			{
				if (Status != TaskStatus.Failed)
				{
					return Status == TaskStatus.Compelete;
				}
				return true;
			}
		}
	}
}
