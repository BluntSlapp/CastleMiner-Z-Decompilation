using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Profiling
{
	public class ProfilerPrimitiveBatch : IDisposable
	{
		private const int DefaultBufferSize = 500;

		private VertexPositionColor[] vertices = new VertexPositionColor[500];

		private int positionInBuffer;

		private BasicEffect basicEffect;

		private GraphicsDevice device;

		private PrimitiveType primitiveType;

		private int numVertsPerPrimitive;

		private bool hasBegun;

		private bool isDisposed;

		public ProfilerPrimitiveBatch(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
			{
				throw new ArgumentNullException("graphicsDevice");
			}
			device = graphicsDevice;
			basicEffect = new BasicEffect(graphicsDevice);
			basicEffect.VertexColorEnabled = true;
			basicEffect.Projection = ProfilerUtils._standard2DProjection;
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

		public void Begin(PrimitiveType primitiveType)
		{
			Begin(primitiveType, Matrix.Identity);
		}

		public void Begin(PrimitiveType primitiveType, Matrix mat)
		{
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
			basicEffect.Projection = ProfilerUtils._standard2DProjection;
			basicEffect.View = mat;
			basicEffect.CurrentTechnique.Passes[0].Apply();
			hasBegun = true;
		}

		public void AddVertex(Vector2 vertex, Color color)
		{
			if (!hasBegun)
			{
				throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
			}
			if (positionInBuffer % numVertsPerPrimitive == 0 && positionInBuffer + numVertsPerPrimitive >= vertices.Length)
			{
				Flush();
			}
			vertices[positionInBuffer].Position = new Vector3(vertex, 0f);
			vertices[positionInBuffer].Color = color;
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
				device.DrawUserPrimitives<VertexPositionColor>(primitiveType, vertices, 0, primitiveCount);
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

		public void AddFilledBox(Vector2 coord, Vector2 size, Color color, bool centered)
		{
			AddFilledBox(ref coord, ref size, color, centered);
		}

		public void AddFilledBox(ref Vector2 coord, ref Vector2 size, Color color, bool centered)
		{
			Vector2 vector = ((!centered) ? coord : (coord - size * 0.5f));
			Vector2 vector2 = vector + size;
			if (hasBegun && primitiveType != 0)
			{
				End();
			}
			if (!hasBegun)
			{
				Begin(PrimitiveType.TriangleList, basicEffect.View);
			}
			AddVertex(new Vector2(vector.X, vector.Y), color);
			AddVertex(new Vector2(vector2.X, vector.Y), color);
			AddVertex(new Vector2(vector.X, vector2.Y), color);
			AddVertex(new Vector2(vector.X, vector2.Y), color);
			AddVertex(new Vector2(vector2.X, vector.Y), color);
			AddVertex(new Vector2(vector2.X, vector2.Y), color);
		}

		public void AddLine(Vector2[] vectors, Color color, bool close)
		{
			if (hasBegun && primitiveType != PrimitiveType.LineList)
			{
				End();
			}
			if (!hasBegun)
			{
				Begin(PrimitiveType.LineList, basicEffect.View);
			}
			for (int i = 0; i < vectors.Length - 1; i++)
			{
				AddVertex(vectors[i], color);
				AddVertex(vectors[i + 1], color);
			}
			if (close)
			{
				AddVertex(vectors[vectors.Length - 1], color);
				AddVertex(vectors[0], color);
			}
		}

		public void DrawGraph(float[] values, int startIndex, Vector2 scale, Vector2 upperLeft, Vector2 size, Color color)
		{
			if (hasBegun && primitiveType != PrimitiveType.LineList)
			{
				End();
			}
			if (!hasBegun)
			{
				Begin(PrimitiveType.LineList, basicEffect.View);
			}
			int num = startIndex;
			float num2 = size.X / ((float)values.Length - 1f);
			Vector2 vertex = Vector2.Zero;
			Vector2 vector = new Vector2(upperLeft.X, 0f);
			for (int i = 0; i < values.Length; i++)
			{
				vector.X += num2;
				if (values[num] < scale.X)
				{
					vector.Y = upperLeft.Y + size.Y;
				}
				else if (values[num] > scale.Y)
				{
					vector.Y = upperLeft.Y;
				}
				else
				{
					vector.Y = upperLeft.Y + size.Y * (1f - (values[num] - scale.X) / (scale.Y - scale.X));
				}
				if (i != 0)
				{
					AddVertex(vertex, color);
					AddVertex(vector, color);
				}
				vertex = vector;
				if (++num == values.Length)
				{
					num = 0;
				}
			}
		}

		public void DrawGraphBar(float value, Vector2 scale, Vector2 upperLeft, Vector2 size, Color color)
		{
			if (!(value < scale.X) && !(value > scale.Y))
			{
				if (hasBegun && primitiveType != PrimitiveType.LineList)
				{
					End();
				}
				if (!hasBegun)
				{
					Begin(PrimitiveType.LineList, basicEffect.View);
				}
				Vector2 vertex = new Vector2(upperLeft.X, 0f);
				Vector2 vertex2 = new Vector2(upperLeft.X + size.X, 0f);
				vertex.Y = upperLeft.Y + size.Y * (1f - (value - scale.X) / (scale.Y - scale.X));
				vertex2.Y = vertex.Y;
				AddVertex(vertex, color);
				AddVertex(vertex2, color);
			}
		}

		public void DrawGraphVerticalAxis(Vector2 upperLeft, Vector2 size, Color color)
		{
			if (hasBegun && primitiveType != PrimitiveType.LineList)
			{
				End();
			}
			if (!hasBegun)
			{
				Begin(PrimitiveType.LineList, basicEffect.View);
			}
			AddVertex(upperLeft, color);
			AddVertex(new Vector2(upperLeft.X, upperLeft.Y + size.Y), color);
		}
	}
}
