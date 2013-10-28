#region Header
// **********
// ServUO - Gump.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public delegate void GumpResponse(IGumpComponent sender, object param);

	public interface IInputEntry
	{
		int EntryID { get; set; }
		string Name { get; set; }
		GumpResponse Callback { get; set; }

		void Invoke();
	}

	public interface IGumpContainer
	{
		Gump RootParent { get; }
		int X { get; set; }
		int Y { get; set; }

		void Add(IGumpComponent g);
		void Remove(IGumpComponent g);

		void Invalidate();
	}

	public interface IGumpComponent
	{
		IGumpContainer Container { get; set; }
		int X { get; set; }
		int Y { get; set; }
	}

	public partial class Gump : IGumpContainer
	{
		private static readonly byte[] _BeginLayout = StringToBuffer("{ ");
		private static readonly byte[] _EndLayout = StringToBuffer(" }");
		private static readonly byte[] _NoMove = StringToBuffer("{ nomove }");
		private static readonly byte[] _NoClose = StringToBuffer("{ noclose }");
		private static readonly byte[] _NoDispose = StringToBuffer("{ nodispose }");
		private static readonly byte[] _NoResize = StringToBuffer("{ noresize }");
		private static int _NextSerial = 1;
		private readonly List<GumpEntry> _Entries;
		private readonly List<string> _Strings;
		private readonly int _TypeID;
		private readonly List<int> _UsedIDs = new List<int>();
		private List<Mobile> _Users = new List<Mobile>();
		private List<Mobile> _Viewers = new List<Mobile>();
		private Mobile _Address;
		private bool _Closable = true;
		private bool _Disposable = true;
		private bool _Dragable = true;
		//private Mobile _Hijacker;
		private bool _MacroProtection;
		private int _NewID = 1;
		private bool _Resizable = true;
		private int _Serial;
		private bool _SharedGump;
		internal int _Switches;
		internal int _TextEntries;
		private int _X, _Y;

		public Gump(int x, int y)
		{
			do
			{
				_Serial = _NextSerial++;
			}
			while (_Serial == 0);

			_X = x;
			_Y = y;

			_TypeID = GetTypeID(GetType());

			_Entries = new List<GumpEntry>();
			_Strings = new List<string>();
		}

		public Gump(int x, int y, List<Mobile> users)
			: this(x, y)
		{
			if (users == null)
			{
				return;
			}

			_Users = users;
			_SharedGump = true;
		}

		public int TypeID { get { return _TypeID; } }

		public List<GumpEntry> Entries { get { return _Entries; } }

		public int Serial
		{
			get { return _Serial; }
			set
			{
				if (_Serial == value)
				{
					return;
				}

				_Serial = value;
				Invalidate();
			}
		}

		public int X
		{
			get { return _X; }
			set
			{
				if (_X == value)
				{
					return;
				}

				_X = value;
				Invalidate();
			}
		}

		public int Y
		{
			get { return _Y; }
			set
			{
				if (_Y == value)
				{
					return;
				}

				_Y = value;
				Invalidate();
			}
		}

		public Mobile Address
		{
			get { return _Address; }
			set
			{
				if (_Address == value)
				{
					return;
				}

				_Address = value;
				Invalidate();
				OnAddressChange();
			}
		}

		public bool Disposable
		{
			get { return _Disposable; }
			set
			{
				if (_Disposable == value)
				{
					return;
				}

				_Disposable = value;
				Invalidate();
			}
		}

		public bool Resizable
		{
			get { return _Resizable; }
			set
			{
				if (_Resizable == value)
				{
					return;
				}

				_Resizable = value;
				Invalidate();
			}
		}

		public bool MacroProtection
		{
			get { return _MacroProtection; }
			set
			{
				if (_MacroProtection == value)
				{
					return;
				}

				_MacroProtection = value;
				Invalidate();
			}
		}

		public bool SharedGump
		{
			get { return _SharedGump; }
			set
			{
				if (_SharedGump == value)
				{
					return;
				}

				_SharedGump = value;
				Invalidate();
			}
		}

		public bool Dragable
		{
			get { return _Dragable; }
			set
			{
				if (_Dragable == value)
				{
					return;
				}

				_Dragable = value;
				Invalidate();
			}
		}

		public bool Closable
		{
			get { return _Closable; }
			set
			{
				if (_Closable == value)
				{
					return;
				}

				_Closable = value;
				Invalidate();
			}
		}

		public static int GetTypeID(Type type)
		{
			return type.FullName.GetHashCode();
		}

		public static byte[] StringToBuffer(string str)
		{
			return Encoding.ASCII.GetBytes(str);
		}

		public Gump RootParent { get { return this; } }

		public virtual void Invalidate()
		{ }

		public virtual void OnAddressChange()
		{ }

		public bool AddUser(Mobile from)
		{
			Invalidate();

			if (_Users == null)
			{
				_Users = new List<Mobile>();
			}

			if (_Users.Contains(from))
			{
				return false;
			}

			_Users.Add(from);

			_SharedGump = true;

			return true;
		}

		public void AddPage(int page)
		{
			Add(new GumpPage(page));
		}

		public void AddAlphaRegion(int x, int y, int width, int height)
		{
			Add(new GumpAlphaRegion(x, y, width, height));
		}

		public void AddBackground(int x, int y, int width, int height, int gumpID)
		{
			Add(new GumpBackground(x, y, width, height, gumpID));
		}

		public void AddGroup(int group)
		{
			Add(new GumpGroup(group));
		}

		public void AddTooltip(int number)
		{
			Add(new GumpTooltip(number));
		}

		public void AddItemProperty(int serial)
		{
			Add(new GumpItemProperty(serial));
		}

		public void AddHtml(int x, int y, int width, int height, string text, bool background, bool scrollbar)
		{
			Add(new GumpHtml(x, y, width, height, text, background, scrollbar));
		}

		public void AddHtmlLocalized(int x, int y, int width, int height, int number, bool background, bool scrollbar)
		{
			Add(new GumpHtmlLocalized(x, y, width, height, number, background, scrollbar));
		}

		public void AddHtmlLocalized(
			int x, int y, int width, int height, int number, int color, bool background, bool scrollbar)
		{
			Add(new GumpHtmlLocalized(x, y, width, height, number, color, background, scrollbar));
		}

		public void AddHtmlLocalized(
			int x, int y, int width, int height, int number, string args, int color, bool background, bool scrollbar)
		{
			Add(new GumpHtmlLocalized(x, y, width, height, number, args, color, background, scrollbar));
		}

		public void AddImage(int x, int y, int gumpID)
		{
			Add(new GumpImage(x, y, gumpID));
		}

		public void AddImage(int x, int y, int gumpID, int hue)
		{
			Add(new GumpImage(x, y, gumpID, hue));
		}

		public void AddImageTiled(int x, int y, int width, int height, int gumpID)
		{
			Add(new GumpImageTiled(x, y, width, height, gumpID));
		}

		public void AddItem(int x, int y, int itemID)
		{
			Add(new GumpItem(x, y, itemID));
		}

		public void AddItem(int x, int y, int itemID, int hue)
		{
			Add(new GumpItem(x, y, itemID, hue));
		}

		public void AddLabel(int x, int y, int hue, string text)
		{
			Add(new GumpLabel(x, y, hue, text));
		}

		public void AddLabelCropped(int x, int y, int width, int height, int hue, string text)
		{
			Add(new GumpLabelCropped(x, y, width, height, hue, text));
		}

		public void Add(IGumpComponent g)
		{
			if (g.Container == null)
			{
				g.Container = this;
			}

			if (g is GumpEntry)
			{
				if (!_Entries.Contains((GumpEntry)g))
				{
					((GumpEntry)g).AssignID();
					_Entries.Add((GumpEntry)g);
					Invalidate();
				}
			}
			else if (g is Gumpling)
			{
				((Gumpling)g).AddToGump(this);
			}
		}

		public void Remove(IGumpComponent g)
		{
			if (g is GumpEntry)
			{
				_Entries.Remove((GumpEntry)g);
				g.Container = null;
				Invalidate();
			}
			else if (g is Gumpling)
			{
				((Gumpling)g).RemoveFromGump(this);
				Invalidate();
			}
		}

		// No need to store used ID's, they are guaranteed to be unique.
		// Down-side is not all 'id' values from 0 to 65535 can be used.
		// Optimization like this is necessary for best gump refresh rate;
		// The goal is to keep the entire rendering of a gump to less than 100ms.
		protected internal int NewIDFast()
		{
			int id = _NewID +
					 (_MacroProtection ? Utility.RandomMinMax(Utility.RandomMinMax(1, 5), Utility.RandomMinMax(5, 10)) : 1);

			return _NewID = id;
		}

		protected internal int NewID()
		{
			int id;

			if (_MacroProtection)
			{
				id = Utility.RandomMinMax(1, 65535);

				if (_UsedIDs.Contains(id))
				{
					return NewID();
				}
			}
			else
			{
				id = _NewID;

				if (_UsedIDs.Contains(id))
				{
					_NewID++;
					return NewID();
				}
			}

			_UsedIDs.Add(id);

			return id;
		}

		public int Intern(string value)
		{
			int indexOf = _Strings.IndexOf(value);

			if (indexOf >= 0)
			{
				return indexOf;
			}

			Invalidate();
			_Strings.Add(value);
			return _Strings.Count - 1;
		}

		public void SendTo(NetState state)
		{
			Address = state.Mobile;

			if (!_SharedGump)
			{
				state.AddGump(this);
				state.Send(Compile(state));
			}
			else
			{
				foreach (Mobile m in _Users)
				{
					m.NetState.AddGump(this);
					m.NetState.Send(Compile(state));
				}

				foreach (Mobile m in _Viewers)
				{
					m.NetState.AddGump(this);
					m.NetState.Send(Compile(state, true));
				}
			}
		}

		public virtual void OnResponse(NetState sender, RelayInfo info)
		{
			int buttonID = info.ButtonID;

			foreach (GumpCheck entry in _Entries.OfType<GumpCheck>())
			{
				entry.InitialState = info.IsSwitched(entry.EntryID);
				entry.Invoke();
			}

			foreach (GumpRadio entry in Entries.OfType<GumpRadio>())
			{
				entry.InitialState = info.IsSwitched(entry.EntryID);
				entry.Invoke();
			}

			foreach (GumpTextEntry entry in _Entries.OfType<GumpTextEntry>())
			{
				entry.InitialText = info.GetTextEntry(entry.EntryID).Text;
				entry.Invoke();
			}

			foreach (GumpTextEntryLimited entry in _Entries.OfType<GumpTextEntryLimited>())
			{
				entry.InitialText = info.GetTextEntry(entry.EntryID).Text;
				entry.Invoke();
			}

			foreach (GumpImageTileButton button in
				_Entries.OfType<GumpImageTileButton>().Where(button => button.EntryID == buttonID))
			{
				button.Invoke();
			}

			foreach (GumpButton button in _Entries.OfType<GumpButton>().Where(button => button.EntryID == buttonID))
			{
				button.Invoke();
			}
		}

		public virtual void OnServerClose(NetState owner)
		{ }

		protected Packet Compile()
		{
			return Compile(null);
		}

		protected Packet Compile(NetState ns, bool convertToViewer = false)
		{
			IGumpWriter disp;

			if (ns != null && ns.Unpack)
			{
				disp = new DisplayGumpPacked(this);
			}
			else
			{
				disp = new DisplayGumpFast(this);
			}

			if (!_Dragable)
			{
				disp.AppendLayout(_NoMove);
			}

			if (!_Closable)
			{
				disp.AppendLayout(_NoClose);
			}

			if (!_Disposable)
			{
				disp.AppendLayout(_NoDispose);
			}

			if (!_Resizable)
			{
				disp.AppendLayout(_NoResize);
			}

			int count = _Entries.Count;

			for (int i = 0; i < count; ++i)
			{
				GumpEntry e = _Entries[i];

				disp.AppendLayout(_BeginLayout);

				if (!convertToViewer)
				{
					e.AppendTo(disp);
				}
				else
				{
					GumpButton button = e as GumpButton;

					if (button != null)
					{
						new GumpImage(button.X, button.Y, button.NormalID).AppendTo(disp);
					}
					else
					{
						GumpImageTileButton tileButton = e as GumpImageTileButton;

						if (tileButton != null)
						{
							new GumpImageTiled(tileButton.X, tileButton.Y, tileButton.Width, tileButton.Height, tileButton.NormalID).AppendTo
								(disp);
						}
						else
						{
							GumpRadio radio = e as GumpRadio;

							if (radio != null)
							{
								new GumpImage(radio.X, radio.Y, radio.InitialState ? radio.ActiveID : radio.InactiveID).AppendTo(disp);
							}
							else
							{
								GumpCheck check = e as GumpCheck;

								if (check != null)
								{
									new GumpImage(check.X, check.Y, check.InitialState ? check.ActiveID : check.InactiveID).AppendTo(disp);
								}
								// Process text fields
							}
						}
					}
				}

				disp.AppendLayout(_EndLayout);
			}

			disp.WriteStrings(_Strings);

			disp.Flush();

			_TextEntries = disp.TextEntries;
			_Switches = disp.Switches;

			return disp as Packet;
		}
	}
}