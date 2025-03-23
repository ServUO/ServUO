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
using System.Drawing;
using System.Linq;
using System.Text;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class TrashCollectionProfilesGump : ListGump<TrashProfile>
	{
		public bool SortByToday { get; set; }
		public bool UseConfirmDialog { get; set; }

		public TrashCollectionProfilesGump(Mobile user, Gump parent = null, bool useConfirm = true, bool sortByToday = false)
			: base(user, parent, emptyText: "There are no profiles to display.", title: "Trash Collection Profiles")
		{
			UseConfirmDialog = useConfirm;
			SortByToday = sortByToday;

			ForceRecompile = true;
			CanMove = false;
			CanResize = false;
		}

		protected override void Compile()
		{
			base.Compile();

			Title = String.Format("Trash Collection Profiles ({0})", (List.Count > 0) ? List.Count.ToString("#,#") : "0");
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (User.AccessLevel >= TrashCollection.Access)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Delete All",
						button => Send(
							new ConfirmDialogGump(User, this)
							{
								Title = "Delete All Profiles?",
								Html = "All profiles in the database will be deleted, erasing all data associated with them.\n" +
									   "This action can not be reversed.\n\nDo you want to continue?",
								AcceptHandler = subButton =>
								{
									TrashCollection.Profiles.Values.ToArray().Where(p => p != null && !p.Deleted).ForEach(p => p.Delete());

									Refresh(true);
								}
							}),
						ErrorHue));
			}

			list.AppendEntry(new ListGumpEntry("My Profile", OnMyProfile, HighlightHue));

			list.AppendEntry(
				new ListGumpEntry(
					SortByToday ? "Sort by Grand Total" : "Sort by Today's Total",
					b =>
					{
						SortByToday = !SortByToday;
						Refresh(true);
					}));

			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected override void CompileList(List<TrashProfile> list)
		{
			list.Clear();
			list.AddRange(TrashCollection.GetSortedProfiles(SortByToday));
			base.CompileList(list);
		}

		public override string GetSearchKeyFor(TrashProfile key)
		{
			return key != null && !key.Deleted ? key.Owner.RawName : base.GetSearchKeyFor(key);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Replace(
				"label/header/title",
				() => AddLabelCropped(160, 15, 215, 20, GetTitleHue(), String.IsNullOrEmpty(Title) ? DefaultTitle : Title));
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout,
			int length,
			int index,
			int pIndex,
			int yOffset,
			TrashProfile entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			layout.Replace(
				"label/list/entry/" + index,
				() =>
				{
					AddLabelCropped(65, 2 + yOffset, 160, 20, GetLabelHue(index, pIndex, entry), GetLabelText(index, pIndex, entry));
					AddLabelCropped(
						225,
						2 + yOffset,
						150,
						20,
						GetSortLabelHue(index, pIndex, entry),
						GetSortLabelText(index, pIndex, entry));
				});
		}

		protected override int GetLabelHue(int index, int pageIndex, TrashProfile entry)
		{
			return index < 3
				? HighlightHue
				: (entry != null
					? Notoriety.GetHue(Notoriety.Compute(User, entry.Owner))
					: base.GetLabelHue(index, pageIndex, null));
		}

		protected override string GetLabelText(int index, int pageIndex, TrashProfile entry)
		{
			return entry != null && entry.Owner != null
				? String.Format("{0}: {1}", (index + 1).ToString("#,#"), entry.Owner.RawName)
				: base.GetLabelText(index, pageIndex, entry);
		}

		protected virtual string GetSortLabelText(int index, int pageIndex, TrashProfile entry)
		{
			if (entry != null)
			{
				var val = SortByToday ? entry.GetTokenTotal(DateTime.UtcNow) : entry.GetTokenTotal();
				return String.Format("Tokens: {0}", (val > 0) ? val.ToString("#,#") : "0");
			}

			return String.Empty;
		}

		protected virtual int GetSortLabelHue(int index, int pageIndex, TrashProfile entry)
		{
			return entry != null ? ((index < 3) ? HighlightHue : TextHue) : ErrorHue;
		}

		protected override void SelectEntry(GumpButton button, TrashProfile entry)
		{
			base.SelectEntry(button, entry);

			if (button != null && entry != null && !entry.Deleted)
			{
				Send(new TrashCollectionProfileGump(User, entry, Hide(true), UseConfirmDialog));
			}
		}

		private void OnMyProfile(GumpButton button)
		{
			Send(new TrashCollectionProfileGump(User, TrashCollection.EnsureProfile(User), Hide(true), UseConfirmDialog));
		}

		private void ShowHelp(GumpButton button)
		{
			if (User == null || User.Deleted)
			{
				return;
			}

			var sb = TrashCollectionGumpUtility.GetHelpText(User);
			Send(
				new HtmlPanelGump<StringBuilder>(User, Refresh())
				{
					Selected = sb,
					Html = sb.ToString(),
					Title = "Trash Collection Help",
					HtmlColor = Color.SkyBlue
				});
		}
	}
}