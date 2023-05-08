using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public class SegmentedLinearSpline : Spline
	{
		private Vector3[] _points;

		private float[] _weights;

		public Vector3[] Points
		{
			get
			{
				return _points;
			}
			set
			{
				_points = value;
			}
		}

		private void ComputeWeights()
		{
			_weights = new float[_points.Length - 1];
			float num = 0f;
			for (int i = 0; i < _points.Length - 1; i++)
			{
				num += Vector3.Distance(_points[i], _points[i + 1]);
				_weights[i] = num;
			}
			for (int j = 0; j < _weights.Length; j++)
			{
				_weights[j] /= num;
			}
		}

		public override Vector3 ComputeValue(float t)
		{
			ComputeWeights();
			int i;
			for (i = 0; t > _weights[i] && i < _weights.Length; i++)
			{
			}
			float num = _weights[i];
			float num2 = _weights[i - 1];
			t = (t - num) / (num2 - num);
			Vector3 vector = _points[i - 1];
			Vector3 vector2 = _points[i];
			return vector + (vector2 - vector) * t;
		}

		public override Vector3 ComputeVelocity(float t)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override Vector3 ComputeAcceleration(float t)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
