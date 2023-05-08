using System;
using DNA.CastleMinerZ.UI;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public abstract class BaseTryAttack : EnemyBaseState
	{
		protected abstract string[] AttackArray { get; }

		protected abstract string RageAnimation { get; }

		protected abstract float[][] HitTimes { get; }

		protected abstract float[] HitDamages { get; }

		protected abstract float[] HitRanges { get; }

		protected abstract float HitDotMultiplier { get; }

		public string GetRandomAttack(BaseZombie entity)
		{
			string[] attackArray = AttackArray;
			return attackArray[entity.AnimationIndex = entity.Rnd.Next(0, attackArray.Length)];
		}

		public override void Enter(BaseZombie entity)
		{
			if (entity.OnGround)
			{
				ZeroVelocity(entity);
			}
			entity.CurrentPlayer = entity.PlayClip(GetRandomAttack(entity), false, TimeSpan.FromSeconds(0.25));
			entity.SwingCount = 2 + entity.Rnd.Next(0, 3);
			entity.HitCount = 0;
			entity.MissCount = entity.Rnd.Next(3, 5);
			entity.CurrentPlayer.Speed = entity.EType.AttackAnimationSpeed;
		}

		public override void Update(BaseZombie entity, float dt)
		{
			if (entity.OnGround)
			{
				ZeroVelocity(entity);
			}
			else
			{
				ReduceVelocity(entity);
			}
			if (entity.IsNearAnimationEnd)
			{
				if (entity.HitCount == 0)
				{
					entity.MissCount--;
				}
				Vector3 vector = entity.Target.WorldPosition - entity.WorldPosition;
				float y = vector.Y;
				vector.Y = 0f;
				if (entity.MissCount <= 0)
				{
					if (Math.Abs(y) > 1.5f && entity.Target.InContact && entity.OnGround)
					{
						entity.StateMachine.ChangeState(entity.EType.GetGiveUpState(entity));
						return;
					}
					entity.MissCount = entity.Rnd.Next(1, 3);
					entity.HitCount = 1;
					entity.AnimationIndex = -1;
					entity.CurrentPlayer = entity.PlayClip(RageAnimation, false, TimeSpan.FromSeconds(0.25));
					return;
				}
				float num = vector.Length();
				if (num >= 1f)
				{
					entity.StateMachine.ChangeState(entity.EType.GetChaseState(entity));
					return;
				}
				if (num > 0.1f)
				{
					float desiredHeading = (float)Math.Atan2(0f - vector.Z, vector.X) + (float)Math.PI / 2f;
					entity.LocalRotation = Quaternion.CreateFromYawPitchRoll(MakeHeading(entity, desiredHeading), 0f, 0f);
				}
				entity.CurrentPlayer = entity.PlayClip(GetRandomAttack(entity), false, TimeSpan.FromSeconds(0.25));
				entity.HitCount = 0;
				entity.SwingCount = 2 + entity.Rnd.Next(0, 3);
			}
			else
			{
				if (!entity.Target.IsLocal || entity.AnimationIndex == -1)
				{
					return;
				}
				float[] array = HitTimes[entity.AnimationIndex];
				if (!(entity.CurrentPlayer.CurrentTime.TotalSeconds >= (double)(array[entity.HitCount] / entity.CurrentPlayer.Speed)))
				{
					return;
				}
				Vector3 vector2 = entity.Target.WorldPosition - entity.WorldPosition;
				if (!(Math.Abs(vector2.Y) < 1.2f))
				{
					return;
				}
				bool flag = false;
				vector2.Y = 0f;
				float num2 = vector2.LengthSquared();
				if ((double)num2 < 0.05)
				{
					flag = true;
				}
				else
				{
					float num3 = HitRanges[entity.AnimationIndex];
					if (num2 < num3 * num3)
					{
						vector2.Normalize();
						if (Vector3.Dot(vector2, Vector3.Normalize(entity.LocalToWorld.Forward)) * HitDotMultiplier > 0.7f)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					InGameHUD.Instance.ApplyDamage(HitDamages[entity.AnimationIndex], entity.WorldPosition);
				}
				entity.HitCount++;
				if (entity.HitCount == array.Length)
				{
					entity.AnimationIndex = -1;
					entity.HitCount = 0;
				}
			}
		}
	}
}
