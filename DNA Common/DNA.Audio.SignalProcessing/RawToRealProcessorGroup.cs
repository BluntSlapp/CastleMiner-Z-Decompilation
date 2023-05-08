namespace DNA.Audio.SignalProcessing
{
	public class RawToRealProcessorGroup : InPlaceProcessorGroup<RawPCMData, RealPCMData>
	{
		protected override RealPCMData GetInternalBuffer(RawPCMData sourceData)
		{
			return new RealPCMData(sourceData.Channels, sourceData.Samples, sourceData.SampleRate);
		}

		protected override void ConvertFrom(RawPCMData inputData, RealPCMData internalData)
		{
			internalData.Convert(inputData);
		}

		protected override void ConvertTo(RealPCMData internalData, RawPCMData outputData)
		{
			outputData.Convert(internalData, 16);
		}
	}
}
