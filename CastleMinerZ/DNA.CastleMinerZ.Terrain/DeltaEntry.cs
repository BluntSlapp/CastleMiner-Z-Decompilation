namespace DNA.CastleMinerZ.Terrain
{
	public class DeltaEntry
	{
		public static IntVector3 GetVector(int delta)
		{
			return new IntVector3((delta >> 16) & 0xF, (delta >> 8) & 0x7F, delta & 0xF);
		}

		public static BlockTypeEnum GetBlockType(int delta)
		{
			return (BlockTypeEnum)((delta >> 24) & 0xFF);
		}

		public static int Create(IntVector3 vec, BlockTypeEnum type)
		{
			return ((int)type << 24) | ((vec.X & 0xF) << 16) | ((vec.Y & 0x7F) << 8) | (vec.Z & 0xF);
		}

		public static bool SameLocation(int delta1, int delta2)
		{
			return ((delta1 ^ delta2) & 0xF7F0F) == 0;
		}
	}
}
