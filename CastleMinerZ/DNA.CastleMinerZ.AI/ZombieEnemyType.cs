namespace DNA.CastleMinerZ.AI
{
	public class ZombieEnemyType : EnemyType
	{
		public ZombieEnemyType(EnemyTypeEnum t, ModelNameEnum model, TextureNameEnum tname, FoundInEnum foundin)
			: base(t, model, tname, foundin)
		{
			ChanceOfBulletStrike = 1f;
			SpawnRadius = 20;
			AttackAnimationSpeed = 2f;
			DieAnimationSpeed = 2f;
			HitAnimationSpeed = 2f;
			SpawnAnimationSpeed = 3f;
			HasRunFast = true;
			BaseSlowSpeed = 2.5f;
			RandomSlowSpeed = 0.5f;
		}

		public override IFSMState<BaseZombie> GetEmergeState(BaseZombie entity)
		{
			return EnemyStates.ZombieEmerge;
		}

		public override IFSMState<BaseZombie> GetAttackState(BaseZombie entity)
		{
			return EnemyStates.ZombieTryAttack;
		}

		public override IFSMState<BaseZombie> GetGiveUpState(BaseZombie entity)
		{
			return EnemyStates.ZombieGiveUp;
		}

		public override IFSMState<BaseZombie> GetHitState(BaseZombie entity)
		{
			return EnemyStates.ZombieHit;
		}

		public override IFSMState<BaseZombie> GetDieState(BaseZombie entity)
		{
			return EnemyStates.ZombieDie;
		}

		public override float GetDamageTypeMultiplier(DamageType damageType, bool headShot)
		{
			float num = 1f;
			if ((damageType & DamageType.BLUNT) != 0)
			{
				num *= 0.5f;
			}
			else if ((damageType & DamageType.BLADE) != 0)
			{
				num *= 0.75f;
			}
			if (headShot)
			{
				num *= 2.5f;
			}
			return num;
		}
	}
}
