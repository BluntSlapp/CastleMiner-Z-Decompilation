namespace DNA.Drawing.Curves
{
	public interface ISlopeFunction : IFunction
	{
		RangeF SlopeRange { get; }

		float GetSlope(float x);
	}
}
