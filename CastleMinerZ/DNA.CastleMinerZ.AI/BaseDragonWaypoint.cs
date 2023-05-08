using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public struct BaseDragonWaypoint
	{
		public float HostTime;

		public DragonAnimEnum Animation;

		public float TargetRoll;

		public DragonSoundEnum Sound;

		public Vector3 Position;

		public Vector3 Velocity;

		public void Write(BinaryWriter writer)
		{
			writer.Write(Position);
			writer.Write(Velocity);
			writer.Write(HostTime);
			writer.Write(TargetRoll);
			writer.Write((byte)Animation);
			writer.Write((byte)Sound);
		}

		public static BaseDragonWaypoint ReadBaseWaypoint(BinaryReader reader)
		{
			BaseDragonWaypoint result = default(BaseDragonWaypoint);
			result.Position = reader.ReadVector3();
			result.Velocity = reader.ReadVector3();
			result.HostTime = reader.ReadSingle();
			result.TargetRoll = reader.ReadSingle();
			result.Animation = (DragonAnimEnum)reader.ReadByte();
			result.Sound = (DragonSoundEnum)reader.ReadByte();
			return result;
		}

		public static void InterpolatePositionVelocity(float time, BaseDragonWaypoint wpt1, BaseDragonWaypoint wpt2, out Vector3 outpos, out Vector3 outvel)
		{
			float num = wpt2.HostTime - wpt1.HostTime;
			if (num == 0f || time >= wpt2.HostTime)
			{
				Extrapolate(time, wpt2, out outpos, out outvel);
				return;
			}
			if (time <= wpt1.HostTime)
			{
				Extrapolate(time, wpt1, out outpos, out outvel);
				return;
			}
			float num2 = (time - wpt1.HostTime) / num;
			float num3 = 1f / num;
			Vector3 tangent = wpt1.Velocity * num;
			Vector3 tangent2 = wpt2.Velocity * num;
			outpos = Vector3.Hermite(wpt1.Position, tangent, wpt2.Position, tangent2, num2);
			outvel = MathTools.Hermite1stDerivative(wpt1.Position, tangent, wpt2.Position, tangent2, num2) * num3;
		}

		public static void Extrapolate(float time, BaseDragonWaypoint wpt, out Vector3 outpos, out Vector3 outvel)
		{
			outpos = wpt.Position + wpt.Velocity * (time - wpt.HostTime);
			outvel = wpt.Velocity;
		}
	}
}
