using System;
using DNA.CastleMinerZ.Inventory;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ
{
	public class FlyingPickupEntity : Entity
	{
		private const float Accel = 20f;

		private const float MaxVel = 100f;

		private const float PickupRadSqu = 4f;

		private Entity _displayEntity;

		private Player _target;

		private float _velocity;

		public FlyingPickupEntity(InventoryItem item, Player player, Vector3 pos)
		{
			_displayEntity = item.CreateEntity(ItemUse.Pickup, false);
			_target = player;
			base.Children.Add(_displayEntity);
			_velocity = 0f;
			base.LocalPosition = pos;
			Collidee = false;
			Collider = false;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			_displayEntity.LocalRotation *= Quaternion.CreateFromAxisAngle(Vector3.Down, (float)gameTime.get_ElapsedGameTime().TotalSeconds);
			NetworkGamer gamer = _target.Gamer;
			if (!_target.ValidLivingGamer)
			{
				RemoveFromParent();
				return;
			}
			Vector3 vector = _target.LocalPosition - base.LocalPosition;
			float num = vector.LengthSquared();
			if (num < 0.5f)
			{
				RemoveFromParent();
				return;
			}
			float num2 = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			_velocity += 20f * num2;
			_velocity = Math.Min(_velocity, 100f);
			float num3 = _velocity * num2;
			if (num3 * num3 > num)
			{
				RemoveFromParent();
				return;
			}
			vector.Normalize();
			vector *= num3;
			base.LocalPosition += vector;
			base.OnUpdate(gameTime);
		}
	}
}
