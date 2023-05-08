using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class SMGInventoryItemClass : GunInventoryItemClass
	{
		public SMGInventoryItemClass(InventoryItemIDs id, ToolMaterialTypes material, string name, string description1, string description2, float damage, float durabilitydamage, InventoryItem.InventoryItemClass ammotype)
			: base(id, CastleMinerZGame.Instance.Content.Load<Model>("M11"), name, description1, description2, TimeSpan.FromMinutes(1.0 / 937.0), material, damage, durabilitydamage, ammotype, "GunShot2", "Reload")
		{
			_playerMode = PlayerMode.SMG;
			ShoulderMagnification = 1.1f;
			Recoil = Angle.FromDegrees(3f);
			ReloadTime = TimeSpan.FromSeconds(2.0);
			Angle.FromDegrees(10f);
			Automatic = true;
			RoundsPerReload = (ClipCapacity = 20);
			FlightTime = 1f;
			Innaccuracy = 0.1f;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			ModelEntity modelEntity = (ModelEntity)base.CreateEntity(use, attachedToLocalPlayer);
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f) * Quaternion.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f);
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(32f / modelEntity.GetLocalBoundingSphere().Radius), rotation);
				localToParent.Translation = new Vector3(12f, -19f, -16f);
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
