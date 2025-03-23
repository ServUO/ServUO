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

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AntiAdverts
{
	public sealed class AntiAvertsReportsGump : ListGump<AntiAdvertsReport>
	{
		public AntiAvertsReportsGump(Mobile user)
			: base(user)
		{
			Title = "Anti-Advert Reports";
			EmptyText = "No reports to display.";

			Sorted = true;
			CanSearch = true;
			CanMove = false;
		}

		protected override void CompileList(List<AntiAdvertsReport> list)
		{
			list.Clear();
			list.AddRange(AntiAdverts.Reports);

			base.CompileList(list);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.AppendEntry(
				new ListGumpEntry(
					"Options",
					() =>
					{
						Refresh();
						User.SendGump(new PropertiesGump(User, AntiAdverts.CMOptions));
					},
					HighlightHue));

			list.AppendEntry(
				new ListGumpEntry("Key Words", () => new AntiAdvertsEditKeyWordsGump(User, this).Send(), HighlightHue));

			list.AppendEntry(
				new ListGumpEntry("Whitespace Aliases", () => new AntiAdvertsEditAliasesGump(User, this).Send(), HighlightHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Mark All: Viewed",
					() =>
					{
						AntiAdverts.Reports.ForEach(t => t.Viewed = true);

						User.SendMessage("All reports have been marked as viewed.");
						Refresh(true);
					},
					TextHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Mark All: Not Viewed",
					() =>
					{
						AntiAdverts.Reports.ForEach(t => t.Viewed = false);

						User.SendMessage("All reports have been marked as not viewed.");
						Refresh(true);
					},
					TextHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Delete All",
					() =>
					{
						AntiAdverts.Reports.Free(true);

						User.SendMessage("All reports have been deleted.");
						Refresh(true);
					},
					ErrorHue));

			list.AppendEntry(
				new ListGumpEntry(
					"Delete Old",
					() =>
					{
						var expire = DateTime.Now - TimeSpan.FromDays(7);

						AntiAdverts.Reports.RemoveAll(t => t.Date <= expire);
						AntiAdverts.Reports.Free(false);

						User.SendMessage("All old reports have been deleted.");
						Refresh(true);
					},
					ErrorHue));

			base.CompileMenuOptions(list);
		}

		protected override void SelectEntry(GumpButton button, AntiAdvertsReport entry)
		{
			base.SelectEntry(button, entry);

			var opts = new MenuGumpOptions();

			opts.AppendEntry(
				new ListGumpEntry(
					"View",
					() =>
					{
						entry.Viewed = true;

						new NoticeDialogGump(User, Refresh(true))
						{
							Title = "Anti-Advert Report",
							Html = entry.ToString(),
							Modal = false,
							CanMove = false
						}.Send();
					},
					HighlightHue));

			opts.AppendEntry(
				!entry.Viewed
					? new ListGumpEntry("Mark Viewed", () => entry.Viewed = true)
					: new ListGumpEntry("Mark Not Viewed", () => entry.Viewed = false));

			opts.AppendEntry(new ListGumpEntry("Delete", () => AntiAdverts.Reports.Remove(entry), ErrorHue));

			new MenuGump(User, Refresh(), opts, button).Send();
		}

		protected override int GetLabelHue(int index, int pageIndex, AntiAdvertsReport entry)
		{
			return entry != null ? entry == Selected ? HighlightHue : !entry.Viewed ? TextHue : ErrorHue : ErrorHue;
		}

		protected override string GetLabelText(int index, int pageIndex, AntiAdvertsReport entry)
		{
			return entry != null ? entry.ToString() : String.Empty;
		}

		public override string GetSearchKeyFor(AntiAdvertsReport key)
		{
			return key != null ? key.ToString() : String.Empty;
		}
	}
}