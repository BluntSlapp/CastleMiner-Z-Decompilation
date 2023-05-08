using System;
using System.Collections.Generic;
using System.IO;

namespace DNA
{
	public class HighScoreManager<T> where T : PlayerStats, new()
	{
		private int MaxScores = 100;

		private Comparison<T> CompareScores;

		private List<T> _scores = new List<T>();

		public IList<T> Scores
		{
			get
			{
				return _scores;
			}
		}

		public HighScoreManager(int maxScores, Comparison<T> comparer)
		{
			CompareScores = comparer;
		}

		public void UpdateScores(IList<T> newScores, T currentStats)
		{
			List<T> list = new List<T>();
			list.AddRange(newScores);
			currentStats.DateRecorded = DateTime.UtcNow;
			list.Add(currentStats);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < _scores.Count; i++)
			{
				dictionary[_scores[i].GamerTag] = i;
			}
			for (int j = 0; j < list.Count; j++)
			{
				T val = list[j];
				int value;
				if (dictionary.TryGetValue(val.GamerTag, out value))
				{
					if (_scores[value].DateRecorded < val.DateRecorded)
					{
						_scores[value] = val;
					}
				}
				else
				{
					_scores.Add(val);
					dictionary[val.GamerTag] = _scores.Count - 1;
				}
			}
			_scores.Sort(CompareScores);
			if (_scores.Count > MaxScores)
			{
				_scores.RemoveRange(MaxScores, _scores.Count - MaxScores);
			}
		}

		public void Save(BinaryWriter writer)
		{
			writer.Write(_scores.Count);
			for (int i = 0; i < Scores.Count; i++)
			{
				T val = _scores[i];
				val.Save(writer);
			}
		}

		public void Load(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			_scores.Clear();
			for (int i = 0; i < num; i++)
			{
				T item = new T();
				item.Load(reader);
				_scores.Add(item);
			}
		}
	}
}
