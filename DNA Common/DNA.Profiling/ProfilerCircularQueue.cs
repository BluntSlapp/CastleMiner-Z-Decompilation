namespace DNA.Profiling
{
	public class ProfilerCircularQueue<T>
	{
		private T[] _buffer;

		private int _head;

		public T[] Buffer
		{
			get
			{
				return _buffer;
			}
		}

		public int Head
		{
			get
			{
				return _head;
			}
		}

		public ProfilerCircularQueue(int size)
		{
			_buffer = new T[size];
			_head = 0;
		}

		public void Reset()
		{
			_head = 0;
		}

		public void Add(T value)
		{
			_buffer[_head] = value;
			if (++_head == _buffer.Length)
			{
				_head = 0;
			}
		}
	}
}
