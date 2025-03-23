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
using System.Threading.Tasks;

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex
{
	[CoreService("Clilocs", "1.0.0.0", TaskPriority.High)]
	public static partial class Clilocs
	{
		static Clilocs()
		{
			CSOptions = new CoreServiceOptions(typeof(Clilocs));

			Tables = new Dictionary<ClilocLNG, ClilocTable>
			{
				{ClilocLNG.ENU, new ClilocTable()},
				{ClilocLNG.DEU, new ClilocTable()},
				{ClilocLNG.ESP, new ClilocTable()},
				{ClilocLNG.FRA, new ClilocTable()},
				{ClilocLNG.JPN, new ClilocTable()},
				{ClilocLNG.KOR, new ClilocTable()},
				{ClilocLNG.CHT, new ClilocTable()}
			};
		}

		private static void CSConfig()
		{
			CommandUtility.Register("ExportCliloc", AccessLevel.Administrator, ExportCommand);

			var tables = new List<ClilocTable>(Tables.Values);

			//bool noFind = false;

			Core.DataDirectories.TakeWhile(path => !tables.TrueForAll(t => t.Loaded))
				.Where(path => !String.IsNullOrWhiteSpace(path))
				.ForEach(
					path => Parallel.ForEach(
						Tables,
						kvp =>
						{
							if (kvp.Value.Loaded)
							{
								return;
							}

							var file = "Cliloc." + kvp.Key.ToString().ToLower();
							var stub = IOUtility.GetSafeFilePath(path + "/" + file, true);

							if (!File.Exists(stub))
							{
								//CSOptions.ToConsole("WARNING: {0} not found!", file);
								//noFind = true;
								return;
							}

							kvp.Value.Load(new FileInfo(stub));
						}));

			/*if (noFind)
			{
				CSOptions.ToConsole(
					"WARNING: One or more required cliloc files could not be loaded, any features that rely on this service will not work as expected and/or may cause a fatal exception!");
			}*/

			tables.Free(true);
		}

		private static void CSInvoke()
		{ }
	}
}