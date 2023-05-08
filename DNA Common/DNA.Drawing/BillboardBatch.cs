using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class BillboardBatch
	{
		private BillBoardMode _billboardMode = BillBoardMode.AxisAligned;

		private BillboardVertex[] billboardVerticies = new BillboardVertex[1024];

		private int cardCount;

		private DynamicVertexBuffer _vertexBuffer;

		private IndexBuffer _indexBuffer;

		private Effect _cardEffect;

		private Texture _texture;

		public static Effect _effect;

		private Game _game;

		private bool _started;

		public BillboardBatch(Game game)
		{
			_game = game;
			LoadParticleEffect();
		}

		public BillboardBatch(Game game, int startSize)
		{
			billboardVerticies = new BillboardVertex[startSize * 4];
			_game = game;
			LoadParticleEffect();
		}

		private void LoadParticleEffect()
		{
			if (_effect == null)
			{
				_effect = _game.Content.Load<Effect>("Billboard");
			}
			_cardEffect = _effect.Clone();
		}

		private void Initialize(GraphicsDevice device, int cards)
		{
			_vertexBuffer = new DynamicVertexBuffer(device, BillboardVertex.VertexDeclaration, cards * 4, BufferUsage.WriteOnly);
			uint[] array = new uint[cards * 6];
			for (int i = 0; i < cards; i++)
			{
				array[i * 6] = (uint)(i * 4);
				array[i * 6 + 1] = (uint)(i * 4 + 1);
				array[i * 6 + 2] = (uint)(i * 4 + 2);
				array[i * 6 + 3] = (uint)(i * 4);
				array[i * 6 + 4] = (uint)(i * 4 + 2);
				array[i * 6 + 5] = (uint)(i * 4 + 3);
			}
			_indexBuffer = new IndexBuffer(device, typeof(uint), array.Length, BufferUsage.WriteOnly);
			_indexBuffer.SetData(array);
		}

		public void Begin(BillBoardMode mode)
		{
			if (_started)
			{
				throw new Exception("Batch Already Started");
			}
			_started = true;
			_billboardMode = mode;
			cardCount = 0;
		}

		public void End(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
		{
			if (!_started)
			{
				throw new Exception("End Before Start");
			}
			if (_vertexBuffer == null)
			{
				Initialize(device, billboardVerticies.Length / 4);
			}
			Draw(device, world, view, projection);
			_started = false;
		}

		private void Draw(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
		{
			if (cardCount != 0)
			{
				EffectParameterCollection parameters = _cardEffect.Parameters;
				device.BlendState = BlendState.AlphaBlend;
				device.DepthStencilState = DepthStencilState.DepthRead;
				device.DepthStencilState = DepthStencilState.Default;
				device.RasterizerState = RasterizerState.CullNone;
				parameters["View"].SetValue(view);
				parameters["Projection"].SetValue(projection);
				parameters["ViewportScale"].SetValue(new Vector2(1f / device.Viewport.AspectRatio, 1f));
				parameters["Texture"].SetValue(_texture);
				switch (_billboardMode)
				{
				case BillBoardMode.AxisAligned:
					_cardEffect.CurrentTechnique = _cardEffect.Techniques["AxisAlignedBillboards"];
					break;
				case BillBoardMode.ScreenAligned:
					_cardEffect.CurrentTechnique = _cardEffect.Techniques["ScreenAlignedBillboards"];
					break;
				default:
					throw new Exception("Unknown Mode");
				}
				_vertexBuffer.SetData(billboardVerticies, 0, cardCount * 4);
				device.SetVertexBuffer(_vertexBuffer);
				device.Indices = _indexBuffer;
				for (int i = 0; i < _cardEffect.CurrentTechnique.Passes.Count; i++)
				{
					EffectPass effectPass = _cardEffect.CurrentTechnique.Passes[i];
					effectPass.Apply();
					device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cardCount * 4, 0, cardCount * 2);
				}
				device.SetVertexBuffer(null);
			}
		}

		private void GrowArray()
		{
			BillboardVertex[] destinationArray = new BillboardVertex[billboardVerticies.Length * 2];
			Array.Copy(billboardVerticies, destinationArray, cardCount * 4);
			billboardVerticies = destinationArray;
			if (_vertexBuffer != null)
			{
				_vertexBuffer.Dispose();
				_vertexBuffer = null;
			}
			if (_indexBuffer != null)
			{
				_indexBuffer.Dispose();
				_indexBuffer = null;
			}
		}

		public void DrawCard(Texture texture, Vector3 position, Vector3 axis, Vector2 scale, Color color)
		{
			_texture = texture;
			if ((cardCount + 1) * 4 > billboardVerticies.Length)
			{
				GrowArray();
			}
			int num = cardCount * 4;
			billboardVerticies[num] = new BillboardVertex(position, scale, axis, new Vector2(0f, 0f), color);
			billboardVerticies[num + 1] = new BillboardVertex(position, scale, axis, new Vector2(1f, 0f), color);
			billboardVerticies[num + 2] = new BillboardVertex(position, scale, axis, new Vector2(1f, 1f), color);
			billboardVerticies[num + 3] = new BillboardVertex(position, scale, axis, new Vector2(0f, 1f), color);
			cardCount++;
		}
	}
}
