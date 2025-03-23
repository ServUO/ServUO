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
using Server.Gumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPRestrictSkillsListGump : PvPRestrictionListGump<int>
	{
		public PvPBattleSkillRestrictions SkillRestrictions => Restrictions as PvPBattleSkillRestrictions;

		public PvPRestrictSkillsListGump(
			Mobile user,
			PvPBattleSkillRestrictions res,
			Gump parent = null,
			bool useConfirm = true)
			: base(user, res, parent, true, useConfirm)
		{ }

		protected override void CompileList(List<int> list)
		{
			base.CompileList(list);

			list.Sort((a, b) => (String.Compare(GetSearchKeyFor(a), GetSearchKeyFor(b), StringComparison.Ordinal)));
		}

		public override string GetSearchKeyFor(int key)
		{
			return key >= 0 ? ((SkillName)key).ToString() : base.GetSearchKeyFor(key);
		}

		protected override string GetLabelText(int index, int pageIndex, int entry)
		{
			return entry < 0 ? "Unknown" : ((SkillName)entry).ToString();
		}

		protected override string OnAddEntryGetHtml()
		{
			return base.OnAddEntryGetHtml() + "\nUse skill names without spaces.\n";
		}

		protected override void OnAddEntryConfirm(GumpButton b, string text)
		{
			SkillRestrictions.SetRestricted(text, true);
			Refresh(true);
		}
	}
}