using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Terrain
{
	public class WaterPlane : Entity
	{
		private struct PositionVX : IVertexType
		{
			public Vector3 Position;

			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));

			VertexDeclaration IVertexType.VertexDeclaration
			{
				get
				{
					return VertexDeclaration;
				}
			}

			public PositionVX(Vector3 pos)
			{
				Position = pos;
			}
		}

		private VertexBuffer _waterVerts;

		private VertexBuffer _wellVerts;

		public RenderTarget2D _reflectionTexture;

		public Effect _effect;

		public Texture2D _normalMap;

		public static WaterPlane Instance;

		public WaterPlane(GraphicsDevice gd, ContentManager cm)
		{
			Instance = this;
			PositionVX[] array = new PositionVX[30];
			float x = 384f;
			float z = 384f;
			float y = -128f;
			array[0] = new PositionVX(new Vector3(x, 0f, 0f));
			array[1] = new PositionVX(new Vector3(x, 0f, z));
			array[2] = new PositionVX(new Vector3(0f, 0f, 0f));
			array[3] = new PositionVX(new Vector3(0f, 0f, 0f));
			array[4] = new PositionVX(new Vector3(x, 0f, z));
			array[5] = new PositionVX(new Vector3(0f, 0f, z));
			_waterVerts = new VertexBuffer(gd, typeof(PositionVX), 6, BufferUsage.WriteOnly);
			_waterVerts.SetData(array, 0, 6);
			array[0] = new PositionVX(new Vector3(x, 0f, 0f));
			array[1] = new PositionVX(new Vector3(x, y, 0f));
			array[2] = new PositionVX(new Vector3(0f, 0f, 0f));
			array[3] = new PositionVX(new Vector3(0f, 0f, 0f));
			array[4] = new PositionVX(new Vector3(x, y, 0f));
			array[5] = new PositionVX(new Vector3(0f, y, 0f));
			array[6] = new PositionVX(new Vector3(0f, 0f, z));
			array[7] = new PositionVX(new Vector3(0f, y, z));
			array[8] = new PositionVX(new Vector3(x, 0f, z));
			array[9] = new PositionVX(new Vector3(x, 0f, z));
			array[10] = new PositionVX(new Vector3(0f, y, z));
			array[11] = new PositionVX(new Vector3(x, y, z));
			array[12] = new PositionVX(new Vector3(x, 0f, z));
			array[13] = new PositionVX(new Vector3(x, y, z));
			array[14] = new PositionVX(new Vector3(x, 0f, 0f));
			array[15] = new PositionVX(new Vector3(x, 0f, 0f));
			array[16] = new PositionVX(new Vector3(x, y, z));
			array[17] = new PositionVX(new Vector3(x, y, 0f));
			array[18] = new PositionVX(new Vector3(0f, 0f, 0f));
			array[19] = new PositionVX(new Vector3(0f, y, 0f));
			array[20] = new PositionVX(new Vector3(0f, 0f, z));
			array[21] = new PositionVX(new Vector3(0f, 0f, z));
			array[22] = new PositionVX(new Vector3(0f, y, 0f));
			array[23] = new PositionVX(new Vector3(0f, y, z));
			array[24] = new PositionVX(new Vector3(x, y, 0f));
			array[25] = new PositionVX(new Vector3(x, y, z));
			array[26] = new PositionVX(new Vector3(0f, y, 0f));
			array[27] = new PositionVX(new Vector3(0f, y, 0f));
			array[28] = new PositionVX(new Vector3(x, y, z));
			array[29] = new PositionVX(new Vector3(0f, y, z));
			_wellVerts = new VertexBuffer(gd, typeof(PositionVX), array.Length, BufferUsage.WriteOnly);
			_wellVerts.SetData(array, 0, array.Length);
			_reflectionTexture = new RenderTarget2D(CastleMinerZGame.Instance.GraphicsDevice, 1280, 720, true, SurfaceFormat.Color, DepthFormat.Depth16);
			_effect = cm.Load<Effect>("Effects\\WaterEffect");
			_normalMap = cm.Load<Texture2D>("Terrain\\water_normalmap");
			_effect.Parameters["NormalTexture"].SetValue(_normalMap);
			Collidee = false;
			DrawPriority = 900;
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (!BlockTerrain.Instance.IsWaterWorld)
			{
				return;
			}
			if (CastleMinerZGame.Instance.DrawingReflection)
			{
				_effect.Parameters["Reflection"].SetValue(view);
				return;
			}
			_effect.Parameters["Projection"].SetValue(projection);
			_effect.Parameters["View"].SetValue(view);
			_effect.Parameters["Time"].SetValue((float)gameTime.get_TotalGameTime().TotalSeconds);
			_effect.Parameters["LightDirection"].SetValue(BlockTerrain.Instance.VectorToSun);
			Vector2 lightAtPoint = BlockTerrain.Instance.GetLightAtPoint(BlockTerrain.Instance.EyePos);
			_effect.Parameters["SunLightColor"].SetValue(BlockTerrain.Instance.SunSpecular.ToVector3() * (float)Math.Pow(lightAtPoint.X, 10.0));
			_effect.Parameters["TorchLightColor"].SetValue(BlockTerrain.Instance.TorchColor.ToVector3() * lightAtPoint.Y);
			Matrix identity = Matrix.Identity;
			Vector3 translation = IntVector3.ToVector3(BlockTerrain.Instance._worldMin);
			translation.Y = BlockTerrain.Instance.WaterLevel;
			identity.Translation = translation;
			_effect.Parameters["World"].SetValue(identity);
			Vector3 eyePo = BlockTerrain.Instance.EyePos;
			_effect.Parameters["EyePos"].SetValue(BlockTerrain.Instance.EyePos);
			_effect.Parameters["ReflectionTexture"].SetValue(_reflectionTexture);
			_effect.Parameters["WaterColor"].SetValue(BlockTerrain.Instance.GetActualWaterColor());
			BlendState blendState = device.BlendState;
			RasterizerState rasterizerState = device.RasterizerState;
			if (BlockTerrain.Instance.EyePos.Y >= BlockTerrain.Instance.WaterLevel)
			{
				_effect.CurrentTechnique = _effect.Techniques[0];
				device.BlendState = BlendState.AlphaBlend;
			}
			else
			{
				_effect.CurrentTechnique = _effect.Techniques[1];
				device.BlendState = BlendState.Opaque;
			}
			device.DepthStencilState = DepthStencilState.DepthRead;
			device.SetVertexBuffer(_wellVerts);
			_effect.CurrentTechnique.Passes[1].Apply();
			device.DrawPrimitives(PrimitiveType.TriangleList, 0, 10);
			device.SetVertexBuffer(_waterVerts);
			_effect.CurrentTechnique.Passes[0].Apply();
			device.RasterizerState = RasterizerState.CullNone;
			device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
			device.DepthStencilState = DepthStencilState.Default;
			device.BlendState = blendState;
			device.RasterizerState = rasterizerState;
			base.Draw(device, gameTime, view, projection);
		}
	}
}
