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
		private Dictionary<GumpTextEntry, Action<GumpTextEntry, string>> _TextInputs;

		public Dictionary<GumpTextEntry, Action<GumpTextEntry, string>> TextInputs
		{
			get => _TextInputs;
			protected set => _TextInputs = value;
		}

		public Action<GumpTextEntry, string> TextInputHandler { get; set; }

		public new void AddTextEntry(int x, int y, int width, int height, int hue, int inputID, string text)
		{
			AddTextEntry(x, y, width, height, hue, inputID, text, null);
		}

		public void AddTextEntry(int x, int y, int width, int height, int hue, string text)
		{
			AddTextEntry(x, y, width, height, hue, NewTextEntryID(), text, null);
		}

		public void AddTextEntry(
			int x,
			int y,
			int width,
			int height,
			int hue,
			string text,
			Action<GumpTextEntry, string> handler)
		{
			AddTextEntry(x, y, width, height, hue, NewTextEntryID(), text, handler);
		}

		public void AddTextEntry(
			int x,
			int y,
			int width,
			int height,
			int hue,
			int entryID,
			string text,
			Action<GumpTextEntry, string> handler)
		{
			AddTextEntry(new GumpTextEntry(x, y, width, height, hue, entryID, text), handler);
		}

		protected void AddTextEntry(GumpTextEntry input, Action<GumpTextEntry, string> handler)
		{
			if (input == null)
			{
				return;
			}

			TextInputs[input] = handler;

			Add(input);
		}

		public virtual void HandleTextInput(GumpTextEntry input, string text)
		{
			if (TextInputHandler != null)
			{
				TextInputHandler(input, text);
			}
			else if (TextInputs[input] != null)
			{
				TextInputs[input](input, text);
			}
		}

		public virtual bool CanDisplay(GumpTextEntry input)
		{
			return input != null;
		}

		public GumpTextEntry GetTextEntry(int inputID)
		{
			return TextInputs.Keys.FirstOrDefault(input => input.EntryID == inputID);
		}
	}
}