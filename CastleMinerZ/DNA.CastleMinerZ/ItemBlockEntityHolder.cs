using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.Utils;
using DNA.Drawing;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ
{
	public class ItemBlockEntityHolder : IReleaseable, ILinkedListNode
	{
		public Vector3 Position;

		public BlockTypeEnum BlockType;

		public TorchEntity TorchEntity;

		private Entity _inWorldEntity;

		public float Distance;

		public float TimeUntilTorchRemovalAllowed;

		public float TimeUntilFlameRemovalAllowed;

		private bool _hasFlame;

		private static ObjectCache<ItemBlockEntityHolder> _cache = new ObjectCache<ItemBlockEntityHolder>();

		private ILinkedListNode _nextNode;

		public Entity InWorldEntity
		{
			get
			{
				return _inWorldEntity;
			}
			set
			{
				if (_inWorldEntity != null && _inWorldEntity != value)
				{
					_inWorldEntity.RemoveFromParent();
				}
				_inWorldEntity = value;
				if (value != null && value is TorchEntity)
				{
					TorchEntity = (TorchEntity)value;
					_hasFlame = TorchEntity.HasFlame;
				}
				else
				{
					TorchEntity = null;
					_hasFlame = false;
				}
			}
		}

		public bool TorchFlame
		{
			get
			{
				return _hasFlame;
			}
			set
			{
				if (value != _hasFlame && TorchEntity != null)
				{
					TorchEntity.HasFlame = value;
					_hasFlame = TorchEntity.HasFlame;
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

		public ItemBlockEntityHolder()
		{
			_inWorldEntity = null;
			TorchEntity = null;
			_hasFlame = false;
		}

		public static ItemBlockEntityHolder Alloc()
		{
			ItemBlockEntityHolder itemBlockEntityHolder = _cache.Get();
			itemBlockEntityHolder.TimeUntilTorchRemovalAllowed = 1f;
			itemBlockEntityHolder.TimeUntilFlameRemovalAllowed = 1f;
			return itemBlockEntityHolder;
		}

		public void Release()
		{
			InWorldEntity = null;
			_cache.Put(this);
		}
	}
}
