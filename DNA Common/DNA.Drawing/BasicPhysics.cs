using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class BasicPhysics : Physics
	{
		public static Vector3 Gravity = new Vector3(0f, -20f, 0f);

		private Vector3 _worldVelocity = Vector3.Zero;

		public Vector3 LocalAcceleration = Vector3.Zero;

		public Vector3 WorldAcceleration = Vector3.Zero;

		public Vector3 WorldVelocity
		{
			get
			{
				return _worldVelocity;
			}
			set
			{
				_worldVelocity = value;
			}
		}

		public Vector3 LocalVelocity
		{
			get
			{
				return Vector3.TransformNormal(WorldVelocity, base.Owner.WorldToLocal);
			}
			set
			{
				WorldVelocity = Vector3.TransformNormal(value, base.Owner.LocalToWorld);
			}
		}

		public BasicPhysics(Entity owner)
			: base(owner)
		{
		}

		public void Reflect(Vector3 normal, float elasticity)
		{
			WorldVelocity = Vector3.Reflect(WorldVelocity, normal) * elasticity;
		}

		public override void Accelerate(TimeSpan dt)
		{
			float num = (float)dt.TotalSeconds;
			Entity owner = base.Owner;
			Vector3 worldVelocity = WorldVelocity;
			Vector3 vector = Vector3.TransformNormal(LocalAcceleration, owner.LocalToParent) + WorldAcceleration;
			worldVelocity.LengthSquared();
			WorldVelocity += (LocalAcceleration + vector) * num;
		}

		public override void Move(TimeSpan dt)
		{
			Entity owner = base.Owner;
			float num = (float)dt.TotalSeconds;
			float num2 = WorldVelocity.Length();
			if (num2 != 0f)
			{
				owner.LocalPosition += WorldVelocity * num;
			}
		}

		public override void Simulate(TimeSpan dt)
		{
		}
	}
}
