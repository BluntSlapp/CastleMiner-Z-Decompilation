using System;

namespace DNA.CastleMinerZ.Utils
{
	public class ObjectCache<iType> where iType : class, ILinkedListNode, new()
	{
		private int _growSize = 5;

		private LockFreeStack<iType> _cache = new LockFreeStack<iType>();

		public int GrowSize
		{
			get
			{
				return _growSize;
			}
			set
			{
				if (value > 0)
				{
					_growSize = value;
					return;
				}
				throw new ArgumentException("PartCache.GrowSize must be a positive integer");
			}
		}

		private void GrowList(int size)
		{
			for (int i = 0; i < size; i++)
			{
				_cache.Push(new iType());
			}
		}

		public iType Get()
		{
			iType val = null;
			val = _cache.Pop();
			if (val == null)
			{
				GrowList(_growSize);
				for (val = _cache.Pop(); val == null; val = _cache.Pop())
				{
					_cache.Push(new iType());
				}
			}
			return val;
		}

		public void Put(iType part)
		{
			_cache.Push(part);
		}

		public void PutList(iType list)
		{
			_cache.PushList(list);
		}
	}
}
