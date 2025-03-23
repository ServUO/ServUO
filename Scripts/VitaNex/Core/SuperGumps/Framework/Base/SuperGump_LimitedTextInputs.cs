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
using System.Linq;

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		private Dictionary<GumpTextEntryLimited, Action<GumpTextEntryLimited, string>> _LimitedTextInputs;

		public Dictionary<GumpTextEntryLimited, Action<GumpTextEntryLimited, string>> LimitedTextInputs
		{
			get => _LimitedTextInputs;
			protected set => _LimitedTextInputs = value;
		}

		public Action<GumpTextEntryLimited, string> LimitedTextInputHandler { get; set; }

		public new void AddTextEntry(int x, int y, int width, int height, int hue, int inputID, string text, int length)
		{
			AddTextEntryLimited(x, y, width, height, hue, inputID, text, length, null);
		}

		public void AddTextEntryLimited(int x, int y, int width, int height, int hue, string text, int length)
		{
			AddTextEntryLimited(x, y, width, height, hue, NewTextEntryID(), text, length, null);
		}

		public void AddTextEntryLimited(
			int x,
			int y,
			int width,
			int height,
			int hue,
			string text,
			int length,
			Action<GumpTextEntryLimited, string> handler)
		{
			AddTextEntryLimited(x, y, width, height, hue, NewTextEntryID(), text, length, handler);
		}

		public void AddTextEntryLimited(int x, int y, int width, int height, int hue, int inputID, string text, int length)
		{
			AddTextEntryLimited(x, y, width, height, hue, inputID, text, length, null);
		}

		public void AddTextEntryLimited(
			int x,
			int y,
			int width,
			int height,
			int hue,
			int inputID,
			string text,
			int length,
			Action<GumpTextEntryLimited, string> handler)
		{
			AddTextEntryLimited(new GumpTextEntryLimited(x, y, width, height, hue, inputID, text, length), handler);
		}

		protected void AddTextEntryLimited(GumpTextEntryLimited input, Action<GumpTextEntryLimited, string> handler)
		{
			if (input == null)
			{
				return;
			}

			LimitedTextInputs[input] = handler;

			Add(input);
		}

		public virtual void HandleLimitedTextInput(GumpTextEntryLimited input, string text)
		{
			if (LimitedTextInputHandler != null)
			{
				LimitedTextInputHandler(input, text);
			}
			else if (LimitedTextInputs[input] != null)
			{
				LimitedTextInputs[input](input, text);
			}
		}

		public virtual bool CanDisplay(GumpTextEntryLimited input)
		{
			return input != null;
		}

		public GumpTextEntryLimited GetTextEntryLimited(int inputID)
		{
			return LimitedTextInputs.Keys.FirstOrDefault(input => input.EntryID == inputID);
		}
	}
}