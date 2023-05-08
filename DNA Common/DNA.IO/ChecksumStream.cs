using System;
using System.IO;
using DNA.IO.Checksums;

namespace DNA.IO
{
	public class ChecksumStream<T> : Stream
	{
		private IChecksum<T> _checkSum;

		private Stream _stream;

		public Stream BaseStream
		{
			get
			{
				return _stream;
			}
		}

		public T ChecksumValue
		{
			get
			{
				return _checkSum.Value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return _stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return _stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return _stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return _stream.Position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public void Reset()
		{
			_checkSum.Reset();
		}

		public ChecksumStream(Stream stream, IChecksum<T> checksum)
		{
			_stream = stream;
			_checkSum = checksum;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int result = _stream.Read(buffer, offset, count);
			_checkSum.Update(buffer, offset, count);
			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_stream.Write(buffer, offset, count);
			_checkSum.Update(buffer, offset, count);
		}

		public override int ReadByte()
		{
			int num = _stream.ReadByte();
			_checkSum.Update((byte)num);
			return num;
		}

		public override void WriteByte(byte value)
		{
			_stream.WriteByte(value);
			_checkSum.Update(value);
		}

		public override void Flush()
		{
			_stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			_stream.SetLength(value);
		}
	}
}
