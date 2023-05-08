using System;
using DNA.Security.Cryptography.Math;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerEnumerated : Asn1Object
	{
		private readonly byte[] bytes;

		public BigInteger Value
		{
			get
			{
				return new BigInteger(bytes);
			}
		}

		public static DerEnumerated GetInstance(object obj)
		{
			if (obj == null || obj is DerEnumerated)
			{
				return (DerEnumerated)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerEnumerated(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerEnumerated GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerEnumerated(int value)
		{
			bytes = BigInteger.ValueOf(value).ToByteArray();
		}

		public DerEnumerated(BigInteger value)
		{
			bytes = value.ToByteArray();
		}

		public DerEnumerated(byte[] bytes)
		{
			this.bytes = bytes;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(10, bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerEnumerated derEnumerated = asn1Object as DerEnumerated;
			if (derEnumerated == null)
			{
				return false;
			}
			return Arrays.AreEqual(bytes, derEnumerated.bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(bytes);
		}
	}
}
