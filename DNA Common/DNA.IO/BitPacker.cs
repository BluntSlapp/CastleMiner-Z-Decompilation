using System;
using System.IO;

namespace DNA.IO
{
	public class BitPacker
	{
		private BinaryWriter _writer;

		private int _accumulator;

		private int _bitcount;

		public BitPacker(BinaryWriter writer)
		{
			_writer = writer;
		}

		private void WriteBool(bool value)
		{
			WriteToAccumulator(value ? 1 : 0, 1);
		}

		private void WriteAngle(Angle angle, int bits)
		{
			int num = 1 << bits;
			angle.Normalize();
			int b = (int)Math.Round(angle.Revolutions * (float)num);
			WriteToAccumulator(b, bits);
		}

		private void WriteToAccumulator(int b)
		{
			WriteToAccumulator(b, 8);
		}

		private void WriteToAccumulator(int b, int bits)
		{
			int num = b & ((1 << bits) - 1);
			num <<= _bitcount;
			_accumulator |= num;
			_bitcount += bits;
		}

		private void WriteToStream()
		{
			while (_bitcount >= 8)
			{
				_writer.Write((byte)_accumulator);
				_accumulator >>= 8;
				_bitcount -= 8;
			}
		}

		public void Flush()
		{
			WriteToStream();
			if (_bitcount > 0)
			{
				_writer.Write((byte)_accumulator);
			}
			_accumulator = 0;
			_bitcount = 0;
			_writer.Flush();
		}
	}
}
