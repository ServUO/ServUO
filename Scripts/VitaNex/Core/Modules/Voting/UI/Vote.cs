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

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Voting
{
	public sealed class VoteGump : ListGump<IVoteSite>
	{
		public VoteGump(PlayerMobile user, Gump parent = null)
			: base(user, parent, emptyText: "There are no sites to display.", title: "Vote Site Listing")
		{
			ForceRecompile = true;
		}

		public override string GetSearchKeyFor(IVoteSite key)
		{
			return key != null ? key.Name : base.GetSearchKeyFor(null);
		}

		protected override int GetLabelHue(int index, int pageIndex, IVoteSite entry)
		{
			return entry != null
				? (entry.CanVote(User as PlayerMobile, false) ? HighlightHue : ErrorHue)
				: base.GetLabelHue(index, pageIndex, null);
		}

		protected override string GetLabelText(int index, int pageIndex, IVoteSite entry)
		{
			return entry != null ? String.Format("{0}", entry.Name) : base.GetLabelText(index, pageIndex, null);
		}

		protected override void CompileList(List<IVoteSite> list)
		{
			list.Clear();
			list.AddRange(Voting.VoteSites.Values.Where(s => s != null && !s.Deleted && s.Enabled && s.Valid));

			base.CompileList(list);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.AppendEntry(new ListGumpEntry("View Profiles", ShowProfiles));
			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout,
			int length,
			int index,
			int pIndex,
			int yOffset,
			IVoteSite entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			layout.Replace(
				"label/list/entry/" + index,
				() =>
				{
					var text = GetLabelText(index, pIndex, entry);

					if (entry.Valid)
					{
						text = text.WrapUOHtmlUrl(entry.Link);
						AddHtml(65, 2 + yOffset, 325, 40, text, false, false);
					}
					else
					{
						AddLabelCropped(65, 2 + yOffset, 325, 20, GetLabelHue(index, pIndex, entry), text);
					}
				});
		}

		protected override void SelectEntry(GumpButton button, IVoteSite entry)
		{
			base.SelectEntry(button, entry);

			CastVote();
		}

		public override int SortCompare(IVoteSite a, IVoteSite b)
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

			if (!a.Valid && !b.Valid)
			{
				return 0;
			}

			if (!a.Valid)
			{
				return 1;
			}

			if (!b.Valid)
			{
				return -1;
			}

			if (a.Interval > b.Interval)
			{
				return 1;
			}

			if (a.Interval < b.Interval)
			{
				return -1;
			}

			return 0;
		}

		private void CastVote()
		{
			if (Selected != null)
			{
				Selected.Vote(User as PlayerMobile);
			}

			Refresh(true);
		}

		private void ShowProfiles(GumpButton button)
		{
			if (User != null && !User.Deleted)
			{
				Send(new VoteProfilesGump(User, Hide()));
			}
		}

		private void ShowHelp(GumpButton button)
		{
			if (User == null || User.Deleted)
			{
				return;
			}

			var sb = VoteGumpUtility.GetHelpText(User);
			var g = new HtmlPanelGump<StringBuilder>(User, Refresh())
			{
				Selected = sb,
				Title = "Voting Help",
				Html = sb.ToString(),
				HtmlColor = Color.SkyBlue
			};
			g.Send();
		}
	}
}