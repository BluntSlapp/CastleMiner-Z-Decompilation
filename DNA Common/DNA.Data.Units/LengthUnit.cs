namespace DNA.Data.Units
{
	public class LengthUnit : Unit
	{
		public LengthUnit(string[] abbrivations, string name)
			: base(abbrivations, name)
		{
		}

		protected override object SetUnit(double value)
		{
			return null;
		}
	}
}
