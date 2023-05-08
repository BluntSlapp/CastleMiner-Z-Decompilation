namespace DNA.CastleMinerZ
{
	public class TransitionMusicTrigger : DistanceTrigger
	{
		public string _songName;

		protected override bool IsSastisfied()
		{
			if (CastleMinerZGame.Instance.MusicCue.IsPlaying || CastleMinerZGame.Instance.MusicCue.IsPreparing)
			{
				return false;
			}
			return base.IsSastisfied();
		}

		public TransitionMusicTrigger(string songName, float distance)
			: base(true, distance)
		{
			_songName = songName;
		}

		public override void OnTriggered()
		{
			CastleMinerZGame.Instance.PlayMusic(_songName);
			base.OnTriggered();
		}
	}
}
