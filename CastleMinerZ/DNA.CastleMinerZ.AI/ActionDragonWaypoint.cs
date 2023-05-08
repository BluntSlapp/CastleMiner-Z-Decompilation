using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public struct ActionDragonWaypoint
	{
		public BaseDragonWaypoint BaseWpt;

		public DragonWaypointActionEnum Action;

		public Vector3 ActionPosition;

		public int FireballIndex;

		public static ActionDragonWaypoint ReadActionWaypoint(BinaryReader reader)
		{
			ActionDragonWaypoint result = default(ActionDragonWaypoint);
			result.BaseWpt = BaseDragonWaypoint.ReadBaseWaypoint(reader);
			result.Action = (DragonWaypointActionEnum)reader.ReadByte();
			result.ActionPosition = reader.ReadVector3();
			result.FireballIndex = 0;
			return result;
		}

		public void Write(BinaryWriter writer)
		{
			BaseWpt.Write(writer);
			writer.Write((byte)Action);
			writer.Write(ActionPosition);
		}

		public static void InterpolatePositionVelocity(float time, ActionDragonWaypoint wpt1, ActionDragonWaypoint wpt2, out Vector3 outpos, out Vector3 outvel)
		{
			BaseDragonWaypoint.InterpolatePositionVelocity(time, wpt1.BaseWpt, wpt2.BaseWpt, out outpos, out outvel);
		}

		public static void Extrapolate(float time, ActionDragonWaypoint wpt, out Vector3 outpos, out Vector3 outvel)
		{
			BaseDragonWaypoint.Extrapolate(time, wpt.BaseWpt, out outpos, out outvel);
		}

		public static ActionDragonWaypoint CreateFromBase(BaseDragonWaypoint wpt)
		{
			ActionDragonWaypoint result = default(ActionDragonWaypoint);
			result.BaseWpt = wpt;
			result.Action = DragonWaypointActionEnum.GOTO;
			result.ActionPosition = Vector3.Zero;
			result.FireballIndex = 0;
			return result;
		}

		public static ActionDragonWaypoint Create(BaseDragonWaypoint wpt, Vector3 target, DragonWaypointActionEnum action, int index)
		{
			ActionDragonWaypoint result = default(ActionDragonWaypoint);
			result.BaseWpt = wpt;
			result.Action = action;
			result.ActionPosition = target;
			result.FireballIndex = index;
			return result;
		}
	}
}
