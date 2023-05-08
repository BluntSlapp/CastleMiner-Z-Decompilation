using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class ReflectionCamera : PerspectiveCamera
	{
		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime time)
		{
			if (BlockTerrain.Instance.IsWaterWorld)
			{
				CastleMinerZGame.Instance.DrawingReflection = true;
				PerspectiveCamera fPSCamera = CastleMinerZGame.Instance.LocalPlayer.FPSCamera;
				FieldOfView = fPSCamera.FieldOfView;
				NearPlane = fPSCamera.NearPlane;
				FarPlane = fPSCamera.FarPlane;
				Matrix localToWorld = fPSCamera.LocalToWorld;
				Matrix matrix2 = (base.LocalToParent = Matrix.Multiply(localToWorld, BlockTerrain.Instance.GetReflectionMatrix()));
				base.Draw(device, spriteBatch, time);
				CastleMinerZGame.Instance.DrawingReflection = false;
			}
		}
	}
}
