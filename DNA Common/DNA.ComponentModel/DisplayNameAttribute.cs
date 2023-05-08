using System;

namespace DNA.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DisplayNameAttribute : Attribute
	{
		private string _displayName;

		public string DisplayName
		{
			get
			{
				return _displayName;
			}
		}

		public static string GetDisplayName(Type t)
		{
			object[] customAttributes = t.GetCustomAttributes(typeof(DisplayNameAttribute), false);
			if (customAttributes.Length == 0)
			{
				throw new ArgumentException("Class " + t.Name + " Does not have a Display Name");
			}
			DisplayNameAttribute displayNameAttribute = (DisplayNameAttribute)customAttributes[0];
			return displayNameAttribute.DisplayName;
		}

		public DisplayNameAttribute(string displayName)
		{
			_displayName = displayName;
		}
	}
}
