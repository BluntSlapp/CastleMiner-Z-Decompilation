using System;
using System.Globalization;
using System.Text;
using DNA.Text;

namespace DNA.Security.Cryptography.Asn1
{
	public class DerGeneralizedTime : Asn1Object
	{
		private readonly string time;

		public string TimeString
		{
			get
			{
				return time;
			}
		}

		private bool HasFractionalSeconds
		{
			get
			{
				return time.IndexOf('.') == 14;
			}
		}

		public static DerGeneralizedTime GetInstance(object obj)
		{
			if (obj == null || obj is DerGeneralizedTime)
			{
				return (DerGeneralizedTime)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerGeneralizedTime(((Asn1OctetString)obj).GetOctets());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name, "obj");
		}

		public static DerGeneralizedTime GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public DerGeneralizedTime(string time)
		{
			this.time = time;
			try
			{
				ToDateTime();
			}
			catch (FormatException ex)
			{
				throw new ArgumentException("invalid date string: " + ex.Message);
			}
		}

		public DerGeneralizedTime(DateTime time)
		{
			this.time = time.ToString("yyyyMMddHHmmss\\Z");
		}

		internal DerGeneralizedTime(byte[] bytes)
		{
			time = ASCIIEncoder.GetString(bytes, 0, bytes.Length);
		}

		public string GetTime()
		{
			if (time[time.Length - 1] == 'Z')
			{
				return time.Substring(0, time.Length - 1) + "GMT+00:00";
			}
			int num = time.Length - 5;
			char c = time[num];
			if (c == '-' || c == '+')
			{
				return time.Substring(0, num) + "GMT" + time.Substring(num, 3) + ":" + time.Substring(num + 3);
			}
			num = time.Length - 3;
			c = time[num];
			if (c == '-' || c == '+')
			{
				return time.Substring(0, num) + "GMT" + time.Substring(num) + ":00";
			}
			return time + CalculateGmtOffset();
		}

		private string CalculateGmtOffset()
		{
			char c = '+';
			int num = 0;
			if (num < 0)
			{
				c = '-';
				num = -num;
			}
			int num2 = num / 60;
			num %= 60;
			return "GMT" + c + Convert(num2) + ":" + Convert(num);
		}

		private static string Convert(int time)
		{
			if (time < 10)
			{
				return "0" + time;
			}
			return time.ToString();
		}

		public DateTime ToDateTime()
		{
			string text = time;
			bool makeUniversal = false;
			string formatStr;
			if (text.EndsWith("Z"))
			{
				if (HasFractionalSeconds)
				{
					int count = text.Length - text.IndexOf('.') - 2;
					formatStr = "yyyyMMddHHmmss." + FString(count) + "\\Z";
				}
				else
				{
					formatStr = "yyyyMMddHHmmss\\Z";
				}
			}
			else if (time.IndexOf('-') > 0 || time.IndexOf('+') > 0)
			{
				text = GetTime();
				makeUniversal = true;
				if (HasFractionalSeconds)
				{
					int count2 = text.IndexOf("GMT") - 1 - text.IndexOf('.');
					formatStr = "yyyyMMddHHmmss." + FString(count2) + "'GMT'zzz";
				}
				else
				{
					formatStr = "yyyyMMddHHmmss'GMT'zzz";
				}
			}
			else if (HasFractionalSeconds)
			{
				int count3 = text.Length - 1 - text.IndexOf('.');
				formatStr = "yyyyMMddHHmmss." + FString(count3);
			}
			else
			{
				formatStr = "yyyyMMddHHmmss";
			}
			return ParseDateString(text, formatStr, makeUniversal);
		}

		private string FString(int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append('f');
			}
			return stringBuilder.ToString();
		}

		private DateTime ParseDateString(string dateStr, string formatStr, bool makeUniversal)
		{
			DateTime result = DateTime.ParseExact(dateStr, formatStr, DateTimeFormatInfo.InvariantInfo);
			if (!makeUniversal)
			{
				return result;
			}
			return result.ToUniversalTime();
		}

		private byte[] GetOctets()
		{
			return ASCIIEncoder.GetBytes(time);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(24, GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerGeneralizedTime derGeneralizedTime = asn1Object as DerGeneralizedTime;
			if (derGeneralizedTime == null)
			{
				return false;
			}
			return time.Equals(derGeneralizedTime.time);
		}

		protected override int Asn1GetHashCode()
		{
			return time.GetHashCode();
		}
	}
}
