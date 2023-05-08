using System;
using System.Text;

namespace DNA.Text
{
	public static class TextConverter
	{
		private static char Base32IndexToChar(int index)
		{
			char c = ((index >= 10) ? ((char)(65 + (index - 10))) : ((char)(48 + index)));
			switch (c)
			{
			case '0':
				c = 'W';
				break;
			case '1':
				c = 'X';
				break;
			case 'O':
				c = 'Y';
				break;
			case 'I':
				c = 'Z';
				break;
			}
			return c;
		}

		private static int Base32CharToIndex(char rchar)
		{
			switch (rchar)
			{
			case 'W':
				rchar = '0';
				break;
			case 'X':
				rchar = '1';
				break;
			case 'Y':
				rchar = 'O';
				break;
			case 'Z':
				rchar = 'I';
				break;
			}
			if (rchar >= '0' && rchar <= '9')
			{
				return rchar - 48;
			}
			if (rchar >= 'A' && rchar <= 'Z')
			{
				return rchar - 65 + 10;
			}
			throw new FormatException("charactor is out of Base32 Range");
		}

		public static string ToBase32String(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num2 < bytes.Length)
			{
				if (num <= 8 && num2 < bytes.Length)
				{
					byte b = bytes[num2];
					num2++;
					num3 |= b << num;
					num += 8;
				}
				char value = Base32IndexToChar(num3 & 0x1F);
				stringBuilder.Append(value);
				num3 >>= 5;
				num -= 5;
			}
			while (num > 0)
			{
				char value2 = Base32IndexToChar(num3 & 0x1F);
				stringBuilder.Append(value2);
				num3 >>= 5;
				num -= 5;
			}
			return stringBuilder.ToString();
		}

		public static byte[] FromBase32String(string str)
		{
			str = str.ToUpper();
			int num = str.Length * 5 / 8;
			byte[] array = new byte[num];
			int i = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			while (num2 < str.Length)
			{
				for (; i < 8; i += 5)
				{
					int num5 = Base32CharToIndex(str[num2++]);
					num3 |= num5 << i;
				}
				byte b = (byte)((uint)num3 & 0xFFu);
				num3 >>= 8;
				i -= 8;
				array[num4++] = b;
			}
			return array;
		}
	}
}
