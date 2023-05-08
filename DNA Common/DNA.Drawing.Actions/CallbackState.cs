using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class CallbackState : State
	{
		public delegate bool StateCallBack(Entity e, CallbackState state, object data, GameTime time);

		private bool _finished;

		private object _data;

		public override bool Complete
		{
			get
			{
				return _finished;
			}
		}

		public event StateCallBack Callback;

		public CallbackState(StateCallBack callback, object data)
		{
			Callback += callback;
			_data = data;
		}

		protected override void OnTick(DNAGame game, Entity entity, GameTime deltaT)
		{
			_finished = true;
			if (this.Callback != null)
			{
				_finished = this.Callback(entity, this, _data, deltaT);
			}
			base.OnTick(game, entity, deltaT);
		}
	}
}
