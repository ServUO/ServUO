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

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public class PvPProfileHistory
	{
		public PvPProfile Profile { get; private set; }

		public Dictionary<int, PvPProfileHistoryEntry> Entries { get; private set; }
		public Dictionary<int, PvPProfileHistoryEntry>.KeyCollection Keys => Entries.Keys;
		public Dictionary<int, PvPProfileHistoryEntry>.ValueCollection Values => Entries.Values;

		private PvPProfileHistory(PvPProfile owner)
		{
			Profile = owner;
		}

		public PvPProfileHistory(PvPProfile owner, params PvPProfileHistoryEntry[] entries)
			: this(owner)
		{
			if (entries == null)
			{
				Entries = new Dictionary<int, PvPProfileHistoryEntry>();
			}
			else
			{
				Entries = new Dictionary<int, PvPProfileHistoryEntry>(entries.Length);

				foreach (var entry in entries)
				{
					var season = AutoPvP.EnsureSeason(entry.Season);

					if (season != null)
					{
						Entries[season.Number] = entry;
					}
				}
			}
		}

		public PvPProfileHistory(PvPProfile owner, IDictionary<int, PvPProfileHistoryEntry> dictionary)
			: this(owner)
		{
			Entries = new Dictionary<int, PvPProfileHistoryEntry>(dictionary);
		}

		public PvPProfileHistory(PvPProfile owner, GenericReader reader)
			: this(owner)
		{
			Deserialize(reader);
		}

		public virtual PvPProfileHistoryEntry EnsureEntry(bool replace = false)
		{
			return EnsureEntry(AutoPvP.CurrentSeason, replace);
		}

		public virtual PvPProfileHistoryEntry EnsureEntry(PvPSeason season, bool replace = false)
		{
			if (!Entries.TryGetValue(season.Number, out var entry) || entry == null || replace)
			{
				Entries[season.Number] = entry = new PvPProfileHistoryEntry(season.Number);
			}

			return entry;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlockDictionary(Entries, (w, k, e) => w.WriteType(e, t => e.Serialize(w)));
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
				{
					Entries = reader.ReadBlockDictionary(
						r =>
						{
							var e = r.ReadTypeCreate<PvPProfileHistoryEntry>(r);

							return new KeyValuePair<int, PvPProfileHistoryEntry>(e.Season, e);
						},
						Entries);
				}
				break;
			}
		}
	}
}