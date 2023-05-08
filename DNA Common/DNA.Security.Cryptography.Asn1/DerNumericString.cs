using System;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerNumericString : DerStringBase
	{
		private readonly string str;

		public static DerNumericString GetInstance(object obj)
		{
			if (obj == null || obj is DerNumericString)
			{
				return (DerNumericString)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerNumericString(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerNumericString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerNumericString(byte[] str)
			: this(ASCIIEncoder.GetString(str, 0, str.Length), false)
		{
		}

		public DerNumericString(string str)
			: this(str, false)
		{
		}

		public DerNumericString(string str, bool validate)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (validate && !IsNumericString(str))
			{
				throw new ArgumentException("string contains illegal characters", "str");
			}
			this.str = str;
		}

		public override string GetString()
		{
			return str;
		}

		public byte[] GetOctets()
		{
			return ASCIIEncoder.GetBytes(str);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(18, GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerNumericString derNumericString = asn1Object as DerNumericString;
			if (derNumericString == null)
			{
				return false;
			}
			return str.Equals(derNumericString.str);
		}

		public static bool IsNumericString(string str)
		{
			foreach (char c in str)
			{
				if (c > '\u007f' || (c != ' ' && !char.IsDigit(c)))
				{
					return false;
				}
			}
			return true;
		}
	}
}
