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
using Server.Commands;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.Games
{
	[CoreModule("Games Arcade", "1.0.0.0")]
	public static class Arcade
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static ArcadeOptions CMOptions { get; private set; }

		public static BinaryDataStore<Type, IGame> Games { get; private set; }

		public static BinaryDataStore<Mobile, ArcadeProfile> Profiles { get; private set; }

		public static Type[] GameTypes { get; private set; }

		static Arcade()
		{
			CMOptions = new ArcadeOptions();

			Games = new BinaryDataStore<Type, IGame>(VitaNexCore.SavesDirectory + "/Arcade", "Games")
			{
				Async = true,
				OnSerialize = SerializeGames,
				OnDeserialize = DeserializeGames
			};

			Profiles = new BinaryDataStore<Mobile, ArcadeProfile>(VitaNexCore.SavesDirectory + "/Arcade", "Profiles")
			{
				Async = true,
				OnSerialize = SerializeProfiles,
				OnDeserialize = DeserializeProfiles
			};

			GameTypes = typeof(IGame).GetConstructableChildren();

			foreach (var t in GameTypes)
			{
				var g = CreateGame(t);

				if (g != null)
				{
					Games[t] = g;
				}
			}
		}

		private static void CMConfig()
		{
			CommandUtility.Register("Arcade", AccessLevel.Player, HandleCommand);
			CommandUtility.RegisterAlias("Arcade", "Games");
		}

		private static void CMInvoke()
		{ }

		private static void CMSave()
		{
			Games.Export();
		}

		private static void CMLoad()
		{
			Games.Import();
		}

		private static void HandleCommand(CommandEventArgs e)
		{
			if (CMOptions.ModuleEnabled)
			{
				new ArcadeUI(e.Mobile).Send();
			}
		}

		private static IGame CreateGame(Type t)
		{
			try
			{
				return t.CreateInstanceSafe<IGame>();
			}
			catch
			{
				return null;
			}
		}

		public static ArcadeProfile EnsureProfile(Mobile m)
		{
			if (m == null)
			{
				return null;
			}

			var p = Profiles.GetValue(m);

			if (p == null || p.Owner != m)
			{
				if (!m.Player || m.Deleted)
				{
					Profiles.Remove(m);

					if (p != null)
					{
						p.Clear();
						p = null;
					}
				}
				else
				{
					Profiles[m] = p = new ArcadeProfile(m);
				}
			}
			else if (!m.Player || m.Deleted)
			{
				Profiles.Remove(m);

				p.Clear();
				p = null;
			}

			return p;
		}

		public static bool OpenGame(Type t, Mobile m)
		{
			return VitaNexCore.TryCatchGet(
				() =>
				{
					var g = Games.GetValue(t);

					return g != null && g.Open(m);
				},
				CMOptions.ToConsole);
		}

		public static bool OpenGame<T>(Mobile m)
			where T : IGame
		{
			return OpenGame(typeof(T), m);
		}

		public static void CloseGame(Mobile m, Type t)
		{
			VitaNexCore.TryCatch(
				() =>
				{
					var g = Games.GetValue(t);

					if (g != null)
					{
						g.Close(m);
					}
				},
				CMOptions.ToConsole);
		}

		private static bool SerializeGames(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlockDictionary(
						Games,
						(w, t, g) =>
						{
							w.WriteType(t);
							g.Serialize(w);
						});
				}
				break;
			}

			return true;
		}

		private static bool DeserializeGames(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					reader.ReadBlockDictionary(
						r =>
						{
							var t = r.ReadType();
							var g = Games.GetValue(t) ?? CreateGame(t);

							g.Deserialize(r);

							return new KeyValuePair<Type, IGame>(t, g);
						},
						Games);
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
						(w, m, p) =>
						{
							w.Write(m);
							p.Serialize(w);
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
							var m = r.ReadMobile();
							var p = new ArcadeProfile(r);

							return new KeyValuePair<Mobile, ArcadeProfile>(m, p);
						},
						Profiles);
				}
				break;
			}

			return true;
		}
	}
}