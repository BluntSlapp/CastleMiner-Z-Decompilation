using System;
using System.Text;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerUniversalString : DerStringBase
	{
		private static readonly char[] table = new char[16]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F'
		};

		private readonly byte[] str;

		public static DerUniversalString GetInstance(object obj)
		{
			if (obj == null || obj is DerUniversalString)
			{
				return (DerUniversalString)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerUniversalString(((Asn1OctetString)obj).GetOctets());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerUniversalString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerUniversalString(byte[] str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
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

		public byte[] GetOctets()
		{
			return (byte[])str.Clone();
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(28, str);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerUniversalString derUniversalString = asn1Object as DerUniversalString;
			if (derUniversalString == null)
			{
				return false;
			}
			return Arrays.AreEqual(str, derUniversalString.str);
		}
	}
}
