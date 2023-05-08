using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class BoltRifleInventoryItemClass : GunInventoryItemClass
	{
		public BoltRifleInventoryItemClass(InventoryItemIDs id, ToolMaterialTypes material, string name, string description1, string description2, float damage, float durabilitydamage, InventoryItem.InventoryItemClass ammotype)
			: base(id, CastleMinerZGame.Instance.Content.Load<Model>("BoltRifle"), name, description1, description2, TimeSpan.FromSeconds(1.0 / 315.0), material, damage, durabilitydamage, ammotype, "GunShot1", "AssaultReload")
		{
			_playerMode = PlayerMode.BoltRifle;
			ShoulderMagnification = 1.5f;
			Recoil = Angle.FromDegrees(12f);
			ReloadTime = TimeSpan.FromSeconds(2.4);
			Angle.FromDegrees(5f);
			Automatic = false;
			RoundsPerReload = (ClipCapacity = 8);
			Innaccuracy = 0f;
			Velocity = 150f;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			ModelEntity modelEntity = (ModelEntity)base.CreateEntity(use, attachedToLocalPlayer);
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -0.408407032f) * Quaternion.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f);
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(38.4f / modelEntity.GetLocalBoundingSphere().Radius), rotation);
				localToParent.Translation = new Vector3(4f, -12f, -16f);
				modelEntity.LocalToParent = localToParent;
				break;
			}
			case ItemUse.Pickup:
			{
				Matrix matrix2 = (modelEntity.LocalToParent = Matrix.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f));
				break;
			}
			}
			return modelEntity;
		}
	}
}
