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
	public class InputDialogGump : DialogGump
	{
		public virtual Action<GumpButton, string> Callback { get; set; }

		public virtual string InputText { get; set; }
		public virtual int Limit { get; set; }

		public bool Limited => Limit > 0;

		public InputDialogGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			string title = null,
			string html = null,
			string input = null,
			int limit = 0,
			int icon = 7020,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null,
			Action<GumpButton, string> callback = null)
			: base(user, parent, x, y, title, html, icon, onAccept, onCancel)
		{
			InputText = input ?? String.Empty;
			Limit = limit;
			Callback = callback;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add("background/body/input", () => AddBackground(20, Height - 50, Width - 120, 30, 9350));

			layout.Add(
				"textentry/body/input",
				() =>
				{
					if (Limited)
					{
						AddTextEntryLimited(25, Height - 45, Width - 130, 20, TextHue, InputText, Limit, ParseInput);
					}
					else
					{
						AddTextEntry(25, Height - 45, Width - 130, 20, TextHue, InputText, ParseInput);
					}
				});
		}

		protected virtual void ParseInput(GumpTextEntry entry, string text)
		{
			ParseInput(text);
		}

		protected virtual void ParseInput(GumpTextEntryLimited entry, string text)
		{
			ParseInput(text);
		}

		protected virtual void ParseInput(string text)
		{
			InputText = text;
		}

		protected override void OnAccept(GumpButton button)
		{
			if (Callback != null)
			{
				Callback(button, InputText);
			}

			base.OnAccept(button);
		}

		protected override void OnCancel(GumpButton button)
		{
			InputText = String.Empty;

			base.OnCancel(button);
		}
	}
}