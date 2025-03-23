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
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class TournamentArchivesUI : ListGump<TournamentArchive>
	{
		public TournamentArchivesUI(Mobile user, Gump parent = null)
			: base(user, parent)
		{ }

		protected override void CompileList(List<TournamentArchive> list)
		{
			list.Clear();
			list.AddRange(TournamentArchives.Registry);

			base.CompileList(list);
		}

		public override string GetSearchKeyFor(TournamentArchive key)
		{
			if (key != null)
			{
				return key.Name + " @ " + key.Date;
			}

			return base.GetSearchKeyFor(null);
		}

		protected override string GetLabelText(int index, int pageIndex, TournamentArchive entry)
		{
			if (entry != null)
			{
				return entry.Name + " @ " + entry.Date.ToShortDateString();
			}

			return base.GetLabelText(index, pageIndex, null);
		}

		protected override void SelectEntry(GumpButton button, TournamentArchive entry)
		{
			base.SelectEntry(button, entry);

			new TournamentArchiveUI(User, entry, this).Send();
		}
	}

	public class TournamentArchiveUI : SuperGumpList<TournamentMatch>
	{
		public IEnumerable<TournamentMatch> Archive { get; private set; }

		public string Title { get; set; }

		public int EntryWidth { get; set; }
		public int EntryHeight { get; set; }

		public int Width { get; private set; }
		public int Height { get; private set; }

		public PlayerMobile Leader { get; private set; }

		public TournamentArchiveUI(Mobile user, TournamentBattle battle, Gump parent = null)
			: this(user, (IEnumerable<TournamentMatch>)battle, parent)
		{ }

		public TournamentArchiveUI(Mobile user, TournamentArchive archive, Gump parent = null)
			: this(user, (IEnumerable<TournamentMatch>)archive, parent)
		{ }

		private TournamentArchiveUI(Mobile user, IEnumerable<TournamentMatch> archive, Gump parent = null)
			: base(user, parent)
		{
			Archive = archive;

			Title = "Tournament Records";

			if (Archive is TournamentArchive)
			{
				var a = (TournamentArchive)Archive;

				Title = String.Format("{0} - {1}", a.Name, a.Date.ToSimpleString("t@h:m:s@ D d M y"));
			}
			else if (Archive is TournamentBattle)
			{
				var b = (TournamentBattle)Archive;

				Title = b.Name;

				if (b.State == PvPBattleState.Running)
				{
					AutoRefresh = true;
					AutoRefreshRate = TimeSpan.FromSeconds(30.0);
				}
			}

			CanClose = true;
			CanDispose = true;
			CanMove = true;
			CanResize = true;

			EntriesPerPage = 5;
			EntryWidth = 400;
			EntryHeight = 120;

			Init();
		}

		protected override void Compile()
		{
			EntryWidth = Math.Max(400, EntryWidth);
			EntryHeight = Math.Max(120, EntryHeight);

			base.Compile();
		}

		protected virtual void Init()
		{
			if (Archive != null)
			{
				Leader = List.Where(m => m.Winner != null)
							 .GroupBy(m => m.Winner)
							 .OrderByDescending(o => o.Count())
							 .ThenByDescending(o => o.Aggregate(0.0, (c, m) => c + m.ComputeScore(o.Key)))
							 .Select(o => o.Key)
							 .FirstOrDefault();
			}

			var pad = SupportsUltimaStore ? 10 : 15;
			var count = Math.Max(1, Math.Min(EntryCount, EntriesPerPage));

			Width = 30 + (pad * 3) + EntryWidth;
			Height = 90 + (pad * 3) + (count * EntryHeight);
		}

		protected override bool OnBeforeSend()
		{
			return base.OnBeforeSend() && Archive != null;
		}

		protected override void CompileList(List<TournamentMatch> list)
		{
			list.Clear();

			if (Archive != null)
			{
				list.AddRange(Archive.Not(o => o.IsEmpty));
			}

			base.CompileList(list);

			Init();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var pad = sup ? 10 : 15;
			var bgID = sup ? 40000 : 9270;
			var bgCol = sup ? Color.FromArgb(0xFF, 0x29, 0x31, 0x39) : Color.Black;

			layout.Add(
				"bg",
				() =>
				{
					var x = 0;
					var y = 0;
					var w = Width;
					var h = Height;

					AddBackground(x, y, w, h, bgID);

					x = pad;
					y = (Height - pad) - 60;
					w = Width - (pad * 2);
					h = 60;

					AddRectangle(x, y, w, h, bgCol, true);
				});

			layout.Add(
				"title",
				() =>
				{
					var title = Title;

					title = title.WrapUOHtmlBig();
					title = title.WrapUOHtmlCenter();
					title = title.WrapUOHtmlColor(Color.Gold, false);

					AddHtml(pad, pad, Width - (pad * 2), 40, title, false, false);
				});

			layout.Add("results", CompileResults);

			layout.Add(
				"scroll",
				() =>
				{
					var x = Width - (pad + 30);
					var y = pad + 30;
					var h = Height - ((pad * 3) + 90);

					AddScrollbarV(x, y, h, PageCount, Page, PreviousPage, NextPage);
				});

			layout.Add("cpanel", CompileControls);
		}

		private void CompileControls()
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 10 : 15;
			//var bgCol = sup ? Color.FromArgb(0xFF, 0x29, 0x31, 0x39) : Color.Black;

			var x = pad;
			var y = Height - (pad + 60);
			var w = (Width - (pad * 2)) / 4;

			//////////

			var players = List.SelectMany(o => o.Players.Where(p => p != null)).Distinct().Count();

			var text = String.Format("PLAYERS:\n{0:#,0}", players);

			text = text.WrapUOHtmlBold();
			text = text.WrapUOHtmlCenter();
			text = text.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddRectangle(x, y, w, 60, Color.White);
			AddHtml(x + pad, y + pad, w - (pad * 2), 60, text, false, false);

			x += w;

			//////////

			text = String.Format("MATCHES:\n{0:#,0}", EntryCount);

			text = text.WrapUOHtmlBold();
			text = text.WrapUOHtmlCenter();
			text = text.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddRectangle(x, y, w, 60, Color.White);
			AddHtml(x + pad, y + pad, w - (pad * 2), 60, text, false, false);

			x += w;

			//////////

			var ticks = List.Select(o => o.IsRunning ? o.Expire : o.TotalTime).Aggregate(0L, (v, o) => v + o.Ticks);

			var duration = TimeSpan.FromTicks(ticks).ToSimpleString(@"!<d\d ><h\h >m\m");

			text = String.Format("DURATION:\n{0}", duration);

			text = text.WrapUOHtmlBold();
			text = text.WrapUOHtmlCenter();
			text = text.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddRectangle(x, y, w, 60, Color.White);
			AddHtml(x + pad, y + pad, w - (pad * 2), 60, text, false, false);

			x += w;

			//////////

			if (Archive is TournamentArchive || (Archive is TournamentBattle && !((TournamentBattle)Archive).IsRunning))
			{
				text = String.Format("WINNER:\n{0}", Leader != null ? Leader.RawName : "N/A");
			}
			else
			{
				text = String.Format("LEADER:\n{0}", Leader != null ? Leader.RawName : "N/A");
			}

			text = text.WrapUOHtmlBold();
			text = text.WrapUOHtmlCenter();
			text = text.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddRectangle(x, y, w, 60, Color.White);
			AddHtml(x + pad, y + pad, w - (pad * 2), 60, text, false, false);
		}

		private void CompileResults()
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 10 : 15;

			var x = pad;
			var y = pad + 30;

			var range = GetListRange();

			foreach (var o in range.Values)
			{
				CompileMatchLayout(x, y, EntryWidth, EntryHeight, o);

				y += EntryHeight;
			}

			range.Clear();
		}

		private void CompileMatchLayout(int x, int y, int w, int h, TournamentMatch match)
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 10 : 15;
			var bgID = sup ? 40000 : 9270;
			//var bgCol = sup ? Color.FromArgb(0xFF, 0x29, 0x31, 0x39) : Color.Black;

			AddTileButton(x, y, w, h, b => Select(match));

			AddBackground(x, y, w, h, bgID);

			var capacity = match.Capacity;

			if (capacity <= 0)
			{
				return;
			}

			var xx = x + pad;
			var yy = y + pad;
			var ww = (w - (pad * 2)) / capacity;
			//var hh = h - (pad * 2);

			string t;
			Color c;
			Size s;
			Point o;
			int ox, oy, tx, ty;

			for (var i = 0; i < capacity; i++)
			{
				var p = match.Players[i];

				if (p != null)
				{
					t = p.RawName;
				}
				else
				{
					t = "???";
				}

				t = t.WrapUOHtmlBig();
				t = t.WrapUOHtmlCenter();

				if (p != null)
				{
					t = t.WrapUOHtmlColor(User.GetNotorietyColor(p), false);
				}
				else
				{
					t = t.WrapUOHtmlColor(Color.White, false);
				}

				AddHtml(xx, yy, ww, 40, t, false, false);

				if (p != null)
				{
					s = GetImageSize(7034);
					o = new Point(ComputeCenter(xx, ww - s.Width), yy + 30);

					AddImage(o.X, o.Y, 7034);

					c = match.Winner == null ? Color.Silver : match.Winner == p ? Color.LawnGreen : Color.IndianRed;

					AddRectangle(o.X, o.Y, s.Width, s.Height, c, 2);
				}
				else
				{
					s = GetImageSize(7034);
					o = new Point(ComputeCenter(xx, ww - s.Width), yy + 30);

					AddImage(o.X, o.Y, 7034);

					c = Color.White;

					AddRectangle(o.X, o.Y, s.Width, s.Height, c, 2);
				}

				if (i + 1 < capacity)
				{
					ox = o.X + s.Width;
					tx = xx + (ww - 30);
				}
				else
				{
					ox = xx + 30;
					tx = o.X;
				}

				oy = ComputeCenter(o.Y, s.Height - 4);
				ty = oy + 4;

				AddRectangle(ox, oy, tx - ox, ty - oy, c, true);

				if (i + 1 < capacity)
				{
					AddImageShadow(xx + (ww - 30), yy + 30, 5578, 0, 90);
				}

				xx += ww;
			}
		}

		public void GetHtml(StringBuilder html, TournamentMatch match, bool extended)
		{
			var text = String.Format("Match #{0}", match.Index + 1);

			text = text.WrapUOHtmlBig();
			text = text.WrapUOHtmlColor(Color.Gold, false);

			html.AppendLine(text);

			var sep = " vs ".WrapUOHtmlColor(Color.IndianRed, Color.White);

			text = String.Join(sep, match.Players.Select(o => o != null ? o.RawName : "???"));
			text = text.WrapUOHtmlColor(Color.White, false);

			html.AppendLine(text);

			if (extended)
			{
				html.AppendLine();
			}

			if (match.Winner != null)
			{
				text = String.Format("Winner: {0}", match.Winner.RawName);
				text = text.WrapUOHtmlColor(Color.PaleGoldenrod, false);
			}
			else if (match.IsComplete)
			{
				text = "Draw";
				text = text.WrapUOHtmlColor(Color.LightGray, false);
			}
			else
			{
				var exp = match.Expire;

				if (exp > TimeSpan.Zero)
				{
					text = String.Format("Expire: {0}", exp.ToSimpleString("h:m:s"));
					text = text.WrapUOHtmlColor(Color.SkyBlue, false);
				}
				else
				{
					text = "Expired";
					text = text.WrapUOHtmlColor(Color.IndianRed, false);
				}
			}

			html.AppendLine(text);

			if (!extended)
			{
				return;
			}

			html.AppendLine();

			if (match.DateStart > DateTime.MinValue)
			{
				text = String.Format("Start: {0}", match.DateStart.ToSimpleString("t@h:m:s@ D d M y"));
				text = text.WrapUOHtmlColor(Color.SkyBlue, false);

				html.AppendLine(text);
			}

			if (match.DateEnd < DateTime.MaxValue)
			{
				text = String.Format("Finish: {0}", match.DateEnd.ToSimpleString("t@h:m:s@ D d M y"));
				text = text.WrapUOHtmlColor(Color.SkyBlue, false);

				html.AppendLine(text);
			}

			html.AppendLine();

			text = "Records";
			text = text.WrapUOHtmlBig();
			text = text.WrapUOHtmlColor(Color.Gold, false);

			html.AppendLine(text);

			html.AppendLine(String.Empty.WrapUOHtmlColor(Color.White, false));

			foreach (var o in match.Records)
			{
				html.AppendLine("[{0}]: {1}", o.Time.ToSimpleString("t@h:m:s@"), o.Value);
			}
		}

		private void Select(TournamentMatch match)
		{
			var html = new StringBuilder();

			GetHtml(html, match, true);

			new NoticeDialogGump(User, Refresh(true))
			{
				Width = 600,
				Height = 400,
				Title = String.Format("Match #{0:#,0}", match.Index + 1),
				Html = html.ToString()
			}.Send();
		}

		public override int SortCompare(TournamentMatch a, TournamentMatch b)
		{
			var res = 0;

			if (a.CompareNull(b, ref res))
			{
				return res;
			}

			if (a.Index < b.Index)
			{
				return -1;
			}

			if (a.Index > b.Index)
			{
				return 1;
			}

			return base.SortCompare(a, b);
		}

		protected override void OnAutoRefresh()
		{
			base.OnAutoRefresh();

			AutoRefresh = Archive is TournamentBattle && ((TournamentBattle)Archive).IsRunning;
		}
	}
}