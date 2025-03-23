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
using System.IO;
using System.Linq;

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public static class TournamentArchives
	{
		static TournamentArchives()
		{
			Registry = new List<TournamentArchive>();
		}

		public static void Initialize()
		{
			CommandUtility.Register("TournamentArchives", AutoPvP.Access, e => new TournamentArchivesUI(e.Mobile).Send());
			CommandUtility.RegisterAlias("TournamentArchives", "TArchives");
		}

		public static FileInfo File => IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/AutoPvP/Tournament/Archives.bin");

		public static List<TournamentArchive> Registry { get; private set; }

		public static int Count => Registry.Count;

		public static void Configure()
		{
			VitaNexCore.OnModuleLoaded += OnModuleLoaded;
			VitaNexCore.OnModuleSaved += OnModuleSaved;
		}

		private static void OnModuleLoaded(CoreModuleInfo cmi)
		{
			if (cmi.Enabled && cmi.TypeOf == typeof(AutoPvP))
			{
				File.Deserialize(r => r.ReadBlockList(r1 => new TournamentArchive(r1), Registry));
			}
		}

		private static void OnModuleSaved(CoreModuleInfo cmi)
		{
			if (cmi.Enabled && cmi.TypeOf == typeof(AutoPvP))
			{
				File.Serialize(w => w.WriteBlockList(Registry, (w1, o) => o.Serialize(w1)));
			}
		}

		public static TournamentArchive CreateArchive(TournamentBattle b)
		{
			TournamentArchive a = null;

			foreach (var r in b.Matches.Where(o => !o.IsDisposed && o.IsComplete && o.Records.Count > 0))
			{
				if (a == null)
				{
					a = new TournamentArchive(b.Name, DateTime.UtcNow);
				}

				a.Matches.Add(r);
			}

			if (a != null)
			{
				Registry.Add(a);
			}

			return a;
		}
	}
}