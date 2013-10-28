#region Header
// **********
// ServUO - GumpImageTileButton.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpImageTileButton : GumpEntry, IInputEntry
	{
		public event GumpResponse OnGumpResponse;

		private static readonly byte[] _LayoutName = Gump.StringToBuffer("buttontileart");
		private static readonly byte[] _LayoutTooltip = Gump.StringToBuffer(" }{ tooltip");
		private int _EntryID;
		private GumpResponse _Callback;
		private int _Height;
		private int _Hue;
		private int _ID1, _ID2;
		private int _ItemID;
		private int _LocalizedTooltip;
		private string _Name;
		private int _Param;
		private GumpButtonType _Type;
		private int _Width;
		private int _X, _Y;

		public GumpImageTileButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			GumpButtonType type,
			int param,
			int itemID,
			int hue,
			int width,
			int height,
			string name)
			: this(x, y, normalID, pressedID, -1, type, param, itemID, hue, width, height, null, -1, name)
		{ }

		public GumpImageTileButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			GumpButtonType type,
			int param,
			int itemID,
			int hue,
			int width,
			int height,
			GumpResponse callback,
			string name)
			: this(x, y, normalID, pressedID, -1, type, param, itemID, hue, width, height, callback, -1, name)
		{ }

		public GumpImageTileButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			GumpButtonType type,
			int param,
			int itemID,
			int hue,
			int width,
			int height,
			GumpResponse callback,
			int localizedTooltip,
			string name)
			: this(x, y, normalID, pressedID, -1, type, param, itemID, hue, width, height, callback, localizedTooltip, name)
		{ }

		public GumpImageTileButton(
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
			: this(x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height, null, -1, "")
		{ }

		public GumpImageTileButton(
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
			GumpResponse callback)
			: this(x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height, callback, -1, "")
		{ }

		public GumpImageTileButton(
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
			int localizedTooltip)
			: this(x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height, callback, localizedTooltip, "")
		{ }

		public GumpImageTileButton(
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
			int localizedTooltip,
			string name)
		{
			_X = x;
			_Y = y;
			_ID1 = normalID;
			_ID2 = pressedID;
			_EntryID = buttonID;
			_Type = type;
			_Param = param;

			_ItemID = itemID;
			_Hue = hue;
			_Width = width;
			_Height = height;

			_LocalizedTooltip = localizedTooltip;

			_Callback = callback;
			_Name = (name != null ? name : "");
		}

		public override int X { get { return _X; } set { Delta(ref _X, value); } }

		public override int Y { get { return _Y; } set { Delta(ref _Y, value); } }

		public int NormalID { get { return _ID1; } set { Delta(ref _ID1, value); } }

		public int PressedID { get { return _ID2; } set { Delta(ref _ID2, value); } }

		public int EntryID { get { return _EntryID; } set { Delta(ref _EntryID, value); } }

		//Legacy Support
		public int ButtonID { get { return EntryID; } }

		public GumpButtonType Type
		{
			get { return _Type; }
			set
			{
				if (_Type != value)
				{
					_Type = value;

					IGumpContainer parent = Container;

					if (parent != null)
					{
						parent.Invalidate();
					}
				}
			}
		}

		public int Param { get { return _Param; } set { Delta(ref _Param, value); } }

		public int ItemID { get { return _ItemID; } set { Delta(ref _ItemID, value); } }

		public int Hue { get { return _Hue; } set { Delta(ref _Hue, value); } }

		public int Width { get { return _Width; } set { Delta(ref _Width, value); } }

		public int Height { get { return _Height; } set { Delta(ref _Height, value); } }

		public int LocalizedTooltip { get { return _LocalizedTooltip; } set { Delta(ref _LocalizedTooltip, value); } }

		public string Name { get { return _Name; } set { Delta(ref _Name, value); } }

		public GumpResponse Callback { get { return _Callback; } set { Delta(ref _Callback, value); } }

		public void Invoke()
		{
			if (Callback != null)
			{
				Callback(this, null);
			}

			if (OnGumpResponse != null)
			{
				OnGumpResponse(this, null);
			}
		}

		public override string Compile()
		{
			return _LocalizedTooltip > 0
					   ? String.Format(
						   "{{ buttontileart {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} }}{{ tooltip {11} }}",
						   _X,
						   _Y,
						   _ID1,
						   _ID2,
						   (int)_Type,
						   _Param,
						   _EntryID,
						   _ItemID,
						   _Hue,
						   _Width,
						   _Height,
						   _LocalizedTooltip)
					   : String.Format(
						   "{{ buttontileart {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} }}",
						   _X,
						   _Y,
						   _ID1,
						   _ID2,
						   (int)_Type,
						   _Param,
						   _EntryID,
						   _ItemID,
						   _Hue,
						   _Width,
						   _Height);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_LayoutName);
			disp.AppendLayout(_X);
			disp.AppendLayout(_Y);
			disp.AppendLayout(_ID1);
			disp.AppendLayout(_ID2);
			disp.AppendLayout((int)_Type);
			disp.AppendLayout(_Param);
			disp.AppendLayout(_EntryID);

			disp.AppendLayout(_ItemID);
			disp.AppendLayout(_Hue);
			disp.AppendLayout(_Width);
			disp.AppendLayout(_Height);

			if (_LocalizedTooltip > 0)
			{
				disp.AppendLayout(_LayoutTooltip);
				disp.AppendLayout(_LocalizedTooltip);
			}
		}
	}
}