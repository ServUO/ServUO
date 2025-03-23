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
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class TextInputPanelGump<T> : PanelGump<T>
	{
		private int _Limit;

		public virtual int Limit
		{
			get => _Limit;
			set
			{
				value = Math.Max(0, value);

				if (_Limit == value)
				{
					return;
				}

				_Limit = value;
				Refresh(true);
			}
		}

		public virtual string Input { get; set; }
		public virtual bool InputBackground { get; set; }

		public bool Limited => (Limit > 0);

		public Action<string> Callback { get; set; }

		public TextInputPanelGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			int width = 420,
			int height = 420,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null,
			T selected = default(T),
			string input = null,
			int limit = 0,
			Action<string> callback = null)
			: base(user, parent, x, y, width, height, emptyText, title, opts, selected)
		{
			InputBackground = true;

			Input = input ?? String.Empty;
			_Limit = limit;
			Callback = callback;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			if (Minimized)
			{
				return;
			}

			if (InputBackground)
			{
				layout.Add("background/input/base", () => AddBackground(15, 65, Width - 30, Height - 30, 9350));
			}

			if (Limited)
			{
				layout.Add(
					"textentry/body/base",
					() => AddTextEntryLimited(25, 75, Width - 40, Height - 40, TextHue, Input, Limit, ParseInput));
			}
			else
			{
				layout.Add("textentry/body/base", () => AddTextEntry(25, 75, Width - 40, Height - 40, TextHue, Input, ParseInput));
			}
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
			Input = text ?? String.Empty;
		}

		protected override void OnClick()
		{
			base.OnClick();

			if (Callback != null)
			{
				Callback(Input);
			}
		}
	}
}