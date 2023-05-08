using System.IO;

namespace DNA.IO
{
	public class BitUnpacker
	{
		private BinaryReader _reader;

		private int _accumulator;

		private int _bitcount;

		public BitUnpacker(BinaryReader reader)
		{
			_reader = reader;
		}

		private void GetByteFromStream()
		{
			_accumulator |= _reader.ReadByte() << _bitcount;
			_bitcount += 8;
		}

		private int ReadBits(int bits)
		{
			while (_bitcount < bits)
			{
				GetByteFromStream();
			}
			int num = (1 << bits) - 1;
			int result = _accumulator & num;
			_accumulator >>= bits;
			_bitcount -= bits;
			return result;
		}

		private Angle ReadAngle(int bits)
		{
			int num = ReadBits(bits);
			int num2 = 1 << bits;
			return Angle.FromRevolutions((float)num / (float)num2);
		}

		public bool ReadBoolean()
		{
			if (ReadBits(1) != 0)
			{
				return true;
			}
			return false;
		}
	}
}
