using System;

namespace DNA.Multimedia.Audio
{
	public struct MusicalNote
	{
		private TimeSpan _duration;

		private Percentage _volume;

		private Tone _tone;

		public Tone Tone
		{
			get
			{
				return _tone;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return _duration;
			}
		}

		public Percentage Volume
		{
			get
			{
				return _volume;
			}
		}

		public MusicalNote(Tone tone, TimeSpan duration)
		{
			_tone = tone;
			_duration = duration;
			_volume = Percentage.OneHundred;
		}

		public MusicalNote(Tone tone, TimeSpan duration, Percentage volume)
		{
			_tone = tone;
			_duration = duration;
			_volume = volume;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public bool Equals(MusicalNote other)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(MusicalNote))
			{
				return Equals((MusicalNote)obj);
			}
			return false;
		}

		public static bool operator ==(MusicalNote a, MusicalNote b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(MusicalNote a, MusicalNote b)
		{
			return !a.Equals(b);
		}
	}
}
