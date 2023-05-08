using System;

namespace DNA.Drawing.Noise
{
	public class IntNoise
	{
		private static int[] _permute = new int[1024];

		public IntNoise()
		{
			Initalize(new Random());
		}

		public IntNoise(Random r)
		{
			Initalize(r);
		}

		private void Initalize(Random r)
		{
			for (int i = 0; i < 256; i++)
			{
				_permute[256 + i] = (_permute[i] = r.Next(256));
			}
			for (int j = 0; j < 512; j++)
			{
				_permute[512 + j] = _permute[j];
			}
		}

		public int ComputeNoise(IntVector3 v)
		{
			return ComputeNoise(v.X, v.Y, v.Z);
		}

		public int ComputeNoise(int x, int y, int z)
		{
			int num = x & 0xFF;
			int num2 = y & 0xFF;
			int num3 = z & 0xFF;
			int num4 = _permute[num] + num2;
			int num5 = _permute[num4] + num3;
			return _permute[num5];
		}

		public int ComputeNoise(int x, int y)
		{
			int num = x & 0xFF;
			int num2 = y & 0xFF;
			int num3 = _permute[num] + num2;
			return _permute[num3];
		}
	}
}
