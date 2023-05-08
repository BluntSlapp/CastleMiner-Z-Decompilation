using System;
using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public class Asn1StreamParser
	{
		private readonly Stream _in;

		private readonly int _limit;

		public Asn1StreamParser(Stream inStream)
			: this(inStream, int.MaxValue)
		{
		}

		public Asn1StreamParser(Stream inStream, int limit)
		{
			if (!inStream.CanRead)
			{
				throw new ArgumentException("Expected stream to be readable", "inStream");
			}
			_in = inStream;
			_limit = limit;
		}

		public Asn1StreamParser(byte[] encoding)
			: this(new MemoryStream(encoding, false), encoding.Length)
		{
		}

		public virtual IAsn1Convertible ReadObject()
		{
			int num = _in.ReadByte();
			if (num == -1)
			{
				return null;
			}
			Set00Check(false);
			int num2 = Asn1InputStream.ReadTagNumber(_in, num);
			bool flag = (num & 0x20) != 0;
			int num3 = Asn1InputStream.ReadLength(_in, _limit);
			if (num3 < 0)
			{
				if (!flag)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				IndefiniteLengthInputStream indefiniteLengthInputStream = new IndefiniteLengthInputStream(_in);
				if (((uint)num & 0x40u) != 0)
				{
					Asn1StreamParser parser = new Asn1StreamParser(indefiniteLengthInputStream);
					return new BerApplicationSpecificParser(num2, parser);
				}
				if (((uint)num & 0x80u) != 0)
				{
					return new BerTaggedObjectParser(num, num2, indefiniteLengthInputStream);
				}
				Asn1StreamParser parser2 = new Asn1StreamParser(indefiniteLengthInputStream);
				switch (num2)
				{
				case 4:
					return new BerOctetStringParser(parser2);
				case 16:
					return new BerSequenceParser(parser2);
				case 17:
					return new BerSetParser(parser2);
				default:
					throw new IOException("unknown BER object encountered");
				}
			}
			DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(_in, num3);
			if (((uint)num & 0x40u) != 0)
			{
				return new DerApplicationSpecific(flag, num2, definiteLengthInputStream.ToArray());
			}
			if (((uint)num & 0x80u) != 0)
			{
				return new BerTaggedObjectParser(num, num2, definiteLengthInputStream);
			}
			if (flag)
			{
				switch (num2)
				{
				case 4:
					return new BerOctetStringParser(new Asn1StreamParser(definiteLengthInputStream));
				case 16:
					return new DerSequenceParser(new Asn1StreamParser(definiteLengthInputStream));
				case 17:
					return new DerSetParser(new Asn1StreamParser(definiteLengthInputStream));
				default:
					return new DerUnknownTag(true, num2, definiteLengthInputStream.ToArray());
				}
			}
			int num4 = num2;
			if (num4 == 4)
			{
				return new DerOctetStringParser(definiteLengthInputStream);
			}
			return Asn1InputStream.CreatePrimitiveDerObject(num2, definiteLengthInputStream.ToArray());
		}

		private void Set00Check(bool enabled)
		{
			if (_in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)_in).SetEofOn00(enabled);
			}
		}

		internal Asn1EncodableVector ReadVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			IAsn1Convertible asn1Convertible;
			while ((asn1Convertible = ReadObject()) != null)
			{
				asn1EncodableVector.Add(asn1Convertible.ToAsn1Object());
			}
			return asn1EncodableVector;
		}
	}
}
