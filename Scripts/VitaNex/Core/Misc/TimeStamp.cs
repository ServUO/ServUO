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
	public struct TimeStamp
		: IComparable<TimeStamp>,
		  IComparable<DateTime>,
		  IComparable<TimeSpan>,
		  IComparable<double>,
		  IComparable<long>,
		  IEquatable<TimeStamp>,
		  IEquatable<DateTime>,
		  IEquatable<TimeSpan>,
		  IEquatable<double>,
		  IEquatable<long>
	{
		public static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);

		public static TimeStamp Zero => Origin;

		public static TimeStamp Now => DateTime.Now;
		public static TimeStamp UtcNow => DateTime.UtcNow;

		public static TimeStamp FromTicks(long value)
		{
			return new TimeStamp(value);
		}

		public static TimeStamp FromTicks(long value, DateTimeKind kind)
		{
			return new TimeStamp(value, kind);
		}

		public static TimeStamp FromMilliseconds(double value)
		{
			return new TimeStamp(value / 1000.0);
		}

		public static TimeStamp FromMilliseconds(double value, DateTimeKind kind)
		{
			return new TimeStamp(value / 1000.0, kind);
		}

		public static TimeStamp FromSeconds(double value)
		{
			return new TimeStamp(value);
		}

		public static TimeStamp FromSeconds(double value, DateTimeKind kind)
		{
			return new TimeStamp(value, kind);
		}

		public static TimeStamp FromMinutes(double value)
		{
			return new TimeStamp(value * 60);
		}

		public static TimeStamp FromMinutes(double value, DateTimeKind kind)
		{
			return new TimeStamp(value * 60, kind);
		}

		public static TimeStamp FromHours(double value)
		{
			return new TimeStamp(value * 3600);
		}

		public static TimeStamp FromHours(double value, DateTimeKind kind)
		{
			return new TimeStamp(value * 3600, kind);
		}

		public static TimeStamp FromDays(double value)
		{
			return new TimeStamp(value * 86400);
		}

		public static TimeStamp FromDays(double value, DateTimeKind kind)
		{
			return new TimeStamp(value * 86400, kind);
		}

		public static TimeStamp FromDateTime(DateTime date)
		{
			return new TimeStamp(date);
		}

		public static int Compare(TimeStamp l, TimeStamp r)
		{
			return l.CompareTo(r);
		}

		public DateTimeKind Kind { get; private set; }
		public DateTime Value { get; private set; }
		public double Stamp { get; private set; }

		public long Ticks => Value.Ticks;

		public TimeStamp(DateTime date)
			: this(date.Kind)
		{
			Value = date;
			Stamp = ResolveStamp();
		}

		public TimeStamp(TimeSpan time)
			: this(time, DateTimeKind.Unspecified)
		{ }

		public TimeStamp(TimeSpan time, DateTimeKind kind)
			: this(kind)
		{
			Stamp = time.TotalSeconds;
			Value = ResolveDate();
		}

		public TimeStamp(double stamp)
			: this(stamp, DateTimeKind.Unspecified)
		{ }

		public TimeStamp(double stamp, DateTimeKind kind)
			: this(kind)
		{
			Stamp = stamp;
			Value = ResolveDate();
		}

		public TimeStamp(long ticks)
			: this(ticks, DateTimeKind.Unspecified)
		{ }

		public TimeStamp(long ticks, DateTimeKind kind)
			: this(kind)
		{
			Value = new DateTime(ticks, kind);
			Stamp = ResolveStamp();
		}

		public TimeStamp(DateTimeKind kind)
			: this()
		{
			Kind = kind;
		}

		public TimeStamp(GenericReader reader)
			: this(Origin)
		{
			Deserialize(reader);
		}

		private DateTime ResolveDate()
		{
			var dt = new DateTime(
				Origin.Year,
				Origin.Month,
				Origin.Day,
				Origin.Hour,
				Origin.Minute,
				Origin.Second,
				Origin.Millisecond,
				Kind);

			return dt.AddSeconds(Stamp);
		}

		private double ResolveStamp()
		{
			return Value.Subtract(Origin).TotalSeconds;
		}

		public TimeStamp Add(TimeSpan ts)
		{
			return Stamp + ts.TotalSeconds;
		}

		public TimeStamp Subtract(TimeSpan ts)
		{
			return Stamp - ts.TotalSeconds;
		}

		public override int GetHashCode()
		{
			return Stamp.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return (obj is TimeStamp && Equals((TimeStamp)obj)) //
				   || (obj is DateTime && Equals((DateTime)obj)) //
				   || (obj is TimeSpan && Equals((TimeSpan)obj)) //
				   || (obj is double && Equals((double)obj)) //
				   || (obj is long && Equals((long)obj));
		}

		public bool Equals(TimeStamp t)
		{
			return Stamp == t.Stamp;
		}

		public bool Equals(DateTime d)
		{
			return Value == d;
		}

		public bool Equals(TimeSpan t)
		{
			return Stamp == t.TotalSeconds;
		}

		public bool Equals(double stamp)
		{
			return Stamp == stamp;
		}

		public bool Equals(long ticks)
		{
			return Ticks == ticks;
		}

		public int CompareTo(TimeStamp t)
		{
			return Stamp.CompareTo(t.Stamp);
		}

		public int CompareTo(DateTime d)
		{
			return Value.CompareTo(d);
		}

		public int CompareTo(TimeSpan t)
		{
			return Stamp.CompareTo(t.TotalSeconds);
		}

		public int CompareTo(double stamp)
		{
			return Stamp.CompareTo(stamp);
		}

		public int CompareTo(long ticks)
		{
			return Ticks.CompareTo(ticks);
		}

		public override string ToString()
		{
			return String.Format("{0:F2}", Stamp);
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.WriteFlag(Kind);
					goto case 0;
				case 0:
					writer.Write(Stamp);
					break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
					Kind = reader.ReadFlag<DateTimeKind>();
					goto case 0;
				case 0:
				{
					Stamp = reader.ReadDouble();
					Value = ResolveDate();
				}
				break;
			}
		}

		#region TimeStamp Equality
		public static bool operator ==(TimeStamp l, TimeStamp r)
		{
			return l.Stamp.Equals(r.Stamp);
		}

		public static bool operator !=(TimeStamp l, TimeStamp r)
		{
			return !l.Stamp.Equals(r.Stamp);
		}

		public static bool operator >(TimeStamp l, TimeStamp r)
		{
			return l.Stamp > r.Stamp;
		}

		public static bool operator <(TimeStamp l, TimeStamp r)
		{
			return l.Stamp < r.Stamp;
		}

		public static bool operator >=(TimeStamp l, TimeStamp r)
		{
			return l.Stamp >= r.Stamp;
		}

		public static bool operator <=(TimeStamp l, TimeStamp r)
		{
			return l.Stamp <= r.Stamp;
		}
		#endregion

		#region DateTime Equality
		public static bool operator ==(TimeStamp l, DateTime r)
		{
			return l.Ticks.Equals(r.Ticks);
		}

		public static bool operator !=(TimeStamp l, DateTime r)
		{
			return !l.Ticks.Equals(r.Ticks);
		}

		public static bool operator >(TimeStamp l, DateTime r)
		{
			return l.Ticks > r.Ticks;
		}

		public static bool operator <(TimeStamp l, DateTime r)
		{
			return l.Ticks < r.Ticks;
		}

		public static bool operator >=(TimeStamp l, DateTime r)
		{
			return l.Ticks >= r.Ticks;
		}

		public static bool operator <=(TimeStamp l, DateTime r)
		{
			return l.Ticks <= r.Ticks;
		}

		public static bool operator ==(DateTime l, TimeStamp r)
		{
			return l.Ticks.Equals(r.Ticks);
		}

		public static bool operator !=(DateTime l, TimeStamp r)
		{
			return !l.Ticks.Equals(r.Ticks);
		}

		public static bool operator >(DateTime l, TimeStamp r)
		{
			return l.Ticks > r.Ticks;
		}

		public static bool operator <(DateTime l, TimeStamp r)
		{
			return l.Ticks < r.Ticks;
		}

		public static bool operator >=(DateTime l, TimeStamp r)
		{
			return l.Ticks >= r.Ticks;
		}

		public static bool operator <=(DateTime l, TimeStamp r)
		{
			return l.Ticks <= r.Ticks;
		}
		#endregion

		#region TimeSpan Equality
		public static bool operator ==(TimeStamp l, TimeSpan r)
		{
			return l.Stamp.Equals(r.TotalSeconds);
		}

		public static bool operator !=(TimeStamp l, TimeSpan r)
		{
			return !l.Stamp.Equals(r.TotalSeconds);
		}

		public static bool operator >(TimeStamp l, TimeSpan r)
		{
			return l.Stamp > r.TotalSeconds;
		}

		public static bool operator <(TimeStamp l, TimeSpan r)
		{
			return l.Stamp < r.TotalSeconds;
		}

		public static bool operator >=(TimeStamp l, TimeSpan r)
		{
			return l.Stamp >= r.TotalSeconds;
		}

		public static bool operator <=(TimeStamp l, TimeSpan r)
		{
			return l.Stamp <= r.TotalSeconds;
		}

		public static bool operator ==(TimeSpan l, TimeStamp r)
		{
			return l.TotalSeconds.Equals(r.Stamp);
		}

		public static bool operator !=(TimeSpan l, TimeStamp r)
		{
			return !l.TotalSeconds.Equals(r.Stamp);
		}

		public static bool operator >(TimeSpan l, TimeStamp r)
		{
			return l.TotalSeconds > r.Stamp;
		}

		public static bool operator <(TimeSpan l, TimeStamp r)
		{
			return l.TotalSeconds < r.Stamp;
		}

		public static bool operator >=(TimeSpan l, TimeStamp r)
		{
			return l.TotalSeconds >= r.Stamp;
		}

		public static bool operator <=(TimeSpan l, TimeStamp r)
		{
			return l.TotalSeconds <= r.Stamp;
		}
		#endregion

		#region Stamp Equality
		public static bool operator ==(TimeStamp l, double r)
		{
			return l.Stamp.Equals(r);
		}

		public static bool operator !=(TimeStamp l, double r)
		{
			return !l.Stamp.Equals(r);
		}

		public static bool operator >(TimeStamp l, double r)
		{
			return l.Stamp > r;
		}

		public static bool operator <(TimeStamp l, double r)
		{
			return l.Stamp < r;
		}

		public static bool operator >=(TimeStamp l, double r)
		{
			return l.Stamp >= r;
		}

		public static bool operator <=(TimeStamp l, double r)
		{
			return l.Stamp <= r;
		}

		public static bool operator ==(double l, TimeStamp r)
		{
			return l.Equals(r.Stamp);
		}

		public static bool operator !=(double l, TimeStamp r)
		{
			return !l.Equals(r.Stamp);
		}

		public static bool operator >(double l, TimeStamp r)
		{
			return l > r.Stamp;
		}

		public static bool operator <(double l, TimeStamp r)
		{
			return l < r.Stamp;
		}

		public static bool operator >=(double l, TimeStamp r)
		{
			return l >= r.Stamp;
		}

		public static bool operator <=(double l, TimeStamp r)
		{
			return l <= r.Stamp;
		}
		#endregion

		#region Ticks Equality
		public static bool operator ==(TimeStamp l, long r)
		{
			return l.Ticks.Equals(r);
		}

		public static bool operator !=(TimeStamp l, long r)
		{
			return !l.Ticks.Equals(r);
		}

		public static bool operator >(TimeStamp l, long r)
		{
			return l.Ticks > r;
		}

		public static bool operator <(TimeStamp l, long r)
		{
			return l.Ticks < r;
		}

		public static bool operator >=(TimeStamp l, long r)
		{
			return l.Ticks >= r;
		}

		public static bool operator <=(TimeStamp l, long r)
		{
			return l.Ticks <= r;
		}

		public static bool operator ==(long l, TimeStamp r)
		{
			return l.Equals(r.Ticks);
		}

		public static bool operator !=(long l, TimeStamp r)
		{
			return !l.Equals(r.Ticks);
		}

		public static bool operator >(long l, TimeStamp r)
		{
			return l > r.Ticks;
		}

		public static bool operator <(long l, TimeStamp r)
		{
			return l < r.Ticks;
		}

		public static bool operator >=(long l, TimeStamp r)
		{
			return l >= r.Ticks;
		}

		public static bool operator <=(long l, TimeStamp r)
		{
			return l <= r.Ticks;
		}
		#endregion

		public static implicit operator TimeStamp(DateTime date)
		{
			return new TimeStamp(date);
		}

		public static implicit operator TimeStamp(TimeSpan time)
		{
			return new TimeStamp(time);
		}

		public static implicit operator TimeStamp(double stamp)
		{
			return new TimeStamp(stamp);
		}

		public static implicit operator TimeStamp(long ticks)
		{
			return new TimeStamp(ticks);
		}
	}
}
