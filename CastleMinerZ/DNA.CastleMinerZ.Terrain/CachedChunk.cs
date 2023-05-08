using System;
using System.Diagnostics;
using System.IO;
using DNA.CastleMinerZ.Utils;
using DNA.Drawing.UI;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.CastleMinerZ.Terrain
{
	public class CachedChunk : IReleaseable, ILinkedListNode
	{
		private enum DISK_VERSION : uint
		{
			FIRST_FIXED_VERSION = 3203334144u
		}

		private const double CHUNK_HIGH_PRI_TIMEOUT = 3.0;

		private const double CHUNK_LOW_PRI_TIMEOUT = 15.0;

		private static Stopwatch TimeoutTimer = Stopwatch.StartNew();

		public IntVector3 _worldMin;

		public int _loadingPriority;

		public long _timeOfRequest;

		public int _numEntries;

		public int[] _delta;

		public int[] _copy;

		public bool _sameAsDisk;

		public bool _saved = true;

		public SynchronizedQueue<ChunkCacheCommand> _commandQueue = new SynchronizedQueue<ChunkCacheCommand>();

		private static ObjectCache<CachedChunk> _cache = new ObjectCache<CachedChunk>();

		private ILinkedListNode _nextNode;

		public bool SameAsDisk
		{
			get
			{
				return _sameAsDisk;
			}
			set
			{
				if (_sameAsDisk != value)
				{
					_sameAsDisk = value;
					if (!_sameAsDisk)
					{
						_saved = false;
					}
				}
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

		public static IntVector3 MakeChunkCorner(IntVector3 position)
		{
			IntVector3 result = default(IntVector3);
			result.X = (int)(Math.Floor((double)position.X / 16.0) * 16.0);
			result.Y = -64;
			result.Z = (int)(Math.Floor((double)position.Z / 16.0) * 16.0);
			return result;
		}

		public static IntVector3 MakeChunkCornerFromCID(int cid)
		{
			IntVector3 zero = IntVector3.Zero;
			short num = (short)((int)(cid & 0xFFFF0000u) >> 16);
			short num2 = (short)((long)cid & 0xFFFFL);
			zero.X = num2 * 16;
			zero.Y = -64;
			zero.Z = num * 16;
			return zero;
		}

		public static int MakeCIDFromChunkCorner(IntVector3 chunkCorner)
		{
			int num = 0;
			uint num2 = (uint)(chunkCorner.Z / 16) & 0xFFFFu;
			uint num3 = (uint)(chunkCorner.X / 16) & 0xFFFFu;
			return (int)((num2 << 16) | num3);
		}

		private string MakeFilename()
		{
			return Path.Combine(ChunkCache.Instance.RootPath, "X" + _worldMin.X + "Y" + _worldMin.Y + "Z" + _worldMin.Z + ".dat");
		}

		public void Init(IntVector3 worldMin)
		{
			_worldMin = worldMin;
			_saved = true;
			_sameAsDisk = true;
			_loadingPriority = 0;
			_numEntries = 0;
			_copy = null;
		}

		public void SetDelta(int[] delta, bool cameFromDisk)
		{
			_saved = cameFromDisk;
			_sameAsDisk = true;
			_delta = delta;
			if (_delta != null)
			{
				_numEntries = delta.Length;
			}
			else
			{
				_numEntries = 0;
			}
		}

		public int[] GetDeltaCopy()
		{
			if (_numEntries == 0)
			{
				return null;
			}
			if (_numEntries == _delta.Length)
			{
				return _delta;
			}
			if (_copy == null)
			{
				_copy = new int[_numEntries];
				Buffer.BlockCopy(_delta, 0, _copy, 0, _numEntries * 4);
			}
			return _copy;
		}

		private void Embiggen()
		{
			int newsize = ((_delta != null) ? (_delta.Length + 32) : 32);
			Resize(newsize);
		}

		private void Resize(int newsize)
		{
			int[] array = new int[newsize];
			if (_numEntries > 0)
			{
				Buffer.BlockCopy(_delta, 0, array, 0, _numEntries * 4);
			}
			_delta = array;
		}

		public void AddWorldVector(IntVector3 entry, BlockTypeEnum type)
		{
			Add(IntVector3.Subtract(entry, _worldMin), type);
		}

		public void Add(IntVector3 entry, BlockTypeEnum type)
		{
			int num = DeltaEntry.Create(entry, type);
			for (int i = 0; i < _numEntries; i++)
			{
				if (DeltaEntry.SameLocation(_delta[i], num))
				{
					if (_delta[i] != num)
					{
						_delta[i] = num;
						_copy = null;
						SameAsDisk = false;
					}
					return;
				}
			}
			if (_delta == null || _numEntries >= _delta.Length)
			{
				Embiggen();
			}
			_delta[_numEntries++] = num;
			_copy = null;
			SameAsDisk = false;
		}

		public void QueueCommand(ChunkCacheCommand command)
		{
			if (command._priority > _loadingPriority)
			{
				GetChunkFromHost(command._priority);
			}
			if (command._command == ChunkCacheCommandEnum.MOD)
			{
				for (ChunkCacheCommand chunkCacheCommand = _commandQueue.Front; chunkCacheCommand != null; chunkCacheCommand = (ChunkCacheCommand)chunkCacheCommand.NextNode)
				{
					if (chunkCacheCommand._command == ChunkCacheCommandEnum.MOD && chunkCacheCommand._worldPosition.Equals(command._worldPosition))
					{
						chunkCacheCommand._blockType = command._blockType;
						command.Release();
						return;
					}
				}
			}
			_commandQueue.Queue(command);
			command._status = ChunkCacheCommandStatus.BLOCKED;
		}

		public void RunCommand(ChunkCacheCommand command)
		{
			bool flag = true;
			switch (command._command)
			{
			case ChunkCacheCommandEnum.MOD:
			{
				IntVector3 entry = IntVector3.Subtract(command._worldPosition, _worldMin);
				Add(entry, command._blockType);
				break;
			}
			case ChunkCacheCommandEnum.FETCHDELTAFORTERRAIN:
				command._delta = GetDeltaCopy();
				command._callback(command);
				flag = false;
				break;
			}
			if (flag)
			{
				command.Release();
			}
		}

		public void StripFetchCommands()
		{
			ChunkCacheCommand chunkCacheCommand = null;
			ChunkCacheCommand chunkCacheCommand2 = null;
			while (!_commandQueue.Empty)
			{
				chunkCacheCommand2 = _commandQueue.Dequeue();
				if (chunkCacheCommand2._command == ChunkCacheCommandEnum.FETCHDELTAFORTERRAIN)
				{
					chunkCacheCommand2._command = ChunkCacheCommandEnum.RESETWAITINGCHUNKS;
					chunkCacheCommand2._callback(chunkCacheCommand2);
					if (_loadingPriority > 0)
					{
						_loadingPriority = 0;
					}
				}
				else
				{
					chunkCacheCommand2.NextNode = chunkCacheCommand;
					chunkCacheCommand = chunkCacheCommand2;
				}
			}
			while (chunkCacheCommand != null)
			{
				chunkCacheCommand2 = chunkCacheCommand;
				chunkCacheCommand = (ChunkCacheCommand)chunkCacheCommand2.NextNode;
				chunkCacheCommand2.NextNode = null;
				_commandQueue.Undequeue(chunkCacheCommand2);
			}
		}

		public void ExecuteCommands()
		{
			while (!_commandQueue.Empty)
			{
				RunCommand(_commandQueue.Dequeue());
			}
		}

		public void GetChunkFromDisk()
		{
			if (!ChunkCache.Instance.IsStorageEnabled || !ChunkCache.Instance.ChunkInLocalList(_worldMin))
			{
				return;
			}
			SignedInGamer currentGamer = Screen.CurrentGamer;
			string fileName = MakeFilename();
			int[] data = null;
			try
			{
				CastleMinerZGame.Instance.SaveDevice.Load(fileName, delegate(Stream stream)
				{
					BinaryReader binaryReader = new BinaryReader(stream);
					int num = 0;
					uint num2 = binaryReader.ReadUInt32();
					if (num2 == 3203334144u)
					{
						num = binaryReader.ReadInt32();
						data = new int[num];
						for (int i = 0; i < num; i++)
						{
							data[i] = binaryReader.ReadInt32();
						}
					}
					else
					{
						int num3;
						if ((num2 & 0xFFFF) == 2)
						{
							stream.Position = 0L;
							num3 = (int)stream.Length;
						}
						else
						{
							num3 = (int)num2;
						}
						int num4 = binaryReader.ReadByte();
						for (int j = 0; j < num4 - 1; j++)
						{
							binaryReader.ReadByte();
						}
						num = (num3 - num4) / 4;
						data = new int[num];
						for (int k = 0; k < num; k++)
						{
							uint num5 = binaryReader.ReadUInt32();
							uint num6 = (num5 & 0xFF000000u) >> 24;
							num6 |= (num5 & 0xFF0000) >> 8;
							num6 |= (num5 & 0xFF00) << 8;
							num6 |= (num5 & 0xFF) << 24;
							data[k] = (int)num6;
						}
					}
				});
			}
			catch (FileNotFoundException)
			{
				data = null;
			}
			catch (Exception)
			{
				data = null;
			}
			SetDelta(data, true);
		}

		public void GetChunkFromHost(int priority)
		{
			ChunkCache.Instance.GetChunkFromServer(_worldMin, priority);
			_loadingPriority = priority;
			_timeOfRequest = TimeoutTimer.ElapsedMilliseconds;
		}

		public void Save()
		{
			if (_saved || !ChunkCache.Instance.IsStorageEnabled)
			{
				return;
			}
			SignedInGamer currentGamer = Screen.CurrentGamer;
			string fileName = MakeFilename();
			bool flag = true;
			if (_numEntries != 0)
			{
				try
				{
					CastleMinerZGame.Instance.SaveDevice.Save(fileName, true, true, delegate(Stream stream)
					{
						BinaryWriter binaryWriter = new BinaryWriter(stream);
						binaryWriter.Write(3203334144u);
						binaryWriter.Write(_numEntries);
						for (int i = 0; i < _numEntries; i++)
						{
							binaryWriter.Write(_delta[i]);
						}
						binaryWriter.Flush();
					});
					flag = true;
				}
				catch (Exception)
				{
					flag = false;
				}
			}
			if (flag)
			{
				_saved = true;
				SameAsDisk = true;
			}
		}

		public void RetroReadFromDisk()
		{
			GetChunkFromDisk();
			ExecuteCommands();
		}

		public void HostChanged()
		{
			GetChunkFromHost(_loadingPriority);
		}

		public void MaybeRetryFetch()
		{
			double num = (double)(TimeoutTimer.ElapsedMilliseconds - _timeOfRequest) / 1000.0;
			double num2 = ((_loadingPriority == 0) ? 15.0 : 3.0);
			if (num > num2)
			{
				GetChunkFromHost(_loadingPriority);
			}
		}

		public static CachedChunk Alloc()
		{
			return _cache.Get();
		}

		public void Release()
		{
			_delta = null;
			_numEntries = 0;
			_copy = null;
			_cache.Put(this);
		}
	}
}
