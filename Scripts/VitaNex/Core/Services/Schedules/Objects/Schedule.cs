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
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Server;

using VitaNex.Crypto;
using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Schedules
{
	[PropertyObject]
	public class Schedule : Timer, ICloneable
	{
		public static List<Schedule> Instances { get; private set; }

		static Schedule()
		{
			Instances = new List<Schedule>(0x20);
		}

		private ScheduleInfo _Info;

		private TimerPriority _DefaultPriority;

		private bool _Enabled;
		private string _Name;

		private DateTime? _LastGlobalTick;
		private DateTime? _CurrentGlobalTick;
		private DateTime? _NextGlobalTick;

		[CommandProperty(Schedules.Access, true)]
		public CryptoHashCode UID { get; private set; }

		[CommandProperty(Schedules.Access)]
		public virtual string Name { get => _Name ?? (_Name = String.Empty); set => _Name = value ?? String.Empty; }

		[CommandProperty(Schedules.Access)]
		public virtual bool Enabled
		{
			get => _Enabled;
			set
			{
				if (!_Enabled && value)
				{
					_Enabled = true;

					InvalidateNextTick();

					Priority = ComputePriority(Interval = TimeSpan.FromSeconds(1.0));

					if (OnEnabled != null)
					{
						OnEnabled(this);
					}
				}
				else if (_Enabled && !value)
				{
					_Enabled = false;

					InvalidateNextTick();

					Priority = ComputePriority(Interval = TimeSpan.FromMinutes(1.0));

					if (OnDisabled != null)
					{
						OnDisabled(this);
					}
				}
			}
		}

		[CommandProperty(Schedules.Access)]
		public virtual ScheduleInfo Info
		{
			get => _Info ?? (_Info = new ScheduleInfo());
			set
			{
				_Info = value ?? new ScheduleInfo();

				InvalidateNextTick();
			}
		}

		[CommandProperty(Schedules.Access)]
		public virtual DateTime? LastGlobalTick => _LastGlobalTick;

		[CommandProperty(Schedules.Access)]
		public virtual DateTime? CurrentGlobalTick => _CurrentGlobalTick;

		[CommandProperty(Schedules.Access)]
		public virtual DateTime? NextGlobalTick => _NextGlobalTick;

		[CommandProperty(Schedules.Access)]
		public TimeSpan WaitGlobalTick
		{
			get
			{
				if (_NextGlobalTick != null)
				{
					return TimeSpan.FromTicks(Math.Max(0, (_NextGlobalTick.Value - Now).Ticks));
				}

				return TimeSpan.Zero;
			}
		}

		[CommandProperty(Schedules.Access)]
		public bool IsRegistered => Schedules.IsRegistered(this);

		[CommandProperty(Schedules.Access)]
		public bool IsLocal
		{
			get => _Info.Local;
			set
			{
				_Info.Local = value;

				InvalidateNextTick();
			}
		}

		public DateTime Now => IsLocal ? DateTime.Now : DateTime.UtcNow;

		public event Action<Schedule> OnGlobalTick;
		public event Action<Schedule> OnEnabled;
		public event Action<Schedule> OnDisabled;

		public Schedule(
			string name,
			bool enabled,
			ScheduleMonths months = ScheduleMonths.None,
			ScheduleDays days = ScheduleDays.None,
			ScheduleTimes times = null,
			params Action<Schedule>[] onTick)
			: this(name, enabled, new ScheduleInfo(months, days, times), onTick)
		{ }

		public Schedule(string name, bool enabled, ScheduleInfo info, params Action<Schedule>[] onTick)
			: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
		{
			UID = new CryptoHashCode(CryptoHashType.MD5, TimeStamp.Now + "+" + Utility.RandomDouble());

			_Enabled = enabled;
			_Name = name ?? String.Empty;
			_Info = info ?? new ScheduleInfo();

			Instances.Add(this);

			UpdateTicks(Now);

			if (onTick != null)
			{
				foreach (var a in onTick)
				{
					OnGlobalTick += a;
				}
			}

			Start();
		}

		public Schedule(GenericReader reader)
			: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
		{
			Instances.Add(this);

			Deserialize(reader);
		}

		~Schedule()
		{
			Free();
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public virtual Schedule Clone()
		{
			return new Schedule(Name, Enabled, Info.Clone());
		}

		public void Free()
		{
			_LastGlobalTick = null;
			_CurrentGlobalTick = null;
			_NextGlobalTick = null;

			OnGlobalTick = null;
			OnEnabled = null;
			OnDisabled = null;

			Instances.Remove(this);
		}

		public void Register()
		{
			if (!IsRegistered)
			{
				Schedules.Register(this);
			}
		}

		public void Unregister()
		{
			if (IsRegistered)
			{
				Schedules.Unregister(this);
			}
		}

		private void UpdateTicks(DateTime dt)
		{
			_LastGlobalTick = dt;

			InvalidateNextTick(dt);
		}

		public void InvalidateNextTick()
		{
			InvalidateNextTick(Now);
		}

		public void InvalidateNextTick(DateTime dt)
		{
			if (!_Enabled)
			{
				_NextGlobalTick = null;
				return;
			}

			_NextGlobalTick = _Info.FindAfter(dt);

			if (_NextGlobalTick != null && _NextGlobalTick < Now)
			{
				InvalidateNextTick(_NextGlobalTick.Value);
			}
		}

		protected override void OnTick()
		{
			base.OnTick();

			if (!_Enabled)
			{
				_LastGlobalTick = null;
				_CurrentGlobalTick = null;
				_NextGlobalTick = null;
				return;
			}

			var now = Now;

			if (_NextGlobalTick == null)
			{
				InvalidateNextTick(now);
			}

			if (_NextGlobalTick == null || now < _NextGlobalTick)
			{
				return;
			}

			_CurrentGlobalTick = now;

			InvalidateNextTick(now);

			if (OnGlobalTick != null)
			{
				OnGlobalTick(this);
			}

			_LastGlobalTick = now;
			_CurrentGlobalTick = null;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(3);

			if (version > 2)
			{
				UID.Serialize(writer);
			}

			switch (version)
			{
				case 3:
				case 2:
				case 1:
				case 0:
				{
					if (version < 2)
					{
						writer.WriteType(_Info, t => _Info.Serialize(writer));
					}
					else
					{
						writer.WriteBlock(w => w.WriteType(_Info, t => _Info.Serialize(w)));
					}

					writer.Write(_Enabled);
					writer.Write(_Name);
					writer.WriteFlag(_DefaultPriority);

					if (_LastGlobalTick != null)
					{
						writer.Write(true);
						writer.Write(_LastGlobalTick.Value);
					}
					else
					{
						writer.Write(false);
					}

					if (_NextGlobalTick != null)
					{
						writer.Write(true);
						writer.Write(_NextGlobalTick.Value);
					}
					else
					{
						writer.Write(false);
					}

					writer.Write(Delay);
					writer.Write(Interval);
				}
				break;
			}

			if (version > 0)
			{
				writer.Write(Running);
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			if (version > 2)
			{
				UID = new CryptoHashCode(reader);
			}
			else
			{
				UID = new CryptoHashCode(CryptoHashType.MD5, TimeStamp.Now + "+" + Utility.RandomDouble());
			}

			switch (version)
			{
				case 3:
				case 2:
				case 1:
				case 0:
				{
					if (version < 2)
					{
						_Info = reader.ReadTypeCreate<ScheduleInfo>(reader) ?? new ScheduleInfo();
					}
					else
					{
						_Info = reader.ReadBlock(r => r.ReadTypeCreate<ScheduleInfo>(r)) ?? new ScheduleInfo();
					}

					_Enabled = reader.ReadBool();
					_Name = reader.ReadString();
					_DefaultPriority = reader.ReadFlag<TimerPriority>();

					if (reader.ReadBool())
					{
						_LastGlobalTick = reader.ReadDateTime();
					}

					if (reader.ReadBool())
					{
						_NextGlobalTick = reader.ReadDateTime();
					}

					Delay = reader.ReadTimeSpan();
					Interval = reader.ReadTimeSpan();
				}
				break;
			}

			InvalidateNextTick();

			if (version > 0)
			{
				Running = reader.ReadBool();
			}
		}

		public override int GetHashCode()
		{
			return UID.ValueHash;
		}

		public override string ToString()
		{
			return _Name;
		}

		public virtual string ToHtmlString(bool big = true)
		{
			var now = Now;
			var html = new StringBuilder();

			html.AppendLine("Current Date: {0}", now.ToSimpleString("D, M d y"));
			html.AppendLine("Current Time: {0}", now.ToSimpleString("t@h:m@ X"));
			html.AppendLine();
			html.AppendLine("Schedule Overview:");
			html.AppendLine();

			if (!_Enabled)
			{
				html.AppendLine("Schedule is currently disabled.".WrapUOHtmlColor(Color.IndianRed));

				return html.ToString();
			}

			var print = false;

			var months = String.Join(", ", _Info.Months.EnumerateValues<string>(true).Not("None".Equals));
			var days = String.Join(", ", _Info.Days.EnumerateValues<string>(true).Not("None".Equals));

			var times = _Info.Times.ToString(6);

			if (months == "None" || String.IsNullOrWhiteSpace(months))
			{
				html.AppendLine("Schedule requires at least one Month to be set.".WrapUOHtmlColor(Color.IndianRed));
			}
			else if (days == "None" || String.IsNullOrWhiteSpace(days))
			{
				html.AppendLine("Schedule requires at least one Day to be set.".WrapUOHtmlColor(Color.IndianRed));
			}
			else if (times == "None" || String.IsNullOrWhiteSpace(times))
			{
				html.AppendLine("Schedule requires at least one Time to be set.".WrapUOHtmlColor(Color.IndianRed));
			}
			else
			{
				print = true;
			}

			if (print)
			{
				html.Append(String.Empty.WrapUOHtmlColor(Color.Cyan, false));

				html.AppendLine("Schedule is set to perform an action:");
				html.AppendLine();
				html.AppendLine("<B>In...</B>");
				html.AppendLine(months);
				html.AppendLine();
				html.AppendLine("<B>On...</B>");
				html.AppendLine(days);
				html.AppendLine();
				html.AppendLine("<B>At...</B>");
				html.AppendLine(times);

				html.Append(String.Empty.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false));

				if (_NextGlobalTick != null)
				{
					var today = _NextGlobalTick.Value.Day == now.Day;

					var t = _NextGlobalTick.Value.ToSimpleString("t@h:m@ X");
					var d = today ? "today" : ("on " + _NextGlobalTick.Value.ToSimpleString("D, M d y"));
					var o = WaitGlobalTick.ToSimpleString(@"!<d\d ><h\h ><m\m >s\s");

					html.AppendLine();
					html.AppendLine("The next tick will be at {0} {1}, in {2}.", t, d, o);
				}
			}

			var value = big ? String.Format("<big>{0}</big>", html) : html.ToString();

			return value.WrapUOHtmlColor(SuperGump.DefaultHtmlColor, false);
		}
	}
}
