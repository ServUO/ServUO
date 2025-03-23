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
	public class SkillOffsetSetMod : EquipmentSetMod
	{
		public string UID { get; private set; }

		public SkillName Skill { get; private set; }

		public double Offset { get; private set; }

		public SkillOffsetSetMod(string uid, string name, int partsReq, bool display, SkillName skill, double offset)
			: base(name, null, partsReq, display)
		{
			UID = uid ?? Name + TimeStamp.UtcNow;

			Skill = skill;
			Offset = offset;

			InvalidateDesc();
		}

		public virtual void InvalidateDesc()
		{
			var name = Skill.GetName();

			Desc = String.Format("{0} {1} By {2:F1}", Offset >= 0 ? "Increase" : "Decrease", name, Offset);
		}

		protected override bool OnActivate(Mobile m, Tuple<EquipmentSetPart, Item>[] equipped)
		{
			if (m == null || m.Deleted || equipped == null)
			{
				return false;
			}

			if (EquipmentSets.CMOptions.ModuleDebug)
			{
				EquipmentSets.CMOptions.ToConsole("OnActivate: '{0}', '{1}', '{2}', '{3}'", m, UID, Skill, Offset);
			}

			UniqueSkillMod.ApplyTo(m, Skill, UID, true, Offset);

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
				EquipmentSets.CMOptions.ToConsole("OnDeactivate: '{0}', '{1}', '{2}', '{3}'", m, UID, Skill, Offset);
			}

			UniqueSkillMod.RemoveFrom(m, Skill, UID);

			return true;
		}
	}
}