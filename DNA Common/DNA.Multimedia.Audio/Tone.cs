using System;
using DNA.Data.Units;

namespace DNA.Multimedia.Audio
{
	public struct Tone
	{
		private const int NotesPerOctive = 12;

		private const float BaseTone = 440f;

		private float _value;

		public int Octive
		{
			get
			{
				if (_value < 0f)
				{
					int num = (int)Math.Ceiling(_value);
					int num2 = num / 12;
					return num2 - 1;
				}
				int num3 = (int)Math.Floor(_value);
				return num3 / 12;
			}
		}

		public string NoteName
		{
			get
			{
				switch (BaseNote)
				{
				case Notes.A:
					return "A";
				case Notes.Bb:
					return "B♭";
				case Notes.B:
					return "B";
				case Notes.C:
					return "C";
				case Notes.Db:
					return "C♯";
				case Notes.D:
					return "D";
				case Notes.Eb:
					return "E♭";
				case Notes.E:
					return "E";
				case Notes.F:
					return "F";
				case Notes.Gb:
					return "F♯";
				case Notes.G:
					return "G";
				case Notes.Ab:
					return "G♯";
				default:
					return "";
				}
			}
		}

		public Notes BaseNote
		{
			get
			{
				int num = (int)Math.Round(_value);
				if (num < 0)
				{
					num = -num;
					num %= 12;
					num = 12 - num;
					return (Notes)(num % 12);
				}
				return (Notes)(num % 12);
			}
		}

		public float Detune
		{
			get
			{
				int num = (int)Math.Round(_value);
				return _value - (float)num;
			}
		}

		public float Value
		{
			get
			{
				return _value;
			}
		}

		public int KeyValue
		{
			get
			{
				return (int)_value;
			}
		}

		public Frequency Frequency
		{
			get
			{
				return GetNoteFrequency(_value);
			}
		}

		public override string ToString()
		{
			return BaseNote.ToString() + Octive;
		}

		public static Tone FromKeyIndex(int value)
		{
			return new Tone(value);
		}

		public static Tone FromNote(Notes note, int octive)
		{
			if (octive < 0)
			{
				octive++;
			}
			float noteNumber = (float)note + (float)(12 * octive);
			return new Tone(noteNumber);
		}

		private static float NoteFromFrequency(Frequency frequency)
		{
			return (float)(12.0 * Math.Log(frequency.Hertz / 440f, 2.0));
		}

		public static Tone FromFrequency(Frequency frequency)
		{
			return new Tone(NoteFromFrequency(frequency));
		}

		public static Tone Parse(string value)
		{
			int num = value.IndexOfAny(new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
			int octive = 5;
			string text;
			if (num > 0)
			{
				string s = value.Substring(num);
				octive = int.Parse(s);
				text = value.Substring(0, num);
			}
			else
			{
				text = value;
			}
			text = text.Trim();
			Notes note;
			try
			{
				note = (Notes)Enum.Parse(typeof(Notes), text, true);
			}
			catch
			{
				switch (text)
				{
				case "C#":
					note = Notes.Db;
					break;
				case "D#":
					note = Notes.Eb;
					break;
				case "F#":
					note = Notes.Gb;
					break;
				case "G#":
					note = Notes.Ab;
					break;
				case "A#":
					note = Notes.Bb;
					break;
				default:
					throw new FormatException("Invalid Note");
				}
			}
			return FromNote(note, octive);
		}

		private Tone(float noteNumber)
		{
			if (float.IsNaN(noteNumber))
			{
				_value = 0f;
			}
			_value = noteNumber;
		}

		private static Frequency GetNoteFrequency(float note)
		{
			return Frequency.FromHertz((float)(440.0 * Math.Pow(2.0, (double)note / 12.0)));
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public bool Equals(Tone other)
		{
			return _value == other._value;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(Tone))
			{
				return Equals((Tone)obj);
			}
			return false;
		}

		public static bool operator ==(Tone a, Tone b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Tone a, Tone b)
		{
			return !a.Equals(b);
		}
	}
}
