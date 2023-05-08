using System;

namespace DNA
{
	public class FastRand
	{
		private const int IA = 16807;

		private const int IM = int.MaxValue;

		private const float AM = 4.656613E-10f;

		private const int IQ = 127773;

		private const int IR = 2836;

		private const int MASK = 123459876;

		private int _idnum;

		public int Seed
		{
			set
			{
				_idnum = value;
			}
		}

		public FastRand()
		{
			_idnum = (int)DateTime.Now.Ticks;
		}

		public FastRand(int seed)
		{
			_idnum = seed;
			GetNextValue();
		}

		public float GetNextValue()
		{
			float num2;
			do
			{
				_idnum ^= 123459876;
				int num = _idnum / 127773;
				_idnum = 16807 * (_idnum - num * 127773) - 2836 * num;
				if (_idnum < 0)
				{
					_idnum += int.MaxValue;
				}
				num2 = 4.656613E-10f * (float)_idnum;
				_idnum ^= 123459876;
				num2 = 1f - num2;
			}
			while (num2 >= 1f);
			return num2;
		}

		public int GetNextValue(int min, int max)
		{
			return (int)((float)(max - min) * GetNextValue()) + min;
		}

		public float GetNextValue(float min, float max)
		{
			float num = GetNextValue() * (max - min);
			return num + min;
		}
	}
}
