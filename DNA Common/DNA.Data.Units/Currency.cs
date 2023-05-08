namespace DNA.Data.Units
{
	public struct Currency
	{
		private decimal _usDollars;

		public Currency(decimal usDollars)
		{
			_usDollars = usDollars;
		}

		public override string ToString()
		{
			return "$" + _usDollars;
		}

		public override int GetHashCode()
		{
			return _usDollars.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _usDollars == ((Currency)obj)._usDollars;
		}

		public static bool operator ==(Currency a, Currency b)
		{
			return a._usDollars == b._usDollars;
		}

		public static bool operator !=(Currency a, Currency b)
		{
			return a._usDollars != b._usDollars;
		}
	}
}
