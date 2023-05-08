namespace DNA.CastleMinerZ.AI
{
	public class StateMachine<T>
	{
		public T _owner;

		public IFSMState<T> _currentState;

		public IFSMState<T> _previousState;

		public IFSMState<T> _globalState;

		public StateMachine(T owner)
		{
			_owner = owner;
			_currentState = null;
			_previousState = null;
			_globalState = null;
		}

		public void ChangeState(IFSMState<T> newState)
		{
			_previousState = _currentState;
			if (_currentState != null)
			{
				_currentState.Exit(_owner);
			}
			_currentState = newState;
			if (_currentState != null)
			{
				_currentState.Enter(_owner);
			}
		}

		public void Revert()
		{
			ChangeState(_previousState);
		}

		public void Update(float dt)
		{
			if (_globalState != null)
			{
				_globalState.Update(_owner, dt);
			}
			if (_currentState != null)
			{
				_currentState.Update(_owner, dt);
			}
		}
	}
}
