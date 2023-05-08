using System;

namespace DNA.Data.Units
{
	public class UnitConversionException : Exception
	{
		public UnitConversionException(string message)
			: base(message)
		{
		}
	}
}
