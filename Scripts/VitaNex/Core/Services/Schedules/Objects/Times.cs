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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;
#endregion

namespace VitaNex.Schedules
{
	public class ScheduleTimes : IEnumerable<TimeSpan>, ICloneable
	{
		private static readonly ScheduleTimes _None;
		private static readonly ScheduleTimes _Noon;
		private static readonly ScheduleTimes _Midnight;
		private static readonly ScheduleTimes _EveryHour;
		private static readonly ScheduleTimes _EveryHalfHour;
		private static readonly ScheduleTimes _EveryQuarterHour;
		private static readonly ScheduleTimes _EveryTenMinutes;
		private static readonly ScheduleTimes _EveryFiveMinutes;
		private static readonly ScheduleTimes _EveryMinute;
		private static readonly ScheduleTimes _FourTwenty;

		public static ScheduleTimes None => new ScheduleTimes(_None);
		public static ScheduleTimes Noon => new ScheduleTimes(_Noon);
		public static ScheduleTimes Midnight => new ScheduleTimes(_Midnight);
		public static ScheduleTimes EveryHour => new ScheduleTimes(_EveryHour);
		public static ScheduleTimes EveryHalfHour => new ScheduleTimes(_EveryHalfHour);
		public static ScheduleTimes EveryQuarterHour => new ScheduleTimes(_EveryQuarterHour);
		public static ScheduleTimes EveryTenMinutes => new ScheduleTimes(_EveryTenMinutes);
		public static ScheduleTimes EveryFiveMinutes => new ScheduleTimes(_EveryFiveMinutes);
		public static ScheduleTimes EveryMinute => new ScheduleTimes(_EveryMinute);
		public static ScheduleTimes FourTwenty => new ScheduleTimes(_FourTwenty);

		static ScheduleTimes()
		{
			_None = new ScheduleTimes();
			_Noon = new ScheduleTimes(TimeSpan.FromHours(12));
			_Midnight = new ScheduleTimes(TimeSpan.Zero);
			_EveryHour = new ScheduleTimes();
			_EveryHalfHour = new ScheduleTimes();
			_EveryQuarterHour = new ScheduleTimes();
			_EveryTenMinutes = new ScheduleTimes();
			_EveryFiveMinutes = new ScheduleTimes();
			_EveryMinute = new ScheduleTimes();

			for (var hours = 0; hours < 24; hours++)
			{
				_EveryHour.Add(new TimeSpan(hours, 0, 0));

				for (var minutes = 0; minutes < 60; minutes++)
				{
					_EveryMinute.Add(new TimeSpan(hours, minutes, 0));

					if (minutes % 5 == 0)
					{
						_EveryFiveMinutes.Add(new TimeSpan(hours, minutes, 0));
					}

					if (minutes % 10 == 0)
					{
						_EveryTenMinutes.Add(new TimeSpan(hours, minutes, 0));
					}

					if (minutes % 15 == 0)
					{
						_EveryQuarterHour.Add(new TimeSpan(hours, minutes, 0));
					}

					if (minutes % 30 == 0)
					{
						_EveryHalfHour.Add(new TimeSpan(hours, minutes, 0));
					}
				}
			}

			_FourTwenty = new ScheduleTimes(new TimeSpan(4, 20, 0), new TimeSpan(16, 20, 0));
		}

		private static void Validate(ref TimeSpan time)
		{
			time = new TimeSpan(0, time.Hours, time.Minutes, 0, 0);
		}

		private List<TimeSpan> _List = new List<TimeSpan>();

		public int Count => _List.Count;

		public TimeSpan? this[int index]
		{
			get => index < 0 || index >= _List.Count ? default(TimeSpan?) : _List[index];
			set
			{
				if (index < 0 || index >= _List.Count)
				{
					return;
				}

				if (value == null)
				{
					_List.RemoveAt(index);
				}
				else
				{
					_List[index] = (TimeSpan)value;
				}
			}
		}

		public ScheduleTimes(ScheduleTimes times)
		{
			Add(times);
		}

		public ScheduleTimes(IEnumerable<TimeSpan> times)
		{
			Add(times);
		}

		public ScheduleTimes(params TimeSpan[] times)
		{
			Add(times);
		}

		public ScheduleTimes(GenericReader reader)
		{
			Deserialize(reader);
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public virtual ScheduleTimes Clone()
		{
			return new ScheduleTimes(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _List.GetEnumerator();
		}

		public IEnumerator<TimeSpan> GetEnumerator()
		{
			return _List.GetEnumerator();
		}

		public bool Contains(TimeSpan time, bool validate = true)
		{
			if (validate)
			{
				Validate(ref time);
			}

			return _List.Contains(time);
		}

		public void Add(ScheduleTimes times)
		{
			foreach (var time in times)
			{
				InternalAdd(time);
			}

			_List.Sort();
		}

		public void Add(IEnumerable<TimeSpan> times)
		{
			foreach (var time in times)
			{
				InternalAdd(time);
			}

			_List.Sort();
		}

		public void Add(params TimeSpan[] times)
		{
			foreach (var time in times)
			{
				InternalAdd(time);
			}

			_List.Sort();
		}

		private void InternalAdd(TimeSpan time)
		{
			Validate(ref time);

			if (Contains(time, false))
			{
				return;
			}

			_List.Add(time);
		}

		public void Remove(ScheduleTimes times)
		{
			foreach (var time in times)
			{
				InternalRemove(time);
			}

			_List.TrimExcess();
			_List.Sort();
		}

		public void Remove(IEnumerable<TimeSpan> times)
		{
			foreach (var time in times)
			{
				InternalRemove(time);
			}

			_List.TrimExcess();
			_List.Sort();
		}

		public void Remove(params TimeSpan[] times)
		{
			foreach (var time in times)
			{
				InternalRemove(time);
			}

			_List.TrimExcess();
			_List.Sort();
		}

		private void InternalRemove(TimeSpan time)
		{
			Validate(ref time);

			if (!Contains(time, false))
			{
				return;
			}

			_List.Remove(time);
		}

		public void Clear()
		{
			_List.Clear();
			_List.TrimExcess();
		}

		public TimeSpan[] ToArray()
		{
			return _List.ToArray();
		}

		public string ToString(int cols)
		{
			if (!_List.IsNullOrEmpty())
			{
				return _List.Select(t => t.ToSimpleString("h:m")).ToWrappedString(", ", cols);
			}

			return "None";
		}

		public override string ToString()
		{
			return ToString(0);
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.WriteList(_List, (w, t) => w.Write(t));
					break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
					_List = reader.ReadList(r => r.ReadTimeSpan());
					break;
			}
		}
	}
}
