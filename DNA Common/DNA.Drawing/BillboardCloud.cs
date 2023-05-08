using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class BillboardCloud : Entity
	{
		private BillBoardMode _billboardMode = BillBoardMode.AxisAligned;

		private List<BillboardVertex> billboardVerticies = new List<BillboardVertex>();

		private VertexBuffer _vertexBuffer;

		private IndexBuffer _indexBuffer;

		private Effect _cardEffect;

		private Texture _texture;

		public static Effect _effect;

		private Game _game;

		private bool _started;

		public BillboardCloud(Game game)
		{
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
			_vertexBuffer = new VertexBuffer(device, BillboardVertex.VertexDeclaration, cards * 4, BufferUsage.WriteOnly);
			_indexBuffer = new IndexBuffer(device, typeof(uint), cards * 6, BufferUsage.WriteOnly);
		}

		public void Clear()
		{
			if (!_started)
			{
				throw new Exception("Must Start Set to Clear");
			}
			billboardVerticies.Clear();
		}

		public void BeginSet(BillBoardMode mode)
		{
			if (_started)
			{
				throw new Exception("Batch Already Started");
			}
			_started = true;
			_billboardMode = mode;
		}

		public void EndSet(GraphicsDevice device)
		{
			if (!_started)
			{
				throw new Exception("End Before Start");
			}
			Initialize(device, billboardVerticies.Count / 4);
			SetData();
			_started = false;
		}

		private void SetData()
		{
			int num = billboardVerticies.Count / 4;
			uint[] array = new uint[num * 6];
			for (int i = 0; i < num; i++)
			{
				array[i * 6] = (uint)(i * 4);
				array[i * 6 + 1] = (uint)(i * 4 + 1);
				array[i * 6 + 2] = (uint)(i * 4 + 2);
				array[i * 6 + 3] = (uint)(i * 4);
				array[i * 6 + 4] = (uint)(i * 4 + 2);
				array[i * 6 + 5] = (uint)(i * 4 + 3);
			}
			_indexBuffer.SetData(array);
			_vertexBuffer.SetData(billboardVerticies.ToArray());
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			Draw(device, base.LocalToWorld, view, projection);
			base.Draw(device, gameTime, view, projection);
		}

		public void Draw(GraphicsDevice device, Matrix world, Matrix view, Matrix projection)
		{
			if (_started)
			{
				throw new Exception("Must finsih set before drawing");
			}
			EffectParameterCollection parameters = _cardEffect.Parameters;
			device.BlendState = BlendState.AlphaBlend;
			device.BlendState = BlendState.Opaque;
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
			int num = billboardVerticies.Count / 4;
			device.SetVertexBuffer(_vertexBuffer);
			device.Indices = _indexBuffer;
			for (int i = 0; i < _cardEffect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass effectPass = _cardEffect.CurrentTechnique.Passes[i];
				effectPass.Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, num * 4, 0, num * 2);
			}
			device.SetVertexBuffer(null);
		}

		public void DrawCard(Texture texture, Vector3 position, Vector3 axis, Vector2 scale, Color color)
		{
			_texture = texture;
			billboardVerticies.Add(new BillboardVertex(position, scale, axis, new Vector2(0f, 0f), color));
			billboardVerticies.Add(new BillboardVertex(position, scale, axis, new Vector2(1f, 0f), color));
			billboardVerticies.Add(new BillboardVertex(position, scale, axis, new Vector2(1f, 1f), color));
			billboardVerticies.Add(new BillboardVertex(position, scale, axis, new Vector2(0f, 1f), color));
		}
	}
}
