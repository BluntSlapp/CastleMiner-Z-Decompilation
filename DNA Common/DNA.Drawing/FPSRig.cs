using System;
using DNA.Input;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class FPSRig : Entity
	{
		public const float EyePointHeight = 1.4f;

		private Entity pitchPiviot = new Entity();

		private Entity recoilPiviot = new Entity();

		public PerspectiveCamera FPSCamera = new PerspectiveCamera();

		public Angle TorsoPitch = Angle.Zero;

		public float Speed = 5f;

		public bool InContact = true;

		public Vector3 GroundNormal = Vector3.Up;

		public float JumpImpulse = 10f;

		public float ControlSensitivity = 1f;

		public BasicPhysics PlayerPhysics
		{
			get
			{
				return (BasicPhysics)base.Physics;
			}
		}

		public Quaternion RecoilRotation
		{
			get
			{
				return recoilPiviot.LocalRotation;
			}
			set
			{
				recoilPiviot.LocalRotation = value;
			}
		}

		protected virtual bool CanJump
		{
			get
			{
				return InContact;
			}
		}

		public FPSRig()
		{
			FPSCamera.FieldOfView = Angle.FromDegrees(73f);
			base.Physics = new BasicPhysics(this);
			new GameTime(TimeSpan.FromSeconds(0.01), TimeSpan.FromSeconds(0.01));
			base.Children.Add(pitchPiviot);
			pitchPiviot.LocalPosition = new Vector3(0f, 1.4f, 0f);
			pitchPiviot.Children.Add(recoilPiviot);
			recoilPiviot.Children.Add(FPSCamera);
		}

		public void Jump()
		{
			if (CanJump)
			{
				float num = Vector3.Dot(GroundNormal, Vector3.Up);
				Vector3 worldVelocity = PlayerPhysics.WorldVelocity;
				worldVelocity.Y += JumpImpulse * num;
				PlayerPhysics.WorldVelocity = worldVelocity;
			}
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			pitchPiviot.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, TorsoPitch.Radians);
			base.OnUpdate(gameTime);
		}

		public virtual void ProcessInput(FPSControllerMapping input, GameTime gameTime)
		{
			float num = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			TorsoPitch += Angle.FromRadians((float)Math.PI * input.Aiming.Y * num * ControlSensitivity);
			if (TorsoPitch > Angle.FromDegrees(89f))
			{
				TorsoPitch = Angle.FromDegrees(89f);
			}
			if (TorsoPitch < Angle.FromDegrees(-89f))
			{
				TorsoPitch = Angle.FromDegrees(-89f);
			}
			base.LocalRotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, -4.712389f * input.Aiming.X * num * ControlSensitivity);
			PlayerPhysics.LocalVelocity = new Vector3(input.Movement.X * Speed, PlayerPhysics.LocalVelocity.Y, (0f - input.Movement.Y) * Speed);
			if (input.Jump.Pressed)
			{
				Jump();
			}
		}
	}
}
