using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class OutputProcessor : BlockingSignalProcessor<RawPCMData>, IDisposable
	{
		private DynamicSoundEffectInstance _playbackBuffer;

		public bool WaitForOutput;

		private Queue<byte[]> waitingBuffers = new Queue<byte[]>();

		private Stack<byte[]> bufferRecycle = new Stack<byte[]>();

		private byte[] currentBuffer;

		private bool _disposed;

		private int BlocksReady
		{
			get
			{
				return waitingBuffers.Count;
			}
		}

		protected override bool IsReady
		{
			get
			{
				if (BlocksReady >= 3)
				{
					return !WaitForOutput;
				}
				return true;
			}
		}

		private void Initalize(int sampleRate)
		{
			_playbackBuffer = new DynamicSoundEffectInstance(sampleRate, AudioChannels.Mono);
			_playbackBuffer.add_BufferNeeded((EventHandler<EventArgs>)_playbackBuffer_BufferNeeded);
			_playbackBuffer.Play();
		}

		private void PushBuffer(RawPCMData data)
		{
			lock (this)
			{
				byte[] array = ((bufferRecycle.Count != 0) ? bufferRecycle.Pop() : new byte[data.ChannelData.Length]);
				Buffer.BlockCopy(data.ChannelData, 0, array, 0, array.Length);
				waitingBuffers.Enqueue(array);
			}
		}

		private void PopBuffer()
		{
			lock (this)
			{
				if (currentBuffer != null)
				{
					bufferRecycle.Push(currentBuffer);
				}
				currentBuffer = null;
				if (waitingBuffers.Count > 0)
				{
					currentBuffer = waitingBuffers.Dequeue();
					_playbackBuffer.SubmitBuffer(currentBuffer);
				}
			}
		}

		public override void OnStart()
		{
			if (_playbackBuffer != null && _playbackBuffer.State == SoundState.Paused)
			{
				_playbackBuffer.Resume();
			}
			base.OnStart();
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			PushBuffer(data);
			if (!base.ProcessBlock(data))
			{
				return false;
			}
			if (_playbackBuffer == null)
			{
				Initalize(data.SampleRate);
			}
			if (currentBuffer == null)
			{
				PopBuffer();
			}
			return true;
		}

		private void _playbackBuffer_BufferNeeded(object sender, EventArgs e)
		{
			for (int num = 2 - _playbackBuffer.PendingBufferCount; num > 0; num--)
			{
				PopBuffer();
			}
			SignalDataReady();
		}

		public override void OnStop()
		{
			_playbackBuffer.Pause();
			SignalDataReady();
			base.OnStop();
		}

		public void Dispose()
		{
			if (_disposed)
			{
				_playbackBuffer.Stop();
				_playbackBuffer.Dispose();
				_disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		~OutputProcessor()
		{
			Dispose();
		}
	}
}
