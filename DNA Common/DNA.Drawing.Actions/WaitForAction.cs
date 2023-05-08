namespace DNA.Drawing.Actions
{
	public class WaitForAction : State
	{
		private State _otherAction;

		public override bool Complete
		{
			get
			{
				return _otherAction.Complete;
			}
		}

		public WaitForAction(State otherACtion)
		{
			_otherAction = otherACtion;
		}
	}
}
