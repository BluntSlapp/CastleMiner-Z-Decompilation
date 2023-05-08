using System;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Security;

namespace DNA.Security.Cryptography.Utilities
{
	public sealed class BigIntegers
	{
		private const int MaxIterations = 1000;

		private BigIntegers()
		{
		}

		public static byte[] AsUnsignedByteArray(BigInteger n)
		{
			return n.ToByteArrayUnsigned();
		}

		public static BigInteger CreateRandomInRange(BigInteger min, BigInteger max, SecureRandom random)
		{
			int num = min.CompareTo(max);
			if (num >= 0)
			{
				if (num > 0)
				{
					throw new ArgumentException("'min' may not be greater than 'max'");
				}
				return min;
			}
			if (min.BitLength > max.BitLength / 2)
			{
				return CreateRandomInRange(BigInteger.Zero, max.Subtract(min), random).Add(min);
			}
			for (int i = 0; i < 1000; i++)
			{
				BigInteger bigInteger = new BigInteger(max.BitLength, random);
				if (bigInteger.CompareTo(min) >= 0 && bigInteger.CompareTo(max) <= 0)
				{
					return bigInteger;
				}
			}
			return new BigInteger(max.Subtract(min).BitLength - 1, random).Add(min);
		}
	}
}
