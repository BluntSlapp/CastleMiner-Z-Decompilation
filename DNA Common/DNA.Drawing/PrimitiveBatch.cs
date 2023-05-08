using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class PrimitiveBatch : IDisposable
	{
		private const int DefaultBufferSize = 500;

		private VertexPositionNormalColor[] vertices = new VertexPositionNormalColor[500];

		private int positionInBuffer;

		private BasicEffect basicEffect;

		private GraphicsDevice device;

		private PrimitiveType primitiveType;

		private int numVertsPerPrimitive;

		private bool hasBegun;

		private bool isDisposed;

		public PrimitiveBatch(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
			{
				throw new ArgumentNullException("graphicsDevice");
			}
			device = graphicsDevice;
			basicEffect = new BasicEffect(graphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.EnableDefaultLighting();
			basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0f, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0f, 0f, 1f);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !isDisposed)
			{
				if (basicEffect != null)
				{
					basicEffect.Dispose();
				}
				isDisposed = true;
			}
		}

		public void Begin(PrimitiveType primitiveType, Matrix world, Matrix view, Matrix projection)
		{
			basicEffect.World = world;
			basicEffect.View = view;
			basicEffect.Projection = projection;
			if (hasBegun)
			{
				throw new InvalidOperationException("End must be called before Begin can be called again.");
			}
			if (primitiveType == PrimitiveType.LineStrip || primitiveType == PrimitiveType.TriangleStrip)
			{
				throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");
			}
			this.primitiveType = primitiveType;
			numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);
			basicEffect.CurrentTechnique.Passes[0].Apply();
			hasBegun = true;
		}

		public void AddVertex(Vector3 vertex, Vector3 normal, Color color)
		{
			if (!hasBegun)
			{
				throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
			}
			if (positionInBuffer % numVertsPerPrimitive == 0 && positionInBuffer + numVertsPerPrimitive >= vertices.Length)
			{
				Flush();
			}
			vertices[positionInBuffer].Position = vertex;
			vertices[positionInBuffer].Normal = normal;
			vertices[positionInBuffer].Color = color;
			vertices[positionInBuffer].TextureCoord = new Vector2(0f, 0f);
			positionInBuffer++;
		}

		public void End()
		{
			if (!hasBegun)
			{
				throw new InvalidOperationException("Begin must be called before End can be called.");
			}
			Flush();
			hasBegun = false;
		}

		private void Flush()
		{
			if (!hasBegun)
			{
				throw new InvalidOperationException("Begin must be called before Flush can be called.");
			}
			if (positionInBuffer != 0)
			{
				int primitiveCount = positionInBuffer / numVertsPerPrimitive;
				device.DrawUserPrimitives(primitiveType, vertices, 0, primitiveCount);
				positionInBuffer = 0;
			}
		}

		private static int NumVertsPerPrimitive(PrimitiveType primitive)
		{
			switch (primitive)
			{
			case PrimitiveType.LineList:
				return 2;
			case PrimitiveType.TriangleList:
				return 3;
			default:
				throw new InvalidOperationException("primitive is not valid");
			}
		}
	}
}
