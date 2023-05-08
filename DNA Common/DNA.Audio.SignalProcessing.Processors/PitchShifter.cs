namespace DNA.Audio.SignalProcessing.Processors
{
	public class PitchShifter : SignalProcessor<SpectralData>
	{
		public float Pitch = 1f;

		private SpectralData _buffer = new SpectralData(1, 0);

		public override bool ProcessBlock(SpectralData data)
		{
			data.CopyTo(_buffer);
			data.SetZero();
			for (int i = 0; i < data.Channels; i++)
			{
				FrequencyPair[] data2 = data.GetData(i);
				FrequencyPair[] data3 = _buffer.GetData(i);
				for (int j = 0; j < data2.Length; j++)
				{
					int num = (int)((float)j * Pitch);
					if (num < data2.Length)
					{
						data2[num].Magnitude += data3[j].Magnitude;
						data2[num].Value.Hertz = data3[j].Value.Hertz * Pitch;
					}
				}
			}
			return true;
		}
	}
}
