namespace DNA.CastleMinerZ.Terrain
{
	public enum ChunkCacheCommandEnum
	{
		MOD,
		FETCHDELTAFORTERRAIN,
		DELTAARRIVED,
		BECOMEHOST,
		BECOMECLIENT,
		FLUSH,
		HOSTCHANGED,
		HEARTBEAT,
		RESETANDSTOP,
		SHUTDOWN,
		RESETWAITINGCHUNKS,
		ASKHOSTFORREMOTECHUNKS,
		SENDREMOTECHUNKLIST,
		REMOTECHUNKLISTARRIVED
	}
}
