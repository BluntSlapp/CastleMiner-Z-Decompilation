namespace DNA.CastleMinerZ.AI
{
	public interface IFSMState<T>
	{
		void Enter(T entity);

		void Update(T entity, float dt);

		void Exit(T entity);
	}
}
