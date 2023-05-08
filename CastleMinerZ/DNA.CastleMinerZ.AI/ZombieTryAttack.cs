namespace DNA.CastleMinerZ.AI
{
	public class ZombieTryAttack : BaseTryAttack
	{
		private string[] _attacks = new string[5] { "attack1", "attack2", "attack3", "attack4", "attack5" };

		private float[][] _hitTimes = new float[5][]
		{
			new float[1] { 0.8333f },
			new float[2] { 0.8333f, 1.466667f },
			new float[1] { 0.7333f },
			new float[1] { 0.9333f },
			new float[1] { 1.5f }
		};

		private float[] _hitDamages = new float[5] { 0.4f, 0.3f, 0.4f, 0.4f, 0.6f };

		private float[] _hitRanges = new float[5] { 1.6f, 1.8f, 1.6f, 2.1f, 2.1f };

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
