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
using System.Linq;

using Server;
using Server.Accounting;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.TimeBoosts
{
	public static partial class TimeBoosts
	{
		public static TimeBoostHours[] Hours { get; private set; }
		public static TimeBoostMinutes[] Minutes { get; private set; }

		public static ITimeBoost[][] Times { get; private set; }
		public static ITimeBoost[] AllTimes { get; private set; }

		public static ITimeBoost RandomHours => Hours.GetRandom();
		public static ITimeBoost RandomMinutes => Minutes.GetRandom();

		public static ITimeBoost RandomValue => AllTimes.GetRandom();

		public static ITimeBoost MinValue => AllTimes.Lowest(b => b.Value);
		public static ITimeBoost MaxValue => AllTimes.Highest(b => b.Value);

		public static BinaryDataStore<IAccount, TimeBoostProfile> Profiles { get; private set; }

		public static TimeBoostProfile FindProfile(PlayerMobile m)
		{
			if (m == null || m.Account == null)
			{
				return null;
			}

			return FindProfile(m.Account);
		}

		public static TimeBoostProfile FindProfile(IAccount a)
		{
			return Profiles.GetValue(a);
		}

		public static TimeBoostProfile EnsureProfile(PlayerMobile m)
		{
			if (m == null || m.Account == null)
			{
				return null;
			}

			return EnsureProfile(m.Account);
		}

		public static TimeBoostProfile EnsureProfile(IAccount a)
		{
			TimeBoostProfile profile = null;

			Profiles.Update(a, p => profile = p ?? new TimeBoostProfile(a));

			return profile;
		}

		public static IEnumerable<KeyValuePair<ITimeBoost, int>> FindBoosts(PlayerMobile m)
		{
			if (m == null || m.Account == null)
			{
				return Enumerable.Empty<KeyValuePair<ITimeBoost, int>>();
			}

			return FindBoosts(m.Account);
		}

		public static IEnumerable<KeyValuePair<ITimeBoost, int>> FindBoosts(IAccount a)
		{
			return FindProfile(a) ?? Enumerable.Empty<KeyValuePair<ITimeBoost, int>>();
		}

		#region Credit
		public static bool CanCredit(PlayerMobile m, ITimeBoost b, int amount)
		{
			return m != null && CanCredit(m.Account, b, amount);
		}

		public static bool Credit(PlayerMobile m, ITimeBoost b, int amount)
		{
			return m != null && Credit(m.Account, b, amount);
		}

		public static bool CreditHours(PlayerMobile m, int value)
		{
			return m != null && CreditHours(m.Account, value);
		}

		public static bool CreditMinutes(PlayerMobile m, int value)
		{
			return m != null && CreditMinutes(m.Account, value);
		}

		public static bool Credit(PlayerMobile m, int hours, int minutes)
		{
			return m != null && Credit(m.Account, hours, minutes);
		}

		public static bool CanCredit(IAccount a, ITimeBoost b, int amount)
		{
			var p = EnsureProfile(a);

			return p != null && p.CanCredit(b, amount);
		}

		public static bool Credit(IAccount a, ITimeBoost b, int amount)
		{
			var p = EnsureProfile(a);

			return p != null && p.Credit(b, amount);
		}

		public static bool CreditHours(IAccount a, int value)
		{
			var p = EnsureProfile(a);

			return p != null && p.CreditHours(value);
		}

		public static bool CreditMinutes(IAccount a, int value)
		{
			var p = EnsureProfile(a);

			return p != null && p.CreditMinutes(value);
		}

		public static bool Credit(IAccount a, int hours, int minutes)
		{
			var p = EnsureProfile(a);

			return p != null && p.Credit(hours, minutes);
		}
		#endregion Credit

		#region Consume
		public static bool CanConsume(PlayerMobile m, ITimeBoost b, int amount)
		{
			return m != null && CanConsume(m.Account, b, amount);
		}

		public static bool Consume(PlayerMobile m, ITimeBoost b, int amount)
		{
			return m != null && Consume(m.Account, b, amount);
		}

		public static bool ConsumeHours(PlayerMobile m, int value)
		{
			return m != null && ConsumeHours(m.Account, value);
		}

		public static bool ConsumeMinutes(PlayerMobile m, int value)
		{
			return m != null && ConsumeMinutes(m.Account, value);
		}

		public static bool Consume(PlayerMobile m, int hours, int minutes)
		{
			return m != null && Consume(m.Account, hours, minutes);
		}

		public static bool CanConsume(IAccount a, ITimeBoost b, int amount)
		{
			var p = EnsureProfile(a);

			return p != null && p.CanConsume(b, amount);
		}

		public static bool Consume(IAccount a, ITimeBoost b, int amount)
		{
			var p = EnsureProfile(a);

			return p != null && p.Consume(b, amount);
		}

		public static bool ConsumeHours(IAccount a, int value)
		{
			var p = EnsureProfile(a);

			return p != null && p.ConsumeHours(value);
		}

		public static bool ConsumeMinutes(IAccount a, int value)
		{
			var p = EnsureProfile(a);

			return p != null && p.ConsumeMinutes(value);
		}

		public static bool Consume(IAccount a, int hours, int minutes)
		{
			var p = EnsureProfile(a);

			return p != null && p.Consume(hours, minutes);
		}
		#endregion Consume

		public static ITimeBoost Find(TimeSpan time)
		{
			var index = AllTimes.Length;

			while (--index >= 0)
			{
				var b = AllTimes[index];

				if (time >= b.Value)
				{
					return b;
				}
			}

			return null;
		}

		public static void Write(this GenericWriter writer, ITimeBoost boost)
		{
			writer.SetVersion(0);

			if (boost != null)
			{
				writer.Write(true);
				writer.Write(boost.Value);
			}
			else
			{
				writer.Write(false);
			}
		}

		public static ITimeBoost ReadTimeBoost(this GenericReader reader)
		{
			reader.GetVersion();

			if (reader.ReadBool())
			{
				return Find(reader.ReadTimeSpan());
			}

			return null;
		}
	}
}