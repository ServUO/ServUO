#region Header
// **********
// ServUO - Buttons.cs
// **********
#endregion

namespace Server.Gumps
{
	public partial class Gump
	{
		public void AddButton(
			int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param, string name = "")
		{
			Add(new GumpButton(x, y, normalID, pressedID, buttonID, type, param, null, name));
		}

		public void AddButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int buttonID,
			GumpButtonType type,
			int param,
			GumpResponse callback,
			string name = "")
		{
			Add(new GumpButton(x, y, normalID, pressedID, buttonID, type, param, callback));
		}

		public void AddButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			GumpButtonType type,
			GumpResponse callback,
			int param = 0,
			string name = "")
		{
			Add(new GumpButton(x, y, normalID, pressedID, NewID(), type, param, callback, name));
		}

		public void AddImageTiledButton(
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
			int height,
			int localizedTooltip = -1,
			string name = "")
		{
			Add(
				new GumpImageTileButton(
					x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height, null, localizedTooltip, name));
		}

		public void AddImageTiledButton(
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
			int height,
			GumpResponse callback,
			int localizedTooltip = -1,
			string name = "")
		{
			Add(
				new GumpImageTileButton(
					x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height, callback, localizedTooltip, name));
		}

		public void AddImageTiledButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int buttonID,
			GumpButtonType type,
			int itemID,
			int hue,
			int width,
			int height,
			GumpResponse callback,
			int param = 0,
			int localizedTooltip = -1,
			string name = "")
		{
			Add(
				new GumpImageTileButton(
					x, y, normalID, pressedID, NewID(), type, param, itemID, hue, width, height, callback, localizedTooltip, name));
		}
	}
}