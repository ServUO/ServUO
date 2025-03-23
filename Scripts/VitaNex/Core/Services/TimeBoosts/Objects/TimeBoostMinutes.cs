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
#endregion

namespace VitaNex.TimeBoosts
{
	public struct TimeBoostMinutes : ITimeBoost
	{
		public int RawValue { get; private set; }
		public TimeSpan Value { get; private set; }

		public string Desc => "Minute";
		public string Name => String.Format("{0}-{1} Boost", RawValue, Desc);

		public int Hue => 2106;

		public TimeBoostMinutes(int minutes)
			: this()
		{
			Value = TimeSpan.FromMinutes(RawValue = minutes);
		}

		public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = RawValue;
				hash = (hash * 397) ^ Value.Days;
				hash = (hash * 397) ^ Value.Hours;
				hash = (hash * 397) ^ Value.Minutes;
				return hash;
			}
		}

		public static implicit operator TimeBoostMinutes(int minutes)
		{
			return new TimeBoostMinutes(minutes);
		}

		public static implicit operator TimeBoostMinutes(TimeSpan time)
		{
			return new TimeBoostMinutes(time.Minutes);
		}

		public static implicit operator TimeSpan(TimeBoostMinutes value)
		{
			return value.Value;
		}

		public static implicit operator int(TimeBoostMinutes value)
		{
			return value.RawValue;
		}
	}
}