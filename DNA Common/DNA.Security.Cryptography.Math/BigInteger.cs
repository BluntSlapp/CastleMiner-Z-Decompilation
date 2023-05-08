using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DNA.Security.Cryptography.Math
{
	[Serializable]
	public class BigInteger
	{
		private const long IMASK = 4294967295L;

		private const int BitsPerByte = 8;

		private const int BitsPerInt = 32;

		private const int BytesPerInt = 4;

		private static readonly int[][] primeLists;

		private static readonly int[] primeProducts;

		private static readonly ulong UIMASK;

		private static readonly int[] ZeroMagnitude;

		private static readonly byte[] ZeroEncoding;

		public static readonly BigInteger Zero;

		public static readonly BigInteger One;

		public static readonly BigInteger Two;

		public static readonly BigInteger Three;

		public static readonly BigInteger Ten;

		private static readonly int chunk2;

		private static readonly BigInteger radix2;

		private static readonly BigInteger radix2E;

		private static readonly int chunk10;

		private static readonly BigInteger radix10;

		private static readonly BigInteger radix10E;

		private static readonly int chunk16;

		private static readonly BigInteger radix16;

		private static readonly BigInteger radix16E;

		private static readonly Random RandomSource;

		private int sign;

		private int[] magnitude;

		private int nBits = -1;

		private int nBitLength = -1;

		private long mQuote = -1L;

		private static readonly byte[] rndMask;

		private static readonly byte[] bitCounts;

		public int BitCount
		{
			get
			{
				if (nBits == -1)
				{
					if (sign < 0)
					{
						nBits = Not().BitCount;
					}
					else
					{
						int num = 0;
						for (int i = 0; i < magnitude.Length; i++)
						{
							num += bitCounts[(byte)magnitude[i]];
							num += bitCounts[(byte)(magnitude[i] >> 8)];
							num += bitCounts[(byte)(magnitude[i] >> 16)];
							num += bitCounts[(byte)(magnitude[i] >> 24)];
						}
						nBits = num;
					}
				}
				return nBits;
			}
		}

		public int BitLength
		{
			get
			{
				if (nBitLength == -1)
				{
					nBitLength = ((sign != 0) ? calcBitLength(0, magnitude) : 0);
				}
				return nBitLength;
			}
		}

		public int IntValue
		{
			get
			{
				if (sign != 0)
				{
					if (sign <= 0)
					{
						return -magnitude[magnitude.Length - 1];
					}
					return magnitude[magnitude.Length - 1];
				}
				return 0;
			}
		}

		public long LongValue
		{
			get
			{
				if (sign == 0)
				{
					return 0L;
				}
				long num = ((magnitude.Length <= 1) ? (magnitude[magnitude.Length - 1] & 0xFFFFFFFFu) : (((long)magnitude[magnitude.Length - 2] << 32) | (magnitude[magnitude.Length - 1] & 0xFFFFFFFFu)));
				if (sign >= 0)
				{
					return num;
				}
				return -num;
			}
		}

		public int SignValue
		{
			get
			{
				return sign;
			}
		}

		static BigInteger()
		{
			primeLists = new int[52][]
			{
				new int[8] { 3, 5, 7, 11, 13, 17, 19, 23 },
				new int[5] { 29, 31, 37, 41, 43 },
				new int[5] { 47, 53, 59, 61, 67 },
				new int[4] { 71, 73, 79, 83 },
				new int[4] { 89, 97, 101, 103 },
				new int[4] { 107, 109, 113, 127 },
				new int[4] { 131, 137, 139, 149 },
				new int[4] { 151, 157, 163, 167 },
				new int[4] { 173, 179, 181, 191 },
				new int[4] { 193, 197, 199, 211 },
				new int[3] { 223, 227, 229 },
				new int[3] { 233, 239, 241 },
				new int[3] { 251, 257, 263 },
				new int[3] { 269, 271, 277 },
				new int[3] { 281, 283, 293 },
				new int[3] { 307, 311, 313 },
				new int[3] { 317, 331, 337 },
				new int[3] { 347, 349, 353 },
				new int[3] { 359, 367, 373 },
				new int[3] { 379, 383, 389 },
				new int[3] { 397, 401, 409 },
				new int[3] { 419, 421, 431 },
				new int[3] { 433, 439, 443 },
				new int[3] { 449, 457, 461 },
				new int[3] { 463, 467, 479 },
				new int[3] { 487, 491, 499 },
				new int[3] { 503, 509, 521 },
				new int[3] { 523, 541, 547 },
				new int[3] { 557, 563, 569 },
				new int[3] { 571, 577, 587 },
				new int[3] { 593, 599, 601 },
				new int[3] { 607, 613, 617 },
				new int[3] { 619, 631, 641 },
				new int[3] { 643, 647, 653 },
				new int[3] { 659, 661, 673 },
				new int[3] { 677, 683, 691 },
				new int[3] { 701, 709, 719 },
				new int[3] { 727, 733, 739 },
				new int[3] { 743, 751, 757 },
				new int[3] { 761, 769, 773 },
				new int[3] { 787, 797, 809 },
				new int[3] { 811, 821, 823 },
				new int[3] { 827, 829, 839 },
				new int[3] { 853, 857, 859 },
				new int[3] { 863, 877, 881 },
				new int[3] { 883, 887, 907 },
				new int[3] { 911, 919, 929 },
				new int[3] { 937, 941, 947 },
				new int[3] { 953, 967, 971 },
				new int[3] { 977, 983, 991 },
				new int[3] { 997, 1009, 1013 },
				new int[3] { 1019, 1021, 1031 }
			};
			UIMASK = 4294967295uL;
			ZeroMagnitude = new int[0];
			ZeroEncoding = new byte[0];
			Zero = new BigInteger(0, ZeroMagnitude, false);
			One = createUValueOf(1uL);
			Two = createUValueOf(2uL);
			Three = createUValueOf(3uL);
			Ten = createUValueOf(10uL);
			chunk2 = 1;
			radix2 = ValueOf(2L);
			radix2E = radix2.Pow(chunk2);
			chunk10 = 19;
			radix10 = ValueOf(10L);
			radix10E = radix10.Pow(chunk10);
			chunk16 = 16;
			radix16 = ValueOf(16L);
			radix16E = radix16.Pow(chunk16);
			RandomSource = new Random();
			rndMask = new byte[8] { 255, 127, 63, 31, 15, 7, 3, 1 };
			bitCounts = new byte[256]
			{
				0, 1, 1, 2, 1, 2, 2, 3, 1, 2,
				2, 3, 2, 3, 3, 4, 1, 2, 2, 3,
				2, 3, 3, 4, 2, 3, 3, 4, 3, 4,
				4, 5, 1, 2, 2, 3, 2, 3, 3, 4,
				2, 3, 3, 4, 3, 4, 4, 5, 2, 3,
				3, 4, 3, 4, 4, 5, 3, 4, 4, 5,
				4, 5, 5, 6, 1, 2, 2, 3, 2, 3,
				3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
				2, 3, 3, 4, 3, 4, 4, 5, 3, 4,
				4, 5, 4, 5, 5, 6, 2, 3, 3, 4,
				3, 4, 4, 5, 3, 4, 4, 5, 4, 5,
				5, 6, 3, 4, 4, 5, 4, 5, 5, 6,
				4, 5, 5, 6, 5, 6, 6, 7, 1, 2,
				2, 3, 2, 3, 3, 4, 2, 3, 3, 4,
				3, 4, 4, 5, 2, 3, 3, 4, 3, 4,
				4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
				2, 3, 3, 4, 3, 4, 4, 5, 3, 4,
				4, 5, 4, 5, 5, 6, 3, 4, 4, 5,
				4, 5, 5, 6, 4, 5, 5, 6, 5, 6,
				6, 7, 2, 3, 3, 4, 3, 4, 4, 5,
				3, 4, 4, 5, 4, 5, 5, 6, 3, 4,
				4, 5, 4, 5, 5, 6, 4, 5, 5, 6,
				5, 6, 6, 7, 3, 4, 4, 5, 4, 5,
				5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
				4, 5, 5, 6, 5, 6, 6, 7, 5, 6,
				6, 7, 6, 7, 7, 8
			};
			primeProducts = new int[primeLists.Length];
			for (int i = 0; i < primeLists.Length; i++)
			{
				int[] array = primeLists[i];
				int num = 1;
				for (int j = 0; j < array.Length; j++)
				{
					num *= array[j];
				}
				primeProducts[i] = num;
			}
		}

		private static int GetByteLength(int nBits)
		{
			return (nBits + 8 - 1) / 8;
		}

		private BigInteger()
		{
		}

		private BigInteger(int signum, int[] mag, bool checkMag)
		{
			if (checkMag)
			{
				int i;
				for (i = 0; i < mag.Length && mag[i] == 0; i++)
				{
				}
				if (i == mag.Length)
				{
					magnitude = ZeroMagnitude;
					return;
				}
				sign = signum;
				if (i == 0)
				{
					magnitude = mag;
					return;
				}
				magnitude = new int[mag.Length - i];
				Array.Copy(mag, i, magnitude, 0, magnitude.Length);
			}
			else
			{
				sign = signum;
				magnitude = mag;
			}
		}

		public BigInteger(string value)
			: this(value, 10)
		{
		}

		public BigInteger(string str, int radix)
		{
			if (str.Length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			NumberStyles style;
			int num;
			BigInteger bigInteger;
			BigInteger val;
			switch (radix)
			{
			case 2:
				style = NumberStyles.Integer;
				num = chunk2;
				bigInteger = radix2;
				val = radix2E;
				break;
			case 10:
				style = NumberStyles.Integer;
				num = chunk10;
				bigInteger = radix10;
				val = radix10E;
				break;
			case 16:
				style = NumberStyles.AllowHexSpecifier;
				num = chunk16;
				bigInteger = radix16;
				val = radix16E;
				break;
			default:
				throw new FormatException("Only bases 2, 10, or 16 allowed");
			}
			int i = 0;
			sign = 1;
			if (str[0] == '-')
			{
				if (str.Length == 1)
				{
					throw new FormatException("Zero length BigInteger");
				}
				sign = -1;
				i = 1;
			}
			for (; i < str.Length && int.Parse(str[i].ToString(), style) == 0; i++)
			{
			}
			if (i >= str.Length)
			{
				sign = 0;
				magnitude = ZeroMagnitude;
				return;
			}
			BigInteger bigInteger2 = Zero;
			int num2 = i + num;
			if (num2 <= str.Length)
			{
				do
				{
					string text = str.Substring(i, num);
					ulong num3 = ulong.Parse(text, style);
					BigInteger value = createUValueOf(num3);
					switch (radix)
					{
					case 2:
						if (num3 > 1)
						{
							throw new FormatException("Bad character in radix 2 string: " + text);
						}
						bigInteger2 = bigInteger2.ShiftLeft(1);
						break;
					case 16:
						bigInteger2 = bigInteger2.ShiftLeft(64);
						break;
					default:
						bigInteger2 = bigInteger2.Multiply(val);
						break;
					}
					bigInteger2 = bigInteger2.Add(value);
					i = num2;
					num2 += num;
				}
				while (num2 <= str.Length);
			}
			if (i < str.Length)
			{
				string text2 = str.Substring(i);
				ulong value2 = ulong.Parse(text2, style);
				BigInteger bigInteger3 = createUValueOf(value2);
				if (bigInteger2.sign > 0)
				{
					switch (radix)
					{
					case 16:
						bigInteger2 = bigInteger2.ShiftLeft(text2.Length << 2);
						break;
					default:
						bigInteger2 = bigInteger2.Multiply(bigInteger.Pow(text2.Length));
						break;
					case 2:
						break;
					}
					bigInteger2 = bigInteger2.Add(bigInteger3);
				}
				else
				{
					bigInteger2 = bigInteger3;
				}
			}
			magnitude = bigInteger2.magnitude;
		}

		public BigInteger(byte[] bytes)
			: this(bytes, 0, bytes.Length)
		{
		}

		public BigInteger(byte[] bytes, int offset, int length)
		{
			if (length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			if ((sbyte)bytes[offset] < 0)
			{
				sign = -1;
				int num = offset + length;
				int i;
				for (i = offset; i < num && (sbyte)bytes[i] == -1; i++)
				{
				}
				if (i >= num)
				{
					magnitude = One.magnitude;
					return;
				}
				int num2 = num - i;
				byte[] array = new byte[num2];
				int num3 = 0;
				while (num3 < num2)
				{
					array[num3++] = (byte)(~bytes[i++]);
				}
				while (array[--num3] == byte.MaxValue)
				{
					array[num3] = 0;
				}
				array[num3]++;
				magnitude = MakeMagnitude(array, 0, array.Length);
			}
			else
			{
				magnitude = MakeMagnitude(bytes, offset, length);
				sign = ((magnitude.Length > 0) ? 1 : 0);
			}
		}

		private static int[] MakeMagnitude(byte[] bytes, int offset, int length)
		{
			int num = offset + length;
			int i;
			for (i = offset; i < num && bytes[i] == 0; i++)
			{
			}
			if (i >= num)
			{
				return ZeroMagnitude;
			}
			int num2 = (num - i + 3) / 4;
			int num3 = (num - i) % 4;
			if (num3 == 0)
			{
				num3 = 4;
			}
			if (num2 < 1)
			{
				return ZeroMagnitude;
			}
			int[] array = new int[num2];
			int num4 = 0;
			int num5 = 0;
			for (int j = i; j < num; j++)
			{
				num4 <<= 8;
				num4 |= bytes[j] & 0xFF;
				num3--;
				if (num3 <= 0)
				{
					array[num5] = num4;
					num5++;
					num3 = 4;
					num4 = 0;
				}
			}
			if (num5 < array.Length)
			{
				array[num5] = num4;
			}
			return array;
		}

		public BigInteger(int sign, byte[] bytes)
			: this(sign, bytes, 0, bytes.Length)
		{
		}

		public BigInteger(int sign, byte[] bytes, int offset, int length)
		{
			switch (sign)
			{
			default:
				throw new FormatException("Invalid sign value");
			case 0:
				magnitude = ZeroMagnitude;
				break;
			case -1:
			case 1:
				magnitude = MakeMagnitude(bytes, offset, length);
				this.sign = ((magnitude.Length >= 1) ? sign : 0);
				break;
			}
		}

		public BigInteger(int sizeInBits, Random random)
		{
			if (sizeInBits < 0)
			{
				throw new ArgumentException("sizeInBits must be non-negative");
			}
			nBits = -1;
			nBitLength = -1;
			if (sizeInBits == 0)
			{
				magnitude = ZeroMagnitude;
				return;
			}
			int byteLength = GetByteLength(sizeInBits);
			byte[] array = new byte[byteLength];
			random.NextBytes(array);
			array[0] &= rndMask[8 * byteLength - sizeInBits];
			magnitude = MakeMagnitude(array, 0, array.Length);
			sign = ((magnitude.Length >= 1) ? 1 : 0);
		}

		public BigInteger(int bitLength, int certainty, Random random)
		{
			if (bitLength < 2)
			{
				throw new ArithmeticException("bitLength < 2");
			}
			sign = 1;
			nBitLength = bitLength;
			if (bitLength == 2)
			{
				magnitude = ((random.Next(2) == 0) ? Two.magnitude : Three.magnitude);
				return;
			}
			int byteLength = GetByteLength(bitLength);
			byte[] array = new byte[byteLength];
			int num = 8 * byteLength - bitLength;
			byte b = rndMask[num];
			while (true)
			{
				random.NextBytes(array);
				array[0] &= b;
				array[0] |= (byte)(1 << 7 - num);
				array[byteLength - 1] |= 1;
				magnitude = MakeMagnitude(array, 0, array.Length);
				nBits = -1;
				mQuote = -1L;
				if (certainty < 1 || CheckProbablePrime(certainty, random))
				{
					break;
				}
				if (bitLength <= 32)
				{
					continue;
				}
				for (int i = 0; i < 10000; i++)
				{
					int num2 = 33 + random.Next(bitLength - 2);
					magnitude[magnitude.Length - (num2 >> 5)] ^= 1 << num2;
					magnitude[magnitude.Length - 1] ^= random.Next() + 1 << 1;
					mQuote = -1L;
					if (CheckProbablePrime(certainty, random))
					{
						return;
					}
				}
			}
		}

		public BigInteger Abs()
		{
			if (sign < 0)
			{
				return Negate();
			}
			return this;
		}

		private static int[] AddMagnitudes(int[] a, int[] b)
		{
			int num = a.Length - 1;
			int num2 = b.Length - 1;
			long num3 = 0L;
			while (num2 >= 0)
			{
				num3 += (long)(uint)a[num] + (long)(uint)b[num2--];
				a[num--] = (int)num3;
				num3 = (long)((ulong)num3 >> 32);
			}
			if (num3 != 0)
			{
				while (num >= 0 && ++a[num--] == 0)
				{
				}
			}
			return a;
		}

		public BigInteger Add(BigInteger value)
		{
			if (sign == 0)
			{
				return value;
			}
			if (sign != value.sign)
			{
				if (value.sign == 0)
				{
					return this;
				}
				if (value.sign < 0)
				{
					return Subtract(value.Negate());
				}
				return value.Subtract(Negate());
			}
			return AddToMagnitude(value.magnitude);
		}

		private BigInteger AddToMagnitude(int[] magToAdd)
		{
			int[] array;
			int[] array2;
			if (magnitude.Length < magToAdd.Length)
			{
				array = magToAdd;
				array2 = magnitude;
			}
			else
			{
				array = magnitude;
				array2 = magToAdd;
			}
			uint num = uint.MaxValue;
			if (array.Length == array2.Length)
			{
				num -= (uint)array2[0];
			}
			bool flag = (uint)array[0] >= num;
			int[] array3;
			if (flag)
			{
				array3 = new int[array.Length + 1];
				array.CopyTo(array3, 1);
			}
			else
			{
				array3 = (int[])array.Clone();
			}
			array3 = AddMagnitudes(array3, array2);
			return new BigInteger(sign, array3, flag);
		}

		public BigInteger And(BigInteger value)
		{
			if (sign == 0 || value.sign == 0)
			{
				return Zero;
			}
			int[] array = ((sign > 0) ? magnitude : Add(One).magnitude);
			int[] array2 = ((value.sign > 0) ? value.magnitude : value.Add(One).magnitude);
			bool flag = sign < 0 && value.sign < 0;
			int num = System.Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = ((i >= num2) ? array[i - num2] : 0);
				int num5 = ((i >= num3) ? array2[i - num3] : 0);
				if (sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = num4 & num5;
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger AndNot(BigInteger val)
		{
			return And(val.Not());
		}

		private int calcBitLength(int indx, int[] mag)
		{
			while (true)
			{
				if (indx >= mag.Length)
				{
					return 0;
				}
				if (mag[indx] != 0)
				{
					break;
				}
				indx++;
			}
			int num = 32 * (mag.Length - indx - 1);
			int num2 = mag[indx];
			num += BitLen(num2);
			if (sign < 0 && (num2 & -num2) == num2)
			{
				do
				{
					if (++indx >= mag.Length)
					{
						num--;
						break;
					}
				}
				while (mag[indx] == 0);
			}
			return num;
		}

		private static int BitLen(int w)
		{
			if (w >= 32768)
			{
				if (w >= 8388608)
				{
					if (w >= 134217728)
					{
						if (w >= 536870912)
						{
							if (w >= 1073741824)
							{
								return 31;
							}
							return 30;
						}
						if (w >= 268435456)
						{
							return 29;
						}
						return 28;
					}
					if (w >= 33554432)
					{
						if (w >= 67108864)
						{
							return 27;
						}
						return 26;
					}
					if (w >= 16777216)
					{
						return 25;
					}
					return 24;
				}
				if (w >= 524288)
				{
					if (w >= 2097152)
					{
						if (w >= 4194304)
						{
							return 23;
						}
						return 22;
					}
					if (w >= 1048576)
					{
						return 21;
					}
					return 20;
				}
				if (w >= 131072)
				{
					if (w >= 262144)
					{
						return 19;
					}
					return 18;
				}
				if (w >= 65536)
				{
					return 17;
				}
				return 16;
			}
			if (w >= 128)
			{
				if (w >= 2048)
				{
					if (w >= 8192)
					{
						if (w >= 16384)
						{
							return 15;
						}
						return 14;
					}
					if (w >= 4096)
					{
						return 13;
					}
					return 12;
				}
				if (w >= 512)
				{
					if (w >= 1024)
					{
						return 11;
					}
					return 10;
				}
				if (w >= 256)
				{
					return 9;
				}
				return 8;
			}
			if (w >= 8)
			{
				if (w >= 32)
				{
					if (w >= 64)
					{
						return 7;
					}
					return 6;
				}
				if (w >= 16)
				{
					return 5;
				}
				return 4;
			}
			if (w >= 2)
			{
				if (w >= 4)
				{
					return 3;
				}
				return 2;
			}
			if (w >= 1)
			{
				return 1;
			}
			if (w >= 0)
			{
				return 0;
			}
			return 32;
		}

		private bool QuickPow2Check()
		{
			if (sign > 0)
			{
				return nBits == 1;
			}
			return false;
		}

		public int CompareTo(object obj)
		{
			return CompareTo((BigInteger)obj);
		}

		private static int CompareTo(int xIndx, int[] x, int yIndx, int[] y)
		{
			while (xIndx != x.Length && x[xIndx] == 0)
			{
				xIndx++;
			}
			while (yIndx != y.Length && y[yIndx] == 0)
			{
				yIndx++;
			}
			return CompareNoLeadingZeroes(xIndx, x, yIndx, y);
		}

		private static int CompareNoLeadingZeroes(int xIndx, int[] x, int yIndx, int[] y)
		{
			int num = x.Length - y.Length - (xIndx - yIndx);
			if (num != 0)
			{
				if (num >= 0)
				{
					return 1;
				}
				return -1;
			}
			while (xIndx < x.Length)
			{
				uint num2 = (uint)x[xIndx++];
				uint num3 = (uint)y[yIndx++];
				if (num2 != num3)
				{
					if (num2 >= num3)
					{
						return 1;
					}
					return -1;
				}
			}
			return 0;
		}

		public int CompareTo(BigInteger value)
		{
			if (sign >= value.sign)
			{
				if (sign <= value.sign)
				{
					if (sign != 0)
					{
						return sign * CompareNoLeadingZeroes(0, magnitude, 0, value.magnitude);
					}
					return 0;
				}
				return 1;
			}
			return -1;
		}

		private int[] Divide(int[] x, int[] y)
		{
			int i;
			for (i = 0; i < x.Length && x[i] == 0; i++)
			{
			}
			int j;
			for (j = 0; j < y.Length && y[j] == 0; j++)
			{
			}
			int num = CompareNoLeadingZeroes(i, x, j, y);
			int[] array3;
			if (num > 0)
			{
				int num2 = calcBitLength(j, y);
				int num3 = calcBitLength(i, x);
				int num4 = num3 - num2;
				int k = 0;
				int l = 0;
				int num5 = num2;
				int[] array;
				int[] array2;
				if (num4 > 0)
				{
					array = new int[(num4 >> 5) + 1];
					array[0] = 1 << num4 % 32;
					array2 = ShiftLeft(y, num4);
					num5 += num4;
				}
				else
				{
					array = new int[1] { 1 };
					int num6 = y.Length - j;
					array2 = new int[num6];
					Array.Copy(y, j, array2, 0, num6);
				}
				array3 = new int[array.Length];
				while (true)
				{
					if (num5 < num3 || CompareNoLeadingZeroes(i, x, l, array2) >= 0)
					{
						Subtract(i, x, l, array2);
						AddMagnitudes(array3, array);
						while (x[i] == 0)
						{
							if (++i == x.Length)
							{
								return array3;
							}
						}
						num3 = 32 * (x.Length - i - 1) + BitLen(x[i]);
						if (num3 <= num2)
						{
							if (num3 < num2)
							{
								return array3;
							}
							num = CompareNoLeadingZeroes(i, x, j, y);
							if (num <= 0)
							{
								break;
							}
						}
					}
					num4 = num5 - num3;
					if (num4 == 1)
					{
						uint num7 = (uint)array2[l] >> 1;
						uint num8 = (uint)x[i];
						if (num7 > num8)
						{
							num4++;
						}
					}
					if (num4 < 2)
					{
						array2 = ShiftRightOneInPlace(l, array2);
						num5--;
						array = ShiftRightOneInPlace(k, array);
					}
					else
					{
						array2 = ShiftRightInPlace(l, array2, num4);
						num5 -= num4;
						array = ShiftRightInPlace(k, array, num4);
					}
					for (; array2[l] == 0; l++)
					{
					}
					for (; array[k] == 0; k++)
					{
					}
				}
			}
			else
			{
				array3 = new int[1];
			}
			if (num == 0)
			{
				AddMagnitudes(array3, One.magnitude);
				Array.Clear(x, i, x.Length - i);
			}
			return array3;
		}

		public BigInteger Divide(BigInteger val)
		{
			if (val.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (sign == 0)
			{
				return Zero;
			}
			if (val.QuickPow2Check())
			{
				BigInteger bigInteger = Abs().ShiftRight(val.Abs().BitLength - 1);
				if (val.sign != sign)
				{
					return bigInteger.Negate();
				}
				return bigInteger;
			}
			int[] x = (int[])magnitude.Clone();
			return new BigInteger(sign * val.sign, Divide(x, val.magnitude), true);
		}

		public BigInteger[] DivideAndRemainder(BigInteger val)
		{
			if (val.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			BigInteger[] array = new BigInteger[2];
			if (sign == 0)
			{
				array[0] = Zero;
				array[1] = Zero;
			}
			else if (val.QuickPow2Check())
			{
				int n = val.Abs().BitLength - 1;
				BigInteger bigInteger = Abs().ShiftRight(n);
				int[] mag = LastNBits(n);
				array[0] = ((val.sign == sign) ? bigInteger : bigInteger.Negate());
				array[1] = new BigInteger(sign, mag, true);
			}
			else
			{
				int[] array2 = (int[])magnitude.Clone();
				int[] mag2 = Divide(array2, val.magnitude);
				array[0] = new BigInteger(sign * val.sign, mag2, true);
				array[1] = new BigInteger(sign, array2, true);
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			BigInteger bigInteger = obj as BigInteger;
			if (bigInteger == null)
			{
				return false;
			}
			if (bigInteger.sign != sign || bigInteger.magnitude.Length != magnitude.Length)
			{
				return false;
			}
			for (int i = 0; i < magnitude.Length; i++)
			{
				if (bigInteger.magnitude[i] != magnitude[i])
				{
					return false;
				}
			}
			return true;
		}

		public BigInteger Gcd(BigInteger value)
		{
			if (value.sign == 0)
			{
				return Abs();
			}
			if (sign == 0)
			{
				return value.Abs();
			}
			BigInteger bigInteger = this;
			BigInteger bigInteger2 = value;
			while (bigInteger2.sign != 0)
			{
				BigInteger bigInteger3 = bigInteger.Mod(bigInteger2);
				bigInteger = bigInteger2;
				bigInteger2 = bigInteger3;
			}
			return bigInteger;
		}

		public override int GetHashCode()
		{
			int num = magnitude.Length;
			if (magnitude.Length > 0)
			{
				num ^= magnitude[0];
				if (magnitude.Length > 1)
				{
					num ^= magnitude[magnitude.Length - 1];
				}
			}
			if (sign >= 0)
			{
				return num;
			}
			return ~num;
		}

		private BigInteger Inc()
		{
			if (sign == 0)
			{
				return One;
			}
			if (sign < 0)
			{
				return new BigInteger(-1, doSubBigLil(magnitude, One.magnitude), true);
			}
			return AddToMagnitude(One.magnitude);
		}

		public bool IsProbablePrime(int certainty)
		{
			if (certainty <= 0)
			{
				return true;
			}
			BigInteger bigInteger = Abs();
			if (!bigInteger.TestBit(0))
			{
				return bigInteger.Equals(Two);
			}
			if (bigInteger.Equals(One))
			{
				return false;
			}
			return bigInteger.CheckProbablePrime(certainty, RandomSource);
		}

		private bool CheckProbablePrime(int certainty, Random random)
		{
			int num = System.Math.Min(BitLength - 1, primeLists.Length);
			for (int i = 0; i < num; i++)
			{
				int num2 = Remainder(primeProducts[i]);
				int[] array = primeLists[i];
				foreach (int num3 in array)
				{
					if (num2 % num3 == 0)
					{
						if (BitLength < 16)
						{
							return IntValue == num3;
						}
						return false;
					}
				}
			}
			return RabinMillerTest(certainty, random);
		}

		internal bool RabinMillerTest(int certainty, Random random)
		{
			BigInteger bigInteger = Subtract(One);
			int lowestSetBit = bigInteger.GetLowestSetBit();
			BigInteger exponent = bigInteger.ShiftRight(lowestSetBit);
			while (true)
			{
				BigInteger bigInteger2 = new BigInteger(BitLength, random);
				if (bigInteger2.CompareTo(One) <= 0 || bigInteger2.CompareTo(bigInteger) >= 0)
				{
					continue;
				}
				BigInteger bigInteger3 = bigInteger2.ModPow(exponent, this);
				if (!bigInteger3.Equals(One))
				{
					int num = 0;
					while (!bigInteger3.Equals(bigInteger))
					{
						if (++num == lowestSetBit)
						{
							return false;
						}
						bigInteger3 = bigInteger3.ModPow(Two, this);
						if (bigInteger3.Equals(One))
						{
							return false;
						}
					}
				}
				certainty -= 2;
				if (certainty <= 0)
				{
					break;
				}
			}
			return true;
		}

		public BigInteger Max(BigInteger value)
		{
			if (CompareTo(value) <= 0)
			{
				return value;
			}
			return this;
		}

		public BigInteger Min(BigInteger value)
		{
			if (CompareTo(value) >= 0)
			{
				return value;
			}
			return this;
		}

		public BigInteger Mod(BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			BigInteger bigInteger = Remainder(m);
			if (bigInteger.sign < 0)
			{
				return bigInteger.Add(m);
			}
			return bigInteger;
		}

		public BigInteger ModInverse(BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			BigInteger bigInteger = new BigInteger();
			BigInteger bigInteger2 = ExtEuclid(Mod(m), m, bigInteger, null);
			if (!bigInteger2.Equals(One))
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			if (bigInteger.sign < 0)
			{
				bigInteger.sign = 1;
				bigInteger.magnitude = doSubBigLil(m.magnitude, bigInteger.magnitude);
			}
			return bigInteger;
		}

		private static BigInteger ExtEuclid(BigInteger a, BigInteger b, BigInteger u1Out, BigInteger u2Out)
		{
			BigInteger bigInteger = One;
			BigInteger bigInteger2 = a;
			BigInteger bigInteger3 = Zero;
			BigInteger bigInteger4 = b;
			while (bigInteger4.sign > 0)
			{
				BigInteger[] array = bigInteger2.DivideAndRemainder(bigInteger4);
				BigInteger n = bigInteger3.Multiply(array[0]);
				BigInteger bigInteger5 = bigInteger.Subtract(n);
				bigInteger = bigInteger3;
				bigInteger3 = bigInteger5;
				bigInteger2 = bigInteger4;
				bigInteger4 = array[1];
			}
			if (u1Out != null)
			{
				u1Out.sign = bigInteger.sign;
				u1Out.magnitude = bigInteger.magnitude;
			}
			if (u2Out != null)
			{
				BigInteger n2 = bigInteger.Multiply(a);
				n2 = bigInteger2.Subtract(n2);
				BigInteger bigInteger6 = n2.Divide(b);
				u2Out.sign = bigInteger6.sign;
				u2Out.magnitude = bigInteger6.magnitude;
			}
			return bigInteger2;
		}

		private static void ZeroOut(int[] x)
		{
			Array.Clear(x, 0, x.Length);
		}

		public BigInteger ModPow(BigInteger exponent, BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			if (m.Equals(One))
			{
				return Zero;
			}
			if (exponent.sign == 0)
			{
				return One;
			}
			if (sign == 0)
			{
				return Zero;
			}
			int[] array = null;
			int[] array2 = null;
			bool flag = (m.magnitude[m.magnitude.Length - 1] & 1) == 1;
			long num = 0L;
			if (flag)
			{
				num = m.GetMQuote();
				BigInteger bigInteger = ShiftLeft(32 * m.magnitude.Length).Mod(m);
				array = bigInteger.magnitude;
				flag = array.Length <= m.magnitude.Length;
				if (flag)
				{
					array2 = new int[m.magnitude.Length + 1];
					if (array.Length < m.magnitude.Length)
					{
						int[] array3 = new int[m.magnitude.Length];
						array.CopyTo(array3, array3.Length - array.Length);
						array = array3;
					}
				}
			}
			if (!flag)
			{
				if (magnitude.Length <= m.magnitude.Length)
				{
					array = new int[m.magnitude.Length];
					magnitude.CopyTo(array, array.Length - magnitude.Length);
				}
				else
				{
					BigInteger bigInteger2 = Remainder(m);
					array = new int[m.magnitude.Length];
					bigInteger2.magnitude.CopyTo(array, array.Length - bigInteger2.magnitude.Length);
				}
				array2 = new int[m.magnitude.Length * 2];
			}
			int[] array4 = new int[m.magnitude.Length];
			for (int i = 0; i < exponent.magnitude.Length; i++)
			{
				int num2 = exponent.magnitude[i];
				int j = 0;
				if (i == 0)
				{
					while (num2 > 0)
					{
						num2 <<= 1;
						j++;
					}
					array.CopyTo(array4, 0);
					num2 <<= 1;
					j++;
				}
				while (num2 != 0)
				{
					if (flag)
					{
						MultiplyMonty(array2, array4, array4, m.magnitude, num);
					}
					else
					{
						Square(array2, array4);
						Remainder(array2, m.magnitude);
						Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
						ZeroOut(array2);
					}
					j++;
					if (num2 < 0)
					{
						if (flag)
						{
							MultiplyMonty(array2, array4, array, m.magnitude, num);
						}
						else
						{
							Multiply(array2, array4, array);
							Remainder(array2, m.magnitude);
							Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
							ZeroOut(array2);
						}
					}
					num2 <<= 1;
				}
				for (; j < 32; j++)
				{
					if (flag)
					{
						MultiplyMonty(array2, array4, array4, m.magnitude, num);
						continue;
					}
					Square(array2, array4);
					Remainder(array2, m.magnitude);
					Array.Copy(array2, array2.Length - array4.Length, array4, 0, array4.Length);
					ZeroOut(array2);
				}
			}
			if (flag)
			{
				ZeroOut(array);
				array[array.Length - 1] = 1;
				MultiplyMonty(array2, array4, array, m.magnitude, num);
			}
			BigInteger bigInteger3 = new BigInteger(1, array4, true);
			if (exponent.sign <= 0)
			{
				return bigInteger3.ModInverse(m);
			}
			return bigInteger3;
		}

		private static int[] Square(int[] w, int[] x)
		{
			int num = w.Length - 1;
			ulong num5;
			ulong num4;
			for (int num2 = x.Length - 1; num2 != 0; num2--)
			{
				ulong num3 = (uint)x[num2];
				num4 = num3 * num3;
				num5 = num4 >> 32;
				num4 = (uint)num4;
				num4 += (uint)w[num];
				w[num] = (int)num4;
				ulong num6 = num5 + (num4 >> 32);
				for (int num7 = num2 - 1; num7 >= 0; num7--)
				{
					num--;
					num4 = num3 * (uint)x[num7];
					num5 = num4 >> 31;
					num4 = (uint)(num4 << 1);
					num4 += num6 + (uint)w[num];
					w[num] = (int)num4;
					num6 = num5 + (num4 >> 32);
				}
				num6 += (uint)w[--num];
				w[num] = (int)num6;
				if (--num >= 0)
				{
					w[num] = (int)(num6 >> 32);
				}
				num += num2;
			}
			num4 = (uint)x[0];
			num4 *= num4;
			num5 = num4 >> 32;
			num4 &= 0xFFFFFFFFu;
			num4 += (uint)w[num];
			w[num] = (int)num4;
			if (--num >= 0)
			{
				w[num] = (int)(num5 + (num4 >> 32) + (uint)w[num]);
			}
			return w;
		}

		private static int[] Multiply(int[] x, int[] y, int[] z)
		{
			int num = z.Length;
			if (num < 1)
			{
				return x;
			}
			int num2 = x.Length - y.Length;
			long num4;
			while (true)
			{
				long num3 = z[--num] & 0xFFFFFFFFu;
				num4 = 0L;
				for (int num5 = y.Length - 1; num5 >= 0; num5--)
				{
					num4 += num3 * (y[num5] & 0xFFFFFFFFu) + (x[num2 + num5] & 0xFFFFFFFFu);
					x[num2 + num5] = (int)num4;
					num4 = (long)((ulong)num4 >> 32);
				}
				num2--;
				if (num < 1)
				{
					break;
				}
				x[num2] = (int)num4;
			}
			if (num2 >= 0)
			{
				x[num2] = (int)num4;
			}
			return x;
		}

		private static long FastExtEuclid(long a, long b, long[] uOut)
		{
			long num = 1L;
			long num2 = a;
			long num3 = 0L;
			long num4 = b;
			while (num4 > 0)
			{
				long num5 = num2 / num4;
				long num6 = num - num3 * num5;
				num = num3;
				num3 = num6;
				num6 = num2 - num4 * num5;
				num2 = num4;
				num4 = num6;
			}
			uOut[0] = num;
			uOut[1] = (num2 - num * a) / b;
			return num2;
		}

		private static long FastModInverse(long v, long m)
		{
			if (m < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			long[] array = new long[2];
			long num = FastExtEuclid(v, m, array);
			if (num != 1)
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			if (array[0] < 0)
			{
				array[0] += m;
			}
			return array[0];
		}

		private long GetMQuote()
		{
			if (mQuote != -1)
			{
				return mQuote;
			}
			if (magnitude.Length == 0 || (magnitude[magnitude.Length - 1] & 1) == 0)
			{
				return -1L;
			}
			long v = (~magnitude[magnitude.Length - 1] | 1) & 0xFFFFFFFFu;
			mQuote = FastModInverse(v, 4294967296L);
			return mQuote;
		}

		private static void MultiplyMonty(int[] a, int[] x, int[] y, int[] m, long mQuote)
		{
			if (m.Length == 1)
			{
				x[0] = (int)MultiplyMontyNIsOne((uint)x[0], (uint)y[0], (uint)m[0], (ulong)mQuote);
				return;
			}
			int num = m.Length;
			int num2 = num - 1;
			long num3 = y[num2] & 0xFFFFFFFFu;
			Array.Clear(a, 0, num + 1);
			for (int num4 = num; num4 > 0; num4--)
			{
				long num5 = x[num4 - 1] & 0xFFFFFFFFu;
				long num6 = ((((a[num] & 0xFFFFFFFFu) + ((num5 * num3) & 0xFFFFFFFFu)) & 0xFFFFFFFFu) * mQuote) & 0xFFFFFFFFu;
				long num7 = num5 * num3;
				long num8 = num6 * (m[num2] & 0xFFFFFFFFu);
				long num9 = (a[num] & 0xFFFFFFFFu) + (num7 & 0xFFFFFFFFu) + (num8 & 0xFFFFFFFFu);
				long num10 = (long)(((ulong)num7 >> 32) + ((ulong)num8 >> 32) + ((ulong)num9 >> 32));
				for (int num11 = num2; num11 > 0; num11--)
				{
					num7 = num5 * (y[num11 - 1] & 0xFFFFFFFFu);
					num8 = num6 * (m[num11 - 1] & 0xFFFFFFFFu);
					num9 = (a[num11] & 0xFFFFFFFFu) + (num7 & 0xFFFFFFFFu) + (num8 & 0xFFFFFFFFu) + (num10 & 0xFFFFFFFFu);
					num10 = (long)(((ulong)num10 >> 32) + ((ulong)num7 >> 32) + ((ulong)num8 >> 32) + ((ulong)num9 >> 32));
					a[num11 + 1] = (int)num9;
				}
				num10 += a[0] & 0xFFFFFFFFu;
				a[1] = (int)num10;
				a[0] = (int)((ulong)num10 >> 32);
			}
			if (CompareTo(0, a, 0, m) >= 0)
			{
				Subtract(0, a, 0, m);
			}
			Array.Copy(a, 1, x, 0, num);
		}

		private static uint MultiplyMontyNIsOne(uint x, uint y, uint m, ulong mQuote)
		{
			ulong num = m;
			ulong num2 = (ulong)x * (ulong)y;
			ulong num3 = (num2 * mQuote) & UIMASK;
			ulong num4 = num3 * num;
			ulong num5 = (num2 & UIMASK) + (num4 & UIMASK);
			ulong num6 = (num2 >> 32) + (num4 >> 32) + (num5 >> 32);
			if (num6 > num)
			{
				num6 -= num;
			}
			return (uint)(num6 & UIMASK);
		}

		public BigInteger Multiply(BigInteger val)
		{
			if (sign == 0 || val.sign == 0)
			{
				return Zero;
			}
			if (val.QuickPow2Check())
			{
				BigInteger bigInteger = ShiftLeft(val.Abs().BitLength - 1);
				if (val.sign <= 0)
				{
					return bigInteger.Negate();
				}
				return bigInteger;
			}
			if (QuickPow2Check())
			{
				BigInteger bigInteger2 = val.ShiftLeft(Abs().BitLength - 1);
				if (sign <= 0)
				{
					return bigInteger2.Negate();
				}
				return bigInteger2;
			}
			int num = (BitLength + val.BitLength) / 32 + 1;
			int[] array = new int[num];
			if (val == this)
			{
				Square(array, magnitude);
			}
			else
			{
				Multiply(array, magnitude, val.magnitude);
			}
			return new BigInteger(sign * val.sign, array, true);
		}

		public BigInteger Negate()
		{
			if (sign == 0)
			{
				return this;
			}
			return new BigInteger(-sign, magnitude, false);
		}

		public BigInteger NextProbablePrime()
		{
			if (sign < 0)
			{
				throw new ArithmeticException("Cannot be called on value < 0");
			}
			if (CompareTo(Two) < 0)
			{
				return Two;
			}
			BigInteger bigInteger = Inc().SetBit(0);
			while (!bigInteger.CheckProbablePrime(100, RandomSource))
			{
				bigInteger = bigInteger.Add(Two);
			}
			return bigInteger;
		}

		public BigInteger Not()
		{
			return Inc().Negate();
		}

		public BigInteger Pow(int exp)
		{
			if (exp < 0)
			{
				throw new ArithmeticException("Negative exponent");
			}
			if (exp == 0)
			{
				return One;
			}
			if (sign == 0 || Equals(One))
			{
				return this;
			}
			BigInteger bigInteger = One;
			BigInteger bigInteger2 = this;
			while (true)
			{
				if ((exp & 1) == 1)
				{
					bigInteger = bigInteger.Multiply(bigInteger2);
				}
				exp >>= 1;
				if (exp == 0)
				{
					break;
				}
				bigInteger2 = bigInteger2.Multiply(bigInteger2);
			}
			return bigInteger;
		}

		public static BigInteger ProbablePrime(int bitLength, Random random)
		{
			return new BigInteger(bitLength, 100, random);
		}

		private int Remainder(int m)
		{
			long num = 0L;
			for (int i = 0; i < magnitude.Length; i++)
			{
				long num2 = (uint)magnitude[i];
				num = ((num << 32) | num2) % m;
			}
			return (int)num;
		}

		private int[] Remainder(int[] x, int[] y)
		{
			int i;
			for (i = 0; i < x.Length && x[i] == 0; i++)
			{
			}
			int j;
			for (j = 0; j < y.Length && y[j] == 0; j++)
			{
			}
			int num = CompareNoLeadingZeroes(i, x, j, y);
			if (num > 0)
			{
				int num2 = calcBitLength(j, y);
				int num3 = calcBitLength(i, x);
				int num4 = num3 - num2;
				int k = 0;
				int num5 = num2;
				int[] array;
				if (num4 > 0)
				{
					array = ShiftLeft(y, num4);
					num5 += num4;
				}
				else
				{
					int num6 = y.Length - j;
					array = new int[num6];
					Array.Copy(y, j, array, 0, num6);
				}
				while (true)
				{
					if (num5 < num3 || CompareNoLeadingZeroes(i, x, k, array) >= 0)
					{
						Subtract(i, x, k, array);
						while (x[i] == 0)
						{
							if (++i == x.Length)
							{
								return x;
							}
						}
						num3 = 32 * (x.Length - i - 1) + BitLen(x[i]);
						if (num3 <= num2)
						{
							if (num3 < num2)
							{
								return x;
							}
							num = CompareNoLeadingZeroes(i, x, j, y);
							if (num <= 0)
							{
								break;
							}
						}
					}
					num4 = num5 - num3;
					if (num4 == 1)
					{
						uint num7 = (uint)array[k] >> 1;
						uint num8 = (uint)x[i];
						if (num7 > num8)
						{
							num4++;
						}
					}
					if (num4 < 2)
					{
						array = ShiftRightOneInPlace(k, array);
						num5--;
					}
					else
					{
						array = ShiftRightInPlace(k, array, num4);
						num5 -= num4;
					}
					for (; array[k] == 0; k++)
					{
					}
				}
			}
			if (num == 0)
			{
				Array.Clear(x, i, x.Length - i);
			}
			return x;
		}

		public BigInteger Remainder(BigInteger n)
		{
			if (n.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (sign == 0)
			{
				return Zero;
			}
			if (n.magnitude.Length == 1)
			{
				int num = n.magnitude[0];
				if (num > 0)
				{
					if (num == 1)
					{
						return Zero;
					}
					int num2 = Remainder(num);
					if (num2 != 0)
					{
						return new BigInteger(sign, new int[1] { num2 }, false);
					}
					return Zero;
				}
			}
			if (CompareNoLeadingZeroes(0, magnitude, 0, n.magnitude) < 0)
			{
				return this;
			}
			int[] mag;
			if (n.QuickPow2Check())
			{
				mag = LastNBits(n.Abs().BitLength - 1);
			}
			else
			{
				mag = (int[])magnitude.Clone();
				mag = Remainder(mag, n.magnitude);
			}
			return new BigInteger(sign, mag, true);
		}

		private int[] LastNBits(int n)
		{
			if (n < 1)
			{
				return ZeroMagnitude;
			}
			int val = (n + 32 - 1) / 32;
			val = System.Math.Min(val, magnitude.Length);
			int[] array = new int[val];
			Array.Copy(magnitude, magnitude.Length - val, array, 0, val);
			int num = n % 32;
			if (num != 0)
			{
				array[0] &= ~(-1 << num);
			}
			return array;
		}

		private static int[] ShiftLeft(int[] mag, int n)
		{
			int num = (int)((uint)n >> 5);
			int num2 = n & 0x1F;
			int num3 = mag.Length;
			int[] array;
			if (num2 == 0)
			{
				array = new int[num3 + num];
				mag.CopyTo(array, 0);
			}
			else
			{
				int num4 = 0;
				int num5 = 32 - num2;
				int num6 = (int)((uint)mag[0] >> num5);
				if (num6 != 0)
				{
					array = new int[num3 + num + 1];
					array[num4++] = num6;
				}
				else
				{
					array = new int[num3 + num];
				}
				int num7 = mag[0];
				for (int i = 0; i < num3 - 1; i++)
				{
					int num8 = mag[i + 1];
					array[num4++] = (num7 << num2) | (int)((uint)num8 >> num5);
					num7 = num8;
				}
				array[num4] = mag[num3 - 1] << num2;
			}
			return array;
		}

		public BigInteger ShiftLeft(int n)
		{
			if (sign == 0 || magnitude.Length == 0)
			{
				return Zero;
			}
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return ShiftRight(-n);
			}
			BigInteger bigInteger = new BigInteger(sign, ShiftLeft(magnitude, n), true);
			if (nBits != -1)
			{
				bigInteger.nBits = ((sign > 0) ? nBits : (nBits + n));
			}
			if (nBitLength != -1)
			{
				bigInteger.nBitLength = nBitLength + n;
			}
			return bigInteger;
		}

		private static int[] ShiftRightInPlace(int start, int[] mag, int n)
		{
			int num = (int)((uint)n >> 5) + start;
			int num2 = n & 0x1F;
			int num3 = mag.Length - 1;
			if (num != start)
			{
				int num4 = num - start;
				for (int num5 = num3; num5 >= num; num5--)
				{
					mag[num5] = mag[num5 - num4];
				}
				for (int num6 = num - 1; num6 >= start; num6--)
				{
					mag[num6] = 0;
				}
			}
			if (num2 != 0)
			{
				int num7 = 32 - num2;
				int num8 = mag[num3];
				for (int num9 = num3; num9 > num; num9--)
				{
					int num10 = mag[num9 - 1];
					mag[num9] = (int)((uint)num8 >> num2) | (num10 << num7);
					num8 = num10;
				}
				mag[num] = (int)((uint)mag[num] >> num2);
			}
			return mag;
		}

		private static int[] ShiftRightOneInPlace(int start, int[] mag)
		{
			int num = mag.Length;
			int num2 = mag[num - 1];
			while (--num > start)
			{
				int num3 = mag[num - 1];
				mag[num] = (int)((uint)num2 >> 1) | (num3 << 31);
				num2 = num3;
			}
			mag[start] = (int)((uint)mag[start] >> 1);
			return mag;
		}

		public BigInteger ShiftRight(int n)
		{
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return ShiftLeft(-n);
			}
			if (n >= BitLength)
			{
				if (sign >= 0)
				{
					return Zero;
				}
				return One.Negate();
			}
			int num = BitLength - n + 31 >> 5;
			int[] array = new int[num];
			int num2 = n >> 5;
			int num3 = n & 0x1F;
			if (num3 == 0)
			{
				Array.Copy(magnitude, 0, array, 0, array.Length);
			}
			else
			{
				int num4 = 32 - num3;
				int num5 = magnitude.Length - 1 - num2;
				for (int num6 = num - 1; num6 >= 0; num6--)
				{
					array[num6] = (int)((uint)magnitude[num5--] >> num3);
					if (num5 >= 0)
					{
						array[num6] |= magnitude[num5] << num4;
					}
				}
			}
			return new BigInteger(sign, array, false);
		}

		private static int[] Subtract(int xStart, int[] x, int yStart, int[] y)
		{
			int num = x.Length;
			int num2 = y.Length;
			int num3 = 0;
			do
			{
				long num4 = (x[--num] & 0xFFFFFFFFu) - (y[--num2] & 0xFFFFFFFFu) + num3;
				x[num] = (int)num4;
				num3 = (int)(num4 >> 63);
			}
			while (num2 > yStart);
			if (num3 != 0)
			{
				while (--x[--num] == -1)
				{
				}
			}
			return x;
		}

		public BigInteger Subtract(BigInteger n)
		{
			if (n.sign == 0)
			{
				return this;
			}
			if (sign == 0)
			{
				return n.Negate();
			}
			if (sign != n.sign)
			{
				return Add(n.Negate());
			}
			int num = CompareNoLeadingZeroes(0, magnitude, 0, n.magnitude);
			if (num == 0)
			{
				return Zero;
			}
			BigInteger bigInteger;
			BigInteger bigInteger2;
			if (num < 0)
			{
				bigInteger = n;
				bigInteger2 = this;
			}
			else
			{
				bigInteger = this;
				bigInteger2 = n;
			}
			return new BigInteger(sign * num, doSubBigLil(bigInteger.magnitude, bigInteger2.magnitude), true);
		}

		private static int[] doSubBigLil(int[] bigMag, int[] lilMag)
		{
			int[] x = (int[])bigMag.Clone();
			return Subtract(0, x, 0, lilMag);
		}

		public byte[] ToByteArray()
		{
			return ToByteArray(false);
		}

		public byte[] ToByteArrayUnsigned()
		{
			return ToByteArray(true);
		}

		private byte[] ToByteArray(bool unsigned)
		{
			if (sign == 0)
			{
				if (!unsigned)
				{
					return new byte[1];
				}
				return ZeroEncoding;
			}
			int num = ((unsigned && sign > 0) ? BitLength : (BitLength + 1));
			int byteLength = GetByteLength(num);
			byte[] array = new byte[byteLength];
			int num2 = magnitude.Length;
			int num3 = array.Length;
			if (sign > 0)
			{
				while (num2 > 1)
				{
					uint num4 = (uint)magnitude[--num2];
					array[--num3] = (byte)num4;
					array[--num3] = (byte)(num4 >> 8);
					array[--num3] = (byte)(num4 >> 16);
					array[--num3] = (byte)(num4 >> 24);
				}
				uint num5;
				for (num5 = (uint)magnitude[0]; num5 > 255; num5 >>= 8)
				{
					array[--num3] = (byte)num5;
				}
				array[--num3] = (byte)num5;
			}
			else
			{
				bool flag = true;
				while (num2 > 1)
				{
					uint num6 = (uint)(~magnitude[--num2]);
					if (flag)
					{
						flag = ++num6 == 0;
					}
					array[--num3] = (byte)num6;
					array[--num3] = (byte)(num6 >> 8);
					array[--num3] = (byte)(num6 >> 16);
					array[--num3] = (byte)(num6 >> 24);
				}
				uint num7 = (uint)magnitude[0];
				if (flag)
				{
					num7--;
				}
				while (num7 > 255)
				{
					array[--num3] = (byte)(~num7);
					num7 >>= 8;
				}
				array[--num3] = (byte)(~num7);
				if (num3 > 0)
				{
					array[--num3] = byte.MaxValue;
				}
			}
			return array;
		}

		public override string ToString()
		{
			return ToString(10);
		}

		public string ToString(int radix)
		{
			if (radix != 2 && radix != 10 && radix != 16)
			{
				throw new FormatException("Only bases 2, 10, 16 are allowed");
			}
			if (magnitude == null)
			{
				return "null";
			}
			if (sign == 0)
			{
				return "0";
			}
			StringBuilder stringBuilder = new StringBuilder();
			switch (radix)
			{
			case 16:
			{
				stringBuilder.Append(magnitude[0].ToString("x"));
				for (int i = 1; i < magnitude.Length; i++)
				{
					stringBuilder.Append(magnitude[i].ToString("x8"));
				}
				break;
			}
			case 2:
			{
				stringBuilder.Append('1');
				for (int num = BitLength - 2; num >= 0; num--)
				{
					stringBuilder.Append(TestBit(num) ? '1' : '0');
				}
				break;
			}
			default:
			{
				Stack<string> stack = new Stack<string>();
				BigInteger bigInteger = ValueOf(radix);
				BigInteger bigInteger2 = Abs();
				while (bigInteger2.sign != 0)
				{
					BigInteger bigInteger3 = bigInteger2.Mod(bigInteger);
					if (bigInteger3.sign == 0)
					{
						stack.Push("0");
					}
					else
					{
						stack.Push(bigInteger3.magnitude[0].ToString("d"));
					}
					bigInteger2 = bigInteger2.Divide(bigInteger);
				}
				while (stack.Count != 0)
				{
					stringBuilder.Append(stack.Pop());
				}
				break;
			}
			}
			string text = stringBuilder.ToString();
			if (text[0] == '0')
			{
				int num2 = 0;
				while (text[++num2] == '0')
				{
				}
				text = text.Substring(num2);
			}
			if (sign == -1)
			{
				text = "-" + text;
			}
			return text;
		}

		private static BigInteger createUValueOf(ulong value)
		{
			int num = (int)(value >> 32);
			int num2 = (int)value;
			if (num != 0)
			{
				return new BigInteger(1, new int[2] { num, num2 }, false);
			}
			if (num2 != 0)
			{
				BigInteger bigInteger = new BigInteger(1, new int[1] { num2 }, false);
				if ((num2 & -num2) == num2)
				{
					bigInteger.nBits = 1;
				}
				return bigInteger;
			}
			return Zero;
		}

		private static BigInteger createValueOf(long value)
		{
			if (value < 0)
			{
				if (value == long.MinValue)
				{
					return createValueOf(~value).Not();
				}
				return createValueOf(-value).Negate();
			}
			return createUValueOf((ulong)value);
		}

		public static BigInteger ValueOf(long value)
		{
			if (value <= 3)
			{
				if (value < 0)
				{
					goto IL_0049;
				}
				switch (value)
				{
				case 0L:
					return Zero;
				case 1L:
					return One;
				case 2L:
					return Two;
				case 3L:
					return Three;
				}
			}
			if (value != 10)
			{
				goto IL_0049;
			}
			return Ten;
			IL_0049:
			return createValueOf(value);
		}

		public int GetLowestSetBit()
		{
			if (sign == 0)
			{
				return -1;
			}
			int num = magnitude.Length;
			while (--num > 0 && magnitude[num] == 0)
			{
			}
			int num2 = magnitude[num];
			int num3 = ((((uint)num2 & 0xFFFFu) != 0) ? (((num2 & 0xFF) == 0) ? 23 : 31) : (((num2 & 0xFF0000) == 0) ? 7 : 15));
			while (num3 > 0 && num2 << num3 != int.MinValue)
			{
				num3--;
			}
			return (magnitude.Length - num) * 32 - (num3 + 1);
		}

		public bool TestBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit position must not be negative");
			}
			if (sign < 0)
			{
				return !Not().TestBit(n);
			}
			int num = n / 32;
			if (num >= magnitude.Length)
			{
				return false;
			}
			int num2 = magnitude[magnitude.Length - 1 - num];
			return ((num2 >> n % 32) & 1) > 0;
		}

		public BigInteger Or(BigInteger value)
		{
			if (sign == 0)
			{
				return value;
			}
			if (value.sign == 0)
			{
				return this;
			}
			int[] array = ((sign > 0) ? magnitude : Add(One).magnitude);
			int[] array2 = ((value.sign > 0) ? value.magnitude : value.Add(One).magnitude);
			bool flag = sign < 0 || value.sign < 0;
			int num = System.Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = ((i >= num2) ? array[i - num2] : 0);
				int num5 = ((i >= num3) ? array2[i - num3] : 0);
				if (sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = num4 | num5;
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger Xor(BigInteger value)
		{
			if (sign == 0)
			{
				return value;
			}
			if (value.sign == 0)
			{
				return this;
			}
			int[] array = ((sign > 0) ? magnitude : Add(One).magnitude);
			int[] array2 = ((value.sign > 0) ? value.magnitude : value.Add(One).magnitude);
			bool flag = (sign < 0 && value.sign >= 0) || (sign >= 0 && value.sign < 0);
			int num = System.Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = ((i >= num2) ? array[i - num2] : 0);
				int num5 = ((i >= num3) ? array2[i - num3] : 0);
				if (sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = num4 ^ num5;
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger SetBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (TestBit(n))
			{
				return this;
			}
			if (sign > 0 && n < BitLength - 1)
			{
				return FlipExistingBit(n);
			}
			return Or(One.ShiftLeft(n));
		}

		public BigInteger ClearBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (!TestBit(n))
			{
				return this;
			}
			if (sign > 0 && n < BitLength - 1)
			{
				return FlipExistingBit(n);
			}
			return AndNot(One.ShiftLeft(n));
		}

		public BigInteger FlipBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (sign > 0 && n < BitLength - 1)
			{
				return FlipExistingBit(n);
			}
			return Xor(One.ShiftLeft(n));
		}

		private BigInteger FlipExistingBit(int n)
		{
			int[] array = (int[])magnitude.Clone();
			array[array.Length - 1 - (n >> 5)] ^= 1 << n;
			return new BigInteger(sign, array, false);
		}
	}
}
