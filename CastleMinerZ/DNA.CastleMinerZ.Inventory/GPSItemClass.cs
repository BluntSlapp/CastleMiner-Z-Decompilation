using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class GPSItemClass : ModelInventoryItemClass
	{
		public override bool IsMeleeWeapon
		{
			get
			{
				return false;
			}
		}

		public GPSItemClass(InventoryItemIDs id, Model model, string name, string description1, string description2)
			: base(id, model, name, description1, description2, 1, TimeSpan.FromSeconds(0.30000001192092896), Color.White)
		{
			_playerMode = PlayerMode.Generic;
			switch (id)
			{
			case InventoryItemIDs.GPS:
				ItemSelfDamagePerUse = 0.1f;
				break;
			case InventoryItemIDs.TeleportGPS:
				ItemSelfDamagePerUse = 1f;
				break;
			}
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			Entity entity = null;
			GPSEntity gPSEntity = null;
			ModelEntity modelEntity = null;
			if (use != 0)
			{
				gPSEntity = new GPSEntity(_model);
				gPSEntity.TrackPosition = false;
				entity = gPSEntity;
			}
			else
			{
				modelEntity = new ModelEntity(_model);
				entity = modelEntity;
			}
			entity.EntityColor = Color.White;
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, (float)Math.PI / 2f, 0f);
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(22.4f / modelEntity.GetLocalBoundingSphere().Radius), rotation);
				localToParent.Translation = new Vector3(0f, 0f, 0f);
				modelEntity.LocalToParent = localToParent;
				modelEntity.EnableDefaultLighting();
				break;
			}
			case ItemUse.Hand:
				gPSEntity.TrackPosition = true;
				gPSEntity.LocalRotation = new Quaternion(0.6469873f, 0.1643085f, 0.7078394f, -0.2310277f);
				gPSEntity.LocalPosition = new Vector3(0f, 0.09360941f, 0f);
				break;
			}
			return entity;
		}

		public override InventoryItem CreateItem(int stackCount)
		{
			return new GPSItem(this, stackCount);
		}
	}
}
