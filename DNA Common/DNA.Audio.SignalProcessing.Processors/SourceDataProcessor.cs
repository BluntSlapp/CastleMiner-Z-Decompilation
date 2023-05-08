using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class SourceDataProcessor : SignalProcessor<RawPCMData>
	{
		private RawPCMData _sourceData;

		private int _readPos;

		private bool _loop = true;

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}

		public override int? SampleRate
		{
			get
			{
				return _sourceData.SampleRate;
			}
		}

		public override int? Channels
		{
			get
			{
				return _sourceData.Channels;
			}
		}

		public int Position
		{
			get
			{
				return _readPos;
			}
			set
			{
				_readPos = value;
			}
		}

		public SourceDataProcessor(RawPCMData sourceData)
		{
			_sourceData = sourceData;
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			if (_readPos + data.ChannelData.Length < _sourceData.ChannelData.Length)
			{
				Buffer.BlockCopy(_sourceData.ChannelData, _readPos, data.ChannelData, 0, data.ChannelData.Length);
				_readPos += data.ChannelData.Length;
			}
			else
			{
				for (int i = 0; i < data.ChannelData.Length; i++)
				{
					if (_readPos >= _sourceData.ChannelData.Length)
					{
						if (!Loop)
						{
							return false;
						}
						_readPos = 0;
					}
					data.ChannelData[i] = _sourceData.ChannelData[_readPos];
					_readPos++;
				}
			}
			return true;
		}

		public override void OnStart()
		{
			base.OnStart();
		}
	}
}
