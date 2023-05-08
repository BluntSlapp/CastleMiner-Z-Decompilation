namespace DNA.Audio.SignalProcessing
{
	public class RawProcessorGroup : InPlaceProcessorGroup<RawPCMData, RawPCMData>
	{
		protected override RawPCMData GetInternalBuffer(RawPCMData sourceData)
		{
			return sourceData;
		}

		protected override void ConvertFrom(RawPCMData inputData, RawPCMData internalData)
		{
		}

		protected override void ConvertTo(RawPCMData internalData, RawPCMData outputData)
		{
		}
	}
}
