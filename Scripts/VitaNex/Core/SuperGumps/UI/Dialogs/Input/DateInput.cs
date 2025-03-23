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

using Server;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class InputDateDialogGump : InputDialogGump
	{
		protected int Day;
		protected int Month;
		protected int Year;

		public virtual Action<GumpButton, DateTime?> CallbackDate { get; set; }

		public virtual DateTime MinDate { get; set; }
		public virtual DateTime MaxDate { get; set; }
		public virtual DateTime? InputDate { get; set; }

		public InputDateDialogGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			string title = "Date Input",
			string html = "Enter a date below.",
			DateTime? input = null,
			DateTime? minDate = null,
			DateTime? maxDate = null,
			int icon = 7020,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null,
			Action<GumpButton, DateTime?> callback = null)
			: base(user, parent, x, y, title, html, String.Empty, 10, icon, onAccept, onCancel)
		{
			if (input != null)
			{
				InputDate = input;
				InputText = InputDate.Value.ToSimpleString("m-d-y");
			}

			MinDate = minDate ?? DateTime.MinValue;
			MaxDate = maxDate ?? DateTime.MaxValue;

			Limit = 10;

			CallbackDate = callback;

			Callback = (b, text) =>
			{
				if (CallbackDate != null)
				{
					CallbackDate(b, InputDate);
				}
			};
		}

		protected override void Compile()
		{
			base.Compile();

			Limit = 10;
			InputText = InputDate.HasValue ? InputDate.Value.ToSimpleString("m-d-y") : String.Empty;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Remove("background/body/input");
			layout.Remove("textentry/body/input");

			layout.Add(
				"background/body/input/month",
				() =>
				{
					AddHtml(20, Height - 45, 20, 20, "MM".WrapUOHtmlColor(HtmlColor), false, false);
					AddBackground(50, 250, 40, 30, 9350);
					AddTextEntryLimited(
						55,
						Height - 45,
						30,
						20,
						TextHue,
						0,
						InputDate.HasValue ? InputDate.Value.Month.ToString("D2") : String.Empty,
						2,
						ParseInput);
				});

			layout.Add(
				"background/body/input/day",
				() =>
				{
					AddHtml(100, Height - 45, 20, 20, "DD".WrapUOHtmlColor(HtmlColor), false, false);
					AddBackground(130, 250, 40, 30, 9350);
					AddTextEntryLimited(
						135,
						Height - 45,
						30,
						20,
						TextHue,
						1,
						InputDate.HasValue ? InputDate.Value.Day.ToString("D2") : String.Empty,
						2,
						ParseInput);
				});

			layout.Add(
				"background/body/input/year",
				() =>
				{
					AddHtml(180, Height - 45, 40, 20, "YYYY".WrapUOHtmlColor(HtmlColor), false, false);
					AddBackground(220, 250, 70, 30, 9350);
					AddTextEntryLimited(
						225,
						Height - 45,
						60,
						20,
						TextHue,
						2,
						InputDate.HasValue ? InputDate.Value.Year.ToString("D4") : String.Empty,
						4,
						ParseInput);
				});
		}

		protected override void ParseInput(GumpTextEntryLimited entry, string text)
		{
			switch (entry.EntryID)
			{
				case 0:
					Month = Int32.TryParse(text, out Month) ? Math.Max(1, Math.Min(12, Month)) : 0;
					break;
				case 1:
					Day = Int32.TryParse(text, out Day) ? Math.Max(1, Math.Min(31, Day)) : 0;
					break;
				case 2:
					Year = Int32.TryParse(text, out Year) ? Math.Max(1, Math.Min(9999, Year)) : 0;
					break;
			}

			base.ParseInput(entry, text);
		}

		protected override void ParseInput(string text)
		{
			if (Year > 0 && Month > 0 && Day > 0)
			{
				Day = Math.Min(DateTime.DaysInMonth(Year, Month), Day);
				InputDate = new DateTime(Year, Month, Day);
			}
			else
			{
				InputDate = null;
			}

			if (InputDate.HasValue)
			{
				if (InputDate < MinDate)
				{
					InputDate = MinDate;
				}
				else if (InputDate > MaxDate)
				{
					InputDate = MaxDate;
				}
			}

			base.ParseInput(InputDate.HasValue ? InputDate.Value.ToSimpleString("m-d-y") : String.Empty);
		}
	}
}