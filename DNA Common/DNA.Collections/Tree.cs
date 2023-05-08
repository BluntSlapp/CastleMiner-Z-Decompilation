using System;
using System.Collections;
using System.Collections.Generic;

namespace DNA.Collections
{
	public class Tree<T> where T : Tree<T>
	{
		public class NodeCollection : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
		{
			private Tree<T> _owner;

			private List<T> List;

			public T this[int index]
			{
				get
				{
					return List[index];
				}
				set
				{
					T val = List[index];
					List[index] = value;
					if (val != value)
					{
						OnSetComplete(index, val, value);
					}
				}
			}

			public int Count
			{
				get
				{
					return List.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public NodeCollection(Tree<T> owner)
			{
				_owner = owner;
				List = new List<T>();
			}

			public NodeCollection(Tree<T> owner, int size)
			{
				_owner = owner;
				List = new List<T>(size);
			}

			public void Add(T t)
			{
				List.Add(t);
				OnInsertComplete(List.Count - 1, t);
			}

			public void AddRange(IEnumerable<T> tlist)
			{
				foreach (T item in tlist)
				{
					List.Add(item);
					OnInsertComplete(List.Count - 1, item);
				}
			}

			public int IndexOf(T t)
			{
				return List.IndexOf(t);
			}

			public bool Contains(T t)
			{
				return List.Contains(t);
			}

			public void Insert(int index, T t)
			{
				List.Insert(index, t);
				OnInsertComplete(index, t);
			}

			private void OnClear()
			{
				for (int i = 0; i < List.Count; i++)
				{
					T val = List[i];
					val._parent = null;
					val.OnParentChanged((T)_owner, null);
				}
			}

			private void OnInsertComplete(int index, T value)
			{
				T val = value;
				val._parent = (T)_owner;
				val.OnParentChanged(null, (T)_owner);
				_owner.OnChildrenChanged();
			}

			private void OnRemoveComplete(int index, T value)
			{
				T val = value;
				val._parent = null;
				val.OnParentChanged((T)_owner, null);
				_owner.OnChildrenChanged();
			}

			private void OnSetComplete(int index, T oldValue, T newValue)
			{
				T val = oldValue;
				T val2 = newValue;
				val._parent = null;
				val2._parent = (T)_owner;
				val.OnParentChanged((T)_owner, null);
				val2.OnParentChanged(null, (T)_owner);
				_owner.OnChildrenChanged();
			}

			public T[] ToArray()
			{
				T[] array = new T[List.Count];
				List.CopyTo(array, 0);
				return array;
			}

			public void Sort(Comparison<T> comparison)
			{
				List.Sort(comparison);
			}

			public void RemoveAt(int index)
			{
				T value = List[index];
				List.RemoveAt(index);
				OnRemoveComplete(index, value);
			}

			public void Clear()
			{
				OnClear();
				List.Clear();
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				List.CopyTo(array, arrayIndex);
			}

			public bool Remove(T item)
			{
				int num = List.IndexOf(item);
				if (num < 0)
				{
					return false;
				}
				RemoveAt(num);
				return true;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return List.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)List).GetEnumerator();
			}
		}

		private T _parent;

		private NodeCollection _children;

		public T Root
		{
			get
			{
				T result = (T)this;
				while (result.Parent != null)
				{
					result = result.Parent;
				}
				return result;
			}
		}

		public T Parent
		{
			get
			{
				return _parent;
			}
		}

		public NodeCollection Children
		{
			get
			{
				return _children;
			}
		}

		public bool IsDecendantOf(T node)
		{
			for (Tree<T> tree = this; tree != null; tree = tree.Parent)
			{
				if (tree == node)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnParentChanged(T oldParent, T newParent)
		{
		}

		protected virtual void OnChildrenChanged()
		{
		}

		public Tree()
		{
			_children = new NodeCollection(this);
		}

		public Tree(int size)
		{
			_children = new NodeCollection(this, size);
		}
	}
}
