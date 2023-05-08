namespace DNA.Threading
{
	public interface ICancelMonitor
	{
		bool Cancelable { set; }

		bool Canceled { get; }

		void AssertContinue();
	}
}
