using System.Threading;

namespace DNA.Profiling
{
	public class ProfilerLockFreeStack<T> where T : class, IProfilerLinkedListNode
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

		public void Push(T newNode)
		{
			T val = null;
			do
			{
				val = (T)(newNode.NextNode = _root);
			}
			while (val != Interlocked.CompareExchange(ref _root, newNode, val));
		}

		public void PushList(T newList)
		{
			IProfilerLinkedListNode profilerLinkedListNode = newList;
			while (profilerLinkedListNode.NextNode != null)
			{
				profilerLinkedListNode = profilerLinkedListNode.NextNode;
			}
			T val = null;
			do
			{
				val = (T)(profilerLinkedListNode.NextNode = _root);
			}
			while (val != Interlocked.CompareExchange(ref _root, newList, val));
		}

		public T Pop()
		{
			T root;
			do
			{
				root = _root;
			}
			while (root != null && root != Interlocked.CompareExchange(ref _root, root.NextNode as T, root));
			return root;
		}

		public T Clear()
		{
			T val = null;
			do
			{
				val = _root;
			}
			while (val != Interlocked.CompareExchange(ref _root, null, val));
			return val;
		}
	}
}
