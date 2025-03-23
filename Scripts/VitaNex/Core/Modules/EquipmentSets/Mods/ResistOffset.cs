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
	public class ResistOffsetSetMod : EquipmentSetMod
	{
		public string UID { get; private set; }

		public ResistanceType Resist { get; private set; }

		public int Offset { get; private set; }

		public ResistOffsetSetMod(string uid, string name, int partsReq, bool display, ResistanceType resist, int offset)
			: base(name, null, partsReq, display, ExpansionFlags.PostAOS)
		{
			UID = uid ?? Name + TimeStamp.UtcNow;

			Resist = resist;
			Offset = offset;

			InvalidateDesc();
		}

		public virtual void InvalidateDesc()
		{
			var name = Resist.ToString().SpaceWords().ToUpperWords();

			Desc = String.Format("{0} {1} Resistance By {2}%", Offset >= 0 ? "Increase" : "Decrease", name, Offset);
		}

		protected override bool OnActivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			if (m == null || m.Deleted || equipped == null)
			{
				return false;
			}

			if (EquipmentSets.CMOptions.ModuleDebug)
			{
				EquipmentSets.CMOptions.ToConsole("OnActivate: '{0}', '{1}', '{2}', '{3}'", m, UID, Resist, Offset);
			}

			UniqueResistMod.ApplyTo(m, Resist, UID, Offset);

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
				EquipmentSets.CMOptions.ToConsole("OnDeactivate: '{0}', '{1}', '{2}', '{3}'", m, UID, Resist, Offset);
			}

			UniqueResistMod.RemoveFrom(m, Resist, UID);

			return true;
		}
	}
}