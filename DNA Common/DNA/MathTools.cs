using System;
using DNA.Collections;
using Microsoft.Xna.Framework;

namespace DNA
{
	public static class MathTools
	{
		public const float Sqrt2 = 1.414213f;

		private static readonly byte[] BitsSetTable256 = new byte[256]
		{
			0, 1, 1, 2, 1, 2, 2, 3, 1, 2,
			2, 3, 2, 3, 3, 4, 1, 2, 2, 3,
			2, 3, 3, 4, 2, 3, 3, 4, 3, 4,
			4, 5, 1, 2, 2, 3, 2, 3, 3, 4,
			2, 3, 3, 4, 3, 4, 4, 5, 2, 3,
			3, 4, 3, 4, 4, 5, 3, 4, 4, 5,
			4, 5, 5, 6, 1, 2, 2, 3, 2, 3,
			3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4,
			4, 5, 4, 5, 5, 6, 2, 3, 3, 4,
			3, 4, 4, 5, 3, 4, 4, 5, 4, 5,
			5, 6, 3, 4, 4, 5, 4, 5, 5, 6,
			4, 5, 5, 6, 5, 6, 6, 7, 1, 2,
			2, 3, 2, 3, 3, 4, 2, 3, 3, 4,
			3, 4, 4, 5, 2, 3, 3, 4, 3, 4,
			4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
			2, 3, 3, 4, 3, 4, 4, 5, 3, 4,
			4, 5, 4, 5, 5, 6, 3, 4, 4, 5,
			4, 5, 5, 6, 4, 5, 5, 6, 5, 6,
			6, 7, 2, 3, 3, 4, 3, 4, 4, 5,
			3, 4, 4, 5, 4, 5, 5, 6, 3, 4,
			4, 5, 4, 5, 5, 6, 4, 5, 5, 6,
			5, 6, 6, 7, 3, 4, 4, 5, 4, 5,
			5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
			4, 5, 5, 6, 5, 6, 6, 7, 5, 6,
			6, 7, 6, 7, 7, 8
		};

		private static readonly byte[] LogTable256 = new byte[256]
		{
			0, 0, 1, 1, 2, 2, 2, 2, 3, 3,
			3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
			4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
			4, 4, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7
		};

		private static readonly int[] MultiplyDeBruijnBitPosition = new int[32]
		{
			0, 1, 28, 2, 29, 14, 24, 3, 30, 22,
			20, 15, 25, 17, 4, 8, 31, 27, 13, 23,
			21, 19, 16, 7, 26, 12, 18, 6, 11, 5,
			10, 9
		};

		public static Random Rnd = new Random();

		public static Angle AngleBetween(this Quaternion q1, Quaternion q2)
		{
			return Angle.ACos(Quaternion.Dot(q1, q2)) * 2f;
		}

		public static Angle AngleBetween(this Vector3 v1, Vector3 v2)
		{
			v1.Normalize();
			v2.Normalize();
			float val = Vector3.Dot(v1, v2);
			val = Math.Min(val, 1f);
			val = Math.Max(val, -1f);
			float rads = (float)Math.Acos(val);
			return Angle.FromRadians(rads);
		}

		public static Quaternion RotationBetween(this Vector3 v1, Vector3 v2)
		{
			v1.Normalize();
			v2.Normalize();
			Vector3 vector = Vector3.Cross(v1, v2);
			return new Quaternion(vector.X, vector.Y, vector.Z, 1f + Vector3.Dot(v1, v2));
		}

		public static double Distance(this Point p1, Point p2)
		{
			int num = p2.X - p1.X;
			int num2 = p2.Y - p1.Y;
			return Math.Sqrt(num * num + num2 * num2);
		}

		public static float Cross(this Vector2 v1, Vector2 v2)
		{
			return v1.X * v2.Y - v1.Y * v2.X;
		}

		public static int[] RandomArray(int length)
		{
			return RandomArray(0, length);
		}

		public static int[] RandomArray(int start, int length)
		{
			return new Random().RandomArray(start, length);
		}

		public static int[] RandomArray(this Random rand, int length)
		{
			return rand.RandomArray(0, length);
		}

		public static bool Decide(this Random rand, float percentTrue)
		{
			return rand.NextDouble() > (double)percentTrue;
		}

		public static int[] RandomArray(this Random rand, int start, int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = start + i;
			}
			ArrayTools.Randomize(array, rand);
			return array;
		}

		public static double RandomDouble(this Random rand, double min, double max)
		{
			if (max < min)
			{
				double num = max;
				max = min;
				min = num;
			}
			double num2 = max - min;
			double num3 = num2 * rand.NextDouble();
			return min + num3;
		}

		public static float RandomFloat()
		{
			return (float)Rnd.NextDouble();
		}

		public static float RandomFloat(float magnitude)
		{
			return (float)Rnd.NextDouble() * magnitude;
		}

		public static float RandomFloat(float min, float max)
		{
			return MathHelper.Lerp(min, max, (float)Rnd.NextDouble());
		}

		public static int RandomInt()
		{
			return Rnd.Next();
		}

		public static int RandomInt(int max)
		{
			return Rnd.Next(0, max);
		}

		public static int RandomInt(int min, int max)
		{
			return Rnd.Next(min, max);
		}

		public static bool RandomBool()
		{
			return Rnd.Next(0, 2) == 0;
		}

		public static int IntDifference(float a, float b)
		{
			throw new NotImplementedException();
		}

		public static int IntDifference(double a, double b)
		{
			throw new NotImplementedException();
		}

		public static int IntRound(double d)
		{
			return (int)Math.Round(d);
		}

		public static int IntRound(float d)
		{
			return (int)Math.Round(d);
		}

		public static float MapAndLerp(float value, float map1, float map2, float lerp1, float lerp2)
		{
			float amount = ((value - map1) / (map2 - map1)).Clamp(0f, 1f);
			return MathHelper.Lerp(lerp1, lerp2, amount);
		}

		public static float Clamp(this float val, float min, float max)
		{
			if (val < min)
			{
				return min;
			}
			if (val > max)
			{
				return max;
			}
			return val;
		}

		public static int Clamp(this int val, int min, int max)
		{
			if (val < min)
			{
				return min;
			}
			if (val > max)
			{
				return max;
			}
			return val;
		}

		public static bool IsPowerOf2(this int v)
		{
			if ((v & (v - 1)) == 0)
			{
				return v != 0;
			}
			return false;
		}

		public static bool IsPowerOf2(this uint v)
		{
			if ((v & (v - 1)) == 0)
			{
				return v != 0;
			}
			return false;
		}

		public static uint NextPowerOf2(this uint v)
		{
			v--;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v++;
			return v;
		}

		public static int LogBase2(this uint v)
		{
			uint num;
			uint num2;
			if ((num = v >> 16) != 0)
			{
				if ((num2 = num >> 8) == 0)
				{
					return 16 + LogTable256[num];
				}
				return 24 + LogTable256[num2];
			}
			if ((num2 = v >> 8) == 0)
			{
				return LogTable256[v];
			}
			return 8 + LogTable256[num2];
		}

		public static Matrix QuickInvert(this Matrix mat)
		{
			Matrix identity = Matrix.Identity;
			Vector3 translation = mat.Translation;
			identity.M11 = mat.M11;
			identity.M12 = mat.M21;
			identity.M13 = mat.M31;
			identity.M21 = mat.M12;
			identity.M22 = mat.M22;
			identity.M23 = mat.M32;
			identity.M31 = mat.M13;
			identity.M32 = mat.M23;
			identity.M33 = mat.M33;
			identity.M41 = 0f - Vector3.Dot(translation, mat.Right);
			identity.M42 = 0f - Vector3.Dot(translation, mat.Up);
			identity.M43 = 0f - Vector3.Dot(translation, mat.Forward);
			return identity;
		}

		public static int TrailingZeroBits(this uint v)
		{
			return MultiplyDeBruijnBitPosition[(v & (0L - (long)v)) * 125613361 >> 27];
		}

		public static int BitsSet(this uint v)
		{
			return BitsSetTable256[v & 0xFF] + BitsSetTable256[(v >> 8) & 0xFF] + BitsSetTable256[(v >> 16) & 0xFF] + BitsSetTable256[v >> 24];
		}

		public static uint Max(uint a, uint b, uint c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				return b;
			}
			if (a <= c)
			{
				return c;
			}
			return a;
		}

		public static int Max(int a, int b, int c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				return b;
			}
			if (a <= c)
			{
				return c;
			}
			return a;
		}

		public static float Max(float a, float b, float c)
		{
			if (!(a > b))
			{
				if (!(b > c))
				{
					return c;
				}
				return b;
			}
			if (!(a > c))
			{
				return c;
			}
			return a;
		}

		public static decimal Max(decimal a, decimal b, decimal c)
		{
			if (!(a > b))
			{
				if (!(b > c))
				{
					return c;
				}
				return b;
			}
			if (!(a > c))
			{
				return c;
			}
			return a;
		}

		public static short Max(short a, short b, short c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				return b;
			}
			if (a <= c)
			{
				return c;
			}
			return a;
		}

		public static char Max(char a, char b, char c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				return b;
			}
			if (a <= c)
			{
				return c;
			}
			return a;
		}

		public static byte Max(byte a, byte b, byte c)
		{
			if (a <= b)
			{
				if (b <= c)
				{
					return c;
				}
				return b;
			}
			if (a <= c)
			{
				return c;
			}
			return a;
		}

		public static uint Min(uint a, uint b, uint c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				return b;
			}
			if (a >= c)
			{
				return c;
			}
			return a;
		}

		public static int Min(int a, int b, int c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				return b;
			}
			if (a >= c)
			{
				return c;
			}
			return a;
		}

		public static float Min(float a, float b, float c)
		{
			if (!(a < b))
			{
				if (!(b < c))
				{
					return c;
				}
				return b;
			}
			if (!(a < c))
			{
				return c;
			}
			return a;
		}

		public static decimal Min(decimal a, decimal b, decimal c)
		{
			if (!(a < b))
			{
				if (!(b < c))
				{
					return c;
				}
				return b;
			}
			if (!(a < c))
			{
				return c;
			}
			return a;
		}

		public static short Min(short a, short b, short c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				return b;
			}
			if (a >= c)
			{
				return c;
			}
			return a;
		}

		public static char Min(char a, char b, char c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				return b;
			}
			if (a >= c)
			{
				return c;
			}
			return a;
		}

		public static byte Min(byte a, byte b, byte c)
		{
			if (a >= b)
			{
				if (b >= c)
				{
					return c;
				}
				return b;
			}
			if (a >= c)
			{
				return c;
			}
			return a;
		}

		public static int DecimalToPow2Fraction(double val, out int denom, int maxDenom)
		{
			double num = 1.0;
			int num2 = maxDenom;
			for (int num3 = 1; num3 < maxDenom; num3 <<= 1)
			{
				double num4 = val * (double)num3;
				double num5 = num4 - Math.Floor(num4);
				if (num5 == 0.0)
				{
					denom = num3;
					return (int)num4;
				}
				if (num5 < num)
				{
					num = num5;
					num2 = num3;
				}
			}
			denom = num2;
			return (int)val * num2;
		}

		public static int Factorial(int n)
		{
			int num = 1;
			for (int num2 = n; num2 > 1; num2--)
			{
				num *= num2;
			}
			return num;
		}

		public static int Permutations(int n, int r)
		{
			return Factorial(n) / Factorial(n - r);
		}

		public static int Combinations(int n, int r)
		{
			return Permutations(n, r) / Factorial(r);
		}

		public static int DecimalToFraction(double val, out int denom, int maxDenom)
		{
			double num = 1.0;
			int num2 = maxDenom;
			for (int i = 1; i < maxDenom; i++)
			{
				double num3 = val * (double)i;
				double num4 = num3 - Math.Floor(num3);
				if (num4 == 0.0)
				{
					denom = i;
					return (int)num3;
				}
				if (num4 < num)
				{
					num = num4;
					num2 = i;
				}
			}
			denom = num2;
			return (int)val * num2;
		}

		public static float Square(float x)
		{
			return x * x;
		}

		public static bool CalculateInitialBallisticVector(Vector3 pt1, Vector3 pt2, float vel, out Vector3 res, float gravity)
		{
			float num = gravity * gravity;
			float num2 = pt2.Y - pt1.Y;
			float num3 = (float)Math.Sqrt(Square(pt1.X - pt2.X) + Square(pt1.Z - pt2.Z));
			res = Vector3.Zero;
			if (num3 == 0f)
			{
				if (num2 >= 0f)
				{
					res = new Vector3(0f, vel, 0f);
				}
				else
				{
					res = new Vector3(0f, 0f - vel, 0f);
				}
				return true;
			}
			float num4 = num2 * num2;
			float num5 = num3 * num3;
			float num6 = 2f * num4 + 2f * num5;
			if (num6 == 0f)
			{
				return false;
			}
			float num7 = vel * vel;
			float num8 = 2f * gravity * num2 * num7 + num7 * num7 - num * num5;
			if (num8 < 0f)
			{
				return false;
			}
			num8 = (float)Math.Sqrt(num8);
			float num9 = gravity * num2 + num7;
			float num10 = num9 - num8;
			float num11 = num9 + num8;
			float num12 = num5 + num4;
			num10 *= num12;
			num11 *= num12;
			num6 = 1.414213f * num3 / num6;
			bool flag = false;
			if (num10 >= 0f)
			{
				num10 = (float)Math.Sqrt(num10) * num6;
				flag = true;
			}
			if (num11 >= 0f)
			{
				num11 = (float)Math.Sqrt(num11) * num6;
				if (!flag || num11 > num10)
				{
					flag = true;
					num10 = num11;
				}
			}
			if (flag)
			{
				if (num10 <= 0f)
				{
					return false;
				}
				float num13 = num10 * num10;
				if (num13 > num7)
				{
					return false;
				}
				float num14 = num3 / num10;
				float num15 = (float)Math.Sqrt(num7 - num13);
				if (Math.Abs(num2 - (num15 * num14 + 0.5f * gravity * num14 * num14)) > Math.Abs(num2 - ((0f - num15) * num14 + 0.5f * gravity * num14 * num14)))
				{
					num15 = 0f - num15;
				}
				Vector2 vector = new Vector2(pt2.X - pt1.X, pt2.Z - pt1.Z);
				vector.Normalize();
				res.X = num10 * vector.X;
				res.Y = num15;
				res.Z = num10 * vector.Y;
			}
			return flag;
		}

		public static float MoveTowardTarget(float current, float target, float rate, float dt)
		{
			float num3;
			if (target != current)
			{
				float num = target - current;
				float num2 = Math.Abs(num);
				if (num2 > rate)
				{
					num *= rate / num2;
				}
				num3 = current + num * dt;
				if (Math.Abs(num3 - target) < 0.01f)
				{
					num3 = target;
				}
			}
			else
			{
				num3 = current;
			}
			return num3;
		}

		public static float MoveTowardTargetAngle(float current, float target, float rate, float dt)
		{
			float num3;
			if (target != current)
			{
				float num = MathHelper.WrapAngle(target - current);
				float num2 = Math.Abs(num);
				if (num2 > rate)
				{
					num *= rate / num2;
				}
				num3 = MathHelper.WrapAngle(current + num * dt);
				if (Math.Abs(num3 - target) < 0.01f)
				{
					num3 = target;
				}
			}
			else
			{
				num3 = current;
			}
			return num3;
		}

		public static Vector3 Hermite1stDerivative(Vector3 Point1, Vector3 Tangent1, Vector3 Point2, Vector3 Tangent2, float t)
		{
			float num = t * t;
			Vector3 vector = Point1 * (6f * (num - t));
			vector += Tangent1 * (3f * num - 4f * t + 1f);
			vector += Point2 * (6f * (t - num));
			return vector + Tangent2 * (3f * num - 2f * t);
		}

		public static Vector3 Hermite2ndDerivative(Vector3 Point1, Vector3 Tangent1, Vector3 Point2, Vector3 Tangent2, float t)
		{
			Vector3 vector = Point1 * (12f * t - 6f);
			vector += Tangent1 * (6f * t - 4f);
			vector += Point2 * (6f - 12f * t);
			return vector + Tangent2 * (6f * t - 2f);
		}
	}
}
