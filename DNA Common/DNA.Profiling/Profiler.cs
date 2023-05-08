using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Profiling
{
	public class Profiler : DrawableGameComponent
	{
		public class ProfileEvent : IProfilerLinkedListNode, IDisposable
		{
			public string _name;

			public int _threadIndex;

			public long _startTime;

			public long _endTime;

			private IProfilerLinkedListNode _next;

			public IProfilerLinkedListNode NextNode
			{
				get
				{
					return _next;
				}
				set
				{
					_next = value;
				}
			}

			public ProfileEvent()
			{
				_name = "Unnamed";
				_threadIndex = -1;
				_startTime = 0L;
				_endTime = 0L;
			}

			public void Init(string name)
			{
				Init(name, ProfilerUtils.ThreadIndex);
			}

			public void Init(string name, ProfilerThreadEnum threadIndex)
			{
				_name = name;
				_threadIndex = (int)threadIndex;
				_startTime = Stopwatch.GetTimestamp();
				_endTime = _startTime;
			}

			public void Dispose()
			{
				_endTime = Stopwatch.GetTimestamp();
			}
		}

		private static Profiler _theInstance;

		private static ProfileEvent _dummyEvent = new ProfileEvent();

		private ProfilerObjectCache<ProfileEvent> _eventPool;

		private ProfilerLockFreeStack<ProfileEvent> _activeEvents;

		private List<ProfileEvent> _eventStack;

		private ProfileEvent _eventsToBeReported;

		private ProfileEvent _gfxEvent;

		private ProfilerPrimitiveBatch _primitiveBatch;

		private SpriteBatch _spriteBatch;

		private ProfilerSimpleStack<ProfileEvent>[] _eventLists;

		private Dictionary<string, Color> _colorDict;

		private ProfilerCircularQueue<float> _memSamples;

		private float _sampleWaitTime;

		private string[] _sectionNames;

		private Color[] _sectionColors;

		private Vector2[] _sectionSizes;

		private Vector2 _stringSize;

		private double _ticksToMilliseconds;

		private long _frameStart;

		private long _newFrameStart;

		private long _frameEnd;

		private bool _wantProfiling;

		private bool _profiling;

		private bool _frameMarked;

		public static Profiler Instance
		{
			get
			{
				return _theInstance;
			}
		}

		public static bool ProfilingAvailable
		{
			get
			{
				return _theInstance != null;
			}
		}

		public static bool Profiling
		{
			get
			{
				if (ProfilingAvailable)
				{
					return _theInstance._wantProfiling;
				}
				return false;
			}
			set
			{
				if (ProfilingAvailable)
				{
					_theInstance._wantProfiling = value;
				}
			}
		}

		public static Profiler CreateComponent(Game game)
		{
			if (_theInstance == null)
			{
				_theInstance = new Profiler(game);
				_theInstance.UpdateOrder = int.MaxValue;
				_theInstance.DrawOrder = int.MaxValue;
				((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)_theInstance);
			}
			return _theInstance;
		}

		public static ProfileEvent TimeSection(string name)
		{
			return TimeSection(name, ProfilerUtils.ThreadIndex);
		}

		public static ProfileEvent TimeSection(string name, ProfilerThreadEnum tindex)
		{
			if (!Profiling)
			{
				return _dummyEvent;
			}
			return _theInstance.InternalTimeSection(name, tindex);
		}

		public static void MarkFrame()
		{
			if (Profiling)
			{
				_theInstance.InternalMarkFrame();
			}
		}

		public static void SetColor(string name, Color color)
		{
			if (ProfilingAvailable)
			{
				_theInstance._colorDict.Add(name, color);
				_theInstance._sectionNames = null;
				_theInstance._sectionColors = null;
				_theInstance._sectionSizes = null;
			}
		}

		private Profiler(Game game)
			: base(game)
		{
			_eventPool = new ProfilerObjectCache<ProfileEvent>();
			_activeEvents = new ProfilerLockFreeStack<ProfileEvent>();
			_wantProfiling = false;
			_profiling = false;
			_dummyEvent = new ProfileEvent();
			_dummyEvent.Init("Dummy");
			_ticksToMilliseconds = 1000.0 / (double)Stopwatch.Frequency;
			_colorDict = new Dictionary<string, Color>();
			_eventLists = new ProfilerSimpleStack<ProfileEvent>[4];
			_memSamples = new ProfilerCircularQueue<float>(120);
			_sampleWaitTime = 0f;
			_eventStack = new List<ProfileEvent>(10);
			_frameMarked = false;
			for (int i = 0; i < 4; i++)
			{
				_eventLists[i] = new ProfilerSimpleStack<ProfileEvent>();
			}
		}

		private ProfileEvent InternalTimeSection(string name, ProfilerThreadEnum tindex)
		{
			ProfileEvent profileEvent = _eventPool.Get();
			_activeEvents.Push(profileEvent);
			profileEvent.Init(name, tindex);
			return profileEvent;
		}

		public override void Initialize()
		{
			base.Initialize();
			_spriteBatch = new SpriteBatch(base.Game.GraphicsDevice);
			_primitiveBatch = new ProfilerPrimitiveBatch(base.Game.GraphicsDevice);
			SetColor("XNA (Graphics)", Color.Maroon);
		}

		private float GetStartMillis(ProfileEvent e)
		{
			return (float)((double)(e._startTime - _frameStart) * _ticksToMilliseconds);
		}

		private float GetElapsedMillis(ProfileEvent e)
		{
			return (float)((double)(e._endTime - e._startTime) * _ticksToMilliseconds);
		}

		private bool GetSectionColors()
		{
			if (_colorDict.Count == 0 || ProfilerUtils.SystemFont == null)
			{
				return false;
			}
			if (_sectionNames == null)
			{
				_sectionNames = Enumerable.ToArray<string>((IEnumerable<string>)_colorDict.Keys);
				_sectionColors = Enumerable.ToArray<Color>((IEnumerable<Color>)_colorDict.Values);
				_sectionSizes = new Vector2[_sectionNames.Length];
				_stringSize = Vector2.Zero;
				int num = 0;
				string[] sectionNames = _sectionNames;
				foreach (string text in sectionNames)
				{
					_sectionSizes[num] = ProfilerUtils.SystemFont.MeasureString(text);
					_stringSize = Vector2.Max(_sectionSizes[num++], _stringSize);
				}
			}
			return true;
		}

		public void InternalMarkFrame()
		{
			if (_gfxEvent != null)
			{
				_gfxEvent.Dispose();
				_gfxEvent = null;
			}
			if (_profiling)
			{
				_frameStart = _newFrameStart;
				_frameEnd = Stopwatch.GetTimestamp();
				if (_eventsToBeReported != null)
				{
					_eventPool.PutList(_eventsToBeReported);
				}
				_eventsToBeReported = _activeEvents.Clear();
			}
			if (_wantProfiling != _profiling)
			{
				_profiling = _wantProfiling;
				if (!_profiling && _eventsToBeReported != null)
				{
					_eventPool.PutList(_eventsToBeReported);
					_eventsToBeReported = null;
					_frameStart = -1L;
				}
			}
			if (_profiling)
			{
				if (!_activeEvents.Empty)
				{
					_eventPool.PutList(_activeEvents.Clear());
				}
				_newFrameStart = Stopwatch.GetTimestamp();
			}
			_frameMarked = true;
		}

		public override void Update(GameTime gameTime)
		{
			if (!_frameMarked)
			{
				InternalMarkFrame();
			}
			_frameMarked = false;
		}

		public override void Draw(GameTime gameTime)
		{
			if (_profiling && _eventsToBeReported != null)
			{
				ProfilerUtils._standard2DProjection = Matrix.CreateOrthographicOffCenter(0f, base.Game.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
				float num = (float)((double)(_frameEnd - _frameStart) * _ticksToMilliseconds);
				_sampleWaitTime -= (float)gameTime.get_ElapsedGameTime().TotalSeconds;
				if (_sampleWaitTime < 0f)
				{
					_sampleWaitTime += 1f;
					_memSamples.Add(GC.GetTotalMemory(false) / 1024);
				}
				ProfileEvent profileEvent = _eventsToBeReported;
				while (profileEvent != null)
				{
					ProfileEvent profileEvent2 = profileEvent;
					profileEvent = profileEvent2.NextNode as ProfileEvent;
					_eventLists[profileEvent2._threadIndex].Push(profileEvent2);
				}
				_eventsToBeReported = null;
				float x = 200f + num * 30f;
				_primitiveBatch.Begin(PrimitiveType.LineList);
				_primitiveBatch.AddVertex(new Vector2(x, 180f), Color.White);
				_primitiveBatch.AddVertex(new Vector2(x, 260f), Color.White);
				_primitiveBatch.End();
				_primitiveBatch.Begin(PrimitiveType.TriangleList);
				_primitiveBatch.AddFilledBox(new Vector2(200f, 180f), new Vector2(990f, 5f), Color.Olive, false);
				_primitiveBatch.AddFilledBox(new Vector2(200f, 180f), new Vector2(510f, 5f), Color.Yellow, false);
				_primitiveBatch.AddFilledBox(new Vector2(200f, 185f), new Vector2(num * 30f, 5f), Color.Maroon, false);
				Vector2 vector = new Vector2(200f, 200f);
				float num2 = 100f;
				ProfilerSimpleStack<ProfileEvent>[] eventLists = _eventLists;
				foreach (ProfilerSimpleStack<ProfileEvent> profilerSimpleStack in eventLists)
				{
					if (!profilerSimpleStack.Empty)
					{
						ProfileEvent profileEvent2 = profilerSimpleStack.Root;
						_eventStack.Clear();
						num2 = 100f;
						while (profileEvent2 != null)
						{
							while (_eventStack.Count != 0 && _eventStack[_eventStack.Count - 1]._endTime <= profileEvent2._startTime)
							{
								num2 = num2 * 3f / 2f;
								_eventStack.RemoveAt(_eventStack.Count - 1);
							}
							_eventStack.Add(profileEvent2);
							num2 = num2 * 2f / 3f;
							float x2 = GetElapsedMillis(profileEvent2) * 30f;
							Color value;
							if (!_colorDict.TryGetValue(profileEvent2._name, out value))
							{
								value = Color.White;
							}
							vector.X = 200f + GetStartMillis(profileEvent2) * 30f;
							_primitiveBatch.AddFilledBox(vector, new Vector2(x2, num2), value, false);
							profileEvent2 = profileEvent2.NextNode as ProfileEvent;
						}
						_eventPool.PutList(profilerSimpleStack.Root);
						profilerSimpleStack.Clear();
					}
					vector.Y += 20f;
				}
				_primitiveBatch.End();
				if (GetSectionColors())
				{
					_spriteBatch.Begin();
					_primitiveBatch.Begin(PrimitiveType.TriangleList);
					vector.Y += 50f;
					vector.X = 200f;
					for (int j = 0; j < _sectionNames.Length; j++)
					{
						if (vector.X + _stringSize.X + _stringSize.Y + 10f >= (float)base.GraphicsDevice.Viewport.Width)
						{
							vector.X = 200f;
							vector.Y += _stringSize.Y + 10f;
						}
						Vector2 vector2 = vector;
						vector2.X += _stringSize.X - _sectionSizes[j].X;
						_spriteBatch.DrawString(ProfilerUtils.SystemFont, _sectionNames[j], vector2, Color.White);
						vector2.X = vector.X + _stringSize.X + 10f;
						_primitiveBatch.AddFilledBox(vector2, new Vector2(_stringSize.Y, _stringSize.Y), _sectionColors[j], false);
						vector.X += _stringSize.X + _stringSize.Y + 20f;
					}
					_primitiveBatch.End();
					_spriteBatch.End();
				}
				vector.X = 320f;
				vector.Y += _stringSize.Y + 40f;
				Vector2 size = new Vector2(620f, 300f);
				Vector2 scale = new Vector2(0f, 256000f);
				_primitiveBatch.DrawGraphVerticalAxis(vector, size, Color.Red);
				_primitiveBatch.DrawGraphBar(0f, scale, vector, size, Color.Red);
				_primitiveBatch.DrawGraphBar(128000f, scale, vector, size, Color.Red);
				for (int k = 2; k < 25; k += 2)
				{
					_primitiveBatch.DrawGraphBar(k * 10000, scale, vector, size, Color.Yellow);
				}
				_primitiveBatch.DrawGraph(_memSamples.Buffer, _memSamples.Head, scale, vector, size, Color.White);
				_primitiveBatch.End();
			}
			_gfxEvent = TimeSection("XNA (Graphics)", ProfilerThreadEnum.MAIN);
		}
	}
}
