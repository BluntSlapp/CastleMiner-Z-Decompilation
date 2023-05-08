using DNA.CastleMinerZ.Utils;

namespace DNA.CastleMinerZ.Terrain
{
	public class ChunkCacheCommand : IReleaseable, ILinkedListNode
	{
		public ChunkCacheCommandEnum _command;

		public IntVector3 _worldPosition = IntVector3.Zero;

		public BlockTypeEnum _blockType;

		public ChunkCacheCommandDelegate _callback;

		public object _context;

		public byte[] _data1;

		public byte[] _data2;

		public string _trackingString;

		public long _submittedTime;

		public int _priority;

		public byte _requesterID;

		public volatile ChunkCacheCommandStatus _status;

		private static int _nextObjID = 0;

		public int _objID;

		public int[] _delta;

		private static ObjectCache<ChunkCacheCommand> _cache = new ObjectCache<ChunkCacheCommand>();

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

		public ChunkCacheCommand()
		{
			_objID = _nextObjID++;
		}

		public static ChunkCacheCommand Alloc()
		{
			ChunkCacheCommand chunkCacheCommand = _cache.Get();
			chunkCacheCommand._status = ChunkCacheCommandStatus.NEW;
			chunkCacheCommand._trackingString = null;
			return chunkCacheCommand;
		}

		public void Release()
		{
			_delta = null;
			_callback = null;
			_context = null;
			_data1 = null;
			_data2 = null;
			_priority = 0;
			_status = ChunkCacheCommandStatus.DONE;
			_cache.Put(this);
		}
	}
}
