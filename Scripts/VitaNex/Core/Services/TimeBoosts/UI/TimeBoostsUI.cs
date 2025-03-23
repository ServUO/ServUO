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

namespace VitaNex.TimeBoosts
{
	public class TimeBoostsUI : SuperGumpList<ITimeBoost>
	{
		public static string DefaultTitle = "Time Boosts";
		public static string DefaultSubTitle = String.Empty;
		public static string DefaultEmptyText = "No available Time Boosts.";
		public static string DefaultSummaryText = "Time Left";
		public static string DefaultHelpText = "Tip: Click an available time boost to update the time remaining!";

		public static Color DefaultTitleColor = Color.PaleGoldenrod;
		public static Color DefaultSubTitleColor = Color.Goldenrod;
		public static Color DefaultEmptyTextColor = Color.SaddleBrown;
		public static Color DefaultBoostTextColor = Color.Yellow;
		public static Color DefaultBoostCountColor = Color.LawnGreen;
		public static Color DefaultSummaryTextColor = Color.Goldenrod;
		public static Color DefaultHelpTextColor = Color.Gold;

		public static void Update(PlayerMobile user)
		{
			foreach (var info in EnumerateInstances<TimeBoostsUI>(user, true).Where(g => g != null && !g.IsDisposed && g.IsOpen))
				info.Refresh(true);
		}

		public string Title { get; set; }
		public string SubTitle { get; set; }
		public string EmptyText { get; set; }
		public string SummaryText { get; set; }
		public string HelpText { get; set; }

		public Color TitleColor { get; set; }
		public Color SubTitleColor { get; set; }
		public Color EmptyTextColor { get; set; }
		public Color BoostTextColor { get; set; }
		public Color BoostCountColor { get; set; }
		public Color SummaryTextColor { get; set; }
		public Color HelpTextColor { get; set; }

		public TimeBoostProfile Profile { get; set; }

		public bool ApplyAutoAllowed { get; set; }
		public bool ApplyAutoPreview { get; set; }

		public Func<ITimeBoost, bool> CanApply { get; set; }
		public Action<ITimeBoost> BoostUsed { get; set; }

		public Func<TimeSpan> GetTime { get; set; }
		public Action<TimeSpan> SetTime { get; set; }

		public TimeSpan? OldTime { get; private set; }

		public TimeSpan? Time { get => GetTime?.Invoke(); set => SetTime?.Invoke(value ?? TimeSpan.Zero); }

		public TimeBoostsUI(Mobile user, Gump parent = null, TimeBoostProfile profile = null, Func<ITimeBoost, bool> canApply = null, Action<ITimeBoost> boostUsed = null, Func<TimeSpan> getTime = null, Action<TimeSpan> setTime = null)
			: base(user, parent)
		{
			Profile = profile;

			CanApply = canApply;
			BoostUsed = boostUsed;

			GetTime = getTime;
			SetTime = setTime;

			Title = DefaultTitle;
			SubTitle = DefaultSubTitle;
			EmptyText = DefaultEmptyText;
			SummaryText = DefaultSummaryText;
			HelpText = DefaultHelpText;

			TitleColor = DefaultTitleColor;
			SubTitleColor = DefaultSubTitleColor;
			EmptyTextColor = DefaultEmptyTextColor;
			BoostTextColor = DefaultBoostTextColor;
			BoostCountColor = DefaultBoostCountColor;
			SummaryTextColor = DefaultSummaryTextColor;
			HelpTextColor = DefaultHelpTextColor;

			ApplyAutoAllowed = true;

			EntriesPerPage = 4;

			Sorted = true;

			CanClose = true;
			CanDispose = true;
			CanMove = true;
			CanResize = false;

			AutoRefreshRate = TimeSpan.FromMinutes(1.0);
			AutoRefresh = true;

			ForceRecompile = true;
		}

		protected override void Compile()
		{
			if (Profile == null && User != null)
				Profile = TimeBoosts.EnsureProfile(User.Account);

			if (!ApplyAutoAllowed)
				ApplyAutoPreview = false;

			base.Compile();
		}

		protected override void CompileList(List<ITimeBoost> list)
		{
			list.Clear();

			var boosts = TimeBoosts.AllTimes.Where(b => Profile == null || Profile[b] > 0);

			if (ApplyAutoAllowed && ApplyAutoPreview)
			{
				var time = Time?.Ticks ?? 0;

				if (time > 0)
				{
					foreach (var boost in boosts.OrderByDescending(b => b.Value.Ticks))
					{
						if (boost.Value.Ticks > time)
							break;

						var take = Math.Min(Profile[boost], (int)(time / boost.Value.Ticks));

						if (take <= 0)
							break;

						time -= take * boost.Value.Ticks;

						list.Add(new BoostPreview(boost, take));
					}
				}
			}
			else
				list.AddRange(boosts);

			base.CompileList(list);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add("background", () =>
			{
				AddBackground(0, 0, 480, 225, 9270);
				AddBackground(15, 40, 450, 140, 9350);
			});

			layout.Add("title", () =>
			{
				if (String.IsNullOrWhiteSpace(Title))
					return;

				var text = Title;

				text = text.ToUpper();
				text = text.WrapUOHtmlBold();
				text = text.WrapUOHtmlColor(TitleColor, false);

				AddHtml(20, 15, 235, 40, text, false, false);
			});

			layout.Add("subtitle", () =>
			{
				if (String.IsNullOrWhiteSpace(SubTitle))
					return;

				var text = SubTitle;

				text = text.ToUpper();
				text = text.WrapUOHtmlBold();
				text = text.WrapUOHtmlColor(SubTitleColor, false);
				text = text.WrapUOHtmlRight();

				AddHtml(255, 15, 200, 40, text, false, false);
			});

			layout.Add("page/prev", () =>
			{
				if (HasPrevPage)
					AddButton(7, 150, 9910, 9911, PreviousPage);
				else
					AddImage(7, 150, 9909);
			});

			layout.Add("page/next", () =>
			{
				if (HasNextPage)
					AddButton(451, 150, 9904, 9905, NextPage);
				else
					AddImage(451, 150, 9903);
			});

			CompileEntryLayout(layout, GetListRange());

			layout.Add("summary", () =>
			{
				TimeSpan time;

				if (ApplyAutoPreview)
					time = TimeSpan.FromTicks(List.OfType<BoostPreview>().Aggregate(0L, (t, b) => t + (b.Value.Ticks * b.Amount)));
				else
					time = Time ?? TimeSpan.Zero;

				if (time < TimeSpan.Zero)
					time = TimeSpan.Zero;

				var text = SummaryText;

				if (ApplyAutoPreview)
					text = "Total Time";

				text = text.ToUpper();
				text = text.WrapUOHtmlBold();
				text = text.WrapUOHtmlColor(SummaryTextColor, false);
				text = text.WrapUOHtmlCenter();

				AddHtml(20, 192, 87, 40, text, false, false);
				AddImage(112, 185, 30223);

				if (ApplyAutoPreview)
					AddImageTime(140, 185, time, 0x55); // 175 x 28
				else
					AddImageTime(140, 185, time, 0x33); // 175 x 28

				AddImage(315, 185, 30223);
			});

			layout.Add("auto", () =>
			{
				if (ApplyAutoPreview)
					AddButton(350, 187, 2124, 2123, b => ApplyAuto(false));
				else if (ApplyAutoAllowed)
					AddButton(350, 187, 2113, 2112, b => ApplyAuto(true));
				else
					AddImage(350, 187, 2113, 900);
			});

			layout.Add("okay", () =>
			{
				if (ApplyAutoPreview)
					AddButton(410, 187, 2121, 2120, b => ApplyAuto(null));
				else
					AddButton(410, 187, 2130, 2129, Close);
			});

			layout.Add("tip", () =>
			{
				AddBackground(0, 225, 480, 40, 9270);

				var text = HelpText;

				if (ApplyAutoPreview)
					text = "Automatically use boosts as displayed above.";

				text = text.WrapUOHtmlSmall();
				text = text.WrapUOHtmlCenter();
				text = text.WrapUOHtmlColor(HelpTextColor);

				AddHtml(20, 235, 440, 40, text, false, false);
			});
		}

		protected virtual void CompileEntryLayout(SuperGumpLayout layout, Dictionary<int, ITimeBoost> range)
		{
			if (range.Count > 0)
			{
				var i = 0;

				foreach (var kv in range)
				{
					CompileEntryLayout(layout, range.Count, kv.Key, i, 53 + (i * 93), kv.Value);

					++i;
				}
			}
			else
			{
				layout.Add("empty", () =>
				{
					var text = EmptyText;

					text = text.WrapUOHtmlColor(EmptyTextColor, false);
					text = text.WrapUOHtmlCenter();

					AddHtml(35, 150, 355, 40, text, false, false);
				});
			}
		}

		protected virtual void CompileEntryLayout(SuperGumpLayout layout, int length, int index, int pIndex, int xOffset, ITimeBoost boost)
		{
			layout.Add("boosts/" + index, () =>
			{
				BoostPreview? bp = null;

				if (boost is BoostPreview p)
					bp = p;

				int hue = boost.Hue, bellID = 10850;

				var text = String.Empty;

				if (boost.Value.Hours != 0)
				{
					bellID = 10810; //Blue Gem
					text = "Hour";
				}
				else if (boost.Value.Minutes != 0)
				{
					bellID = 10830; //Green Gem
					text = "Min";
				}
				else
					hue = 2999;

				if (!String.IsNullOrWhiteSpace(text) && boost.RawValue != 1)
					text += "s";

				if (hue == 2999)
				{
					AddImage(xOffset + 7, 56, bellID, hue); //Left Bell
					AddImage(xOffset + 57, 56, bellID, hue); //Right Bell
				}
				else
				{
					AddImage(xOffset + 7, 56, bellID); //Left Bell
					AddImage(xOffset + 57, 56, bellID); //Right Bell
				}

				AddImage(xOffset, 45, 30058, hue); //Hammer

				if (Profile != null && bp == null)
					AddButton(xOffset + 5, 65, 1417, 1417, b => SelectBoost(boost)); //Disc

				AddImage(xOffset + 5, 65, 1417, hue); //Disc
				AddImage(xOffset + 14, 75, 5577, 2999); //Blackout

				AddImageNumber(xOffset + 44, 99, boost.RawValue, hue, Axis.Both);

				if (!String.IsNullOrWhiteSpace(text))
				{
					text = text.WrapUOHtmlSmall();
					text = text.WrapUOHtmlCenter();
					text = text.WrapUOHtmlColor(BoostTextColor, false);

					AddHtml(xOffset + 20, 110, 50, 40, text, false, false);
				}

				if (hue == 2999)
					AddImage(xOffset, 130, 30062, hue); //Feet
				else
				{
					if (Profile != null)
						AddBackground(xOffset + 10, 143, 70, 30, 9270); //9300

					AddImage(xOffset, 130, 30062); //Feet

					if (Profile != null)
					{
						text = $"{(bp?.Amount ?? Profile[boost]):N0}";
						text = text.WrapUOHtmlBold();
						text = text.WrapUOHtmlCenter();
						text = text.WrapUOHtmlColor(BoostCountColor, false);

						AddHtml(xOffset + 12, 150, 65, 40, text, false, false);
					}
				}
			});
		}

		public virtual void ApplyAuto(bool? state)
		{
			if (!ApplyAutoAllowed || state == null)
				ApplyAutoPreview = false;
			else if (state == true)
				ApplyAutoPreview = true;
			else if (ApplyAutoPreview)
			{
				foreach (var bp in List.OfType<BoostPreview>())
				{
					var amount = bp.Amount;

					while (--amount >= 0 && (IsOpen || Hidden))
					{
						if (ApplyBoost(bp.Boost))
						{
							OnBoostUsed(bp.Boost);

							if (IsOpen || Hidden)
								continue;
						}

						break;
					}

					if (!IsOpen && !Hidden)
						break;
				}

				ApplyAutoPreview = false;
			}

			if (IsOpen || Hidden)
				Refresh(true);
		}

		public virtual void SelectBoost(ITimeBoost boost)
		{
			if (CanApplyBoost(boost))
				ApplyBoost(boost, boost.Value.TotalHours >= 6);
			else
				Refresh(true);
		}

		public virtual void ApplyBoost(ITimeBoost boost, bool confirm)
		{
			if (CanApplyBoost(boost))
			{
				if (confirm)
				{
					new ConfirmDialogGump(User, Hide(true))
					{
						Icon = 7057,
						Title = $"Use {boost.Name}?",
						Html = $"{Title}: {SubTitle}\n{boost.RawValue} {boost.Desc}{(boost.RawValue != 1 ? "s" : String.Empty)}.\nClick OK to apply this Time Boost.",
						AcceptHandler = b => ApplyBoost(boost, false),
						CancelHandler = b => Refresh(true)
					}.Send();

					return;
				}

				if (ApplyBoost(boost))
					OnBoostUsed(boost);
			}

			if (IsOpen || Hidden)
				Refresh(true);
		}

		public virtual bool ApplyBoost(ITimeBoost boost)
		{
			if (!CanApplyBoost(boost) || !Profile.Consume(boost, 1))
				return false;

			OldTime = Time;
			Time -= boost.Value;

			return true;
		}

		protected virtual bool CanApplyBoost(ITimeBoost boost)
		{
			return GetTime != null && SetTime != null && Time > TimeSpan.Zero && boost != null && boost.RawValue > 0 &&
				   Profile != null && Profile.Owner == User.Account && (CanApply == null || CanApply(boost));
		}

		protected virtual void OnBoostUsed(ITimeBoost boost)
		{
			BoostUsed?.Invoke(boost);

			LogBoostUsed(boost);
		}

		protected virtual void LogBoostUsed(ITimeBoost boost)
		{
			var log = new StringBuilder();

			log.AppendLine();
			log.AppendLine("UI: '{0}' : '{1}' : '{2}'", Title, SubTitle, SummaryText);
			log.AppendLine("User: {0}", User);
			log.AppendLine("Boost: {0}", boost);
			log.AppendLine("Get: {0}", GetTime.Trace());
			log.AppendLine("Set: {0}", SetTime.Trace());
			log.AppendLine("Time: {0} > {1}", OldTime, Time);
			log.AppendLine();

			log.Log($"/TimeBoosts/{DateTime.Now.ToDirectoryName()}/{boost}.log");
		}

		public override int SortCompare(ITimeBoost a, ITimeBoost b)
		{
			var res = 0;

			if (a.CompareNull(b, ref res))
				return res;

			if (a.Value < b.Value)
				return -1;

			if (a.Value > b.Value)
				return 1;

			return 0;
		}

		protected override void OnDisposed()
		{
			base.OnDisposed();

			Profile = null;
		}

		protected struct BoostPreview : ITimeBoost
		{
			public readonly ITimeBoost Boost;
			public readonly int Amount;

			public int RawValue => Boost.RawValue;
			public TimeSpan Value => Boost.Value;
			public string Desc => Boost.Desc;
			public string Name => Boost.Name;
			public int Hue => Boost.Hue;

			public BoostPreview(ITimeBoost boost, int amount)
			{
				Boost = boost;
				Amount = amount;
			}
		}
	}
}