namespace DNA.Data.Units
{
	[Serializable]
	public struct ElectricCurrent
	{
		private float _ampere;

		public float Amperes
		{
			get
			{
				return _ampere;
			}
			set
			{
				_ampere = value;
			}
		}

		public override int GetHashCode()
		{
			return _ampere.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _ampere == ((ElectricCurrent)obj)._ampere;
		}

		public static bool operator ==(ElectricCurrent a, ElectricCurrent b)
		{
			return a._ampere == b._ampere;
		}

		public static bool operator !=(ElectricCurrent a, ElectricCurrent b)
		{
			return a._ampere != b._ampere;
		}
	}
}
