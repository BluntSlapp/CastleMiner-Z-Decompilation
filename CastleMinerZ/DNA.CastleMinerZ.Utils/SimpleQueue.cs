namespace DNA.CastleMinerZ.Utils
{
	public class SimpleQueue<T> where T : class, ILinkedListNode
	{
		private T _back = null;

		private T _front = null;

		private int _count;

		public T Front
		{
			get
			{
				return _front;
			}
		}

		public T Back
		{
			get
			{
				return _back;
			}
		}

		public bool Empty
		{
			get
			{
				return _front == null;
			}
		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public virtual void Queue(T obj)
		{
			if (_front == null)
			{
				_back = obj;
				_front = obj;
				obj.NextNode = null;
			}
			else
			{
				_back.NextNode = obj;
				_back = obj;
				obj.NextNode = null;
			}
			_count++;
		}

		private void InnerReplace(SimpleQueue<T> q)
		{
			_front = q._front;
			_back = q._back;
			_count = q._count;
			q._front = null;
			q._back = null;
			q._count = 0;
		}

		public virtual void ReplaceFromList(T root)
		{
			_front = root;
			_back = null;
			_count = 0;
			while (root != null)
			{
				_back = root;
				_count++;
				root = (T)root.NextNode;
			}
		}

		public virtual void ReplaceContentsWith(SimpleQueue<T> q)
		{
			if (q is SynchronizedQueue<T>)
			{
				lock (q)
				{
					InnerReplace(q);
					return;
				}
			}
			InnerReplace(q);
		}

		public virtual T Clear()
		{
			T front = _front;
			_back = null;
			_front = null;
			_count = 0;
			return front;
		}

		public virtual T Dequeue()
		{
			T front = _front;
			if (_front != null)
			{
				_front = (T)_front.NextNode;
				if (_front == null)
				{
					_back = null;
				}
				_count--;
			}
			if (front != null)
			{
				front.NextNode = null;
			}
			return front;
		}

		public virtual void Undequeue(T obj)
		{
			obj.NextNode = _front;
			_front = obj;
			if (_back == null)
			{
				_back = _front;
			}
			_count++;
		}

		public virtual void Remove(T obj)
		{
			if (_front == obj)
			{
				_front = (T)_front.NextNode;
				if (_front == null)
				{
					_back = null;
				}
				_count--;
			}
			else
			{
				T back = _front;
				while (back.NextNode != null)
				{
					if (back.NextNode == obj)
					{
						back.NextNode = obj.NextNode;
						if (_back == obj)
						{
							_back = back;
						}
						_count--;
						break;
					}
					back = (T)back.NextNode;
				}
			}
			obj.NextNode = null;
		}
	}
}
