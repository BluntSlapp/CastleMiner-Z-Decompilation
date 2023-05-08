using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public static class ControllerImages
	{
		public static Texture2D X;

		public static Texture2D Y;

		public static Texture2D A;

		public static Texture2D B;

		public static Texture2D Back;

		public static Texture2D Guide;

		public static Texture2D Start;

		public static Texture2D DPad;

		public static Texture2D LeftShoulder;

		public static Texture2D LeftThumstick;

		public static Texture2D LeftTrigger;

		public static Texture2D RightShoulder;

		public static Texture2D RightThumstick;

		public static Texture2D RightTrigger;

		public static void Load(ContentManager content)
		{
			X = content.Load<Texture2D>("ControllerGlyphs\\ButtonX");
			Y = content.Load<Texture2D>("ControllerGlyphs\\ButtonY");
			A = content.Load<Texture2D>("ControllerGlyphs\\ButtonA");
			B = content.Load<Texture2D>("ControllerGlyphs\\ButtonB");
			Back = content.Load<Texture2D>("ControllerGlyphs\\ButtonBack");
			Guide = content.Load<Texture2D>("ControllerGlyphs\\ButtonGuide");
			Start = content.Load<Texture2D>("ControllerGlyphs\\ButtonStart");
			DPad = content.Load<Texture2D>("ControllerGlyphs\\DPad");
			LeftShoulder = content.Load<Texture2D>("ControllerGlyphs\\LeftShoulder");
			LeftThumstick = content.Load<Texture2D>("ControllerGlyphs\\LeftThumbstick");
			LeftTrigger = content.Load<Texture2D>("ControllerGlyphs\\LeftTrigger");
			RightShoulder = content.Load<Texture2D>("ControllerGlyphs\\RightShoulder");
			RightThumstick = content.Load<Texture2D>("ControllerGlyphs\\RightThumbstick");
			RightTrigger = content.Load<Texture2D>("ControllerGlyphs\\RightTrigger");
		}

		static ControllerImages()
		{
		}
	}
}
