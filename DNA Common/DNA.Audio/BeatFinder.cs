using System;
using Microsoft.Xna.Framework.Media;

namespace DNA.Audio
{
	public class BeatFinder
	{
		private TimeSpan[] beats = new TimeSpan[30];

		private TimeSpan timeSinceLastPeek = TimeSpan.Zero;

		private float averageBMP;

		private float[] historicalAverages = new float[50];

		private bool beatFound;

		public bool BeatFound
		{
			get
			{
				return beatFound;
			}
		}

		public float AverageBMP
		{
			get
			{
				return averageBMP;
			}
		}

		public void Update(VisualizationData visData, TimeSpan elaspedTime)
		{
			timeSinceLastPeek += elaspedTime;
			float num = 0f;
			for (int i = 0; i < visData.get_Samples().Count; i++)
			{
				float num2 = visData.get_Samples()[i];
				num2 *= num2;
				num += num2;
			}
			num /= (float)visData.get_Samples().Count;
			if (num == historicalAverages[historicalAverages.Length - 1])
			{
				return;
			}
			float num3 = 0f;
			for (int j = 0; j < historicalAverages.Length; j++)
			{
				num3 += historicalAverages[j];
			}
			num3 /= (float)historicalAverages.Length;
			float num4 = 0f;
			for (int k = 0; k < historicalAverages.Length; k++)
			{
				num4 += Math.Abs(historicalAverages[k] - num3);
			}
			num4 /= (float)historicalAverages.Length;
			float num5 = num - num3;
			if (num5 >= num4 && timeSinceLastPeek.TotalSeconds > 0.20000000298023224)
			{
				beatFound = true;
				TimeSpan zero = TimeSpan.Zero;
				for (int l = 0; l < beats.Length - 1; l++)
				{
					zero += beats[l];
					beats[l] = beats[l + 1];
				}
				averageBMP = 60f / (float)TimeSpan.FromSeconds(zero.TotalSeconds / (double)(beats.Length - 1)).TotalSeconds;
				beats[beats.Length - 1] = timeSinceLastPeek;
				timeSinceLastPeek = TimeSpan.Zero;
			}
			else
			{
				beatFound = false;
			}
			for (int m = 0; m < historicalAverages.Length - 1; m++)
			{
				historicalAverages[m] = historicalAverages[m + 1];
			}
			historicalAverages[historicalAverages.Length - 1] = num;
		}
	}
}
