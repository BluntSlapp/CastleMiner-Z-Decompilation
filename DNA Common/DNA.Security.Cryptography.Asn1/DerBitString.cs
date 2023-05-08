using System;
using System.Text;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerBitString : DerStringBase
	{
		private static readonly char[] table = new char[16]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F'
		};

		private readonly byte[] data;

		private readonly int padBits;

		public int PadBits
		{
			get
			{
				return padBits;
			}
		}

		public int IntValue
		{
			get
			{
				int num = 0;
				for (int i = 0; i != data.Length && i != 4; i++)
				{
					num |= (data[i] & 0xFF) << 8 * i;
				}
				return num;
			}
		}

		internal static int GetPadBits(int bitString)
		{
			int num = 0;
			for (int num2 = 3; num2 >= 0; num2--)
			{
				if (num2 != 0)
				{
					if (bitString >> num2 * 8 != 0)
					{
						num = (bitString >> num2 * 8) & 0xFF;
						break;
					}
				}
				else if (bitString != 0)
				{
					num = bitString & 0xFF;
					break;
				}
			}
			if (num == 0)
			{
				return 7;
			}
			int num3 = 1;
			while (((uint)(num <<= 1) & 0xFFu) != 0)
			{
				num3++;
			}
			return 8 - num3;
		}

		internal static byte[] GetBytes(int bitString)
		{
			int num = 4;
			int num2 = 3;
			while (num2 >= 1 && (bitString & (255 << num2 * 8)) == 0)
			{
				num--;
				num2--;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (byte)((uint)(bitString >> i * 8) & 0xFFu);
			}
			return array;
		}

		public static DerBitString GetInstance(object obj)
		{
			if (obj == null || obj is DerBitString)
			{
				return (DerBitString)obj;
			}
			if (obj is Asn1OctetString)
			{
				byte[] octets = ((Asn1OctetString)obj).GetOctets();
				int num = octets[0];
				byte[] destinationArray = new byte[octets.Length - 1];
				Array.Copy(octets, 1, destinationArray, 0, octets.Length - 1);
				return new DerBitString(destinationArray, num);
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerBitString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		internal DerBitString(byte data, int padBits)
		{
			this.data = new byte[1] { data };
			this.padBits = padBits;
		}

		public DerBitString(byte[] data, int padBits)
		{
			this.data = data;
			this.padBits = padBits;
		}

		public DerBitString(byte[] data)
		{
			this.data = data;
		}

		public DerBitString(Asn1Encodable obj)
		{
			data = obj.GetDerEncoded();
		}

		public byte[] GetBytes()
		{
			return data;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			byte[] array = new byte[GetBytes().Length + 1];
			array[0] = (byte)PadBits;
			Array.Copy(GetBytes(), 0, array, 1, array.Length - 1);
			derOut.WriteEncoded(3, array);
		}

		protected override int Asn1GetHashCode()
		{
			return padBits.GetHashCode() ^ Arrays.GetHashCode(data);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerBitString derBitString = asn1Object as DerBitString;
			if (derBitString == null)
			{
				return false;
			}
			if (padBits == derBitString.padBits)
			{
				return Arrays.AreEqual(data, derBitString.data);
			}
			return false;
		}

		public override string GetString()
		{
			StringBuilder stringBuilder = new StringBuilder("#");
			byte[] derEncoded = GetDerEncoded();
			for (int i = 0; i != derEncoded.Length; i++)
			{
				uint num = derEncoded[i];
				stringBuilder.Append(table[(num >> 4) & 0xF]);
				stringBuilder.Append(table[derEncoded[i] & 0xF]);
			}
			return stringBuilder.ToString();
		}
	}
}
