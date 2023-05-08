namespace DNA.Profiling
{
	public class ProfilerSimpleStack<T> where T : class, IProfilerLinkedListNode
	{
		private T _root = null;

		public T Root
		{
			get
			{
				return _root;
			}
		}

		public bool Empty
		{
			get
			{
				return _root == null;
			}
		}

		public void PushList(T newList)
		{
			IProfilerLinkedListNode profilerLinkedListNode = newList;
			while (profilerLinkedListNode.NextNode != null)
			{
				profilerLinkedListNode = profilerLinkedListNode.NextNode;
			}
			profilerLinkedListNode.NextNode = _root;
			_root = newList;
		}

		public void Push(T newNode)
		{
			newNode.NextNode = _root;
			_root = newNode;
		}

		public T Pop()
		{
			T root = _root;
			if (_root != null)
			{
				_root = _root.NextNode as T;
			}
			return root;
		}

		public void Clear()
		{
			_root = null;
		}
	}
}
