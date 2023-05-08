namespace DNA.CastleMinerZ.AI
{
	public class ArcherSkeletonEnemyType : SkeletonEnemyType
	{
		public ArcherSkeletonEnemyType(EnemyTypeEnum t, ModelNameEnum model, TextureNameEnum tname, FoundInEnum foundin, SkeletonClassEnum skelclass)
			: base(t, model, tname, foundin, skelclass)
		{
			SpawnRadius = 40;
		}

		public override IFSMState<BaseZombie> GetEmergeState(BaseZombie entity)
		{
			return EnemyStates.ArcherEmerge;
		}

		public override IFSMState<BaseZombie> GetChaseState(BaseZombie entity)
		{
			return EnemyStates.ArcherChase;
		}

		public override IFSMState<BaseZombie> GetAttackState(BaseZombie entity)
		{
			return EnemyStates.ArcherAttack;
		}
	}
}
