using System;

namespace DNA.CastleMinerZ.Terrain
{
	public class BlockType
	{
		private static readonly BlockType[] _blockTypes = new BlockType[42]
		{
			new BlockType(BlockTypeEnum.Empty, "Air", 1f, 0f, 1f, false, false, false, true, false, false, false, false, false, false, false, -1),
			new BlockType(BlockTypeEnum.Dirt, "Dirt", 0f, 0f, 0.8f, false, false, false, false, false, true, true, true, true, false, false, Octal._00),
			new BlockType(BlockTypeEnum.Grass, "Grass", 0f, 0f, 0.8f, false, false, false, false, false, true, true, true, true, false, false, Octal._02, Octal._01, Octal._01, Octal._00, Octal._01, Octal._01),
			new BlockType(BlockTypeEnum.Sand, "Sand", 0f, 0f, 0.7f, false, false, false, false, false, true, true, true, true, false, false, Octal._03),
			new BlockType(BlockTypeEnum.Lantern, "Lantern", 0f, 1f, 1f, false, false, false, false, true, true, true, true, true, false, true, Octal._04),
			new BlockType(BlockTypeEnum.FixedLantern, "Lantern", 0f, 1f, 1f, false, false, false, false, true, true, true, true, false, false, false, Octal._04),
			new BlockType(BlockTypeEnum.Rock, "Rock", 0f, 0f, 0.5f, false, false, false, false, false, true, true, true, true, false, false, Octal._05),
			new BlockType(BlockTypeEnum.GoldOre, "Gold Ore", 0f, 0f, 0.5f, false, false, false, false, true, true, true, true, true, false, false, Octal._06),
			new BlockType(BlockTypeEnum.IronOre, "Iron Ore", 0f, 0f, 0.5f, false, false, false, false, true, true, true, true, true, false, false, Octal._07),
			new BlockType(BlockTypeEnum.CopperOre, "Copper Ore", 0f, 0f, 0.5f, false, false, false, false, true, true, true, true, true, false, false, Octal._10),
			new BlockType(BlockTypeEnum.CoalOre, "Coal", 0f, 0f, 0.5f, false, false, false, false, true, true, true, true, true, false, false, Octal._11),
			new BlockType(BlockTypeEnum.DiamondOre, "Diamonds", 0f, 0f, 0.4f, false, false, false, false, true, true, true, true, true, false, false, Octal._12),
			new BlockType(BlockTypeEnum.SurfaceLava, "Lava", 0f, 1f, 1f, false, false, false, false, false, true, true, true, true, true, false, Octal._13),
			new BlockType(BlockTypeEnum.DeepLava, "Lava", 0f, 1f, 1f, false, false, false, false, false, false, true, true, true, true, false, Octal._13),
			new BlockType(BlockTypeEnum.Bedrock, "Bedrock", 0f, 0f, 0.3f, false, false, false, false, false, true, true, true, false, false, false, Octal._14),
			new BlockType(BlockTypeEnum.Snow, "Snow", 0f, 0f, 1f, false, false, false, false, false, true, true, true, true, false, false, Octal._15),
			new BlockType(BlockTypeEnum.Ice, "Ice", 0.9f, 0f, 0.9f, false, true, false, false, false, true, true, true, true, false, false, Octal._16),
			new BlockType(BlockTypeEnum.Log, "Log", 0f, 0f, 0.8f, false, false, false, false, false, true, true, true, true, false, false, Octal._20, Octal._17, Octal._17, Octal._20, Octal._17, Octal._17),
			new BlockType(BlockTypeEnum.Leaves, "Leaves", 0.4f, 0f, 1f, false, true, true, true, false, false, true, true, true, false, false, Octal._21),
			new BlockType(BlockTypeEnum.Wood, "Wood", 0f, 0f, 0.8f, false, false, false, false, false, true, true, true, true, false, false, Octal._22),
			new BlockType(BlockTypeEnum.BloodStone, "BloodStone", 0f, 0f, 0.2f, false, false, false, false, false, true, true, true, true, false, false, Octal._23),
			new BlockType(BlockTypeEnum.SpaceRock, "Space Rock", 0f, 0f, 0.1f, false, false, false, false, false, true, true, true, true, false, false, Octal._24),
			new BlockType(BlockTypeEnum.IronWall, "Iron Wall", 0f, 0f, 0.3f, false, false, false, false, true, true, true, true, true, false, false, Octal._25),
			new BlockType(BlockTypeEnum.CopperWall, "Copper Wall", 0f, 0f, 0.3f, false, false, false, false, true, true, true, true, true, false, false, Octal._26),
			new BlockType(BlockTypeEnum.GoldenWall, "Golden Wall", 0f, 0f, 0.3f, false, false, false, false, true, true, true, true, true, false, false, Octal._27),
			new BlockType(BlockTypeEnum.DiamondWall, "Diamond Wall", 0f, 0f, 0.2f, false, false, false, false, true, true, true, true, true, false, false, Octal._30),
			new BlockType(BlockTypeEnum.Torch, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, -1),
			new BlockType(BlockTypeEnum.TorchPOSX, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.POSX, BlockTypeEnum.Torch),
			new BlockType(BlockTypeEnum.TorchNEGZ, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.NEGZ, BlockTypeEnum.Torch),
			new BlockType(BlockTypeEnum.TorchNEGX, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.NEGX, BlockTypeEnum.Torch),
			new BlockType(BlockTypeEnum.TorchPOSZ, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.POSZ, BlockTypeEnum.Torch),
			new BlockType(BlockTypeEnum.TorchPOSY, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.POSY, BlockTypeEnum.Torch),
			new BlockType(BlockTypeEnum.TorchNEGY, "Torch", 1f, 1f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.NEGY, BlockTypeEnum.Torch),
			new BlockType(BlockTypeEnum.Crate, "Crate", 0f, 0f, 1f, false, false, false, false, false, true, true, true, true, false, false, Octal._31),
			new BlockType(BlockTypeEnum.LowerDoorClosedZ, "Door", 0f, 0f, 0.8f, true, false, false, true, false, true, true, false, true, false, true, BlockFace.POSY, BlockTypeEnum.LowerDoor),
			new BlockType(BlockTypeEnum.LowerDoorClosedX, "Door", 0f, 0f, 0.8f, true, false, false, true, false, true, true, false, true, false, true, BlockFace.POSY, BlockTypeEnum.LowerDoor),
			new BlockType(BlockTypeEnum.LowerDoor, "Door", 0f, 0f, 0.8f, true, false, false, true, false, true, true, false, true, false, true, -1),
			new BlockType(BlockTypeEnum.UpperDoorClosed, "Door", 0.5f, 0f, 0.8f, true, false, false, true, false, true, true, false, true, false, false, BlockFace.NUM_FACES, BlockTypeEnum.LowerDoor),
			new BlockType(BlockTypeEnum.LowerDoorOpenZ, "Door", 1f, 0f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.POSY, BlockTypeEnum.LowerDoor),
			new BlockType(BlockTypeEnum.LowerDoorOpenX, "Door", 1f, 0f, 1f, true, false, false, true, false, false, true, false, true, false, true, BlockFace.POSY, BlockTypeEnum.LowerDoor),
			new BlockType(BlockTypeEnum.UpperDoorOpen, "Door", 1f, 0f, 1f, true, false, false, true, false, false, true, false, true, false, false, BlockFace.NUM_FACES, BlockTypeEnum.LowerDoor),
			new BlockType(BlockTypeEnum.NumberOfBlocks, "Air", 1f, 0f, 1f, false, false, false, true, false, true, false, false, false, false, false, -1)
		};

		public BlockTypeEnum _type;

		public string Name;

		public int[] TileIndices;

		public int LightTransmission;

		public int SelfIllumination;

		public int DamageMask;

		public BlockFace Facing;

		public BlockTypeEnum ParentBlockType;

		public bool IsItemEntity;

		public bool BlockPlayer;

		public bool NeedsFancyLighting;

		public bool HasAlpha;

		public bool CanBeDug;

		public bool CanBeTouched;

		public bool CanBuildOn;

		public bool DrawFullBright;

		public bool LightAsTranslucent;

		public bool InteriorFaces;

		public bool SpawnEntity;

		public float DamageTransmision;

		public bool Opaque
		{
			get
			{
				return LightTransmission == 0;
			}
		}

		public bool Clear
		{
			get
			{
				return LightTransmission == 16;
			}
		}

		public int this[BlockFace i]
		{
			get
			{
				return TileIndices[(int)i % 6];
			}
		}

		public int this[int i]
		{
			get
			{
				return TileIndices[i % 6];
			}
		}

		public static bool IsEmpty(BlockTypeEnum btype)
		{
			if (btype != BlockTypeEnum.NumberOfBlocks)
			{
				return btype == BlockTypeEnum.Empty;
			}
			return true;
		}

		public override string ToString()
		{
			return Name;
		}

		public static BlockType GetType(BlockTypeEnum t)
		{
			return _blockTypes[(int)t];
		}

		public int TransmitLight(int inlight)
		{
			return Math.Max(0, (inlight - 1) * LightTransmission >> 4);
		}

		private BlockType(BlockTypeEnum type, string name, float lightTransmission, float selfIllumination, float damageTransmision, bool isItemEntity, bool xlucent, bool interior, bool alphaInTexture, bool hasSpecular, bool blockPlayer, bool canBeTouched, bool canBuildOn, bool canBeDug, bool fullBright, bool spawnEntity, int texIndexPosY, int texIndexPosX, int texIndexPosZ, int texIndexNegY, int texIndexNegX, int texIndexNegZ)
		{
			Name = name;
			_type = type;
			LightTransmission = (int)Math.Floor(lightTransmission * 16f + 0.5f);
			SelfIllumination = (int)Math.Floor(selfIllumination * 15f + 0.5f);
			IsItemEntity = isItemEntity;
			HasAlpha = alphaInTexture;
			NeedsFancyLighting = hasSpecular;
			BlockPlayer = blockPlayer;
			CanBeDug = canBeDug;
			CanBeTouched = canBeTouched;
			DrawFullBright = fullBright;
			LightAsTranslucent = xlucent;
			InteriorFaces = interior;
			SpawnEntity = spawnEntity;
			CanBuildOn = canBuildOn;
			DamageTransmision = damageTransmision;
			Facing = BlockFace.NUM_FACES;
			ParentBlockType = type;
			TileIndices = new int[6] { texIndexPosX, texIndexNegZ, texIndexNegX, texIndexPosZ, texIndexPosY, texIndexNegY };
		}

		private BlockType(BlockTypeEnum type, string name, float lightTransmission, float selfIllumination, float damageTransmision, bool isItemEntity, bool xlucent, bool interior, bool alphaInTexture, bool hasSpecular, bool blockPlayer, bool canBeTouched, bool canBuildOn, bool canBeDug, bool fullBright, bool spawnEntity, int texIndexPosY)
		{
			Name = name;
			_type = type;
			LightTransmission = (int)Math.Floor(lightTransmission * 16f + 0.5f);
			SelfIllumination = (int)Math.Floor(selfIllumination * 15f + 0.5f);
			IsItemEntity = isItemEntity;
			HasAlpha = alphaInTexture;
			NeedsFancyLighting = hasSpecular;
			BlockPlayer = blockPlayer;
			CanBeDug = canBeDug;
			CanBeTouched = canBeTouched;
			DrawFullBright = fullBright;
			LightAsTranslucent = xlucent;
			InteriorFaces = interior;
			SpawnEntity = spawnEntity;
			CanBuildOn = canBuildOn;
			DamageTransmision = damageTransmision;
			Facing = BlockFace.NUM_FACES;
			ParentBlockType = type;
			TileIndices = new int[6] { texIndexPosY, texIndexPosY, texIndexPosY, texIndexPosY, texIndexPosY, texIndexPosY };
		}

		private BlockType(BlockTypeEnum type, string name, float lightTransmission, float selfIllumination, float damageTransmision, bool isItemEntity, bool xlucent, bool interior, bool alphaInTexture, bool hasSpecular, bool blockPlayer, bool canBeTouched, bool canBuildOn, bool canBeDug, bool fullBright, bool spawnEntity, BlockFace facing, BlockTypeEnum parentBlock)
		{
			Name = name;
			_type = type;
			LightTransmission = (int)Math.Floor(lightTransmission * 16f + 0.5f);
			SelfIllumination = (int)Math.Floor(selfIllumination * 15f + 0.5f);
			IsItemEntity = isItemEntity;
			HasAlpha = alphaInTexture;
			NeedsFancyLighting = hasSpecular;
			BlockPlayer = blockPlayer;
			CanBeDug = canBeDug;
			CanBeTouched = canBeTouched;
			DrawFullBright = fullBright;
			LightAsTranslucent = xlucent;
			InteriorFaces = interior;
			SpawnEntity = spawnEntity;
			CanBuildOn = canBuildOn;
			DamageTransmision = damageTransmision;
			Facing = facing;
			ParentBlockType = parentBlock;
			TileIndices = new int[6] { -1, -1, -1, -1, -1, -1 };
		}
	}
}
