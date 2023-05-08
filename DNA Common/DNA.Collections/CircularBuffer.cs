using System;

namespace DNA.Collections
{
	public class CircularBuffer<T>
	{
		private T[] _data;

		private int _writePosition;

		private int _readPosition;

		private int _dataCount;

		private int ReadTailLength
		{
			get
			{
				if (_writePosition > _readPosition)
				{
					return _writePosition - _readPosition;
				}
				return _data.Length - _readPosition;
			}
		}

		private int WriteTailLength
		{
			get
			{
				if (_readPosition > _writePosition)
				{
					return _readPosition - _writePosition;
				}
				return _data.Length - _writePosition;
			}
		}

		public int Capacity
		{
			get
			{
				return _data.Length;
			}
		}

		public int Remaining
		{
			get
			{
				return Capacity - Count;
			}
		}

		public int Count
		{
			get
			{
				return _dataCount;
			}
		}

		public void Reset()
		{
			_writePosition = (_readPosition = (_dataCount = 0));
		}

		public CircularBuffer(int capacity)
		{
			_data = new T[capacity];
		}

		public void Write(T[] data, int pos, int length)
		{
			throw new NotImplementedException();
		}

		public void Read(T[] data, int pos, int length)
		{
			throw new NotImplementedException();
		}

		public void Write(T[] data)
		{
			Write(data, 0, data.Length);
		}

		public void Read(T[] data)
		{
			Read(data, 0, data.Length);
		}
	}
}
