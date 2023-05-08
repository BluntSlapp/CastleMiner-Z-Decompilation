namespace DNA.Threading
{
	public interface IProgressMonitor : IStatusMonitor, ICancelMonitor
	{
		Percentage Complete { set; }

		bool IsComplex { get; }

		IProgressMonitor[] AddTasks(int count);

		IProgressMonitor[] AddTasks(float[] weights);

		IProgressMonitor AddTask(float weight);

		void RemoveTask(IProgressMonitor task);

		void Clear();
	}
}
