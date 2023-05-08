using System;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerInteger : Asn1Object
	{
		private readonly byte[] bytes;

		public BigInteger Value
		{
			get
			{
				return new BigInteger(bytes);
			}
		}

		public BigInteger PositiveValue
		{
			get
			{
				return new BigInteger(1, bytes);
			}
		}

		public static DerInteger GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			DerInteger derInteger = obj as DerInteger;
			if (derInteger != null)
			{
				return derInteger;
			}
			Asn1OctetString asn1OctetString = obj as Asn1OctetString;
			if (asn1OctetString != null)
			{
				return new DerInteger(asn1OctetString.GetOctets());
			}
			Asn1TaggedObject asn1TaggedObject = obj as Asn1TaggedObject;
			if (asn1TaggedObject != null)
			{
				return GetInstance(asn1TaggedObject.GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerInteger GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return GetInstance(obj.GetObject());
		}

		public DerInteger(int value)
		{
			bytes = BigInteger.ValueOf(value).ToByteArray();
		}

		public DerInteger(BigInteger value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			bytes = value.ToByteArray();
		}

		public DerInteger(byte[] bytes)
		{
			this.bytes = bytes;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(2, bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerInteger derInteger = asn1Object as DerInteger;
			if (derInteger == null)
			{
				return false;
			}
			return Arrays.AreEqual(bytes, derInteger.bytes);
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
