using DNA.CastleMinerZ.Terrain;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Utils.Trace
{
	public class TraceProbe
	{
		private static Plane[] _boxFaces = new Plane[6]
		{
			new Plane(1f, 0f, 0f, 0f),
			new Plane(0f, 0f, -1f, 0f),
			new Plane(-1f, 0f, 0f, 0f),
			new Plane(0f, 0f, 1f, 0f),
			new Plane(0f, 1f, 0f, 0f),
			new Plane(0f, -1f, 0f, 0f)
		};

		public BoundingBox _bounds;

		public Vector3 _start;

		public Vector3 _end;

		public float _inT;

		public float _outT;

		public Vector3 _inNormal;

		public Vector3 _outNormal;

		public bool _collides;

		public bool _startsIn;

		public bool _endsIn;

		public IntVector3 _worldIndex;

		public BlockFace _inFace;

		public BlockFace _outFace;

		public bool SkipEmbedded;

		public bool TraceCompletePath;

		protected BlockTypeEnum _currentTestingBlockType = BlockTypeEnum.NumberOfBlocks;

		public virtual bool AbleToBuild
		{
			get
			{
				if (_collides)
				{
					return !_startsIn;
				}
				return false;
			}
		}

		public static Plane[] MakeBox(BoundingBox bb)
		{
			_boxFaces[0].D = 0f - bb.Max.X;
			_boxFaces[1].D = bb.Min.Z;
			_boxFaces[2].D = bb.Min.X;
			_boxFaces[3].D = 0f - bb.Max.Z;
			_boxFaces[4].D = 0f - bb.Max.Y;
			_boxFaces[5].D = bb.Min.Y;
			return _boxFaces;
		}

		public static void MakeOrientedBox(Matrix mat, BoundingBox bb, Plane[] planes)
		{
			Plane[] array = MakeBox(bb);
			for (int i = 0; i < 6; i++)
			{
				planes[i] = Plane.Transform(array[i], mat);
			}
		}

		public virtual void Init(Vector3 start, Vector3 end)
		{
			Vector3 min = Vector3.Min(start, end);
			Vector3 max = Vector3.Max(start, end);
			_bounds = new BoundingBox(min, max);
			_start = start;
			_end = end;
			Reset();
		}

		public virtual void Reset()
		{
			_startsIn = false;
			_endsIn = false;
			_inT = 1f;
			_outT = 0f;
			_collides = false;
		}

		public virtual bool TestThisType(BlockTypeEnum e)
		{
			return BlockType.GetType(e).CanBeTouched;
		}

		public virtual Vector3 GetIntersection()
		{
			if (_collides)
			{
				return CalculatePoint(_inT);
			}
			return Vector3.Zero;
		}

		public virtual Vector3 CalculatePoint(float t)
		{
			return Vector3.Lerp(_start, _end, t);
		}

		public virtual bool TouchesBlock(float inT, ref Vector3 inNormal, bool startsIn, BlockFace inFace, float outT, ref Vector3 outNormal, bool endsIn, BlockFace outFace, IntVector3 worldindex)
		{
			return true;
		}

		public bool SetIntersection(float inT, ref Vector3 inNormal, bool startsIn, BlockFace inFace, float outT, ref Vector3 outNormal, bool endsIn, BlockFace outFace, IntVector3 worldindex)
		{
			bool result = false;
			if (startsIn && SkipEmbedded)
			{
				return result;
			}
			if (TraceCompletePath)
			{
				result = TouchesBlock(inT, ref inNormal, startsIn, inFace, outT, ref outNormal, endsIn, outFace, worldindex);
				bool flag = false;
				bool flag2 = false;
				if (!_collides)
				{
					flag = true;
					flag2 = true;
				}
				else
				{
					if (inT < _inT)
					{
						flag = true;
					}
					if (outT > _outT)
					{
						flag2 = true;
					}
				}
				if (flag || flag2)
				{
					_collides = true;
					_worldIndex = worldindex;
					if (flag)
					{
						_inT = inT;
						_inNormal = inNormal;
						_startsIn = startsIn;
						_inFace = inFace;
					}
					if (flag2)
					{
						_outT = outT;
						_outNormal = outNormal;
						_endsIn = endsIn;
						_outFace = outFace;
					}
				}
			}
			else if (!_collides || inT < _inT)
			{
				_collides = true;
				_inT = inT;
				_inNormal = inNormal;
				_startsIn = startsIn;
				_inFace = inFace;
				_outT = outT;
				_outNormal = outNormal;
				_endsIn = endsIn;
				_outFace = outFace;
				_worldIndex = worldindex;
			}
			return result;
		}

		public virtual bool TestBoundBox(BoundingBox bb)
		{
			return TestShape(MakeBox(bb), IntVector3.Zero);
		}

		public virtual bool TestShape(Plane[] planes, IntVector3 worldIndex, BlockTypeEnum blockType)
		{
			_currentTestingBlockType = blockType;
			return TestShape(planes, worldIndex);
		}

		public virtual bool TestShape(Plane[] planes, IntVector3 worldIndex)
		{
			float num = 0f;
			float num2 = 1f;
			bool startsIn = true;
			bool endsIn = true;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < planes.Length; i++)
			{
				float num5 = planes[i].DotCoordinate(_start);
				float num6 = planes[i].DotCoordinate(_end);
				bool flag = num5 > 0f;
				bool flag2 = num6 > 0f;
				if (flag && flag2)
				{
					return false;
				}
				if (flag == flag2)
				{
					continue;
				}
				if (num5 > num6)
				{
					float num7 = num5 / (num5 - num6);
					if (num7 > num)
					{
						startsIn = false;
						num3 = i;
						num = num7;
					}
				}
				else
				{
					float num8 = (0f - num5) / (num6 - num5);
					if (num8 < num2)
					{
						endsIn = false;
						num4 = i;
						num2 = num8;
					}
				}
			}
			if (num <= num2)
			{
				return SetIntersection(num, ref planes[num3].Normal, startsIn, (BlockFace)num3, num2, ref planes[num4].Normal, endsIn, (BlockFace)num4, worldIndex);
			}
			return true;
		}
	}
}
