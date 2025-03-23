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

using Server;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.Voting
{
	[CoreModule("Voting", "2.0.0.0")]
	public static partial class Voting
	{
		static Voting()
		{
			SiteTypes = typeof(IVoteSite).GetConstructableChildren();

			CMOptions = new VotingOptions();

			VoteSites = new BinaryDataStore<int, IVoteSite>(VitaNexCore.SavesDirectory + "/Voting", "Sites")
			{
				Async = true,
				OnSerialize = SerializeVoteSites,
				OnDeserialize = DeserializeVoteSites
			};

			Profiles = new BinaryDataStore<PlayerMobile, VoteProfile>(VitaNexCore.SavesDirectory + "/Voting", "Profiles")
			{
				Async = true,
				OnSerialize = SerializeProfiles,
				OnDeserialize = DeserializeProfiles
			};
		}

		private static void CMConfig()
		{ }

		private static void CMEnabled()
		{ }

		private static void CMDisabled()
		{ }

		private static void CMInvoke()
		{
			if (VoteSites.Count <= 0)
			{
				var sites = new List<IVoteSite>();

				SiteTypes.ForEach(
					type =>
					{
						var site = type.CreateInstanceSafe<IVoteSite>();

						if (site == null)
						{
							return;
						}

						if (site.Name == "Vita-Nex")
						{
							site.Enabled = true;
						}

						sites.Add(site);
						CMOptions.ToConsole(
							"Created site ({0}) '{1}', '{2}'",
							site.GetType().Name,
							site.Name,
							site.Enabled ? "Enabled" : "Disabled");
					});

				sites.ForEach(s => VoteSites.Update(s.UID, s));
			}

			Prune();
		}

		private static void CMSave()
		{
			var result = VitaNexCore.TryCatchGet(VoteSites.Export, CMOptions.ToConsole);
			CMOptions.ToConsole(
				"{0:#,0} site{1} saved, {2}",
				VoteSites.Count,
				VoteSites.Count != 1 ? "s" : String.Empty,
				result);

			result = VitaNexCore.TryCatchGet(Profiles.Export, CMOptions.ToConsole);
			CMOptions.ToConsole(
				"{0:#,0} profile{1} saved, {2}",
				Profiles.Count,
				Profiles.Count != 1 ? "s" : String.Empty,
				result);
		}

		private static void CMLoad()
		{
			var result = VitaNexCore.TryCatchGet(VoteSites.Import, CMOptions.ToConsole);
			CMOptions.ToConsole(
				"{0:#,0} site{1} loaded, {2}.",
				VoteSites.Count,
				VoteSites.Count != 1 ? "s" : String.Empty,
				result);

			result = VitaNexCore.TryCatchGet(Profiles.Import, CMOptions.ToConsole);
			CMOptions.ToConsole(
				"{0:#,0} profile{1} loaded, {2}.",
				Profiles.Count,
				Profiles.Count != 1 ? "s" : String.Empty,
				result);
		}

		private static bool SerializeVoteSites(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.WriteBlockDictionary(VoteSites, (w, k, v) => w.WriteType(v, t => v.Serialize(w)));
					break;
			}

			return true;
		}

		private static bool DeserializeVoteSites(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					reader.ReadBlockDictionary(
						r =>
						{
							var v = r.ReadTypeCreate<IVoteSite>(r);
							return new KeyValuePair<int, IVoteSite>(v.UID, v);
						},
						VoteSites);
				}
				break;
			}

			return true;
		}

		private static bool SerializeProfiles(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlockDictionary(
						Profiles,
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

		private static bool DeserializeProfiles(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadMobile<PlayerMobile>();
							var v = new VoteProfile(r);
							return new KeyValuePair<PlayerMobile, VoteProfile>(k, v);
						},
						Profiles);
				}
				break;
			}

			return true;
		}
	}
}