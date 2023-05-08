namespace DNA.Drawing.Actions
{
	public class KillState : State
	{
		protected override void OnStart(Entity entity)
		{
			entity.RemoveFromParent();
			base.OnStart(entity);
		}
	}
}
