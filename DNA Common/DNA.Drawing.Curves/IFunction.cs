namespace DNA.Drawing.Curves
{
	public interface IFunction
	{
		RangeF Range { get; }

		float GetValue(float x);
	}
}
