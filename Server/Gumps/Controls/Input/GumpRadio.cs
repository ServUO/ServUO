#region Header
// **********
// ServUO - GumpRadio.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpRadio : GumpEntry, IInputEntry
	{
		public event GumpResponse OnGumpResponse;

		private static readonly byte[] _LayoutName = Gump.StringToBuffer("radio");
		private GumpResponse _Callback;
		private int _ID1, _ID2;
		private bool _InitialState;
		private string _Name;
		private int _EntryID;
		private int _X, _Y;

		public GumpRadio(int x, int y, int inactiveID, int activeID, bool initialState, string name)
			: this(x, y, inactiveID, activeID, initialState, -1, null, name)
		{ }

		public GumpRadio(int x, int y, int inactiveID, int activeID, bool initialState, GumpResponse callback, string name)
			: this(x, y, inactiveID, activeID, initialState, -1, callback, name)
		{ }

		public GumpRadio(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
			: this(x, y, inactiveID, activeID, initialState, switchID, null, "")
		{ }

		public GumpRadio(int x, int y, int inactiveID, int activeID, bool initialState, int switchID, GumpResponse callback)
			: this(x, y, inactiveID, activeID, initialState, switchID, callback, "")
		{ }

		public GumpRadio(
			int x, int y, int inactiveID, int activeID, bool initialState, int switchID, GumpResponse callback, string name)
		{
			_X = x;
			_Y = y;
			_ID1 = inactiveID;
			_ID2 = activeID;
			_InitialState = initialState;
			_EntryID = switchID;
			_Callback = callback;
			_Name = (name != null ? name : "");
		}

		public override int X { get { return _X; } set { Delta(ref _X, value); } }

		public override int Y { get { return _Y; } set { Delta(ref _Y, value); } }

		public int InactiveID { get { return _ID1; } set { Delta(ref _ID1, value); } }

		public int ActiveID { get { return _ID2; } set { Delta(ref _ID2, value); } }

		public bool InitialState { get { return _InitialState; } set { Delta(ref _InitialState, value); } }

		public int EntryID { get { return _EntryID; } set { Delta(ref _EntryID, value); } }

		//Legacy Support
		public int SwitchID { get { return EntryID; } }

		public string Name { get { return _Name; } set { Delta(ref _Name, value); } }

		public GumpResponse Callback { get { return _Callback; } set { Delta(ref _Callback, value); } }

		public void Invoke()
		{
			if (Callback != null)
			{
				Callback(this, InitialState);
			}

			if (OnGumpResponse != null)
			{
				OnGumpResponse(this, InitialState);
			}
		}

		public override string Compile()
		{
			return String.Format("{{ radio {0} {1} {2} {3} {4} {5} }}", _X, _Y, _ID1, _ID2, _InitialState ? 1 : 0, _EntryID);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_LayoutName);
			disp.AppendLayout(_X);
			disp.AppendLayout(_Y);
			disp.AppendLayout(_ID1);
			disp.AppendLayout(_ID2);
			disp.AppendLayout(_InitialState);
			disp.AppendLayout(_EntryID);

			disp.Switches++;
		}
	}
}