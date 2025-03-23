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
using System.Collections.Generic;
using System.Net;

using Server;

using VitaNex.IO;
using VitaNex.Web;
#endregion

namespace VitaNex.Modules.WebStats
{
	[CoreModule("Web Stats", "3.1.0.0")]
	public static partial class WebStats
	{
		static WebStats()
		{
			CMOptions = new WebStatsOptions();

			Snapshot = new Dictionary<IPAddress, List<Mobile>>();

			Stats = new BinaryDataStore<string, WebStatsEntry>(VitaNexCore.SavesDirectory + "/WebStats", "Stats")
			{
				Async = true,
				OnSerialize = SerializeStats,
				OnDeserialize = DeserializeStats
			};

			_Json = new Dictionary<string, object>();

			var uptime = VitaNexCore.UpTime;

			Stats["uptime"] = new WebStatsEntry(uptime, false);
			Stats["uptime_peak"] = new WebStatsEntry(uptime, true);

			Stats["online"] = new WebStatsEntry(0, false);
			Stats["online_max"] = new WebStatsEntry(0, false);
			Stats["online_peak"] = new WebStatsEntry(0, true);

			Stats["unique"] = new WebStatsEntry(0, false);
			Stats["unique_max"] = new WebStatsEntry(0, false);
			Stats["unique_peak"] = new WebStatsEntry(0, true);

			Stats["items"] = new WebStatsEntry(0, false);
			Stats["items_max"] = new WebStatsEntry(0, false);
			Stats["items_peak"] = new WebStatsEntry(0, true);

			Stats["mobiles"] = new WebStatsEntry(0, false);
			Stats["mobiles_max"] = new WebStatsEntry(0, false);
			Stats["mobiles_peak"] = new WebStatsEntry(0, true);

			Stats["guilds"] = new WebStatsEntry(0, false);
			Stats["guilds_max"] = new WebStatsEntry(0, false);
			Stats["guilds_peak"] = new WebStatsEntry(0, true);

			Stats["memory"] = new WebStatsEntry(0L, false);
			Stats["memory_max"] = new WebStatsEntry(0L, false);
			Stats["memory_peak"] = new WebStatsEntry(0L, true);
		}

		private static void CMConfig()
		{
			WebAPI.Register("/shard", HandleWebRequest);
			WebAPI.Register("/status", HandleWebRequest);
		}

		private static void CMSave()
		{
			Stats.Export();
		}

		private static void CMLoad()
		{
			Stats.Import();
		}

		private static bool SerializeStats(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlockDictionary(
						Stats,
						(w, k, v) =>
						{
							w.Write(k);
							v.Serialize(w);
						});
				}
				break;
			}

			return true;
		}

		private static bool DeserializeStats(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadString();
							var v = new WebStatsEntry(r);

							return new KeyValuePair<string, WebStatsEntry>(k, v);
						},
						Stats);
				}
				break;
			}

			return true;
		}
	}
}