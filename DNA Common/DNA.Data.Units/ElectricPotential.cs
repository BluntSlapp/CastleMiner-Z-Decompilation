namespace DNA.Data.Units
{
	[Serializable]
	public struct ElectricPotential
	{
		private float _volts;

		public float Volts
		{
			get
			{
				return _volts;
			}
			set
			{
				_volts = value;
			}
		}

		public override int GetHashCode()
		{
			return _volts.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _volts == ((ElectricPotential)obj)._volts;
		}

		public static bool operator ==(ElectricPotential a, ElectricPotential b)
		{
			return a._volts == b._volts;
		}

		public static bool operator !=(ElectricPotential a, ElectricPotential b)
		{
			return a._volts != b._volts;
		}
	}
}
