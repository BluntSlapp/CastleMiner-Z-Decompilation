using System;
using System.IO;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Asn1
{
	public class BerTaggedObjectParser : Asn1TaggedObjectParser, IAsn1Convertible
	{
		private int _baseTag;

		private int _tagNumber;

		private Stream _contentStream;

		private bool _indefiniteLength;

		public bool IsConstructed
		{
			get
			{
				return (_baseTag & 0x20) != 0;
			}
		}

		public int TagNo
		{
			get
			{
				return _tagNumber;
			}
		}

		internal BerTaggedObjectParser(int baseTag, int tagNumber, Stream contentStream)
		{
			if (!contentStream.CanRead)
			{
				throw new ArgumentException("Expected stream to be readable", "contentStream");
			}
			_baseTag = baseTag;
			_tagNumber = tagNumber;
			_contentStream = contentStream;
			_indefiniteLength = contentStream is IndefiniteLengthInputStream;
		}

		public IAsn1Convertible GetObjectParser(int tag, bool isExplicit)
		{
			if (isExplicit)
			{
				return new Asn1StreamParser(_contentStream).ReadObject();
			}
			switch (tag)
			{
			case 17:
				if (_indefiniteLength)
				{
					return new BerSetParser(new Asn1StreamParser(_contentStream));
				}
				return new DerSetParser(new Asn1StreamParser(_contentStream));
			case 16:
				if (_indefiniteLength)
				{
					return new BerSequenceParser(new Asn1StreamParser(_contentStream));
				}
				return new DerSequenceParser(new Asn1StreamParser(_contentStream));
			case 4:
				if (_indefiniteLength || IsConstructed)
				{
					return new BerOctetStringParser(new Asn1StreamParser(_contentStream));
				}
				return new DerOctetStringParser((DefiniteLengthInputStream)_contentStream);
			default:
				throw Platform.CreateNotImplementedException("implicit tagging");
			}
		}

		private Asn1EncodableVector rLoadVector(Stream inStream)
		{
			try
			{
				return new Asn1StreamParser(inStream).ReadVector();
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(ex.Message, ex);
			}
		}

		public Asn1Object ToAsn1Object()
		{
			if (_indefiniteLength)
			{
				Asn1EncodableVector asn1EncodableVector = rLoadVector(_contentStream);
				if (asn1EncodableVector.Count != 1)
				{
					return new BerTaggedObject(false, _tagNumber, BerSequence.FromVector(asn1EncodableVector));
				}
				return new BerTaggedObject(true, _tagNumber, asn1EncodableVector[0]);
			}
			if (IsConstructed)
			{
				Asn1EncodableVector asn1EncodableVector2 = rLoadVector(_contentStream);
				if (asn1EncodableVector2.Count != 1)
				{
					return new DerTaggedObject(false, _tagNumber, DerSequence.FromVector(asn1EncodableVector2));
				}
				return new DerTaggedObject(true, _tagNumber, asn1EncodableVector2[0]);
			}
			try
			{
				DefiniteLengthInputStream definiteLengthInputStream = (DefiniteLengthInputStream)_contentStream;
				return new DerTaggedObject(false, _tagNumber, new DerOctetString(definiteLengthInputStream.ToArray()));
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(ex.Message, ex);
			}
		}
	}
}
