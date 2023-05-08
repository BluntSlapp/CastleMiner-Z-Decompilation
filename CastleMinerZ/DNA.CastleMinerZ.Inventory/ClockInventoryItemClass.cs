using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class ClockInventoryItemClass : ModelInventoryItemClass
	{
		public override bool IsMeleeWeapon
		{
			get
			{
				return false;
			}
		}

		public ClockInventoryItemClass(InventoryItemIDs id, Model model)
			: base(id, model, "Clock", "Show the time of day", "", 1, TimeSpan.FromSeconds(0.30000001192092896), Color.White)
		{
			_playerMode = PlayerMode.Generic;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			Entity entity = null;
			ClockEntity clockEntity = null;
			ModelEntity modelEntity = null;
			if (use != 0)
			{
				clockEntity = new ClockEntity(_model);
				clockEntity.TrackPosition = false;
				entity = clockEntity;
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
				clockEntity.TrackPosition = true;
				clockEntity.LocalRotation = new Quaternion(0.6469873f, 0.1643085f, 0.7078394f, -0.2310277f);
				clockEntity.LocalPosition = new Vector3(0f, 0.09360941f, 0f);
				break;
			}
			return entity;
		}
	}
}
