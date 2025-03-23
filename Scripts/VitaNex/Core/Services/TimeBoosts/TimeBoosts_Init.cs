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

using VitaNex.IO;
#endregion

namespace VitaNex.TimeBoosts
{
	[CoreService("Time Boosts", "1.0.0.0", TaskPriority.High)]
	public static partial class TimeBoosts
	{
		public static CoreServiceOptions CSOptions { get; private set; }

		static TimeBoosts()
		{
			Hours = new TimeBoostHours[] { 1, 3, 6, 12 };
			Minutes = new TimeBoostMinutes[] { 1, 3, 5, 15, 30 };

			Times = new[] { Hours.CastToArray<ITimeBoost>(), Minutes.CastToArray<ITimeBoost>() };

			AllTimes = Times.SelectMany(t => t).OrderBy(b => b.Value).ToArray();

			CSOptions = new CoreServiceOptions(typeof(TimeBoosts));

			Profiles = new BinaryDataStore<IAccount, TimeBoostProfile>(VitaNexCore.SavesDirectory + "/TimeBoosts", "Profiles")
			{
				Async = true,
				OnSerialize = Serialize,
				OnDeserialize = Deserialize
			};
		}

		private static void CSSave()
		{
			Profiles.Export();
		}

		private static void CSLoad()
		{
			Profiles.Import();
		}

		private static bool Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockDictionary(Profiles, (w, k, v) =>
			{
				w.Write(k);
				v.Serialize(w);
			});

			return true;
		}

		private static bool Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockDictionary(r =>
			{
				var k = r.ReadAccount();
				var v = new TimeBoostProfile(r);

				return new KeyValuePair<IAccount, TimeBoostProfile>(k, v);
			},
			Profiles);

			return true;
		}
	}
}