using System;

namespace DNA.Security.Cryptography.Crypto.Digests
{
	public class Sha1Digest : GeneralDigest
	{
		private const int DigestLength = 20;

		private const int Y1 = 1518500249;

		private const int Y2 = 1859775393;

		private const int Y3 = -1894007588;

		private const int Y4 = -899497514;

		private int H1;

		private int H2;

		private int H3;

		private int H4;

		private int H5;

		private int[] X = new int[80];

		private int xOff;

		public override string AlgorithmName
		{
			get
			{
				return "SHA-1";
			}
		}

		public Sha1Digest()
		{
			Reset();
		}

		public Sha1Digest(Sha1Digest t)
			: base(t)
		{
			H1 = t.H1;
			H2 = t.H2;
			H3 = t.H3;
			H4 = t.H4;
			H5 = t.H5;
			Array.Copy(t.X, 0, X, 0, t.X.Length);
			xOff = t.xOff;
		}

		public override int GetDigestSize()
		{
			return 20;
		}

		internal override void ProcessWord(byte[] input, int inOff)
		{
			X[xOff++] = ((input[inOff] & 0xFF) << 24) | ((input[inOff + 1] & 0xFF) << 16) | ((input[inOff + 2] & 0xFF) << 8) | (input[inOff + 3] & 0xFF);
			if (xOff == 16)
			{
				ProcessBlock();
			}
		}

		private static void UnpackWord(int word, byte[] outBytes, int outOff)
		{
			outBytes[outOff++] = (byte)((uint)word >> 24);
			outBytes[outOff++] = (byte)((uint)word >> 16);
			outBytes[outOff++] = (byte)((uint)word >> 8);
			outBytes[outOff++] = (byte)word;
		}

		internal override void ProcessLength(long bitLength)
		{
			if (xOff > 14)
			{
				ProcessBlock();
			}
			X[14] = (int)((ulong)bitLength >> 32);
			X[15] = (int)(bitLength & 0xFFFFFFFFu);
		}

		public override int DoFinal(byte[] output, int outOff)
		{
			Finish();
			UnpackWord(H1, output, outOff);
			UnpackWord(H2, output, outOff + 4);
			UnpackWord(H3, output, outOff + 8);
			UnpackWord(H4, output, outOff + 12);
			UnpackWord(H5, output, outOff + 16);
			Reset();
			return 20;
		}

		public override void Reset()
		{
			base.Reset();
			H1 = 1732584193;
			H2 = -271733879;
			H3 = -1732584194;
			H4 = 271733878;
			H5 = -1009589776;
			xOff = 0;
			for (int i = 0; i != X.Length; i++)
			{
				X[i] = 0;
			}
		}

		private static int F(int u, int v, int w)
		{
			return (u & v) | (~u & w);
		}

		private static int H(int u, int v, int w)
		{
			return u ^ v ^ w;
		}

		private static int G(int u, int v, int w)
		{
			return (u & v) | (u & w) | (v & w);
		}

		internal override void ProcessBlock()
		{
			for (int i = 16; i < 80; i++)
			{
				int num = X[i - 3] ^ X[i - 8] ^ X[i - 14] ^ X[i - 16];
				X[i] = (num << 1) | (int)((uint)num >> 31);
			}
			int num2 = H1;
			int num3 = H2;
			int num4 = H3;
			int num5 = H4;
			int num6 = H5;
			int num7 = 0;
			for (int j = 0; j < 4; j++)
			{
				num6 += (int)((uint)(num2 << 5) | ((uint)num2 >> 27)) + F(num3, num4, num5) + X[num7++] + 1518500249;
				num3 = (num3 << 30) | (int)((uint)num3 >> 2);
				num5 += (int)((uint)(num6 << 5) | ((uint)num6 >> 27)) + F(num2, num3, num4) + X[num7++] + 1518500249;
				num2 = (num2 << 30) | (int)((uint)num2 >> 2);
				num4 += (int)((uint)(num5 << 5) | ((uint)num5 >> 27)) + F(num6, num2, num3) + X[num7++] + 1518500249;
				num6 = (num6 << 30) | (int)((uint)num6 >> 2);
				num3 += (int)((uint)(num4 << 5) | ((uint)num4 >> 27)) + F(num5, num6, num2) + X[num7++] + 1518500249;
				num5 = (num5 << 30) | (int)((uint)num5 >> 2);
				num2 += (int)((uint)(num3 << 5) | ((uint)num3 >> 27)) + F(num4, num5, num6) + X[num7++] + 1518500249;
				num4 = (num4 << 30) | (int)((uint)num4 >> 2);
			}
			for (int k = 0; k < 4; k++)
			{
				num6 += (int)((uint)(num2 << 5) | ((uint)num2 >> 27)) + H(num3, num4, num5) + X[num7++] + 1859775393;
				num3 = (num3 << 30) | (int)((uint)num3 >> 2);
				num5 += (int)((uint)(num6 << 5) | ((uint)num6 >> 27)) + H(num2, num3, num4) + X[num7++] + 1859775393;
				num2 = (num2 << 30) | (int)((uint)num2 >> 2);
				num4 += (int)((uint)(num5 << 5) | ((uint)num5 >> 27)) + H(num6, num2, num3) + X[num7++] + 1859775393;
				num6 = (num6 << 30) | (int)((uint)num6 >> 2);
				num3 += (int)((uint)(num4 << 5) | ((uint)num4 >> 27)) + H(num5, num6, num2) + X[num7++] + 1859775393;
				num5 = (num5 << 30) | (int)((uint)num5 >> 2);
				num2 += (int)((uint)(num3 << 5) | ((uint)num3 >> 27)) + H(num4, num5, num6) + X[num7++] + 1859775393;
				num4 = (num4 << 30) | (int)((uint)num4 >> 2);
			}
			for (int l = 0; l < 4; l++)
			{
				num6 += (int)((uint)(num2 << 5) | ((uint)num2 >> 27)) + G(num3, num4, num5) + X[num7++] + -1894007588;
				num3 = (num3 << 30) | (int)((uint)num3 >> 2);
				num5 += (int)((uint)(num6 << 5) | ((uint)num6 >> 27)) + G(num2, num3, num4) + X[num7++] + -1894007588;
				num2 = (num2 << 30) | (int)((uint)num2 >> 2);
				num4 += (int)((uint)(num5 << 5) | ((uint)num5 >> 27)) + G(num6, num2, num3) + X[num7++] + -1894007588;
				num6 = (num6 << 30) | (int)((uint)num6 >> 2);
				num3 += (int)((uint)(num4 << 5) | ((uint)num4 >> 27)) + G(num5, num6, num2) + X[num7++] + -1894007588;
				num5 = (num5 << 30) | (int)((uint)num5 >> 2);
				num2 += (int)((uint)(num3 << 5) | ((uint)num3 >> 27)) + G(num4, num5, num6) + X[num7++] + -1894007588;
				num4 = (num4 << 30) | (int)((uint)num4 >> 2);
			}
			for (int m = 0; m <= 3; m++)
			{
				num6 += (int)((uint)(num2 << 5) | ((uint)num2 >> 27)) + H(num3, num4, num5) + X[num7++] + -899497514;
				num3 = (num3 << 30) | (int)((uint)num3 >> 2);
				num5 += (int)((uint)(num6 << 5) | ((uint)num6 >> 27)) + H(num2, num3, num4) + X[num7++] + -899497514;
				num2 = (num2 << 30) | (int)((uint)num2 >> 2);
				num4 += (int)((uint)(num5 << 5) | ((uint)num5 >> 27)) + H(num6, num2, num3) + X[num7++] + -899497514;
				num6 = (num6 << 30) | (int)((uint)num6 >> 2);
				num3 += (int)((uint)(num4 << 5) | ((uint)num4 >> 27)) + H(num5, num6, num2) + X[num7++] + -899497514;
				num5 = (num5 << 30) | (int)((uint)num5 >> 2);
				num2 += (int)((uint)(num3 << 5) | ((uint)num3 >> 27)) + H(num4, num5, num6) + X[num7++] + -899497514;
				num4 = (num4 << 30) | (int)((uint)num4 >> 2);
			}
			H1 += num2;
			H2 += num3;
			H3 += num4;
			H4 += num5;
			H5 += num6;
			xOff = 0;
			for (int n = 0; n < 16; n++)
			{
				X[n] = 0;
			}
		}
	}
}
