using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public struct VertexPositionNormalColor : IVertexType
	{
		public Vector3 Position;

		public Color Color;

		public Vector3 Normal;

		public Vector2 TextureCoord;

		public static VertexDeclaration s_VertexDeclaration;

		public static VertexElement[] VertexElements;

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return s_VertexDeclaration;
			}
		}

		static VertexPositionNormalColor()
		{
			VertexElements = new VertexElement[4]
			{
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			};
			s_VertexDeclaration = new VertexDeclaration(VertexElements);
		}
	}
}
