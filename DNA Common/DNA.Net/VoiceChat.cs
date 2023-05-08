using System;
using DNA.Audio;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;

namespace DNA.Net
{
	public class VoiceChat
	{
		private Microphone _microphone;

		private EventHandler<EventArgs> handler;

		private DynamicSoundEffectInstance _playbackEffect;

		private LocalNetworkGamer _gamer;

		private byte[] _micBuffer;

		private byte[] _sendBuffer;

		private byte[] _playBuffer;

		private static byte[] _pcmToALawMap;

		private static short[] _aLawToPcmMap;

		static VoiceChat()
		{
			BuildALawMap();
			Microphone @default = Microphone.Default;
		}

		public VoiceChat(LocalNetworkGamer gamer)
		{
			_gamer = gamer;
			_microphone = AudioTools.GetMic(gamer.SignedInGamer);
			_microphone.set_BufferDuration(TimeSpan.FromMilliseconds(100.0));
			handler = _microphone_BufferReady;
			_microphone.add_BufferReady(handler);
			_micBuffer = new byte[_microphone.GetSampleSizeInBytes(_microphone.get_BufferDuration())];
			_playBuffer = new byte[_micBuffer.Length];
			_sendBuffer = new byte[_micBuffer.Length / 2];
			_playbackEffect = new DynamicSoundEffectInstance(_microphone.SampleRate, AudioChannels.Mono);
			_playbackEffect.SubmitBuffer(_micBuffer);
			_playbackEffect.Play();
		}

		private void _microphone_BufferReady(object sender, EventArgs e)
		{
			_microphone.GetData(_micBuffer);
			int num = _micBuffer.Length / 2;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				short num3 = _micBuffer[num2 + 1];
				short num4 = (short)(_micBuffer[num2] << 8);
				short num5 = (short)(num3 | num4);
				_sendBuffer[i] = _pcmToALawMap[num5];
				num2 += 2;
			}
			VoiceChatMessage.Send(_gamer, _sendBuffer);
		}

		public void ProcessMessage(VoiceChatMessage message)
		{
			int num = 0;
			for (int i = 0; i < message.AudioBuffer.Length; i++)
			{
				short num2 = _aLawToPcmMap[message.AudioBuffer[i]];
				_playBuffer[num++] = (byte)((uint)num2 & 0xFFu);
				_playBuffer[num++] = (byte)(num2 >> 8);
			}
			_playbackEffect.SubmitBuffer(_playBuffer);
		}

		private static void BuildALawMap()
		{
			_pcmToALawMap = new byte[65536];
			for (int i = -32768; i <= 32767; i++)
			{
				_pcmToALawMap[i & 0xFFFF] = EncodeALawSample(i);
			}
			_aLawToPcmMap = new short[256];
			for (byte b = 0; b < byte.MaxValue; b = (byte)(b + 1))
			{
				_aLawToPcmMap[b] = DecodeALawSample(b);
			}
		}

		private static byte EncodeALawSample(int pcm)
		{
			int num = (pcm & 0x8000) >> 8;
			if (num != 0)
			{
				pcm = -pcm;
			}
			if (pcm > 32767)
			{
				pcm = 32767;
			}
			int num2 = 7;
			int num3 = 16384;
			while ((pcm & num3) == 0 && num2 > 0)
			{
				num2--;
				num3 >>= 1;
			}
			int num4 = (pcm >> ((num2 == 0) ? 4 : (num2 + 3))) & 0xF;
			byte b = (byte)(num | (num2 << 4) | num4);
			return (byte)(b ^ 0xD5u);
		}

		private static short DecodeALawSample(byte alaw)
		{
			alaw = (byte)(alaw ^ 0xD5u);
			int num = alaw & 0x80;
			int num2 = (alaw & 0x70) >> 4;
			int num3 = alaw & 0xF;
			num3 <<= 4;
			num3 += 8;
			if (num2 != 0)
			{
				num3 += 256;
			}
			if (num2 > 1)
			{
				num3 <<= num2 - 1;
			}
			return (short)((num == 0) ? num3 : (-num3));
		}
	}
}
