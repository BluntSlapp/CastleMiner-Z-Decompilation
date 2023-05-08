using System;

namespace DNA.ComponentModel
{
	public class CategoryAttribute : Attribute
	{
		private string _category;

		public string Category
		{
			get
			{
				return _category;
			}
		}

		public CategoryAttribute(string category)
		{
			_category = category;
		}
	}
}
