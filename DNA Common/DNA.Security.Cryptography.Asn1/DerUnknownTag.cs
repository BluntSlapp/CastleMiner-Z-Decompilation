using System;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerUnknownTag : Asn1Object
	{
		private readonly bool isConstructed;

		private readonly int tag;

		private readonly byte[] data;

		public bool IsConstructed
		{
			get
			{
				return isConstructed;
			}
		}

		public int Tag
		{
			get
			{
				return tag;
			}
		}

		public DerUnknownTag(int tag, byte[] data)
			: this(false, tag, data)
		{
		}

		public DerUnknownTag(bool isConstructed, int tag, byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.isConstructed = isConstructed;
			this.tag = tag;
			this.data = data;
		}

		public byte[] GetData()
		{
			return data;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(isConstructed ? 32 : 0, tag, data);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerUnknownTag derUnknownTag = asn1Object as DerUnknownTag;
			if (derUnknownTag == null)
			{
				return false;
			}
			if (isConstructed == derUnknownTag.isConstructed && tag == derUnknownTag.tag)
			{
				return Arrays.AreEqual(data, derUnknownTag.data);
			}
			return false;
		}

		protected override int Asn1GetHashCode()
		{
			return isConstructed.GetHashCode() ^ tag.GetHashCode() ^ Arrays.GetHashCode(data);
		}
	}
}
