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
		private Dictionary<GumpButton, Action<GumpButton>> _Buttons;

		public Dictionary<GumpButton, Action<GumpButton>> Buttons
		{
			get => _Buttons;
			protected set => _Buttons = value;
		}

		public virtual Action<GumpButton> ButtonHandler { get; set; }

		public GumpButton LastButtonClicked { get; protected set; }

		public new void AddButton(int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param)
		{
			if (type == GumpButtonType.Page)
			{
				AddButton(new GumpButton(x, y, normalID, pressedID, 0, GumpButtonType.Page, param), null);
			}
			else
			{
				AddButton(x, y, normalID, pressedID, buttonID, null);
			}
		}

		public void AddButton(int x, int y, int normalID, int pressedID)
		{
			AddButton(x, y, normalID, pressedID, NewButtonID(), null);
		}

		public void AddButton(int x, int y, int normalID, int pressedID, Action<GumpButton> handler)
		{
			AddButton(x, y, normalID, pressedID, NewButtonID(), handler);
		}

		public void AddButton(int x, int y, int normalID, int pressedID, int buttonID)
		{
			AddButton(x, y, normalID, pressedID, buttonID, null);
		}

		public void AddButton(int x, int y, int normalID, int pressedID, int buttonID, Action<GumpButton> handler)
		{
			AddButton(new GumpButton(x, y, normalID, pressedID, buttonID, GumpButtonType.Reply, 0), handler);
		}

		protected void AddButton(GumpButton entry, Action<GumpButton> handler)
		{
			if (entry == null || !CanDisplay(entry))
			{
				return;
			}

			Buttons[entry] = handler;

			Add(entry);
		}

		public virtual void HandleButtonClick(GumpButton button)
		{
			var now = DateTime.UtcNow;
			var lbc = LastButtonClicked;
			var lbt = LastButtonClick + DClickInterval;

			DoubleClicked = lbc != null && now <= lbt && (lbc == button || lbc.ButtonID == button.ButtonID ||
														  (lbc.Parent == button.Parent && lbc.X == button.X && lbc.Y == button.Y && lbc.Type == button.Type &&
														   lbc.Param == button.Param));

			LastButtonClicked = button;
			LastButtonClick = now;

			OnClick();
			OnClick(button);

			if (DoubleClicked)
			{
				OnDoubleClick(button);
			}

			if (ButtonHandler != null)
			{
				ButtonHandler(button);
			}
			else if (Buttons[button] != null)
			{
				Buttons[button](button);
			}
		}

		protected virtual void OnClick(GumpButton entry)
		{ }

		protected virtual void OnDoubleClick(GumpButton entry)
		{ }

		protected virtual bool CanDisplay(GumpButton entry)
		{
			return entry != null;
		}

		public GumpButton GetButtonEntry(int buttonID)
		{
			return Buttons.Keys.FirstOrDefault(button => button.ButtonID == buttonID);
		}
	}
}