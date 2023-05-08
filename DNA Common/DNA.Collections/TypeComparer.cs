using System;
using System.Collections;

namespace DNA.Collections
{
	public class TypeComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			Type type = (Type)x;
			Type type2 = (Type)y;
			return string.Compare(type.FullName, type2.FullName);
		}
	}
}
