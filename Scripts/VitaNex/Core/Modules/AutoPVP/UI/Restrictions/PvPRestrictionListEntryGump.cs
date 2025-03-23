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
using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPRestrictionListEntryGump<TKey> : MenuGump
	{
		public PvPRestrictionListEntryGump(
			Mobile user,
			PvPBattleRestrictionsBase<TKey> res,
			Gump parent = null,
			GumpButton clicked = null,
			TKey key = default(TKey),
			bool locked = false,
			bool useConfirm = true)
			: base(user, parent, clicked: clicked)
		{
			Restrictions = res;
			Key = key;
			Locked = locked;
			UseConfirmDialog = useConfirm;
		}

		public PvPBattleRestrictionsBase<TKey> Restrictions { get; set; }

		public TKey Key { get; set; }
		public bool Locked { get; set; }
		public bool UseConfirmDialog { get; set; }

		protected override void CompileOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (User.AccessLevel >= AutoPvP.Access)
			{
				if (Restrictions.IsRestricted(Key))
				{
					list.AppendEntry(
						new ListGumpEntry(
							"Set Unrestricted",
							button =>
							{
								Restrictions.SetRestricted(Key, false);
								Refresh(true);
							},
							ErrorHue));
				}
				else
				{
					list.AppendEntry(
						new ListGumpEntry(
							"Set Restricted",
							button =>
							{
								Restrictions.SetRestricted(Key, true);
								Refresh(true);
							},
							HighlightHue));
				}

				if (!Locked && User.AccessLevel >= AutoPvP.Access)
				{
					list.AppendEntry(
						new ListGumpEntry(
							"Delete",
							button =>
							{
								if (UseConfirmDialog)
								{
									Send(
										new ConfirmDialogGump(
											User,
											Refresh(),
											title: "Delete Entry?",
											html:
											"All data associated with this entry will be deleted.\nThis action can not be reversed!\nDo you want to continue?",
											onAccept: OnConfirmDelete));
								}
								else
								{
									OnConfirmDelete(button);
								}
							}));
				}
			}

			base.CompileOptions(list);

			list.Replace("Cancel", new ListGumpEntry("Done", Cancel));
		}

		protected virtual void OnConfirmDelete(GumpButton button)
		{
			if (Selected == null)
			{
				Close();
				return;
			}

			Restrictions.Remove(Key);
			Close();
		}
	}
}