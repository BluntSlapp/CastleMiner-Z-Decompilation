using System;
using System.Collections.Generic;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Inventory
{
	public class BlockInventoryItemClass : InventoryItem.InventoryItemClass
	{
		public static Dictionary<BlockTypeEnum, InventoryItem.InventoryItemClass> BlockClasses = new Dictionary<BlockTypeEnum, InventoryItem.InventoryItemClass>();

		public BlockType BlockType;

		public override InventoryItem CreateItem(int stackCount)
		{
			return new BlockInventoryItem(this, stackCount);
		}

		public static InventoryItem CreateBlockItem(BlockTypeEnum blockType, int stackCount)
		{
			InventoryItem.InventoryItemClass value;
			if (BlockClasses.TryGetValue(BlockType.GetType(blockType).ParentBlockType, out value))
			{
				return value.CreateItem(stackCount);
			}
			return null;
		}

		public BlockInventoryItemClass(InventoryItemIDs id, BlockTypeEnum blockType, string description1, string description2, float meleeDamage)
			: base(id, BlockType.GetType(blockType).Name, description1, description2, 64, TimeSpan.FromSeconds(0.10000000149011612), "Place")
		{
			BlockType = BlockType.GetType(blockType);
			BlockClasses[blockType] = this;
			_playerMode = PlayerMode.Block;
			EnemyDamageType = DamageType.BLUNT;
			EnemyDamage = meleeDamage;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			BlockEntity blockEntity = new BlockEntity(BlockType._type, attachedToLocalPlayer);
			switch (use)
			{
			case ItemUse.Hand:
				blockEntity.LocalRotation = new Quaternion(-0.02067951f, -0.007977718f, 0.03257636f, 0.9992235f);
				blockEntity.LocalPosition = new Vector3(0f, 0.06533548f, 0f);
				blockEntity.Scale = 0.1f;
				break;
			case ItemUse.Pickup:
				blockEntity.Scale = 0.2f;
				break;
			case ItemUse.UI:
			{
				Matrix matrix2 = (blockEntity.LocalToParent = Matrix.CreateFromYawPitchRoll((float)Math.PI, -(float)Math.PI / 12f, (float)Math.PI / 12f));
				blockEntity.UIObject();
				blockEntity.Scale = 38.4f;
				break;
			}
			}
			blockEntity.Update(CastleMinerZGame.Instance, new GameTime());
			return blockEntity;
		}
	}
}
