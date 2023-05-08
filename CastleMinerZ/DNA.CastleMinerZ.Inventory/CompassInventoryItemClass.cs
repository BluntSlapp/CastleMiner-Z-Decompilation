using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class CompassInventoryItemClass : ModelInventoryItemClass
	{
		public override bool IsMeleeWeapon
		{
			get
			{
				return false;
			}
		}

		public CompassInventoryItemClass(InventoryItemIDs id, Model model)
			: base(id, model, "Compass", "Show the direction to or away from the start point", "In endurance mode travel in the direction of the green arrow", 1, TimeSpan.FromSeconds(0.30000001192092896), Color.White)
		{
			_playerMode = PlayerMode.Generic;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			Entity entity = null;
			CompassEntity compassEntity = null;
			ModelEntity modelEntity = null;
			if (use != 0)
			{
				compassEntity = new CompassEntity(_model);
				compassEntity.TrackPosition = false;
				entity = compassEntity;
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
				compassEntity.TrackPosition = true;
				compassEntity.LocalRotation = new Quaternion(0.6469873f, 0.1643085f, 0.7078394f, -0.2310277f);
				compassEntity.LocalPosition = new Vector3(0f, 0.09360941f, 0f);
				break;
			}
			return entity;
		}
	}
}
