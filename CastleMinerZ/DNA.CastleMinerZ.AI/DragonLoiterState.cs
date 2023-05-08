using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonLoiterState : DragonBaseState
	{
		public override void Enter(DragonEntity entity)
		{
			if (entity.Target == null)
			{
				entity.LoitersLeft--;
				if (entity.LoitersLeft == 0)
				{
					entity.StateMachine.ChangeState(DragonStates.Default);
				}
			}
			else
			{
				entity.Target = null;
				entity.LoitersLeft = entity.EType.MaxLoiters;
			}
			entity.TargetAltitude = entity.EType.LoiterAltitude;
			entity.TargetVelocity = entity.EType.Speed;
		}

		public override void Update(DragonEntity entity, float dt)
		{
			Vector3 worldPosition = entity.WorldPosition;
			worldPosition.Y = entity.TargetAltitude;
			Vector3 vector = worldPosition - entity.TravelTarget;
			vector.Y = 0f;
			Vector3 forward = entity.LocalToWorld.Forward;
			forward.Y = 0f;
			if (Vector3.Dot(forward, vector) < 0f)
			{
				return;
			}
			if (entity.Target == null && DragonBaseState.SearchForNewTarget(entity, dt))
			{
				if (entity.Target != null)
				{
					entity.StateMachine.ChangeState(DragonBaseState.GetNextAttackType(entity));
					entity.StateMachine.Update(dt);
				}
			}
			else
			{
				float num = vector.Length();
				if (num > entity.EType.BreakOffStrafeDistance)
				{
					entity.StateMachine.ChangeState(MathTools.RandomBool() ? DragonStates.LoiterLeft : DragonStates.LoiterRight);
				}
			}
		}
	}
}
