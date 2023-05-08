using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.Utils.Trace;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DamageLOSProbe : TraceProbe
	{
		private float TotalDistance;

		public float TotalDamageMultiplier;

		public int DragonTypeIndex;

		public override void Reset()
		{
			TotalDistance = Vector3.Distance(_start, _end);
			TotalDamageMultiplier = 1f;
			TraceCompletePath = true;
			base.Reset();
		}

		public override bool TestThisType(BlockTypeEnum e)
		{
			if (e != BlockTypeEnum.NumberOfBlocks)
			{
				return BlockType.GetType(e).BlockPlayer;
			}
			return false;
		}

		public override bool TouchesBlock(float inT, ref Vector3 inNormal, bool startsIn, BlockFace inFace, float outT, ref Vector3 outNormal, bool endsIn, BlockFace outFace, IntVector3 worldindex)
		{
			float num = (outT - inT) * TotalDistance;
			if (num <= 0f)
			{
				return true;
			}
			if (DragonType.BreakLookup[DragonTypeIndex, (int)_currentTestingBlockType])
			{
				TotalDamageMultiplier = 0f;
				return false;
			}
			float num2 = 1f - BlockType.GetType(_currentTestingBlockType).DamageTransmision;
			if (num2 <= 0f)
			{
				return true;
			}
			num2 *= num;
			TotalDamageMultiplier *= (1f - num2).Clamp(0f, 1f);
			return TotalDamageMultiplier > 0f;
		}
	}
}
