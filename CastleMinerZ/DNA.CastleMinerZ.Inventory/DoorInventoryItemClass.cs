using System;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Inventory
{
	public class DoorInventoryItemClass : ItemBlockInventoryItemClass
	{
		public DoorInventoryItemClass()
			: base(InventoryItemIDs.Door, BlockTypeEnum.LowerDoor, "Open or close to keep monsters out", "")
		{
			_playerMode = PlayerMode.Block;
		}

		public override InventoryItem CreateItem(int stackCount)
		{
			return new DoorInventoryitem(this, stackCount);
		}

		public override Entity CreateWorldEntity(bool attachedToLocalPlayer, BlockTypeEnum blockType)
		{
			DoorEntity doorEntity = new DoorEntity();
			doorEntity.SetPosition(blockType);
			return doorEntity;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			Entity entity = null;
			switch (use)
			{
			case ItemUse.Hand:
			{
				DoorEntity doorEntity2 = new DoorEntity();
				entity = doorEntity2;
				doorEntity2.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 4f);
				doorEntity2.LocalScale = new Vector3(0.1f, 0.1f, 0.1f);
				doorEntity2.LocalPosition = new Vector3(0f, 0.11126215f, 0f);
				break;
			}
			case ItemUse.Pickup:
			{
				DoorEntity doorEntity = new DoorEntity();
				entity = doorEntity;
				break;
			}
			case ItemUse.UI:
			{
				ModelEntity modelEntity = new ModelEntity(DoorEntity._doorModel);
				modelEntity.EnableDefaultLighting();
				entity = modelEntity;
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, 0f);
				float scale = 32f / modelEntity.GetLocalBoundingSphere().Radius;
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(scale), rotation);
				localToParent.Translation = new Vector3(-15f, -25f, 0f);
				modelEntity.LocalToParent = localToParent;
				break;
			}
			}
			if (entity != null)
			{
				entity.EntityColor = Color.White;
			}
			return entity;
		}
	}
}
