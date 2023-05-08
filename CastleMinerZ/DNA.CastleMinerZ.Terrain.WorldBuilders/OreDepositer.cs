using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class OreDepositer : Biome
	{
		private IntNoise _noiseFunction = new IntNoise(new Random(1));

		public OreDepositer(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new IntNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = (int)(blender * 10f);
			for (int i = 0; i < 128; i++)
			{
				int y = i + minY;
				IntVector3 intVector = new IntVector3(worldX, y, worldZ);
				int num2 = terrain.MakeIndexFromWorldIndexVector(intVector);
				if (terrain._blocks[num2] != Biome.rockblock)
				{
					continue;
				}
				int num3 = _noiseFunction.ComputeNoise(intVector / 4);
				int num4 = _noiseFunction.ComputeNoise(intVector);
				num3 += (num4 - 128) / 8;
				if (num3 > 255 - num)
				{
					terrain._blocks[num2] = Biome.coalBlock;
				}
				else if (num3 < num - 5)
				{
					terrain._blocks[num2] = Biome.copperBlock;
				}
				IntVector3 intVector2 = intVector + new IntVector3(1000, 1000, 1000);
				num3 = _noiseFunction.ComputeNoise(intVector2 / 3);
				num4 = _noiseFunction.ComputeNoise(intVector2);
				num3 += (num4 - 128) / 8;
				if (num3 > 264 - num)
				{
					terrain._blocks[num2] = Biome.ironBlock;
				}
				else if (num3 < -9 + num && i < 50)
				{
					terrain._blocks[num2] = Biome.goldBlock;
				}
				if (i < 50)
				{
					IntVector3 intVector3 = intVector + new IntVector3(777, 777, 777);
					num3 = _noiseFunction.ComputeNoise(intVector3 / 2);
					num4 = _noiseFunction.ComputeNoise(intVector3);
					num3 += (num4 - 128) / 8;
					if (num3 > 265 - num)
					{
						terrain._blocks[num2] = Biome.surfaceLavablock;
					}
					else if (num3 < -11 + num && i < 40)
					{
						terrain._blocks[num2] = Biome.diamondsBlock;
					}
				}
			}
		}
	}
}
