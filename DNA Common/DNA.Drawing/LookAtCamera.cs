using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class LookAtCamera : PerspectiveCamera
	{
		public Entity LookAtEntity;

		public Vector3 Up = new Vector3(0f, 1f, 0f);

		public override Matrix View
		{
			get
			{
				if (LookAtEntity != null)
				{
					Vector3 worldPosition = base.WorldPosition;
					Vector3 worldPosition2 = LookAtEntity.WorldPosition;
					return Matrix.CreateLookAt(worldPosition, worldPosition2, Up);
				}
				return base.View;
			}
		}
	}
}
