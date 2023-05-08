using System;
using System.Collections.Generic;
using System.IO;
using DNA.Security.Cryptography.Utilities;
using DNA.Security.Cryptography.Utilities.Encoders;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public abstract class Asn1OctetString : Asn1Object, Asn1OctetStringParser, IAsn1Convertible
	{
		internal byte[] str;

		public Asn1OctetStringParser Parser
		{
			get
			{
				return this;
			}
		}

		public static Asn1OctetString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public static Asn1OctetString GetInstance(object obj)
		{
			if (obj == null || obj is Asn1OctetString)
			{
				return (Asn1OctetString)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			if (obj is Asn1Sequence)
			{
				List<object> list = new List<object>();
				foreach (object item in (Asn1Sequence)obj)
				{
					list.Add(item);
				}
				return new BerOctetString(list);
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		internal Asn1OctetString(byte[] str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		internal Asn1OctetString(Asn1Encodable obj)
		{
			try
			{
				str = obj.GetDerEncoded();
			}
			catch (IOException ex)
			{
				throw new ArgumentException("Error processing object : " + ex.ToString());
			}
		}

		public Stream GetOctetStream()
		{
			return new MemoryStream(str, false);
		}

		public virtual byte[] GetOctets()
		{
			return str;
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerOctetString derOctetString = asn1Object as DerOctetString;
			if (derOctetString == null)
			{
				return false;
			}
			return Arrays.AreEqual(GetOctets(), derOctetString.GetOctets());
		}

		public override string ToString()
		{
			byte[] array = Hex.Encode(str);
			return "#" + ASCIIEncoder.GetString(array, 0, array.Length);
		}
	}
}
