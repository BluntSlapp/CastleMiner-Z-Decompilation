using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Collections
{
	public class NotifyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private List<T> _list = new List<T>();

		public T this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				OnValidate(value);
				T oldValue = _list[index];
				OnSet(index, _list[index], value);
				_list[index] = value;
				OnSetComplete(index, oldValue, value);
				if (this.Set != null)
				{
					this.Set(this, null);
				}
				if (this.Modified != null)
				{
					this.Modified(this, null);
				}
			}
		}

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public event EventHandler Cleared;

		public event EventHandler Inserted;

		public event EventHandler Removed;

		public event EventHandler Set;

		public event EventHandler Modified;

		protected virtual void OnModified()
		{
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnInsert(int index, T value)
		{
		}

		protected virtual void OnInsertComplete(int index, T value)
		{
		}

		protected virtual void OnRemove(int index, T value)
		{
		}

		protected virtual void OnRemoveComplete(int index, T value)
		{
		}

		protected virtual void OnSet(int index, T oldValue, T newValue)
		{
		}

		protected virtual void OnSetComplete(int index, T oldValue, T newValue)
		{
		}

		protected virtual void OnValidate(T value)
		{
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			OnValidate(item);
			OnInsert(index, item);
			_list.Insert(index, item);
			OnInsertComplete(index, item);
			if (this.Inserted != null)
			{
				this.Inserted(this, null);
			}
			if (this.Modified != null)
			{
				this.Modified(this, null);
			}
		}

		public void RemoveAt(int index)
		{
			T value = _list[index];
			OnRemove(index, value);
			_list.RemoveAt(index);
			OnRemoveComplete(index, value);
			if (this.Removed != null)
			{
				this.Removed(this, null);
			}
			if (this.Modified != null)
			{
				this.Modified(this, null);
			}
		}

		public void Add(T item)
		{
			OnValidate(item);
			OnInsert(_list.Count, item);
			_list.Add(item);
			OnInsert(_list.Count - 1, item);
			if (this.Inserted != null)
			{
				this.Inserted(this, null);
			}
			if (this.Modified != null)
			{
				this.Modified(this, null);
			}
		}

		public void Clear()
		{
			OnClear();
			_list.Clear();
			OnClearComplete();
			if (this.Cleared != null)
			{
				this.Cleared(this, null);
			}
			if (this.Modified != null)
			{
				this.Modified(this, null);
			}
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			int num = _list.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			OnRemove(num, item);
			_list.RemoveAt(num);
			OnRemoveComplete(num, item);
			if (this.Removed != null)
			{
				this.Removed(this, null);
			}
			if (this.Modified != null)
			{
				this.Modified(this, null);
			}
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
	}
}
