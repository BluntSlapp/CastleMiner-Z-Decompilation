namespace DNA.CastleMinerZ.AI
{
	public class AxeSkeletonTryAttack : BaseTryAttack
	{
		private string[] _attacks = new string[11]
		{
			"attack1", "attack2", "attack1", "attack2", "attack1", "attack2", "attack3", "axes_atack1", "axes_atack2", "axes_atack1",
			"axes_atack2"
		};

		private float[] _hitRanges = new float[11]
		{
			1.6f, 2f, 1.6f, 2f, 1.6f, 2f, 2.1f, 1.6f, 1.6f, 1.6f,
			1.6f
		};

		private float[][] _hitTimes = new float[11][]
		{
			new float[1] { 0.7f },
			new float[1] { 0.9333f },
			new float[1] { 0.7f },
			new float[1] { 0.9333f },
			new float[1] { 0.7f },
			new float[1] { 0.9333f },
			new float[1] { 1.5667f },
			new float[4] { 0.2667f, 0.5667f, 0.933f, 1.2667f },
			new float[4] { 0.3333f, 0.6667f, 1.033f, 1.3667f },
			new float[4] { 0.2667f, 0.5667f, 0.933f, 1.2667f },
			new float[4] { 0.3333f, 0.6667f, 1.033f, 1.3667f }
		};

		private float[] _hitDamages = new float[11]
		{
			0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f, 0.6f, 0.2f, 0.2f, 0.2f,
			0.2f
		};

		protected override string[] AttackArray
		{
			get
			{
				return _attacks;
			}
		}

		protected override string RageAnimation
		{
			get
			{
				return "enraged";
			}
		}

		protected override float[][] HitTimes
		{
			get
			{
				return _hitTimes;
			}
		}

		protected override float[] HitDamages
		{
			get
			{
				return _hitDamages;
			}
		}

		protected override float[] HitRanges
		{
			get
			{
				return _hitRanges;
			}
		}

		protected override float HitDotMultiplier
		{
			get
			{
				return 1f;
			}
		}
	}
}
