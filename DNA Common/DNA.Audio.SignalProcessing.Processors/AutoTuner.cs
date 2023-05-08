using System;
using DNA.Data.Units;
using DNA.Multimedia.Audio;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class AutoTuner : SignalProcessor<SpectralData>
	{
		private PitchShifter _pitchShifter = new PitchShifter();

		private AnaylzeProcessor _anaylizer = new AnaylzeProcessor();

		public AnaylzeProcessor Anaylizer
		{
			get
			{
				return _anaylizer;
			}
		}

		public override bool ProcessBlock(SpectralData data)
		{
			_anaylizer.ProcessBlock(data);
			Frequency value = _anaylizer.PrimaryFrequency.Value;
			value.Hertz = Math.Abs(value.Hertz);
			Tone tone = Tone.FromFrequency(value);
			int num = tone.KeyValue;
			if (tone.Detune > 0.5f)
			{
				num++;
			}
			if (tone.Detune < -0.5f)
			{
				num--;
			}
			Tone tone2 = Tone.FromKeyIndex(num);
			if (value.Hertz > 0f)
			{
				float pitch = tone2.Frequency.Hertz / value.Hertz;
				_pitchShifter.Pitch = pitch;
			}
			else
			{
				_pitchShifter.Pitch = 1f;
			}
			_pitchShifter.ProcessBlock(data);
			return true;
		}
	}
}
