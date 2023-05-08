using System.Collections.Generic;
using DNA.CastleMinerZ.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Terrain
{
	public class BlockBuildData : IReleaseable, ILinkedListNode
	{
		private const int NUM_VERTS = 7000;

		public float[] _sun = new float[9];

		public float[] _torch = new float[9];

		public int[] _vxsun = new int[4];

		public int[] _vxtorch = new int[4];

		public IntVector3 _min = default(IntVector3);

		public IntVector3 _max = default(IntVector3);

		public BlockVertex[] _vxList = new BlockVertex[7000];

		private int _vxBufferSize = 7000;

		private int _vxCount;

		public BlockVertex[] _fancyVXList = new BlockVertex[7000];

		private int _fancyVXBufferSize = 7000;

		private int _fancyVXCount;

		private static ObjectCache<BlockBuildData> _cache = new ObjectCache<BlockBuildData>();

		private ILinkedListNode _nextNode;

		public ILinkedListNode NextNode
		{
			get
			{
				return _nextNode;
			}
			set
			{
				_nextNode = value;
			}
		}

		public void AddVertex(BlockVertex bv, bool fancy)
		{
			if (fancy)
			{
				if (_fancyVXCount == _fancyVXBufferSize)
				{
					BlockVertex[] array = new BlockVertex[_fancyVXBufferSize + 100];
					_fancyVXList.CopyTo(array, 0);
					_fancyVXList = array;
					_fancyVXBufferSize += 100;
				}
				_fancyVXList[_fancyVXCount++] = bv;
			}
			else
			{
				if (_vxCount == _vxBufferSize)
				{
					BlockVertex[] array2 = new BlockVertex[_vxBufferSize + 100];
					_vxList.CopyTo(array2, 0);
					_vxList = array2;
					_vxBufferSize += 100;
				}
				_vxList[_vxCount++] = bv;
			}
		}

		public void BuildVBs(GraphicsDevice gd, ref List<VertexBuffer> vbs, ref List<VertexBuffer> fancy)
		{
			if (_vxCount > 0)
			{
				int num = 0;
				int num2 = _vxCount;
				int num3 = 0;
				while (num2 != 0)
				{
					int num4 = ((num2 > 16384) ? 16384 : num2);
					num2 -= num4;
					if (gd.IsDisposed)
					{
						return;
					}
					VertexBuffer vertexBuffer = new VertexBuffer(gd, typeof(BlockVertex), num4, BufferUsage.WriteOnly);
					vertexBuffer.SetData(_vxList, num, num4);
					num += num4;
					vbs.Add(vertexBuffer);
					num3++;
				}
			}
			if (_fancyVXCount <= 0)
			{
				return;
			}
			int num5 = 0;
			int num6 = _fancyVXCount;
			int num7 = 0;
			while (num6 != 0)
			{
				int num8 = ((num6 > 16384) ? 16384 : num6);
				num6 -= num8;
				if (gd.IsDisposed)
				{
					break;
				}
				VertexBuffer vertexBuffer2 = new VertexBuffer(gd, typeof(BlockVertex), num8, BufferUsage.WriteOnly);
				vertexBuffer2.SetData(_fancyVXList, num5, num8);
				num5 += num8;
				fancy.Add(vertexBuffer2);
				num7++;
			}
		}

		public static BlockBuildData Alloc()
		{
			BlockBuildData blockBuildData = _cache.Get();
			blockBuildData._min.SetValues(int.MaxValue, int.MaxValue, int.MaxValue);
			blockBuildData._max.SetValues(int.MinValue, int.MinValue, int.MinValue);
			blockBuildData._vxCount = 0;
			blockBuildData._fancyVXCount = 0;
			return blockBuildData;
		}

		public void Release()
		{
			_cache.Put(this);
		}
	}
}
