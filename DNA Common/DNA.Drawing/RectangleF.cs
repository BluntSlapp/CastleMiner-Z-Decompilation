using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct RectangleF
	{
		private Vector2 _location;

		private Vector2 _size;

		public static readonly RectangleF Empty = new RectangleF(new Vector2(0f, 0f), new Vector2(0f, 0f));

		public float Bottom
		{
			get
			{
				return _location.Y + _size.Y;
			}
		}

		public float Height
		{
			get
			{
				return _size.Y;
			}
			set
			{
				_size.Y = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				if (_size.X != 0f)
				{
					return _size.Y == 0f;
				}
				return true;
			}
		}

		public float Left
		{
			get
			{
				return X;
			}
		}

		public Vector2 Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public float Right
		{
			get
			{
				return _location.X + _size.X;
			}
		}

		public Vector2 Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public float Top
		{
			get
			{
				return _location.Y;
			}
		}

		public float Width
		{
			get
			{
				return _size.X;
			}
			set
			{
				_size.X = value;
			}
		}

		public float X
		{
			get
			{
				return _location.X;
			}
			set
			{
				_location.X = value;
			}
		}

		public float Y
		{
			get
			{
				return _location.Y;
			}
			set
			{
				_location.Y = value;
			}
		}

		public RectangleF(Vector2 location, Vector2 size)
		{
			_location = location;
			_size = size;
		}

		public RectangleF(float x, float y, float width, float height)
		{
			_location = new Vector2(x, y);
			_size = new Vector2(width, height);
		}

		public static bool operator !=(RectangleF left, RectangleF right)
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(RectangleF left, RectangleF right)
		{
			throw new NotImplementedException();
		}

		public static implicit operator RectangleF(Rectangle r)
		{
			return new RectangleF(r.X, r.Y, r.Width, r.Height);
		}

		public bool Contains(Vector2 pt)
		{
			if (pt.X >= _location.X && pt.Y >= _location.Y && pt.X <= Right)
			{
				return pt.Y <= Bottom;
			}
			return false;
		}

		public bool Contains(RectangleF rect)
		{
			if (rect.Left >= Left && rect.Right <= Right && rect.Top >= Top)
			{
				return rect.Bottom <= Bottom;
			}
			return false;
		}

		public bool Contains(float x, float y)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			throw new NotImplementedException();
		}

		public static RectangleF FromLTRB(float left, float top, float right, float bottom)
		{
			throw new NotImplementedException();
		}

		public void Inflate(Vector2 size)
		{
			throw new NotImplementedException();
		}

		public void Inflate(float x, float y)
		{
			throw new NotImplementedException();
		}

		public static RectangleF Inflate(RectangleF rect, float x, float y)
		{
			throw new NotImplementedException();
		}

		public void Intersect(RectangleF rect)
		{
			throw new NotImplementedException();
		}

		public static RectangleF Intersect(RectangleF a, RectangleF b)
		{
			throw new NotImplementedException();
		}

		public bool IntersectsWith(RectangleF rect)
		{
			if (!(rect.Right < Left) && !(rect.Left > Right) && !(rect.Bottom < Top))
			{
				return !(rect.Top > Bottom);
			}
			return false;
		}

		public void Offset(Vector2 pos)
		{
			throw new NotImplementedException();
		}

		public void Offset(float x, float y)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return X + "," + Y + "," + Width + "," + Height;
		}

		public static RectangleF Union(RectangleF a, RectangleF b)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}
