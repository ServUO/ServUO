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
using System.Text;

using Server;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Voting
{
	public class VoteProfilesGump : ListGump<VoteProfile>
	{
		public bool SortByToday { get; set; }
		public bool UseConfirmDialog { get; set; }

		public VoteProfilesGump(Mobile user, Gump parent = null, bool useConfirm = true, bool sortByToday = false)
			: base(user, parent, emptyText: "There are no profiles to display.", title: "Vote Profiles")
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

			Title = String.Format("Vote Profiles ({0:#,0})", List.Count);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (User.AccessLevel >= Voting.Access)
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
									while (Voting.Profiles.Count > 0)
									{
										var p = Voting.Profiles.Pop();

										if (p.Value != null && !p.Value.Deleted)
										{
											p.Value.Delete();
										}
										else
										{
											Voting.Profiles.Remove(p.Key);
										}
									}

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

		protected override void CompileList(List<VoteProfile> list)
		{
			list.Clear();
			list.AddRange(Voting.Profiles.Values);

			base.CompileList(list);
		}

		public override string GetSearchKeyFor(VoteProfile key)
		{
			return key != null && !key.Deleted ? key.Owner.RawName : base.GetSearchKeyFor(key);
		}

		public override int SortCompare(VoteProfile a, VoteProfile b)
		{
			if (a == b)
			{
				return 0;
			}

			if (a == null)
			{
				return 1;
			}

			if (b == null)
			{
				return -1;
			}

			if (a.Deleted && b.Deleted)
			{
				return 0;
			}

			if (a.Deleted)
			{
				return 1;
			}

			if (b.Deleted)
			{
				return -1;
			}

			int aTotal;
			int bTotal;

			if (SortByToday)
			{
				var when = DateTime.UtcNow;

				aTotal = a.GetTokenTotal(when);
				bTotal = b.GetTokenTotal(when);
			}
			else
			{
				aTotal = a.GetTokenTotal();
				bTotal = b.GetTokenTotal();
			}

			if (aTotal > bTotal)
			{
				return -1;
			}

			if (aTotal < bTotal)
			{
				return 1;
			}

			return 0;
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
			VoteProfile entry)
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

		protected override int GetLabelHue(int index, int pageIndex, VoteProfile entry)
		{
			return index < 3
				? HighlightHue
				: (entry != null
					? Notoriety.GetHue(Notoriety.Compute(User, entry.Owner))
					: base.GetLabelHue(index, pageIndex, null));
		}

		protected override string GetLabelText(int index, int pageIndex, VoteProfile entry)
		{
			return entry != null && entry.Owner != null
				? String.Format("{0}: {1}", (index + 1).ToString("#,#"), entry.Owner.RawName)
				: base.GetLabelText(index, pageIndex, entry);
		}

		protected virtual string GetSortLabelText(int index, int pageIndex, VoteProfile entry)
		{
			if (entry != null)
			{
				var val = SortByToday ? entry.GetTokenTotal(DateTime.UtcNow) : entry.GetTokenTotal();
				return String.Format("Tokens: {0}", (val > 0) ? val.ToString("#,#") : "0");
			}

			return String.Empty;
		}

		protected virtual int GetSortLabelHue(int index, int pageIndex, VoteProfile entry)
		{
			return entry != null ? ((index < 3) ? HighlightHue : TextHue) : ErrorHue;
		}

		protected override void SelectEntry(GumpButton button, VoteProfile entry)
		{
			base.SelectEntry(button, entry);

			if (button != null && entry != null && !entry.Deleted)
			{
				Send(new VoteProfileGump(User, entry, Hide(true), UseConfirmDialog));
			}
		}

		private void OnMyProfile(GumpButton button)
		{
			Send(new VoteProfileGump(User, Voting.EnsureProfile(User as PlayerMobile), Hide(true), UseConfirmDialog));
		}

		private void ShowHelp(GumpButton button)
		{
			if (User == null || User.Deleted)
			{
				return;
			}

			var sb = VoteGumpUtility.GetHelpText(User);
			Send(
				new HtmlPanelGump<StringBuilder>(User, Hide(true))
				{
					Selected = sb,
					Html = sb.ToString(),
					Title = "Voting Help",
					HtmlColor = Color.SkyBlue
				});
		}
	}
}