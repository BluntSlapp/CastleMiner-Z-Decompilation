using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class EnemyBaseState : IFSMState<BaseZombie>
	{
		public void ZeroVelocity(BaseZombie entity)
		{
			Vector3 worldVelocity = entity.PlayerPhysics.WorldVelocity;
			worldVelocity.X = 0f;
			worldVelocity.Z = 0f;
			entity.PlayerPhysics.WorldVelocity = worldVelocity;
		}

		public void ReduceVelocity(BaseZombie entity)
		{
			Vector3 worldVelocity = entity.PlayerPhysics.WorldVelocity;
			worldVelocity.X *= 0.99f;
			worldVelocity.Z *= 0.99f;
			entity.PlayerPhysics.WorldVelocity = worldVelocity;
		}

		public float MakeHeading(BaseZombie entity, float desiredHeading)
		{
			return desiredHeading + entity.EType.Facing;
		}

		public virtual bool IsRestartable()
		{
			return false;
		}

		public void Restart(BaseZombie entity)
		{
			if (IsRestartable())
			{
				entity.StateMachine.Revert();
			}
			else
			{
				entity.StateMachine.ChangeState(entity.EType.GetRestartState(entity));
			}
		}

		public virtual void HandleSpeedUp(BaseZombie entity)
		{
		}

		public virtual void Enter(BaseZombie entity)
		{
		}

		public virtual void Update(BaseZombie entity, float dt)
		{
		}

		public virtual void Exit(BaseZombie entity)
		{
		}
	}
}
