namespace DNA.Audio.SignalProcessing.Processors
{
	public class CompressorProcessor : RealLevelsProcessor
	{
		public float targetLevel = 1f;

		private float[] levelHistory = new float[20];

		private int index;

		public override bool ProcessBlock(RealPCMData data)
		{
			base.ProcessBlock(data);
			levelHistory[index++] = PeakLevel;
			if (index >= levelHistory.Length - 1)
			{
				index = 0;
			}
			float num = 0f;
			for (int i = 0; i < levelHistory.Length; i++)
			{
				num += levelHistory[i];
			}
			num /= (float)levelHistory.Length;
			float num2 = targetLevel / num;
			float[] data2 = data.GetData(0);
			if (PeakLevel * num2 > 0.8f)
			{
				num2 = 0.8f / PeakLevel;
			}
			if (PeakLevel > 0.15f)
			{
				for (int j = 0; j < data2.Length; j++)
				{
					data2[j] *= num2;
				}
			}
			return true;
		}
	}
}
