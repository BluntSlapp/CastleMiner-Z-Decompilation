using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class RealLevelsProcessor : SignalProcessor<RealPCMData>
	{
		public float PeakLevel;

		public float averageLevel;

		public float Volume = 1f;

		public override bool ProcessBlock(RealPCMData data)
		{
			float[] data2 = data.GetData(0);
			PeakLevel = 0f;
			averageLevel = 0f;
			for (int i = 0; i < data2.Length; i++)
			{
				float num = Math.Abs(data2[i]);
				PeakLevel = ((num > PeakLevel) ? num : PeakLevel);
				averageLevel += num;
			}
			averageLevel /= data2.Length;
			return true;
		}
	}
}
