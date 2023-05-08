using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace DNA.Drawing.Particles
{
	internal struct ParticleVertex
	{
		public const int SizeInBytes = 36;

		public Short2 Corner;

		public Vector3 Position;

		public Vector3 Velocity;

		public Color Random;

		public float Time;

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Short2, VertexElementUsage.Position, 0), new VertexElement(4, VertexElementFormat.Vector3, VertexElementUsage.Position, 1), new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(28, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0));
	}
}
