using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Terrain
{
	public struct BlockVertex : IVertexType
	{
		public static readonly IntVector3[] _faceVertices = new IntVector3[24]
		{
			new IntVector3(1, 1, 1),
			new IntVector3(1, 1, 0),
			new IntVector3(1, 0, 1),
			new IntVector3(1, 0, 0),
			new IntVector3(1, 1, 0),
			new IntVector3(0, 1, 0),
			new IntVector3(1, 0, 0),
			new IntVector3(0, 0, 0),
			new IntVector3(0, 1, 0),
			new IntVector3(0, 1, 1),
			new IntVector3(0, 0, 0),
			new IntVector3(0, 0, 1),
			new IntVector3(0, 1, 1),
			new IntVector3(1, 1, 1),
			new IntVector3(0, 0, 1),
			new IntVector3(1, 0, 1),
			new IntVector3(0, 1, 0),
			new IntVector3(1, 1, 0),
			new IntVector3(0, 1, 1),
			new IntVector3(1, 1, 1),
			new IntVector3(1, 0, 0),
			new IntVector3(0, 0, 0),
			new IntVector3(1, 0, 1),
			new IntVector3(0, 0, 1)
		};

		private int _blockOffsetFace;

		private int _vxSunLampFace;

		public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 0), new VertexElement(4, VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 1));

		VertexDeclaration IVertexType.VertexDeclaration
		{
			get
			{
				return VertexDeclaration;
			}
		}

		public BlockVertex(BlockFace face, int vx, int tx)
		{
			IntVector3 intVector = _faceVertices[(int)face * 4 + vx];
			_blockOffsetFace = intVector.X | (intVector.Y << 8) | (intVector.Z << 16) | (tx << 24);
			_vxSunLampFace = vx | 0xF00 | 0xF0000 | 0x10000000;
		}

		public BlockVertex(IntVector3 iv, BlockFace face, int vx, BlockType mat, int sun, int lamp, int aoindex)
		{
			IntVector3 intVector = IntVector3.Add(iv, _faceVertices[(int)face * 4 + vx]);
			_blockOffsetFace = intVector.X | (intVector.Y << 8) | (intVector.Z << 16) | (mat[face] << 24);
			if (mat.DrawFullBright)
			{
				_vxSunLampFace = 0xF00 | (lamp << 16) | (int)(((uint)face | (uint)(vx << 4)) + 128 << 24);
			}
			else
			{
				_vxSunLampFace = aoindex | (sun << 8) | (lamp << 16) | (int)(((uint)face | (uint)(vx << 4)) << 24);
			}
		}
	}
}
