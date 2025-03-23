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
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex
{
	public static partial class PlayerNames
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static PlayerNamesOptions CSOptions { get; private set; }

		public static BinaryDataStore<string, List<PlayerMobile>> Registry { get; private set; }

		public static event Action<PlayerMobile> OnRegistered;

		private static void InvokeRegistered(PlayerMobile pm)
		{
			if (OnRegistered != null)
			{
				OnRegistered(pm);
			}
		}

		public static bool IsRegistered(PlayerMobile pm)
		{
			return pm != null && !String.IsNullOrWhiteSpace(FindRegisteredName(pm));
		}

		public static void Register(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return;
			}

			var rName = FindRegisteredName(pm);

			if (!String.IsNullOrWhiteSpace(rName) && rName != pm.RawName)
			{
				if (String.IsNullOrWhiteSpace(pm.RawName))
				{
					pm.RawName = rName;
					return;
				}

				Registry[rName].Remove(pm);
				Registry[rName].TrimExcess();

				if (Registry[rName].Count == 0)
				{
					Registry.Remove(rName);
				}
			}

			if (!Registry.ContainsKey(pm.RawName))
			{
				Registry.Add(
					pm.RawName,
					new List<PlayerMobile>
					{
						pm
					});
				InvokeRegistered(pm);
			}
			else if (!Registry[pm.RawName].Contains(pm))
			{
				Registry[pm.RawName].Add(pm);
				InvokeRegistered(pm);
			}
		}

		public static void ValidateSharedName(PlayerMobile m)
		{
			if (m != null && !CSOptions.NameSharing &&
				FindPlayers(m.RawName, p => p != m && p.CreationTime < m.CreationTime).Any())
			{
				new ForcePlayerRenameDialog(m).Send();
			}
		}

		public static bool HasNameChanged(PlayerMobile pm)
		{
			return pm != null && !pm.Deleted && pm.RawName != FindRegisteredName(pm);
		}

		public static string FindRegisteredName(PlayerMobile pm)
		{
			if (pm == null || pm.Deleted)
			{
				return String.Empty;
			}

			var val = Registry.Values.IndexOf(list => list.Contains(pm));

			if (Registry.InBounds(val))
			{
				return Registry.GetKeyAt(val);
			}

			return String.Empty;
		}

		public static IEnumerable<PlayerMobile> FindPlayers(string name)
		{
			return FindPlayers(name, null);
		}

		public static IEnumerable<PlayerMobile> FindPlayers(string name, Func<PlayerMobile, bool> match)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				yield break;
			}

			var players = Registry.GetValue(name);

			if (players == null || players.Count == 0)
			{
				yield break;
			}

			foreach (var m in (match == null ? players : players.Where(match)))
			{
				yield return m;
			}
		}
	}
}
