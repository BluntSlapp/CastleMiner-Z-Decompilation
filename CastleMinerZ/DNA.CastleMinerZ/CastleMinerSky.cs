using System;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class CastleMinerSky : SkySphere
	{
		private static Effect _effect;

		private static TextureCube _dayTexture;

		private static TextureCube _nightTexture;

		private static TextureCube _sunSetTexture;

		private static TextureCube _dawnTexture;

		public bool drawLightning;

		public float Day = 0.41f;

		private float rot;

		public float TimeOfDay
		{
			get
			{
				return Day - (float)Math.Floor(Day);
			}
		}

		static CastleMinerSky()
		{
			_dayTexture = CastleMinerZGame.Instance.Content.Load<TextureCube>("Textures\\Skys\\ClearSky");
			_nightTexture = CastleMinerZGame.Instance.Content.Load<TextureCube>("Textures\\Skys\\NightSky");
			_sunSetTexture = CastleMinerZGame.Instance.Content.Load<TextureCube>("Textures\\Skys\\SunSet");
			_dawnTexture = CastleMinerZGame.Instance.Content.Load<TextureCube>("Textures\\Skys\\DawnSky");
			_effect = CastleMinerZGame.Instance.Content.Load<Effect>("TextureSky");
		}

		public CastleMinerSky()
			: base(CastleMinerZGame.Instance.GraphicsDevice, 500f, Vector3.Zero, 20, _effect, _dayTexture)
		{
			DrawPriority = -1000;
		}

		public void SetParameters()
		{
			float num = TimeOfDay * 24f;
			float value = num - (float)(int)num;
			int num2 = (int)num;
			_effect.Parameters["Blender"].SetValue(value);
			switch (num2)
			{
			default:
				_effect.Parameters["Sky1Texture"].SetValue(_nightTexture);
				_effect.Parameters["Sky2Texture"].SetValue(_nightTexture);
				break;
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
				_effect.Parameters["Sky1Texture"].SetValue(_dayTexture);
				_effect.Parameters["Sky2Texture"].SetValue(_dayTexture);
				break;
			case 6:
			case 7:
			case 8:
			case 18:
			case 19:
			case 20:
				switch (num2)
				{
				case 6:
					_effect.Parameters["Sky1Texture"].SetValue(_nightTexture);
					_effect.Parameters["Sky2Texture"].SetValue(_dawnTexture);
					break;
				case 7:
					_effect.Parameters["Sky1Texture"].SetValue(_dawnTexture);
					_effect.Parameters["Sky2Texture"].SetValue(_dawnTexture);
					break;
				case 8:
					_effect.Parameters["Sky1Texture"].SetValue(_dawnTexture);
					_effect.Parameters["Sky2Texture"].SetValue(_dayTexture);
					break;
				case 18:
					_effect.Parameters["Sky1Texture"].SetValue(_dayTexture);
					_effect.Parameters["Sky2Texture"].SetValue(_sunSetTexture);
					break;
				case 19:
					_effect.Parameters["Sky1Texture"].SetValue(_sunSetTexture);
					_effect.Parameters["Sky2Texture"].SetValue(_sunSetTexture);
					break;
				case 20:
					_effect.Parameters["Sky1Texture"].SetValue(_sunSetTexture);
					_effect.Parameters["Sky2Texture"].SetValue(_nightTexture);
					break;
				}
				break;
			}
			Vector3 value2 = Vector3.Zero;
			float num3 = (CastleMinerZGame.Instance.LocalPlayer.LocalPosition.Y + 32f) / 8f;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			num3 = 1f - num3;
			if (drawLightning)
			{
				value2 = new Vector3(1f, 1f, 1f);
				num3 = 1f;
			}
			_effect.Parameters["LerpColor"].SetValue(value2);
			_effect.Parameters["LerpAmount"].SetValue(num3);
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (CastleMinerZGame.Instance.DrawingReflection && BlockTerrain.Instance.EyePos.Y >= BlockTerrain.Instance.WaterLevel)
			{
				DrawReflection(device, gameTime, view, projection);
				return;
			}
			SetParameters();
			float num = TimeOfDay * 24f;
			int num2 = (int)num;
			if (num2 > 5 && num2 < 20)
			{
				rot += (float)gameTime.get_ElapsedGameTime().TotalSeconds / 80f;
			}
			_effect.Parameters["CloudOffset"].SetValue(Matrix.CreateRotationY(rot));
			_effect.Parameters["Offset"].SetValue(new Vector3(0f, -100f, 0f));
			base.Draw(device, gameTime, view, projection);
		}

		public void DrawReflection(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			SetParameters();
			float y = BlockTerrain.Instance.EyePos.Y - BlockTerrain.Instance.WaterLevel - 100f;
			_effect.Parameters["Offset"].SetValue(new Vector3(0f, y, 0f));
			device.RasterizerState = RasterizerState.CullClockwise;
			base.Draw(device, gameTime, view, projection);
			device.RasterizerState = RasterizerState.CullCounterClockwise;
		}
	}
}
