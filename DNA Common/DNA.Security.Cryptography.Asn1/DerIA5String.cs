using System;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerIA5String : DerStringBase
	{
		private readonly string str;

		public static DerIA5String GetInstance(object obj)
		{
			if (obj == null || obj is DerIA5String)
			{
				return (DerIA5String)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerIA5String(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerIA5String GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerIA5String(byte[] str)
			: this(ASCIIEncoder.GetString(str, 0, str.Length), false)
		{
		}

		public DerIA5String(string str)
			: this(str, false)
		{
		}

		public DerIA5String(string str, bool validate)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (validate && !IsIA5String(str))
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
			derOut.WriteEncoded(22, GetOctets());
		}

		protected override int Asn1GetHashCode()
		{
			return str.GetHashCode();
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerIA5String derIA5String = asn1Object as DerIA5String;
			if (derIA5String == null)
			{
				return false;
			}
			return str.Equals(derIA5String.str);
		}

		public static bool IsIA5String(string str)
		{
			foreach (char c in str)
			{
				if (c > '\u007f')
				{
					return false;
				}
			}
			return true;
		}
	}
}
