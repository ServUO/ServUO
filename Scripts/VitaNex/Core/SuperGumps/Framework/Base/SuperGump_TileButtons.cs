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
		private Dictionary<GumpImageTileButton, Action<GumpImageTileButton>> _TileButtons;

		public Dictionary<GumpImageTileButton, Action<GumpImageTileButton>> TileButtons
		{
			get => _TileButtons;
			protected set => _TileButtons = value;
		}

		public virtual Action<GumpImageTileButton> TileButtonHandler { get; set; }

		public GumpImageTileButton LastTileButtonClicked { get; protected set; }

		public new void AddImageTiledButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int buttonID,
			GumpButtonType type,
			int param,
			int itemID,
			int hue,
			int width,
			int height)
		{
			if (type == GumpButtonType.Page)
			{
				AddImageTiledButton(
					new GumpImageTileButton(x, y, normalID, pressedID, 0, GumpButtonType.Page, param, itemID, hue, width, height),
					null);
			}
			else
			{
				AddImageTiledButton(x, y, normalID, pressedID, buttonID, itemID, hue, width, height, null);
			}
		}

		public void AddImageTiledButton(int x, int y, int normalID, int pressedID, int itemID, int hue, int width, int height)
		{
			AddImageTiledButton(x, y, normalID, pressedID, NewButtonID(), itemID, hue, width, height, null);
		}

		public void AddImageTiledButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int itemID,
			int hue,
			int width,
			int height,
			Action<GumpImageTileButton> handler)
		{
			AddImageTiledButton(x, y, normalID, pressedID, NewButtonID(), itemID, hue, width, height, handler);
		}

		public void AddImageTiledButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int buttonID,
			int itemID,
			int hue,
			int width,
			int height)
		{
			AddImageTiledButton(x, y, normalID, pressedID, buttonID, itemID, hue, width, height, null);
		}

		public void AddImageTiledButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int buttonID,
			int itemID,
			int hue,
			int width,
			int height,
			Action<GumpImageTileButton> handler)
		{
			AddImageTiledButton(
				new GumpImageTileButton(x, y, normalID, pressedID, buttonID, GumpButtonType.Reply, 0, itemID, hue, width, height),
				handler);
		}

		protected void AddImageTiledButton(GumpImageTileButton entry, Action<GumpImageTileButton> handler)
		{
			if (entry == null || !CanDisplay(entry))
			{
				return;
			}

			TileButtons[entry] = handler;

			Add(entry);
		}

		public virtual void HandleTileButtonClick(GumpImageTileButton button)
		{
			var now = DateTime.UtcNow;
			var lbc = LastTileButtonClicked;
			var lbt = LastButtonClick + DClickInterval;

			DoubleClicked = lbc != null && now <= lbt && (lbc == button || lbc.ButtonID == button.ButtonID ||
														  (lbc.Parent == button.Parent && lbc.X == button.X && lbc.Y == button.Y && lbc.Type == button.Type &&
														   lbc.Param == button.Param));

			LastTileButtonClicked = button;
			LastButtonClick = now;

			OnClick();
			OnClick(button);

			if (DoubleClicked)
			{
				OnDoubleClick(button);
			}

			if (TileButtonHandler != null)
			{
				TileButtonHandler(button);
			}
			else if (TileButtons[button] != null)
			{
				TileButtons[button](button);
			}
		}

		protected virtual void OnClick(GumpImageTileButton entry)
		{ }

		protected virtual void OnDoubleClick(GumpImageTileButton entry)
		{ }

		protected virtual bool CanDisplay(GumpImageTileButton entry)
		{
			return entry != null;
		}

		public GumpImageTileButton GetTileButtonEntry(int buttonID)
		{
			return TileButtons.Keys.FirstOrDefault(button => button.ButtonID == buttonID);
		}
	}
}