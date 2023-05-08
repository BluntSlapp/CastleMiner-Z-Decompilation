using DNA.Drawing.Collision;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class BoundedLookAtCamera : LookAtCamera
	{
		private CollisionMap _bsp;

		private int _percision = 4;

		public override Matrix View
		{
			get
			{
				if (LookAtEntity != null)
				{
					Vector3 worldPosition = base.WorldPosition;
					Vector3 worldPosition2 = LookAtEntity.WorldPosition;
					LineF3D line = new LineF3D(worldPosition2, worldPosition);
					float? num = _bsp.CollidesWith(line);
					if (num.HasValue)
					{
						return Matrix.CreateLookAt(line.GetValue(num.Value * 0.9f), worldPosition2, Vector3.Up);
					}
				}
				return base.View;
			}
		}

		public BoundedLookAtCamera(CollisionMap bsp, int percision)
		{
			_bsp = bsp;
			_percision = percision;
		}
	}
}
