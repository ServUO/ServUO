#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        ThirtySeconds,
		OneMinute,
        FiveMinutes
	}

	public delegate void TimerCallback();

	public delegate void TimerStateCallback(object state);

	public delegate void TimerStateCallback<in T>(T state);

	public delegate void TimerStateCallback<in T1, in T2>(T1 state1, T2 state2);

	public delegate void TimerStateCallback<in T1, in T2, in T3>(T1 state1, T2 state2, T3 state3);

	public delegate void TimerStateCallback<in T1, in T2, in T3, in T4>(T1 state1, T2 state2, T3 state3, T4 state4);

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

			return string.Format("{0}.{1}", callback.Method.DeclaringType.FullName, callback.Method.Name);
		}

		public static void DumpInfo(TextWriter tw)
		{
			TimerThread.Dump(tw);
		}

		public TimerPriority Priority
		{
			get => m_Priority;
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

		public DateTime Next => DateTime.UtcNow.AddMilliseconds(m_Next - Core.TickCount);

		public TimeSpan Delay
		{
			get => TimeSpan.FromMilliseconds(m_Delay);
			set => m_Delay = (long)value.TotalMilliseconds;
		}

		public TimeSpan Interval
		{
			get => TimeSpan.FromMilliseconds(m_Interval);
			set => m_Interval = (long)value.TotalMilliseconds;
		}

		public bool Running
		{
			get => m_Running;
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

			private static readonly long[] m_NextPriorities = new long[10];
			private static readonly long[] m_PriorityDelays = { 0, 10, 25, 50, 250, 1000, 5000, 30000, 60000, 300000 };

			private static readonly List<Timer>[] m_Timers =
			{
				new List<Timer>(), new List<Timer>(), new List<Timer>(), new List<Timer>(), new List<Timer>(),
                new List<Timer>(), new List<Timer>(), new List<Timer>(), new List<Timer>(), new List<Timer>()
			};

            private static readonly Dictionary<string, int>[] m_Dump = new Dictionary<string, int>[m_Timers.Length];

            private static DateTime m_Dumped;

            public static void Dump(TextWriter tw)
            {
                var now = DateTime.UtcNow;

                tw.WriteLine($"Date: {now}");

                if (m_Dumped > DateTime.MinValue)
                {
                    tw.WriteLine($"Last: {m_Dumped}");
                    tw.WriteLine($"Span: {now - m_Dumped}");
                }

                tw.WriteLine();
                tw.WriteLine();

                for (var i = 0; i < 10; i++)
                {
                    tw.WriteLine($"Priority: {(TimerPriority)i}");
                    tw.WriteLine();

                    var total = (double)m_Timers[i].Count;

                    var timers = m_Timers[i].GroupBy(t => t.ToString()).ToDictionary(o => o.Key, o => o.Count());

                    foreach (var o in timers.OrderByDescending(o => o.Value))
                    {
                        var name = o.Key;
                        var count = o.Value;
                        var percent = count / total;

                        var line = $"{count:#,0} ({percent:P1})";

                        if (m_Dump[i] != null && m_Dump[i].TryGetValue(o.Key, out var lastCount))
                        {
                            var diff = count - lastCount;

                            if (diff > 0)
                                line += $" [+{diff:#,0}]";
                            else if (diff < 0)
                                line += $" [{diff:#,0}]";
                        }

                        var tabs = new string('\t', 6 - (line.Length / 8));

                        tw.WriteLine($"{line}{tabs}{name}");
                    }

                    m_Dump[i] = timers;

                    tw.WriteLine();
                    tw.WriteLine();
                }

                m_Dumped = now;
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
					long curTicks = Core.TickCount;

					foreach (TimerChangeEntry tce in m_Changed.Values)
					{
						Timer timer = tce.m_Timer;
						int newIndex = tce.m_NewIndex;

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
							Timer t = m_Timers[i][j];

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

		public static int BreakCount { get => m_BreakCount; set => m_BreakCount = value; }

		private bool m_Queued;

		public static void Slice()
		{
			lock (m_Queue)
			{
				int index = 0;

				while (index < m_BreakCount && m_Queue.Count != 0)
				{
					Timer t = m_Queue.Dequeue();
					TimerProfile prof = t.GetProfile();

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

		public virtual bool DefRegCreation => true;

		public void RegCreation()
		{
			TimerProfile prof = GetProfile();

			if (prof != null)
			{
				prof.Created++;
			}
		}

		public Timer(TimeSpan delay, TimeSpan interval, int count)
		{
			_ToString = GetType().FullName;

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

		private readonly string _ToString;

		public override string ToString()
		{
			return _ToString;
		}

		public static TimerPriority ComputePriority(TimeSpan ts)
		{
            if (ts.TotalMinutes >= 30.0)
            {
                return TimerPriority.FiveMinutes;
            }

            if (ts.TotalMinutes >= 10.0)
			{
				return TimerPriority.OneMinute;
			}

            if (ts.TotalMinutes >= 5.0)
            {
                return TimerPriority.ThirtySeconds;
            }

            if (ts.TotalSeconds >= 30.0)
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

		public static Timer DelayCall<T>(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state)
		{
			Timer t = new DelayStateCallTimer<T>(delay, interval, count, callback, state)
			{
				Priority = ComputePriority(count == 1 ? delay : interval)
			};

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall<T1, T2>(..)
		public static Timer DelayCall<T1, T2>(TimerStateCallback<T1, T2> callback, T1 state1, T2 state2)
		{
			return DelayCall(TimeSpan.Zero, TimeSpan.Zero, 1, callback, state1, state2);
		}

		public static Timer DelayCall<T1, T2>(TimeSpan delay, TimerStateCallback<T1, T2> callback, T1 state1, T2 state2)
		{
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state1, state2);
		}

		public static Timer DelayCall<T1, T2>(TimeSpan delay, TimeSpan interval, TimerStateCallback<T1, T2> callback, T1 state1, T2 state2)
		{
			return DelayCall(delay, interval, 0, callback, state1, state2);
		}

		public static Timer DelayCall<T1, T2>(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T1, T2> callback, T1 state1, T2 state2)
		{
			Timer t = new DelayStateCallTimer<T1, T2>(delay, interval, count, callback, state1, state2)
			{
				Priority = ComputePriority(count == 1 ? delay : interval)
			};

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall<T1, T2, T3>(..)
		public static Timer DelayCall<T1, T2, T3>(TimerStateCallback<T1, T2, T3> callback, T1 state1, T2 state2, T3 state3)
		{
			return DelayCall(TimeSpan.Zero, TimeSpan.Zero, 1, callback, state1, state2, state3);
		}

		public static Timer DelayCall<T1, T2, T3>(TimeSpan delay, TimerStateCallback<T1, T2, T3> callback, T1 state1, T2 state2, T3 state3)
		{
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state1, state2, state3);
		}

		public static Timer DelayCall<T1, T2, T3>(TimeSpan delay, TimeSpan interval, TimerStateCallback<T1, T2, T3> callback, T1 state1, T2 state2, T3 state3)
		{
			return DelayCall(delay, interval, 0, callback, state1, state2, state3);
		}

		public static Timer DelayCall<T1, T2, T3>(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T1, T2, T3> callback, T1 state1, T2 state2, T3 state3)
		{
			Timer t = new DelayStateCallTimer<T1, T2, T3>(delay, interval, count, callback, state1, state2, state3)
			{
				Priority = ComputePriority(count == 1 ? delay : interval)
			};

			t.Start();

			return t;
		}
		#endregion

		#region DelayCall<T1, T2, T3, T4>(..)
		public static Timer DelayCall<T1, T2, T3, T4>(TimerStateCallback<T1, T2, T3, T4> callback, T1 state1, T2 state2, T3 state3, T4 state4)
		{
			return DelayCall(TimeSpan.Zero, TimeSpan.Zero, 1, callback, state1, state2, state3, state4);
		}

		public static Timer DelayCall<T1, T2, T3, T4>(TimeSpan delay, TimerStateCallback<T1, T2, T3, T4> callback, T1 state1, T2 state2, T3 state3, T4 state4)
		{
			return DelayCall(delay, TimeSpan.Zero, 1, callback, state1, state2, state3, state4);
		}

		public static Timer DelayCall<T1, T2, T3, T4>(TimeSpan delay, TimeSpan interval, TimerStateCallback<T1, T2, T3, T4> callback, T1 state1, T2 state2, T3 state3, T4 state4)
		{
			return DelayCall(delay, interval, 0, callback, state1, state2, state3, state4);
		}

		public static Timer DelayCall<T1, T2, T3, T4>(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T1, T2, T3, T4> callback, T1 state1, T2 state2, T3 state3, T4 state4)
		{
			Timer t = new DelayStateCallTimer<T1, T2, T3, T4>(delay, interval, count, callback, state1, state2, state3, state4)
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

			public TimerCallback Callback => m_Callback;

			public override bool DefRegCreation => false;

			public DelayCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerCallback callback)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				RegCreation();
			}

			protected override void OnTick()
			{
                m_Callback?.Invoke();
            }

			public override string ToString()
			{
				return string.Format("DelayCallTimer[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer : Timer
		{
			private readonly TimerStateCallback m_Callback;
			private readonly object m_State;

			public TimerStateCallback Callback => m_Callback;

			public override bool DefRegCreation => false;

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback callback, object state)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State = state;

				RegCreation();
			}

			protected override void OnTick()
			{
                m_Callback?.Invoke(m_State);
            }

			public override string ToString()
			{
				return string.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer<T> : Timer
		{
			private readonly TimerStateCallback<T> m_Callback;
			private readonly T m_State;

			public TimerStateCallback<T> Callback => m_Callback;

			public override bool DefRegCreation => false;

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T> callback, T state)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State = state;

				RegCreation();
			}

			protected override void OnTick()
			{
                m_Callback?.Invoke(m_State);
            }

			public override string ToString()
			{
				return string.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer<T1, T2> : Timer
		{
			private readonly TimerStateCallback<T1, T2> m_Callback;
			private readonly T1 m_State1;
			private readonly T2 m_State2;

			public TimerStateCallback<T1, T2> Callback => m_Callback;

			public override bool DefRegCreation => false;

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T1, T2> callback, T1 state1, T2 state2)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State1 = state1;
				m_State2 = state2;

				RegCreation();
			}

			protected override void OnTick()
			{
                m_Callback?.Invoke(m_State1, m_State2);
            }

			public override string ToString()
			{
				return string.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer<T1, T2, T3> : Timer
		{
			private readonly TimerStateCallback<T1, T2, T3> m_Callback;
			private readonly T1 m_State1;
			private readonly T2 m_State2;
			private readonly T3 m_State3;

			public TimerStateCallback<T1, T2, T3> Callback => m_Callback;

			public override bool DefRegCreation => false;

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T1, T2, T3> callback, T1 state1, T2 state2, T3 state3)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State1 = state1;
				m_State2 = state2;
				m_State3 = state3;

				RegCreation();
			}

			protected override void OnTick()
			{
                m_Callback?.Invoke(m_State1, m_State2, m_State3);
            }

			public override string ToString()
			{
				return string.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
			}
		}

		private class DelayStateCallTimer<T1, T2, T3, T4> : Timer
		{
			private readonly TimerStateCallback<T1, T2, T3, T4> m_Callback;
			private readonly T1 m_State1;
			private readonly T2 m_State2;
			private readonly T3 m_State3;
			private readonly T4 m_State4;

			public TimerStateCallback<T1, T2, T3, T4> Callback => m_Callback;

			public override bool DefRegCreation => false;

			public DelayStateCallTimer(TimeSpan delay, TimeSpan interval, int count, TimerStateCallback<T1, T2, T3, T4> callback, T1 state1, T2 state2, T3 state3, T4 state4)
				: base(delay, interval, count)
			{
				m_Callback = callback;
				m_State1 = state1;
				m_State2 = state2;
				m_State3 = state3;
				m_State4 = state4;

				RegCreation();
			}

			protected override void OnTick()
			{
                m_Callback?.Invoke(m_State1, m_State2, m_State3, m_State4);
            }

			public override string ToString()
			{
				return string.Format("DelayStateCall[{0}]", FormatDelegate(m_Callback));
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

			TimerProfile prof = GetProfile();

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

			TimerProfile prof = GetProfile();

			if (prof != null)
			{
				prof.Stopped++;
			}
		}

		protected virtual void OnTick()
		{ }
	}
}
