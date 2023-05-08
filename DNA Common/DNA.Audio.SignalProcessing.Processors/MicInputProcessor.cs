using System;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class MicInputProcessor : BlockingSignalProcessor<RawPCMData>, IDisposable
	{
		private int _micBufferSize;

		private int _readPos;

		private int _writePos;

		private int _dataLength;

		private int _micSampleRate;

		private Microphone _microphone;

		private byte[] _micBuffer;

		private EventHandler<EventArgs> handler;

		private object readerLock = new object();

		private int _windowSize;

		private bool _disposed;

		public override int? SampleRate
		{
			get
			{
				return _micSampleRate;
			}
		}

		public override int? Channels
		{
			get
			{
				return 1;
			}
		}

		protected override bool IsReady
		{
			get
			{
				return _dataLength >= _windowSize;
			}
		}

		public event EventHandler MicrophoneDisconnected;

		public MicInputProcessor(Microphone mic)
		{
			_microphone = mic;
			_microphone.set_BufferDuration(TimeSpan.FromMilliseconds(100.0));
			handler = _microphone_BufferReady;
			_microphone.add_BufferReady(handler);
			_micBufferSize = _microphone.GetSampleSizeInBytes(_microphone.get_BufferDuration());
			_micBuffer = new byte[_micBufferSize * 3];
			_micSampleRate = _microphone.SampleRate;
		}

		private void _microphone_BufferReady(object sender, EventArgs e)
		{
			if (_writePos >= _micBuffer.Length)
			{
				_writePos = 0;
			}
			int num = _micBufferSize;
			int num2 = _micBuffer.Length - _dataLength;
			if (num2 < num)
			{
				return;
			}
			int num3 = _micBuffer.Length - _writePos;
			if (num3 < _micBufferSize)
			{
				try
				{
					_microphone.GetData(_micBuffer, _writePos, num3);
				}
				catch (Exception)
				{
					DoMicDisconnect();
					return;
				}
				_writePos += num3;
				num -= num3;
			}
			if (_writePos >= _micBuffer.Length)
			{
				_writePos = 0;
			}
			try
			{
				_microphone.GetData(_micBuffer, _writePos, num);
			}
			catch (NoMicrophoneConnectedException)
			{
				DoMicDisconnect();
				return;
			}
			_writePos += num;
			lock (readerLock)
			{
				_dataLength += _micBufferSize;
			}
			SignalDataReady();
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			_windowSize = data.ChannelData.Length;
			if (!base.ProcessBlock(data))
			{
				return false;
			}
			int num = data.ChannelData.Length;
			for (int i = 0; i < num; i++)
			{
				if (_readPos >= _micBuffer.Length)
				{
					_readPos = 0;
				}
				data.ChannelData[i] = _micBuffer[_readPos++];
			}
			lock (readerLock)
			{
				_dataLength -= num;
			}
			return true;
		}

		public override void OnStart()
		{
			_microphone.Start();
			base.OnStart();
		}

		public override void OnStop()
		{
			try
			{
				_microphone.Stop();
			}
			catch
			{
			}
			base.OnStop();
		}

		private void DoMicDisconnect()
		{
			try
			{
				_microphone.Start();
				return;
			}
			catch
			{
			}
			try
			{
				_microphone.Stop();
			}
			catch
			{
			}
			if (this.MicrophoneDisconnected != null)
			{
				this.MicrophoneDisconnected(this, new EventArgs());
			}
		}

		public void Dispose()
		{
			if (_disposed)
			{
				_microphone.remove_BufferReady(handler);
				_disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		~MicInputProcessor()
		{
			Dispose();
		}
	}
}
