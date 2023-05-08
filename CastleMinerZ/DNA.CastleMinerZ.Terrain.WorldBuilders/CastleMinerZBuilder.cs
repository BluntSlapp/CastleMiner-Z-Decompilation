using System;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	internal class CastleMinerZBuilder : WorldBuilder
	{
		private const int BandWidth = 600;

		private const int TransitionWidth = 300;

		private CaveBiome caves;

		private OceanBiome ocean;

		private ClassicBiome classic;

		private DesertBiome dessert;

		private LagoonBiome lagoon;

		private MountainBiome mountains;

		private DecentBiome decent;

		private ArcticBiome arctic;

		private OreDepositer oreDepositor;

		private BedrockDepositer bedrockDepositor;

		private HellFloorBiome hell;

		private HellCeilingBiome hellCeiling;

		private TreeDepositer trees;

		private OriginBiome orginArea;

		private TreeTestBiome testBiome;

		public CastleMinerZBuilder(WorldInfo worldInfo)
			: base(worldInfo)
		{
			ocean = new OceanBiome(worldInfo);
			classic = new ClassicBiome(worldInfo);
			dessert = new DesertBiome(worldInfo);
			lagoon = new LagoonBiome(worldInfo);
			mountains = new MountainBiome(worldInfo);
			arctic = new ArcticBiome(worldInfo);
			oreDepositor = new OreDepositer(worldInfo);
			bedrockDepositor = new BedrockDepositer(worldInfo);
			decent = new DecentBiome(worldInfo);
			caves = new CaveBiome(worldInfo);
			hell = new HellFloorBiome(worldInfo);
			trees = new TreeDepositer(worldInfo);
			testBiome = new TreeTestBiome(worldInfo);
			orginArea = new OriginBiome(worldInfo);
			hellCeiling = new HellCeilingBiome(worldInfo);
		}

		public override void BuildWorldChunk(BlockTerrain terrain, IntVector3 minLoc)
		{
			terrain.WaterLevel = 1.5f;
			for (int i = 0; i < 16; i++)
			{
				int num = i + minLoc.Z;
				long num2 = (long)num * (long)num;
				for (int j = 0; j < 16; j++)
				{
					int num3 = j + minLoc.X;
					float num4 = (float)Math.Sqrt((long)num3 * (long)num3 + num2);
					float blender = 1f;
					int num5 = 0;
					while (num4 > 4400f)
					{
						num4 -= 4400f;
						num5++;
					}
					if (((uint)num5 & (true ? 1u : 0u)) != 0)
					{
						num4 = 4400f - num4;
					}
					if (num4 < 200f)
					{
						classic.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					}
					else if (num4 < 300f)
					{
						float num6 = (num4 - 200f) / 100f;
						classic.BuildColumn(terrain, num3, num, minLoc.Y, 1f - num6);
						lagoon.BuildColumn(terrain, num3, num, minLoc.Y, num6);
					}
					else if (num4 < 900f)
					{
						lagoon.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					}
					else if (num4 < 1000f)
					{
						float num7 = (num4 - 900f) / 100f;
						lagoon.BuildColumn(terrain, num3, num, minLoc.Y, 1f - num7);
						dessert.BuildColumn(terrain, num3, num, minLoc.Y, num7);
					}
					else if (num4 < 1600f)
					{
						dessert.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					}
					else if (num4 < 1700f)
					{
						float num8 = (num4 - 1600f) / 100f;
						dessert.BuildColumn(terrain, num3, num, minLoc.Y, 1f - num8);
						mountains.BuildColumn(terrain, num3, num, minLoc.Y, num8);
					}
					else if (num4 < 2300f)
					{
						mountains.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					}
					else if (num4 < 2400f)
					{
						float num9 = (num4 - 2300f) / 100f;
						mountains.BuildColumn(terrain, num3, num, minLoc.Y, 1f - num9);
						arctic.BuildColumn(terrain, num3, num, minLoc.Y, num9);
					}
					else if (num4 < 3000f)
					{
						arctic.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					}
					else if (num4 < 3600f)
					{
						float num10 = (num4 - 3000f) / 600f;
						blender = 1f - num10;
						decent.BuildColumn(terrain, num3, num, minLoc.Y, num10);
					}
					else
					{
						float num11 = 4400f;
					}
					float blender2 = MathHelper.Clamp(num4 / 3600f, 0f, 1f);
					hellCeiling.BuildColumn(terrain, num3, num, minLoc.Y, blender);
					caves.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					oreDepositor.BuildColumn(terrain, num3, num, minLoc.Y, blender2);
					hell.BuildColumn(terrain, num3, num, minLoc.Y, blender);
					bedrockDepositor.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
					orginArea.BuildColumn(terrain, num3, num, minLoc.Y, 1f);
				}
			}
			for (int k = 3; k < 13; k++)
			{
				int worldZ = k + minLoc.Z;
				for (int l = 3; l < 13; l++)
				{
					int worldX = l + minLoc.X;
					trees.BuildColumn(terrain, worldX, worldZ, minLoc.Y, 1f);
				}
			}
		}
	}
}
