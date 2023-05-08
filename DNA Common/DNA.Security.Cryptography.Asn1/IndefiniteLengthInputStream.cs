using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	internal class IndefiniteLengthInputStream : LimitedInputStream
	{
		private int _b1;

		private int _b2;

		private bool _eofReached;

		private bool _eofOn00 = true;

		internal IndefiniteLengthInputStream(Stream inStream)
			: base(inStream)
		{
			_b1 = inStream.ReadByte();
			_b2 = inStream.ReadByte();
			if (_b2 < 0)
			{
				throw new EndOfStreamException();
			}
			CheckForEof();
		}

		internal void SetEofOn00(bool eofOn00)
		{
			_eofOn00 = eofOn00;
			CheckForEof();
		}

		private bool CheckForEof()
		{
			if (!_eofReached && _eofOn00 && _b1 == 0 && _b2 == 0)
			{
				_eofReached = true;
				SetParentEofDetect(true);
			}
			return _eofReached;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_eofOn00 || count < 3)
			{
				return base.Read(buffer, offset, count);
			}
			if (_eofReached)
			{
				return 0;
			}
			int num = _in.Read(buffer, offset + 2, count - 2);
			if (num <= 0)
			{
				throw new EndOfStreamException();
			}
			buffer[offset] = (byte)_b1;
			buffer[offset + 1] = (byte)_b2;
			_b1 = _in.ReadByte();
			_b2 = _in.ReadByte();
			if (_b2 < 0)
			{
				throw new EndOfStreamException();
			}
			return num + 2;
		}

		public override int ReadByte()
		{
			if (CheckForEof())
			{
				return -1;
			}
			int num = _in.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException();
			}
			int b = _b1;
			_b1 = _b2;
			_b2 = num;
			return b;
		}
	}
}
