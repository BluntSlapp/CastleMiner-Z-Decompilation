using System;
using System.Text;

namespace DNA
{
	public class SuperStringBuilder
	{
		private char[] _buffer;

		private int _length;

		public int Capacity
		{
			get
			{
				return _buffer.Length;
			}
			set
			{
				ResizeBuffer(value);
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
				if (_length > Capacity)
				{
					Capacity = _length;
				}
			}
		}

		public char this[int index]
		{
			get
			{
				return _buffer[index];
			}
			set
			{
				_buffer[index] = value;
			}
		}

		public SuperStringBuilder()
		{
			_buffer = new char[16];
		}

		public SuperStringBuilder(int capacity)
		{
			_buffer = new char[capacity];
		}

		public SuperStringBuilder(string value)
		{
			_buffer = value.ToCharArray();
			_length = value.Length;
		}

		public SuperStringBuilder(int capacity, int maxCapacity)
		{
			throw new NotImplementedException();
		}

		public SuperStringBuilder(string value, int capacity)
		{
			if (value.Length > capacity)
			{
				throw new Exception();
			}
			_buffer = new char[capacity];
			value.CopyTo(0, _buffer, 0, value.Length);
			_length = value.Length;
		}

		public SuperStringBuilder(string value, int startIndex, int length, int capacity)
		{
			if (value.Length > capacity)
			{
				throw new Exception();
			}
			_buffer = new char[capacity];
			value.CopyTo(startIndex, _buffer, 0, length);
			_length = length;
		}

		private void ResizeBuffer(int capacity)
		{
			if (Capacity != capacity)
			{
				char[] array = new char[capacity];
				Array.Copy(_buffer, array, Math.Min(_length, capacity));
				_buffer = array;
			}
		}

		public StringBuilder Append(bool value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(byte value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(char value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(char[] value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(double value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(float value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(int value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(long value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(object value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(sbyte value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(short value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(string value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(uint value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(ulong value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(ushort value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(char value, int repeatCount)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(char[] value, int startIndex, int charCount)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Append(string value, int startIndex, int count)
		{
			throw new NotImplementedException();
		}

		public StringBuilder AppendFormat(string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
		{
			throw new NotImplementedException();
		}

		public StringBuilder AppendLine()
		{
			throw new NotImplementedException();
		}

		public StringBuilder AppendLine(string value)
		{
			throw new NotImplementedException();
		}

		public int EnsureCapacity(int capacity)
		{
			throw new NotImplementedException();
		}

		public bool Equals(StringBuilder sb)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Insert(int index, char[] value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Insert(int index, string value)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Insert(int index, string value, int count)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Insert(int index, char[] value, int startIndex, int charCount)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Remove(int startIndex, int length)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Replace(char oldChar, char newChar)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Replace(string oldValue, string newValue)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
		{
			throw new NotImplementedException();
		}

		public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return new string(_buffer, 0, _length);
		}

		public string ToString(int startIndex, int length)
		{
			return new string(_buffer, startIndex, length);
		}
	}
}
