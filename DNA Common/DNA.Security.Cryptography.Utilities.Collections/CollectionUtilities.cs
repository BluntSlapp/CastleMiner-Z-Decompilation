using System;
using System.Collections;
using System.Text;

namespace DNA.Security.Cryptography.Utilities.Collections
{
	public sealed class CollectionUtilities
	{
		private CollectionUtilities()
		{
		}

		public static bool CheckElementsAreOfType(IEnumerable e, Type t)
		{
			foreach (object item in e)
			{
				if (!t.IsInstanceOfType(item))
				{
					return false;
				}
			}
			return true;
		}

		public static string ToString(IEnumerable c)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			IEnumerator enumerator = c.GetEnumerator();
			if (enumerator.MoveNext())
			{
				stringBuilder.Append(enumerator.Current.ToString());
				while (enumerator.MoveNext())
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(enumerator.Current.ToString());
				}
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}
	}
}
