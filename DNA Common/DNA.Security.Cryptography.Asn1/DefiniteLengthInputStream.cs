using System;
using System.IO;
using DNA.Security.Cryptography.Utilities.IO;

namespace DNA.Security.Cryptography.Asn1
{
	internal class DefiniteLengthInputStream : LimitedInputStream
	{
		private static readonly byte[] EmptyBytes = new byte[0];

		private readonly int _originalLength;

		private int _remaining;

		internal DefiniteLengthInputStream(Stream inStream, int length)
			: base(inStream)
		{
			if (length < 0)
			{
				throw new ArgumentException("negative lengths not allowed", "length");
			}
			_originalLength = length;
			_remaining = length;
			if (length == 0)
			{
				SetParentEofDetect(true);
			}
		}

		public override int ReadByte()
		{
			if (_remaining == 0)
			{
				return -1;
			}
			int num = _in.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);
			}
			if (--_remaining == 0)
			{
				SetParentEofDetect(true);
			}
			return num;
		}

		public override int Read(byte[] buf, int off, int len)
		{
			if (_remaining == 0)
			{
				return 0;
			}
			int count = System.Math.Min(len, _remaining);
			int num = _in.Read(buf, off, count);
			if (num < 1)
			{
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);
			}
			if ((_remaining -= num) == 0)
			{
				SetParentEofDetect(true);
			}
			return num;
		}

		internal byte[] ToArray()
		{
			if (_remaining == 0)
			{
				return EmptyBytes;
			}
			byte[] array = new byte[_remaining];
			if ((_remaining -= Streams.ReadFully(_in, array)) != 0)
			{
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);
			}
			SetParentEofDetect(true);
			return array;
		}
	}
}
