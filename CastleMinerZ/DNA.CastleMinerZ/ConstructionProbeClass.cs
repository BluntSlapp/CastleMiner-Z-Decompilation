using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.Utils.Trace;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ
{
	public class ConstructionProbeClass : TraceProbe
	{
		public bool HitZombie;

		public bool CheckEnemies;

		public BaseZombie EnemyHit;

		public override bool AbleToBuild
		{
			get
			{
				if (base.AbleToBuild)
				{
					return !HitZombie;
				}
				return false;
			}
		}

		public void Init(Vector3 start, Vector3 end, bool checkEnemies)
		{
			base.Init(start, end);
			HitZombie = false;
			EnemyHit = null;
			CheckEnemies = checkEnemies;
		}

		public void Trace()
		{
			HitZombie = false;
			EnemyHit = null;
			if (CheckEnemies)
			{
				IShootableEnemy shootableEnemy = EnemyManager.Instance.Trace(this, true);
				if (shootableEnemy is BaseZombie)
				{
					EnemyHit = (BaseZombie)shootableEnemy;
					HitZombie = true;
				}
			}
			else
			{
				BlockTerrain.Instance.Trace(this);
			}
		}
	}
}
