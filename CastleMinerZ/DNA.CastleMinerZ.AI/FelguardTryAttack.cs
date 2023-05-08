namespace DNA.CastleMinerZ.AI
{
	public class FelguardTryAttack : BaseTryAttack
	{
		private string[] _attacks = new string[2] { "Attack1", "atack_3" };

		private float[][] _hitTimes = new float[2][]
		{
			new float[1] { 0.7f },
			new float[1] { 1.5667f }
		};

		private float[] _hitDamages = new float[2] { 0.4f, 0.6f };

		private float[] _hitRanges = new float[2] { 1.6f, 2.1f };

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
				return "Attack1";
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
