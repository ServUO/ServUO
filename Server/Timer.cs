#region Header
// **********
// ServUO - Timer.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Server.Diagnostics;
#endregion

namespace Server
{
	public enum TimerPriority
	{
		EveryTick,
		TenMS,
		TwentyFiveMS,
		FiftyMS,
		TwoFiftyMS,
		OneSecond,
		FiveSeconds,
		OneMinute
	}

	public delegate void TimerCallback();

	public delegate void TimerStateCallback(object state);

	public delegate void TimerStateCallback<in T>(T state);

	public class Timer
	{
		private long m_Next;
		private long m_Delay;
		private long m_Interval;
		private bool m_Running;
		private int m_Index;
		private readonly int m_Count;
		private TimerPriority m_Priority;
		private List<Timer> m_List;
		private bool m_PrioritySet;

		private static string FormatDelegate(Delegate callback)
		{
			if (callback == null)
			{
				return "null";
			}

			if (callback.Method.DeclaringType == null)
			{
				return callback.Method.Name;
			}

			return String.Format("{0}.{1}", callback.Method.DeclaringType.FullName, callback.Method.Name);
		}

		public static void DumpInfo(TextWriter tw)
		{
			TimerThread.DumpInfo2(tw);
		}

		public TimerPriority Priority
		{
			get { return m_Priority; }
			set
			{
				if (!m_PrioritySet)
				{
					m_PrioritySet = true;
				}

				if (m_Priority == value)
				{
					return;
				}

				m_Priority = value;

				if (m_Running)
				{
					TimerThread.PriorityChange(this, (int)m_Priority);
				}
			}
		}

		public DateTime Next
		{
			// Obnoxious
			get { return DateTime.UtcNow.AddMilliseconds(m_Next - Core.TickCount); }
		}

		public TimeSpan Delay
		{
			get { return TimeSpan.FromMilliseconds(m_Delay); }
			set { m_Delay = (long)value.TotalMilliseconds; }
		}

		public TimeSpan Interval
		{
			get { return TimeSpan.FromMilliseconds(m_Interval); }
			set { m_Interval = (long)value.TotalMilliseconds; }
		}

		public bool Running
		{
			get { return m_Running; }
			set
			{
				if (value)
				{
					Start();
				}
				else
				{
					Stop();
				}
			}
		}

		public TimerProfile GetProfile()
		{
			return Core.Profiling ? TimerProfile.Acquire(ToString()) : null;
		}

		public class TimerThread
		{
			private static readonly Dictionary<Timer, TimerChangeEntry> m_Changed = new Dictionary<Timer, TimerChangeEntry>();

			private static readonly long[] m_NextPriorities = new long[8];
			private static readonly long[] m_PriorityDelays = { 0, 10, 25, 50, 250, 1000, 5000, 60000 };

			private static readonly List<Timer>[] m_Timers =
			{
				new List<Timer>(), new List<Timer>(), new List<Timer>(),
				new List<Timer>(), new List<Timer>(), new List<Timer>(), new List<Timer>(), new List<Timer>()
			};

			public static void DumpInfo2(TextWriter tw)
			{
				for (var i = 0; i < 8; ++i)
				{
					tw.WriteLine("Priority: {0}", (TimerPriority)i);
					tw.WriteLine();

					var hash = new Dictionary<string, List<Timer>>();

					for (var j = 0; j < m_Timers[i].Count; ++j)
					{
						var t = m_Timers[i][j];

						var key = t.ToString();

						List<Timer> list;
						hash.TryGetValue(key, out list);

						if (list == null)
						{
							hash[key] = list = new List<Timer>();
						}

						list.Add(t);
					}

					foreach (var kv in hash)
					{
						var key = kv.Key;
						var list = kv.Value;

						tw.WriteLine(
							"Type: {0}; Count: {1}; Percent: {2}%",
							key,
							list.Count,
							(int)(100 * (list.Count / (double)m_Timers[i].Count)));
					}

					tw.WriteLine();
					tw.WriteLine();
				}
			}

			private class TimerChangeEntry
			{
				public Timer m_Timer;

				public int m_NewIndex;
				public bool m_IsAdd;

				private TimerChangeEntry(Timer t, int newIndex, bool isAdd)
				{
					m_Timer = t;
					m_NewIndex = newIndex;
					m_IsAdd = isAdd;
				}

				public void Free()
				{
					m_Timer = null;

					lock (m_InstancePool)
					{
						if (m_InstancePool.Count < 512) // Arbitrary
						{
							m_InstancePool.Enqueue(this);
						}
					}
				}

				private static readonly Queue<TimerChangeEntry> m_InstancePool = new Queue<TimerChangeEntry>();

				public static TimerChangeEntry GetInstance(Timer t, int newIndex, bool isAdd)
				{
					TimerChangeEntry e = null;

					lock (m_InstancePool)
					{
						if (m_InstancePool.Count > 0)
						{
							e = m_InstancePool.Dequeue();
						}
					}

					if (e != null)
					{
						e.m_Timer = t;
						e.m_NewIndex = newIndex;
						e.m_IsAdd = isAdd;
					}
					else
					{
						e = new TimerChangeEntry(t, newIndex, isAdd);
					}

					return e;
				}
			}

			public static void Change(Timer t, int newIndex, bool isAdd)
			{
				lock (m_Changed)
				{
					m_Changed[t] = TimerChangeEntry.GetInstance(t, newIndex, isAdd);
				}

				m_Signal.Set();
			}

			public static void AddTimer(Timer t)
			{
				Change(t, (int)t.Priority, true);
			}

			public static void PriorityChange(Timer t, int newPrio)
			{
				Change(t, newPrio, false);
			}

			public static void RemoveTimer(Timer t)
			{
				Change(t, -1, false);
			}

			private static void ProcessChanged()
			{
				lock (m_Changed)
				{
					var curTicks = Core.TickCount;

					foreach (var tce in m_Changed.Values)
					{
						var timer = tce.m_Timer;
						var newIndex = tce.m_NewIndex;

						if (timer.m_List != null)
						{
							timer.m_List.Remove(timer);
						}

						if (tce.m_IsAdd)
						{
							timer.m_Next = curTicks + timer.m_Delay;
							timer.m_Index = 0;
						}

						if (newIndex >= 0)
						{
							timer.m_List = m_Timers[newIndex];
							timer.m_List.Add(timer);
						}
						else
						{
							timer.m_List = null;
						}

						tce.Free();
					}

					m_Changed.Clear();
				}
			}

			private static readonly AutoResetEvent m_Signal = new AutoResetEvent(false);

			public static void Set()
			{
				m_Signal.Set();
			}

			public void TimerMain()
			{
				long now;
				int i, j;
				bool loaded;

				while (!Core.Closing)
				{
					if (World.Loading || World.Saving)
					{
						m_Signal.WaitOne(1, false);
						continue;
					}

					ProcessChanged();

					loaded = false;

					for (i = 0; i < m_Timers.Length; i++)
					{
						now = Core.TickCount;

						if (now < m_NextPriorities[i])
						{
							break;
						}

						m_NextPriorities[i] = now + m_PriorityDelays[i];

						for (j = 0; j < m_Timers[i].Count; j++)
						{
							var t = m_Timers[i][j];

							if (t.m_Queued || now <= t.m_Next)
							{
								continue;
							}

							t.m_Queued = true;

							lock (m_Queue)
							{
								m_Queue.Enqueue(t);
							}

							loaded = true;

							if (t.m_Count != 0 && (++t.m_Index >= t.m_Count))
							{
								t.Stop();
							}
							else
							{
								t.m_Next = now + t.m_Interval;
							}
						}
					}

					if (loaded)
					{
						Core.Set();
					}

					m_Signal.WaitOne(1, false);
				}
			}
		}

		private static readonly Queue<Timer> m_Queue = new Queue<Timer>();
		private static int m_BreakCount = 20000;

		public static int BreakCount { get { return m_BreakCount; } set { m_BreakCount = value; } }

		private static int m_QueueCountAtSlice;

		private bool m_Queued;

		public static void Slice()
		{
			lock (m_Queue)
			{
				m_QueueCountAtSlice = m_Queue.Count;

				var index = 0;

				while (index < m_BreakCount && m_Queue.Count != 0)
				{
					var t = m_Queue.Dequeue();
					var prof = t.GetProfile();

					if (prof != null)
					{
						prof.Start();
					}

					t.OnTick();
					t.m_Queued = false;
					++index;

					if (prof != null)
					{
						prof.Finish();
					}
				}
			}
		}

		public Timer(TimeSpan delay)
			: this(delay, TimeSpan.Zero, 1)
		{ }

		public Timer(TimeSpan delay, TimeSpan interval)
			: this(delay, interval, 0)
		{ }

		public virtual bool DefRegCreation { get { return true; } }

		public void RegCreation()
		{
			var prof = GetProfile();

			if (prof != null)
			{
				prof.Created++;
			}
		}

		public Timer(TimeSpan delay, TimeSpan interval, int count)
		{
			m_Delay = (long)delay.TotalMilliseconds;
			m_Interval = (long)interval.TotalMilliseconds;
			m_Count = count;

			if (!m_PrioritySet)
			{
				m_Priority = ComputePriority(count == 1 ? delay : interval);
				m_PrioritySet = true;
			}

			if (DefRegCreation)
			{
				RegCreation();
			}
		}

		public override string ToString()
		{
			return GetType().FullName;
		}

		public static TimerPriority ComputePriority(TimeSpan ts)
		{
			if (ts.TotalMinutes >= 10.0)
			{
				return TimerPriority.OneMinute;
			}

			if (ts.TotalMinutes >= 1.0)
			{
				return TimerPriority.FiveSeconds;
			}

			if (ts.TotalSeconds >= 10.0)
			{
				return TimerPriority.OneSecond;
			}

			if (ts.TotalSeconds >= 5.0)
			{
				return TimerPriority.TwoFiftyMS;
			}

			if (ts.TotalSeconds >= 2.5)
			{
				return TimerPriority.FiftyMS;
			}

			if (ts.TotalSeconds >= 1.0)
			{
				return TimerPriority.TwentyFiveMS;
			}

			if (ts.TotalSeconds >= 0.5)
			{
				return TimerPriority.TenMS;
			}

			return TimerPriority.EveryTick;
		}

		#region DelayCall(..)
		public static Timer DelayCall(TimerCallback callback)
		{
			return DelayCall(TimeSpan.Zero, TimeSpan.Zero, 1, callback);
		}

		public static Timer DelayCall(TimeSpan delay, TimerCallback callback)
		{
			return DelayCall(delay, TimeSpan.Zero, 1, callback);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, TimerCallback callback)
		{
			return DelayCall(delay, interval, 0, callback);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback)
		{
			Timer t = new DelayCallTimer(delay, interval, count, callback)
			{
				Priority = ComputePriority(count == 1 ? delay : interval)
			};

			t.Start();

			return t;
		}

		public static Timer DelayCall(TimerStateCallback callback, object state)
		{
			return DelayCall(TimeSpan.Zero, TimeSpan.Zero, 1, callback, state);
		}

		public static Timer DelayCall(TimeSpan delay, TimerStateCallback callback, object state)
		{
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, TimerStateCallback callback, object state)
		{
			return DelayCall(delay, interval, 0, callback, state);
		}

		public static Timer DelayCall(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state)
		{
			Timer t = new DelayStateCallTimer(delay, interval, count, callback, state)
			{
				Priority = ComputePriority(count == 1 ? delay : interval)
			};

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall<T>(..)
		public static Timer DelayCall<T>(TimerStateCallback<T> callback, T state)
		{
			return DelayCall(TimeSpan.Zero, TimeSpan.Zero, 1, callback, state);
		}

		public static Timer DelayCall<T>(TimeSpan delay, TimerStateCallback<T> callback, T state)
		{
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state);
		}

		public static Timer DelayCall<T>(TimeSpan delay, TimeSpan interval, TimerStateCallback<T> callback, T state)
		{
			return DelayCall(delay, interval, 0, callback, state);
		}

		public static Timer DelayCall<T>(
			TimeSpan delay,
			TimeSpan interval,
			int count,
			TimerStateCallback<T> callback,
			T state)
		{
			Timer t = new DelayStateCallTimer<T>(delay, interval, count, callback, state)
			{
				Priority = ComputePriority(count == 1 ? delay : interval)
			};

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall Timers
		private class DelayCallTimer : Timer
		{
			private readonly TimerCallback m_Callback;

			public TimerCallback Callback { get { return m_Callback; } }

			public override bool DefRegCreation { get { return false; } }

			public DelayCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				RegCreation();
			}

			protected override void OnTick()
			{
				if (m_Callback != null)
				{
					m_Callback();
				}
			}

			public override string ToString()
			{
				return String.Format("DelayCallTimer[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer : Timer
		{
			private readonly TimerStateCallback m_Callback;
			private readonly object m_State;

			public TimerStateCallback Callback { get { return m_Callback; } }

			public override bool DefRegCreation { get { return false; } }

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State = state;

				RegCreation();
			}

			protected override void OnTick()
			{
				if (m_Callback != null)
				{
					m_Callback(m_State);
				}
			}

			public override string ToString()
			{
				return String.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer<T> : Timer
		{
			private readonly TimerStateCallback<T> m_Callback;
			private readonly T m_State;

			public TimerStateCallback<T> Callback { get { return m_Callback; } }

			public override bool DefRegCreation { get { return false; } }

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State = state;

				RegCreation();
			}

			protected override void OnTick()
			{
				if (m_Callback != null)
				{
					m_Callback(m_State);
				}
			}

			public override string ToString()
			{
				return String.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
			}
		}
		#endregion

		public void Start()
		{
			if (m_Running)
			{
				return;
			}

			m_Running = true;

			TimerThread.AddTimer(this);

			var prof = GetProfile();

			if (prof != null)
			{
				prof.Started++;
			}
		}

		public void Stop()
		{
			if (!m_Running)
			{
				return;
			}

			m_Running = false;

			TimerThread.RemoveTimer(this);

			var prof = GetProfile();

			if (prof != null)
			{
				prof.Stopped++;
			}
		}

		protected virtual void OnTick()
		{ }
	}
}