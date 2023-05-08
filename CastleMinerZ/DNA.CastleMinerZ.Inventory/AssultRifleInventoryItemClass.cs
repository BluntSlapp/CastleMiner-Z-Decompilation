using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class AssultRifleInventoryItemClass : GunInventoryItemClass
	{
		public AssultRifleInventoryItemClass(InventoryItemIDs id, ToolMaterialTypes material, string name, string description1, string description2, float bulletdamage, float durabilitydamage, InventoryItem.InventoryItemClass ammotype)
			: base(id, CastleMinerZGame.Instance.Content.Load<Model>("AK"), name, description1, description2, TimeSpan.FromSeconds(0.10000000149011612), material, bulletdamage, durabilitydamage, ammotype, "GunShot3", "AssaultReload")
		{
			_playerMode = PlayerMode.Assault;
			ReloadTime = TimeSpan.FromSeconds(3.0);
			Angle.FromDegrees(10f);
			Automatic = true;
			RoundsPerReload = (ClipCapacity = 30);
			ShoulderMagnification = 2f;
			Innaccuracy = 0.02f;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			ModelEntity modelEntity = (ModelEntity)base.CreateEntity(use, attachedToLocalPlayer);
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f) * Quaternion.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f);
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
