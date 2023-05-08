using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.CastleMinerZ.Utils
{
	internal class FlyCamera : PerspectiveCamera
	{
		public Vector3 velocity = Vector3.Zero;

		public Vector3 angles;

		protected override void OnUpdate(GameTime gameTime)
		{
			float num = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			GamePadState state = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
			angles.Y -= state.ThumbSticks.Right.X * num;
			angles.X += state.ThumbSticks.Right.Y * num;
			if (Math.Abs(angles.X) > (float)Math.PI * 2f / 5f)
			{
				angles.X = (float)Math.PI * 2f / 5f * (float)Math.Sign(angles.X);
			}
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll(angles.Y, angles.X, 0f);
			velocity = state.ThumbSticks.Left.Y * base.LocalToWorld.Forward;
			velocity += state.ThumbSticks.Left.X * base.LocalToWorld.Right;
			base.LocalPosition += velocity * num * 3f;
			base.OnUpdate(gameTime);
		}
	}
}
