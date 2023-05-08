using System;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerBoolean : Asn1Object
	{
		private readonly byte value;

		public static readonly DerBoolean False = new DerBoolean(false);

		public static readonly DerBoolean True = new DerBoolean(true);

		public bool IsTrue
		{
			get
			{
				return value != 0;
			}
		}

		public static DerBoolean GetInstance(object obj)
		{
			if (obj == null || obj is DerBoolean)
			{
				return (DerBoolean)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerBoolean(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerBoolean GetInstance(bool value)
		{
			if (!value)
			{
				return False;
			}
			return True;
		}

		public static DerBoolean GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerBoolean(byte[] value)
		{
			this.value = value[0];
		}

		private DerBoolean(bool value)
		{
			this.value = (byte)(value ? byte.MaxValue : 0);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(1, new byte[1] { value });
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerBoolean derBoolean = asn1Object as DerBoolean;
			if (derBoolean == null)
			{
				return false;
			}
			return IsTrue == derBoolean.IsTrue;
		}

		protected override int Asn1GetHashCode()
		{
			return IsTrue.GetHashCode();
		}

		public override string ToString()
		{
			if (!IsTrue)
			{
				return "FALSE";
			}
			return "TRUE";
		}
	}
}
