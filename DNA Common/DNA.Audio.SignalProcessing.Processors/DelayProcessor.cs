using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class DelayProcessor : SignalProcessor<RealPCMData>
	{
		public TimeSpan Delay = TimeSpan.FromMilliseconds(300.0);

		public float Decay = 0.5f;

		private RealPCMData _delayBuffer = new RealPCMData(1, 0, 1);

		private int _delayPos;

		public override bool ProcessBlock(RealPCMData data)
		{
			int num = (int)((double)data.SampleRate * Delay.TotalSeconds);
			int num2 = num * 2;
			if (data.Channels != _delayBuffer.Channels || num2 != _delayBuffer.Samples)
			{
				_delayBuffer = new RealPCMData(data.Channels, num2, data.SampleRate);
			}
			for (int i = 0; i < data.Channels; i++)
			{
				float[] data2 = data.GetData(i);
				float[] data3 = _delayBuffer.GetData(i);
				for (int j = 0; j < data2.Length; j++)
				{
					float num3 = data2[j];
					float num4 = data3[_delayPos] * Decay;
					num3 = (data2[j] = num3 + num4);
					int num5 = (_delayPos + num) % data3.Length;
					data3[num5] = num3;
					_delayPos++;
					if (_delayPos >= data3.Length)
					{
						_delayPos = 0;
					}
				}
			}
			return true;
		}
	}
}
