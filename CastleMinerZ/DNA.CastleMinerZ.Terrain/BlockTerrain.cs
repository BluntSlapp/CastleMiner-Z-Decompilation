using System;
using System.Diagnostics;
using System.Threading;
using DNA.CastleMinerZ.Terrain.WorldBuilders;
using DNA.CastleMinerZ.Utils;
using DNA.CastleMinerZ.Utils.Threading;
using DNA.CastleMinerZ.Utils.Trace;
using DNA.Drawing;
using DNA.Profiling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace DNA.CastleMinerZ.Terrain
{
	public class BlockTerrain : Entity
	{
		private class AsynchInitData
		{
			public Vector3 center;

			public WorldInfo worldInfo;

			public bool host;

			public AsyncCallback callback;

			public bool teleporting;
		}

		public class ItemBlockCommand : IReleaseable, ILinkedListNode
		{
			public bool AddItem;

			public IntVector3 WorldPosition = IntVector3.Zero;

			public BlockTypeEnum BlockType;

			private static ObjectCache<ItemBlockCommand> _cache = new ObjectCache<ItemBlockCommand>();

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

			public static ItemBlockCommand Alloc()
			{
				return _cache.Get();
			}

			public static void ReleaseList(ItemBlockCommand head)
			{
				_cache.PutList(head);
			}

			public void Release()
			{
				_cache.Put(this);
			}
		}

		public class BuildTaskData : IReleaseable, ILinkedListNode
		{
			public IntVector3 _intVec0;

			public IntVector3 _intVec1;

			public int _intData0;

			public int _intData1;

			public bool _skipProcessing;

			private static ObjectCache<BuildTaskData> _cache = new ObjectCache<BuildTaskData>();

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

			public BuildTaskData()
			{
				_intVec0 = IntVector3.Zero;
				_intVec1 = IntVector3.Zero;
				_intData0 = 0;
				_intData1 = 0;
				_skipProcessing = false;
			}

			public static BuildTaskData Alloc()
			{
				return _cache.Get();
			}

			public void Release()
			{
				_skipProcessing = false;
				_cache.Put(this);
			}
		}

		public class ShiftingTerrainData
		{
			public int source;

			public int dest;

			public int length;

			public int fillStart;

			public int fillLength;

			public int dx;

			public int dz;

			public bool running;

			public bool done = true;
		}

		public class BlockReference : IReleaseable, ILinkedListNode
		{
			public static int _refcount = 0;

			public IntVector3 _vecIndex;

			public int _index;

			private static ObjectCache<BlockReference> _cache = new ObjectCache<BlockReference>();

			private ILinkedListNode _nextNode;

			public bool IsValid
			{
				get
				{
					return _index != -1;
				}
			}

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

			public BlockReference()
			{
				Interlocked.Increment(ref _refcount);
				_index = -1;
				_vecIndex = IntVector3.Zero;
				_nextNode = null;
			}

			public bool SetIndex(int x, int y, int z)
			{
				return SetIndex(new IntVector3(x, y, z));
			}

			public void SetIndex(int x, int y, int z, int index)
			{
				_vecIndex.SetValues(x, y, z);
				_index = index;
			}

			public void SetIndex(IntVector3 vi, int index)
			{
				_vecIndex = vi;
				_index = index;
			}

			public bool SetIndex(IntVector3 vi)
			{
				_vecIndex = vi;
				if (_theTerrain.IsIndexValid(_vecIndex))
				{
					_index = _theTerrain.MakeIndex(_vecIndex);
					return true;
				}
				_index = -1;
				return false;
			}

			public int Get()
			{
				return _theTerrain.GetBlockAt(_index);
			}

			public void Set(int value)
			{
				_theTerrain.SetBlockAt(_index, value);
			}

			public static BlockReference Alloc(int x, int y, int z)
			{
				BlockReference blockReference = Alloc();
				blockReference.SetIndex(x, y, z);
				return blockReference;
			}

			public static BlockReference Alloc(IntVector3 i)
			{
				BlockReference blockReference = Alloc();
				blockReference.SetIndex(i);
				return blockReference;
			}

			public static BlockReference Alloc(Vector3 v)
			{
				return Alloc(_theTerrain.MakeIndexVectorFromPosition(v));
			}

			public static BlockReference Alloc()
			{
				return _cache.Get();
			}

			public void Release()
			{
				_cache.Put(this);
			}
		}

		public class ChunkActionPool
		{
			public int[] _pool;

			public int _nextOffset;

			public NextChunkAction _action;

			public NextChunkAction _nextAction;

			public TaskDelegate _work;

			public bool Empty
			{
				get
				{
					return _nextOffset == -1;
				}
			}

			public ChunkActionPool(NextChunkAction action, NextChunkAction nextAction, TaskDelegate work)
			{
				_pool = new int[576];
				_nextOffset = -1;
				_action = action;
				_work = work;
				_nextAction = nextAction;
			}

			public void Add(int index)
			{
				int num = Interlocked.Increment(ref _nextOffset);
				if (num < 576)
				{
					_pool[num] = index;
					Instance._chunks[index]._action = _action;
				}
			}

			public virtual void Drain()
			{
				BlockTerrain instance = Instance;
				GatherTask gatherTask = TaskDispatcher.Instance.AddGatherTask(instance._stepUpdateDelegate, null);
				IntVector3 worldMin = instance._worldMin;
				for (int i = 0; i <= _nextOffset; i++)
				{
					int num = _pool[i];
					BuildTaskData buildTaskData = BuildTaskData.Alloc();
					buildTaskData._intVec0 = IntVector3.Add(worldMin, instance.MakeIndexVectorFromChunkIndex(num));
					buildTaskData._intData0 = num;
					instance._chunks[num]._action = _nextAction;
					gatherTask.AddTask(_work, buildTaskData);
				}
				Clear();
				instance.IncrementBuildTasks();
				gatherTask.Start();
			}

			public void Clear()
			{
				_nextOffset = -1;
			}
		}

		public class LoadChunkActionPool : ChunkActionPool
		{
			private ChunkCacheCommandDelegate _loadedDelegate;

			public int _chunksInFlight;

			public LoadChunkActionPool(NextChunkAction action, NextChunkAction nextAction, TaskDelegate work)
				: base(action, nextAction, work)
			{
				_chunksInFlight = 0;
				_loadedDelegate = ChunkLoaded;
			}

			public override void Drain()
			{
				BlockTerrain instance = Instance;
				GatherTask gatherTask = TaskDispatcher.Instance.AddGatherTask(instance._stepUpdateDelegate, null);
				gatherTask.SetCount(_nextOffset + 1);
				IntVector3 worldMin = instance._worldMin;
				instance.IncrementBuildTasks();
				for (int i = 0; i <= _nextOffset; i++)
				{
					int num = _pool[i];
					BuildTaskData buildTaskData = BuildTaskData.Alloc();
					buildTaskData._intVec0 = IntVector3.Add(worldMin, instance.MakeIndexVectorFromChunkIndex(num));
					buildTaskData._intData0 = num;
					instance._chunks[num]._action = _nextAction;
					Task task = Task.Alloc();
					task.Init(_work, buildTaskData, gatherTask);
					ChunkCacheCommand chunkCacheCommand = ChunkCacheCommand.Alloc();
					chunkCacheCommand._context = task;
					chunkCacheCommand._worldPosition = buildTaskData._intVec0;
					if (_loadedDelegate == null)
					{
						chunkCacheCommand._callback = ChunkLoaded;
					}
					else
					{
						chunkCacheCommand._callback = _loadedDelegate;
					}
					chunkCacheCommand._command = ChunkCacheCommandEnum.FETCHDELTAFORTERRAIN;
					chunkCacheCommand._priority = 1;
					_chunksInFlight++;
					ChunkCache.Instance.AddCommand(chunkCacheCommand);
				}
				Clear();
			}

			public void ChunkLoaded(ChunkCacheCommand cmd)
			{
				_chunksInFlight--;
				Task task = (Task)cmd._context;
				bool flag = true;
				if (cmd._command != ChunkCacheCommandEnum.RESETWAITINGCHUNKS && !Instance._resetRequested)
				{
					int num = Instance.MakeChunkIndexFromWorldIndexVector(cmd._worldPosition);
					if (num != -1)
					{
						Instance._chunks[num]._delta = cmd._delta;
						flag = false;
					}
				}
				else
				{
					int num2 = Instance.MakeChunkIndexFromWorldIndexVector(cmd._worldPosition);
					if (num2 != -1)
					{
						Instance._chunks[num2]._action = NextChunkAction.WAITING_TO_LOAD;
					}
				}
				if (flag)
				{
					BuildTaskData buildTaskData = task._context as BuildTaskData;
					if (buildTaskData != null)
					{
						buildTaskData._skipProcessing = true;
					}
				}
				TaskDispatcher.Instance.AddTask(task);
				cmd.Release();
			}
		}

		public class LightingPool
		{
			public static int[] _blocks;

			public int _maxUsed;

			public IntVector3 _min;

			public IntVector3 _max;

			private int _length;

			private int[] _list1;

			private int[] _list2;

			private int[] _neighbors;

			private int[] _currentList;

			private int _currentIndex;

			public bool Empty
			{
				get
				{
					return _currentIndex == -1;
				}
			}

			public LightingPool(int size)
			{
				if (_blocks == null)
				{
					_blocks = Instance._blocks;
				}
				_list1 = new int[size];
				_list2 = new int[size];
				_neighbors = new int[6];
				_currentList = _list1;
				_currentIndex = -1;
				_length = size;
			}

			public void Add(int index)
			{
				int num = Interlocked.Increment(ref _currentIndex);
				if (num < _length)
				{
					_currentList[num] = index;
				}
				else
				{
					_blocks[index] &= -1025;
				}
				_maxUsed = ((num > _maxUsed) ? num : _maxUsed);
			}

			public void GetList(out int[] list, out int count, out int[] neighbors)
			{
				list = _currentList;
				count = _currentIndex + 1;
				count = Math.Min(count, _length);
				neighbors = _neighbors;
				_currentList = ((_currentList == _list1) ? _list2 : _list1);
				_currentIndex = -1;
			}

			public void Clear()
			{
				_currentIndex = -1;
			}

			public void ResetAABB()
			{
				_min = new IntVector3(int.MaxValue, int.MaxValue, int.MaxValue);
				_max = new IntVector3(int.MinValue, int.MinValue, int.MinValue);
			}

			public void UpdateMinAABB(ref IntVector3 value)
			{
				_min.SetToMin(value);
				_max.SetToMax(value);
			}
		}

		public enum NextChunkAction
		{
			WAITING_TO_LOAD,
			NEEDS_BLOCKS,
			COMPUTING_BLOCKS,
			NEEDS_LIGHTING,
			COMPUTING_LIGHTING,
			NEEDS_GEOMETRY,
			COMPUTING_GEOMETRY,
			NONE
		}

		public class PendingMod : IReleaseable, ILinkedListNode
		{
			public IntVector3 _worldPosition;

			public BlockTypeEnum _blockType;

			private static ObjectCache<PendingMod> _cache = new ObjectCache<PendingMod>();

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

			public static PendingMod Alloc()
			{
				return _cache.Get();
			}

			public void Release()
			{
				_cache.Put(this);
			}
		}

		public struct TerrainChunk
		{
			public RenderChunk _chunk;

			public DNA.CastleMinerZ.Utils.Threading.SpinLock _chunkLock;

			public CountdownLatch _numUsers;

			public SynchronizedQueue<PendingMod> _mods;

			public int[] _delta;

			public NextChunkAction _action;

			public void Init()
			{
				_chunk = RenderChunk.Alloc();
				_action = NextChunkAction.WAITING_TO_LOAD;
				_chunkLock = default(DNA.CastleMinerZ.Utils.Threading.SpinLock);
				_numUsers = default(CountdownLatch);
				_mods = new SynchronizedQueue<PendingMod>();
				_delta = null;
			}

			public void SwapIn(TerrainChunk newChunk)
			{
				_chunk = newChunk._chunk;
				_chunk.AddRef();
				_action = newChunk._action;
				_numUsers.Value = newChunk._numUsers.Value;
				_mods.ReplaceContentsWith(newChunk._mods);
				_delta = newChunk._delta;
			}

			public void Reset()
			{
				_chunk.Release();
				_chunk = RenderChunk.Alloc();
				_action = NextChunkAction.WAITING_TO_LOAD;
				_numUsers.Value = 0;
				_delta = null;
				while (!_mods.Empty)
				{
					_mods.Dequeue().Release();
				}
			}

			public void ReplaceChunk(RenderChunk newChunk)
			{
				_chunkLock.Lock();
				RenderChunk chunk = _chunk;
				_chunk = newChunk;
				_chunkLock.Unlock();
				chunk.Release();
			}

			public RenderChunk GetChunk()
			{
				_chunkLock.Lock();
				RenderChunk chunk = _chunk;
				chunk.AddRef();
				_chunkLock.Unlock();
				return chunk;
			}
		}

		public delegate void ThreadedResetDelegate();

		public const int BADINDEX = -1;

		public const int MAXX = 384;

		public const int MAXY = 128;

		public const int MAXZ = 384;

		public const int BUFFER_SIZE = 18874368;

		public const float BLOCKSIZE = 1f;

		public const int CHUNK_WIDTH = 16;

		public const int CHUNK_DEPTH = 16;

		public const int CHUNK_HEIGHT = 128;

		public const int MAX_CHUNK_X = 24;

		public const int MAX_CHUNK_Z = 24;

		public const int NUM_CHUNKS = 576;

		public const int NUM_CHUNKS_IN_LOAD = 64;

		public const int MAX_LOADING_COUNT = 183;

		public const float TILES_PER_ROW = 8f;

		public const float AO_TILES_PER_ROW = 16f;

		private const int LIGHT_BUFFER_SIZE = 262144;

		private const int UPDATE_LIGHT_BUFFER_SIZE = 5000;

		protected static BlockTerrain _theTerrain = null;

		public static readonly IntVector3[] _faceNeighbors = new IntVector3[6]
		{
			new IntVector3(1, 0, 0),
			new IntVector3(0, 0, -1),
			new IntVector3(-1, 0, 0),
			new IntVector3(0, 0, 1),
			new IntVector3(0, 1, 0),
			new IntVector3(0, -1, 0)
		};

		private static readonly int[] _faceIndexNeighbors = new int[6] { 128, -49152, -128, 49152, 1, -1 };

		private static readonly IntVector3[][] _lightNeighbors = new IntVector3[6][]
		{
			new IntVector3[9]
			{
				new IntVector3(1, 1, 1),
				new IntVector3(1, 1, 0),
				new IntVector3(1, 1, -1),
				new IntVector3(1, 0, 1),
				new IntVector3(1, 0, 0),
				new IntVector3(1, 0, -1),
				new IntVector3(1, -1, 1),
				new IntVector3(1, -1, 0),
				new IntVector3(1, -1, -1)
			},
			new IntVector3[9]
			{
				new IntVector3(1, 1, -1),
				new IntVector3(0, 1, -1),
				new IntVector3(-1, 1, -1),
				new IntVector3(1, 0, -1),
				new IntVector3(0, 0, -1),
				new IntVector3(-1, 0, -1),
				new IntVector3(1, -1, -1),
				new IntVector3(0, -1, -1),
				new IntVector3(-1, -1, -1)
			},
			new IntVector3[9]
			{
				new IntVector3(-1, 1, -1),
				new IntVector3(-1, 1, 0),
				new IntVector3(-1, 1, 1),
				new IntVector3(-1, 0, -1),
				new IntVector3(-1, 0, 0),
				new IntVector3(-1, 0, 1),
				new IntVector3(-1, -1, -1),
				new IntVector3(-1, -1, 0),
				new IntVector3(-1, -1, 1)
			},
			new IntVector3[9]
			{
				new IntVector3(-1, 1, 1),
				new IntVector3(0, 1, 1),
				new IntVector3(1, 1, 1),
				new IntVector3(-1, 0, 1),
				new IntVector3(0, 0, 1),
				new IntVector3(1, 0, 1),
				new IntVector3(-1, -1, 1),
				new IntVector3(0, -1, 1),
				new IntVector3(1, -1, 1)
			},
			new IntVector3[9]
			{
				new IntVector3(-1, 1, -1),
				new IntVector3(0, 1, -1),
				new IntVector3(1, 1, -1),
				new IntVector3(-1, 1, 0),
				new IntVector3(0, 1, 0),
				new IntVector3(1, 1, 0),
				new IntVector3(-1, 1, 1),
				new IntVector3(0, 1, 1),
				new IntVector3(1, 1, 1)
			},
			new IntVector3[9]
			{
				new IntVector3(1, -1, -1),
				new IntVector3(0, -1, -1),
				new IntVector3(-1, -1, -1),
				new IntVector3(1, -1, 0),
				new IntVector3(0, -1, 0),
				new IntVector3(-1, -1, 0),
				new IntVector3(1, -1, 1),
				new IntVector3(0, -1, 1),
				new IntVector3(-1, -1, 1)
			}
		};

		private Plane[] _facePlanes = new Plane[6]
		{
			new Plane(1f, 0f, 0f, 0f),
			new Plane(0f, 0f, -1f, 0f),
			new Plane(-1f, 0f, 0f, 0f),
			new Plane(0f, 0f, 1f, 0f),
			new Plane(0f, 1f, 0f, 0f),
			new Plane(0f, -1f, 0f, 0f)
		};

		public int[] _blocks;

		private int[] _shiftedBlocks;

		public TerrainChunk[] _chunks;

		public int[] _renderIndexList;

		public int _currentEyeChunkIndex;

		public int _currentRenderOrder;

		public IntVector3[] _radiusOrderOffsets;

		public IntVector3 _worldMin;

		public GraphicsDevice _graphicsDevice;

		public Effect _effect;

		public Texture2D _diffuseAlpha;

		public Texture2D _normalSpec;

		public Texture2D _metalLight;

		public Texture3D _envMap;

		public Texture2D _mipMapNormals;

		public Texture2D _mipMapDiffuse;

		public BoundingFrustum _boundingFrustum;

		private CountdownLatch _buildTasksRemaining;

		private CountdownLatch _updateTasksRemaining;

		public TaskDelegate _computeGeometryDelegate;

		public TaskDelegate _computeBlocksDelegate;

		public TaskDelegate _computeLightingDelegate;

		public TaskDelegate _stepUpdateDelegate;

		public TaskDelegate _finishSetBlockDelegate;

		public TaskDelegate _finishRegionOpDelegate;

		public TaskDelegate _shiftTerrainDelegate;

		public ShiftingTerrainData _shiftTerrainData;

		public DepthStencilState _zWriteDisable;

		public DepthStencilState _zWriteEnable;

		public BlendState _disableColorWrites;

		public BlendState _enableColorWrites;

		public IndexBuffer _staticIB;

		private int initblock = Block.IsUninitialized(Block.SetType(0, BlockTypeEnum.NumberOfBlocks), true);

		private LoadChunkActionPool _computeBlocksPool;

		private ChunkActionPool _computeLightingPool;

		private ChunkActionPool _computeGeometryPool;

		private LightingPool _mainLightingPool;

		private LightingPool _updateLightingPool;

		public IntVector3 _cursorPosition;

		public BlockFace _cursorFace;

		public bool _drawCursor;

		private bool _allChunksLoaded;

		private bool _initted;

		public bool _resetRequested = true;

		public Matrix[] _faceMatrices;

		public Vector4[] _vertexUVs;

		private int _loadingProgress;

		private int _maxChunksAtOnce = 64;

		public bool IsWaterWorld;

		private AsynchInitData _asyncInitData;

		public SynchronizedQueue<ItemBlockCommand> ItemBlockCommandQueue = new SynchronizedQueue<ItemBlockCommand>();

		public WorldInfo WorldInfo;

		public WorldBuilder _worldBuilder;

		private int maxLightNodes;

		private float[] avatarSun = new float[27];

		private float[] avatarTorch = new float[27];

		public Vector3 EyePos = new Vector3(0f, 64f, 0f);

		public Vector3 ViewVector = new Vector3(1f, 0f, 0f);

		public Vector3 VectorToSun = new Vector3(1f, 0f, 0f);

		public Color TorchColor = new Color(255, 235, 190);

		public Color SunlightColor = new Color(1f, 1f, 1f);

		public Color SunSpecular = new Color(1, 1, 1);

		public Color FogColor = new Color(0.6f, 0.6f, 1f);

		public Color AmbientSunColor = new Color(1, 1, 1);

		public Color BelowWaterColor = new Color(0.0941f, 0.16f, 0.2235f);

		public float WaterLevel = -0.5f;

		public float PercentMidnight;

		public static BlockTerrain Instance
		{
			get
			{
				return _theTerrain;
			}
		}

		public bool ReadyForInit
		{
			get
			{
				return !_initted;
			}
		}

		public bool MinimallyLoaded
		{
			get
			{
				if (IsReady)
				{
					return _loadingProgress >= 183;
				}
				return false;
			}
		}

		public int LoadingProgress
		{
			get
			{
				if (!IsReady)
				{
					return 0;
				}
				return Math.Min(_loadingProgress * 134 / 183, 100);
			}
		}

		public bool Calculating
		{
			get
			{
				if (!_buildTasksRemaining)
				{
					return _updateTasksRemaining;
				}
				return true;
			}
		}

		public bool IsReady
		{
			get
			{
				if (_initted)
				{
					return !_resetRequested;
				}
				return false;
			}
		}

		public bool CursorVisible
		{
			get
			{
				return _drawCursor;
			}
		}

		public BlockTerrain(GraphicsDevice gd, ContentManager cm)
		{
			_theTerrain = this;
			_graphicsDevice = gd;
			IsWaterWorld = false;
			_effect = cm.Load<Effect>("Effects\\BlockEffect");
			Texture[] array = cm.Load<Texture[]>("Terrain\\Textures");
			_diffuseAlpha = (Texture2D)array[0];
			_normalSpec = (Texture2D)array[1];
			_metalLight = (Texture2D)array[2];
			_mipMapNormals = (Texture2D)array[3];
			_mipMapDiffuse = (Texture2D)array[4];
			Texture2D texture2D = cm.Load<Texture2D>("Textures\\EnvMaps\\envmap0");
			Byte4[] data = new Byte4[texture2D.Bounds.Width * texture2D.Bounds.Height * 4];
			texture2D.GetData<Byte4>(data, 0, texture2D.Bounds.Width * texture2D.Bounds.Height);
			texture2D = cm.Load<Texture2D>("Textures\\EnvMaps\\envmap1");
			texture2D.GetData<Byte4>(data, texture2D.Bounds.Width * texture2D.Bounds.Height, texture2D.Bounds.Width * texture2D.Bounds.Height);
			texture2D = cm.Load<Texture2D>("Textures\\EnvMaps\\envmap2");
			texture2D.GetData<Byte4>(data, texture2D.Bounds.Width * texture2D.Bounds.Height * 2, texture2D.Bounds.Width * texture2D.Bounds.Height);
			texture2D = cm.Load<Texture2D>("Textures\\EnvMaps\\envmap3");
			texture2D.GetData<Byte4>(data, texture2D.Bounds.Width * texture2D.Bounds.Height * 3, texture2D.Bounds.Width * texture2D.Bounds.Height);
			_envMap = new Texture3D(gd, texture2D.Bounds.Width, texture2D.Bounds.Height, 4, false, texture2D.Format);
			_envMap.SetData<Byte4>(data);
			texture2D = null;
			data = null;
			Vector2 vector = new Vector2(3f / ((float)_diffuseAlpha.Width * 2f), 3f / ((float)_diffuseAlpha.Height * 2f));
			Vector2 vector2 = new Vector2(0.125f - vector.X, 0.125f - vector.Y);
			Vector2 vector3 = new Vector2(0.0625f - vector.X, 0.0625f - vector.Y);
			_vertexUVs = new Vector4[4]
			{
				new Vector4(vector.X, vector.Y, vector.X, vector.Y),
				new Vector4(vector2.X, vector.Y, vector3.X, vector.Y),
				new Vector4(vector.X, vector2.Y, vector.X, vector3.Y),
				new Vector4(vector2.X, vector2.Y, vector3.X, vector3.Y)
			};
			_effect.Parameters["VertexUVs"].SetValue(_vertexUVs);
			_faceMatrices = new Matrix[6]
			{
				new Matrix(0f, 0f, -1f, 0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f),
				new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f),
				new Matrix(0f, 0f, 1f, 0f, 0f, 1f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 0f, 0f, 1f),
				new Matrix(-1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 1f),
				new Matrix(-1f, 0f, 0f, 0f, 0f, 0f, -1f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 0f, 1f),
				new Matrix(1f, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 0f, 1f)
			};
			_effect.Parameters["FaceMatrices"].SetValue(_faceMatrices);
			_effect.Parameters["DiffuseAlphaTexture"].SetValue(_diffuseAlpha);
			_effect.Parameters["NormalSpecTexture"].SetValue(_normalSpec);
			_effect.Parameters["MetalLightTexture"].SetValue(_metalLight);
			_effect.Parameters["EnvMapTexture"].SetValue(_envMap);
			_effect.Parameters["MipMapSpecularTexture"].SetValue(_mipMapNormals);
			_effect.Parameters["MipMapDiffuseTexture"].SetValue(_mipMapDiffuse);
			_disableColorWrites = new BlendState();
			_disableColorWrites.ColorWriteChannels = ColorWriteChannels.None;
			_enableColorWrites = new BlendState();
			_enableColorWrites.ColorWriteChannels = ColorWriteChannels.All;
			_zWriteDisable = new DepthStencilState();
			_zWriteDisable.DepthBufferWriteEnable = false;
			_zWriteEnable = new DepthStencilState();
			_zWriteEnable.DepthBufferWriteEnable = true;
			_boundingFrustum = new BoundingFrustum(Matrix.Identity);
			_blocks = new int[18874368];
			_shiftedBlocks = new int[18874368];
			for (int i = 0; i < 18874368; i++)
			{
				_blocks[i] = initblock;
				_shiftedBlocks[i] = initblock;
			}
			SetWorldMin(new IntVector3(-192, -64, -192));
			_renderIndexList = new int[576];
			_chunks = new TerrainChunk[576];
			for (int j = 0; j < 576; j++)
			{
				_chunks[j] = default(TerrainChunk);
				_chunks[j].Init();
			}
			_computeGeometryDelegate = DoThreadedComputeGeometry;
			_computeBlocksDelegate = DoThreadedComputeBlocks;
			_stepUpdateDelegate = DoThreadedStepUpdateChunks;
			_computeLightingDelegate = DoThreadedComputeLighting;
			_shiftTerrainDelegate = DoThreadedShiftTerrain;
			_finishSetBlockDelegate = DoThreadedFinishSetBlock;
			_finishRegionOpDelegate = DoFinishThreadedRegionOperation;
			_shiftTerrainData = new ShiftingTerrainData();
			_buildTasksRemaining = default(CountdownLatch);
			_updateTasksRemaining = default(CountdownLatch);
			_currentEyeChunkIndex = -1;
			_currentRenderOrder = 0;
			int[] array2 = new int[6] { 0, 1, 2, 2, 1, 3 };
			ushort[] array3 = new ushort[98304];
			int num = 0;
			int num2 = 0;
			for (int k = 0; k < 16384; k++)
			{
				for (int l = 0; l < 6; l++)
				{
					array3[num2++] = (ushort)(num + array2[l]);
					if (((num + array2[l]) & 0xFFFF0000u) != 0)
					{
						break;
					}
				}
				num += 4;
			}
			_staticIB = new IndexBuffer(gd, IndexElementSize.SixteenBits, 98304, BufferUsage.WriteOnly);
			_staticIB.SetData(array3);
			_mainLightingPool = new LightingPool(262144);
			_updateLightingPool = new LightingPool(5000);
			_computeBlocksPool = new LoadChunkActionPool(NextChunkAction.NEEDS_BLOCKS, NextChunkAction.COMPUTING_BLOCKS, _computeBlocksDelegate);
			_computeLightingPool = new ChunkActionPool(NextChunkAction.NEEDS_LIGHTING, NextChunkAction.COMPUTING_LIGHTING, _computeLightingDelegate);
			_computeGeometryPool = new ChunkActionPool(NextChunkAction.NEEDS_GEOMETRY, NextChunkAction.COMPUTING_GEOMETRY, _computeGeometryDelegate);
			_allChunksLoaded = false;
			Collidee = true;
			BuildRadiusOrderOffsets();
			GC.Collect();
		}

		private void BuildRadiusOrderOffsets()
		{
			_radiusOrderOffsets = new IntVector3[676];
			_radiusOrderOffsets[0] = IntVector3.Zero;
			int num = 13;
			int num2 = 1;
			for (int i = 1; i <= num; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					int num3 = -i;
					int num4;
					if (num3 >= -num)
					{
						num4 = j;
						if (num4 < num)
						{
							_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
						}
						if (j != 0)
						{
							num4 = -j;
							if (num4 >= -num)
							{
								_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
							}
						}
					}
					num3 = i;
					if (num3 < num)
					{
						num4 = j;
						if (num4 < num)
						{
							_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
						}
						if (j != 0)
						{
							num4 = -j;
							if (num4 >= -num)
							{
								_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
							}
						}
					}
					if (j == i)
					{
						continue;
					}
					num4 = i;
					if (num4 < num)
					{
						num3 = j;
						if (num3 < num)
						{
							_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
						}
						if (j != 0)
						{
							num3 = -j;
							if (num3 >= -num)
							{
								_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
							}
						}
					}
					num4 = -i;
					if (num4 < -num)
					{
						continue;
					}
					num3 = j;
					if (num3 < num)
					{
						_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
					}
					if (j != 0)
					{
						num3 = -j;
						if (num3 >= -num)
						{
							_radiusOrderOffsets[num2++].SetValues(num4, 0, num3);
						}
					}
				}
			}
		}

		public void Init(Vector3 center, WorldInfo worldInfo, bool host)
		{
			WorldInfo = worldInfo;
			if (IsReady)
			{
				if (!_resetRequested)
				{
					Reset();
				}
				while (!ReadyForInit)
				{
				}
			}
			_initted = true;
			_resetRequested = false;
			ChunkCache.Instance.Start(true);
			ChunkCache.Instance.MakeHost(worldInfo, host);
			_worldBuilder = WorldInfo.GetBuilder();
			_loadingProgress = 0;
			_allChunksLoaded = false;
			IsWaterWorld = false;
			IntVector3 worldMin = new IntVector3((int)Math.Floor(center.X / 1f), (int)Math.Floor(center.Y / 1f), (int)Math.Floor(center.Z / 1f));
			worldMin.X -= 192;
			worldMin.Y = -64;
			worldMin.Z -= 192;
			SetWorldMin(worldMin);
			_maxChunksAtOnce = 64;
			CenterOn(center, false);
		}

		private void InternalTeleport(Vector3 center)
		{
			_initted = true;
			_resetRequested = false;
			_loadingProgress = 0;
			_allChunksLoaded = false;
			IntVector3 worldMin = new IntVector3((int)Math.Floor(center.X / 1f), (int)Math.Floor(center.Y / 1f), (int)Math.Floor(center.Z / 1f));
			worldMin.X -= 192;
			worldMin.Y = -64;
			worldMin.Z -= 192;
			SetWorldMin(worldMin);
			_maxChunksAtOnce = 64;
			CenterOn(center, false);
		}

		public void DoThreadedReset(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			ChunkCache.Instance.Stop(true);
			MainThreadMessageSender.Instance.GameOver();
			DoThreadedCleanup(threadIndex, task, context);
			ItemBlockCommand head = ItemBlockCommandQueue.Clear();
			ItemBlockCommand.ReleaseList(head);
		}

		public void DoThreadedCleanup(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			while ((bool)_buildTasksRemaining || (bool)_updateTasksRemaining)
			{
				if (stopwatch.ElapsedMilliseconds > 120000 && TaskDispatcher.Instance.IsIdle(threadIndex))
				{
					if (_asyncInitData != null && _asyncInitData.teleporting)
					{
						throw new Exception("Cleanup timed out waiting for completion during teleport");
					}
					throw new Exception("Cleanup timed out waiting for completion outside of teleport");
				}
				Thread.Sleep(500);
			}
			_loadingProgress = 0;
			_buildTasksRemaining.Value = 0;
			_updateTasksRemaining.Value = 0;
			_shiftTerrainData.running = false;
			_shiftTerrainData.done = false;
			for (int i = 0; i < 576; i++)
			{
				_chunks[i].Reset();
			}
			_computeBlocksPool.Clear();
			_computeLightingPool.Clear();
			_computeGeometryPool.Clear();
			_mainLightingPool.Clear();
			_updateLightingPool.Clear();
			for (int j = 0; j < _blocks.Length; j++)
			{
				_blocks[j] = initblock;
			}
			_initted = false;
		}

		public void BlockingReset()
		{
			Reset();
			while (!ReadyForInit)
			{
			}
		}

		public void Reset()
		{
			if (IsReady)
			{
				_resetRequested = true;
				TaskDispatcher.Instance.AddRushTask(DoThreadedReset, null);
			}
		}

		public void Teleport(Vector3 newPosition)
		{
			AsynchInitData asynchInitData = new AsynchInitData();
			asynchInitData.center = newPosition;
			asynchInitData.worldInfo = WorldInfo;
			asynchInitData.teleporting = true;
			_asyncInitData = asynchInitData;
			if (IsReady)
			{
				_resetRequested = true;
				ChunkCache.Instance.ResetWaitingChunks();
				TaskDispatcher.Instance.AddRushTask(DoThreadedCleanup, null);
			}
		}

		public void AsyncInit(WorldInfo worldInfo, bool host, AsyncCallback callback)
		{
			AsynchInitData asynchInitData = new AsynchInitData();
			asynchInitData.center = worldInfo.LastPosition;
			asynchInitData.worldInfo = worldInfo;
			asynchInitData.host = host;
			asynchInitData.callback = callback;
			asynchInitData.teleporting = false;
			_asyncInitData = asynchInitData;
			Reset();
		}

		protected void AddToLoadingProgress(int value)
		{
			int loadingProgress;
			int value2;
			do
			{
				loadingProgress = _loadingProgress;
				if (loadingProgress >= 182)
				{
					break;
				}
				value2 = Math.Min(loadingProgress + value, 182);
			}
			while (Interlocked.CompareExchange(ref _loadingProgress, value2, loadingProgress) != loadingProgress);
		}

		public bool IsTracerStillInWorld(Vector3 pos)
		{
			pos -= IntVector3.ToVector3(_worldMin);
			if (pos.X < 0f || pos.Z < 0f || pos.Y < 0f || pos.X >= 384f || pos.Z >= 384f)
			{
				return false;
			}
			return true;
		}

		protected void SetWorldMin(IntVector3 newmin)
		{
			_worldMin.X = newmin.X / 16 * 16;
			_worldMin.Y = -64;
			_worldMin.Z = newmin.Z / 16 * 16;
		}

		public IntVector3 GetChunkVectorIndex(Vector3 pos)
		{
			IntVector3 result = MakeIndexVectorFromPosition(pos);
			result.X /= 16;
			result.Y = 0;
			result.Z /= 16;
			return result;
		}

		public Vector3 MakePositionFromIndexVector(IntVector3 v)
		{
			Vector3 value = IntVector3.ToVector3(IntVector3.Add(v, _worldMin));
			return Vector3.Multiply(value, 1f);
		}

		public IntVector3 MakeIndexVectorFromPosition(Vector3 a)
		{
			IntVector3 a2 = new IntVector3((int)Math.Floor(a.X / 1f), (int)Math.Floor(a.Y / 1f), (int)Math.Floor(a.Z / 1f));
			return IntVector3.Subtract(a2, _worldMin);
		}

		public int MakeIndexFromPosition(Vector3 a)
		{
			IntVector3 a2 = MakeIndexVectorFromPosition(a);
			if (IsIndexValid(a2))
			{
				return MakeIndex(a2);
			}
			return -1;
		}

		public int MakeIndexFromWorldIndexVector(IntVector3 a)
		{
			IntVector3 a2 = IntVector3.Subtract(a, _worldMin);
			if (IsIndexValid(a2))
			{
				return MakeIndex(a2);
			}
			return -1;
		}

		public IntVector3 MakeIndexVectorFromChunkIndex(int index)
		{
			int num = index / 24;
			int num2 = index % 24;
			return new IntVector3(num2 * 16, 0, num * 16);
		}

		public IntVector3 MakeIndexVectorFromIndex(int index)
		{
			int num = index / 49152;
			index -= num * 49152;
			int num2 = index / 128;
			int y = index - num2 * 128;
			return new IntVector3(num2, y, num);
		}

		public bool IndexVectorIsOnEdge(IntVector3 index)
		{
			if (index.X != 0 && index.X != 383 && index.Z != 0 && index.Z != 383 && index.Y != 0)
			{
				return index.Y == 127;
			}
			return true;
		}

		public bool IsIndexValid(IntVector3 a)
		{
			if (a.X >= 0 && a.X < 384 && a.Y >= 0 && a.Y < 128 && a.Z >= 0)
			{
				return a.Z < 384;
			}
			return false;
		}

		public bool IsIndexValid(int x, int y, int z)
		{
			if (x >= 0 && x < 384 && y >= 0 && y < 128 && z >= 0)
			{
				return z < 384;
			}
			return false;
		}

		public int MakeIndex(Vector3 a)
		{
			IntVector3 a2 = MakeIndexVectorFromPosition(a);
			return MakeIndex(a2);
		}

		public int MakeIndex(int x, int y, int z)
		{
			return y + x * 128 + z * 128 * 384;
		}

		public int MakeIndex(IntVector3 a)
		{
			return a.Y + a.X * 128 + a.Z * 128 * 384;
		}

		public int MakeChunkIndexFromWorldIndexVector(IntVector3 i)
		{
			IntVector3 a = IntVector3.Subtract(i, _worldMin);
			if (IsIndexValid(a))
			{
				return MakeChunkIndexFromIndexVector(a);
			}
			return -1;
		}

		public int MakeChunkIndexFromIndexVector(IntVector3 a)
		{
			return a.Z / 16 * 24 + a.X / 16;
		}

		public IntVector3 MakeSafeVectorIndex(IntVector3 a)
		{
			return new IntVector3(a.X.Clamp(0, 383), a.Y.Clamp(0, 127), a.Z.Clamp(0, 383));
		}

		public int GetSafeBlockAt(IntVector3 a)
		{
			return GetBlockAt(MakeSafeVectorIndex(a));
		}

		public IntVector3 GetLocalIndex(IntVector3 a)
		{
			return IntVector3.Subtract(a, _worldMin);
		}

		public int GetSafeBlockAtABS(IntVector3 a)
		{
			IntVector3 a2 = IntVector3.Subtract(a, _worldMin);
			return GetSafeBlockAt(a2);
		}

		public IntVector3 GetNeighborIndex(IntVector3 a, BlockFace face)
		{
			return IntVector3.Add(a, _faceNeighbors[(int)face]);
		}

		public int GetNeighborBlockAtABS(IntVector3 a, BlockFace face)
		{
			IntVector3 a2 = IntVector3.Subtract(IntVector3.Add(a, _faceNeighbors[(int)face]), _worldMin);
			return GetSafeBlockAt(a2);
		}

		public void FillFaceLightTable(IntVector3 local, BlockFace face, ref float[] sun, ref float[] torch)
		{
			IntVector3[] array = _lightNeighbors[(int)face];
			for (int i = 0; i < 9; i++)
			{
				IntVector3 a = IntVector3.Add(local, array[i]);
				if (IsIndexValid(a))
				{
					int blockAt = GetBlockAt(a);
					BlockTypeEnum typeIndex = Block.GetTypeIndex(blockAt);
					if (BlockType.GetType(typeIndex).Opaque || typeIndex == BlockTypeEnum.NumberOfBlocks || Block.IsInList(blockAt))
					{
						sun[i] = -1f;
						torch[i] = -1f;
					}
					else
					{
						sun[i] = Block.GetSunLightLevel(blockAt);
						torch[i] = Block.GetTorchLightLevel(blockAt);
					}
				}
				else
				{
					sun[i] = -1f;
					torch[i] = -1f;
				}
			}
		}

		public void FillCubeLightTable(IntVector3 center, ref float[] sun, ref float[] torch)
		{
			IntVector3 a = IntVector3.Subtract(center, _worldMin);
			IntVector3 b = new IntVector3(-1, -1, -1);
			int num = 0;
			b.Z = -1;
			while (b.Z <= 1)
			{
				b.Y = -1;
				while (b.Y <= 1)
				{
					b.X = -1;
					while (b.X <= 1)
					{
						IntVector3 a2 = IntVector3.Add(a, b);
						if (IsIndexValid(a2))
						{
							int blockAt = GetBlockAt(a2);
							BlockTypeEnum typeIndex = Block.GetTypeIndex(blockAt);
							if (typeIndex == BlockTypeEnum.NumberOfBlocks || Block.IsInList(blockAt))
							{
								sun[num] = 1f;
								torch[num] = 0f;
							}
							else if (BlockType.GetType(typeIndex).Opaque)
							{
								sun[num] = -1f;
								torch[num] = -1f;
							}
							else
							{
								sun[num] = (float)Block.GetSunLightLevel(blockAt) / 15f;
								torch[num] = (float)Block.GetTorchLightLevel(blockAt) / 15f;
							}
						}
						else
						{
							sun[num] = 1f;
							torch[num] = 0f;
						}
						b.X++;
						num++;
					}
					b.Y++;
				}
				b.Z++;
			}
		}

		public BlockTypeEnum GetBlockWithChanges(Vector3 worldLocation)
		{
			return GetBlockWithChanges(IntVector3.FromVector3(worldLocation));
		}

		public BlockTypeEnum GetBlockWithChanges(IntVector3 worldIndex)
		{
			if (IsReady)
			{
				IntVector3 a = IntVector3.Subtract(worldIndex, _worldMin);
				if (IsIndexValid(a))
				{
					int num = MakeChunkIndexFromIndexVector(a);
					lock (_chunks[num]._mods)
					{
						for (PendingMod pendingMod = _chunks[num]._mods.Front; pendingMod != null; pendingMod = (PendingMod)pendingMod.NextNode)
						{
							if (pendingMod._worldPosition.Equals(worldIndex))
							{
								return pendingMod._blockType;
							}
						}
					}
					int num2 = MakeIndex(a);
					return Block.GetTypeIndex(_blocks[num2]);
				}
			}
			return BlockTypeEnum.Empty;
		}

		public int GetBlockAt(int index)
		{
			return _blocks[index];
		}

		public int GetBlockAt(int x, int y, int z)
		{
			return GetBlockAt(MakeIndex(x, y, z));
		}

		public int GetBlockAt(IntVector3 a)
		{
			return GetBlockAt(MakeIndex(a));
		}

		public void SetBlockAt(IntVector3 a, int data)
		{
			SetBlockAt(MakeIndex(a), data);
		}

		public void SetBlockAt(int x, int y, int z, int data)
		{
			SetBlockAt(MakeIndex(x, y, z), data);
		}

		public void SetBlockAt(int index, int data)
		{
			_blocks[index] = data;
		}

		public BlockReference GetBlockReference(IntVector3 i)
		{
			return BlockReference.Alloc(i);
		}

		public BlockReference GetBlockReference(int x, int y, int z)
		{
			return BlockReference.Alloc(x, y, z);
		}

		public BlockReference GetBlockReference(Vector3 v)
		{
			return BlockReference.Alloc(v);
		}

		public BlockReference GetBlockReference(int index)
		{
			return BlockReference.Alloc(MakeIndexVectorFromIndex(index));
		}

		public void DecrementBuildTasks()
		{
			_buildTasksRemaining.Decrement();
		}

		public void IncrementBuildTasks()
		{
			_buildTasksRemaining.Increment();
		}

		public void SetSkyAndEmitterLightingForChunk(IntVector3 corner)
		{
			IntVector3 regionMin = IntVector3.Subtract(corner, _worldMin);
			regionMin.Y = 0;
			SetSkyAndEmitterLightingForRegion(regionMin, new IntVector3(regionMin.X + 16 - 1, regionMin.Y + 128 - 1, regionMin.Z + 16 - 1));
		}

		public void SetSkyAndEmitterLightingForRegion(IntVector3 regionMin, IntVector3 regionMax)
		{
			IntVector3 a = regionMin;
			a.Z = regionMin.Z;
			while (a.Z <= regionMax.Z)
			{
				a.X = regionMin.X;
				while (a.X <= regionMax.X)
				{
					a.Y = regionMax.Y;
					int num = MakeIndex(a);
					bool flag = regionMax.Y != 127 && (!Block.IsSky(_blocks[num + 1]) || Block.GetType(_blocks[num + 1]).LightTransmission != 16);
					for (; a.Y >= regionMin.Y; a.Y--, num--)
					{
						if (_resetRequested)
						{
							return;
						}
						int num2 = _blocks[num];
						BlockType type = Block.GetType(num2);
						if (Block.IsOpaque(num2))
						{
							flag = true;
							if (type.SelfIllumination == 0)
							{
								continue;
							}
						}
						else if (!flag)
						{
							int num3 = type.LightTransmission - 1;
							flag = flag || num3 != 15;
							num2 |= 0x100 | ((num3 >= 0) ? num3 : 0);
						}
						_blocks[num] = Block.SetTorchLightLevel(num2, type.SelfIllumination);
					}
					a.X++;
				}
				a.Z++;
			}
		}

		public void ResetSkyAndEmitterLightingForRegion(IntVector3 regionMin, IntVector3 regionMax)
		{
			IntVector3 a = regionMin;
			if (!IsIndexValid(regionMin) || !IsIndexValid(regionMax))
			{
				return;
			}
			a.Z = regionMin.Z;
			while (a.Z <= regionMax.Z)
			{
				a.X = regionMin.X;
				while (a.X <= regionMax.X)
				{
					a.Y = regionMax.Y;
					int num = MakeIndex(a);
					bool flag = regionMax.Y != 127 && (!Block.IsSky(_blocks[num + 1]) || Block.GetType(_blocks[num + 1]).LightTransmission != 16);
					while (a.Y >= 0)
					{
						int num2 = _blocks[num];
						BlockType type = Block.GetType(num2);
						bool flag2 = Block.IsSky(num2);
						if (Block.IsOpaque(num2))
						{
							flag = true;
							num2 &= -257;
						}
						else if (!flag)
						{
							int num3 = type.LightTransmission - 1;
							flag = flag || num3 != 15;
							num2 |= 0x100 | ((num3 >= 0) ? num3 : 0);
						}
						else
						{
							num2 &= -257;
						}
						_blocks[num] = Block.SetTorchLightLevel(num2, type.SelfIllumination);
						if (a.Y < regionMin.Y && flag2 == Block.IsSky(num2))
						{
							break;
						}
						a.Y--;
						num--;
					}
					a.X++;
				}
				a.Z++;
			}
		}

		protected void ApplyDelta(BuildTaskData data)
		{
			IntVector3 intVector = IntVector3.Subtract(data._intVec0, _worldMin);
			if (!IsIndexValid(intVector) || _chunks[data._intData0]._delta == null)
			{
				return;
			}
			int[] delta = _chunks[data._intData0]._delta;
			_chunks[data._intData0]._delta = null;
			for (int i = 0; i < delta.Length; i++)
			{
				IntVector3 a = IntVector3.Add(DeltaEntry.GetVector(delta[i]), intVector);
				BlockTypeEnum blockType = DeltaEntry.GetBlockType(delta[i]);
				int num = MakeIndex(a);
				if (Block.GetType(_blocks[num]).SpawnEntity)
				{
					RemoveItemBlockEntity(Block.GetTypeIndex(_blocks[num]), IntVector3.Add(a, _worldMin));
				}
				if (BlockType.GetType(blockType).SpawnEntity)
				{
					CreateItemBlockEntity(blockType, IntVector3.Add(a, _worldMin));
				}
				_blocks[num] = Block.SetType(0, blockType);
			}
		}

		public void DoThreadedComputeBlocks(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			BuildTaskData buildTaskData = (BuildTaskData)context;
			if (!_resetRequested && !buildTaskData._skipProcessing)
			{
				IntVector3 intVector = IntVector3.Subtract(buildTaskData._intVec0, _worldMin);
				if (IsIndexValid(intVector))
				{
					intVector.Y = 0;
					IntVector3 intVector2 = IntVector3.Add(new IntVector3(15, 127, 15), intVector);
					if (IsIndexValid(intVector2))
					{
						FillRegion(intVector, intVector2, BlockTypeEnum.NumberOfBlocks);
						_worldBuilder.BuildWorldChunk(this, buildTaskData._intVec0);
						ApplyDelta(buildTaskData);
						ApplyModListDuringCreate(buildTaskData._intData0);
						ReplaceRegion(intVector, intVector2, BlockTypeEnum.NumberOfBlocks, BlockTypeEnum.Empty);
						SetSkyAndEmitterLightingForChunk(buildTaskData._intVec0);
					}
				}
				AddToLoadingProgress(1);
			}
			buildTaskData.Release();
		}

		public void DoThreadedComputeGeometry(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			BuildTaskData buildTaskData = (BuildTaskData)context;
			if (!_resetRequested && !buildTaskData._skipProcessing)
			{
				if (_chunks[buildTaskData._intData0]._action >= NextChunkAction.NEEDS_GEOMETRY)
				{
					IntVector3 intVector = IntVector3.Subtract(buildTaskData._intVec0, _worldMin);
					if (intVector.X >= 0 && intVector.X < 384 && intVector.Z >= 0 && intVector.Z < 384)
					{
						RenderChunk renderChunk = RenderChunk.Alloc();
						renderChunk._worldMin = buildTaskData._intVec0;
						renderChunk._basePosition = IntVector3.ToVector3(renderChunk._worldMin);
						renderChunk.BuildFaces(_graphicsDevice);
						_chunks[buildTaskData._intData0].ReplaceChunk(renderChunk);
					}
					_chunks[buildTaskData._intData0]._action = NextChunkAction.NONE;
				}
				_chunks[buildTaskData._intData0]._numUsers.Decrement();
				AddToLoadingProgress(1);
			}
			buildTaskData.Release();
		}

		public void DoThreadedComputeLighting(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			BuildTaskData buildTaskData = (BuildTaskData)context;
			if (!_resetRequested && !buildTaskData._skipProcessing)
			{
				if (_chunks[buildTaskData._intData0]._action >= NextChunkAction.NEEDS_LIGHTING)
				{
					IntVector3 intVector = IntVector3.Subtract(buildTaskData._intVec0, _worldMin);
					intVector.Y = 0;
					IntVector3 regionMax = IntVector3.Add(new IntVector3(15, 127, 15), intVector);
					ComputeFirstPassLightForRegion(intVector, regionMax, _mainLightingPool);
					_computeGeometryPool.Add(buildTaskData._intData0);
				}
				else
				{
					_chunks[buildTaskData._intData0]._numUsers.Decrement();
					AddToLoadingProgress(1);
				}
			}
			buildTaskData.Release();
		}

		public void ComputeFirstPassLightForRegion(IntVector3 regionMin, IntVector3 regionMax, LightingPool lightingPool)
		{
			IntVector3 a = regionMin;
			if (!IsIndexValid(regionMin) || !IsIndexValid(regionMax))
			{
				return;
			}
			a.Z = regionMin.Z;
			while (a.Z <= regionMax.Z)
			{
				a.X = regionMin.X;
				while (a.X <= regionMax.X)
				{
					a.Y = regionMin.Y;
					int num = MakeIndex(a);
					while (a.Y <= regionMax.Y)
					{
						int blockAt = GetBlockAt(a);
						if (Block.IsLit(blockAt))
						{
							int num2 = ((Block.GetTorchLightLevel(blockAt) != 0) ? (-2147482112) : (-2147481856));
							for (int i = 0; i < 6; i++)
							{
								IntVector3 a2 = IntVector3.Add(a, _faceNeighbors[i]);
								if (IsIndexValid(a2))
								{
									int num3 = MakeIndex(a2);
									int num4 = _blocks[num3];
									if ((num4 & num2) == 0 && Interlocked.CompareExchange(ref _blocks[num3], Block.IsInList(num4, true), num4) == num4)
									{
										lightingPool.Add(num3);
									}
								}
							}
						}
						a.Y++;
						num++;
					}
					a.X++;
				}
				a.Z++;
			}
			a = regionMin;
			a.Z--;
			if (a.Z >= 0)
			{
				a.Y = 0;
				int num5 = MakeIndex(a);
				int num6 = regionMin.X;
				while (num6 <= regionMax.X)
				{
					a.Y = regionMin.Y;
					while (a.Y <= regionMax.Y)
					{
						if (Block.NeedToLightNewNeighbors(_blocks[num5]))
						{
							int num2 = ((Block.GetTorchLightLevel(_blocks[num5]) != 0) ? (-2147482112) : (-2147481856));
							int num7 = num5 + 49152;
							int num8 = _blocks[num7];
							if ((num8 & num2) == 0 && Interlocked.CompareExchange(ref _blocks[num7], Block.IsInList(num8, true), num8) == num8)
							{
								lightingPool.Add(num7);
							}
						}
						a.Y++;
						num5++;
					}
					num6++;
					a.X++;
				}
			}
			a = regionMin;
			a.Z += 16;
			if (a.Z < 384)
			{
				a.Y = regionMin.Y;
				int num9 = MakeIndex(a);
				int num10 = regionMin.X;
				while (num10 <= regionMax.X)
				{
					a.Y = regionMin.Y;
					while (a.Y <= regionMax.Y)
					{
						if (Block.NeedToLightNewNeighbors(_blocks[num9]))
						{
							int num2 = ((Block.GetTorchLightLevel(_blocks[num9]) != 0) ? (-2147482112) : (-2147481856));
							int num11 = num9 - 49152;
							int num12 = _blocks[num11];
							if ((num12 & num2) == 0 && Interlocked.CompareExchange(ref _blocks[num11], Block.IsInList(num12, true), num12) == num12)
							{
								lightingPool.Add(num11);
							}
						}
						a.Y++;
						num9++;
					}
					num10++;
					a.X++;
				}
			}
			a = regionMin;
			a.X--;
			if (a.X >= 0)
			{
				int num13 = regionMin.Z;
				while (num13 <= regionMax.Z)
				{
					a.Y = regionMin.Y;
					a.Z = num13;
					int num14 = MakeIndex(a);
					while (a.Y <= regionMax.Y)
					{
						if (Block.NeedToLightNewNeighbors(_blocks[num14]))
						{
							int num2 = ((Block.GetTorchLightLevel(_blocks[num14]) != 0) ? (-2147482112) : (-2147481856));
							int num15 = num14 + 128;
							int num16 = _blocks[num15];
							if ((num16 & num2) == 0 && Interlocked.CompareExchange(ref _blocks[num15], Block.IsInList(num16, true), num16) == num16)
							{
								lightingPool.Add(num15);
							}
						}
						num14++;
						a.Y++;
					}
					num13++;
					a.Z++;
				}
			}
			a = regionMin;
			a.X++;
			if (a.X >= 384)
			{
				return;
			}
			int num17 = regionMin.Z;
			while (num17 <= regionMax.Z)
			{
				a.Y = regionMin.Y;
				a.Z = num17;
				int num18 = MakeIndex(a);
				while (a.Y <= regionMax.Y)
				{
					if (Block.NeedToLightNewNeighbors(_blocks[num18]))
					{
						int num2 = ((Block.GetTorchLightLevel(_blocks[num18]) != 0) ? (-2147482112) : (-2147481856));
						int num19 = num18 - 128;
						int num20 = _blocks[num19];
						if ((num20 & num2) == 0 && Interlocked.CompareExchange(ref _blocks[num19], Block.IsInList(num20, true), num20) == num20)
						{
							lightingPool.Add(num19);
						}
					}
					num18++;
					a.Y++;
				}
				num17++;
				a.Z++;
			}
		}

		public void FillLighting(LightingPool lightPool)
		{
			while (!lightPool.Empty && !_resetRequested)
			{
				AddToLoadingProgress(1);
				int[] list;
				int count;
				int[] neighbors;
				lightPool.GetList(out list, out count, out neighbors);
				maxLightNodes = Math.Max(maxLightNodes, count);
				for (int i = 0; i < count; i++)
				{
					if (_resetRequested)
					{
						return;
					}
					int num = list[i];
					IntVector3 value = MakeIndexVectorFromIndex(num);
					bool flag = IndexVectorIsOnEdge(value);
					int data = _blocks[num] & -1025;
					if (Block.IsUninitialized(data))
					{
						continue;
					}
					BlockType type = Block.GetType(data);
					int lighting = Block.GetLighting(data);
					int num2 = 0;
					int num3 = 0;
					if (!flag)
					{
						for (int j = 0; j < 6; j++)
						{
							int num4 = num + _faceIndexNeighbors[j];
							if ((_blocks[num4] & int.MinValue) == 0)
							{
								neighbors[num3++] = num4;
							}
						}
					}
					else
					{
						for (int k = 0; k < 6; k++)
						{
							IntVector3 a = IntVector3.Add(value, _faceNeighbors[k]);
							if (IsIndexValid(a))
							{
								int num5 = MakeIndex(a);
								if ((_blocks[num5] & int.MinValue) == 0)
								{
									neighbors[num3++] = num5;
								}
							}
						}
					}
					if (Block.IsSky(data))
					{
						for (int l = 0; l < num3; l++)
						{
							num2 = Math.Max(num2, Block.GetTorchLightLevel(_blocks[neighbors[l]]));
						}
					}
					else
					{
						int num6 = int.MinValue;
						for (int m = 0; m < num3; m++)
						{
							int data2 = _blocks[neighbors[m]];
							num6 = Math.Max(num6, Block.GetSunLightLevel(data2));
							num2 = Math.Max(num2, Block.GetTorchLightLevel(data2));
						}
						data = Block.SetSunLightLevel(data, type.TransmitLight(num6));
					}
					data = Block.SetTorchLightLevel(data, Math.Max(type.SelfIllumination, type.TransmitLight(num2)));
					_blocks[num] = data;
					if (Block.GetLighting(lighting ^ data) == 0)
					{
						continue;
					}
					lightPool.UpdateMinAABB(ref value);
					for (int n = 0; n < num3; n++)
					{
						int num7 = neighbors[n];
						int num8 = _blocks[num7];
						if ((num8 & 0x600) == 0)
						{
							_blocks[num7] = num8 | 0x400;
							lightPool.Add(num7);
						}
					}
				}
			}
		}

		public void DoThreadedStepUpdateChunks(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			if (!_resetRequested)
			{
				if (!_mainLightingPool.Empty)
				{
					FillLighting(_mainLightingPool);
				}
				if (!_computeGeometryPool.Empty && !_resetRequested)
				{
					_computeGeometryPool.Drain();
				}
				else if (!_computeLightingPool.Empty && !_resetRequested)
				{
					_computeLightingPool.Drain();
				}
			}
			DecrementBuildTasks();
		}

		public void DoThreadedShiftTerrain(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			ShiftingTerrainData shiftingTerrainData = (ShiftingTerrainData)context;
			Buffer.BlockCopy(_blocks, shiftingTerrainData.source, _shiftedBlocks, shiftingTerrainData.dest, shiftingTerrainData.length);
			int fillStart = shiftingTerrainData.fillStart;
			int fillLength = shiftingTerrainData.fillLength;
			for (int i = 0; i < fillLength; i++)
			{
				_shiftedBlocks[fillStart++] = initblock;
			}
			_shiftTerrainData.done = true;
			_updateTasksRemaining.Decrement();
		}

		public bool FinishShiftTerrain()
		{
			if (_shiftTerrainData.running && _shiftTerrainData.done)
			{
				_shiftTerrainData.running = false;
				int dx = _shiftTerrainData.dx;
				int dz = _shiftTerrainData.dz;
				_worldMin.X += dx * 16;
				_worldMin.Z += dz * 16;
				_currentEyeChunkIndex -= dx + dz * 24;
				int num = -dz * 16 * 128 * 384 + -dx * 16 * 128;
				int num2 = 18874368 - Math.Abs(num);
				bool flag = num > 0;
				int[] blocks = _blocks;
				_blocks = _shiftedBlocks;
				_shiftedBlocks = blocks;
				num = -dz * 24 + -dx;
				num2 = 576 - Math.Abs(num);
				int num3 = num;
				int num4;
				int num5;
				if (!flag)
				{
					num = -num;
					num4 = num;
					num5 = 1;
					for (int i = 0; i < num; i++)
					{
						_chunks[i]._chunk.Release();
					}
				}
				else
				{
					num4 = 576 - num - 1;
					num5 = -1;
					for (int j = 0; j < num; j++)
					{
						_chunks[576 - j - 1]._chunk.Release();
					}
				}
				for (int k = 0; k < num2; k++)
				{
					_chunks[num4 + num3].SwapIn(_chunks[num4]);
					num4 += num5;
				}
				if (dx < 0)
				{
					int num6 = 0;
					for (int l = 0; l < 24; l++)
					{
						num6 = l * 24;
						int num7 = 0;
						while (num7 < -dx)
						{
							_chunks[num6].Reset();
							num7++;
							num6++;
						}
					}
				}
				else if (dx > 0)
				{
					int num8 = 0;
					for (int m = 0; m < 24; m++)
					{
						num8 = m * 24 + 24 - 1;
						int num9 = 0;
						while (num9 < dx)
						{
							_chunks[num8].Reset();
							num9++;
							num8--;
						}
					}
				}
				if (dz < 0)
				{
					int num10 = 0;
					for (int n = 0; n < -dz; n++)
					{
						num10 = n * 24;
						int num11 = 0;
						while (num11 < 24)
						{
							if (_chunks[num10]._action != NextChunkAction.COMPUTING_BLOCKS)
							{
								_chunks[num10].Reset();
							}
							num11++;
							num10++;
						}
					}
				}
				else if (dz > 0)
				{
					int num12 = 0;
					for (int num13 = 0; num13 < dz; num13++)
					{
						num12 = (23 - num13) * 24;
						int num14 = 0;
						while (num14 < 24)
						{
							if (_chunks[num12]._action != NextChunkAction.COMPUTING_BLOCKS)
							{
								_chunks[num12].Reset();
							}
							num14++;
							num12++;
						}
					}
				}
				_allChunksLoaded = false;
				return false;
			}
			return true;
		}

		public bool ShiftTerrain(int desireddx, int desireddz)
		{
			if (!_shiftTerrainData.running)
			{
				_shiftTerrainData.dx = desireddx;
				_shiftTerrainData.dz = desireddz;
				_shiftTerrainData.running = true;
				_shiftTerrainData.done = false;
				int num = -desireddz * 16 * 128 * 384 + -desireddx * 16 * 128;
				_shiftTerrainData.fillLength = Math.Abs(num);
				int num2 = 18874368 - _shiftTerrainData.fillLength;
				if (num <= 0)
				{
					_shiftTerrainData.source = -num * 4;
					_shiftTerrainData.dest = 0;
					_shiftTerrainData.fillStart = num2;
				}
				else
				{
					_shiftTerrainData.source = 0;
					_shiftTerrainData.dest = num * 4;
					_shiftTerrainData.fillStart = 0;
				}
				_shiftTerrainData.length = num2 * 4;
				TaskDispatcher.Instance.AddRushTask(_shiftTerrainDelegate, _shiftTerrainData);
				_updateTasksRemaining.Increment();
				return true;
			}
			return false;
		}

		public void FillRegion(IntVector3 min, IntVector3 max, BlockTypeEnum blockType)
		{
			int num = Block.SetType(0, blockType);
			for (int i = min.Z; i <= max.Z; i++)
			{
				for (int j = min.X; j <= max.X; j++)
				{
					int num2 = MakeIndex(j, min.Y, i);
					int num3 = min.Y;
					while (num3 <= max.Y)
					{
						_blocks[num2] = num;
						num3++;
						num2++;
					}
				}
			}
		}

		public void ReplaceRegion(IntVector3 min, IntVector3 max, BlockTypeEnum replaceme, BlockTypeEnum withme)
		{
			for (int i = min.Z; i <= max.Z; i++)
			{
				for (int j = min.X; j <= max.X; j++)
				{
					int num = MakeIndex(j, min.Y, i);
					int num2 = min.Y;
					while (num2 <= max.Y)
					{
						if (Block.GetTypeIndex(_blocks[num]) == replaceme)
						{
							_blocks[num] = Block.SetType(_blocks[num], withme);
						}
						num2++;
						num++;
					}
				}
			}
		}

		public void DoFinishThreadedRegionOperation(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			_updateTasksRemaining.Decrement();
		}

		public void AddBlockToLightList(IntVector3 block, LightingPool lightPool)
		{
			IntVector3 intVector = IntVector3.Min(IntVector3.Add(block, new IntVector3(1, 1, 1)), new IntVector3(383, 127, 383));
			IntVector3 intVector2 = IntVector3.Max(IntVector3.Subtract(block, new IntVector3(1, 1, 1)), IntVector3.Zero);
			int num = -1;
			if (intVector2.Y > 0)
			{
				num = intVector2.Y - 1;
			}
			for (int i = intVector2.Z; i <= intVector.Z; i++)
			{
				for (int j = intVector2.X; j <= intVector.X; j++)
				{
					int num2 = MakeIndex(j, intVector2.Y, i);
					int num3 = intVector2.Y;
					while (num3 <= intVector.Y)
					{
						if (!Block.IsUninitialized(_blocks[num2]))
						{
							lightPool.Add(num2);
							_blocks[num2] |= 1024;
						}
						num3++;
						num2++;
					}
				}
			}
			if (num < 0)
			{
				return;
			}
			IntVector3 a = new IntVector3(block.X, num, block.Z);
			int num4 = MakeIndex(a);
			while (a.Y >= 0 && Block.IsSky(_blocks[num4]))
			{
				for (int k = 0; k < 6; k++)
				{
					if (k == 5)
					{
						continue;
					}
					IntVector3 a2 = IntVector3.Add(a, _faceNeighbors[k]);
					if (IsIndexValid(a2))
					{
						int num5 = MakeIndex(a2);
						int num6 = _blocks[num5];
						if ((num6 & -2147481856) == 0 && Interlocked.CompareExchange(ref _blocks[num5], Block.IsInList(num6, true), num6) == num6)
						{
							lightPool.Add(num5);
						}
					}
				}
				a.Y--;
				num4--;
			}
		}

		private void DoThreadedFinishSetBlock(TaskThreadEnum threadIndex, BaseTask task, object context)
		{
			BuildTaskData buildTaskData = (BuildTaskData)context;
			if (!_resetRequested)
			{
				FillLighting(_updateLightingPool);
				IntVector3 intVector = IntVector3.Min(IntVector3.Add(_updateLightingPool._max, new IntVector3(1, 1, 1)), new IntVector3(383, 127, 383));
				IntVector3 intVector2 = IntVector3.Max(IntVector3.Subtract(_updateLightingPool._min, new IntVector3(1, 1, 1)), IntVector3.Zero);
				intVector2.X /= 16;
				intVector2.Z /= 16;
				intVector.X /= 16;
				intVector.Z /= 16;
				GatherTask gatherTask = TaskDispatcher.Instance.AddGatherTask(_finishRegionOpDelegate, null);
				for (int i = intVector2.Z; i <= intVector.Z; i++)
				{
					for (int j = intVector2.X; j <= intVector.X; j++)
					{
						int num = i * 24 + j;
						BuildTaskData buildTaskData2 = BuildTaskData.Alloc();
						buildTaskData2._intVec0 = IntVector3.Add(_worldMin, MakeIndexVectorFromChunkIndex(num));
						buildTaskData2._intData0 = num;
						_chunks[num]._numUsers.Increment();
						gatherTask.AddTask(_computeGeometryDelegate, buildTaskData2);
					}
				}
				DecrChunkInUse(buildTaskData._intData0);
				gatherTask.StartNow();
			}
			buildTaskData.Release();
		}

		public bool OkayToBuildHere(IntVector3 b)
		{
			b.Y -= _worldMin.Y;
			if (b.Y > 0)
			{
				return b.Y < 127;
			}
			return false;
		}

		private void FillFacePlanes(IntVector3 final)
		{
			_facePlanes[0].D = -final.X - 1;
			_facePlanes[1].D = final.Z;
			_facePlanes[2].D = final.X;
			_facePlanes[3].D = -final.Z - 1;
			_facePlanes[4].D = -final.Y - 1;
			_facePlanes[5].D = final.Y;
		}

		public bool ProbeTouchesBlock(TraceProbe probe, IntVector3 pos)
		{
			FillFacePlanes(pos);
			probe.TestShape(_facePlanes, pos);
			return probe._collides;
		}

		private void BruteForceTrace(TraceProbe tp)
		{
			IntVector3 intVector = MakeIndexVectorFromPosition(tp._bounds.Min);
			IntVector3 b = MakeIndexVectorFromPosition(tp._bounds.Max);
			intVector.SetToMax(IntVector3.Zero);
			b.SetToMin(new IntVector3(383, 127, 383));
			IntVector3 intVector2 = IntVector3.Subtract(intVector, b);
			if (intVector2.X * intVector2.Y * intVector2.Z >= 27)
			{
				return;
			}
			IntVector3 intVector3 = intVector;
			intVector3.Z = intVector.Z;
			while (intVector3.Z <= b.Z)
			{
				intVector3.X = intVector.X;
				while (intVector3.X <= b.X)
				{
					intVector3.Y = intVector.Y;
					int num = MakeIndex(intVector3);
					IntVector3 intVector4 = IntVector3.Add(_worldMin, intVector3);
					while (intVector3.Y <= b.Y)
					{
						if (tp.TestThisType(Block.GetTypeIndex(_blocks[num])))
						{
							FillFacePlanes(intVector4);
							tp.TestShape(_facePlanes, intVector4);
						}
						intVector4.Y++;
						intVector3.Y++;
						num++;
					}
					intVector3.X++;
				}
				intVector3.Z++;
			}
		}

		public void Trace(TraceProbe tp)
		{
			if (tp is AABBTraceProbe)
			{
				BruteForceTrace(tp);
				return;
			}
			Vector3 vector = tp._end - tp._start;
			IntVector3 intVector = MakeIndexVectorFromPosition(tp._start);
			IntVector3 intVector2 = IntVector3.Add(_worldMin, intVector);
			IntVector3 zero = IntVector3.Zero;
			Vector3 zero2 = Vector3.Zero;
			Vector3 zero3 = Vector3.Zero;
			float num = 0f;
			if (vector.X == 0f)
			{
				zero2.X = 2f;
				zero3.X = 0f;
			}
			else
			{
				zero3.X = Math.Abs(1f / vector.X);
				float num2 = intVector2.X;
				if (vector.X > 0f)
				{
					num2 += 1f;
					zero.X = 1;
				}
				else
				{
					zero.X = -1;
				}
				zero2.X = (num2 - tp._start.X) / vector.X;
			}
			if (vector.Y == 0f)
			{
				zero2.Y = 2f;
				zero3.Y = 0f;
			}
			else
			{
				zero3.Y = Math.Abs(1f / vector.Y);
				float num2 = intVector2.Y;
				if (vector.Y > 0f)
				{
					num2 += 1f;
					zero.Y = 1;
				}
				else
				{
					zero.Y = -1;
				}
				zero2.Y = (num2 - tp._start.Y) / vector.Y;
			}
			if (vector.Z == 0f)
			{
				zero2.Z = 2f;
				zero3.Z = 0f;
			}
			else
			{
				zero3.Z = Math.Abs(1f / vector.Z);
				float num2 = intVector2.Z;
				if (vector.Z > 0f)
				{
					num2 += 1f;
					zero.Z = 1;
				}
				else
				{
					zero.Z = -1;
				}
				zero2.Z = (num2 - tp._start.Z) / vector.Z;
			}
			while (true)
			{
				if (IsIndexValid(intVector))
				{
					int num3 = MakeIndex(intVector);
					BlockTypeEnum typeIndex = Block.GetTypeIndex(_blocks[num3]);
					if (tp.TestThisType(typeIndex))
					{
						FillFacePlanes(intVector2);
						if (!tp.TestShape(_facePlanes, intVector2, typeIndex))
						{
							break;
						}
					}
				}
				if (num >= 1f)
				{
					break;
				}
				if (zero2.X < zero2.Y)
				{
					if (zero2.X < zero2.Z)
					{
						num = zero2.X;
						zero2.X += zero3.X;
						intVector.X += zero.X;
						intVector2.X += zero.X;
					}
					else
					{
						num = zero2.Z;
						zero2.Z += zero3.Z;
						intVector.Z += zero.Z;
						intVector2.Z += zero.Z;
					}
				}
				else if (zero2.Y < zero2.Z)
				{
					num = zero2.Y;
					zero2.Y += zero3.Y;
					intVector.Y += zero.Y;
					intVector2.Y += zero.Y;
				}
				else
				{
					num = zero2.Z;
					zero2.Z += zero3.Z;
					intVector.Z += zero.Z;
					intVector2.Z += zero.Z;
				}
			}
		}

		public FallLockTestResult FallLockFace(Vector3 v, BlockFace f)
		{
			return FallLockFace(IntVector3.FromVector3(v), f);
		}

		public FallLockTestResult FallLockFace(IntVector3 v, BlockFace f)
		{
			IntVector3 a = IntVector3.Subtract(v, _worldMin);
			if (IsIndexValid(a))
			{
				int num = MakeIndex(a);
				BlockType type = Block.GetType(_blocks[num]);
				if (type.BlockPlayer)
				{
					IntVector3 neighborIndex = GetNeighborIndex(a, f);
					if (IsIndexValid(neighborIndex))
					{
						int num2 = MakeIndex(neighborIndex);
						type = Block.GetType(_blocks[num2]);
						if (!type.BlockPlayer)
						{
							return FallLockTestResult.SOLID_BLOCK_NEEDS_WALL;
						}
						return FallLockTestResult.SOLID_BLOCK_NO_WALL;
					}
					return FallLockTestResult.SOLID_BLOCK_NEEDS_WALL;
				}
			}
			return FallLockTestResult.EMPTY_BLOCK;
		}

		public int DepthUnderGround(Vector3 location)
		{
			IntVector3 a = MakeIndexVectorFromPosition(location);
			if (!IsIndexValid(a))
			{
				return 0;
			}
			int num = 0;
			for (int i = a.Y; i < 128; i++)
			{
				a.Y = i;
				int num2 = MakeIndex(a);
				if (Block.GetType(_blocks[num2]).BlockPlayer)
				{
					num++;
				}
			}
			return num;
		}

		public float DepthUnderWater(Vector3 location)
		{
			return Math.Max(WaterLevel - location.Y, 0f);
		}

		public Vector3 FindClosestCeiling(Vector3 guess)
		{
			guess.X = (float)Math.Floor(guess.X) + 0.1f;
			guess.Y = (float)Math.Floor(guess.Y) + 0.1f;
			guess.Z = (float)Math.Floor(guess.Z) + 0.1f;
			IntVector3 intVector = MakeIndexVectorFromPosition(guess);
			intVector.X = intVector.X.Clamp(0, 383);
			intVector.Y = intVector.Y.Clamp(0, 127);
			intVector.Z = intVector.Z.Clamp(0, 383);
			int num = MakeIndex(intVector);
			IntVector3 b = IntVector3.Zero;
			IntVector3 intVector2 = intVector;
			while (intVector2.Y < 128 && Block.GetType(_blocks[num]).BlockPlayer)
			{
				num++;
				intVector2.Y++;
			}
			while (intVector2.Y < 128)
			{
				if (!Block.GetType(_blocks[num]).BlockPlayer)
				{
					num++;
					intVector2.Y++;
					continue;
				}
				b = intVector2;
				break;
			}
			if (b.Y != 0)
			{
				b = IntVector3.Add(_worldMin, b);
				Vector3 result = IntVector3.ToVector3(b);
				result.X += 0.5f;
				result.Y += 0.1f;
				result.Z += 0.5f;
				return result;
			}
			return Vector3.Zero;
		}

		public Vector3 FindSafeStartLocation(Vector3 guess)
		{
			guess.X = (float)Math.Floor(guess.X) + 0.1f;
			guess.Y = (float)Math.Floor(guess.Y) + 0.1f;
			guess.Z = (float)Math.Floor(guess.Z) + 0.1f;
			IntVector3 intVector = MakeIndexVectorFromPosition(guess);
			intVector.X = intVector.X.Clamp(0, 383);
			intVector.Y = intVector.Y.Clamp(0, 127);
			intVector.Z = intVector.Z.Clamp(0, 383);
			int num = -1;
			int num2 = 1;
			int num3 = (IsWaterWorld ? ((int)Math.Floor(64f + WaterLevel)) : (-1));
			do
			{
				for (int i = intVector.X; i < intVector.X + num2; i++)
				{
					if (i < 0)
					{
						continue;
					}
					if (i >= 384)
					{
						break;
					}
					for (int j = intVector.Z; j < intVector.Z + num2; j++)
					{
						if (j < 0)
						{
							continue;
						}
						if (j >= 384)
						{
							break;
						}
						bool flag = false;
						if (i == intVector.X || i == intVector.X + num2 - 1)
						{
							flag = true;
						}
						else if (j == intVector.Z || j == intVector.Z + num2 - 1)
						{
							flag = true;
						}
						if (!flag)
						{
							continue;
						}
						IntVector3 intVector2 = new IntVector3(i, intVector.Y, j);
						int num4 = MakeIndex(intVector2);
						if (Block.GetType(_blocks[num4]).BlockPlayer)
						{
							while (intVector2.Y < 127 && Block.GetType(_blocks[num4]).BlockPlayer)
							{
								intVector2.Y++;
								num4++;
							}
						}
						else
						{
							while (intVector2.Y > 1)
							{
								if (Block.GetType(_blocks[num4]).BlockPlayer)
								{
									intVector2.Y++;
									break;
								}
								intVector2.Y--;
								num4--;
							}
						}
						IntVector3 a = intVector2;
						num4 = MakeIndex(a);
						while (a.Y < 128)
						{
							if (!Block.GetType(_blocks[num4]).BlockPlayer && (a.Y == 127 || !Block.GetType(_blocks[num4 + 1]).BlockPlayer))
							{
								num = a.Y;
								break;
							}
							a.Y++;
							num4++;
						}
						a = intVector2;
						a.Y--;
						num4 = MakeIndex(a);
						while (a.Y > 0)
						{
							if (!Block.GetType(_blocks[num4]).BlockPlayer && !Block.GetType(_blocks[num4 + 1]).BlockPlayer)
							{
								if (intVector2.Y - a.Y < num - intVector2.Y)
								{
									num = a.Y;
								}
								break;
							}
							a.Y--;
							num4--;
						}
						if (num >= num3)
						{
							Vector3 result = MakePositionFromIndexVector(new IntVector3(intVector2.X, num, intVector2.Z));
							result.X += 0.5f;
							result.Y += 0.1f;
							result.Z += 0.5f;
							return result;
						}
					}
				}
				intVector.X--;
				intVector.Z--;
				num2 += 2;
			}
			while (num2 <= 256);
			Vector3 result2 = MakePositionFromIndexVector(new IntVector3(intVector.X, 128, intVector.Z));
			result2.X += 0.5f;
			result2.Y += 0.1f;
			result2.Z += 0.5f;
			return result2;
		}

		public bool IsInsideWorld(Vector3 r)
		{
			if (!IsReady)
			{
				return false;
			}
			IntVector3 chunkVectorIndex = GetChunkVectorIndex(r);
			if (chunkVectorIndex.X >= 24 || chunkVectorIndex.X < 0 || chunkVectorIndex.Z >= 24 || chunkVectorIndex.Z < 0)
			{
				return false;
			}
			return true;
		}

		public bool RegionIsLoaded(Vector3 r)
		{
			if (!IsReady)
			{
				return false;
			}
			IntVector3 chunkVectorIndex = GetChunkVectorIndex(r);
			if (chunkVectorIndex.X >= 24 || chunkVectorIndex.X < 0 || chunkVectorIndex.Z >= 24 || chunkVectorIndex.Z < 0)
			{
				return false;
			}
			int num = chunkVectorIndex.X + chunkVectorIndex.Z * 24;
			RenderChunk chunk = _chunks[num].GetChunk();
			bool result = chunk != null && chunk.HasGeometry();
			chunk.Release();
			return result;
		}

		public Vector3 FindTopmostGroundLocation(Vector3 guess)
		{
			guess.X = (float)Math.Floor(guess.X) + 0.1f;
			guess.Y = (float)Math.Floor(guess.Y) + 0.1f;
			guess.Z = (float)Math.Floor(guess.Z) + 0.1f;
			IntVector3 intVector = MakeIndexVectorFromPosition(guess);
			intVector.X = intVector.X.Clamp(0, 383);
			intVector.Y = 127;
			intVector.Z = intVector.Z.Clamp(0, 383);
			int num = 1;
			int num2 = (IsWaterWorld ? ((int)Math.Floor(64f + WaterLevel)) : (-1));
			do
			{
				for (int i = intVector.X; i < intVector.X + num; i++)
				{
					if (i < 0)
					{
						continue;
					}
					if (i >= 384)
					{
						break;
					}
					for (int j = intVector.Z; j < intVector.Z + num; j++)
					{
						if (j < 0)
						{
							continue;
						}
						if (j >= 384)
						{
							break;
						}
						bool flag = false;
						if (i == intVector.X || i == intVector.X + num - 1)
						{
							flag = true;
						}
						else if (j == intVector.Z || j == intVector.Z + num - 1)
						{
							flag = true;
						}
						if (flag)
						{
							IntVector3 intVector2 = new IntVector3(i, 127, j);
							int num3 = MakeIndex(intVector2);
							while (intVector2.Y > 0 && !Block.GetType(_blocks[num3]).BlockPlayer)
							{
								intVector2.Y--;
								num3--;
							}
							intVector2.Y++;
							if (intVector2.Y >= num2)
							{
								Vector3 result = MakePositionFromIndexVector(intVector2);
								result.X += 0.5f;
								result.Y += 0.1f;
								result.Z += 0.5f;
								return result;
							}
						}
					}
				}
				intVector.X--;
				intVector.Z--;
				num += 2;
			}
			while (num <= 256);
			Vector3 result2 = MakePositionFromIndexVector(new IntVector3(intVector.X, 128, intVector.Z));
			result2.X += 0.5f;
			result2.Y += 0.1f;
			result2.Z += 0.5f;
			return result2;
		}

		public Vector3 ClipPositionToLoadedWorld(Vector3 pos, float radius)
		{
			if (IsReady)
			{
				Vector3 vector = IntVector3.ToVector3(_worldMin);
				pos -= vector;
				pos.X = pos.X.Clamp(radius, 384f - radius);
				pos.Z = pos.Z.Clamp(radius, 384f - radius);
				pos += vector;
			}
			return pos;
		}

		public void StepInitialization()
		{
			if ((bool)_updateTasksRemaining)
			{
				return;
			}
			IntVector3 a;
			if (!_buildTasksRemaining)
			{
				if (TryShiftTerrain())
				{
					return;
				}
				if (!_allChunksLoaded)
				{
					if (_loadingProgress != 0)
					{
						_loadingProgress = 183;
					}
					int num = 0;
					_allChunksLoaded = true;
					a = new IntVector3(_currentEyeChunkIndex % 24, 0, _currentEyeChunkIndex / 24);
					for (int i = 0; i < _radiusOrderOffsets.Length; i++)
					{
						IntVector3 intVector = IntVector3.Add(a, _radiusOrderOffsets[i]);
						if (intVector.X < 0 || intVector.X >= 24 || intVector.Z < 0 || intVector.Z >= 24)
						{
							continue;
						}
						int num2 = intVector.X + intVector.Z * 24;
						if (_chunks[num2]._action == NextChunkAction.WAITING_TO_LOAD)
						{
							_computeBlocksPool.Add(num2);
							AddSurroundingBlocksToLightList(num2);
							num++;
							if (num == _maxChunksAtOnce)
							{
								_maxChunksAtOnce = 8;
								_allChunksLoaded = false;
								break;
							}
						}
					}
					if (!_computeBlocksPool.Empty)
					{
						_computeBlocksPool.Drain();
					}
				}
			}
			a = new IntVector3(_currentEyeChunkIndex % 24, 0, _currentEyeChunkIndex / 24);
			for (int j = 0; j < _radiusOrderOffsets.Length; j++)
			{
				IntVector3 intVector2 = IntVector3.Add(a, _radiusOrderOffsets[j]);
				if (intVector2.X >= 0 && intVector2.X < 24 && intVector2.Z >= 0 && intVector2.Z < 24)
				{
					int num3 = intVector2.X + intVector2.Z * 24;
					if (_chunks[num3]._action == NextChunkAction.NONE && !_chunks[num3]._mods.Empty && !ChunkOrNeighborInUse(num3) && ApplyModList(num3))
					{
						IncrChunkInUse(num3);
						BuildTaskData buildTaskData = BuildTaskData.Alloc();
						buildTaskData._intData0 = num3;
						_updateTasksRemaining.Increment();
						TaskDispatcher.Instance.AddRushTask(_finishSetBlockDelegate, buildTaskData);
						break;
					}
				}
			}
		}

		protected void AddSurroundingBlocksToLightList(int index)
		{
			_computeLightingPool.Add(index);
			_chunks[index]._numUsers.Increment();
			IntVector3 intVector = MakeIndexVectorFromChunkIndex(index);
			for (int i = 0; i < 4; i++)
			{
				IntVector3 a = intVector;
				switch (i)
				{
				case 0:
					a.X += 16;
					break;
				case 1:
					a.X -= 16;
					break;
				case 2:
					a.Z += 16;
					break;
				case 3:
					a.Z -= 16;
					break;
				}
				if (IsIndexValid(a))
				{
					int num = MakeChunkIndexFromIndexVector(a);
					if (_chunks[num]._action >= NextChunkAction.NEEDS_GEOMETRY)
					{
						_computeLightingPool.Add(num);
						_chunks[num]._numUsers.Increment();
					}
				}
			}
		}

		protected void ApplyModListDuringCreate(int index)
		{
			SynchronizedQueue<PendingMod> mods = _chunks[index]._mods;
			while (!mods.Empty)
			{
				PendingMod pendingMod = mods.Dequeue();
				IntVector3 a = IntVector3.Subtract(pendingMod._worldPosition, _worldMin);
				int num = MakeIndex(a);
				if (Block.GetType(_blocks[num]).SpawnEntity)
				{
					RemoveItemBlockEntity(Block.GetTypeIndex(_blocks[num]), pendingMod._worldPosition);
				}
				if (BlockType.GetType(pendingMod._blockType).SpawnEntity)
				{
					CreateItemBlockEntity(pendingMod._blockType, pendingMod._worldPosition);
				}
				_blocks[num] = Block.SetType(0, pendingMod._blockType);
			}
		}

		protected bool ApplyModList(int index)
		{
			bool result = false;
			SynchronizedQueue<PendingMod> mods = _chunks[index]._mods;
			_updateLightingPool.Clear();
			_updateLightingPool.ResetAABB();
			while (!mods.Empty)
			{
				PendingMod pendingMod = mods.Dequeue();
				IntVector3 value = IntVector3.Subtract(pendingMod._worldPosition, _worldMin);
				int num = MakeIndex(value);
				if (Block.GetTypeIndex(_blocks[num]) != pendingMod._blockType)
				{
					if (Block.GetType(_blocks[num]).SpawnEntity)
					{
						RemoveItemBlockEntity(Block.GetTypeIndex(_blocks[num]), pendingMod._worldPosition);
					}
					if (BlockType.GetType(pendingMod._blockType).SpawnEntity)
					{
						CreateItemBlockEntity(pendingMod._blockType, pendingMod._worldPosition);
					}
					_blocks[num] = Block.SetType(0, pendingMod._blockType);
					_updateLightingPool.UpdateMinAABB(ref value);
					ResetSkyAndEmitterLightingForRegion(value, value);
					AddBlockToLightList(value, _updateLightingPool);
					result = true;
				}
			}
			return result;
		}

		protected void IncrChunkInUse(int index)
		{
			IntVector3 intVector = MakeIndexVectorFromChunkIndex(index);
			for (int i = -2; i < 3; i++)
			{
				IntVector3 a = intVector;
				a.X += i * 16;
				for (int j = -2; j < 3; j++)
				{
					a.Z += j * 128;
					if (IsIndexValid(a))
					{
						int num = MakeChunkIndexFromIndexVector(a);
						if (_chunks[num]._action > NextChunkAction.WAITING_TO_LOAD)
						{
							_chunks[num]._numUsers.Increment();
						}
					}
				}
			}
		}

		protected void DecrChunkInUse(int index)
		{
			IntVector3 intVector = MakeIndexVectorFromChunkIndex(index);
			for (int i = -2; i < 3; i++)
			{
				IntVector3 a = intVector;
				a.X += i * 16;
				for (int j = -2; j < 3; j++)
				{
					a.Z += j * 128;
					if (IsIndexValid(a))
					{
						int num = MakeChunkIndexFromIndexVector(a);
						if (_chunks[num]._action > NextChunkAction.WAITING_TO_LOAD)
						{
							_chunks[num]._numUsers.Decrement();
						}
					}
				}
			}
		}

		protected bool ChunkOrNeighborInUse(int index)
		{
			IntVector3 intVector = MakeIndexVectorFromChunkIndex(index);
			for (int i = -2; i < 3; i++)
			{
				IntVector3 a = intVector;
				a.X += i * 16;
				for (int j = -2; j < 3; j++)
				{
					a.Z += j * 128;
					if (IsIndexValid(a))
					{
						int num = MakeChunkIndexFromIndexVector(a);
						if ((bool)_chunks[num]._numUsers)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void GlobalUpdate(GameTime gameTime)
		{
			if (IsReady)
			{
				StepInitialization();
				ChunkCache.Instance.Update(gameTime);
			}
			else
			{
				if (_asyncInitData == null || !ReadyForInit)
				{
					return;
				}
				if (_asyncInitData.teleporting)
				{
					InternalTeleport(_asyncInitData.center);
				}
				else
				{
					Init(_asyncInitData.center, _asyncInitData.worldInfo, _asyncInitData.host);
					if (_asyncInitData.callback != null)
					{
						_asyncInitData.callback(null);
					}
				}
				_asyncInitData = null;
			}
		}

		public bool TryShiftTerrain()
		{
			if (_shiftTerrainData.running)
			{
				return FinishShiftTerrain();
			}
			IntVector3 intVector = new IntVector3(_currentEyeChunkIndex % 24, 0, _currentEyeChunkIndex / 24);
			intVector.X -= 12;
			intVector.Z -= 12;
			if (intVector.X < 0)
			{
				intVector.X++;
			}
			if (intVector.Z < 0)
			{
				intVector.Z++;
			}
			if (intVector.X != 0 || intVector.Z != 0)
			{
				return ShiftTerrain(intVector.X, intVector.Z);
			}
			return false;
		}

		public void CenterOn(Vector3 eye, bool scrollIfPossible)
		{
			if (!IsReady)
			{
				return;
			}
			IntVector3 chunkVectorIndex = GetChunkVectorIndex(eye);
			if (scrollIfPossible)
			{
				bool flag = false;
				if (chunkVectorIndex.X < 0 || chunkVectorIndex.X >= 24 || chunkVectorIndex.Z < 0 || chunkVectorIndex.Z >= 24)
				{
					flag = true;
				}
				else
				{
					IntVector3 intVector = IntVector3.Subtract(b: new IntVector3(_currentEyeChunkIndex % 24, 0, _currentEyeChunkIndex / 24), a: chunkVectorIndex);
					if (Math.Abs(intVector.X) > 1 || Math.Abs(intVector.Z) > 1)
					{
						int num = 0;
						for (int i = 0; i < 25; i++)
						{
							IntVector3 intVector2 = IntVector3.Add(chunkVectorIndex, _radiusOrderOffsets[i]);
							if (intVector2.X >= 0 && intVector2.X < 24 && intVector2.Z >= 0 && intVector2.Z < 24)
							{
								int num2 = intVector2.X + intVector2.Z * 24;
								if (_chunks[num2]._action != 0)
								{
									num++;
								}
							}
						}
						if (num < 25)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					Teleport(eye);
					return;
				}
			}
			chunkVectorIndex.X = chunkVectorIndex.X.Clamp(0, 23);
			chunkVectorIndex.Z = chunkVectorIndex.Z.Clamp(0, 23);
			Interlocked.Exchange(ref _currentEyeChunkIndex, chunkVectorIndex.X + chunkVectorIndex.Z * 24);
			IntVector3 intVector3 = chunkVectorIndex;
			intVector3.X = (intVector3.X - 12).Clamp(-1, 0);
			intVector3.Z = (intVector3.Z - 12).Clamp(-1, 0);
			Interlocked.Exchange(ref _currentRenderOrder, Math.Abs(intVector3.Z * 2 + intVector3.X));
		}

		public void SetCursor(bool draw, IntVector3 worldIndex, BlockFace face)
		{
			if (IsReady)
			{
				_drawCursor = draw;
				if (draw)
				{
					_cursorPosition = worldIndex;
					_cursorFace = face;
				}
			}
		}

		public void SetCursor(bool draw, Vector3 position, Vector3 normal)
		{
			if (!IsReady)
			{
				return;
			}
			_drawCursor = draw;
			if (!draw)
			{
				return;
			}
			_cursorPosition = IntVector3.Add(_worldMin, MakeIndexVectorFromPosition(position));
			Vector3 vector = new Vector3(Math.Abs(normal.X), Math.Abs(normal.Y), Math.Abs(normal.Z));
			if (vector.X > vector.Y)
			{
				if (vector.X > vector.Z)
				{
					if (normal.X > 0f)
					{
						_cursorFace = BlockFace.POSX;
					}
					else
					{
						_cursorFace = BlockFace.NEGX;
					}
				}
				else if (normal.Z > 0f)
				{
					_cursorFace = BlockFace.POSZ;
				}
				else
				{
					_cursorFace = BlockFace.NEGZ;
				}
			}
			else if (vector.Y > vector.Z)
			{
				if (normal.Y > 0f)
				{
					_cursorFace = BlockFace.POSY;
				}
				else
				{
					_cursorFace = BlockFace.NEGY;
				}
			}
			else if (normal.Z > 0f)
			{
				_cursorFace = BlockFace.POSZ;
			}
			else
			{
				_cursorFace = BlockFace.NEGZ;
			}
		}

		public void GetAvatarColor(Vector3 position, out Vector3 ambient, out Vector3 directional, out Vector3 direction)
		{
			Vector2 lightAtPoint = GetLightAtPoint(position);
			float y = lightAtPoint.Y;
			float x = lightAtPoint.X;
			direction = -VectorToSun;
			ambient = Vector3.Multiply(AmbientSunColor.ToVector3(), x) + Vector3.Multiply(TorchColor.ToVector3(), y * (1f - x * SunlightColor.ToVector3().Y));
			directional = Vector3.Multiply(SunlightColor.ToVector3(), (float)Math.Pow(x, 30.0));
		}

		public void GetEnemyLighting(Vector3 position, ref Vector3 l1d, ref Vector3 l1c, ref Vector3 l2d, ref Vector3 l2c, ref Vector3 ambient)
		{
			Vector2 lightAtPoint = GetLightAtPoint(position);
			float y = lightAtPoint.Y;
			float x = lightAtPoint.X;
			ambient = Vector3.Multiply(AmbientSunColor.ToVector3(), x) + Vector3.Multiply(TorchColor.ToVector3(), 0.5f * y * (1f - x * SunlightColor.ToVector3().Y));
			l1d = Vector3.Negate(VectorToSun);
			l2d = position - EyePos;
			float num = l2d.LengthSquared();
			if (num > 0f)
			{
				l2d *= 1f / (float)Math.Sqrt(num);
			}
			l1c = Vector3.Multiply(SunlightColor.ToVector3(), (float)Math.Pow(x, 30.0));
			l2c = Vector3.Multiply(TorchColor.ToVector3(), 0.5f * y * (1f - x * SunlightColor.ToVector3().Y));
		}

		public Vector2 GetLightAtPoint(Vector3 position)
		{
			IntVector3 intVector = IntVector3.FromVector3(position);
			FillCubeLightTable(intVector, ref avatarSun, ref avatarTorch);
			Vector3 vector = IntVector3.ToVector3(intVector);
			Vector3 zero = Vector3.Zero;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			zero.Z = -1f;
			while (zero.Z < 1.5f)
			{
				zero.Y = -1f;
				while (zero.Y < 1.5f)
				{
					zero.X = -1f;
					while (zero.X < 1.5f)
					{
						float num6 = ((2.25f - (vector + zero - position).LengthSquared()) / 2.25f).Clamp(0f, 1f);
						if (avatarTorch[num5] != -1f)
						{
							num3 += num6 * avatarTorch[num5];
							num += num6;
						}
						if (avatarSun[num5] != -1f)
						{
							num4 += num6 * avatarSun[num5];
							num2 += num6;
						}
						zero.X += 1f;
						num5++;
					}
					zero.Y += 1f;
				}
				zero.Z += 1f;
			}
			if (num > 0f)
			{
				num3 /= num;
			}
			if (num2 > 0f)
			{
				num4 /= num2;
			}
			return new Vector2(num4, num3);
		}

		public Vector2 GetSimpleLightAtPoint(Vector3 position)
		{
			Vector2 zero = Vector2.Zero;
			IntVector3 a = IntVector3.FromVector3(position);
			IntVector3 a2 = IntVector3.Subtract(a, _worldMin);
			if (IsIndexValid(a2))
			{
				int blockAt = GetBlockAt(a2);
				BlockTypeEnum typeIndex = Block.GetTypeIndex(blockAt);
				if (typeIndex == BlockTypeEnum.NumberOfBlocks || Block.IsInList(blockAt))
				{
					zero.X = 1f;
					zero.Y = 0f;
				}
				else if (BlockType.GetType(typeIndex).Opaque)
				{
					zero.X = -1f;
					zero.Y = -1f;
				}
				else
				{
					zero.X = (float)Block.GetSunLightLevel(blockAt) / 15f;
					zero.Y = (float)Block.GetTorchLightLevel(blockAt) / 15f;
				}
			}
			return zero;
		}

		public float GetSimpleSunlightAtPoint(Vector3 position)
		{
			float result = 0f;
			IntVector3 a = IntVector3.FromVector3(position);
			IntVector3 a2 = IntVector3.Subtract(a, _worldMin);
			if (IsIndexValid(a2))
			{
				int blockAt = GetBlockAt(a2);
				BlockTypeEnum typeIndex = Block.GetTypeIndex(blockAt);
				result = ((typeIndex == BlockTypeEnum.NumberOfBlocks || Block.IsInList(blockAt)) ? 1f : ((!BlockType.GetType(typeIndex).Opaque) ? ((float)Block.GetSunLightLevel(blockAt) / 15f) : (-1f)));
			}
			return result;
		}

		public float GetSimpleTorchlightAtPoint(Vector3 position)
		{
			float result = 0f;
			IntVector3 a = IntVector3.FromVector3(position);
			IntVector3 a2 = IntVector3.Subtract(a, _worldMin);
			if (IsIndexValid(a2))
			{
				int blockAt = GetBlockAt(a2);
				BlockTypeEnum typeIndex = Block.GetTypeIndex(blockAt);
				result = ((typeIndex == BlockTypeEnum.NumberOfBlocks || Block.IsInList(blockAt)) ? 1f : ((!BlockType.GetType(typeIndex).Opaque) ? ((float)Block.GetTorchLightLevel(blockAt) / 15f) : (-1f)));
			}
			return result;
		}

		public void CreateItemBlockEntity(BlockTypeEnum blockType, IntVector3 location)
		{
			ItemBlockCommand itemBlockCommand = ItemBlockCommand.Alloc();
			itemBlockCommand.AddItem = true;
			itemBlockCommand.BlockType = blockType;
			itemBlockCommand.WorldPosition = location;
			ItemBlockCommandQueue.Queue(itemBlockCommand);
		}

		public void RemoveItemBlockEntity(BlockTypeEnum blockType, IntVector3 location)
		{
			ItemBlockCommand itemBlockCommand = ItemBlockCommand.Alloc();
			itemBlockCommand.AddItem = false;
			itemBlockCommand.WorldPosition = location;
			itemBlockCommand.BlockType = blockType;
			ItemBlockCommandQueue.Queue(itemBlockCommand);
		}

		public bool SetBlock(IntVector3 worldIndex, BlockTypeEnum type)
		{
			ChunkCacheCommand chunkCacheCommand = ChunkCacheCommand.Alloc();
			chunkCacheCommand._command = ChunkCacheCommandEnum.MOD;
			chunkCacheCommand._worldPosition = worldIndex;
			chunkCacheCommand._blockType = type;
			chunkCacheCommand._priority = 1;
			ChunkCache.Instance.AddCommand(chunkCacheCommand);
			if (!IsReady)
			{
				return false;
			}
			IntVector3 a = IntVector3.Subtract(worldIndex, _worldMin);
			if (IsIndexValid(a))
			{
				int num = MakeChunkIndexFromIndexVector(a);
				lock (_chunks[num]._mods)
				{
					for (PendingMod pendingMod = _chunks[num]._mods.Front; pendingMod != null; pendingMod = (PendingMod)pendingMod.NextNode)
					{
						if (pendingMod._worldPosition.Equals(worldIndex))
						{
							pendingMod._blockType = type;
							return false;
						}
					}
				}
				if (_chunks[num]._action > NextChunkAction.COMPUTING_BLOCKS && Block.GetTypeIndex(_blocks[MakeIndex(a)]) == type)
				{
					return false;
				}
				PendingMod pendingMod2 = PendingMod.Alloc();
				pendingMod2._worldPosition = worldIndex;
				pendingMod2._blockType = type;
				_chunks[num]._mods.Queue(pendingMod2);
				return true;
			}
			return false;
		}

		public Vector3 GetActualWaterColor()
		{
			Vector3 vector = BelowWaterColor.ToVector3();
			return vector * SunlightColor.ToVector3();
		}

		public Matrix GetReflectionMatrix()
		{
			if (EyePos.Y < WaterLevel)
			{
				return Matrix.Identity;
			}
			return Matrix.CreateReflection(new Plane(0f, 1f, 0f, 0f - WaterLevel));
		}

		public float GetUnderwaterSkyTint(out Vector3 color)
		{
			if (!IsWaterWorld)
			{
				color = Vector3.Zero;
				return 0f;
			}
			float num = WaterLevel - EyePos.Y;
			num = ((!(num < 0f)) ? Math.Min(num / 12f, 1f - SunlightColor.ToVector3().X * SunlightColor.ToVector3().X) : 0f);
			color = GetActualWaterColor();
			return num;
		}

		public void DrawReflection(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (!IsReady)
			{
				return;
			}
			_updateTasksRemaining.Increment();
			_boundingFrustum.Matrix = view * projection;
			Matrix reflectionMatrix = GetReflectionMatrix();
			_effect.Parameters["Projection"].SetValue(projection);
			_effect.Parameters["World"].SetValue(Matrix.Identity);
			_effect.Parameters["View"].SetValue(view);
			_effect.Parameters["WaterLevel"].SetValue(WaterLevel - 0.5f);
			_effect.Parameters["EyePosition"].SetValue(Vector3.Transform(EyePos, reflectionMatrix));
			_effect.Parameters["LightDirection"].SetValue(VectorToSun);
			_effect.Parameters["TorchLight"].SetValue(TorchColor.ToVector3());
			_effect.Parameters["SunLight"].SetValue(SunlightColor.ToVector3());
			_effect.Parameters["AmbientSun"].SetValue(AmbientSunColor.ToVector3());
			_effect.Parameters["SunSpecular"].SetValue(SunSpecular.ToVector3());
			_effect.Parameters["FogColor"].SetValue(FogColor.ToVector3());
			device.BlendState = BlendState.AlphaBlend;
			device.Indices = _staticIB;
			int num = 0;
			IntVector3 chunkVectorIndex = GetChunkVectorIndex(EyePos);
			for (int i = 0; i < _radiusOrderOffsets.Length; i++)
			{
				IntVector3 intVector = IntVector3.Add(chunkVectorIndex, _radiusOrderOffsets[i]);
				if (intVector.X >= 0 && intVector.X < 24 && intVector.Z >= 0 && intVector.Z < 24)
				{
					int num2 = intVector.X + intVector.Z * 24;
					RenderChunk chunk = _chunks[num2].GetChunk();
					if (chunk.TouchesFrustum(_boundingFrustum))
					{
						_renderIndexList[num++] = num2;
					}
					chunk.Release();
				}
			}
			if (num > 0)
			{
				_effect.CurrentTechnique = _effect.Techniques[0];
				device.BlendState = BlendState.Opaque;
				if (EyePos.Y >= WaterLevel)
				{
					device.RasterizerState = RasterizerState.CullClockwise;
				}
				for (int j = 0; j < num; j++)
				{
					RenderChunk chunk2 = _chunks[_renderIndexList[j]].GetChunk();
					chunk2.DrawReflection(_graphicsDevice, _effect, _boundingFrustum);
					chunk2.Release();
				}
				device.RasterizerState = RasterizerState.CullCounterClockwise;
				device.BlendState = BlendState.NonPremultiplied;
			}
			_updateTasksRemaining.Decrement();
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (CastleMinerZGame.Instance.DrawingReflection && EyePos.Y >= WaterLevel)
			{
				DrawReflection(device, gameTime, view, projection);
				return;
			}
			using (Profiler.TimeSection("Drawing Terrain", ProfilerThreadEnum.MAIN))
			{
				if (!IsReady)
				{
					return;
				}
				_updateTasksRemaining.Increment();
				_boundingFrustum.Matrix = view * projection;
				_effect.Parameters["Projection"].SetValue(projection);
				_effect.Parameters["World"].SetValue(Matrix.Identity);
				_effect.Parameters["View"].SetValue(view);
				_effect.Parameters["EyePosition"].SetValue(EyePos);
				_effect.Parameters["WaterDepth"].SetValue(_worldBuilder.WaterDepth);
				_effect.Parameters["WaterLevel"].SetValue(WaterLevel);
				Vector3 value = default(Vector3);
				value.Z = WaterLevel - EyePos.Y;
				if (value.Z >= 0f)
				{
					value.X = value.Z;
					value.Y = 100000f;
				}
				else
				{
					value.X = 0f;
					value.Y = 1f;
					value.Z = 0f - value.Z;
				}
				_effect.Parameters["EyeWaterConstants"].SetValue(value);
				_effect.Parameters["LightDirection"].SetValue(VectorToSun);
				_effect.Parameters["TorchLight"].SetValue(TorchColor.ToVector3());
				_effect.Parameters["SunLight"].SetValue(SunlightColor.ToVector3());
				_effect.Parameters["AmbientSun"].SetValue(AmbientSunColor.ToVector3());
				_effect.Parameters["SunSpecular"].SetValue(SunSpecular.ToVector3());
				_effect.Parameters["FogColor"].SetValue(FogColor.ToVector3());
				_effect.Parameters["BelowWaterColor"].SetValue(GetActualWaterColor());
				device.Indices = _staticIB;
				int num = 0;
				IntVector3 chunkVectorIndex = GetChunkVectorIndex(EyePos);
				for (int i = 0; i < _radiusOrderOffsets.Length; i++)
				{
					IntVector3 intVector = IntVector3.Add(chunkVectorIndex, _radiusOrderOffsets[i]);
					if (intVector.X >= 0 && intVector.X < 24 && intVector.Z >= 0 && intVector.Z < 24)
					{
						int num2 = intVector.X + intVector.Z * 24;
						RenderChunk chunk = _chunks[num2].GetChunk();
						if (chunk.TouchesFrustum(_boundingFrustum))
						{
							_renderIndexList[num++] = num2;
						}
						chunk.Release();
					}
				}
				if (num > 0)
				{
					if (CastleMinerZGame.Instance.DrawingReflection)
					{
						_effect.CurrentTechnique = _effect.Techniques[1];
					}
					else
					{
						_effect.CurrentTechnique = _effect.Techniques[(!IsWaterWorld) ? 2 : 0];
					}
					device.BlendState = BlendState.Opaque;
					for (int j = 0; j < 2; j++)
					{
						for (int k = 0; k < num; k++)
						{
							RenderChunk chunk2 = _chunks[_renderIndexList[k]].GetChunk();
							chunk2.Draw(_graphicsDevice, _effect, j == 0, _boundingFrustum);
							chunk2.Release();
						}
					}
					device.BlendState = BlendState.NonPremultiplied;
				}
				_updateTasksRemaining.Decrement();
			}
		}
	}
}
