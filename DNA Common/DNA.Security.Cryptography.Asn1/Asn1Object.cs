using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public abstract class Asn1Object : Asn1Encodable
	{
		public static Asn1Object FromByteArray(byte[] data)
		{
			return new Asn1InputStream(data).ReadObject();
		}

		public static Asn1Object FromStream(Stream inStr)
		{
			return new Asn1InputStream(inStr).ReadObject();
		}

		public sealed override Asn1Object ToAsn1Object()
		{
			return this;
		}

		internal abstract void Encode(DerOutputStream derOut);

		protected abstract bool Asn1Equals(Asn1Object asn1Object);

		protected abstract int Asn1GetHashCode();

		internal bool CallAsn1Equals(Asn1Object obj)
		{
			return Asn1Equals(obj);
		}

		internal int CallAsn1GetHashCode()
		{
			return Asn1GetHashCode();
		}
	}
}
