namespace DNA.CastleMinerZ.AI
{
	public class EnemyStates
	{
		public static IFSMState<BaseZombie> ZombieEmerge = new ZombieEmerge();

		public static IFSMState<BaseZombie> ZombieTryAttack = new ZombieTryAttack();

		public static IFSMState<BaseZombie> ZombieGiveUp = new ZombieGiveUp();

		public static IFSMState<BaseZombie> ZombieHit = new ZombieHit();

		public static IFSMState<BaseZombie> ZombieDie = new ZombieDie();

		public static IFSMState<BaseZombie> SkeletonTryAttack = new SkeletonTryAttack();

		public static IFSMState<BaseZombie> AxeSkeletonTryAttack = new AxeSkeletonTryAttack();

		public static IFSMState<BaseZombie> SkeletonGiveUp = new SkeletonGiveUp();

		public static IFSMState<BaseZombie> SkeletonHit = new SkeletonHit();

		public static IFSMState<BaseZombie> SkeletonDie = new SkeletonDie();

		public static IFSMState<BaseZombie> ArcherEmerge = new ArcherSkeletonEmerge();

		public static IFSMState<BaseZombie> ArcherChase = new ArcherSkeletonChase();

		public static IFSMState<BaseZombie> ArcherIdle = new ArcherSkeletonIdle();

		public static IFSMState<BaseZombie> ArcherAttack = new ArcherSkeletonTryAttack();

		public static IFSMState<BaseZombie> FelguardEmerge = new FelguardEmerge();

		public static IFSMState<BaseZombie> FelguardChase = new FelguardChase();

		public static IFSMState<BaseZombie> FelguardTryAttack = new FelguardTryAttack();

		public static IFSMState<BaseZombie> FelguardGiveUp = new FelguardGiveUp();

		public static IFSMState<BaseZombie> FelguardHit = new FelguardHit();

		public static IFSMState<BaseZombie> FelguardDie = new FelguardDie();

		public static IFSMState<BaseZombie> Chase = new ZombieChase();
	}
}
