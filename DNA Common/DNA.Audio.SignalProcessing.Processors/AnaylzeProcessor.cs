using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class AnaylzeProcessor : SignalProcessor<SpectralData>
	{
		private FrequencyPair _primary;

		public FrequencyPair PrimaryFrequency
		{
			get
			{
				return _primary;
			}
		}

		public override bool ProcessBlock(SpectralData data)
		{
			FrequencyPair[] data2 = data.GetData(0);
			float num = float.MinValue;
			for (int i = 0; i < data2.Length; i++)
			{
				if (data2[i].Magnitude > num)
				{
					num = data2[i].Magnitude;
					_primary = data2[i];
				}
			}
			_primary.Value.Hertz = Math.Abs(_primary.Value.Hertz);
			return true;
		}
	}
}
