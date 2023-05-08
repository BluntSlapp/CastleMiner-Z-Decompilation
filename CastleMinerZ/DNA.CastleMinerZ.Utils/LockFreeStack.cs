using System.Threading;

namespace DNA.CastleMinerZ.Utils
{
	public class LockFreeStack<T> where T : class, ILinkedListNode
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
			if (newNode != null)
			{
				T val = null;
				do
				{
					val = (T)(newNode.NextNode = _root);
				}
				while (val != Interlocked.CompareExchange(ref _root, newNode, val));
			}
		}

		public void PushList(T newList)
		{
			if (newList != null)
			{
				ILinkedListNode linkedListNode = newList;
				while (linkedListNode.NextNode != null)
				{
					linkedListNode = linkedListNode.NextNode;
				}
				T val = null;
				do
				{
					val = (T)(linkedListNode.NextNode = _root);
				}
				while (val != Interlocked.CompareExchange(ref _root, newList, val));
			}
		}

		public T Pop()
		{
			T root;
			do
			{
				root = _root;
			}
			while (root != null && root != Interlocked.CompareExchange(ref _root, root.NextNode as T, root));
			if (root != null)
			{
				root.NextNode = null;
			}
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
