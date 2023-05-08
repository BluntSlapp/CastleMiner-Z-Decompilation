namespace DNA.IO.Checksums
{
	public class XOR8Checksum : IChecksum<byte>
	{
		private byte _value;

		public byte Value
		{
			get
			{
				return _value;
			}
		}

		public void Reset()
		{
			_value = 0;
		}

		public void Update(byte bval)
		{
			_value ^= bval;
		}

		public void Update(byte[] buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				_value ^= buffer[i];
			}
		}

		public void Update(byte[] buf, int off, int len)
		{
			for (int i = 0; i < len; i++)
			{
				_value ^= buf[off + i];
			}
		}
	}
}
