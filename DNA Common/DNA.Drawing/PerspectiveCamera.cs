using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class PerspectiveCamera : Camera
	{
		public Angle FieldOfView = Angle.FromDegrees(73f);

		public float NearPlane = 0.01f;

		public float FarPlane = 1000f;

		public override Matrix View
		{
			get
			{
				return base.WorldToLocal;
			}
		}

		public override Matrix GetProjection(GraphicsDevice device)
		{
			return Matrix.CreatePerspectiveFieldOfView(FieldOfView.Radians, device.Viewport.AspectRatio, NearPlane, FarPlane);
		}
	}
}
