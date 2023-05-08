using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public interface ISpline
	{
		Vector3 ComputeValue(float t);

		Vector3 ComputeVelocity(float t);

		Vector3 ComputeAcceleration(float t);
	}
}
