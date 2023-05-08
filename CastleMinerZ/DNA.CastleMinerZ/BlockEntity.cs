using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class BlockEntity : Entity
	{
		private struct BlockEntityVertex : IVertexType
		{
			private int _fvxFaceTxCvx;

			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 0));

			VertexDeclaration IVertexType.VertexDeclaration
			{
				get
				{
					return VertexDeclaration;
				}
			}

			public BlockEntityVertex(BlockFace face, int cvx, int fvx, int tx)
			{
				_fvxFaceTxCvx = fvx | ((int)face << 8) | (tx << 16) | (cvx << 24);
			}
		}

		private static Effect _effect;

		private static VertexBuffer _vb;

		private static float[] _materialTx = new float[6];

		private BlockTypeEnum _blockType;

		public float Scale;

		public float SunLight = 255f;

		public float TorchLight;

		public bool _uiObject;

		public bool AttachedToLocalPlayer = true;

		public static void Initialize()
		{
			CastleMinerZGame instance = CastleMinerZGame.Instance;
			_effect = BlockTerrain.Instance._effect;
			_vb = new DynamicVertexBuffer(instance.GraphicsDevice, typeof(BlockEntityVertex), 24, BufferUsage.WriteOnly);
			BlockEntityVertex[] array = new BlockEntityVertex[24];
			int[] array2 = new int[6] { 0, 0, 0, 0, 1, 2 };
			IntVector3 intVector = new IntVector3(1, 2, 4);
			for (BlockFace blockFace = BlockFace.POSX; blockFace < BlockFace.NUM_FACES; blockFace++)
			{
				int num = (int)blockFace * 4;
				int num3 = array2[(int)blockFace];
				int num2 = 0;
				while (num2 < 4)
				{
					array[(int)blockFace * 4 + num2] = new BlockEntityVertex(blockFace, intVector.Dot(BlockVertex._faceVertices[num]), num2, (int)blockFace);
					num2++;
					num++;
				}
			}
			_vb.SetData(array, 0, array.Length);
		}

		public BlockEntity(BlockTypeEnum blockType, bool attachedToLocalPlayer)
		{
			_blockType = blockType;
			AttachedToLocalPlayer = attachedToLocalPlayer;
			if (attachedToLocalPlayer)
			{
				DrawPriority = 602;
			}
			else if (BlockType.GetType(_blockType).NeedsFancyLighting)
			{
				DrawPriority = 601;
			}
			else
			{
				DrawPriority = 600;
			}
		}

		public static void InitUIRendering(Matrix projection)
		{
			_effect.Parameters["Projection"].SetValue(projection);
			_effect.Parameters["View"].SetValue(Matrix.Identity);
			_effect.Parameters["WaterDepth"].SetValue(10000);
			_effect.Parameters["WaterLevel"].SetValue(-10000);
			Vector3 value = default(Vector3);
			value.X = 0f;
			value.Y = 1f;
			value.Z = -1000f;
			_effect.Parameters["EyeWaterConstants"].SetValue(value);
			_effect.Parameters["LightDirection"].SetValue(Vector3.Backward);
			_effect.Parameters["TorchLight"].SetValue(new Color(255, 235, 190).ToVector3());
			_effect.Parameters["SunLight"].SetValue(Vector3.One);
			_effect.Parameters["AmbientSun"].SetValue(new Vector3(0.4f, 0.4f, 0.4f));
			_effect.Parameters["SunSpecular"].SetValue(Vector3.One);
			_effect.Parameters["FogColor"].SetValue(Vector3.One);
			_effect.Parameters["BelowWaterColor"].SetValue(Vector3.Zero);
		}

		public void UIObject()
		{
			_uiObject = true;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (!_uiObject && BlockTerrain.Instance != null && BlockTerrain.Instance.IsReady)
			{
				Vector3 worldPosition = base.WorldPosition;
				if (AttachedToLocalPlayer)
				{
					worldPosition = CastleMinerZGame.Instance.LocalPlayer.FPSCamera.WorldPosition;
				}
				Vector2 lightAtPoint = BlockTerrain.Instance.GetLightAtPoint(worldPosition);
				SunLight = lightAtPoint.X * 255f;
				TorchLight = lightAtPoint.Y * 255f;
			}
			base.OnUpdate(gameTime);
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			BlockType type = BlockType.GetType(_blockType);
			Matrix localToWorld = base.LocalToWorld;
			_effect.Parameters["World"].SetValue(localToWorld);
			_effect.Parameters["View"].SetValue(view);
			_effect.Parameters["Projection"].SetValue(projection);
			_effect.Parameters["InverseWorld"].SetValue(localToWorld.QuickInvert());
			_effect.Parameters["CubeScaleSunTorch"].SetValue(new Vector3(Scale, SunLight, TorchLight));
			_effect.Parameters["CubeTx"].SetValue(type.TileIndices);
			GraphicsDevice graphicsDevice = CastleMinerZGame.Instance.GraphicsDevice;
			graphicsDevice.SetVertexBuffer(_vb);
			graphicsDevice.Indices = BlockTerrain.Instance._staticIB;
			if (_uiObject)
			{
				Vector3 translation = localToWorld.Translation;
				translation.Z = 0f;
				_effect.Parameters["EyePosition"].SetValue(translation);
				_effect.CurrentTechnique = _effect.Techniques[5];
				_effect.CurrentTechnique.Passes[type.NeedsFancyLighting ? 3 : 2].Apply();
			}
			else if (CastleMinerZGame.Instance.DrawingReflection)
			{
				_effect.CurrentTechnique = _effect.Techniques[3];
				_effect.CurrentTechnique.Passes[2].Apply();
			}
			else
			{
				_effect.CurrentTechnique = _effect.Techniques[BlockTerrain.Instance.IsWaterWorld ? 3 : 5];
				_effect.CurrentTechnique.Passes[type.NeedsFancyLighting ? 1 : 0].Apply();
			}
			graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
		}
	}
}
