#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;

using Server;
#endregion

namespace VitaNex
{
	[CoreService("PollTimer", "1.0.0.0", TaskPriority.Highest)]
	public sealed class PollTimer : Timer, IDisposable
	{
		public static PollTimer FromMilliseconds(
			double interval,
			Action callback,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromMilliseconds(interval), callback, condition, autoStart);
		}

		public static PollTimer FromMilliseconds<TObj>(
			double interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromMilliseconds(interval), callback, o, condition, autoStart);
		}

		public static PollTimer FromSeconds(
			double interval,
			Action callback,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromSeconds(interval), callback, condition, autoStart);
		}

		public static PollTimer FromSeconds<TObj>(
			double interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromSeconds(interval), callback, o, condition, autoStart);
		}

		public static PollTimer FromMinutes(
			double interval,
			Action callback,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromMinutes(interval), callback, condition, autoStart);
		}

		public static PollTimer FromMinutes<TObj>(
			double interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromMinutes(interval), callback, o, condition, autoStart);
		}

		public static PollTimer FromHours(
			double interval,
			Action callback,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromHours(interval), callback, condition, autoStart);
		}

		public static PollTimer FromHours<TObj>(
			double interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromHours(interval), callback, o, condition, autoStart);
		}

		public static PollTimer FromDays(double interval, Action callback, Func<bool> condition = null, bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromDays(interval), callback, condition, autoStart);
		}

		public static PollTimer FromDays<TObj>(
			double interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromDays(interval), callback, o, condition, autoStart);
		}

		public static PollTimer FromTicks(long interval, Action callback, Func<bool> condition = null, bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromTicks(interval), callback, condition, autoStart);
		}

		public static PollTimer FromTicks<TObj>(
			long interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return CreateInstance(TimeSpan.FromTicks(interval), callback, o, condition, autoStart);
		}

		public static PollTimer CreateInstance(
			TimeSpan interval,
			Action callback,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return new PollTimer(interval, callback, condition, autoStart);
		}

		public static PollTimer CreateInstance<TObj>(
			TimeSpan interval,
			Action<TObj> callback,
			TObj o,
			Func<bool> condition = null,
			bool autoStart = true)
		{
			return new PollTimer(interval, () => callback(o), condition, autoStart);
		}

		public bool IsDisposed { get; private set; }

		public bool IgnoreWorldIO { get; set; }

		public Func<bool> Condition { get; set; }
		public Action Callback { get; set; }

		public PollTimer(TimeSpan interval, Action callback, Func<bool> condition = null, bool autoStart = true)
			: base(interval, interval)
		{
			Condition = condition;
			Callback = callback;
			Running = autoStart;
		}

		~PollTimer()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			Running = false;
			Condition = null;
			Callback = null;
		}

		/// <summary>
		///     Calls the protected PollTimer.OnTick method to force the PollTimer to tick without affecting its current state.
		/// </summary>
		public void Tick()
		{
			OnTick();
		}

		protected override void OnTick()
		{
			base.OnTick();

			if (Callback != null && (IgnoreWorldIO || (!World.Loading && !World.Saving)))
			{
				if (Condition != null)
				{
					if (VitaNexCore.TryCatchGet(Condition, VitaNexCore.ToConsole))
					{
						VitaNexCore.TryCatch(Callback, VitaNexCore.ToConsole);
					}
				}
				else
				{
					VitaNexCore.TryCatch(Callback, VitaNexCore.ToConsole);
				}
			}

			if (Interval <= TimeSpan.Zero)
			{
				Running = false;
			}
			else
			{
				var p = ResolvePriority();

				if (Priority != p)
				{
					Priority = p;
				}
			}
		}

		private TimerPriority ResolvePriority()
		{
			var ms = Interval.TotalMilliseconds;

			if (ms >= 600000.0)
			{
				return TimerPriority.OneMinute;
			}

			if (ms >= 60000.0)
			{
				return TimerPriority.FiveSeconds;
			}

			if (ms >= 10000.0)
			{
				return TimerPriority.OneSecond;
			}

			if (ms >= 5000.0)
			{
				return TimerPriority.TwoFiftyMS;
			}

			if (ms >= 1000.0)
			{
				return TimerPriority.FiftyMS;
			}

			if (ms >= 500.0)
			{
				return TimerPriority.TwentyFiveMS;
			}

			if (ms >= 100.0)
			{
				return TimerPriority.TenMS;
			}

			return TimerPriority.EveryTick;
		}

		public override string ToString()
		{
			return String.Format("PollTimer[{0}]", FormatDelegate(Callback));
		}

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
	}
}