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

using Server;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	public class StatOffsetSetMod : EquipmentSetMod
	{
		public string UID { get; private set; }

		public StatType Stat { get; private set; }

		public int Offset { get; private set; }

		public StatOffsetSetMod(string uid, string name, int partsReq, bool display, StatType stat, int offset)
			: base(name, null, partsReq, display)
		{
			UID = uid ?? Name + TimeStamp.UtcNow;

			Stat = stat;
			Offset = offset;

			InvalidateDesc();
		}

		public virtual void InvalidateDesc()
		{
			var statName = String.Empty;

			switch (Stat)
			{
				case StatType.All:
					statName = "All Stats";
					break;
				case StatType.Dex:
					statName = "Dexterity";
					break;
				case StatType.Int:
					statName = "Intelligence";
					break;
				case StatType.Str:
					statName = "Strength";
					break;
			}

			Desc = String.Format("{0} {1} By {2}", Offset >= 0 ? "Increase" : "Decrease", statName, Offset);
		}

		protected override bool OnActivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			if (m == null || m.Deleted || equipped == null)
			{
				return false;
			}

			if (EquipmentSets.CMOptions.ModuleDebug)
			{
				EquipmentSets.CMOptions.ToConsole("OnActivate: '{0}', '{1}', '{2}', '{3}'", m, UID, Stat, Offset);
			}

			UniqueStatMod.ApplyTo(m, Stat, UID, Offset, TimeSpan.Zero);

			return true;
		}

		protected override bool OnDeactivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			if (m == null || m.Deleted || equipped == null)
			{
				return false;
			}

			if (EquipmentSets.CMOptions.ModuleDebug)
			{
				EquipmentSets.CMOptions.ToConsole("OnDeactivate: '{0}', '{1}', '{2}', '{3}'", m, UID, Stat, Offset);
			}

			UniqueStatMod.RemoveFrom(m, Stat, UID);

			return true;
		}
	}
}