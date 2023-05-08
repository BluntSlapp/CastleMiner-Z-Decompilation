using System;
using System.Collections.Generic;

namespace DNA.Collections
{
	public static class ArrayTools
	{
		public static T[][] AllocSquareJaggedArray<T>(int x, int y)
		{
			T[][] array = new T[x][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new T[y];
			}
			return array;
		}

		public static void Randomize<T>(IList<T> array, Random rand)
		{
			if (array is T[])
			{
				T[] array2 = (T[])array;
				for (int i = 0; i < array2.Length; i++)
				{
					int num = i + rand.Next(array2.Length - i);
					T val = array2[num];
					array2[num] = array2[i];
					array2[i] = val;
				}
			}
			else
			{
				for (int j = 0; j < array.Count; j++)
				{
					int index = j + rand.Next(array.Count - j);
					T value = array[index];
					array[index] = array[j];
					array[j] = value;
				}
			}
		}

		public static void Randomize<T>(IList<T> array)
		{
			Randomize(array, new Random());
		}

		public static void QSort<T>(IList<T> list, Comparison<T> comparison)
		{
			QSort_r(list, comparison, 0, list.Count - 1, Order.Ascending);
		}

		private static void QSort_r<T>(IList<T> list, Comparison<T> comparison, int d, int h, Order direction)
		{
			if (list.Count == 0)
			{
				return;
			}
			int num = h;
			int i = d;
			T y = list[(d + h) / 2];
			do
			{
				if (direction == Order.Ascending)
				{
					for (; comparison(list[i], y) < 0; i++)
					{
					}
					while (comparison(list[num], y) > 0)
					{
						num--;
					}
				}
				else
				{
					for (; comparison(list[i], y) > 0; i++)
					{
					}
					while (comparison(list[num], y) < 0)
					{
						num--;
					}
				}
				if (num >= i)
				{
					if (num != i)
					{
						T value = list[num];
						list[num] = list[i];
						list[i] = value;
					}
					num--;
					i++;
				}
			}
			while (i <= num);
			if (d < num)
			{
				QSort_r(list, comparison, d, num, direction);
			}
			if (i < h)
			{
				QSort_r(list, comparison, i, h, direction);
			}
		}

		public static void QSort<T>(IList<T> list) where T : IComparable<T>
		{
			QSort_r(list, 0, list.Count - 1, Order.Ascending);
		}

		private static void QSort_r<T>(IList<T> list, int d, int h, Order direction) where T : IComparable<T>
		{
			if (list.Count == 0)
			{
				return;
			}
			int num = h;
			int i = d;
			T other = list[(d + h) / 2];
			do
			{
				if (direction == Order.Ascending)
				{
					for (; list[i].CompareTo(other) < 0; i++)
					{
					}
					while (list[num].CompareTo(other) > 0)
					{
						num--;
					}
				}
				else
				{
					for (; list[i].CompareTo(other) > 0; i++)
					{
					}
					while (list[num].CompareTo(other) < 0)
					{
						num--;
					}
				}
				if (num >= i)
				{
					if (num != i)
					{
						T value = list[num];
						list[num] = list[i];
						list[i] = value;
					}
					num--;
					i++;
				}
			}
			while (i <= num);
			if (d < num)
			{
				QSort_r(list, d, num, direction);
			}
			if (i < h)
			{
				QSort_r(list, i, h, direction);
			}
		}
	}
}
