using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	[Serializable]
	public struct ColorF
	{
		private float _red;

		private float _green;

		private float _blue;

		private float _alpha;

		public float Red
		{
			get
			{
				return _red;
			}
		}

		public float Green
		{
			get
			{
				return _green;
			}
		}

		public float Blue
		{
			get
			{
				return _blue;
			}
		}

		public float Alpha
		{
			get
			{
				return _alpha;
			}
		}

		public static ColorF FromRGB(float r, float g, float b)
		{
			return new ColorF(1f, r, g, b);
		}

		public static ColorF FromARGB(float a, float r, float g, float b)
		{
			return new ColorF(a, r, g, b);
		}

		public static ColorF FromColor(Color col)
		{
			return new ColorF(col);
		}

		public static implicit operator Color(ColorF col)
		{
			return col.GetColor();
		}

		public static implicit operator ColorF(Color col)
		{
			return FromColor(col);
		}

		public ColorF Brighten(float factor)
		{
			float r = Red * factor;
			float g = Green * factor;
			float b = Blue * factor;
			return FromARGB(Alpha, r, g, b);
		}

		public ColorF Saturate(float factor)
		{
			Angle h;
			float v;
			float s;
			GetHSV(out h, out s, out v);
			s *= factor;
			return FromAHSV(Alpha, h, s, v);
		}

		public ColorF AdjustBrightness(float factor)
		{
			float r = Red * factor;
			float g = Green * factor;
			float b = Blue * factor;
			return FromARGB(Alpha, r, g, b);
		}

		private static float ColVal(float n1, float n2, float hue)
		{
			if (hue > 360f)
			{
				hue -= 360f;
			}
			else if (hue < 0f)
			{
				hue += 360f;
			}
			if (hue < 60f)
			{
				return n1 + (n2 - n1) * hue / 60f;
			}
			if (hue < 180f)
			{
				return n2;
			}
			if (hue < 240f)
			{
				return n1 + (n2 - n1) * (240f - hue) / 60f;
			}
			return n1;
		}

		public static ColorF FromHSL(Angle hue, float sat, float brt)
		{
			return FromAHSL(1f, hue, sat, brt);
		}

		public static ColorF FromAHSL(float alpha, Angle hue, float sat, float brt)
		{
			float num = ((brt <= 0.5f) ? (brt * (1f + sat)) : (brt + sat - brt * sat));
			float n = 2f * brt - num;
			float r;
			float g;
			float b;
			if (sat == 0f)
			{
				r = (g = (b = brt));
			}
			else
			{
				float num2 = hue.Degrees;
				r = ColVal(n, num, num2 + 120f);
				g = ColVal(n, num, num2);
				b = ColVal(n, num, num2 - 120f);
			}
			return FromARGB(alpha, r, g, b);
		}

		public static ColorF FromHSV(Angle hue, float sat, float brt)
		{
			return FromAHSV(1f, hue, sat, brt);
		}

		public static ColorF FromAHSV(float alpha, Angle h, float sat, float brt)
		{
			float r;
			float g;
			float b;
			if (sat == 0f)
			{
				r = (g = (b = brt));
			}
			else
			{
				float num = h.Degrees;
				if (num == 360f)
				{
					num = 0f;
				}
				num /= 60f;
				int num2 = (int)Math.Floor(num);
				float num3 = num - (float)num2;
				float num4 = brt * (1f - sat);
				float num5 = brt * (1f - sat * num3);
				float num6 = brt * (1f - sat * (1f - num3));
				switch (num2)
				{
				case 0:
					r = brt;
					g = num6;
					b = num4;
					break;
				case 1:
					r = num5;
					g = brt;
					b = num4;
					break;
				case 2:
					r = num4;
					g = brt;
					b = num6;
					break;
				case 3:
					r = num4;
					g = num5;
					b = brt;
					break;
				case 4:
					r = num6;
					g = num4;
					b = brt;
					break;
				case 5:
					r = brt;
					g = num4;
					b = num5;
					break;
				default:
					r = 0f;
					g = 0f;
					b = 0f;
					throw new Exception("Hue Out of Range");
				}
			}
			return FromARGB(alpha, r, g, b);
		}

		public static ColorF FromCMY(float c, float m, float y)
		{
			return FromACMY(1f, c, m, y);
		}

		public static ColorF FromACMY(float a, float c, float m, float y)
		{
			return FromARGB(a, 1f - c, 1f - m, 1f - y);
		}

		public static ColorF FromCMYK(float c, float m, float y, float k)
		{
			return FromACMYK(1f, c, m, y, k);
		}

		public static ColorF FromACMYK(float a, float c, float m, float y, float k)
		{
			c = c * (1f - k) + k;
			m = m * (1f - k) + k;
			y = y * (1f - k) + k;
			return FromACMY(a, c, m, y);
		}

		public void GetHSL(out Angle h, out float s, out float l)
		{
			float num = MathTools.Max(Red, Green, Blue);
			float num2 = MathTools.Min(Red, Green, Blue);
			l = (num + num2) / 2f;
			float num3 = num - num2;
			if (num3 == 0f)
			{
				s = 0f;
				h = Angle.FromDegrees(0f);
				return;
			}
			if (l < 0.5f)
			{
				s = num3 / (num + num2);
			}
			else
			{
				s = num3 / (2f - (num + num2));
			}
			float num4 = ((Red == num) ? ((Green - Blue) / num3) : ((Green == num) ? (2f + (Blue - Red) / num3) : ((Blue != num) ? 0f : (4f + (Red - Green) / num3))));
			num4 *= 60f;
			if (num4 < 0f)
			{
				num4 += 360f;
			}
			h = Angle.FromDegrees(num4);
		}

		public void GetHSV(out Angle h, out float s, out float v)
		{
			float red = Red;
			float green = Green;
			float blue = Blue;
			float num = MathTools.Max(red, green, blue);
			float num2 = MathTools.Min(red, green, blue);
			v = num;
			float num3 = num - num2;
			if (num == 0f || num3 == 0f)
			{
				h = Angle.FromDegrees(0f);
				s = 0f;
				return;
			}
			s = num3 / num;
			float num4 = ((red == num) ? ((green - blue) / num3) : ((green != num) ? (4f + (red - green) / num3) : (2f + (blue - red) / num3)));
			num4 *= 60f;
			if (num4 < 0f)
			{
				num4 += 360f;
			}
			h = Angle.FromDegrees(num4);
		}

		public void GetCMY(out float c, out float m, out float y)
		{
			c = 1f - Red;
			m = 1f - Green;
			y = 1f - Blue;
		}

		public void GetCMYK(out float c, out float m, out float y, out float k)
		{
			c = 1f - Red;
			m = 1f - Green;
			y = 1f - Blue;
			k = 1f;
			if (c < k)
			{
				k = c;
			}
			if (m < k)
			{
				k = m;
			}
			if (y < k)
			{
				k = y;
			}
			c = (c - k) / (1f - k);
			m = (m - k) / (1f - k);
			y = (y - k) / (1f - k);
		}

		public Color GetColor()
		{
			return new Color(Red, Green, Blue, Alpha);
		}

		public static ColorF Lerp(ColorF a, ColorF b, float factor)
		{
			return new ColorF(a.Alpha * factor + b.Alpha * (1f - factor), a.Red * factor + b.Red * (1f - factor), a.Green * factor + b.Green * (1f - factor), a.Blue * factor + b.Blue * (1f - factor));
		}

		private ColorF(Color col)
		{
			_red = (float)(int)col.R / 255f;
			_green = (float)(int)col.G / 255f;
			_blue = (float)(int)col.B / 255f;
			_alpha = (float)(int)col.A / 255f;
		}

		private ColorF(float a, float r, float g, float b)
		{
			_red = r;
			_green = g;
			_blue = b;
			_alpha = a;
		}

		public override string ToString()
		{
			return Red + "," + Green + "," + Blue + "," + Alpha;
		}

		public static ColorF Parse(string strval)
		{
			string[] array = strval.Split(',');
			float r = float.Parse(array[0]);
			float g = float.Parse(array[1]);
			float b = float.Parse(array[2]);
			float a = float.Parse(array[3]);
			return FromARGB(a, r, g, b);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(ColorF))
			{
				return this == (ColorF)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(ColorF a, ColorF b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(ColorF a, ColorF b)
		{
			throw new NotImplementedException();
		}
	}
}
