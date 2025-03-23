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
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPRestrictionListGump<TKey> : ListGump<TKey>
	{
		private const string _HelpText = "Restrictions: Lists specific restrictions for this battle.";

		public PvPBattleRestrictionsBase<TKey> Restrictions { get; set; }

		public virtual bool Locked { get; set; }
		public virtual bool UseConfirmDialog { get; set; }

		public PvPRestrictionListGump(
			Mobile user,
			PvPBattleRestrictionsBase<TKey> res,
			Gump parent = null,
			bool locked = true,
			bool useConfirm = true)
			: base(
				user,
				parent,
				emptyText: "There are no restrictions to display.",
				title: res != null ? res.ToString() : "Restrictions")
		{
			Restrictions = res;
			Locked = locked;
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;
		}

		protected override void CompileList(List<TKey> list)
		{
			list.Clear();
			list.AddRange(Restrictions.List.Keys);
			base.CompileList(list);
		}

		protected override int GetLabelHue(int index, int pageIndex, TKey entry)
		{
			return entry != null
				? (Restrictions.IsRestricted(entry) ? ErrorHue : HighlightHue)
				: base.GetLabelHue(index, pageIndex, default(TKey));
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.AppendEntry(new ListGumpEntry("Restrict All", OnRestrictAll, HighlightHue));
			list.AppendEntry(new ListGumpEntry("Unrestrict All", OnUnrestrictAll, HighlightHue));
			list.AppendEntry(new ListGumpEntry("Invert All", OnInvertAll, HighlightHue));

			if (!Locked && User.AccessLevel >= AutoPvP.Access)
			{
				list.AppendEntry(new ListGumpEntry("Delete All", OnDeleteAll, HighlightHue));
				list.AppendEntry(new ListGumpEntry("Add Entry", OnAddRestriction, HighlightHue));
			}

			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected virtual void ShowHelp(GumpButton button)
		{
			if (User != null && !User.Deleted)
			{
				new NoticeDialogGump(User, this, null, null, "Help", _HelpText).Send();
			}
		}

		protected virtual void OnDeleteAll(GumpButton button)
		{
			if (UseConfirmDialog)
			{
				new ConfirmDialogGump(User, this)
				{
					Title = "Delete All Entries?",
					Html = "All entries in the " + Restrictions + " will be deleted, erasing all data associated with them.\n" +
						   "This action can not be undone.\n\nDo you want to continue?",
					AcceptHandler = b =>
					{
						Restrictions.Clear();
						Refresh(true);
					},
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				Restrictions.Clear();
				Refresh(true);
			}
		}

		protected virtual void OnRestrictAll(GumpButton button)
		{
			if (UseConfirmDialog)
			{
				new ConfirmDialogGump(User, this)
				{
					Title = "Restrict All Entries?",
					Html = "All entries in the " + Restrictions + " will be restricted.\n" +
						   "This action can not be undone.\n\nDo you want to continue?",
					AcceptHandler = b =>
					{
						Restrictions.Reset(true);
						Refresh(true);
					},
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				Restrictions.Reset(true);
				Refresh(true);
			}
		}

		protected virtual void OnUnrestrictAll(GumpButton button)
		{
			if (UseConfirmDialog)
			{
				new ConfirmDialogGump(User, this)
				{
					Title = "Unrestrict All Entries?",
					Html = "All entries in the " + Restrictions + " will be unrestricted.\n" +
						   "This action can not be undone.\n\nDo you want to continue?",
					AcceptHandler = b =>
					{
						Restrictions.Reset(false);
						Refresh(true);
					},
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				Restrictions.Reset(false);
				Refresh(true);
			}
		}

		protected virtual void OnInvertAll(GumpButton button)
		{
			if (UseConfirmDialog)
			{
				new ConfirmDialogGump(User, this)
				{
					Title = "Invert All Entries?",
					Html = "All entries in the " + Restrictions + " will be toggled to their opposite setting.\n" +
						   "This action can not be undone.\n\nDo you want to continue?",
					AcceptHandler = b =>
					{
						Restrictions.Invert();
						Refresh(true);
					},
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				Restrictions.Invert();
				Refresh(true);
			}
		}

		protected virtual void OnAddRestriction(GumpButton button)
		{
			new InputDialogGump(User, this)
			{
				Title = "Add Restriction",
				Html = OnAddEntryGetHtml(),
				Callback = OnAddEntryConfirm,
				CancelHandler = Refresh
			}.Send();
		}

		protected virtual string OnAddEntryGetHtml()
		{
			return "Add an entry by name.";
		}

		protected virtual void OnAddEntryConfirm(GumpButton b, string text)
		{ }

		protected override void SelectEntry(GumpButton button, TKey entry)
		{
			base.SelectEntry(button, entry);

			if (button != null && entry != null)
			{
				new PvPRestrictionListEntryGump<TKey>(User, Restrictions, Refresh(), button, entry, Locked, UseConfirmDialog)
					.Send();
			}
		}
	}
}