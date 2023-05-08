using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNA.IO
{
	[Serializable]
	public class HTFElement
	{
		private const char EscapeChar = '~';

		private string _id = "";

		private string _value = "";

		private List<HTFElement> _children = new List<HTFElement>();

		public string ID
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public virtual List<HTFElement> Children
		{
			get
			{
				return _children;
			}
		}

		public override string ToString()
		{
			if (_id != "")
			{
				return _id + "=" + _value;
			}
			return _value;
		}

		public bool IsID(string id)
		{
			return string.Compare(id, _id, StringComparison.CurrentCultureIgnoreCase) == 0;
		}

		public HTFElement()
		{
		}

		public HTFElement(Stream stream)
		{
			char c;
			do
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException("No Elements Found");
				}
				c = (char)num;
			}
			while (char.IsWhiteSpace(c));
			if (c != '<')
			{
				throw new IOException("This is not a valid File");
			}
			ParseChildren(stream);
		}

		public HTFElement(string value)
		{
			_value = value;
		}

		public HTFElement(string id, string value)
		{
			_id = id;
			_value = value;
		}

		private static char TranslateEscapeChar(char c)
		{
			switch (c)
			{
			case '0':
				return '\0';
			case 'a':
				return '\a';
			case 'b':
				return '\b';
			case 'f':
				return '\f';
			case 'n':
				return '\n';
			case 'r':
				return '\r';
			case 't':
				return '\t';
			case 'v':
				return '\v';
			default:
				return c;
			}
		}

		protected void ParseChildren(Stream stream)
		{
			int num = 0;
			int length = 0;
			bool flag = true;
			while (flag)
			{
				HTFElement hTFElement = new HTFElement();
				flag = hTFElement.Parse(stream, out length);
				_children.Add(hTFElement);
				num += length;
			}
		}

		private bool AddChildNode(Stream stream)
		{
			try
			{
				ParseChildren(stream);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				return false;
			}
			return true;
		}

		private bool Parse(Stream stream, out int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool result = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			while (!flag)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					throw new EndOfStreamException("Unexpected End of File");
				}
				char c = (char)num;
				if (flag3)
				{
					c = TranslateEscapeChar(c);
					stringBuilder.Append(c);
					flag3 = false;
				}
				else
				{
					if (!flag5 && char.IsWhiteSpace(c))
					{
						continue;
					}
					if (flag4)
					{
						switch (c)
						{
						case '"':
							flag4 = false;
							break;
						case '~':
							flag3 = true;
							break;
						default:
							stringBuilder.Append(c);
							break;
						}
					}
					else if (c == ',')
					{
						flag = true;
						result = true;
					}
					else if (c == '>')
					{
						flag = true;
						result = false;
					}
					else if (c == '<')
					{
						ParseChildren(stream);
					}
					else if (c == '~')
					{
						flag3 = true;
					}
					else if (c == '=' && !flag2)
					{
						_id = stringBuilder.ToString().Trim();
						flag5 = false;
						stringBuilder = new StringBuilder();
					}
					else if (c == '"')
					{
						flag4 = true;
					}
					else
					{
						stringBuilder.Append(c);
					}
					flag5 = true;
				}
			}
			_value = stringBuilder.ToString().Trim();
			length = _value.Length;
			return result;
		}

		private static char GetEscapeChar(char c)
		{
			switch (c)
			{
			case '\a':
				return 'a';
			case '\b':
				return 'b';
			case '\f':
				return 'f';
			case '\n':
				return 'n';
			case '\r':
				return 'r';
			case '\0':
				return '0';
			default:
				return c;
			}
		}

		private static bool NeedsEscape(char c)
		{
			if (c != '~' && c != '\a' && c != '\b' && c != '\f' && c != '\n' && c != '\r' && c != 0 && c != '<' && c != '>' && c != ',' && c != '=')
			{
				return c == '"';
			}
			return true;
		}

		private static string BuildEscapedString(string s)
		{
			bool flag = true;
			bool flag2 = false;
			foreach (char c in s)
			{
				if (NeedsEscape(c))
				{
					flag2 = true;
				}
				if (c == '"' || c == '\a' || c == '\b' || c == '\f' || c == '\n' || c == '\r' || c == '\0')
				{
					flag = false;
					break;
				}
			}
			if (flag && flag2)
			{
				return "\"" + s + "\"";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c2 in s)
			{
				if (NeedsEscape(c2))
				{
					stringBuilder.Append('~');
					stringBuilder.Append(GetEscapeChar(c2));
				}
				else
				{
					stringBuilder.Append(c2);
				}
			}
			return stringBuilder.ToString();
		}

		public void Save(StreamWriter writer)
		{
			Save(writer, 0);
		}

		private void Save(StreamWriter writer, int level)
		{
			if (_id != "" && _id != null)
			{
				writer.Write(BuildEscapedString(_id));
				writer.Write('=');
			}
			writer.Write(BuildEscapedString(_value));
			writer.Write('<');
			writer.WriteLine();
			for (int i = 0; i < Children.Count; i++)
			{
				for (int j = 0; j < level; j++)
				{
					writer.Write('\t');
				}
				Children[i].Save(writer, level + 1);
				if (i < Children.Count - 1)
				{
					writer.Write(',');
				}
				writer.WriteLine();
			}
			for (int k = 0; k < level; k++)
			{
				writer.Write('\t');
			}
			writer.Write('>');
		}
	}
}
