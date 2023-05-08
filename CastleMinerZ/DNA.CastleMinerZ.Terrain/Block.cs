using System.Runtime.InteropServices;

namespace DNA.CastleMinerZ.Terrain
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Block
	{
		public const int NUM_BITS_IN_LIGHT_LEVEL = 4;

		public const int SUNLIGHT_NUM_BITS = 4;

		public const int TORCHLIGHT_NUM_BITS = 4;

		public const int SKY_NUM_BITS = 1;

		public const int OPAQUE_NUM_BITS = 1;

		public const int IN_LIST_NUM_BITS = 1;

		public const int HAS_ALPHA_NUM_BITS = 1;

		public const int UNINITIALIZED_NUM_BITS = 1;

		public const int BLOCKTYPE_NUM_BITS = 19;

		public const int SUNLIGHT_SHIFT = 0;

		public const int TORCHLIGHT_SHIFT = 4;

		public const int SKY_SHIFT = 8;

		public const int OPAQUE_SHIFT = 9;

		public const int IN_LIST_SHIFT = 10;

		public const int HAS_ALPHA_SHIFT = 11;

		public const int BLOCKTYPE_SHIFT = 12;

		public const int UNINITIALIZED_SHIFT = 31;

		public const int SUNLIGHT_MASK = 15;

		public const int TORCHLIGHT_MASK = 240;

		public const int SKY_MASK = 256;

		public const int OPAQUE_MASK = 512;

		public const int IN_LIST_MASK = 1024;

		public const int HAS_ALPHA_MASK = 2048;

		public const int BLOCKTYPE_MASK = 2147479552;

		public const int UNINITIALIZED_MASK = int.MinValue;

		public const int MAXLIGHTLEVEL = 15;

		public static readonly BlockFace[] OpposingFace = new BlockFace[6]
		{
			BlockFace.NEGX,
			BlockFace.POSZ,
			BlockFace.POSX,
			BlockFace.NEGZ,
			BlockFace.NEGY,
			BlockFace.POSY
		};

		public static int ClearLighting(int data)
		{
			return data &= -512;
		}

		public static bool IsLit(int data)
		{
			return (data & 0xFF) != 0;
		}

		public static bool NeedToLightNewNeighbors(int data)
		{
			if (IsLit(data))
			{
				return (data & -2147482624) == 0;
			}
			return false;
		}

		public static bool IsUninitialized(int data)
		{
			return (data & int.MinValue) != 0;
		}

		public static int IsUninitialized(int data, bool value)
		{
			if (value)
			{
				return data | int.MinValue;
			}
			return data & 0x7FFFFFFF;
		}

		public static int InitAsSky(int data, int light)
		{
			data &= -16;
			return data | 0x100 | light;
		}

		public static int GetLighting(int data)
		{
			return data & 0x1FF;
		}

		public static bool IsSky(int data)
		{
			return (data & 0x100) != 0;
		}

		public static int IsSky(int data, bool value)
		{
			if (!value)
			{
				return data & -257;
			}
			return data | 0x100;
		}

		public static bool IsOpaque(int data)
		{
			return (data & 0x200) != 0;
		}

		public static int IsOpaque(int data, bool value)
		{
			if (!value)
			{
				return data & -513;
			}
			return data | 0x200;
		}

		public static bool HasAlpha(int data)
		{
			return (data & 0x800) != 0;
		}

		public static int HasAlpha(int data, bool value)
		{
			if (!value)
			{
				return data & -2049;
			}
			return data | 0x800;
		}

		public static bool IsInList(int data)
		{
			return (data & 0x400) != 0;
		}

		public static int IsInList(int data, bool value)
		{
			if (!value)
			{
				return data & -1025;
			}
			return data | 0x400;
		}

		public static int GetSunLightLevel(int data)
		{
			return data & 0xF;
		}

		public static int SetSunLightLevel(int data, int value)
		{
			return (data & -16) | value;
		}

		public static int GetTorchLightLevel(int data)
		{
			return (data & 0xF0) >> 4;
		}

		public static int SetTorchLightLevel(int data, int value)
		{
			return (data & -241) | (value << 4);
		}

		public static BlockTypeEnum GetTypeIndex(int data)
		{
			uint num = (uint)(data & 0x7FFFF000) >> 12;
			if (num > 41)
			{
				return BlockTypeEnum.Dirt;
			}
			return (BlockTypeEnum)num;
		}

		public static BlockType GetType(int data)
		{
			return BlockType.GetType(GetTypeIndex(data));
		}

		public static int SetType(int data, BlockTypeEnum value)
		{
			uint num = (uint)value;
			if (num == 21 || num > 41)
			{
				num = 1u;
			}
			int num2 = (data & -2147479553) | (int)(num << 12);
			BlockType type = BlockType.GetType((BlockTypeEnum)num);
			num2 = ((!type.Opaque) ? (num2 & -513) : (num2 | 0x200));
			if (type.HasAlpha)
			{
				return num2 | 0x800;
			}
			return num2 & -2049;
		}
	}
}
