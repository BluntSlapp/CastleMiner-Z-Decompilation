using System;
using System.Collections.Generic;

namespace DNA
{
	public abstract class AchievementManager<T> where T : PlayerStats
	{
		public abstract class Achievement
		{
			private string _name;

			private string _howToUnlock;

			private bool _acheived;

			private T _stats;

			public object Tag;

			protected T PlayerStats
			{
				get
				{
					return _stats;
				}
			}

			public string HowToUnlock
			{
				get
				{
					return _howToUnlock;
				}
			}

			public virtual string Name
			{
				get
				{
					return _name;
				}
			}

			public virtual bool Acheived
			{
				get
				{
					return _acheived;
				}
			}

			protected abstract bool IsSastified { get; }

			public abstract string ProgressTowardsUnlockMessage { get; }

			public virtual float ProgressTowardsUnlock
			{
				get
				{
					return 0f;
				}
			}

			public Achievement(AchievementManager<T> manager, string name, string howToUnlock)
			{
				_name = name;
				_howToUnlock = howToUnlock;
				_stats = manager._stats;
			}

			public bool Update()
			{
				if (!_acheived && IsSastified)
				{
					_acheived = true;
					return true;
				}
				return false;
			}
		}

		public class AcheimentEventArgs : EventArgs
		{
			public Achievement Achievement;

			public AcheimentEventArgs(Achievement achievement)
			{
				Achievement = achievement;
			}
		}

		private T _stats;

		private List<Achievement> _acheviements = new List<Achievement>();

		protected T PlayerStats
		{
			get
			{
				return _stats;
			}
		}

		public Achievement this[int index]
		{
			get
			{
				return _acheviements[index];
			}
		}

		public int Count
		{
			get
			{
				return _acheviements.Count;
			}
		}

		public event EventHandler<AcheimentEventArgs> Achieved;

		public int AddAcheivement(Achievement achievement)
		{
			_acheviements.Add(achievement);
			return _acheviements.Count - 1;
		}

		public AchievementManager(T stats)
		{
			_stats = stats;
			CreateAcheivements();
			for (int i = 0; i < _acheviements.Count; i++)
			{
				_acheviements[i].Update();
			}
		}

		public abstract void CreateAcheivements();

		public void Update()
		{
			for (int i = 0; i < _acheviements.Count; i++)
			{
				if (_acheviements[i].Update())
				{
					OnAchieved(_acheviements[i]);
					if (this.Achieved != null)
					{
						this.Achieved(this, new AcheimentEventArgs(_acheviements[i]));
					}
				}
				_acheviements[i].Update();
			}
		}

		public virtual void OnAchieved(Achievement acheivement)
		{
		}
	}
}
