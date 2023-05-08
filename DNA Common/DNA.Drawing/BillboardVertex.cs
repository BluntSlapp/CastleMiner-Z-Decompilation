using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public struct BillboardVertex
	{
		public const int SizeInBytes = 44;

		public Vector3 Position;

		public Vector2 Scale;

		public Vector3 Axis;

		public Vector2 TexCoord;

		public Color Color;

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.Position, 1), new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(32, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), new VertexElement(40, VertexElementFormat.Color, VertexElementUsage.Color, 0));

		public BillboardVertex(Vector3 pos, Vector2 scale, Vector3 alignAxis, Vector2 texCoord, Color col)
		{
			Position = pos;
			Scale = scale;
			TexCoord = texCoord;
			Axis = alignAxis;
			Color = col;
		}
	}
}
