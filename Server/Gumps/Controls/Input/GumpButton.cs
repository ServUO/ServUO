#region Header
// **********
// ServUO - GumpButton.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public enum GumpButtonType
	{
		Page = 0,
		Reply = 1
	}

	public class GumpButton : GumpEntry, IInputEntry
	{
		public event GumpResponse OnGumpResponse;

		private static readonly byte[] _LayoutName = Gump.StringToBuffer("button");
		private int _EntryID;
		private GumpResponse _Callback;
		private int _ID1, _ID2;
		private string _Name;
		private int _Param;
		private GumpButtonType _Type;
		private int _X, _Y;

		public GumpButton(int x, int y, int normalID, int pressedID, GumpButtonType type, int param, string name)
			: this(x, y, normalID, pressedID, -1, type, param, null, name)
		{ }

		public GumpButton(
			int x, int y, int normalID, int pressedID, GumpButtonType type, int param, GumpResponse callback, string name)
			: this(x, y, normalID, pressedID, -1, type, param, callback, name)
		{ }

		public GumpButton(int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param)
			: this(x, y, normalID, pressedID, buttonID, type, param, null, "")
		{ }

		public GumpButton(
			int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param, GumpResponse callback)
			: this(x, y, normalID, pressedID, buttonID, type, param, callback, "")
		{ }

		public GumpButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int buttonID,
			GumpButtonType type,
			int param,
			GumpResponse callback,
			string name)
		{
			_X = x;
			_Y = y;
			_ID1 = normalID;
			_ID2 = pressedID;
			_EntryID = buttonID;
			_Type = type;
			_Param = param;
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
				if (_Type == value)
				{
					return;
				}

				_Type = value;
				IGumpContainer parent = Container;

				if (parent != null)
				{
					parent.Invalidate();
				}
			}
		}

		public int Param { get { return _Param; } set { Delta(ref _Param, value); } }

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
			return String.Format("{{ button {0} {1} {2} {3} {4} {5} {6} }}", _X, _Y, _ID1, _ID2, (int)_Type, _Param, _EntryID);
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
		}
	}
}