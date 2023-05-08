using System;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Inventory
{
	public class TorchInventoryItemClass : ItemBlockInventoryItemClass
	{
		public TorchInventoryItemClass()
			: base(InventoryItemIDs.Torch, BlockTypeEnum.Torch, "Use these to light your world", "They also keep some monsters away")
		{
			_playerMode = PlayerMode.Pick;
		}

		public override InventoryItem CreateItem(int stackCount)
		{
			return new TorchInventoryitem(this, stackCount);
		}

		public override Entity CreateWorldEntity(bool attachedToLocalPlayer, BlockTypeEnum blockType)
		{
			TorchEntity torchEntity = new TorchEntity(false);
			torchEntity.SetPosition(BlockType.GetType(blockType).Facing);
			return torchEntity;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			Entity result = null;
			switch (use)
			{
			case ItemUse.Hand:
			{
				TorchEntity torchEntity2 = new TorchEntity(false);
				result = torchEntity2;
				torchEntity2.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 4f);
				torchEntity2.LocalScale = new Vector3(0.5f, 0.5f, 0.5f);
				torchEntity2.LocalPosition = new Vector3(0f, 0.11126215f, 0f);
				break;
			}
			case ItemUse.Pickup:
			{
				TorchEntity torchEntity = new TorchEntity(false);
				result = torchEntity;
				break;
			}
			case ItemUse.UI:
			{
				ModelEntity modelEntity = new ModelEntity(TorchEntity._torchModel);
				modelEntity.EnableDefaultLighting();
				result = modelEntity;
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f);
				float scale = 32f / modelEntity.GetLocalBoundingSphere().Radius;
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(scale), rotation);
				localToParent.Translation = new Vector3(-15f, -15f, 0f);
				modelEntity.LocalToParent = localToParent;
				break;
			}
			}
			return result;
		}
	}
}
