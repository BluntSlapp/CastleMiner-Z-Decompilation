namespace DNA.SignalProcessing
{
	public interface ISpectralTransform
	{
		void Transform(float[] data);

		void InverseTransform(float[] data);

		void Transform(float[,] data);

		void InverseTransform(float[,] data);

		void Transform(float[,,] data);

		void InverseTransform(float[,,] data);
	}
}
