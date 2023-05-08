using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DNA.CastleMinerZ.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Terrain
{
	public class RenderChunk : IReleaseable, ILinkedListNode
	{
		private static readonly int[] _lightNeighborIndexes = new int[16]
		{
			4, 1, 3, 0, 4, 1, 5, 2, 4, 3,
			7, 6, 4, 5, 7, 8
		};

		public Vector3 _basePosition;

		public IntVector3 _worldMin;

		public Vector3[] _boundingCorners = new Vector3[8];

		public int _refcount = 1;

		public List<VertexBuffer> _vbs;

		public List<VertexBuffer> _fancyVBs;

		private static readonly float[] faceBrightness = new float[10] { 0.92f, 0.8367f, 0.92f, 0.8367f, 1f, 0.7071f, 0.92f, 0.8367f, 0.92f, 0.8367f };

		private static readonly float[] AOFACTOR = new float[11]
		{
			0.4f, 0.4f, 0.4f, 0.4f, 0.3f, 0.4f, 0f, 0f, 0f, 0f,
			0f
		};

		private static ObjectCache<RenderChunk> _cache = new ObjectCache<RenderChunk>();

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

		public int AddRef()
		{
			return Interlocked.Increment(ref _refcount);
		}

		public void FillFaceColors(ref int[] vxsun, ref int[] vxtorch, ref float[] sun, ref float[] torch)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				int num5 = 0;
				for (int j = 0; j < 4; j++)
				{
					int num6 = _lightNeighborIndexes[num++];
					if (j != 3 || num5 != 2)
					{
						if (sun[num6] >= 0f)
						{
							num2 += 1f;
							num3 += sun[num6];
							num4 += torch[num6];
						}
					}
					else
					{
						num5++;
					}
				}
				if (num2 == 0f)
				{
					vxsun[i] = 0;
					vxtorch[i] = 0;
					continue;
				}
				float num7 = num3 / (num2 * 15f);
				vxsun[i] = (int)Math.Floor(num7 * 255f + 0.5f);
				num7 = num4 / (num2 * 15f);
				vxtorch[i] = (int)Math.Floor(num7 * 255f + 0.5f);
			}
		}

		public void MakeFace(IntVector3 iv, IntVector3 chunkLocal, IntVector3 local, BlockFace face, BlockType t, BlockBuildData bd, int block)
		{
			bool needsFancyLighting = t.NeedsFancyLighting;
			int num = 0;
			bd._min.SetToMin(iv);
			bd._max.SetToMax(iv);
			BlockTerrain.Instance.FillFaceLightTable(local, face, ref bd._sun, ref bd._torch);
			if (t.LightAsTranslucent)
			{
				float val = Block.GetSunLightLevel(block);
				float val2 = Block.GetTorchLightLevel(block);
				for (int i = 0; i < 9; i++)
				{
					bd._sun[i] = Math.Max(bd._sun[i], val);
					bd._torch[i] = Math.Max(bd._torch[i], val2);
				}
			}
			for (int j = 0; j < 9; j++)
			{
				if (bd._sun[j] < 0f)
				{
					if (j < 4)
					{
						num |= 1 << j;
					}
					else if (j > 4)
					{
						num |= 1 << j - 1;
					}
				}
			}
			FillFaceColors(ref bd._vxsun, ref bd._vxtorch, ref bd._sun, ref bd._torch);
			for (int k = 0; k < 4; k++)
			{
				bd.AddVertex(new BlockVertex(chunkLocal, face, k, t, bd._vxsun[k], bd._vxtorch[k], num), needsFancyLighting);
			}
		}

		public void AddBlock(IntVector3 iv, BlockBuildData bd)
		{
			int safeBlockAtABS = BlockTerrain.Instance.GetSafeBlockAtABS(iv);
			BlockType type = Block.GetType(safeBlockAtABS);
			bool interiorFaces = type.InteriorFaces;
			if (type[BlockFace.NEGY] == -1)
			{
				return;
			}
			IntVector3 intVector = IntVector3.Subtract(iv, BlockTerrain.Instance._worldMin);
			IntVector3 intVector2 = IntVector3.Subtract(iv, _worldMin);
			if (!Block.HasAlpha(safeBlockAtABS))
			{
				for (BlockFace blockFace = BlockFace.POSX; blockFace < BlockFace.NUM_FACES; blockFace++)
				{
					IntVector3 intVector3 = IntVector3.Add(intVector, BlockTerrain._faceNeighbors[(int)blockFace]);
					int num = intVector3.Y.Clamp(0, 127) + intVector3.X.Clamp(0, 383) * 128 + intVector3.Z.Clamp(0, 383) * 128 * 384;
					if (Block.HasAlpha(BlockTerrain.Instance._blocks[num]))
					{
						MakeFace(iv, intVector2, intVector, blockFace, type, bd, safeBlockAtABS);
					}
				}
				return;
			}
			if (interiorFaces)
			{
				for (BlockFace blockFace2 = BlockFace.POSX; blockFace2 < BlockFace.NUM_FACES; blockFace2++)
				{
					MakeFace(iv, intVector2, intVector, blockFace2, type, bd, safeBlockAtABS);
					IntVector3 local = IntVector3.Add(intVector, BlockTerrain._faceNeighbors[(int)blockFace2]);
					int num2 = local.Y.Clamp(0, 127) + local.X.Clamp(0, 383) * 128 + local.Z.Clamp(0, 383) * 128 * 384;
					int num3 = BlockTerrain.Instance._blocks[num2];
					BlockType type2 = Block.GetType(num3);
					if (type2[BlockFace.NEGY] == -1)
					{
						MakeFace(IntVector3.Add(iv, BlockTerrain._faceNeighbors[(int)blockFace2]), IntVector3.Add(intVector2, BlockTerrain._faceNeighbors[(int)blockFace2]), local, Block.OpposingFace[(int)blockFace2], type, bd, num3);
					}
				}
				return;
			}
			for (BlockFace blockFace3 = BlockFace.POSX; blockFace3 < BlockFace.NUM_FACES; blockFace3++)
			{
				IntVector3 intVector4 = IntVector3.Add(intVector, BlockTerrain._faceNeighbors[(int)blockFace3]);
				int num4 = intVector4.Y.Clamp(0, 127) + intVector4.X.Clamp(0, 383) * 128 + intVector4.Z.Clamp(0, 383) * 128 * 384;
				if (Block.GetType(BlockTerrain.Instance._blocks[num4]) != type)
				{
					MakeFace(iv, intVector2, intVector, blockFace3, type, bd, safeBlockAtABS);
				}
			}
		}

		public void BuildFaces(GraphicsDevice gd)
		{
			BlockBuildData blockBuildData = BlockBuildData.Alloc();
			IntVector3 worldMin = _worldMin;
			IntVector3 intVector = IntVector3.Add(new IntVector3(16, 128, 16), _worldMin);
			worldMin.Y = intVector.Y - 1;
			while (worldMin.Y >= _worldMin.Y)
			{
				if (!BlockTerrain.Instance.IsReady)
				{
					blockBuildData.Release();
					return;
				}
				worldMin.Z = _worldMin.Z;
				while (worldMin.Z < intVector.Z)
				{
					worldMin.X = _worldMin.X;
					while (worldMin.X < intVector.X)
					{
						AddBlock(worldMin, blockBuildData);
						worldMin.X++;
					}
					worldMin.Z++;
				}
				worldMin.Y--;
			}
			if (_vbs == null)
			{
				_vbs = new List<VertexBuffer>();
			}
			if (_fancyVBs == null)
			{
				_fancyVBs = new List<VertexBuffer>();
			}
			blockBuildData.BuildVBs(gd, ref _vbs, ref _fancyVBs);
			if (_vbs.Count != 0 || _fancyVBs.Count != 0)
			{
				blockBuildData._max = IntVector3.Add(new IntVector3(1, 1, 1), blockBuildData._max);
				IntVector3.FillBoundingCorners(blockBuildData._min, blockBuildData._max, ref _boundingCorners);
			}
			blockBuildData.Release();
		}

		public bool ChunkIsOutsidePlane(Plane plane)
		{
			for (int i = 0; i < 8; i++)
			{
				if (plane.DotCoordinate(_boundingCorners[i]) <= 0.001f)
				{
					return false;
				}
			}
			return true;
		}

		public bool HasGeometry()
		{
			if (_vbs == null || _vbs.Count == 0)
			{
				if (_fancyVBs != null)
				{
					return _fancyVBs.Count != 0;
				}
				return false;
			}
			return true;
		}

		public bool TouchesFrustum(BoundingFrustum frustum)
		{
			if (!HasGeometry())
			{
				return false;
			}
			if (!ChunkIsOutsidePlane(frustum.Near) && !ChunkIsOutsidePlane(frustum.Far) && !ChunkIsOutsidePlane(frustum.Left) && !ChunkIsOutsidePlane(frustum.Right) && !ChunkIsOutsidePlane(frustum.Bottom) && !ChunkIsOutsidePlane(frustum.Top))
			{
				return true;
			}
			return false;
		}

		public void DrawReflection(GraphicsDevice gd, Effect effect, BoundingFrustum frustum)
		{
			effect.Parameters["WorldBase"].SetValue(_basePosition);
			if (_vbs != null && _vbs.Count > 0)
			{
				for (int i = 0; i < _vbs.Count; i++)
				{
					VertexBuffer vertexBuffer = _vbs[i];
					gd.SetVertexBuffer(vertexBuffer);
					effect.CurrentTechnique.Passes[2].Apply();
					gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, vertexBuffer.VertexCount / 2);
				}
			}
			if (_fancyVBs != null && _fancyVBs.Count > 0)
			{
				for (int j = 0; j < _fancyVBs.Count; j++)
				{
					VertexBuffer vertexBuffer2 = _fancyVBs[j];
					gd.SetVertexBuffer(vertexBuffer2);
					effect.CurrentTechnique.Passes[2].Apply();
					gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer2.VertexCount, 0, vertexBuffer2.VertexCount / 2);
				}
			}
		}

		public void Draw(GraphicsDevice gd, Effect effect, bool fancy, BoundingFrustum frustum)
		{
			effect.Parameters["WorldBase"].SetValue(_basePosition);
			if (fancy)
			{
				if (_fancyVBs != null && _fancyVBs.Count > 0)
				{
					for (int i = 0; i < _fancyVBs.Count; i++)
					{
						VertexBuffer vertexBuffer = _fancyVBs[i];
						gd.SetVertexBuffer(vertexBuffer);
						effect.CurrentTechnique.Passes[1].Apply();
						gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, vertexBuffer.VertexCount / 2);
					}
				}
			}
			else if (_vbs != null && _vbs.Count > 0)
			{
				for (int j = 0; j < _vbs.Count; j++)
				{
					VertexBuffer vertexBuffer2 = _vbs[j];
					gd.SetVertexBuffer(vertexBuffer2);
					effect.CurrentTechnique.Passes[0].Apply();
					gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer2.VertexCount, 0, vertexBuffer2.VertexCount / 2);
				}
			}
		}

		public static RenderChunk Alloc()
		{
			RenderChunk renderChunk = _cache.Get();
			renderChunk._refcount = 1;
			return renderChunk;
		}

		public void Release()
		{
			if (Interlocked.Decrement(ref _refcount) != 0)
			{
				return;
			}
			if (_vbs != null)
			{
				for (int i = 0; i < Enumerable.Count<VertexBuffer>((IEnumerable<VertexBuffer>)_vbs); i++)
				{
					_vbs[i].Dispose();
				}
				_vbs.Clear();
			}
			if (_fancyVBs != null)
			{
				for (int j = 0; j < Enumerable.Count<VertexBuffer>((IEnumerable<VertexBuffer>)_fancyVBs); j++)
				{
					_fancyVBs[j].Dispose();
				}
				_fancyVBs.Clear();
			}
			_cache.Put(this);
		}
	}
}
