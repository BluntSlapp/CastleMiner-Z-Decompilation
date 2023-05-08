using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class BedrockDepositer : Biome
	{
		private const int BedRockVariance = 3;

		private IntNoise _noiseFunction = new IntNoise(new Random(1));

		public BedrockDepositer(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new IntNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = _noiseFunction.ComputeNoise(worldX, worldZ);
			num = 1 + num * 3 / 256;
			int num2 = num;
			for (int i = 0; i < num2; i++)
			{
				int y = i + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num3 = terrain.MakeIndexFromWorldIndexVector(a);
				terrain._blocks[num3] = Biome.bedrockBlock;
			}
		}
	}
}
