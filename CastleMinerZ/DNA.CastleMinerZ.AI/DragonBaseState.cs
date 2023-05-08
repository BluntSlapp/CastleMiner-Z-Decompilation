using System;
using System.Collections.ObjectModel;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.Utils.Trace;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.AI
{
	public class DragonBaseState : IFSMState<DragonEntity>
	{
		protected struct TargetSearchResult
		{
			public Vector3 vectorToPlayer;

			public Player player;

			public float distance;

			public float light;
		}

		protected const int SHOTS_DURING_HOVER_MIN = 3;

		protected const int SHOTS_DURING_HOVER_MAX = 6;

		protected const float TIME_BETWEEN_SHOTS = 1f;

		protected const float MIN_STEER_DIST = 10f;

		private const float MAX_VIEW_DOT = 0.17f;

		protected static TraceProbe tp = new TraceProbe();

		protected static bool CanSeePosition(DragonEntity entity, Vector3 target)
		{
			Vector3 worldPosition = entity.WorldPosition;
			worldPosition += entity.LocalToWorld.Forward * 7f;
			tp.Init(worldPosition, target);
			BlockTerrain.Instance.Trace(tp);
			return !tp._collides;
		}

		public static DragonBaseState GetNextAttackType(DragonEntity entity)
		{
			if (entity.ChancesToNotAttack == 0 && entity.EType.ChanceOfHoverAttack > MathTools.RandomFloat())
			{
				return DragonStates.HoverAttack;
			}
			return DragonStates.StrafeAttack;
		}

		protected static TargetSearchResult FindBestTarget(DragonEntity entity, Vector3 forward, float maxViewDistance)
		{
			float num = float.MaxValue;
			bool flag = true;
			TargetSearchResult result = default(TargetSearchResult);
			result.player = null;
			Vector3 worldPosition = entity.WorldPosition;
			worldPosition += entity.LocalToWorld.Forward * 7f;
			float num2 = maxViewDistance * maxViewDistance;
			if (CastleMinerZGame.Instance.CurrentNetworkSession != null)
			{
				for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers).Count; i++)
				{
					NetworkGamer networkGamer = ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers)[i];
					if (networkGamer == null)
					{
						continue;
					}
					Player player = (Player)networkGamer.Tag;
					if (player == null || !player.ValidLivingGamer)
					{
						continue;
					}
					Vector3 worldPosition2 = player.WorldPosition;
					worldPosition2.Y += 1.5f;
					Vector3 vector = worldPosition2 - worldPosition;
					float num3 = vector.LengthSquared();
					if (num3 < num2 && num3 > 0.001f)
					{
						flag = false;
						if (!BlockTerrain.Instance.RegionIsLoaded(worldPosition2))
						{
							continue;
						}
						float simpleSunlightAtPoint = BlockTerrain.Instance.GetSimpleSunlightAtPoint(worldPosition2);
						if (!(simpleSunlightAtPoint > 0f))
						{
							continue;
						}
						float num4 = num3 / (simpleSunlightAtPoint * simpleSunlightAtPoint);
						if (!(num4 < num))
						{
							continue;
						}
						vector.Normalize();
						if (Vector3.Dot(vector, forward) > 0.17f)
						{
							tp.Init(worldPosition, worldPosition2);
							BlockTerrain.Instance.Trace(tp);
							if (!tp._collides)
							{
								result.player = player;
								result.distance = (float)Math.Sqrt(num3);
								result.light = simpleSunlightAtPoint;
								result.vectorToPlayer = vector;
								num = num4;
							}
						}
					}
					else if (num3 < MathTools.Square(entity.EType.SpawnDistance * 1.25f))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				entity.Removed = true;
				EnemyManager.Instance.RemoveDragon();
			}
			return result;
		}

		public static bool DoViewCheck(DragonEntity entity, float dt, float interval)
		{
			entity.TimeLeftBeforeNextViewCheck -= dt;
			if (entity.TimeLeftBeforeNextViewCheck <= 0f)
			{
				entity.TimeLeftBeforeNextViewCheck += CalculateNextCheckInterval(entity, interval);
				return true;
			}
			return false;
		}

		public static bool SearchForNewTarget(DragonEntity entity, float dt)
		{
			if (DoViewCheck(entity, dt, entity.EType.SlowViewCheckInterval))
			{
				TargetSearchResult targetSearchResult = FindBestTarget(entity, entity.LocalToWorld.Forward, entity.EType.MaxViewDistance);
				if (targetSearchResult.player != null)
				{
					entity.Target = targetSearchResult.player;
					entity.TravelTarget = targetSearchResult.player.WorldPosition;
					if (entity.ChancesToNotAttack != 0)
					{
						entity.NextSound = DragonSoundEnum.CRY;
					}
					entity.ChancesToNotAttack = 0;
					entity.TimeLeftTilShotsHeard = 0f;
					if (entity.Target != CastleMinerZGame.Instance.LocalPlayer && CastleMinerZGame.Instance.LocalPlayer != null && CastleMinerZGame.Instance.LocalPlayer.ValidGamer && Vector3.DistanceSquared(entity.TravelTarget, CastleMinerZGame.Instance.LocalPlayer.WorldPosition) > 22500f)
					{
						entity.MigrateDragonTo = entity.Target;
						entity.MigrateDragon = true;
					}
					return true;
				}
			}
			if (entity.ChancesToNotAttack != 0)
			{
				entity.TimeLeftTilShotsHeard -= dt;
				if (entity.TimeLeftTilShotsHeard <= 0f)
				{
					float num = entity.EType.MaxViewDistance * entity.EType.MaxViewDistance * 2.25f;
					float num2 = float.MaxValue;
					Vector3 travelTarget = Vector3.Zero;
					Vector3 value = ((entity.ChancesToNotAttack == entity.EType.ChancesToNotAttack) ? entity.WorldPosition : entity.TravelTarget);
					for (int i = 0; i < entity.Gunshots.Count; i++)
					{
						Vector3 vector = entity.Gunshots[i];
						float num3 = Vector3.DistanceSquared(value, vector);
						if (num3 < num && num3 < num2 && BlockTerrain.Instance.RegionIsLoaded(vector))
						{
							num2 = num3;
							travelTarget = vector;
						}
					}
					if (num2 != float.MaxValue)
					{
						entity.ChancesToNotAttack--;
						if (entity.ChancesToNotAttack == 0)
						{
							entity.NextSound = DragonSoundEnum.CRY;
						}
						entity.TravelTarget = travelTarget;
						entity.Target = null;
						entity.TimeLeftTilShotsHeard = entity.EType.ShotHearingInterval;
						return true;
					}
				}
			}
			return false;
		}

		public static float CalculateNextCheckInterval(DragonEntity entity, float interval)
		{
			return MathTools.RandomFloat(interval * 0.75f, interval * 1.25f);
		}

		public static float GetHeading(Vector3 forward, float defaultHeading)
		{
			if (forward.X != 0f || forward.Z != 0f)
			{
				return (float)Math.Atan2(0f - forward.X, 0f - forward.Z);
			}
			return defaultHeading;
		}

		public static Vector3 MakeYawVector(float yaw)
		{
			return new Vector3((float)(0.0 - Math.Sin(yaw)), 0f, (float)(0.0 - Math.Cos(yaw)));
		}

		public static BaseDragonWaypoint GetBaseWaypoint(DragonEntity entity, DragonAnimEnum nextanim)
		{
			BaseDragonWaypoint result = default(BaseDragonWaypoint);
			result.Position = entity.LocalPosition;
			result.Velocity = entity.LocalToWorld.Forward * entity.Velocity;
			result.HostTime = entity.DragonTime;
			result.Animation = nextanim;
			result.TargetRoll = entity.TargetRoll;
			result.Sound = entity.NextSound;
			entity.NextSound = DragonSoundEnum.NONE;
			return result;
		}

		public static void SendAttack(DragonEntity entity, bool animatedAttack, DragonAnimEnum nextanim)
		{
			BaseDragonWaypoint baseWaypoint = GetBaseWaypoint(entity, nextanim);
			Vector3 target;
			if (animatedAttack)
			{
				target = entity.TravelTarget;
			}
			else
			{
				target = entity.ShootTarget;
				entity.ShotPending = false;
			}
			DragonAttackMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, baseWaypoint, target, entity.GetNextFireballIndex(), animatedAttack);
			entity.UpdatesSent++;
		}

		public static void SendRegularUpdate(DragonEntity entity, DragonAnimEnum nextanim)
		{
			if (entity.CurrentAnimation != DragonAnimEnum.ATTACK && entity.ShotPending)
			{
				SendAttack(entity, false, nextanim);
				return;
			}
			UpdateDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, GetBaseWaypoint(entity, nextanim));
			entity.UpdatesSent++;
		}

		public virtual void SendUpdateMessage(DragonEntity entity)
		{
			SendRegularUpdate(entity, entity.CurrentAnimation);
		}

		public static float SteerTowardTarget(DragonEntity entity, out Vector3 dest)
		{
			Vector3 worldPosition = entity.WorldPosition;
			if (entity.Target != null)
			{
				Vector3 worldPosition2 = entity.Target.WorldPosition;
				worldPosition2.Y += 1.5f;
				if (CanSeePosition(entity, worldPosition2))
				{
					entity.TravelTarget = entity.Target.WorldPosition;
				}
			}
			dest = entity.TravelTarget - worldPosition;
			dest.Y = 0f;
			float num = dest.Length();
			if (num > 10f)
			{
				entity.TargetYaw = MathHelper.WrapAngle(GetHeading(dest, entity.TargetYaw));
			}
			return num;
		}

		public virtual void Enter(DragonEntity entity)
		{
		}

		public virtual void Update(DragonEntity entity, float dt)
		{
		}

		public virtual void Exit(DragonEntity entity)
		{
		}
	}
}
