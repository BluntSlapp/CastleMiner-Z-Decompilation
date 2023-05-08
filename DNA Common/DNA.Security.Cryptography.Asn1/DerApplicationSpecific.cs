using System;
using System.IO;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerApplicationSpecific : Asn1Object
	{
		private readonly bool isConstructed;

		private readonly int tag;

		private readonly byte[] octets;

		public int ApplicationTag
		{
			get
			{
				return tag;
			}
		}

		internal DerApplicationSpecific(bool isConstructed, int tag, byte[] octets)
		{
			this.isConstructed = isConstructed;
			this.tag = tag;
			this.octets = octets;
		}

		public DerApplicationSpecific(int tag, byte[] octets)
			: this(false, tag, octets)
		{
		}

		public DerApplicationSpecific(int tag, Asn1Encodable obj)
			: this(true, tag, obj)
		{
		}

		public DerApplicationSpecific(bool isExplicit, int tag, Asn1Encodable obj)
		{
			byte[] derEncoded = obj.GetDerEncoded();
			isConstructed = isExplicit;
			this.tag = tag;
			if (isExplicit)
			{
				octets = derEncoded;
				return;
			}
			int lengthOfLength = GetLengthOfLength(derEncoded);
			byte[] array = new byte[derEncoded.Length - lengthOfLength];
			Array.Copy(derEncoded, lengthOfLength, array, 0, array.Length);
			octets = array;
		}

		public DerApplicationSpecific(int tagNo, Asn1EncodableVector vec)
		{
			tag = tagNo;
			isConstructed = true;
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i != vec.Count; i++)
			{
				try
				{
					byte[] encoded = vec[i].GetEncoded();
					memoryStream.Write(encoded, 0, encoded.Length);
				}
				catch (IOException innerException)
				{
					throw new InvalidOperationException("malformed object", innerException);
				}
			}
			octets = memoryStream.ToArray();
		}

		private int GetLengthOfLength(byte[] data)
		{
			int i;
			for (i = 2; (data[i - 1] & 0x80u) != 0; i++)
			{
			}
			return i;
		}

		public bool IsConstructed()
		{
			return isConstructed;
		}

		public byte[] GetContents()
		{
			return octets;
		}

		public Asn1Object GetObject()
		{
			return Asn1Object.FromByteArray(GetContents());
		}

		public Asn1Object GetObject(int derTagNo)
		{
			if (derTagNo >= 31)
			{
				throw new IOException("unsupported tag number");
			}
			byte[] encoded = GetEncoded();
			byte[] array = ReplaceTagNumber(derTagNo, encoded);
			if ((encoded[0] & 0x20u) != 0)
			{
				array[0] |= 32;
			}
			return Asn1Object.FromByteArray(array);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			int num = 64;
			if (isConstructed)
			{
				num |= 0x20;
			}
			derOut.WriteEncoded(num, tag, octets);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerApplicationSpecific derApplicationSpecific = asn1Object as DerApplicationSpecific;
			if (derApplicationSpecific == null)
			{
				return false;
			}
			if (isConstructed == derApplicationSpecific.isConstructed && tag == derApplicationSpecific.tag)
			{
				return Arrays.AreEqual(octets, derApplicationSpecific.octets);
			}
			return false;
		}

		protected override int Asn1GetHashCode()
		{
			return isConstructed.GetHashCode() ^ tag.GetHashCode() ^ Arrays.GetHashCode(octets);
		}

		private byte[] ReplaceTagNumber(int newTag, byte[] input)
		{
			int num = input[0] & 0x1F;
			int num2 = 1;
			if (num == 31)
			{
				num = 0;
				int num3 = input[num2++] & 0xFF;
				if ((num3 & 0x7F) == 0)
				{
					throw new InvalidOperationException("corrupted stream - invalid high tag number found");
				}
				while (num3 >= 0 && ((uint)num3 & 0x80u) != 0)
				{
					num |= num3 & 0x7F;
					num <<= 7;
					num3 = input[num2++] & 0xFF;
				}
				num |= num3 & 0x7F;
			}
			byte[] array = new byte[input.Length - num2 + 1];
			Array.Copy(input, num2, array, 1, array.Length - 1);
			array[0] = (byte)newTag;
			return array;
		}
	}
}
