using System;
using System.IO;
using DNA.IO.Checksums;

namespace DNA.IO.Compression.Zip.Compression.Streams
{
	public class InflaterInputStream : Stream
	{
		protected Inflater inf;

		protected byte[] buf;

		protected int len;

		private byte[] onebytebuffer = new byte[1];

		protected Stream baseInputStream;

		protected long csize;

		private bool isStreamOwner = true;

		private int readChunkSize;

		protected byte[] cryptbuffer;

		private uint[] keys;

		public bool IsStreamOwner
		{
			get
			{
				return isStreamOwner;
			}
			set
			{
				isStreamOwner = value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return baseInputStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
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
				return len;
			}
		}

		public override long Position
		{
			get
			{
				return baseInputStream.Position;
			}
			set
			{
				throw new NotSupportedException("InflaterInputStream Position not supported");
			}
		}

		public virtual int Available
		{
			get
			{
				if (!inf.IsFinished)
				{
					return 1;
				}
				return 0;
			}
		}

		protected int BufferReadSize
		{
			get
			{
				return readChunkSize;
			}
			set
			{
				readChunkSize = value;
			}
		}

		public override void Flush()
		{
			baseInputStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		public override void SetLength(long val)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		public override void Write(byte[] array, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		public override void WriteByte(byte val)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		public InflaterInputStream(Stream baseInputStream)
			: this(baseInputStream, new Inflater(), 4096)
		{
		}

		public InflaterInputStream(Stream baseInputStream, Inflater inf)
			: this(baseInputStream, inf, 4096)
		{
		}

		public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
		{
			if (baseInputStream == null)
			{
				throw new ArgumentNullException("InflaterInputStream baseInputStream is null");
			}
			if (inflater == null)
			{
				throw new ArgumentNullException("InflaterInputStream Inflater is null");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			this.baseInputStream = baseInputStream;
			inf = inflater;
			buf = new byte[bufferSize];
			if (baseInputStream.CanSeek)
			{
				len = (int)baseInputStream.Length;
			}
			else
			{
				len = 0;
			}
		}

		public override void Close()
		{
			if (isStreamOwner)
			{
				baseInputStream.Close();
			}
		}

		protected void FillInputBuffer()
		{
			if (readChunkSize <= 0)
			{
				len = baseInputStream.Read(buf, 0, buf.Length);
			}
			else
			{
				len = baseInputStream.Read(buf, 0, readChunkSize);
			}
		}

		protected void Fill()
		{
			FillInputBuffer();
			if (keys != null)
			{
				DecryptBlock(buf, 0, len);
			}
			if (len <= 0)
			{
				throw new CompressionException("Deflated stream ends early.");
			}
			inf.SetInput(buf, 0, len);
		}

		public override int ReadByte()
		{
			int num = Read(onebytebuffer, 0, 1);
			if (num > 0)
			{
				return onebytebuffer[0] & 0xFF;
			}
			return -1;
		}

		public override int Read(byte[] b, int off, int len)
		{
			while (true)
			{
				int num;
				try
				{
					num = inf.Inflate(b, off, len);
				}
				catch (Exception ex)
				{
					throw new CompressionException(ex.ToString());
				}
				if (num > 0)
				{
					return num;
				}
				if (inf.IsNeedingDictionary)
				{
					throw new CompressionException("Need a dictionary");
				}
				if (inf.IsFinished)
				{
					return 0;
				}
				if (!inf.IsNeedingInput)
				{
					break;
				}
				Fill();
			}
			throw new InvalidOperationException("Don't know what to do");
		}

		public long Skip(long n)
		{
			if (n <= 0)
			{
				throw new ArgumentOutOfRangeException("n");
			}
			if (baseInputStream.CanSeek)
			{
				baseInputStream.Seek(n, SeekOrigin.Current);
				return n;
			}
			int num = 2048;
			if (n < num)
			{
				num = (int)n;
			}
			byte[] array = new byte[num];
			return baseInputStream.Read(array, 0, array.Length);
		}

		protected byte DecryptByte()
		{
			uint num = (keys[2] & 0xFFFFu) | 2u;
			return (byte)(num * (num ^ 1) >> 8);
		}

		protected void DecryptBlock(byte[] buf, int off, int len)
		{
			for (int i = off; i < off + len; i++)
			{
				buf[i] ^= DecryptByte();
				UpdateKeys(buf[i]);
			}
		}

		protected void InitializePassword(string password)
		{
			keys = new uint[3] { 305419896u, 591751049u, 878082192u };
			for (int i = 0; i < password.Length; i++)
			{
				UpdateKeys((byte)password[i]);
			}
		}

		protected void UpdateKeys(byte ch)
		{
			keys[0] = Crc32.ComputeCrc32(keys[0], ch);
			keys[1] = keys[1] + (byte)keys[0];
			keys[1] = keys[1] * 134775813 + 1;
			keys[2] = Crc32.ComputeCrc32(keys[2], (byte)(keys[1] >> 24));
		}

		protected void StopDecrypting()
		{
			keys = null;
			cryptbuffer = null;
		}
	}
}
