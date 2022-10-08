#region References
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Server.Network;
#endregion

namespace Server.Gumps
{
	[Flags]
	public enum GumpFlags
	{
		None = 0,

		Disposable = 1 << 0,
		Resizable = 1 << 1,
		Dragable = 1 << 2,
		Closable = 1 << 3,

		Default = Disposable | Resizable | Dragable | Closable,

		All = ~None
	}

	public class Gump
	{
		private static readonly Dictionary<Type, int> m_TypeCodes = new Dictionary<Type, int>(0x100);

		public static int GetTypeID(Type type)
		{
			if (!m_TypeCodes.TryGetValue(type, out var id))
			{
				unchecked
				{
					id = 0x1337;

					var name = type.FullName;

					for (var i = 0; i < name.Length; i++)
					{
						id = (id * 397) ^ name[i];
					}
				}

				m_TypeCodes[type] = id;
			}

			return id;
		}

		private readonly List<string> m_Strings;

		internal int m_TextEntries, m_Switches;

		private static int m_NextSerial = 1;

		public List<GumpEntry> Entries { get; }

		private int m_TypeID;

		public int TypeID
		{
			get => m_TypeID;
			set
			{
				if (m_TypeID != value)
				{
					m_TypeID = value;

					Invalidate();
				}
			}
		}

		private int m_Serial;

		public int Serial
		{
			get => m_Serial;
			set
			{
				if (m_Serial != value)
				{
					m_Serial = value;

					Invalidate();
				}
			}
		}

		private int m_X;

		public int X
		{
			get => m_X;
			set
			{
				if (m_X != value)
				{
					m_X = value;

					Invalidate();
				}
			}
		}

		private int m_Y;

		public int Y
		{
			get => m_Y;
			set
			{
				if (m_Y != value)
				{
					m_Y = value;

					Invalidate();
				}
			}
		}

		private GumpFlags m_Flags;

		public GumpFlags Flags
		{
			get => m_Flags;
			set
			{
				if (m_Flags != value)
				{
					m_Flags = value;

					Invalidate();
				}
			}
		}

		public bool Disposable { get => GetFlag(GumpFlags.Disposable); set => SetFlag(GumpFlags.Disposable, value); }
		public bool Resizable { get => GetFlag(GumpFlags.Resizable); set => SetFlag(GumpFlags.Resizable, value); }
		public bool Dragable { get => GetFlag(GumpFlags.Dragable); set => SetFlag(GumpFlags.Dragable, value); }
		public bool Closable { get => GetFlag(GumpFlags.Closable); set => SetFlag(GumpFlags.Closable, value); }

		public Gump(int x, int y)
		{
			do
			{
				m_Serial = m_NextSerial++;
			}
			while (m_Serial == 0); // standard client apparently doesn't send a gump response packet if serial == 0

			m_Flags = GumpFlags.Default;

			m_X = x;
			m_Y = y;

			TypeID = GetTypeID(GetType());

			Entries = new List<GumpEntry>();
			m_Strings = new List<string>();
		}

		public virtual int GetTypeID()
		{
			return GetTypeID(GetType());
		}

		public virtual void Invalidate()
		{
			ReleasePackets();
		}

		public bool GetFlag(GumpFlags flag)
		{
			return (Flags & flag) != 0;
		}

		public void SetFlag(GumpFlags flag, bool state)
		{
			if (state)
			{
				Flags |= flag;
			}
			else
			{
				Flags &= ~flag;
			}
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

		public void AddButton(int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param)
		{
			Add(new GumpButton(x, y, normalID, pressedID, buttonID, type, param));
		}

		public void AddCheck(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
		{
			Add(new GumpCheck(x, y, inactiveID, activeID, initialState, switchID));
		}

		public void AddGroup(int group)
		{
			Add(new GumpGroup(group));
		}

		public void AddTooltip(int number)
		{
			Add(new GumpTooltip(number));
		}

		public void AddTooltip(int number, string args)
		{
			Add(new GumpTooltip(number, args));
		}

		public void AddTooltip(string text)
		{
			Add(new GumpTooltip(1042971, text));
		}

		public void AddHtml(int x, int y, int width, int height, string text, bool background, bool scrollbar)
		{
			if (!scrollbar && !background && height < 40 && !Insensitive.Contains(text, "BODYBGCOLOR"))
			{
				height = 40;
			}

			Add(new GumpHtml(x, y, width, height, text, background, scrollbar));
		}

		public void AddHtmlIntern(int x, int y, int width, int height, int textid, bool background, bool scrollbar)
		{
			if (!scrollbar && !background && height < 40 && textid >= 0 && textid < m_Strings.Count && !Insensitive.Contains(m_Strings[textid], "BODYBGCOLOR"))
			{
				height = 40;
			}

			Add(new GumpHtml(x, y, width, height, textid, background, scrollbar));
		}

		public void AddHtmlLocalized(int x, int y, int width, int height, int number, bool background, bool scrollbar)
		{
			if (!scrollbar && !background && height < 40)
			{
				height = 40;
			}

			Add(new GumpHtmlLocalized(x, y, width, height, number, background, scrollbar));
		}

		public void AddHtmlLocalized(int x, int y, int width, int height, int number, int color, bool background, bool scrollbar)
		{
			if (!scrollbar && !background && height < 40)
			{
				height = 40;
			}

			Add(new GumpHtmlLocalized(x, y, width, height, number, color, background, scrollbar));
		}

		public void AddHtmlLocalized(int x, int y, int width, int height, int number, string args, int color, bool background, bool scrollbar)
		{
			if (!scrollbar && !background && height < 40)
			{
				height = 40;
			}

			Add(new GumpHtmlLocalized(x, y, width, height, number, args, color, background, scrollbar));
		}

		public void AddSpriteImage(int x, int y, int gumpID, int width, int height, int sx, int sy)
		{
			Add(new GumpSpriteImage(x, y, gumpID, width, height, sx, sy));
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

		public void AddImageTiledButton(int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param, int itemID, int hue, int width, int height)
		{
			Add(new GumpImageTileButton(x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height));
		}

		public void AddImageTiledButton(int x, int y, int normalID, int pressedID, int buttonID, GumpButtonType type, int param, int itemID, int hue, int width, int height, int localizedTooltip)
		{
			Add(new GumpImageTileButton(x, y, normalID, pressedID, buttonID, type, param, itemID, hue, width, height, localizedTooltip));
		}

		public void AddItem(int x, int y, int itemID)
		{
			Add(new GumpItem(x, y, itemID));
		}

		public void AddItem(int x, int y, int itemID, int hue)
		{
			Add(new GumpItem(x, y, itemID, hue));
		}

		public void AddLabelIntern(int x, int y, int hue, int textid)
		{
			Add(new GumpLabel(x, y, hue, textid));
		}

		public void AddLabel(int x, int y, int hue, string text)
		{
			Add(new GumpLabel(x, y, hue, text));
		}

		public void AddLabelCropped(int x, int y, int width, int height, int hue, string text)
		{
			Add(new GumpLabelCropped(x, y, width, height, hue, text));
		}

		public void AddLabelCroppedIntern(int x, int y, int width, int height, int hue, int textid)
		{
			Add(new GumpLabelCropped(x, y, width, height, hue, textid));
		}

		public void AddRadio(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
		{
			Add(new GumpRadio(x, y, inactiveID, activeID, initialState, switchID));
		}

		public void AddTextEntry(int x, int y, int width, int height, int hue, int entryID, string initialText)
		{
			Add(new GumpTextEntry(x, y, width, height, hue, entryID, initialText));
		}

		public void AddTextEntry(int x, int y, int width, int height, int hue, int entryID, string initialText, int size)
		{
			Add(new GumpTextEntryLimited(x, y, width, height, hue, entryID, initialText, size));
		}

		public void AddTextEntryIntern(int x, int y, int width, int height, int hue, int entryID, int initialTextID)
		{
			Add(new GumpTextEntry(x, y, width, height, hue, entryID, initialTextID));
		}

		public void AddItemProperty(Item item)
		{
			Add(new GumpItemProperty(item.Serial.Value));
		}

		public void AddItemProperty(int serial)
		{
			Add(new GumpItemProperty(serial));
		}

		public void AddECHandleInput()
		{
			Add(new ECHandleInput());
		}

		public void Add(GumpEntry g)
		{
			if (g.Parent != this)
			{
				g.Parent = this;
			}
			else if (!Entries.Contains(g))
			{
				Invalidate();

				Entries.Add(g);
			}
		}

		public void Remove(GumpEntry g)
		{
			if (g == null || !Entries.Remove(g))
			{
				return;
			}

			Invalidate();

			g.Parent = null;
		}

		public int Intern(string value)
		{
			return Intern(value, true);
		}

		public int Intern(string value, bool enforceUnique)
		{
			if (enforceUnique)
			{
				var indexOf = m_Strings.IndexOf(value);

				if (indexOf >= 0)
				{
					return indexOf;
				}
			}

			m_Strings.Add(value);

			return m_Strings.Count - 1;
		}

		public void SendTo(NetState state)
		{
			state.AddGump(this);

			state.Send(GetPacketFor(state));
		}

		public static byte[] StringToBuffer(string str)
		{
			return Encoding.ASCII.GetBytes(str);
		}

		private static readonly byte[] m_BeginLayout = StringToBuffer("{ ");
		private static readonly byte[] m_EndLayout = StringToBuffer(" }");

		private static readonly byte[] m_NoMove = StringToBuffer("{ nomove }");
		private static readonly byte[] m_NoClose = StringToBuffer("{ noclose }");
		private static readonly byte[] m_NoDispose = StringToBuffer("{ nodispose }");
		private static readonly byte[] m_NoResize = StringToBuffer("{ noresize }");

		public virtual Packet GetPacketFor(NetState ns)
		{
			return OpenPacket;
		}

		private DisplayGumpPacked m_OpenPacket;

		public DisplayGumpPacked OpenPacket => GetOpenPacket();

		[MethodImpl(MethodImplOptions.Synchronized)]
		private DisplayGumpPacked GetOpenPacket()
		{
			if (m_OpenPacket == null)
			{
				var disp = new DisplayGumpPacked(this);

				if (!Dragable)
				{
					disp.AppendLayout(m_NoMove);
				}

				if (!Closable)
				{
					disp.AppendLayout(m_NoClose);
				}

				if (!Disposable)
				{
					disp.AppendLayout(m_NoDispose);
				}

				if (!Resizable)
				{
					disp.AppendLayout(m_NoResize);
				}

				var count = Entries.Count;

				GumpEntry e;

				for (var i = 0; i < count; ++i)
				{
					e = Entries[i];

					disp.AppendLayout(m_BeginLayout);
					e.AppendTo(disp);
					disp.AppendLayout(m_EndLayout);
				}

				disp.WriteStrings(m_Strings);

				disp.Flush();
				disp.SetStatic();

				m_OpenPacket = disp;

				m_TextEntries = disp.TextEntries;
				m_Switches = disp.Switches;
			}

			return m_OpenPacket;
		}

		private CloseGump m_ClosePacket;

		public CloseGump ClosePacket => GetClosePacket();

		[MethodImpl(MethodImplOptions.Synchronized)]
		private CloseGump GetClosePacket()
		{
			if (m_ClosePacket == null)
			{
				var close = new CloseGump(m_TypeID, 0);

				close.SetStatic();

				m_ClosePacket = close;
			}

			return m_ClosePacket;
		}

		public virtual void ReleasePackets()
		{
			Packet.Release(ref m_OpenPacket);
			Packet.Release(ref m_ClosePacket);
		}

		public virtual void OnResponse(NetState sender, RelayInfo info)
		{ }

		public virtual void OnServerClose(NetState owner)
		{ }
	}
}
