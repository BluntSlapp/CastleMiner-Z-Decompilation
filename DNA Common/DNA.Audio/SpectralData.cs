using System;
using DNA.Collections;
using DNA.Data.Units;

namespace DNA.Audio
{
	public class SpectralData
	{
		private FrequencyPair[][] _channelData = new FrequencyPair[1][] { new FrequencyPair[0] };

		public int Channels
		{
			get
			{
				return _channelData.Length;
			}
		}

		public int FrequencyCount
		{
			get
			{
				return _channelData[0].Length;
			}
		}

		public FrequencyPair[] GetData(int channel)
		{
			return _channelData[channel];
		}

		public void CopyTo(SpectralData data)
		{
			if (Channels != data.Channels || FrequencyCount != data.FrequencyCount)
			{
				data._channelData = ArrayTools.AllocSquareJaggedArray<FrequencyPair>(Channels, FrequencyCount);
			}
			for (int i = 0; i < Channels; i++)
			{
				_channelData[i].CopyTo(data._channelData[i], 0);
			}
		}

		public void SetZero()
		{
			for (int i = 0; i < Channels; i++)
			{
				for (int j = 0; j < _channelData[i].Length; j++)
				{
					_channelData[i][j].Magnitude = 0f;
					_channelData[i][j].Value = Frequency.Zero;
				}
			}
		}

		public SpectralData(int channels, int frequencies)
		{
			_channelData = ArrayTools.AllocSquareJaggedArray<FrequencyPair>(channels, frequencies);
		}

		public void Convert(RealPCMData data)
		{
			throw new NotImplementedException();
		}
	}
}
