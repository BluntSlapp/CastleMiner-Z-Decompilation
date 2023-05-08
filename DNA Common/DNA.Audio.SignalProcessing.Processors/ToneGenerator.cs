using System;
using DNA.Data.Units;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class ToneGenerator : SignalProcessor<RealPCMData>
	{
		private float t;

		public Frequency Frequency = Frequency.FromHertz(440f);

		public void GenerateBlock()
		{
		}

		public override bool ProcessBlock(RealPCMData data)
		{
			float num = Frequency.Hertz;
			float num2 = 1f / (float)data.SampleRate;
			for (int i = 0; i < data.Channels; i++)
			{
				float[] data2 = data.GetData(i);
				for (int j = 0; j < data2.Length; j++)
				{
					t += num2;
					data2[j] = (float)Math.Sin((double)(t * num) * Math.PI * 2.0);
				}
			}
			return true;
		}
	}
}
