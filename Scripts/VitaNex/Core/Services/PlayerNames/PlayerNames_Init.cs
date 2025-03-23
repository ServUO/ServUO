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
using Server.Gumps;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex
{
	[CoreService("Player Name Register", "1.0.0.0", TaskPriority.Low)]
	public static partial class PlayerNames
	{
		static PlayerNames()
		{
			CSOptions = new PlayerNamesOptions();

			Registry = new BinaryDataStore<string, List<PlayerMobile>>(VitaNexCore.SavesDirectory + "/PlayerNames", "Registry")
			{
				OnSerialize = Serialize,
				OnDeserialize = Deserialize,
				Async = true
			};
		}

		private static void CSConfig()
		{
			EventSink.CharacterCreated += e => Register(e.Mobile as PlayerMobile);

			EventSink.Login += e =>
			{
				Register(e.Mobile as PlayerMobile);
				ValidateSharedName(e.Mobile as PlayerMobile);
			};

			EventSink.Logout += e => Register(e.Mobile as PlayerMobile);
			EventSink.PlayerDeath += e => Register(e.Mobile as PlayerMobile);

			CommandUtility.Register(
				"PlayerNames",
				Access,
				e =>
				{
					if (e.Arguments != null && e.Arguments.Length > 0 && Insensitive.Equals(e.Arguments[0], "index"))
					{
						e.Mobile.SendMessage("Indexing player names, please wait...");

						Index();

						e.Mobile.SendMessage(
							"Player name indexing complete, there are {0:#,0} registered names by {1:#,0} players.",
							Registry.Count,
							Registry.Values.Aggregate(0, (c, list) => c + list.Count));
					}
					else
					{
						e.Mobile.SendGump(new PropertiesGump(e.Mobile, CSOptions));
					}
				});
		}

		public static void Clear()
		{
			Registry.Clear();
		}

		public static void Index()
		{
			CSOptions.ToConsole("Indexing...");
			World.Broadcast(0x55, true, "Indexing player names...");

			World.Mobiles.Values.AsParallel().OfType<PlayerMobile>().ForEach(Register);

			World.Broadcast(0x55, true, "Indexing complete.");
			CSOptions.ToConsole("Indexing complete.");

			CSOptions.ToConsole(
				"{0:#,0} registered names by {1:#,0} players.",
				Registry.Count,
				Registry.Values.Aggregate(0, (c, list) => c + list.Count));
		}

		private static void CSInvoke()
		{
			if (CSOptions.IndexOnStart)
			{
				Index();
			}
		}

		private static void CSSave()
		{
			Registry.Export();
		}

		private static void CSLoad()
		{
			Registry.Import();
		}

		private static bool Serialize(GenericWriter writer)
		{
			writer.WriteBlockDictionary(
				Registry,
				(w, name, players) =>
				{
					w.Write(name);
					w.WriteMobileList(players, true);
				});

			return true;
		}

		private static bool Deserialize(GenericReader reader)
		{
			reader.ReadBlockDictionary(
				r =>
				{
					var name = r.ReadString();
					var players = r.ReadStrongMobileList<PlayerMobile>();
					return new KeyValuePair<string, List<PlayerMobile>>(name, players);
				},
				Registry);

			return true;
		}
	}
}