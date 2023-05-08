namespace DNA.Data.Units
{
	[Serializable]
	public struct Amount
	{
		public const float Mole = 6.02214136E+23f;

		private float _value;

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public float Moles
		{
			get
			{
				return _value / 6.02214136E+23f;
			}
			set
			{
				_value = value * 6.02214136E+23f;
			}
		}

		public static Amount FromMoles(float moles)
		{
			return new Amount(moles * 6.02214136E+23f);
		}

		public Amount(float value)
		{
			_value = value;
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _value == ((Amount)obj)._value;
		}

		public static bool operator ==(Amount a, Amount b)
		{
			return a._value == b._value;
		}

		public static bool operator !=(Amount a, Amount b)
		{
			return a._value != b._value;
		}
	}
}
