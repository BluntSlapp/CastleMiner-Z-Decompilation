using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class GunEntity : CastleMinerToolModel
	{
		private static Model _muzzleFlashModel;

		private ModelEntity _muzzleFlash;

		private Random rand = new Random();

		public void ShowMuzzleFlash()
		{
			_muzzleFlash.Visible = true;
		}

		static GunEntity()
		{
			_muzzleFlashModel = CastleMinerZGame.Instance.Content.Load<Model>("MuzzleFlash");
		}

		public GunEntity(Model gunModel)
			: base(gunModel)
		{
			_muzzleFlash = new ModelEntity(_muzzleFlashModel);
			_muzzleFlash.BlendState = BlendState.Additive;
			_muzzleFlash.DepthStencilState = DepthStencilState.DepthRead;
			_muzzleFlash.Visible = false;
			base.Children.Add(_muzzleFlash);
			EnableDefaultLighting();
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			base.Draw(device, gameTime, view, projection);
			_muzzleFlash.LocalToParent = Matrix.CreateScale(0.75f + (float)rand.NextDouble() / 2f) * Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)rand.NextDouble() * ((float)Math.PI * 2f))) * base.Skeleton["BarrelTip"].Transform;
			_muzzleFlash.Visible = false;
		}
	}
}
