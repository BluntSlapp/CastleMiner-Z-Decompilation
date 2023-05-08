using System;
using DNA.CastleMinerZ.Terrain;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Utils.Trace
{
	public class AABBTraceProbe : TraceProbe
	{
		private Vector3 _offsetToRay;

		private Vector3 _halfVec;

		private Vector3 _direction;

		private bool hasDirection;

		public float Radius
		{
			get
			{
				return _halfVec.X;
			}
		}

		public void Init(Vector3 start, Vector3 end, BoundingBox box)
		{
			Vector3 vector = Vector3.Multiply(box.Min + box.Max, 0.5f);
			_offsetToRay = Vector3.Negate(vector);
			_start = start + vector;
			_end = end + vector;
			_halfVec = box.Max - vector;
			_halfVec.X = Math.Abs(_halfVec.X);
			_halfVec.Y = Math.Abs(_halfVec.Y);
			_halfVec.Z = Math.Abs(_halfVec.Z);
			Vector3 min = Vector3.Min(_start, _end);
			min -= _halfVec;
			Vector3 max = Vector3.Max(_start, _end);
			max += _halfVec;
			_bounds = new BoundingBox(min, max);
			_direction = _start - _end;
			hasDirection = _direction.LengthSquared() > 0f;
			_direction.Normalize();
			Reset();
		}

		public override bool TestThisType(BlockTypeEnum e)
		{
			return BlockType.GetType(e).BlockPlayer;
		}

		public override Vector3 GetIntersection()
		{
			if (_collides)
			{
				return base.GetIntersection() + _offsetToRay;
			}
			return Vector3.Zero;
		}

		public override bool TestShape(Plane[] planes, IntVector3 worldIndex)
		{
			float num = 0f;
			float num2 = 1f;
			bool startsIn = true;
			bool endsIn = true;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < planes.Length; i++)
			{
				Vector3 vector = Vector3.Multiply(_halfVec, planes[i].Normal);
				float num5 = 0f - (Math.Abs(vector.X) + Math.Abs(vector.Y) + Math.Abs(vector.Z));
				float num6 = planes[i].DotCoordinate(_start) + num5;
				float num7 = planes[i].DotCoordinate(_end) + num5;
				bool flag;
				if (num6 > -0.0001f)
				{
					flag = true;
					num6 = Math.Max(num6, 0f);
				}
				else
				{
					flag = false;
				}
				bool flag2;
				if (num7 > -0.0001f)
				{
					flag2 = true;
					num7 = Math.Max(num7, 0f);
				}
				else
				{
					flag2 = false;
				}
				if (flag && flag2)
				{
					return false;
				}
				if (flag == flag2)
				{
					continue;
				}
				if (num6 > num7)
				{
					float num8 = num6 / (num6 - num7);
					if (num8 >= num)
					{
						startsIn = false;
						num3 = i;
						num = num8;
					}
				}
				else
				{
					float num9 = (0f - num6) / (num7 - num6);
					if (num9 <= num2)
					{
						endsIn = false;
						num4 = i;
						num2 = num9;
					}
				}
			}
			if (num <= num2 && (!hasDirection || Math.Abs(Vector3.Dot(_direction, planes[num3].Normal)) > 1E-05f))
			{
				SetIntersection(num, ref planes[num3].Normal, startsIn, (BlockFace)num3, num2, ref planes[num4].Normal, endsIn, (BlockFace)num4, worldIndex);
			}
			return true;
		}
	}
}
