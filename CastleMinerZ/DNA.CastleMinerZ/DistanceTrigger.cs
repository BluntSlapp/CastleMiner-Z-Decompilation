using DNA.Triggers;

namespace DNA.CastleMinerZ
{
	public abstract class DistanceTrigger : Trigger
	{
		private float _distance;

		private float _currentDistance;

		private float _lastDistance;

		private bool _neverTrigger;

		protected override bool IsSastisfied()
		{
			if (_neverTrigger)
			{
				return false;
			}
			if (_currentDistance > _distance)
			{
				if (_currentDistance - _lastDistance > 10f)
				{
					_neverTrigger = true;
					return false;
				}
				return true;
			}
			return false;
		}

		public DistanceTrigger(bool oneShot, float distance)
			: base(oneShot)
		{
			_distance = distance;
		}

		protected override void OnUpdate()
		{
			_lastDistance = _currentDistance;
			_currentDistance = CastleMinerZGame.Instance.LocalPlayer.LocalPosition.Length();
			base.OnUpdate();
		}
	}
}
