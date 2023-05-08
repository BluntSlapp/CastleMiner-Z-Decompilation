using System;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using DNA.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class TorchEntity : Entity
	{
		public static Model _torchModel;

		private static ParticleEffect _smokeEffect;

		private static ParticleEffect _fireEffect;

		private BlockFace AttachedFace = BlockFace.NUM_FACES;

		private ModelEntity _modelEnt;

		private ParticleEmitter _smokeEmitter;

		private ParticleEmitter _fireEmitter;

		private bool _hasFlame;

		public bool HasFlame
		{
			get
			{
				return _hasFlame;
			}
			set
			{
				if (value)
				{
					AddFlame();
				}
				else
				{
					RemoveFlame();
				}
			}
		}

		static TorchEntity()
		{
			_torchModel = CastleMinerZGame.Instance.Content.Load<Model>("Torch");
			_smokeEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("TorchSmoke");
			_fireEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("TorchFire");
		}

		public TorchEntity(bool hasParticles)
		{
			_modelEnt = new ModelEntity(_torchModel);
			base.Children.Add(_modelEnt);
			HasFlame = hasParticles;
			SetPosition(AttachedFace);
		}

		protected override void OnParentChanged(Entity oldParent, Entity newParent)
		{
			if (newParent == null)
			{
				RemoveFlame();
			}
			base.OnParentChanged(oldParent, newParent);
		}

		public void RemoveFlame()
		{
			if (_hasFlame)
			{
				_hasFlame = false;
				if (_smokeEmitter != null)
				{
					_smokeEmitter.RemoveFromParent();
				}
				if (_fireEmitter != null)
				{
					_fireEmitter.RemoveFromParent();
				}
			}
		}

		public void AddFlame()
		{
			if (!_hasFlame)
			{
				_hasFlame = true;
				_smokeEmitter = _smokeEffect.CreateEmitter(CastleMinerZGame.Instance);
				_smokeEmitter.Emitting = true;
				_smokeEmitter.DrawPriority = 800;
				_modelEnt.Children.Add(_smokeEmitter);
				_fireEmitter = _fireEffect.CreateEmitter(CastleMinerZGame.Instance);
				_fireEmitter.Emitting = true;
				_fireEmitter.DrawPriority = 800;
				_modelEnt.Children.Add(_fireEmitter);
				Matrix transform = _modelEnt.Skeleton["Flame"].Transform;
				_smokeEmitter.LocalToParent = transform;
				_fireEmitter.LocalToParent = transform;
			}
		}

		public void SetPosition(BlockFace face)
		{
			AttachedFace = face;
			switch (AttachedFace)
			{
			case BlockFace.NUM_FACES:
				_modelEnt.LocalPosition = Vector3.Zero;
				_modelEnt.LocalRotation = Quaternion.Identity;
				break;
			case BlockFace.POSY:
				_modelEnt.LocalPosition = new Vector3(0f, -0.5f, 0f);
				_modelEnt.LocalRotation = Quaternion.Identity;
				break;
			case BlockFace.NEGX:
				_modelEnt.LocalPosition = new Vector3(0.5f, -0.25f, 0f);
				_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / 4f);
				break;
			case BlockFace.NEGZ:
				_modelEnt.LocalPosition = new Vector3(0f, -0.25f, 0.5f);
				_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -(float)Math.PI / 4f);
				break;
			case BlockFace.POSX:
				_modelEnt.LocalPosition = new Vector3(-0.5f, -0.25f, 0f);
				_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -(float)Math.PI / 4f);
				break;
			case BlockFace.POSZ:
				_modelEnt.LocalPosition = new Vector3(0f, -0.25f, -0.5f);
				_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 4f);
				break;
			case BlockFace.NEGY:
				_modelEnt.LocalPosition = new Vector3(0f, 0.5f, 0f);
				_modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI);
				break;
			}
		}
	}
}
