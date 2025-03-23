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
using System.Text.RegularExpressions;

using Server;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;

using VitaNex.Collections;
using VitaNex.Schedules;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattlesUI : TreeGump
	{
		private const string _Info = //
			"PvP Battles allow you to engage in combat with other players in different scenarios." +
			"\nNo deaths, no insurance loss and no looting (unless specified)!" +
			"\nEach battle may offer a reward for both winners and losers!" +
			"\nJoin a battle now and earn your rank amongst the top players of {0}!";

		private static readonly string _States = //
			"Queueing: The battle queue is open.".WrapUOHtmlColor(Color.RoyalBlue) +
			"\nPreparing: Allows time to prepare for the battle.".WrapUOHtmlColor(Color.OrangeRed) +
			"\nRunning: The battle has started!".WrapUOHtmlColor(Color.MediumSeaGreen) +
			"\nEnded: A delay before the battle opens again.".WrapUOHtmlColor(Color.IndianRed);

		private static readonly List<PvPBattlesUI> _Refreshing = new List<PvPBattlesUI>();

		public static Color GetStateColor(PvPBattleState state)
		{
			switch (state)
			{
				case PvPBattleState.Queueing:
					return Color.RoyalBlue;
				case PvPBattleState.Preparing:
					return Color.OrangeRed;
				case PvPBattleState.Running:
					return Color.MediumSeaGreen;
				case PvPBattleState.Ended:
					return Color.IndianRed;
				default:
					return Color.IndianRed;
			}
		}

		public static string GetNodePath(PvPBattle o)
		{
			var useCat = AutoPvP.CMOptions.Advanced.Misc.UseCategories;
			var fmtCat = "Battles|" + (useCat ? "{0}|{1}" : "{1}");

			var cat = GetCategory(o);

			var oidx = AutoPvP.Battles.Values.IndexOf(o);
			var bidx = 0;

			foreach (var b in AutoPvP.Battles.Values.Skip(oidx + 1))
			{
				if (b != o && GetCategory(b) == cat && b.Name == o.Name)
				{
					++bidx;
				}
			}

			var name = o.Name + (bidx > 0 ? (" " + (Numeral)bidx) : String.Empty);

			return String.Format(fmtCat, GetCategory(o), name);
		}

		public static string GetCategory(PvPBattle o)
		{
			return String.IsNullOrWhiteSpace(o.Category) ? "Misc" : o.Category;
		}

		private static PvPBattlesUI GetInstance(Mobile user, bool create)
		{
			var ui = GetInstance<PvPBattlesUI>(user, true);

			if ((ui == null || ui.IsDisposed) && create)
			{
				ui = new PvPBattlesUI(user);
			}

			return ui;
		}

		public static void RefreshAll(PvPBattle battle)
		{
			_Refreshing.AddRange(GlobalInstances.Values.OfType<PvPBattlesUI>());

			foreach (var ui in _Refreshing)
			{
				if (!ui.IsDisposed && ui.IsOpen && ui.Battle == battle)
				{
					ui.Refresh(true);
				}
			}

			_Refreshing.Clear();
		}

		public static void Refresh(Mobile user, PvPBattle battle)
		{
			var ui = GetInstance(user, false);

			if (ui != null && !ui.IsDisposed && ui.IsOpen && ui.Battle == battle)
			{
				ui.Refresh(true);
			}
		}

		public static void DisplayTo(Mobile user, PvPBattle battle, bool refreshOnly)
		{
			var ui = GetInstance(user, !refreshOnly);

			if (ui != null)
			{
				if (battle != null)
				{
					ui.Battle = battle;

					ui.SelectedNode = GetNodePath(battle);
				}

				ui.Refresh(true);
			}
		}

		private readonly List<string> _Categories = new List<string>();

		private readonly List<PvPSeason> _Seasons = new List<PvPSeason>();
		private readonly List<PvPBattle> _Battles = new List<PvPBattle>();

		private string _RankSearch;

		private int _RankPage, _RankPages;
		private int _TeamPage, _TeamPages;
		private int _BattlePage, _BattlePages;

		public string Category { get; protected set; }

		public PvPSeason Season { get; protected set; }
		public PvPBattle Battle { get; protected set; }
		public PvPTeam Team { get; protected set; }
		public PvPProfile Profile { get; protected set; }

		public bool EditBattle { get; protected set; }
		public bool EditProfile { get; protected set; }

		protected virtual bool CanEdit => (Battle == null || !Battle.Deleted) && (Profile == null || !Profile.Deleted) && User.AccessLevel >= AutoPvP.Access;

		public PvPBattlesUI(Mobile user)
			: base(user)
		{
			Title = "Battlegrounds";

			Width = 900;
			Height = 500;

			AutoRefreshRate = TimeSpan.FromSeconds(15.0);
			AutoRefresh = true;
		}

		protected override bool CanAutoRefresh()
		{
			return Battle != null && base.CanAutoRefresh();
		}

		protected override void OnSelected(TreeGumpNode oldNode, TreeGumpNode newNode)
		{
			base.OnSelected(oldNode, newNode);

			if (oldNode.IsChildOf("Seasons") && !newNode.IsChildOf("Seasons"))
			{
				Season = null;
			}
			else if (oldNode.IsChildOf("Battles") && !newNode.IsChildOf("Battles"))
			{
				Battle = null;
			}
			else if (oldNode.IsChildOf("Profiles") && !newNode.IsChildOf("Profiles"))
			{
				Profile = null;
			}
		}

		protected override void CompileNodes(Dictionary<TreeGumpNode, Action<Rectangle, int, TreeGumpNode>> list)
		{
			list.Clear();

			var useCat = AutoPvP.CMOptions.Advanced.Misc.UseCategories;

			var seasons = AutoPvP.Seasons.Values.AsEnumerable();
			var battles = AutoPvP.Battles.Values.AsEnumerable();

			if (!CanEdit)
			{
				if (AutoPvP.SeasonSchedule.Enabled)
				{
					seasons = seasons.Where(o => o.Started != null);
				}
				else
				{
					seasons = Enumerable.Empty<PvPSeason>();
				}

				battles = battles.Where(o => !o.IsInternal && !o.Hidden);
			}

			_Seasons.Clear();
			_Seasons.AddRange(seasons);

			if (_Seasons.Count > 0)
			{
				list["Seasons"] = (b, i, n) => CompileSeasonsLayout(b);

				foreach (var o in _Seasons)
				{
					var season = o;

					list["Seasons|Season " + season.Number] = (b, i, n) => CompileSeasonLayout(b, season);
				}
			}

			_Battles.Clear();
			_Battles.AddRange(battles);

			_Categories.Clear();

			if (_Battles.Count > 0)
			{
				list["Battles"] = (b, i, n) => CompileBattlesLayout(b, null);

				foreach (var bo in _Battles)
				{
					if (useCat)
					{
						var cat = GetCategory(bo);

						_Categories.Update(cat);

						list["Battles|" + cat] = (b, i, n) => CompileBattlesLayout(b, cat);
					}

					var battle = bo;

					var key = GetNodePath(battle);

					list[key] = (b, i, n) => CompileBattleLayout(b, battle);

					foreach (var tg in battle.Teams.ToLookup(to => key + "|" + to.Name))
					{
						var tidx = 0;

						foreach (var to in tg)
						{
							var team = to;

							var tkey = tg.Key + (tidx > 0 ? (" " + (Numeral)tidx) : String.Empty);

							list[tkey] = (b, i, n) => CompileTeamLayout(b, battle, team);

							++tidx;
						}
					}
				}
			}

			list["Profiles"] = (b, i, n) => CompileTopRankedLayout(b);

			var user = User as PlayerMobile;

			if (user != null)
			{
				var pro = AutoPvP.EnsureProfile(user);

				if (pro != null)
				{
					list["Profiles|My Profile"] = (b, i, n) => CompileProfileLayout(b, pro);
				}
			}

			base.CompileNodes(list);
		}

		protected override void CompileNodeLayout(
			SuperGumpLayout layout,
			int x,
			int y,
			int w,
			int h,
			int index,
			TreeGumpNode node)
		{
			x += 5;
			y += 5;
			w -= 10;
			h -= 10;

			base.CompileNodeLayout(layout, x, y, w, h, index, node);
		}

		#region Rank
		private int ResolveProfiles(Dictionary<PvPProfile, int> pool)
		{
			var edit = CanEdit;
			var search = !String.IsNullOrWhiteSpace(_RankSearch);

			int index = -1, count = 0;

			var order = AutoPvP.CMOptions.Advanced.Profiles.RankingOrder;

			var profiles = AutoPvP.Profiles.Values.AsEnumerable();

			if (!edit)
			{
				profiles = profiles.Where(p => AutoPvP.GetSortedValue(order, p) > 0);
			}

			foreach (var p in AutoPvP.GetSortedProfiles(order, profiles))
			{
				int idx;

				if (p.Owner.AccessLevel > AccessLevel.Player)
				{
					if (!edit)
					{
						continue;
					}

					idx = -1;
				}
				else
				{
					idx = ++index;
				}

				if (!search || Regex.IsMatch(p.Owner.RawName, _RankSearch))
				{
					pool[p] = idx;
				}

				++count;
			}

			return count;
		}

		protected virtual void CompileTopRankedLayout(Rectangle b)
		{
			string text;

			var bgcol = Color.DarkSlateGray;

			var profiles = DictionaryPool<PvPProfile, int>.AcquireObject();

			var count = ResolveProfiles(profiles);

			var limit = ((b.Height - 30) / 25) - 1;

			_RankPages = (int)Math.Ceiling(count / (double)limit);
			_RankPage = Math.Max(0, Math.Min(_RankPages, _RankPage));

			var cols = new[] { 50, 150, -1, -1, -1, -1 };
			var rows = profiles.Skip(_RankPage * limit).Take(limit);

			AddTable(b.X, b.Y, b.Width, b.Height, true, cols, rows, 25, Color.Empty, 0, RenderTopRankTable);

			ObjectPool.Free(ref profiles);

			b.Y += b.Height - 25;
			b.Height -= b.Height - 25;

			var bw = (b.Width - 4) / 4;

			if (_RankPage - 1 >= 0)
			{
				text = UniGlyph.TriLeftFill.ToString().WrapUOHtmlCenter();

				var page = _RankPage - 1;

				AddHtmlButton(b.X, b.Y, bw, 25, o => OnRankPage(page), text, Color.SkyBlue, bgcol);
				AddTooltip(3000405); // Previous
			}
			else
			{
				text = UniGlyph.TriLeftEmpty.ToString().WrapUOHtmlCenter();

				AddRectangle(b.X, b.Y, bw, 25, bgcol, true);
				AddHtml(b.X, b.Y + 2, bw, 20, text, Color.SkyBlue, bgcol);
				AddTooltip(3000405); // Previous
			}

			b.X += bw + 2;
			b.Width -= bw + 2;

			AddRectangle(b.X, b.Y, bw * 2, 25, bgcol, 1);

			if (String.IsNullOrWhiteSpace(_RankSearch))
			{
				AddTextEntryLimited(b.X + 5, b.Y + 2, (bw * 2) - 30, 20, TextHue, String.Empty, 30, (t, s) => _RankSearch = s);

				text = UniGlyph.CircleDot.ToString().WrapUOHtmlCenter();

				AddHtmlButton(b.X + ((bw * 2) - 25), b.Y, 25, 25, o => OnRankSearch(_RankSearch), text, Color.SkyBlue, bgcol);
				AddTooltip(1154641); // Search
			}
			else
			{
				AddLabelCropped(b.X + 5, b.Y + 2, (bw * 2) - 30, 20, TextHue, _RankSearch);

				text = UniGlyph.CircleX.ToString().WrapUOHtmlCenter();

				AddHtmlButton(b.X + ((bw * 2) - 25), b.Y, 25, 25, o => OnRankSearch(null), text, Color.SkyBlue, bgcol);
				AddTooltip(3000413); // Clear
			}

			b.X += (bw * 2) + 2;
			b.Width -= (bw * 2) + 2;

			if (_RankPage + 1 < _RankPages)
			{
				text = UniGlyph.TriRightFill.ToString().WrapUOHtmlCenter();

				var page = _RankPage + 1;

				AddHtmlButton(b.X, b.Y, bw, 25, o => OnRankPage(page), text, Color.SkyBlue, bgcol);
				AddTooltip(3000406); // Next
			}
			else
			{
				text = UniGlyph.TriRightEmpty.ToString().WrapUOHtmlCenter();

				AddRectangle(b.X, b.Y, bw, 25, bgcol, true);
				AddHtml(b.X, b.Y + 2, bw, 20, text, Color.SkyBlue, bgcol);
				AddTooltip(3000406); // Next
			}
		}

		protected virtual void OnRankSearch(string search)
		{
			if (String.IsNullOrWhiteSpace(search))
			{
				_RankSearch = String.Empty;
			}

			Refresh(true);
		}

		protected virtual void OnRankPage(int page)
		{
			_RankPage = Math.Max(0, Math.Min(_RankPages - 1, page));

			Refresh(true);
		}

		protected virtual void RenderTopRankTable(
			int x,
			int y,
			int w,
			int h,
			KeyValuePair<PvPProfile, int> entry,
			int row,
			int col)
		{
			var profile = entry.Key;
			var rank = entry.Value;

			var bgcol = row >= 0 && (row + 1) % 2 != 0 ? Color.DarkSlateGray : Color.Empty;

			if (!bgcol.IsEmpty)
			{
				AddRectangle(x, y, w, h, bgcol, true);
			}

			x += 4;
			y += 2;
			w -= 8;
			h -= 4;

			if (row < 0) // headers
			{
				switch (col)
				{
					case 0:
					{
						AddHtml(x, y, w, h, UniGlyph.StarFill.ToString(), Color.Gold, Color.Empty);
						AddTooltip(3000563); // Rank
					}
					break;
					case 1:
					{
						AddHtml(x, y, w, h, "Name", HtmlColor, Color.Empty);
						AddTooltip(3000547); // Player
					}
					break;
					case 2:
					{
						AddHtml(x, y, w, h, "Points", HtmlColor, Color.Empty);
						AddTooltip(1078503); // Score
					}
					break;
					case 3:
						AddHtml(x, y, w, h, "Kills", HtmlColor, Color.Empty);
						break;
					case 4:
						AddHtml(x, y, w, h, "Wins", HtmlColor, Color.Empty);
						break;
					case 5:
						AddHtml(x, y, w, h, "Losses", HtmlColor, Color.Empty);
						break;
				}
			}
			else
			{
				switch (col)
				{
					case 0:
					{
						var label = UniGlyph.HashTag + " " + (rank < 0 ? "---" : (rank + 1).ToString("#,0"));
						var color = HtmlColor;

						switch (rank)
						{
							case 0:
								color = Color.Gold;
								break;
							case 1:
								color = Color.Silver;
								break;
							case 2:
								color = Color.SandyBrown;
								break;
						}

						AddHtmlButton(x, y, w, h, o => OnViewProfile(profile), label, color, bgcol);
						AddTooltip(3000563); // Rank
					}
					break;
					case 1:
					{
						var label = User.GetNotorietyColor(profile.Owner);

						AddHtmlButton(x, y, w, h, o => OnViewProfile(profile), profile.Owner.Name, label, bgcol);
						AddTooltip(3000547); // Player
					}
					break;
					case 2:
					{
						AddHtml(x, y, w, h, profile.TotalPoints.ToString("#,0"), HtmlColor, Color.Empty);
						AddTooltip(1078503); // Score
					}
					break;
					case 3:
						AddHtml(x, y, w, h, profile.TotalKills.ToString("#,0"), HtmlColor, Color.Empty);
						break;
					case 4:
						AddHtml(x, y, w, h, profile.TotalWins.ToString("#,0"), HtmlColor, Color.Empty);
						break;
					case 5:
						AddHtml(x, y, w, h, profile.TotalLosses.ToString("#,0"), HtmlColor, Color.Empty);
						break;
				}
			}
		}

		protected virtual void OnViewProfile(PvPProfile profile)
		{
			Profile = profile;

			new PvPProfileUI(User, profile, Hide(true)).Send();
		}
		#endregion

		#region Profile
		protected virtual void CompileProfileLayout(Rectangle b, PvPProfile profile)
		{
			if (profile == null || profile.Deleted)
			{
				return;
			}

			var ec = IsEnhancedClient;

			var accent = User.GetNotorietyColor(profile.Owner);

			string text;

			text = profile.Owner.RawName;
			text = text.WrapUOHtmlBig();
			text = text.WrapUOHtmlCenter();

			AddHtml(b.X, b.Y, b.Width, 20, text, accent, Color.Empty);

			b.Y += 25;
			b.Height -= 25;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, accent, true);
			}

			++b.Y;
			--b.Height;

			var ww = b.Width / 4;
			var hh = b.Height / 2;

			text = profile.ToHtmlString(User, false);
			text = text.WrapUOHtmlColor(HtmlColor, false);

			AddHtml(b.X, b.Y, b.Width, hh, text, false, true);

			b.Y += hh;
			b.Height -= hh;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
				AddImageTiled(b.X + ww, b.Y, 1, b.Height, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, accent, true);
				AddRectangle(b.X + ww, b.Y, 1, b.Height, accent, true);
			}

			b.Y += 6;
			b.Height -= 6;

			//var ob = b;
		}
		#endregion

		#region Season
		protected virtual void CompileSeasonsLayout(Rectangle b)
		{ }

		protected virtual void CompileSeasonLayout(Rectangle b, PvPSeason season)
		{
			Season = season;

			var ec = IsEnhancedClient;

			string text;

			text = "Season " + (Numeral)season.Number;
			text = text.WrapUOHtmlBig();
			text = text.WrapUOHtmlCenter();

			AddHtml(b.X, b.Y, b.Width, 20, text, Color.Gold, Color.Empty);

			b.Y += 25;
			b.Height -= 25;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, Color.PaleGoldenrod, true);
			}

			++b.Y;
			--b.Height;

			text = season.ToHtmlString(User, false);
			text = text.WrapUOHtmlColor(HtmlColor, false);

			AddHtml(b.X, b.Y, b.Width, b.Height - 26, text, false, true);

			b.Y += b.Height - 26;
			b.Height -= b.Height - 26;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, Color.PaleGoldenrod, true);
			}

			++b.Y;
			--b.Height;

			b.Y += 5;
			b.Height -= 5;

			AddRectangle(b, Color.SkyBlue, true);
		}
		#endregion

		#region Battle
		protected virtual void CompileBattlesLayout(Rectangle b, string cat)
		{
			if (cat == null || !AutoPvP.CMOptions.Advanced.Misc.UseCategories)
			{
				Category = null;

				RenderBattlesLayout(b.X, b.Y, b.Width, b.Height, _Battles.Order());
			}
			else
			{
				if (Category != cat)
				{
					_BattlePage = 0;
				}

				Category = cat;

				var battles = _Battles.Where(o => GetCategory(o) == cat);

				RenderBattlesLayout(b.X, b.Y, b.Width, b.Height, battles.Order());
			}
		}

		protected virtual void RenderBattlesLayout(int x, int y, int w, int h, IEnumerable<PvPBattle> list)
		{
			var bgcol = Color.DarkSlateGray;

			var battles = ListPool<PvPBattle>.AcquireObject();

			battles.AddRange(list);

			var limit = (h - 30) / 25;

			_BattlePages = (int)Math.Ceiling(battles.Count / (double)limit);
			_BattlePage = Math.Max(0, Math.Min(_BattlePages, _BattlePage));

			var cols = Enumerable.Repeat(-1, 3);

			var rows = battles.Order().AsEnumerable();

			rows = rows.Skip(_BattlePage * limit).Take(limit);

			AddTable(x, y, w, h - 30, false, cols, rows, 25, Color.Empty, 0, RenderBattlesTable);

			ObjectPool.Free(ref battles);

			y += h - 25;
			h -= h - 25;

			var bw = (w - 2) / 2;

			if (_BattlePage - 1 >= 0)
			{
				var g = UniGlyph.TriLeftFill.ToString().WrapUOHtmlCenter();

				var page = _BattlePage - 1;

				AddHtmlButton(x, y, bw, 25, o => OnBattlesPage(page), g, Color.SkyBlue, bgcol);
				AddTooltip(3000405); // Previous
			}
			else
			{
				var g = UniGlyph.TriLeftEmpty.ToString().WrapUOHtmlCenter();

				AddRectangle(x, y, bw, 25, bgcol, true);
				AddHtml(x, y + 2, bw, 20, g, Color.SkyBlue, bgcol);
				AddTooltip(3000405); // Previous
			}

			x += bw + 2;
			w -= bw + 2;

			if (_BattlePage + 1 < _BattlePages)
			{
				var g = UniGlyph.TriRightFill.ToString().WrapUOHtmlCenter();

				var page = _BattlePage + 1;

				AddHtmlButton(x, y, bw, 20, o => OnBattlesPage(page), g, Color.SkyBlue, bgcol);
				AddTooltip(3000406); // Next
			}
			else
			{
				var g = UniGlyph.TriRightEmpty.ToString().WrapUOHtmlCenter();

				AddRectangle(x, y, bw, 25, bgcol, true);
				AddHtml(x, y + 2, bw, 20, g, Color.SkyBlue, bgcol);
				AddTooltip(3000406); // Next
			}
		}

		protected virtual void OnBattlesPage(int page)
		{
			_BattlePage = Math.Max(0, Math.Min(_BattlePages - 1, page));
		}

		protected virtual void RenderBattlesTable(int x, int y, int w, int h, PvPBattle battle, int row, int col)
		{
			var bgcol = Color.DarkSlateGray;
			var accent = GetStateColor(battle.State);

			string text;

			switch (col)
			{
				case 0: // Name
				{
					AddColoredButton(x, y, w, h, bgcol, b => OnViewBattle(battle));

					text = battle.Ranked ? "Ranked" : "Unranked";
					text = text.WrapUOHtmlRight();

					AddHtml(x + 5, y, w - 10, h, battle.Name, Color.White, Color.Empty);
					AddHtml(x + 5, y, w - 10, h, text, Color.White, Color.Empty);
				}
				break;
				case 1: // Capacity
				{
					double cur = battle.CurrentCapacity, min = battle.MinCapacity, max = battle.MaxCapacity;

					if (!battle.RequireCapacity)
					{
						AddRectangle(x, y, w, h, bgcol, true);

						text = cur.ToString("#,0");
						text = text.WrapUOHtmlCenter();

						AddHtml(x + 5, y, w - 10, h, text, Color.White, Color.Empty);
						AddTooltip(1018090); // Players
					}
					else if (max > 0)
					{
						var fgcol = Color.Orange.Interpolate(Color.Green, cur / max);

						if (min > 0 && min < max && cur < min)
						{
							AddProgress(x, y, w, h, cur / min, Direction.Right, bgcol, fgcol, Color.Empty);

							text = String.Format("{0:#,0} / {1:#,0} +{2:#,0}", cur, min, max - min);
							text = text.WrapUOHtmlCenter();
						}
						else
						{
							AddProgress(x, y, w, h, cur / max, Direction.Right, bgcol, fgcol, Color.Empty);

							text = String.Format("{0:#,0} / {1:#,0}", cur, max);
							text = text.WrapUOHtmlCenter();
						}

						AddHtml(x + 5, y, w - 10, h, text, Color.White, Color.Empty);
						AddTooltip(1018090); // Players
					}
				}
				break;
				case 2: // Status
				{
					AddRectangle(x, y, w, h, accent, true);

					var ts = battle.GetStateTimeLeft(battle.State);

					if (ts < TimeSpan.Zero)
					{
						ts = TimeSpan.Zero;
					}

					text = ts.ToSimpleString(@"!<d\d h\h m\m s\s>");
					text = text.WrapUOHtmlRight();

					AddHtml(x + 5, y, w - 10, h, battle.State.ToString(true), Color.White, Color.Empty);
					AddHtml(x + 5, y, w - 10, h, text, Color.White, Color.Empty);
					AddTooltip(3000159); // Time
				}
				break;
			}
		}

		protected virtual void OnViewBattle(PvPBattle battle)
		{
			Battle = battle;

			SelectNode(GetNodePath(battle));
		}

		protected virtual void CompileBattleLayout(Rectangle b, PvPBattle battle)
		{
			if (battle == null || battle.Deleted)
			{
				return;
			}

			var ec = IsEnhancedClient;

			var accent = GetStateColor(battle.State);

			string text;

			text = battle.Name + " (" + (battle.Ranked ? "Ranked" : "Unranked") + ")";
			text = text.WrapUOHtmlBig();
			text = text.WrapUOHtmlCenter();

			AddHtml(b.X, b.Y, b.Width, 20, text, accent, Color.Empty);

			b.Y += 25;
			b.Height -= 25;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, accent, true);
			}

			++b.Y;
			--b.Height;

			var ww = b.Width / 4;
			var hh = b.Height / 3;

			text = battle.ToHtmlString(User);
			text = text.WrapUOHtmlColor(HtmlColor, false);

			AddHtml(b.X, b.Y, b.Width, hh, text, false, true);

			b.Y += hh;
			b.Height -= hh;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
				AddImageTiled(b.X + ww, b.Y, 1, b.Height, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, accent, true);
				AddRectangle(b.X + ww, b.Y, 1, b.Height, accent, true);
			}

			b.Y += 6;
			b.Height -= 6;

			var ob = b;

			if (!battle.Hidden || CanEdit)
			{
				#region Controls
				if (User is PlayerMobile)
				{
					var user = (PlayerMobile)User;

					if (battle.IsParticipant(user))
					{
						text = UniGlyph.CircleX + " Leave Battle";

						AddHtmlButton(b.X, b.Y, ww, 20, o => OnQuit(battle), text, Color.Yellow, Color.Black);

						b.Y += 20;
						b.Height -= 20;
					}
					else
					{
						if (battle.IsQueued(user))
						{
							text = UniGlyph.CircleX + " Leave Queue";

							AddHtmlButton(b.X, b.Y, ww, 20, o => OnDequeue(battle), text, Color.Yellow, Color.Black);

							b.Y += 20;
							b.Height -= 20;
						}
						else if (battle.CanQueue(user))
						{
							text = UniGlyph.StarFill + " Join Queue";

							AddHtmlButton(b.X, b.Y, ww, 20, o => OnQueue(battle, null), text, Color.LawnGreen, Color.Black);

							b.Y += 20;
							b.Height -= 20;
						}

						if (battle.IsSpectator(user))
						{
							text = UniGlyph.CircleX + " Leave";

							AddHtmlButton(b.X, b.Y, ww, 20, o => OnLeave(battle), text, Color.Yellow, Color.Black);

							b.Y += 20;
							b.Height -= 20;
						}
						else if (battle.CanSpectate(user))
						{
							text = UniGlyph.CircleDot + " Spectate";

							AddHtmlButton(b.X, b.Y, ww, 20, o => OnSpectate(battle), text, Color.Yellow, Color.Black);

							b.Y += 20;
							b.Height -= 20;
						}

						if (!battle.IsInternal)
						{
							var pro = AutoPvP.EnsureProfile(user);

							if (pro != null)
							{
								if (pro.IsSubscribed(battle))
								{
									text = UniGlyph.Coffee + " Unsubscribe";

									AddHtmlButton(b.X, b.Y, ww, 20, o => OnUnsubscribe(battle), text, Color.Yellow, Color.Black);

									b.Y += 20;
									b.Height -= 20;
								}
								else
								{
									text = UniGlyph.Coffee + " Subscribe";

									AddHtmlButton(b.X, b.Y, ww, 20, o => OnSubscribe(battle), text, Color.Yellow, Color.Black);

									b.Y += 20;
									b.Height -= 20;
								}
							}
						}
					}
				}
				#endregion
			}

			if (b.Y > ob.Y && b.Height < ob.Height)
			{
				b.Y += 5;
				b.Height -= 5;

				if (ec)
				{
					AddImageTiled(b.X, b.Y, ww, 1, 2624);
				}
				else
				{
					AddRectangle(b.X, b.Y, ww, 1, accent, true);
				}

				b.Y += 6;
				b.Height -= 6;
			}

			if (CanEdit)
			{
				#region Edit
				if (battle.IsInternal)
				{
					if (battle.Validate(User))
					{
						text = UniGlyph.StarFill + " Publish";

						AddHtmlButton(b.X, b.Y, ww, 20, o => OnPublish(battle), text, Color.LawnGreen, Color.Black);
					}
					else
					{
						text = UniGlyph.StarEmpty + " Publish";

						AddHtml(b.X, b.Y, ww, 20, text, Color.Gray, Color.Black);
					}
				}
				else
				{
					text = UniGlyph.StarEmpty + " Internalize";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnInternalize(battle), text, Color.Yellow, Color.Black);
				}

				b.Y += 20;
				b.Height -= 20;

				if (battle.Hidden)
				{
					text = UniGlyph.CircleLeftFill + " Hidden";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnVisibility(battle, true), text, Color.Yellow, Color.Black);
				}
				else
				{
					text = UniGlyph.CircleRightFill + " Visible";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnVisibility(battle, false), text, Color.Yellow, Color.Black);
				}

				b.Y += 20;
				b.Height -= 20;

				if (!battle.Ranked)
				{
					text = UniGlyph.CircleLeftFill + " Unranked";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnRanked(battle, true), text, Color.Yellow, Color.Black);
				}
				else
				{
					text = UniGlyph.CircleRightFill + " Ranked";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnRanked(battle, false), text, Color.Yellow, Color.Black);
				}

				b.Y += 20;
				b.Height -= 20;

				if (!battle.AutoAssign)
				{
					text = UniGlyph.CircleLeftFill + " Choose Team";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnAutoAssign(battle, true), text, Color.Yellow, Color.Black);
				}
				else
				{
					text = UniGlyph.CircleRightFill + " Random Team";

					AddHtmlButton(b.X, b.Y, ww, 20, o => OnAutoAssign(battle, false), text, Color.Yellow, Color.Black);
				}

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleDot + " Edit";

				AddHtmlButton(b.X, b.Y, ww, 20, o => OnEdit(battle), text, Color.SkyBlue, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleDot + " Settings";

				AddHtmlButton(b.X, b.Y, ww, 20, o => OnOptions(battle, false), text, Color.SkyBlue, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleDot + " Options";

				AddHtmlButton(b.X, b.Y, ww, 20, o => OnOptions(battle, true), text, Color.SkyBlue, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleX + " Delete";

				AddHtmlButton(b.X, b.Y, ww, 20, o => OnDelete(battle, true), text, Color.IndianRed, Color.Black);

				b.Y += 20;
				b.Height -= 20;
				#endregion
			}

			b = ob;
			b.X += ww + 8;
			b.Width -= ww + 8;

			#region Stats
			var bgcol = Color.DarkSlateGray;

			double cur = battle.CurrentCapacity, min = battle.MinCapacity, max = battle.MaxCapacity;

			var ts = battle.GetStateTimeLeft();

			if (ts < TimeSpan.Zero)
			{
				ts = TimeSpan.Zero;
			}

			text = ts.ToSimpleString(@"!<d\d h\h m\m s\s>");
			text = text.WrapUOHtmlRight();

			AddRectangle(b.X, b.Y, b.Width, 20, accent, true);
			AddHtml(b.X + 5, b.Y, b.Width - 10, 20, battle.State.ToString(true), Color.White, Color.Empty);
			AddHtml(b.X + 5, b.Y, b.Width - 10, 20, text, Color.White, Color.Empty);
			AddTooltip(3000159); // Time

			b.Y += 25;
			b.Height -= 25;

			if (!battle.RequireCapacity)
			{
				text = cur.ToString("#,0");
				text = text.WrapUOHtmlRight();

				AddRectangle(b.X, b.Y, b.Width, 20, bgcol, true);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, "Participants", Color.White, Color.Empty);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, text, Color.White, Color.Empty);
				AddTooltip(1018090); // Players

				b.Y += 25;
				b.Height -= 25;
			}
			else if (max > 0)
			{
				var fgcol = Color.Orange.Interpolate(Color.Green, cur / max);

				if (min > 0 && min < max && cur < min)
				{
					AddProgress(b.X, b.Y, b.Width, 20, cur / min, Direction.Right, bgcol, fgcol, Color.Empty);

					text = String.Format("{0:#,0} / {1:#,0} +{2:#,0}", cur, min, max - min);
					text = text.WrapUOHtmlRight();
				}
				else
				{
					AddProgress(b.X, b.Y, b.Width, 20, cur / max, Direction.Right, bgcol, fgcol, Color.Empty);

					text = String.Format("{0:#,0} / {1:#,0}", cur, max);
					text = text.WrapUOHtmlRight();
				}

				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, "Participants", Color.White, Color.Empty);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, text, Color.White, Color.Empty);
				AddTooltip(1018090); // Players

				b.Y += 25;
				b.Height -= 25;
			}

			if (battle.SpectateAllowed || battle.Spectators.Count > 0)
			{
				text = battle.Spectators.Count.ToString();
				text = text.WrapUOHtmlRight();

				AddRectangle(b.X, b.Y, b.Width, 20, bgcol, true);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, "Spectators", Color.White, Color.Empty);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, text, Color.White, Color.Empty);
				AddTooltip(1018090); // Players

				b.Y += 25;
				b.Height -= 25;
			}

			if (battle.QueueAllowed || battle.Queue.Count > 0)
			{
				text = battle.Queue.Count.ToString();
				text = text.WrapUOHtmlRight();

				AddRectangle(b.X, b.Y, b.Width, 20, bgcol, true);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, "Queued", Color.White, Color.Empty);
				AddHtml(b.X + 5, b.Y, b.Width - 10, 20, text, Color.White, Color.Empty);
				AddTooltip(1018090); // Players

				b.Y += 25;
				b.Height -= 25;
			}

			text = battle.Teams.Count.ToString();
			text = text.WrapUOHtmlRight();

			AddRectangle(b.X, b.Y, b.Width, 20, bgcol, true);
			AddHtml(b.X + 5, b.Y, b.Width - 10, 20, "Teams", Color.White, Color.Empty);
			AddHtml(b.X + 5, b.Y, b.Width - 10, 20, text, Color.White, Color.Empty);

			b.Y += 25;
			b.Height -= 25;

			var limit = (b.Height - 30) / 25;

			_TeamPages = (int)Math.Ceiling(battle.Teams.Count / (double)limit);
			_TeamPage = Math.Max(0, Math.Min(_TeamPages, _TeamPage));

			// Name, Members, Score, Join
			IEnumerable<int> cols;

			if (battle.IsInternal)
			{
				cols = Enumerable.Repeat(-1, 2);
			}
			else if (battle.AutoAssign)
			{
				cols = Enumerable.Repeat(-1, 3);
			}
			else
			{
				cols = Enumerable.Repeat(-1, 4);
			}

			var rows = battle.Teams.AsEnumerable();

			if (battle.IsRunning || battle.IsEnded)
			{
				rows = rows.Order();
			}
			else if (!battle.IsInternal)
			{
				rows = rows.OrderByDescending(t => t.Count);
			}

			rows = rows.Skip(_TeamPage * limit).Take(limit);

			AddTable(b.X, b.Y, b.Width, b.Height - 30, false, cols, rows, 25, Color.Empty, 0, RenderTeamsTable);

			b.Y += b.Height - 25;
			b.Height -= b.Height - 25;

			var bw = (b.Width - (CanEdit ? 4 : 2)) / (CanEdit ? 4 : 2);

			if (_TeamPage - 1 >= 0)
			{
				var g = UniGlyph.TriLeftFill.ToString().WrapUOHtmlCenter();

				var page = _TeamPage - 1;

				AddHtmlButton(b.X, b.Y, bw, 25, o => OnTeamPage(page), g, Color.SkyBlue, bgcol);
				AddTooltip(3000405); // Previous
			}
			else
			{
				var g = UniGlyph.TriLeftEmpty.ToString().WrapUOHtmlCenter();

				AddRectangle(b.X, b.Y, bw, 25, bgcol, true);
				AddHtml(b.X, b.Y + 2, bw, 20, g, Color.SkyBlue, bgcol);
				AddTooltip(3000405); // Previous
			}

			b.X += bw + 2;
			b.Width -= bw + 2;

			if (CanEdit)
			{
				text = "Create Team";
				text = text.WrapUOHtmlCenter();

				AddHtmlButton(b.X, b.Y, bw * 2, 25, o => OnTeamCreate(battle, null), text, Color.SkyBlue, bgcol);

				b.X += (bw * 2) + 2;
				b.Width -= (bw * 2) + 2;
			}

			if (_TeamPage + 1 < _TeamPages)
			{
				var g = UniGlyph.TriRightFill.ToString().WrapUOHtmlCenter();

				var page = _TeamPage + 1;

				AddHtmlButton(b.X, b.Y, bw, 20, o => OnTeamPage(page), g, Color.SkyBlue, bgcol);
				AddTooltip(3000406); // Next
			}
			else
			{
				var g = UniGlyph.TriRightEmpty.ToString().WrapUOHtmlCenter();

				AddRectangle(b.X, b.Y, bw, 25, bgcol, true);
				AddHtml(b.X, b.Y + 2, bw, 20, g, Color.SkyBlue, bgcol);
				AddTooltip(3000406); // Next
			}
			#endregion
		}

		protected virtual void OnTeamCreate(PvPBattle battle, string name)
		{
			if (name == null)
			{
				new InputDialogGump(User, Hide(true))
				{
					Title = "Team Name",
					Html = "Enter a name for this team.\n255 Chars Max.",
					InputText = NameList.RandomName("daemon"),
					Limit = 255,
					Callback = (o, n) => OnTeamCreate(battle, n ?? String.Empty),
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				if (String.IsNullOrEmpty(name))
				{
					name = NameList.RandomName("daemon");
				}

				battle.AddTeam(name, 1, 5, ((battle.Teams.Count + 1) * 11) % 110);

				Refresh(true);
			}
		}

		protected virtual void OnTeamPage(int page)
		{
			_TeamPage = Math.Max(0, Math.Min(_TeamPages - 1, page));
		}

		protected virtual void RenderTeamsTable(int x, int y, int w, int h, PvPTeam team, int row, int col)
		{
			int cmax;

			if (team.Battle.IsInternal)
			{
				cmax = 2;
			}
			else if (team.Battle.AutoAssign)
			{
				cmax = 3;
			}
			else
			{
				cmax = 4;
			}

			if (col < cmax)
			{
				w -= 3;
			}

			var bgcol = Color.DarkSlateGray;

			AddRectangle(x, y, w, h, bgcol, true);

			x += 4;
			y += 2;
			w -= 8;
			h -= 4;

			switch (col)
			{
				case 0:
				{
					AddHtmlButton(x, y, w, h, o => OnViewTeam(team), team.Name, Color.White, bgcol);
					AddTooltip(1116493); // Team
				}
				break;
				case 1:
				{
					double cur = team.Count, min = team.MinCapacity, max = team.MaxCapacity;

					var text = String.Empty;

					if (!team.Battle.RequireCapacity)
					{
						text = cur.ToString("#,0");
					}
					else if (max > 0)
					{
						if (min > 0 && min < max && cur < min)
						{
							text = String.Format("{0:#,0} / {1:#,0} +{2:#,0}", cur, min, max - min);
						}
						else
						{
							text = String.Format("{0:#,0} / {1:#,0}", cur, max);
						}
					}

					AddHtml(x, y, w, h, text, Color.White, Color.Empty);
					AddTooltip(1018090); // Players
				}
				break;
				case 2:
				{
					var text = team.GetScore().ToString("#,0");

					AddHtml(x, y, w, h, text, Color.White, Color.Empty);
					AddTooltip(1078503); // Score
				}
				break;
				case 3:
				{
					var text = String.Empty;

					var user = User as PlayerMobile;

					if (user != null)
					{
						if (!team.Battle.Queue.TryGetValue(user, out var t))
						{
							if (team.IsFull)
							{
								text = UniGlyph.StarEmpty + " Full";
							}
							else
							{
								text = UniGlyph.StarFill + " Join";

								AddColoredButton(x, y, w, h, bgcol, o => OnQueue(team.Battle, team));
							}
						}
						else if (team == t)
						{
							text = UniGlyph.CircleX + " Leave";

							AddColoredButton(x, y, w, h, bgcol, o => OnDequeue(team.Battle));
						}
					}

					AddHtml(x, y, w, h, text, Color.White, Color.Empty);
				}
				break;
			}
		}

		protected virtual void OnViewTeam(PvPTeam team)
		{
			Team = team;

			new PvPTeamUI(User, team, Hide(true)).Send();
		}

		#region Edit
		protected virtual void OnPublish(PvPBattle b)
		{
			if (CanEdit)
			{
				b.State = PvPBattleState.Queueing;
			}

			Refresh(true);
		}

		protected virtual void OnInternalize(PvPBattle b)
		{
			if (CanEdit)
			{
				b.State = PvPBattleState.Internal;
			}

			Refresh(true);
		}

		protected virtual void OnVisibility(PvPBattle b, bool value)
		{
			if (CanEdit)
			{
				b.Hidden = !value;
			}

			Refresh(true);
		}

		protected virtual void OnRanked(PvPBattle b, bool value)
		{
			if (CanEdit)
			{
				b.Ranked = value;
			}

			Refresh(true);
		}

		protected virtual void OnAutoAssign(PvPBattle b, bool value)
		{
			if (CanEdit)
			{
				b.AutoAssign = value;
			}

			Refresh(true);
		}

		protected virtual void OnEdit(PvPBattle b)
		{
			if (CanEdit)
			{
				new PvPBattleUI(User, Hide(true), b).Send();
			}
			else
			{
				Refresh(true);
			}
		}

		protected virtual void OnApply(PvPBattle b)
		{
			if (CanEdit)
			{
				EditBattle = false;
			}

			Refresh(true);
		}

		protected virtual void OnOptions(PvPBattle b, bool adv)
		{
			Refresh(true);

			if (CanEdit)
			{
				if (adv)
				{
					User.SendGump(new PropertiesGump(User, b.Options));
				}
				else
				{
					User.SendGump(new PropertiesGump(User, b));
				}
			}
		}

		protected virtual void OnDelete(PvPBattle b, bool confirm)
		{
			if (CanEdit)
			{
				if (confirm)
				{
					new ConfirmDialogGump(User, this)
					{
						Title = "Delete Battle?",
						Html = "All data associated with this battle will be deleted." +
							   "\nThis action can not be reversed!\nDo you want to continue?",
						AcceptHandler = o => OnDelete(b, false)
					}.Send();
				}
				else
				{
					b.Delete();
				}
			}

			Refresh(true);
		}
		#endregion

		#region Controls
		protected virtual void OnQuit(PvPBattle b)
		{
			var user = User as PlayerMobile;

			if (user != null && b.IsParticipant(user))
			{
				b.Quit(user, true);
			}

			Refresh(true);
		}

		protected virtual void OnQueue(PvPBattle b, PvPTeam t)
		{
			var user = User as PlayerMobile;

			if (user != null && b.CanQueue(user))
			{
				b.Enqueue(user, t);
			}

			Refresh(true);
		}

		protected virtual void OnDequeue(PvPBattle b)
		{
			var user = User as PlayerMobile;

			if (user != null && b.IsQueued(user))
			{
				b.Dequeue(user);
			}

			Refresh(true);
		}

		protected virtual void OnSpectate(PvPBattle b)
		{
			var user = User as PlayerMobile;

			if (user != null && b.CanSpectate(user))
			{
				b.AddSpectator(user, true);
			}

			Refresh(true);
		}

		protected virtual void OnLeave(PvPBattle b)
		{
			var user = User as PlayerMobile;

			if (user != null && b.IsSpectator(user))
			{
				b.RemoveSpectator(user, true);
			}

			Refresh(true);
		}

		protected virtual void OnSubscribe(PvPBattle b)
		{
			var user = User as PlayerMobile;

			if (user != null)
			{
				var pro = AutoPvP.EnsureProfile(user);

				if (pro != null)
				{
					pro.Subscribe(b);
				}
			}

			Refresh(true);
		}

		protected virtual void OnUnsubscribe(PvPBattle b)
		{
			var user = User as PlayerMobile;

			if (user != null)
			{
				var pro = AutoPvP.EnsureProfile(user);

				if (pro != null)
				{
					pro.Unsubscribe(b);
				}
			}

			Refresh(true);
		}
		#endregion
		#endregion

		#region Team
		protected virtual void CompileTeamLayout(Rectangle b, PvPBattle battle, PvPTeam team)
		{
			if (battle == null || battle.Deleted || team == null || team.Deleted)
			{
				return;
			}

			var ec = IsEnhancedClient;

			var accent = GetStateColor(battle.State);

			string text;

			text = team.Name;
			text = text.WrapUOHtmlBig();
			text = text.WrapUOHtmlCenter();

			AddHtml(b.X, b.Y, b.Width, 20, text, accent, Color.Empty);

			b.Y += 25;
			b.Height -= 25;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, accent, true);
			}

			++b.Y;
			--b.Height;

			var ww = b.Width / 4;
			var hh = b.Height / 2;

			text = team.ToHtmlString(User, false);
			text = text.WrapUOHtmlColor(HtmlColor, false);

			AddHtml(b.X, b.Y, b.Width, hh, text, false, true);

			b.Y += hh;
			b.Height -= hh;

			if (ec)
			{
				AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
				AddImageTiled(b.X + ww, b.Y, 1, b.Height, 2624);
			}
			else
			{
				AddRectangle(b.X, b.Y, b.Width, 1, accent, true);
				AddRectangle(b.X + ww, b.Y, 1, b.Height, accent, true);
			}

			b.Y += 6;
			b.Height -= 6;

			//var ob = b;
		}
		#endregion

		protected override void CompileEmptyNodeLayout(
			SuperGumpLayout layout,
			int x,
			int y,
			int w,
			int h,
			int index,
			TreeGumpNode node)
		{
			base.CompileEmptyNodeLayout(layout, x, y, w, h, index, node);

			var b = new Rectangle(x + 5, y + 5, w - 10, h - 10);

			layout.Add("node/overview", () => CompileOverviewLayout(b));
		}

		protected virtual void CompileOverviewLayout(Rectangle b)
		{
			var ec = IsEnhancedClient;

			var build = ObjectPool<StringBuilder>.AcquireObject();

			build.Append(String.Empty.WrapUOHtmlColor(DefaultHtmlColor, false));
			build.AppendLine(_Info, ServerList.ServerName);
			build.AppendLine();
			build.AppendLine(_States);
			build.AppendLine();

			var stats = AutoPvP.CMOptions.Statistics;

			if (CanEdit)
			{
				build.AppendLine("{0:#,0} battles total.", stats.Battles);
				build.AppendLine("{0:#,0} battles internalized.", stats.BattlesInternal);
			}
			else
			{
				build.AppendLine("{0:#,0} battles total.", stats.Battles - stats.BattlesInternal);
			}

			build.AppendLine("{0:#,0} battles queueing.", stats.BattlesQueueing);
			build.AppendLine("{0:#,0} battles preparing.", stats.BattlesPreparing);
			build.AppendLine("{0:#,0} battles running.", stats.BattlesRunning);
			build.AppendLine("{0:#,0} battles ended.", stats.BattlesEnded);

			build.AppendLine("{0:#,0} players queueing.", stats.Queueing);
			build.AppendLine("{0:#,0} players fighting.", stats.Participants);
			build.AppendLine("{0:#,0} players watching.", stats.Spectators);

			var text = build.ToString();

			var hh = CanEdit ? b.Height - (b.Height / 3) : b.Height;

			AddHtml(b.X, b.Y, b.Width, hh, text, false, true);

			b.Y += hh;
			b.Height -= hh;

			if (CanEdit)
			{
				var w3 = b.Width / 3;

				if (ec)
				{
					AddImageTiled(b.X, b.Y, b.Width, 1, 2624);
					AddImageTiled(b.X + w3, b.Y, 1, b.Height, 2624);
				}
				else
				{
					AddRectangle(b.X, b.Y, b.Width, 1, Color.PaleGoldenrod, true);
					AddRectangle(b.X + w3, b.Y, 1, b.Height, Color.PaleGoldenrod, true);
				}

				b.Y += 6;
				b.Height -= 6;

				var xx = b.X + w3 + 5;
				var ww = b.Width - (w3 + 5);

				if (AutoPvP.CMOptions.ModuleEnabled)
				{
					text = UniGlyph.CircleRightFill + " Enabled";

					AddHtmlButton(b.X, b.Y, w3, 20, o => OnDisable(), text, Color.LawnGreen, Color.Black);
				}
				else
				{
					text = UniGlyph.CircleLeftFill + " Disabled";

					AddHtmlButton(b.X, b.Y, w3, 20, o => OnEnable(), text, Color.IndianRed, Color.Black);
				}

				text = "Enable or disable the module";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleDot + " Options";

				AddHtmlButton(b.X, b.Y, w3, 20, o => OnOptions(), text, Color.SkyBlue, Color.Black);

				text = "Opens the module settings UI";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleDot + " Seasons";

				AddHtmlButton(b.X, b.Y, w3, 20, o => OnSchedule(), text, Color.SkyBlue, Color.Black);

				text = "Opens the season schedule UI";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.StarFill + " Create Battle";

				AddHtmlButton(b.X, b.Y, w3, 20, o => OnCreateBattle(), text, Color.LawnGreen, Color.Black);

				text = "Begin creating a new battle";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.Coffee + " Internalize Battles";

				AddHtmlButton(b.X, b.Y, w3, 20, o => OnInternalizeBattles(true), text, Color.Yellow, Color.Black);

				text = "Internalizes all Battles";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleX + " Delete Battles";

				AddHtmlButton(b.X, b.Y, w3, 20, o => OnDeleteBattles(true), text, Color.IndianRed, Color.Black);

				text = "Deletes all Battles";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;

				text = UniGlyph.CircleX + " Delete Profiles";

				AddHtmlButton(b.X, b.Y, w3, 20, o => OnDeleteProfiles(true), text, Color.IndianRed, Color.Black);

				text = "Deletes all Profiles";

				AddHtml(xx, b.Y, ww, 20, text, HtmlColor, Color.Black);

				b.Y += 20;
				b.Height -= 20;
			}

			ObjectPool.Free(ref build);
		}

		#region Module
		protected virtual void OnEnable()
		{
			if (CanEdit)
			{
				AutoPvP.CMOptions.ModuleEnabled = true;
			}

			Refresh(true);
		}

		protected virtual void OnDisable()
		{
			if (CanEdit)
			{
				AutoPvP.CMOptions.ModuleEnabled = false;
			}

			Refresh(true);
		}

		protected virtual void OnSchedule()
		{
			new ScheduleOverviewGump(User, AutoPvP.SeasonSchedule, Hide(true)).Send();
		}

		protected virtual void OnOptions()
		{
			Refresh(true);

			if (CanEdit)
			{
				User.SendGump(new PropertiesGump(User, AutoPvP.CMOptions));
			}
		}

		protected virtual void OnCreateBattle()
		{
			new PvPScenariosUI(User, Hide(true)).Send();
		}

		protected virtual void OnInternalizeBattles(bool confirm)
		{
			if (CanEdit)
			{
				if (confirm)
				{
					new ConfirmDialogGump(User, this)
					{
						Title = "Internalize All Battles?",
						Html = "All battles will be internalized, forcing them to end.\nDo you want to continue?",
						AcceptHandler = o => OnInternalizeBattles(false)
					}.Send();
				}
				else
				{
					AutoPvP.InternalizeAllBattles();
				}
			}

			Refresh(true);
		}

		protected virtual void OnDeleteBattles(bool confirm)
		{
			if (CanEdit)
			{
				if (confirm)
				{
					new ConfirmDialogGump(User, this)
					{
						Title = "Delete All Battles?",
						Html = "All data associated with all battles will be deleted." +
							   "\nThis action can not be reversed!\nDo you want to continue?",
						AcceptHandler = o => OnDeleteBattles(false)
					}.Send();
				}
				else
				{
					AutoPvP.DeleteAllBattles();
				}
			}

			Refresh(true);
		}

		protected virtual void OnDeleteProfiles(bool confirm)
		{
			if (CanEdit)
			{
				if (confirm)
				{
					new ConfirmDialogGump(User, this)
					{
						Title = "Delete All Profiles?",
						Html = "All data associated with all profiles will be deleted." +
							   "\nThis action can not be reversed!\nDo you want to continue?",
						AcceptHandler = o => OnDeleteProfiles(false)
					}.Send();
				}
				else
				{
					AutoPvP.DeleteAllProfiles();
				}
			}

			Refresh(true);
		}
		#endregion
	}
}