using DNA.Data.Units;

namespace DNA.Audio
{
	public struct FrequencyPair
	{
		public Frequency Value;

		public float Magnitude;

		public override string ToString()
		{
			return Value.ToString() + " " + Magnitude;
		}
	}
}
