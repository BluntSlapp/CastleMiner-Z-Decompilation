using System;

namespace DNA.ComponentModel
{
	public class DescriptionAttribute : Attribute
	{
		private string _description;

		public string Description
		{
			get
			{
				return _description;
			}
		}

		public DescriptionAttribute(string category)
		{
			_description = category;
		}
	}
}
