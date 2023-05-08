using System;

namespace DNA.CastleMinerZ.AI
{
	[Flags]
	public enum DamageType
	{
		BLUNT = 1,
		PIERCING = 2,
		BLADE = 4,
		BULLET = 8,
		SHOTGUN = 0x10
	}
}
