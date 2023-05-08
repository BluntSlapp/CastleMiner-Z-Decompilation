using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class DiscreetTransform : Transform
	{
		private Vector3 _localPosition = Vector3.Zero;

		private Quaternion _localRotation = Quaternion.Identity;

		private Vector3 _localScale = new Vector3(1f, 1f, 1f);

		public Vector3 Position
		{
			get
			{
				return _localPosition;
			}
			set
			{
				_localPosition = value;
			}
		}

		public Vector3 Scale
		{
			get
			{
				return _localScale;
			}
			set
			{
				_localScale = value;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return _localRotation;
			}
			set
			{
				_localRotation = value;
				_localRotation.Normalize();
			}
		}

		public override Matrix Matrix
		{
			get
			{
				return Matrix.CreateScale(_localScale) * Matrix.CreateFromQuaternion(_localRotation) * Matrix.CreateTranslation(_localPosition);
			}
		}
	}
}
