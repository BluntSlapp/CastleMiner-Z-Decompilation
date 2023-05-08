using System;
using System.IO;
using System.Text;
using DNA.Audio;
using DNA.Avatars;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.CastleMinerZ.Utils.Trace;
using DNA.Drawing;
using DNA.Drawing.Animation;
using DNA.Drawing.Particles;
using DNA.Input;
using DNA.IO.Storage;
using DNA.Net;
using DNA.Security.Cryptography;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ
{
	public class Player : FPSRig
	{
		public enum AnimChannels
		{
			Lower,
			Tilt,
			Upper,
			UpperUse,
			Head,
			Death
		}

		public class NoMovePhysics : BasicPhysics
		{
			public NoMovePhysics(Entity e)
				: base(e)
			{
			}

			public override void Move(TimeSpan dt)
			{
			}
		}

		private enum AnimationState
		{
			Unshouldered,
			Shouldering,
			UnShouldering,
			Shouldered,
			Reloading
		}

		private const float FALL_DAMAGE_MIN_VELOCITY = 15f;

		private const float FALL_DAMAGE_VELOCITY_MULTIPLIER = 1f / 15f;

		private static MD5HashProvider hasher;

		public TimeSpan TimeConnected = TimeSpan.Zero;

		public NetworkGamer Gamer;

		public Texture2D GamerPicture;

		public GamerProfile Profile;

		public Avatar Avatar;

		public AABBTraceProbe MovementProbe = new AABBTraceProbe();

		public BoundingBox PlayerAABB = new BoundingBox(new Vector3(-0.35f, 0f, -0.35f), new Vector3(0.35f, 1.7f, 0.35f));

		public ModelEntity _shadow;

		public Angle DefaultFOV = Angle.FromDegrees(73f);

		public Angle DefaultAvatarFOV = Angle.FromDegrees(90f);

		public Angle ShoulderedAvatarFOV = Angle.FromDegrees(45f);

		public PerspectiveCamera GunEyePointCamera = new PerspectiveCamera();

		public IntVector3 FocusCrate;

		public Point FocusCrateItem;

		public bool UsingTool;

		public bool Shouldering;

		public bool Reloading;

		public PlayerMode _playerMode = PlayerMode.Fist;

		public bool IsActive = true;

		private bool _isRunning;

		private bool _isMoveing;

		public bool LockedFromFalling;

		public bool _flyMode;

		public bool FinalSaveRegistered;

		public PlayerInventory PlayerInventory;

		private AnimationPlayer _torsoPitchAnimation;

		public AudioEmitter SoundEmitter = new AudioEmitter();

		public static ParticleEffect _smokeEffect;

		private static ParticleEffect _sparkEffect;

		private static ParticleEffect _rocksEffect;

		private static ParticleEffect _starsEffect;

		private static Model _shadowModel;

		public bool Dead;

		public Entity RightHand;

		private OneShotTimer _footStepTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		public float _walkSpeed = 1f;

		private bool _prevUnderwaterState;

		public Cue UnderwaterCue;

		private static AvatarExpression[] mouthLevels;

		private OneShotTimer mouthTimer = new OneShotTimer(TimeSpan.FromSeconds(0.1));

		private Random rand = new Random();

		private FastRand _fastRand = new FastRand();

		private TraceProbe shadowProbe = new TraceProbe();

		private Angle recoilDecay = Angle.FromDegrees(30f);

		private bool usingAnimationPlaying;

		private AnimationState currentAnimState;

		private float underWaterSpeed = 1.894f;

		private SoundCue3D _reloadCue;

		public string ReloadSound;

		public string PlayerHash
		{
			get
			{
				return GetHashFromGamerTag(Gamer.Gamertag);
			}
		}

		private AnimationPlayer LowerAnimation
		{
			get
			{
				return Avatar.Animations[0];
			}
		}

		private AnimationPlayer UpperAnimation
		{
			get
			{
				return Avatar.Animations[2];
			}
		}

		private AnimationPlayer HeadAnimation
		{
			get
			{
				return Avatar.Animations[4];
			}
		}

		public ParticleEffect SparkEffect
		{
			get
			{
				return _sparkEffect;
			}
		}

		protected override bool CanJump
		{
			get
			{
				if (!base.CanJump)
				{
					return InWater;
				}
				return true;
			}
		}

		public bool IsLocal
		{
			get
			{
				return Gamer.IsLocal;
			}
		}

		public bool FPSMode
		{
			get
			{
				return !base.Children.Contains(Avatar);
			}
			set
			{
				if (value)
				{
					if (Avatar.Parent != null)
					{
						Avatar.RemoveFromParent();
					}
					CastleMinerZGame.Instance.GameScreen._fpsScene.Children.Add(Avatar);
					Avatar.HideHead = true;
				}
				else
				{
					if (Avatar.Parent != null)
					{
						Avatar.RemoveFromParent();
					}
					base.Children.Add(Avatar);
					Avatar.HideHead = false;
				}
			}
		}

		public bool FlyMode
		{
			get
			{
				return _flyMode;
			}
			set
			{
				_flyMode = value;
			}
		}

		public bool InWater
		{
			get
			{
				if (BlockTerrain.Instance.IsWaterWorld)
				{
					return BlockTerrain.Instance.DepthUnderWater(base.WorldPosition) > 0f;
				}
				return false;
			}
		}

		public bool Underwater
		{
			get
			{
				if (BlockTerrain.Instance.IsWaterWorld)
				{
					return (double)BlockTerrain.Instance.DepthUnderWater(base.WorldPosition) >= 1.5;
				}
				return false;
			}
		}

		public float PercentSubmergedWater
		{
			get
			{
				if (!BlockTerrain.Instance.IsWaterWorld)
				{
					return 0f;
				}
				float num = BlockTerrain.Instance.DepthUnderWater(base.WorldPosition);
				if (num < 0f)
				{
					return 0f;
				}
				return Math.Min(1f, num);
			}
		}

		public bool InLava
		{
			get
			{
				return PercentSubmergedLava > 0f;
			}
		}

		public bool UnderLava
		{
			get
			{
				return PercentSubmergedLava >= 1f;
			}
		}

		public float PercentSubmergedLava
		{
			get
			{
				float num = (-60f - base.WorldPosition.Y) / 2f;
				if (num < 0f)
				{
					return 0f;
				}
				return Math.Min(1f, num);
			}
		}

		public bool ValidGamer
		{
			get
			{
				if (Gamer != null && !Gamer.IsDisposed)
				{
					return !Gamer.HasLeftSession;
				}
				return false;
			}
		}

		public bool ValidLivingGamer
		{
			get
			{
				if (!Dead)
				{
					return ValidGamer;
				}
				return false;
			}
		}

		private static string GetHashFromGamerTag(string gamerTag)
		{
			Hash hash = hasher.Compute(Encoding.UTF8.GetBytes(gamerTag));
			return hash.ToString();
		}

		public bool LoadInventory(SaveDevice device, string path)
		{
			bool result = false;
			PlayerInventory playerInventory = new PlayerInventory(this, false);
			try
			{
				string path2 = Path.Combine(path, PlayerHash + ".inv");
				playerInventory.LoadFromStorage(device, path2);
			}
			catch
			{
				result = true;
				playerInventory = new PlayerInventory(this, true);
			}
			PlayerInventory = playerInventory;
			return result;
		}

		public void SaveInventory(SaveDevice device, string path)
		{
			try
			{
				string path2 = Path.Combine(path, PlayerHash + ".inv");
				PlayerInventory.SaveToStorage(device, path2);
			}
			catch
			{
			}
		}

		public void DeleteInventory(SaveDevice device, string path)
		{
			try
			{
				string fileName = Path.Combine(path, PlayerHash + ".inv");
				device.Delete(fileName);
			}
			catch
			{
			}
		}

		static Player()
		{
			hasher = new MD5HashProvider();
			mouthLevels = new AvatarExpression[4];
			_smokeEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("SmokeEffect");
			_sparkEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("SparksEffect");
			_rocksEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("RocksEffect");
			_starsEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("StarRingEffect");
			mouthLevels[0].Mouth = AvatarMouth.Neutral;
			mouthLevels[0].LeftEye = AvatarEye.Neutral;
			mouthLevels[0].RightEye = AvatarEye.Neutral;
			mouthLevels[1].Mouth = AvatarMouth.PhoneticEe;
			mouthLevels[1].LeftEye = AvatarEye.Neutral;
			mouthLevels[1].RightEye = AvatarEye.Neutral;
			mouthLevels[2].Mouth = AvatarMouth.PhoneticAi;
			mouthLevels[2].LeftEye = AvatarEye.Neutral;
			mouthLevels[2].RightEye = AvatarEye.Neutral;
			mouthLevels[3].Mouth = AvatarMouth.Shocked;
			mouthLevels[3].LeftEye = AvatarEye.Blink;
			mouthLevels[3].RightEye = AvatarEye.Blink;
			_shadowModel = CastleMinerZGame.Instance.Content.Load<Model>("Shadow");
		}

		private void UpdateAudio(GameTime gameTime)
		{
			Vector3 normal = new Vector3(0f, 0f, -1f);
			normal = Vector3.TransformNormal(normal, base.LocalToWorld);
			if (Gamer.IsLocal)
			{
				CastleMinerZGame.Instance.Listener.Position = base.WorldPosition + new Vector3(0f, 1.8f, 0f);
				CastleMinerZGame.Instance.Listener.Forward = normal;
				CastleMinerZGame.Instance.Listener.Up = new Vector3(0f, 1f, 0f);
				CastleMinerZGame.Instance.Listener.Velocity = base.PlayerPhysics.WorldVelocity;
			}
			SoundEmitter.Position = base.WorldPosition;
			SoundEmitter.Forward = normal;
			SoundEmitter.Up = Vector3.Up;
			SoundEmitter.Velocity = base.PlayerPhysics.WorldVelocity;
			if (_prevUnderwaterState != InWater)
			{
				SoundManager.Instance.PlayInstance("Splash", SoundEmitter);
			}
			_prevUnderwaterState = InWater;
			if (Underwater && IsLocal)
			{
				if (UnderwaterCue == null)
				{
					UnderwaterCue = SoundManager.Instance.PlayInstance("Underwater");
				}
				else if (UnderwaterCue.IsPaused)
				{
					UnderwaterCue.Resume();
				}
			}
			else if (UnderwaterCue != null)
			{
				UnderwaterCue.Pause();
			}
			if (!_isMoveing || !IsActive)
			{
				return;
			}
			_footStepTimer.Update(TimeSpan.FromSeconds(gameTime.get_ElapsedGameTime().TotalSeconds * (double)_walkSpeed));
			if (_footStepTimer.Expired)
			{
				if (!_flyMode && !InWater && InContact)
				{
					SoundManager.Instance.PlayInstance("FootStep", SoundEmitter);
				}
				else if (!_flyMode && InWater && InContact && !Underwater)
				{
					SoundManager.Instance.PlayInstance("Splat", SoundEmitter);
				}
				_footStepTimer.Reset();
				if (_isRunning)
				{
					_footStepTimer.MaxTime = TimeSpan.FromSeconds(0.4);
				}
				else
				{
					_footStepTimer.MaxTime = TimeSpan.FromSeconds(0.5);
				}
			}
		}

		public void Equip(InventoryItem item)
		{
			Reloading = false;
			ChangeCarriedItemMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, item.ItemClass.ID);
		}

		public void PutItemInHand(InventoryItemIDs itemID)
		{
			RightHand.Children.Clear();
			InventoryItem.InventoryItemClass @class = InventoryItem.GetClass(itemID);
			if (@class != null)
			{
				RightHand.Children.Add(@class.CreateEntity(ItemUse.Hand, IsLocal));
				_playerMode = @class.PlayerAnimationMode;
			}
			else
			{
				_playerMode = PlayerMode.Fist;
			}
		}

		public Player(NetworkGamer gamer)
		{
			PlayerInventory = new PlayerInventory(this, false);
			Gamer = gamer;
			Gamer.Tag = this;
			Avatar = new Avatar(Avatar.DefaultDescription);
			RightHand = Avatar.GetAvatarPart(AvatarBone.PropRight);
			AsyncCallback asyncCallback = delegate(IAsyncResult result)
			{
				Avatar.SetDescription(AvatarDescription.EndGetFromGamer(result));
			};
			AvatarDescription.BeginGetFromGamer((Gamer)gamer, asyncCallback, (object)null);
			Avatar.EyePointCamera.FieldOfView = Angle.FromDegrees(90f);
			base.Children.Add(Avatar);
			Collider = true;
			base.Physics = new NoMovePhysics(this);
			FlyMode = false;
			SetupAnimation();
			((Gamer)Gamer).BeginGetProfile((AsyncCallback)delegate(IAsyncResult result)
			{
				try
				{
					Profile = ((Gamer)Gamer).EndGetProfile(result);
					Stream gamerPicture = Profile.GetGamerPicture();
					GamerPicture = Texture2D.FromStream(CastleMinerZGame.Instance.GraphicsDevice, gamerPicture);
				}
				catch
				{
				}
			}, (object)null);
			_shadow = new ModelEntity(_shadowModel);
			_shadow.LocalPosition = new Vector3(0f, 0.05f, 0f);
			_shadow.BlendState = BlendState.AlphaBlend;
			_shadow.DepthStencilState = DepthStencilState.DepthRead;
			_shadow.DrawPriority = 200;
			base.Children.Add(_shadow);
			Avatar.EyePointCamera.Children.Add(GunEyePointCamera);
		}

		public void UpdateGunEyePointCamera(Vector2 location)
		{
			Vector3 localPosition = GunEyePointCamera.LocalPosition;
			localPosition.X = location.X;
			localPosition.Y = location.Y;
			GunEyePointCamera.LocalPosition = localPosition;
		}

		public void SetupAnimation()
		{
			Avatar.Animations.Play("Stand", 0, TimeSpan.Zero);
			_torsoPitchAnimation = Avatar.Animations.Play("Tilt", 1, TimeSpan.Zero);
			_torsoPitchAnimation.Pause();
			Avatar.Animations.Play("GenericIdle", 2, TimeSpan.Zero);
			Avatar.Animations.Play("IdleHead", 4, TimeSpan.Zero);
			HeadAnimation.PingPong = true;
		}

		public string GetDigSound(BlockTypeEnum blockType)
		{
			switch (blockType)
			{
			case BlockTypeEnum.Sand:
				return "Sand";
			case BlockTypeEnum.Snow:
				return "Sand";
			case BlockTypeEnum.Leaves:
				return "leaves";
			default:
				return "punch";
			}
		}

		public virtual void ProcessMessage(Message message)
		{
			if (message is PlayerUpdateMessage)
			{
				if (!Gamer.IsLocal)
				{
					PlayerUpdateMessage playerUpdateMessage = (PlayerUpdateMessage)message;
					playerUpdateMessage.Apply(this);
				}
			}
			else if (message is GunshotMessage)
			{
				GunshotMessage gunshotMessage = (GunshotMessage)message;
				if (TracerManager.Instance != null)
				{
					TracerManager.Instance.AddTracer(FPSCamera.WorldPosition, gunshotMessage.Direction, gunshotMessage.ItemID, message.Sender.Id);
					InventoryItem.InventoryItemClass @class = InventoryItem.GetClass(gunshotMessage.ItemID);
					if (@class.UseSound == null)
					{
						SoundManager.Instance.PlayInstance("GunShot3", SoundEmitter);
					}
					else
					{
						SoundManager.Instance.PlayInstance(@class.UseSound, SoundEmitter);
					}
					if (RightHand.Children.Count > 0 && RightHand.Children[0] is GunEntity)
					{
						GunEntity gunEntity = (GunEntity)RightHand.Children[0];
						gunEntity.ShowMuzzleFlash();
					}
				}
			}
			else if (message is ShotgunShotMessage)
			{
				ShotgunShotMessage shotgunShotMessage = (ShotgunShotMessage)message;
				if (TracerManager.Instance != null)
				{
					for (int i = 0; i < 5; i++)
					{
						TracerManager.Instance.AddTracer(FPSCamera.WorldPosition, shotgunShotMessage.Directions[i], shotgunShotMessage.ItemID, message.Sender.Id);
					}
					InventoryItem.InventoryItemClass class2 = InventoryItem.GetClass(shotgunShotMessage.ItemID);
					if (class2.UseSound == null)
					{
						SoundManager.Instance.PlayInstance("GunShot3", SoundEmitter);
					}
					else
					{
						SoundManager.Instance.PlayInstance(class2.UseSound, SoundEmitter);
					}
					if (RightHand.Children.Count > 0 && RightHand.Children[0] is GunEntity)
					{
						GunEntity gunEntity2 = (GunEntity)RightHand.Children[0];
						gunEntity2.ShowMuzzleFlash();
					}
				}
			}
			else if (message is ChangeCarriedItemMessage)
			{
				ChangeCarriedItemMessage changeCarriedItemMessage = (ChangeCarriedItemMessage)message;
				PutItemInHand(changeCarriedItemMessage.ItemID);
			}
			else if (message is DigMessage)
			{
				DigMessage digMessage = (DigMessage)message;
				if (digMessage.Placing)
				{
					SoundManager.Instance.PlayInstance("Place", SoundEmitter);
				}
				else
				{
					SoundManager.Instance.PlayInstance(GetDigSound(digMessage.BlockType), SoundEmitter);
				}
				if (base.Scene != null && BlockTerrain.Instance.RegionIsLoaded(digMessage.Location))
				{
					ParticleEmitter particleEmitter = _smokeEffect.CreateEmitter(CastleMinerZGame.Instance);
					particleEmitter.Reset();
					particleEmitter.Emitting = true;
					particleEmitter.DrawPriority = 800;
					base.Scene.Children.Add(particleEmitter);
					ParticleEmitter particleEmitter2 = _sparkEffect.CreateEmitter(CastleMinerZGame.Instance);
					particleEmitter2.Reset();
					particleEmitter2.Emitting = true;
					particleEmitter2.DrawPriority = 800;
					base.Scene.Children.Add(particleEmitter2);
					ParticleEmitter particleEmitter3 = _rocksEffect.CreateEmitter(CastleMinerZGame.Instance);
					particleEmitter3.Reset();
					particleEmitter3.Emitting = true;
					particleEmitter3.DrawPriority = 800;
					base.Scene.Children.Add(particleEmitter3);
					Vector3 axis = Vector3.Cross(Vector3.Forward, -digMessage.Direction);
					Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis, Vector3.Forward.AngleBetween(-digMessage.Direction).Radians);
					Vector3 vector = (particleEmitter2.LocalPosition = digMessage.Location);
					Vector3 vector4 = (particleEmitter3.LocalPosition = (particleEmitter.LocalPosition = vector));
					Quaternion quaternion3 = (particleEmitter2.LocalRotation = quaternion);
					Quaternion quaternion6 = (particleEmitter3.LocalRotation = (particleEmitter.LocalRotation = quaternion3));
				}
			}
			else if (message is TimeConnectedMessage)
			{
				if (!Gamer.IsLocal)
				{
					TimeConnectedMessage timeConnectedMessage = (TimeConnectedMessage)message;
					timeConnectedMessage.Apply(this);
				}
			}
			else if (message is CrateFocusMessage)
			{
				CrateFocusMessage crateFocusMessage = (CrateFocusMessage)message;
				FocusCrate = crateFocusMessage.Location;
				FocusCrateItem = crateFocusMessage.ItemIndex;
			}
		}

		public void PlayTeleportEffect()
		{
			Vector3 worldPosition = base.WorldPosition;
			CastleMinerZGame instance = CastleMinerZGame.Instance;
			SoundManager.Instance.PlayInstance("Teleport", SoundEmitter);
		}

		private void SimulateTalking(bool talking, GameTime gameTime)
		{
			if (talking)
			{
				mouthTimer.Update(gameTime.get_ElapsedGameTime());
				if (mouthTimer.Expired)
				{
					mouthTimer.Reset();
					mouthTimer.MaxTime = TimeSpan.FromSeconds(rand.NextDouble() * 0.1 + 0.05);
					Avatar.Expression = mouthLevels[rand.Next(mouthLevels.Length)];
				}
			}
			else
			{
				Avatar.Expression = mouthLevels[0];
			}
		}

		public void ApplyRecoil(Angle amount)
		{
			Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, _fastRand.GetNextValue(-0.25f, 0.25f) * amount.Radians);
			Quaternion quaternion2 = Quaternion.CreateFromAxisAngle(Vector3.UnitX, _fastRand.GetNextValue(0.5f, 1f) * amount.Radians);
			base.RecoilRotation = base.RecoilRotation * quaternion2 * quaternion;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			Angle angle = Quaternion.Identity.AngleBetween(base.RecoilRotation);
			if (angle > Angle.Zero)
			{
				Angle angle2 = recoilDecay * (float)gameTime.get_ElapsedGameTime().TotalSeconds;
				Angle angle3 = angle - angle2;
				if (angle3 < Angle.Zero)
				{
					angle3 = Angle.Zero;
				}
				base.RecoilRotation = Quaternion.Slerp(Quaternion.Identity, base.RecoilRotation, angle3 / angle);
			}
			if (_flyMode)
			{
				base.PlayerPhysics.WorldAcceleration = Vector3.Zero;
			}
			else
			{
				base.PlayerPhysics.WorldAcceleration = Vector3.Lerp(BasicPhysics.Gravity, new Vector3(0f, 4f, 0f), PercentSubmergedWater);
			}
			if (InWater)
			{
				float num = MathHelper.Lerp(0f, 3f, PercentSubmergedWater);
				Vector3 vector = -base.PlayerPhysics.WorldVelocity * (float)gameTime.get_ElapsedGameTime().TotalSeconds * num;
				base.PlayerPhysics.WorldVelocity += vector;
			}
			UpdateAudio(gameTime);
			TimeConnected += gameTime.get_ElapsedGameTime();
			if (IsLocal)
			{
				BlockTerrain.Instance.CenterOn(base.WorldPosition, true);
				BlockTerrain.Instance.EyePos = FPSCamera.WorldPosition;
				BlockTerrain.Instance.ViewVector = FPSCamera.LocalToWorld.Forward;
			}
			SimulateTalking(Gamer.IsTalking, gameTime);
			base.OnUpdate(gameTime);
			if (IsLocal)
			{
				Vector3 localPosition = BlockTerrain.Instance.ClipPositionToLoadedWorld(base.LocalPosition, MovementProbe.Radius);
				localPosition.Y = Math.Min(74f, localPosition.Y);
				base.LocalPosition = localPosition;
			}
			shadowProbe.Init(base.WorldPosition + new Vector3(0f, 1f, 0f), base.WorldPosition + new Vector3(0f, -2.5f, 0f));
			shadowProbe.SkipEmbedded = true;
			BlockTerrain.Instance.Trace(shadowProbe);
			_shadow.Visible = shadowProbe._collides;
			if (_shadow.Visible)
			{
				Vector3 intersection = shadowProbe.GetIntersection();
				Vector3 vector2 = intersection - base.WorldPosition;
				float num2 = Math.Abs(vector2.Y);
				_shadow.LocalPosition = vector2 + new Vector3(0f, 0.05f, 0f);
				int num3 = 2;
				float num4 = num2 / (float)num3;
				_shadow.LocalScale = new Vector3(1f + 2f * num4, 1f, 1f + 2f * num4);
				_shadow.EntityColor = new Color(1f, 1f, 1f, Math.Max(0f, 0.5f * (1f - num4)));
			}
		}

		private bool ClipMovementToAvoidFalling(Vector3 worldPos, ref Vector3 nextPos, ref Vector3 velocity)
		{
			bool result = false;
			BlockFace blockFace = BlockFace.NUM_FACES;
			BlockFace blockFace2 = BlockFace.NUM_FACES;
			FallLockTestResult fallLockTestResult = FallLockTestResult.EMPTY_BLOCK;
			FallLockTestResult fallLockTestResult2 = FallLockTestResult.EMPTY_BLOCK;
			float num = 0f;
			float num2 = 0f;
			if (velocity.X > 0f)
			{
				blockFace = BlockFace.POSX;
			}
			else if (velocity.X < 0f)
			{
				blockFace = BlockFace.NEGX;
			}
			else
			{
				fallLockTestResult = FallLockTestResult.SOLID_BLOCK_NO_WALL;
			}
			if (velocity.Z > 0f)
			{
				blockFace2 = BlockFace.POSZ;
			}
			else if (velocity.Z < 0f)
			{
				blockFace2 = BlockFace.NEGZ;
			}
			else
			{
				fallLockTestResult2 = FallLockTestResult.SOLID_BLOCK_NO_WALL;
			}
			IntVector3 v = IntVector3.FromVector3(worldPos + PlayerAABB.Min);
			v.Y--;
			if (blockFace == BlockFace.POSX && fallLockTestResult != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult = fallLockTestResult3;
					if (fallLockTestResult == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num = (float)v.X + 0.95f - PlayerAABB.Min.X;
					}
				}
			}
			if (blockFace2 == BlockFace.POSZ && fallLockTestResult2 != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace2);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult2 = fallLockTestResult3;
					if (fallLockTestResult2 == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num2 = (float)v.Z + 0.95f - PlayerAABB.Min.Z;
					}
				}
			}
			v.Z = (int)Math.Floor(worldPos.Z + PlayerAABB.Max.Z);
			if (blockFace == BlockFace.POSX && fallLockTestResult != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult = fallLockTestResult3;
					if (fallLockTestResult == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num = (float)v.X + 0.95f - PlayerAABB.Min.X;
					}
				}
			}
			if (blockFace2 == BlockFace.NEGZ && fallLockTestResult2 != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace2);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult2 = fallLockTestResult3;
					if (fallLockTestResult2 == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num2 = (float)v.Z + 0.05f - PlayerAABB.Max.Z;
					}
				}
			}
			v.X = (int)Math.Floor(worldPos.X + PlayerAABB.Max.X);
			v.Z = (int)Math.Floor(worldPos.Z + PlayerAABB.Min.Z);
			if (blockFace == BlockFace.NEGX && fallLockTestResult != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult = fallLockTestResult3;
					if (fallLockTestResult == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num = (float)v.X + 0.05f - PlayerAABB.Max.X;
					}
				}
			}
			if (blockFace2 == BlockFace.POSZ && fallLockTestResult2 != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace2);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult2 = fallLockTestResult3;
					if (fallLockTestResult2 == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num2 = (float)v.Z + 0.95f - PlayerAABB.Min.Z;
					}
				}
			}
			v.Z = (int)Math.Floor(worldPos.Z + PlayerAABB.Max.Z);
			if (blockFace == BlockFace.NEGX && fallLockTestResult != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult = fallLockTestResult3;
					if (fallLockTestResult == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num = (float)v.X + 0.05f - PlayerAABB.Max.X;
					}
				}
			}
			if (blockFace2 == BlockFace.NEGZ && fallLockTestResult2 != FallLockTestResult.SOLID_BLOCK_NO_WALL)
			{
				FallLockTestResult fallLockTestResult3 = BlockTerrain.Instance.FallLockFace(v, blockFace2);
				if (fallLockTestResult3 != 0)
				{
					fallLockTestResult2 = fallLockTestResult3;
					if (fallLockTestResult2 == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
					{
						num2 = (float)v.Z + 0.05f - PlayerAABB.Max.Z;
					}
				}
			}
			if (fallLockTestResult == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
			{
				if (blockFace == BlockFace.POSX)
				{
					if (nextPos.X > num)
					{
						nextPos.X = num;
						velocity.X = 0f;
						result = true;
					}
				}
				else if (nextPos.X < num)
				{
					velocity.X = 0f;
					nextPos.X = num;
					result = true;
				}
			}
			if (fallLockTestResult2 == FallLockTestResult.SOLID_BLOCK_NEEDS_WALL)
			{
				if (blockFace2 == BlockFace.POSZ)
				{
					if (nextPos.Z > num2)
					{
						velocity.Z = 0f;
						nextPos.Z = num2;
						result = true;
					}
				}
				else if (nextPos.Z < num2)
				{
					velocity.Z = 0f;
					nextPos.Z = num2;
					result = true;
				}
			}
			return result;
		}

		public override bool ResolveCollsion(Entity e, out Plane collsionPlane, GameTime dt)
		{
			base.ResolveCollsion(e, out collsionPlane, dt);
			bool flag = false;
			if (e == BlockTerrain.Instance)
			{
				float num = (float)dt.get_ElapsedGameTime().TotalSeconds;
				Vector3 worldPosition = base.WorldPosition;
				Vector3 nextPos = worldPosition;
				Vector3 velocity = base.PlayerPhysics.WorldVelocity;
				Vector3 fwd = velocity;
				fwd.Y = 0f;
				fwd.Normalize();
				float num2 = EnemyManager.Instance.AttentuateVelocity(this, fwd, worldPosition);
				velocity.X *= num2;
				if (velocity.Y > 0f)
				{
					velocity.Y *= num2;
				}
				velocity.Z *= num2;
				float y = velocity.Y;
				InContact = false;
				MovementProbe.SkipEmbedded = true;
				int num3 = 0;
				do
				{
					Vector3 vector = nextPos;
					Vector3 vector2 = Vector3.Multiply(velocity, num);
					nextPos += vector2;
					MovementProbe.Init(vector, nextPos, PlayerAABB);
					BlockTerrain.Instance.Trace(MovementProbe);
					if (MovementProbe._collides)
					{
						flag = true;
						if (MovementProbe._inFace == BlockFace.POSY)
						{
							InContact = true;
							GroundNormal = new Vector3(0f, 1f, 0f);
						}
						if (MovementProbe._startsIn)
						{
							break;
						}
						float num4 = Math.Max(MovementProbe._inT - 0.001f, 0f);
						nextPos = vector + vector2 * num4;
						velocity -= Vector3.Multiply(MovementProbe._inNormal, Vector3.Dot(MovementProbe._inNormal, velocity));
						num *= 1f - num4;
						if (num <= 1E-07f)
						{
							break;
						}
						if (velocity.LengthSquared() <= 1E-06f || Vector3.Dot(base.PlayerPhysics.WorldVelocity, velocity) <= 1E-06f)
						{
							velocity = Vector3.Zero;
							break;
						}
					}
					num3++;
				}
				while (MovementProbe._collides && num3 < 4);
				if (num3 == 4)
				{
					velocity = Vector3.Zero;
				}
				if (InContact && LockedFromFalling && (velocity.X != 0f || velocity.Z != 0f))
				{
					flag = ClipMovementToAvoidFalling(worldPosition, ref nextPos, ref velocity) || flag;
				}
				float num5 = velocity.Y - y;
				base.LocalPosition = nextPos;
				base.PlayerPhysics.WorldVelocity = velocity;
				if (!IsLocal)
				{
					Avatar.Visible = BlockTerrain.Instance.RegionIsLoaded(nextPos);
				}
				else if (!_flyMode && num5 > 15f && velocity.Y < 0.1f)
				{
					Vector3 localPosition = base.LocalPosition;
					localPosition.Y -= 1f;
					InGameHUD.Instance.ApplyDamage((num5 - 15f) * (1f / 15f), localPosition);
				}
				if (Avatar != null && Avatar.AvatarRenderer != null)
				{
					nextPos.Y += 1.2f;
					Vector3 ambient;
					Vector3 directional;
					Vector3 direction;
					BlockTerrain.Instance.GetAvatarColor(nextPos, out ambient, out directional, out direction);
					Avatar.AvatarRenderer.AmbientLightColor = ambient;
					Avatar.AvatarRenderer.LightColor = directional;
					Avatar.AvatarRenderer.LightDirection = direction;
				}
			}
			return flag;
		}

		public override void ProcessInput(FPSControllerMapping controller, GameTime gameTime)
		{
			if (Dead)
			{
				UpdateAnimation(0f, 0f, TorsoPitch, _playerMode, false);
				return;
			}
			if (_flyMode)
			{
				Speed = 5f;
			}
			else
			{
				Speed = MathHelper.Lerp(5f, underWaterSpeed, PercentSubmergedWater);
			}
			base.ProcessInput(controller, gameTime);
			CastleMinerZControllerMapping castleMinerZControllerMapping = (CastleMinerZControllerMapping)controller;
			if ((double)controller.Movement.LengthSquared() < 0.1)
			{
				LockedFromFalling = false;
			}
			UpdateAnimation(controller.Movement.Y, controller.Movement.X, TorsoPitch, _playerMode, UsingTool);
		}

		public void FinishReload()
		{
			Reloading = false;
			if (IsLocal)
			{
				InventoryItem activeInventoryItem = CastleMinerZGame.Instance.GameScreen.HUD.ActiveInventoryItem;
				if (activeInventoryItem is GunInventoryItem)
				{
					GunInventoryItem gunInventoryItem = (GunInventoryItem)activeInventoryItem;
					Reloading = gunInventoryItem.Reload(CastleMinerZGame.Instance.GameScreen.HUD);
				}
			}
		}

		public void UpdateAnimation(float walkAmount, float strafeAmount, Angle torsoPitch, PlayerMode playerMode, bool doAction)
		{
			float num = Math.Abs(walkAmount);
			float num2 = Math.Abs(strafeAmount);
			float num3 = Math.Max(num, num2);
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = MathHelper.Lerp(0.947f, underWaterSpeed, PercentSubmergedWater);
			float num8 = MathHelper.Lerp(4f, underWaterSpeed, PercentSubmergedWater);
			_isRunning = false;
			_isMoveing = num3 >= 0.1f;
			string id = "GenericUse";
			string text = "GenericIdle";
			string text2 = "GenericWalk";
			string text3 = null;
			string text4 = null;
			switch (playerMode)
			{
			case PlayerMode.Fist:
				text = "FistIdle";
				text2 = "FistWalk";
				id = "FistUse";
				break;
			case PlayerMode.Pick:
				text = "PickIdle";
				text2 = "PickWalk";
				id = "PickUse";
				break;
			case PlayerMode.Block:
				text = "BlockIdle";
				text2 = "BlockWalk";
				id = "BlockUse";
				break;
			case PlayerMode.Assault:
				text3 = "GunShoulder";
				text4 = "GunReload";
				if (Shouldering)
				{
					text = "GunShoulderIdle";
					text2 = "GunShoulderWalk";
					id = "GunShoulderShoot";
				}
				else
				{
					text = "GunIdle";
					text2 = "GunRun";
					id = "GunShoot";
				}
				break;
			case PlayerMode.BoltRifle:
				text3 = "RifleShoulder";
				text4 = "RifleReload";
				if (Shouldering)
				{
					text = "RifleShoulderIdle";
					text2 = "RifleShoulderWalk";
					id = "RifleShoulderShoot";
				}
				else
				{
					text = "RifleIdle";
					text2 = "RifleWalk";
					id = "RifleShoot";
				}
				break;
			case PlayerMode.SMG:
				text3 = "SMGShoulder";
				text4 = "SMGReload";
				if (Shouldering)
				{
					text = "SMGShoulderIdle";
					text2 = "SMGShoulderWalk";
					id = "SMGShoulderShoot";
				}
				else
				{
					text = "SMGIdle";
					text2 = "SMGWalk";
					id = "SMGShoot";
				}
				break;
			case PlayerMode.Pistol:
				text3 = "PistolShoulder";
				text4 = "PistolReload";
				if (Shouldering)
				{
					text = "PistolShoulderIdle";
					text2 = "PistolShoulderWalk";
					id = "PistolShoulderShoot";
				}
				else
				{
					text = "PistolIdle";
					text2 = "PistolWalk";
					id = "PistolShoot";
				}
				break;
			case PlayerMode.PumnpShotgun:
				text3 = "GunShoulder";
				text4 = "PumpShotgunReload";
				if (Shouldering)
				{
					text = "GunShoulderIdle";
					text2 = "GunShoulderWalk";
					id = "PumpShotgunShoulderShoot";
				}
				else
				{
					text = "GunIdle";
					text2 = "GunRun";
					id = "PumpShotgunShoot";
				}
				break;
			}
			if (Dead)
			{
				if (Avatar.Animations[5] == null)
				{
					Avatar.Animations.Play("Die", 5, TimeSpan.FromSeconds(0.25));
				}
			}
			else if (Avatar.Animations[5] != null)
			{
				Avatar.Animations.ClearAnimation(5, TimeSpan.FromSeconds(0.25));
			}
			if (UsingTool)
			{
				Avatar.Animations.Play(id, 3, TimeSpan.Zero);
				usingAnimationPlaying = true;
			}
			else if (Avatar.Animations[3] != null)
			{
				Avatar.Animations[3].Looping = false;
				if (Avatar.Animations[3].Finished)
				{
					Avatar.Animations.ClearAnimation(3, TimeSpan.FromSeconds(0.25));
					usingAnimationPlaying = false;
				}
			}
			if (Underwater && !FPSMode)
			{
				if (Avatar.Animations[0] == null || Avatar.Animations[0].Name != "Swim")
				{
					Avatar.Animations.Play("Swim", 0, TimeSpan.FromSeconds(0.25));
				}
				if (num3 < 0.1f)
				{
					Vector3 localVelocity = base.PlayerPhysics.LocalVelocity;
					localVelocity.X = (localVelocity.Z = 0f);
					base.PlayerPhysics.LocalVelocity = localVelocity;
					_isMoveing = false;
				}
				else
				{
					num5 = MathHelper.Lerp(4f, underWaterSpeed, PercentSubmergedWater);
					num6 = MathHelper.Lerp(4f, underWaterSpeed, PercentSubmergedWater);
					num4 = walkAmount * num5;
				}
			}
			else
			{
				if (currentAnimState == AnimationState.Reloading && Avatar.Animations[2].Finished)
				{
					currentAnimState = AnimationState.Unshouldered;
					FinishReload();
				}
				if (Reloading)
				{
					if (currentAnimState == AnimationState.Shouldered && !usingAnimationPlaying)
					{
						currentAnimState = AnimationState.UnShouldering;
						AnimationPlayer animationPlayer = Avatar.Animations.Play(text3, 2, TimeSpan.FromSeconds(0.25));
						animationPlayer.Reversed = true;
					}
					else if (currentAnimState == AnimationState.Unshouldered && !usingAnimationPlaying)
					{
						if (text4 != null)
						{
							currentAnimState = AnimationState.Reloading;
							AnimationPlayer animationPlayer2 = Avatar.Animations.Play(text4, 2, TimeSpan.FromSeconds(0.25));
							if (IsLocal)
							{
								GunInventoryItem gunInventoryItem = InGameHUD.Instance.ActiveInventoryItem as GunInventoryItem;
								if (gunInventoryItem != null)
								{
									animationPlayer2.Speed = (float)(animationPlayer2.Duration.TotalSeconds / gunInventoryItem.GunClass.ReloadTime.TotalSeconds);
								}
							}
							if (ReloadSound == null)
							{
								_reloadCue = SoundManager.Instance.PlayInstance("Reload", SoundEmitter);
							}
							else
							{
								_reloadCue = SoundManager.Instance.PlayInstance(ReloadSound, SoundEmitter);
							}
						}
						else
						{
							FinishReload();
						}
					}
				}
				else if (currentAnimState == AnimationState.Reloading)
				{
					currentAnimState = AnimationState.Unshouldered;
					if (_reloadCue != null && _reloadCue.IsPlaying)
					{
						_reloadCue.Stop(AudioStopOptions.Immediate);
					}
				}
				if (Shouldering && text3 != null && currentAnimState == AnimationState.Unshouldered)
				{
					currentAnimState = AnimationState.Shouldering;
					Avatar.Animations.Play(text3, 2, TimeSpan.Zero);
				}
				if (!Shouldering && text3 != null && currentAnimState == AnimationState.Shouldered)
				{
					currentAnimState = AnimationState.UnShouldering;
					AnimationPlayer animationPlayer3 = Avatar.Animations.Play(text3, 2, TimeSpan.Zero);
					animationPlayer3.Reversed = true;
				}
				if (!Shouldering && text3 == null && currentAnimState == AnimationState.Shouldered)
				{
					currentAnimState = AnimationState.Unshouldered;
				}
				if (currentAnimState == AnimationState.Shouldering && Avatar.Animations[2].Finished)
				{
					currentAnimState = AnimationState.Shouldered;
				}
				if (currentAnimState == AnimationState.UnShouldering && Avatar.Animations[2].Finished)
				{
					currentAnimState = AnimationState.Unshouldered;
				}
				if (!Reloading && (currentAnimState == AnimationState.Unshouldered || currentAnimState == AnimationState.Shouldered))
				{
					if (_isMoveing)
					{
						if (Avatar.Animations[2] == null || Avatar.Animations[2].Name != text2)
						{
							Avatar.Animations.Play(text2, 2, TimeSpan.FromSeconds(0.25));
						}
					}
					else if (Avatar.Animations[2] == null || Avatar.Animations[2].Name != text)
					{
						if (Shouldering)
						{
							Avatar.Animations.Play(text, 2, TimeSpan.Zero);
						}
						else
						{
							Avatar.Animations.Play(text, 2, TimeSpan.FromSeconds(0.25));
						}
					}
				}
				if (num3 < 0.1f)
				{
					if (_flyMode)
					{
						base.PlayerPhysics.WorldVelocity = Vector3.Zero;
					}
					else
					{
						Vector3 localVelocity2 = base.PlayerPhysics.LocalVelocity;
						localVelocity2.X = (localVelocity2.Z = 0f);
						base.PlayerPhysics.LocalVelocity = localVelocity2;
					}
					if (!FPSMode && (Avatar.Animations[0] == null || Avatar.Animations[0].Name != "Stand"))
					{
						Avatar.Animations.Play("Stand", 0, TimeSpan.FromSeconds(0.25));
					}
					_isMoveing = false;
				}
				else
				{
					num4 = 0f;
					num6 = MathHelper.Lerp(4f, underWaterSpeed, PercentSubmergedWater);
					if (num < 0.8f)
					{
						float num9 = (num - 0.1f) / 0.4f;
						num4 = ((!(walkAmount < 0f)) ? ((0f - num9) * num7) : (num9 * num7));
					}
					else
					{
						float num10 = 0.8f + (num - 0.8f);
						num4 = ((!(walkAmount < 0f)) ? ((0f - num10) * num8) : (num10 * num8));
						_isRunning = true;
					}
					if (FPSMode)
					{
						if (num < 0.8f)
						{
							_walkSpeed = (num - 0.1f) / 0.4f;
						}
						else
						{
							_walkSpeed = 0.8f + (num - 0.8f);
						}
					}
					else if (!_flyMode)
					{
						if (num > num2)
						{
							if (num < 0.8f)
							{
								if (Avatar.Animations[0] == null || Avatar.Animations[0].Name != "Walk")
								{
									Avatar.Animations.Play("Walk", 0, TimeSpan.FromSeconds(0.25));
								}
								float num11 = (num - 0.1f) / 0.4f;
								float num13 = (_walkSpeed = (Avatar.Animations[0].Speed = num11));
							}
							else
							{
								if (Avatar.Animations[0] == null || Avatar.Animations[0].Name != "Run")
								{
									Avatar.Animations.Play("Run", 0, TimeSpan.FromSeconds(0.25));
								}
								float num14 = 0.8f + (num - 0.8f);
								float num16 = (_walkSpeed = (Avatar.Animations[0].Speed = num14));
							}
						}
						else
						{
							if (num2 > 0.1f)
							{
								if (num3 > 0.8f)
								{
									if (Avatar.Animations[0] == null || Avatar.Animations[0].Name != "Run")
									{
										Avatar.Animations.Play("Run", 0, TimeSpan.FromSeconds(0.25));
									}
								}
								else if (Avatar.Animations[0] == null || Avatar.Animations[0].Name != "Walk")
								{
									Avatar.Animations.Play("Walk", 0, TimeSpan.FromSeconds(0.25));
								}
							}
							Avatar.Animations[0].Speed = num2;
						}
						Avatar.Animations[0].Reversed = walkAmount < 0f;
					}
				}
				if (FPSMode)
				{
					GunInventoryItem gunInventoryItem2 = InGameHUD.Instance.ActiveInventoryItem as GunInventoryItem;
					float num17 = 1f;
					if (gunInventoryItem2 != null)
					{
						num17 = gunInventoryItem2.GunClass.ShoulderMagnification;
					}
					switch (currentAnimState)
					{
					case AnimationState.Unshouldered:
						FPSCamera.FieldOfView = DefaultFOV;
						Avatar.EyePointCamera.FieldOfView = DefaultAvatarFOV;
						GunEyePointCamera.FieldOfView = DefaultAvatarFOV;
						ControlSensitivity = 1f;
						break;
					case AnimationState.Shouldered:
						FPSCamera.FieldOfView = DefaultFOV / num17;
						GunEyePointCamera.FieldOfView = (Avatar.EyePointCamera.FieldOfView = ShoulderedAvatarFOV);
						ControlSensitivity = 0.25f;
						break;
					case AnimationState.UnShouldering:
						FPSCamera.FieldOfView = Angle.Lerp(DefaultFOV / num17, DefaultFOV, Avatar.Animations[2].Progress);
						GunEyePointCamera.FieldOfView = (Avatar.EyePointCamera.FieldOfView = Angle.Lerp(ShoulderedAvatarFOV, DefaultAvatarFOV, Avatar.Animations[2].Progress));
						ControlSensitivity = 0.25f;
						break;
					case AnimationState.Shouldering:
						FPSCamera.FieldOfView = Angle.Lerp(DefaultFOV, DefaultFOV / num17, Avatar.Animations[2].Progress);
						GunEyePointCamera.FieldOfView = (Avatar.EyePointCamera.FieldOfView = Angle.Lerp(DefaultAvatarFOV, ShoulderedAvatarFOV, Avatar.Animations[2].Progress));
						ControlSensitivity = 0.25f;
						break;
					}
				}
				float num18 = 8f;
				if (_flyMode)
				{
					Matrix localToWorld = FPSCamera.LocalToWorld;
					Vector3 worldVelocity = Vector3.Multiply(localToWorld.Right, strafeAmount * num6 * num18);
					worldVelocity += Vector3.Multiply(localToWorld.Forward, (0f - num4) * num18);
					base.PlayerPhysics.WorldVelocity = worldVelocity;
				}
				else if (InWater && !InContact)
				{
					Matrix localToWorld2 = FPSCamera.LocalToWorld;
					Vector3 worldVelocity2 = base.PlayerPhysics.WorldVelocity;
					Vector3.Multiply(localToWorld2.Right, strafeAmount * num6);
					Vector3 vector = Vector3.Multiply(localToWorld2.Forward, 0f - num4);
					vector.Y *= PercentSubmergedWater;
					if (Math.Abs(vector.Y) < Math.Abs(worldVelocity2.Y))
					{
						vector.Y = worldVelocity2.Y;
					}
					base.PlayerPhysics.WorldVelocity = Vector3.Multiply(localToWorld2.Right, strafeAmount * num6) + vector;
				}
				else if (InContact)
				{
					float num19 = Math.Abs(Vector3.Dot(GroundNormal, Vector3.Up));
					base.PlayerPhysics.LocalVelocity = new Vector3(strafeAmount * num6 * num19, base.PlayerPhysics.LocalVelocity.Y, num4 * num19);
				}
			}
			Avatar.Animations.PlayAnimation(1, _torsoPitchAnimation, TimeSpan.Zero);
			if (FPSMode)
			{
				if (Avatar.Animations[0].Name != "Stand")
				{
					Avatar.Animations.Play("Stand", 0, TimeSpan.Zero);
				}
				Avatar.Animations.ClearAnimation(4, TimeSpan.Zero);
				_torsoPitchAnimation.Progress = 0.5f;
			}
			else
			{
				_torsoPitchAnimation.Progress = (torsoPitch.Degrees + 90f) / 180f;
			}
		}
	}
}
