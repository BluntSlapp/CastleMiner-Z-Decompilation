namespace DNA.Audio.SignalProcessing
{
	public abstract class InPlaceProcessorGroup<InputDataType, InternalDataType> : SignalProcessorGroup<InputDataType, InternalDataType>
	{
		private InternalDataType _internalDataBuffer = default(InternalDataType);

		protected abstract InternalDataType GetInternalBuffer(InputDataType sourceData);

		protected abstract void ConvertFrom(InputDataType inputData, InternalDataType internalData);

		protected abstract void ConvertTo(InternalDataType internalData, InputDataType outputData);

		public override bool ProcessBlock(InputDataType data)
		{
			if (_internalDataBuffer == null)
			{
				_internalDataBuffer = GetInternalBuffer(data);
			}
			ConvertFrom(data, _internalDataBuffer);
			for (int i = 0; i < base.Count; i++)
			{
				SignalProcessor<InternalDataType> signalProcessor = base[i];
				if (signalProcessor.Active && !signalProcessor.ProcessBlock(_internalDataBuffer))
				{
					return false;
				}
			}
			ConvertTo(_internalDataBuffer, data);
			return true;
		}
	}
}
