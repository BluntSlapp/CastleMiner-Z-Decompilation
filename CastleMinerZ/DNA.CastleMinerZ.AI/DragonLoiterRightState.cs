using System;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonLoiterRightState : DragonLoiterLeftState
	{
		public override float GetNewYaw(DragonEntity entity, Vector3 dest)
		{
			float num = dest.Length();
			float num2 = DragonBaseState.GetHeading(dest, 0f) - (float)Math.PI / 2f;
			num2 = ((!(num > entity.EType.LoiterDistance)) ? (num2 + Math.Min(1.5f, (entity.EType.LoiterDistance - num) / 20f)) : (num2 - Math.Min(1.5f, (num - entity.EType.LoiterDistance) / 30f)));
			return MathHelper.WrapAngle(num2);
		}
	}
}
