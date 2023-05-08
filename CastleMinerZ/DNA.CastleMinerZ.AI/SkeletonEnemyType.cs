namespace DNA.CastleMinerZ.AI
{
	public class SkeletonEnemyType : EnemyType
	{
		public SkeletonClassEnum SkeletonClass;

		public SkeletonEnemyType(EnemyTypeEnum t, ModelNameEnum model, TextureNameEnum tname, FoundInEnum foundin, SkeletonClassEnum skelclass)
			: base(t, model, tname, foundin)
		{
			SkeletonClass = skelclass;
			ChanceOfBulletStrike = 0.6f;
			SpawnRadius = 10;
		}

		public override float GetMaxSpeed()
		{
			return MathTools.RandomFloat(2f, 5.5f);
		}

		public override IFSMState<BaseZombie> GetEmergeState(BaseZombie entity)
		{
			return EnemyStates.Chase;
		}

		public override IFSMState<BaseZombie> GetAttackState(BaseZombie entity)
		{
			if (SkeletonClass == SkeletonClassEnum.AXES)
			{
				return EnemyStates.AxeSkeletonTryAttack;
			}
			return EnemyStates.SkeletonTryAttack;
		}

		public override IFSMState<BaseZombie> GetGiveUpState(BaseZombie entity)
		{
			return EnemyStates.SkeletonGiveUp;
		}

		public override IFSMState<BaseZombie> GetHitState(BaseZombie entity)
		{
			return EnemyStates.SkeletonHit;
		}

		public override IFSMState<BaseZombie> GetDieState(BaseZombie entity)
		{
			return EnemyStates.SkeletonDie;
		}

		public override float GetDamageTypeMultiplier(DamageType damageType, bool headShot)
		{
			float num = 1f;
			if ((damageType & DamageType.PIERCING) != 0)
			{
				num *= 0.5f;
			}
			else if ((damageType & DamageType.SHOTGUN) != 0)
			{
				num *= 1.5f;
			}
			else if ((damageType & DamageType.BLADE) != 0)
			{
				num *= 0.75f;
			}
			if (headShot)
			{
				num *= 2f;
			}
			return num;
		}
	}
}
