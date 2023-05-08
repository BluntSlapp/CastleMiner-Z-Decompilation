using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public abstract class Spline : ISpline
	{
		public abstract Vector3 ComputeValue(float t);

		public abstract Vector3 ComputeVelocity(float t);

		public abstract Vector3 ComputeAcceleration(float t);

		protected static int GetControlPointIndex(int total, ref float t)
		{
			float num = 1f / (float)(total - 1);
			float num2 = t / num;
			int num3 = (int)num2;
			t = num2 - (float)num3;
			return num3;
		}
	}
}
