using System;

namespace DNA.CastleMinerZ.Utils.Threading
{
	public class BaseTask : IReleaseable, ILinkedListNode
	{
		public TaskDelegate _workDelegate;

		public GatherTask _parent;

		public object _context;

		private volatile bool _interrupted;

		public ILinkedListNode _nextNode;

		public virtual bool Interrupted
		{
			get
			{
				return _interrupted;
			}
		}

		public ILinkedListNode NextNode
		{
			get
			{
				return _nextNode;
			}
			set
			{
				_nextNode = value;
			}
		}

		public virtual void Init(TaskDelegate work, object context, GatherTask parent)
		{
			Init(work, context);
			_parent = parent;
		}

		public virtual void Init(TaskDelegate work, object context)
		{
			if (work == null)
			{
				throw new ArgumentNullException("work", "Task initialized with null work delegate");
			}
			_parent = null;
			_workDelegate = work;
			_context = context;
		}

		public virtual void DoWork(TaskThreadEnum threadIndex)
		{
			_workDelegate(threadIndex, this, _context);
			if (_parent != null)
			{
				_parent.ChildFinished();
			}
		}

		public virtual void Interrupt()
		{
			_interrupted = true;
		}

		public virtual void Release()
		{
			_context = null;
			_workDelegate = null;
			_parent = null;
			_interrupted = false;
		}
	}
}
