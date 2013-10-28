#region Header
// **********
// ServUO - GumpTextEntry.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpTextEntry : GumpEntry, IInputEntry
	{
		public event GumpResponse OnGumpResponse;

		private static readonly byte[] _LayoutName = Gump.StringToBuffer("textentry");
		private GumpResponse _Callback;
		private int _EntryID;
		private int _Height;
		private int _Hue;
		private string _InitialText;
		private string _Name;
		private int _Width;
		private int _X, _Y;

		public GumpTextEntry(int x, int y, int width, int height, int hue, string initialText, string name)
			: this(x, y, width, height, hue, -1, initialText, null, name)
		{ }

		public GumpTextEntry(
			int x, int y, int width, int height, int hue, string initialText, GumpResponse callback, string name)
			: this(x, y, width, height, hue, -1, initialText, callback, name)
		{ }

		public GumpTextEntry(int x, int y, int width, int height, int hue, int entryID, string initialText)
			: this(x, y, width, height, hue, entryID, initialText, null, "")
		{ }

		public GumpTextEntry(
			int x, int y, int width, int height, int hue, int entryID, string initialText, GumpResponse callback)
			: this(x, y, width, height, hue, entryID, initialText, callback, "")
		{ }

		public GumpTextEntry(
			int x, int y, int width, int height, int hue, int entryID, string initialText, GumpResponse callback, string name)
		{
			_X = x;
			_Y = y;
			_Width = width;
			_Height = height;
			_Hue = hue;
			_EntryID = entryID;
			_InitialText = initialText;
			_Callback = callback;
			_Name = (name != null ? name : "");
		}

		public override int X { get { return _X; } set { Delta(ref _X, value); } }

		public override int Y { get { return _Y; } set { Delta(ref _Y, value); } }

		public int Width { get { return _Width; } set { Delta(ref _Width, value); } }

		public int Height { get { return _Height; } set { Delta(ref _Height, value); } }

		public int Hue { get { return _Hue; } set { Delta(ref _Hue, value); } }

		public int EntryID { get { return _EntryID; } set { Delta(ref _EntryID, value); } }

		public string InitialText { get { return _InitialText; } set { Delta(ref _InitialText, value); } }

		public string Name { get { return _Name; } set { Delta(ref _Name, value); } }

		public GumpResponse Callback { get { return _Callback; } set { Delta(ref _Callback, value); } }

		public void Invoke()
		{
			if (Callback != null)
			{
				Callback(this, InitialText);
			}

			if (OnGumpResponse != null)
			{
				OnGumpResponse(this, InitialText);
			}
		}

		public override string Compile()
		{
			return String.Format(
				"{{ textentry {0} {1} {2} {3} {4} {5} {6} }}",
				_X,
				_Y,
				_Width,
				_Height,
				_Hue,
				_EntryID,
				Container.RootParent.Intern(_InitialText));
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_LayoutName);
			disp.AppendLayout(_X);
			disp.AppendLayout(_Y);
			disp.AppendLayout(_Width);
			disp.AppendLayout(_Height);
			disp.AppendLayout(_Hue);
			disp.AppendLayout(_EntryID);
			disp.AppendLayout(Container.RootParent.Intern(_InitialText));

			disp.TextEntries++;
		}
	}
}