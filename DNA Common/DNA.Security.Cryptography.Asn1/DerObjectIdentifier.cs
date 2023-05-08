using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DNA.Security.Cryptography.Math;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerObjectIdentifier : Asn1Object
	{
		private static readonly Regex OidRegex = new Regex("\\A[0-2](\\.[0-9]+)+\\z");

		private readonly string identifier;

		public string Id
		{
			get
			{
				return identifier;
			}
		}

		public static DerObjectIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is DerObjectIdentifier)
			{
				return (DerObjectIdentifier)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerObjectIdentifier(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name, "obj");
		}

		public static DerObjectIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerObjectIdentifier(string identifier)
		{
			if (identifier == null)
			{
				throw new ArgumentNullException("identifier");
			}
			if (!OidRegex.IsMatch(identifier))
			{
				throw new FormatException("string " + identifier + " not an OID");
			}
			this.identifier = identifier;
		}

		internal DerObjectIdentifier(byte[] bytes)
			: this(MakeOidStringFromBytes(bytes))
		{
		}

		private void WriteField(Stream outputStream, long fieldValue)
		{
			if (fieldValue >= 128)
			{
				if (fieldValue >= 16384)
				{
					if (fieldValue >= 2097152)
					{
						if (fieldValue >= 268435456)
						{
							if (fieldValue >= 34359738368L)
							{
								if (fieldValue >= 4398046511104L)
								{
									if (fieldValue >= 562949953421312L)
									{
										if (fieldValue >= 72057594037927936L)
										{
											outputStream.WriteByte((byte)((fieldValue >> 56) | 0x80));
										}
										outputStream.WriteByte((byte)((fieldValue >> 49) | 0x80));
									}
									outputStream.WriteByte((byte)((fieldValue >> 42) | 0x80));
								}
								outputStream.WriteByte((byte)((fieldValue >> 35) | 0x80));
							}
							outputStream.WriteByte((byte)((fieldValue >> 28) | 0x80));
						}
						outputStream.WriteByte((byte)((fieldValue >> 21) | 0x80));
					}
					outputStream.WriteByte((byte)((fieldValue >> 14) | 0x80));
				}
				outputStream.WriteByte((byte)((fieldValue >> 7) | 0x80));
			}
			outputStream.WriteByte((byte)(fieldValue & 0x7F));
		}

		private void WriteField(Stream outputStream, BigInteger fieldValue)
		{
			int num = (fieldValue.BitLength + 6) / 7;
			if (num == 0)
			{
				outputStream.WriteByte(0);
				return;
			}
			BigInteger bigInteger = fieldValue;
			byte[] array = new byte[num];
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				array[num2] = (byte)(((uint)bigInteger.IntValue & 0x7Fu) | 0x80u);
				bigInteger = bigInteger.ShiftRight(7);
			}
			array[num - 1] &= 127;
			outputStream.Write(array, 0, array.Length);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			OidTokenizer oidTokenizer = new OidTokenizer(identifier);
			MemoryStream memoryStream = new MemoryStream();
			DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
			string s = oidTokenizer.NextToken();
			int num = int.Parse(s);
			s = oidTokenizer.NextToken();
			int num2 = int.Parse(s);
			WriteField(memoryStream, num * 40 + num2);
			while (oidTokenizer.HasMoreTokens)
			{
				s = oidTokenizer.NextToken();
				if (s.Length < 18)
				{
					WriteField(memoryStream, long.Parse(s));
				}
				else
				{
					WriteField(memoryStream, new BigInteger(s));
				}
			}
			derOutputStream.Close();
			derOut.WriteEncoded(6, memoryStream.ToArray());
		}

		protected override int Asn1GetHashCode()
		{
			return identifier.GetHashCode();
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerObjectIdentifier derObjectIdentifier = asn1Object as DerObjectIdentifier;
			if (derObjectIdentifier == null)
			{
				return false;
			}
			return identifier.Equals(derObjectIdentifier.identifier);
		}

		public override string ToString()
		{
			return identifier;
		}

		private static string MakeOidStringFromBytes(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			long num = 0L;
			BigInteger bigInteger = null;
			bool flag = true;
			for (int i = 0; i != bytes.Length; i++)
			{
				int num2 = bytes[i];
				if (num < 36028797018963968L)
				{
					num = num * 128 + (num2 & 0x7F);
					if (((uint)num2 & 0x80u) != 0)
					{
						continue;
					}
					if (flag)
					{
						switch ((int)num / 40)
						{
						case 0:
							stringBuilder.Append('0');
							break;
						case 1:
							stringBuilder.Append('1');
							num -= 40;
							break;
						default:
							stringBuilder.Append('2');
							num -= 80;
							break;
						}
						flag = false;
					}
					stringBuilder.Append('.');
					stringBuilder.Append(num);
					num = 0L;
				}
				else
				{
					if (bigInteger == null)
					{
						bigInteger = BigInteger.ValueOf(num);
					}
					bigInteger = bigInteger.ShiftLeft(7);
					bigInteger = bigInteger.Or(BigInteger.ValueOf(num2 & 0x7F));
					if ((num2 & 0x80) == 0)
					{
						stringBuilder.Append('.');
						stringBuilder.Append(bigInteger);
						bigInteger = null;
						num = 0L;
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
