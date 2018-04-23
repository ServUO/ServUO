//CEO's Moongate Library V1.1
using System;
using System.IO;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{

	public class MoongateLibrary : Item
	{

		private bool m_Active;
		private AccessLevel m_AccessLevel;
		private DateTime m_LastChanged;
		private ArrayList m_RuneBooks = new ArrayList();
		public Mobile m_Owner;
		public bool m_RedsAnywhere = true;
		private int m_Cost = 25;
		private long m_AmountCollected;

		// DecayTimer Stuff
		private bool m_Decays = true;
		private DateTime m_DecayEnd;
		private TimeSpan m_DecayTime = TimeSpan.FromDays(90);
		private DecayTimer m_DecayTimer = null;
		private bool m_DecayRunning = false;

		[CommandProperty(AccessLevel.Counselor)]
		public Mobile MobOwner
		{
			get
			{
				return m_Owner;
			}
			set
			{
				if (value == null)
				{
					m_Owner = null;
					UpdateDate();
				}
				else if (value is PlayerMobile)
				{
					m_Owner = value;
					UpdateDate();
				}
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public bool Active
		{
			get
			{
				return m_Active;
			}
			set
			{
				if (m_Active == value)
					return;
				if (value && m_RuneBooks != null && m_RuneBooks.Count > 0)
				{
					m_Active = value;
					if (m_RuneBooks != null && m_RuneBooks.Count > 0)
					{
						Runebook runebook = m_RuneBooks[0] as Runebook;
						Hue = runebook.Hue == 1121 ? 58 : runebook.Hue;
					}
					else
						Hue = 58;
					Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 1481);
					Effects.SendLocationEffect(new Point3D(this.X, this.Y + 1, this.Z), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.SendLocationEffect(new Point3D(this.X + 1, this.Y, this.Z), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.SendLocationEffect(new Point3D(this.X, this.Y, this.Z - 1), this.Map, 0x373A, 15, this.Hue - 1, 0);
					this.PublicOverheadMessage(0, this.Hue, false, "CEOMoongateLibrary Active!");
				}
				else
				{
					m_Active = false;
					Hue = 826;
					this.PublicOverheadMessage(0, this.Hue, false, "CEOMoongateLibrary Inactive.");
				}
				UpdateDate();
			}
		}

		[CommandProperty(AccessLevel.Counselor)]
		public AccessLevel Security
		{
			get
			{
				return m_AccessLevel;
			}
			set
			{
				if ((value <= AccessLevel.Administrator) && (value >= AccessLevel.Player))
				{
					m_AccessLevel = value;
					UpdateDate();
				}
				else
				{
					return;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Cost
		{
			get { return m_Cost; }
			set
			{
				if (value < 0)
					m_Cost = 0;
				else if (value > 5000)
					m_Cost = 5000;
				else
					m_Cost = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long Collected
		{
			get { return m_AmountCollected; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool DecayRunning
		{
			get { return m_DecayRunning; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DecayTimer1
		{
			get { return m_DecayTime; }
			set { m_DecayTime = value; if (m_Decays && m_DecayTime != TimeSpan.FromSeconds(0)) DoDecayTimer(value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GateDecays
		{
			get { return m_Decays; }
			set
			{
				m_Decays = value;
				if (m_Decays && (m_DecayTime != TimeSpan.FromSeconds(0))) DoDecayTimer(m_DecayTime);
				if (!m_Decays)
				{
					if (m_DecayRunning)
						m_DecayRunning = false;
					if (m_DecayTimer != null)
						m_DecayTimer.Stop();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DecayOver
		{
			get
			{
				if (m_DecayRunning)
					return m_DecayEnd - DateTime.Now;
				else
					return TimeSpan.FromSeconds(0);
			}
			set
			{
				if (!m_Decays)
					m_Decays = true;
				DoDecayTimer(value);
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public DateTime LastChanged
		{
			get
			{
				return m_LastChanged;
			}
			set
			{
				m_LastChanged = value;
			}
		}

		[Constructable]
		public MoongateLibrary(Mobile owner, Point3D loc, bool active, ArrayList runebooks, bool decays, TimeSpan decaytime, TimeSpan timeleft, int cost, long amountcollected)
			: base(0xF6C)
		{
			Movable = false;
			Name = "Moongate Library";
			m_DecayRunning = false;
			m_Owner = owner;
			m_RuneBooks = runebooks;
			m_Cost = cost;
			m_AmountCollected = amountcollected;
			m_Decays = decays;
			m_DecayTime = decaytime;
			m_AccessLevel = owner.AccessLevel;
			Light = LightType.Circle300;
			m_Active = active;
			RemoveDeleted();
			if (m_Active && m_RuneBooks != null && m_RuneBooks.Count > 0)
			{
				Runebook runebook = m_RuneBooks[0] as Runebook;
				Hue = runebook.Hue == 1121 ? 58 : runebook.Hue;
			}
			else
				Hue = 826;
			if (m_Decays)
				DoDecayTimer(timeleft);
			MoveToWorld(loc, owner.Map);
			Effects.PlaySound(this.Location, this.Map, 0x20E);
			UpdateDate();
		}

		[Constructable]
		public MoongateLibrary()
			: base(0xF6C)
		{
			Movable = false;
			Name = "Moongate Library";
			Light = LightType.Circle300;
			Hue = 826;
			m_AccessLevel = AccessLevel.GameMaster;
			m_Active = false;
			m_Decays = true;
			m_DecayRunning = false;
			Effects.PlaySound(this.Location, this.Map, 0x20E);
			UpdateDate();
		}

		public MoongateLibrary(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!from.Player)
				return;
			if (from.InRange(GetWorldLocation(), 1))
				UseGate(from);
			else
				from.SendLocalizedMessage(500446); // That is too far away.
		}

		public void AddToCollectedAmount(int amount)
		{
			m_AmountCollected += amount;
		}

		public bool Owner(Mobile m)
		{
			if (m_Owner == null || m_Owner.Deleted)
			{
				m_Owner = null;
				if (m.AccessLevel > AccessLevel.Player)
					return true;
				else
					return false;
			}
			if (m.Account == m_Owner.Account || m.AccessLevel > AccessLevel.Player)
				return true;
			return false;
		}

		public void ReDeed(Mobile m)
		{
			//bool active, ArrayList runebooks, bool decays, bool decayrunning, Timespan decaytimer, TimeSpan timeleft)
			try
			{
				m.AddToBackpack(new MoongateLibraryDeed(m_Active, m_RuneBooks, m_Decays, m_DecayTime, m_Decays ? m_DecayEnd - DateTime.Now : TimeSpan.FromSeconds(0), m_Cost, m_AmountCollected));
			}
			catch
			{
				m.SendMessage("There's a problem Re-Deeding the Moongate Library. Page for help.");
				return;
			}
			if (m_DecayRunning && m_DecayTimer != null)
				m_DecayTimer.Stop();
			Delete();
		}

		public override bool OnMoveOver(Mobile m)
		{
			return !m.Player || UseGate(m);
		}

		public void AddRunebook(Mobile m)
		{
			if (m_RuneBooks != null && m_RuneBooks.Count > 95) // 96 Maximum Runebooks
			{
				m.SendMessage("No more runebooks may be added to this Moongate Library!");
				SendLibraryGump(m, 0, 0, false);
			}
			else
			{
				m.Target = new InternalTarget(this, m_RuneBooks);
			}
		}

		public void UpdateDate()
		{
			m_LastChanged = DateTime.Now;
		}

		private class InternalTarget : Target
		{

			private ArrayList m_RuneBooks;
			private MoongateLibrary m_MoongateLibrary;

			public InternalTarget(MoongateLibrary moongatelibrary, ArrayList RuneBooks)
				: base(12, false, TargetFlags.None)
			{
				this.CheckLOS = true;
				this.AllowGround = false;
				this.Range = 15;
				m_MoongateLibrary = moongatelibrary;
				m_RuneBooks = RuneBooks;
			}

			protected override void OnTarget(Mobile from, object o)
			{
				if (o == null || !(o is Runebook))
				{
					from.SendMessage("Invalid target, must be a Runebook.");
				}
				else
				{
					if (validateRunebook(from, (Runebook)o))
					{
						m_RuneBooks.Add(o);
						Runebook orunebook = (Runebook)o;
						if (m_RuneBooks.Count == 1 && m_MoongateLibrary.Active)
							m_MoongateLibrary.Hue = orunebook.Hue == 1121 ? 58 : orunebook.Hue;
						m_MoongateLibrary.UpdateDate();
					}

				}
				m_MoongateLibrary.SendLibraryGump(from, 0, 0, false);
			}

			private bool validateRunebook(Mobile from, Runebook runebook)
			{
				for (int i = 0; i < m_RuneBooks.Count; i++)
				{
					if (((Item)m_RuneBooks[i]) == runebook)
					{
						from.SendMessage("That runebook is already in this library.");
						return false;
					}
				}
				return true;
			}
		}

		public bool UseGate(Mobile m)
		{
			if (!m_DecayRunning && m_DecayTime != TimeSpan.FromSeconds(0))
				DoDecayTimer(m_DecayTime);
			if (m.Criminal)
			{
				m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
			else if (Server.Spells.SpellHelper.CheckCombat(m))
			{
				m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				return false;
			}
			else if (m.Spell != null)
			{
				m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
				return false;
			}
			else if (m.AccessLevel < m_AccessLevel && !Owner(m))
			{
				m.SendMessage("You are unable to use that!");
				return false;
			}
			else
			{
				RemoveDeleted();
				if (m.AccessLevel == AccessLevel.Player && (!Active || m_RuneBooks == null || m_RuneBooks.Count < 1))
				{
					if (!Owner(m))
					{
						m.SendMessage("This Moongate Library is currently inactive or has no locations.");
						return false;
					}
					else
						return SendLibraryGump(m, 0, -1, true);
				}
				else
					return SendLibraryGump(m, 0, -1, true);
			}
		}

		public void RemoveDeleted()
		{
			if (m_RuneBooks == null || m_RuneBooks.Count < 1)
				return;
			for (int i = 0; i < m_RuneBooks.Count; i++)
			{
				if (((Item)m_RuneBooks[i]).Deleted)
					RemoveEntry(i);
			}
			if (m_RuneBooks == null || m_RuneBooks.Count < 1)
				Active = false;
		}

		public void RemoveAll()
		{
			m_RuneBooks.Clear();
			Active = false;
		}

		public void RemoveEntry(int entry)
		{
			m_RuneBooks.RemoveAt(entry);
			if (m_RuneBooks == null || m_RuneBooks.Count < 1)
				Active = false;
			UpdateDate();
		}

		public void ShiftUp(int entry)
		{
			if (entry == 0)
				return;
			object o = m_RuneBooks[entry];
			m_RuneBooks[entry] = m_RuneBooks[entry - 1];
			m_RuneBooks[entry - 1] = o;
			UpdateDate();
		}

		public void ShiftDown(int entry)
		{
			if (entry > m_RuneBooks.Count)
				return;
			object o = m_RuneBooks[entry];
			m_RuneBooks[entry] = m_RuneBooks[entry + 1];
			m_RuneBooks[entry + 1] = o;
			//m_RuneBooks.RemoveAt(entry);
			//if (m_RuneBooks == null || m_RuneBooks.Count < 1)
			//	Active = false;
			UpdateDate();
		}

		public bool SendLibraryGump(Mobile m, int page, int selected, bool sound)
		{
			m.CloseGump(typeof(MoongateLibraryGump));
			m.SendGump(new MoongateLibraryGump(m, this, m_LastChanged, m_RuneBooks, page, selected));
			if (sound)
				Effects.PlaySound(m.Location, m.Map, 0x20E);
			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version

			writer.Write(m_Cost);
			writer.Write(m_AmountCollected);

			writer.Write(m_Active);
			if (m_Owner != null && m_Owner.Deleted)
				m_Owner = null;
			writer.Write(m_Owner);
			writer.Write(m_Decays);
			writer.Write(m_DecayTime);
			writer.Write((int)m_AccessLevel);
			if (m_RuneBooks != null && m_RuneBooks.Count > 0)
			{
				writer.Write(true);
				writer.WriteItemList(m_RuneBooks);
			}
			else
				writer.Write(false);
			writer.Write(m_DecayRunning);
			if (m_DecayRunning)
				writer.Write(this.m_DecayEnd - DateTime.Now);

			if (m_Active && m_RuneBooks != null && m_RuneBooks.Count > 0)
			{
				Runebook runebook = m_RuneBooks[0] as Runebook;
				if (runebook.Hue != Hue)
					Hue = runebook.Hue == 1121 ? 58 : runebook.Hue;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						m_Cost = reader.ReadInt();
						m_AmountCollected = reader.ReadLong();
						goto case 0;
					}

				case 0:
					{
						m_Active = reader.ReadBool();
						m_Owner = reader.ReadMobile();
						m_Decays = reader.ReadBool();
						m_DecayTime = reader.ReadTimeSpan();

						m_AccessLevel = (AccessLevel)reader.ReadInt();
						if (reader.ReadBool())
							m_RuneBooks = reader.ReadItemList();
						m_DecayRunning = reader.ReadBool();
						if (m_DecayRunning)
							this.DoDecayTimer(reader.ReadTimeSpan());
						break;
					}
		}
			UpdateDate();
		}



		public void DoDecayTimer(TimeSpan delay)
		{

			if (m_DecayTimer != null)
				m_DecayTimer.Stop();
			if (!m_Decays) return;
			m_DecayTimer = new DecayTimer(this, delay);
			m_DecayTimer.Start();
			m_DecayRunning = true;
			m_DecayEnd = DateTime.Now + delay;
		}

		private class DecayTimer : Timer
		{
			private MoongateLibrary m_ml;

			public DecayTimer(MoongateLibrary ml, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.OneMinute;
				m_ml = ml;
			}

			protected override void OnTick()
			{
				if (m_ml != null && !m_ml.Deleted)
				{
					m_ml.Delete();
				}
			}
		}
	}

	public class MoongateLibraryGump : Gump
	{
		private Mobile m_Mobile;
		private MoongateLibrary m_MoongateLibrary;
		private ArrayList m_RuneBooks;
		private int m_page;
		private int m_selected;
		private DateTime m_LastChanged;
		private int[] MapHues = new int[] { 37, 98, 53, 1102, 23, 1121, 1121, 1121 };
		private bool myGate;
		public MoongateLibraryGump(Mobile mobile, MoongateLibrary rgate, DateTime LastChanged, ArrayList RuneBooks, int page, int selected)
			: base(100, 100)
		{
			m_Mobile = mobile;
			m_MoongateLibrary = rgate;
			m_LastChanged = LastChanged;
			m_RuneBooks = RuneBooks;
			m_page = page;
			m_selected = selected;
			myGate = m_MoongateLibrary.Owner(m_Mobile);
			AddPage(0);

			if (myGate)
				AddBackground(0, 0, 435, 465, 5054);
			else
				AddBackground(0, 0, 435, 440, 5054);
			AddHtmlLocalized(5, 5, 210, 20, 1012011, false, false); // Pick your destination:
			AddHtml(280, 5, 210, 20, "<basefont size=2 color=#0000ff>CEO's Moongate Library V1.1<basefont>", false, false);
			if (m_MoongateLibrary.Cost != 0)
			{
				if (myGate)
					AddHtml(120, 20, 210, 20, String.Format("<basefont size=4 color=#FFCC33>Cost: {0}gp<basefont>", m_MoongateLibrary.Cost), false, false);
				else
					AddHtml(165, 5, 210, 20, String.Format("<basefont size=4 color=#FFCC33>Cost: {0}gp<basefont>", m_MoongateLibrary.Cost), false, false);

			}
			if (myGate)
			{
				AddButton(110, 385, 4005, 4007, 2, GumpButtonType.Reply, 0);
				AddLabel(145, 385, 0, "ADD"); // Add Runebook
				AddButton(10, 435, 4005, 4007, 6, GumpButtonType.Reply, 0);
				AddLabel(45, 435, 0, "REDEED"); // Deed Moongate (saves all entries)
				AddButton(110, 410, 4005, 4007, 5, GumpButtonType.Reply, 0);
				AddLabel(145, 410, 0, "REMALL"); // Add Runebook
				AddButton(110, 435, 4005, 4007, 7, GumpButtonType.Reply, 0);
				AddLabel(145, 435, 0, "HELP"); // Add Runebook
				if (m_MoongateLibrary.Active)
					AddButton(212, 10, 0x2c88, 0x2c94, 3, GumpButtonType.Reply, 0);
				else
					AddButton(212, 10, 0x2c94, 0x2c88, 4, GumpButtonType.Reply, 0);
				AddHtml(410, 20, 210, 20, string.Format("<basefont size=2 color=#0000ff>({0})<basefont>", m_RuneBooks.Count), false, false);
			}

			if (m_RuneBooks.Count == 0)
				return;
			AddButton(10, 385, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 385, 140, 25, 1011036, false, false); // OKAY
			AddButton(10, 410, 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 410, 140, 25, 1011012, false, false); // CANCEL
			int forcount = m_RuneBooks.Count < 15 ? m_RuneBooks.Count : 12;
			int dispy = myGate ? 0 : 20;
			int slength = 0;
			for (int i = 0; i < forcount; i++)
			{
				int index = m_page * 12 + i;
				if (index >= m_RuneBooks.Count)
					break;
				Runebook runebook = m_RuneBooks[index] as Runebook;
				string bookdesc = runebook.Description == null ? "No Description" : runebook.Description;
				bookdesc.Trim();
				string[] keywordargs = ParseString(bookdesc, 15, ":");
				//Console.WriteLine("Args={0} len={1}", keywordargs.Length, bookdesc.Length);
				string booksecurity = null;
				if (keywordargs != null && keywordargs.Length > 1)
				{
					bookdesc = keywordargs[0];
					booksecurity = keywordargs[1];
				}
				if (bookdesc.Length > 21)
				{
					if (Insensitive.StartsWith(bookdesc, "TITLE,"))
					{
						slength = bookdesc.Length - 6;
						if (bookdesc.Length > 27)
							bookdesc = bookdesc.Substring(0, 26);
					}
					else
						bookdesc = bookdesc.Substring(0, 20);
				}
				//Console.WriteLine("bookdesc={0}", bookdesc);
				if (AccessAllowed(booksecurity, m_Mobile))
				{
					if (selected == -1)
						m_selected = selected = i;
					if (bookdesc.Equals("SKIP") || bookdesc.Equals("BREAK") || Insensitive.StartsWith(bookdesc, "TITLE,")) 
					{
						if (myGate)
							AddButton(30 - dispy, 35 + (i * 25), 2117, 2118, 100 + i, GumpButtonType.Reply, 0);
					}
					else
						AddButton(30 - dispy, 35 + (i * 25), 2117, 2118, 100 + i, GumpButtonType.Reply, 0);

					if (selected == i)
					{
						if (myGate)
						{
							AddButton(5, 35 + (i * 25), 3, 4, 200 + i, GumpButtonType.Reply, 0);
							if (index > 0)
								AddButton(20, 29 + (i * 25), 2435, 2436, 300 + i, GumpButtonType.Reply, 0);
							if (index < (m_RuneBooks.Count - 1))
								AddButton(20, 41 + (i * 25), 2437, 2438, 400 + i, GumpButtonType.Reply, 0);
						}
						if (bookdesc.Equals("SKIP") || bookdesc.Equals("BREAK") || Insensitive.StartsWith(bookdesc, "TITLE,"))
						{
							if (Insensitive.StartsWith(bookdesc, "TITLE,"))
							{
								slength = bookdesc.Length - 6;
								AddHtml(50 - dispy, 35 + (i * 25), 155, 20, string.Format("<basefont color=#FFFFCC><U>{0}</U></basefont>", slength < 1 ? "FIX YOUR TITLE!" : bookdesc.Substring(6, slength)), false, false);
							}
							else if (myGate)
								AddHtml(50 - dispy, 35 + (i * 25), 155, 20, "<basefont color=#00ff42>" + bookdesc + "</basefont>", false, false);
						}
						else
							AddHtml(50 - dispy, 35 + (i * 25), 155, 20, "<basefont color=#00ff42>" + bookdesc + "</basefont>", false, false);
						DisplayBookLocations(runebook);
					}
					else
					{
						if (Insensitive.StartsWith(bookdesc, "TITLE,"))
						{
							slength = bookdesc.Length - 6;
							AddHtml(50 - dispy, 35 + (i * 25), 155, 20, string.Format("<basefont color=#FFFFCC><U>{0}</U></basefont>", slength < 1 ? "FIX YOUR TITLE!" : bookdesc.Substring(6, slength)), false, false);
						}
						else if (bookdesc.Equals("BREAK"))
							AddHtml(50 - dispy, 35 + (i * 25), 155, 20, "<basefont color=#6600CC>---------------------</basefont>", false, false);
						else
							if (bookdesc.Equals("SKIP"))
							{
								if (myGate)
									AddHtml(50 - dispy, 35 + (i * 25), 155, 20, bookdesc, false, false);
							}
							else
								AddHtml(50 - dispy, 35 + (i * 25), 155, 20, bookdesc, false, false);
					}
				}
				else
				{
					if (Insensitive.StartsWith(bookdesc, "TITLE,"))
					{
						slength = bookdesc.Length - 6;
						AddHtml(50 - dispy, 35 + (i * 25), 155, 20, string.Format("<basefont color=#FFFFCC><U>{0}</U></basefont>", slength < 1 ? "FIX YOUR TITLE!" : bookdesc.Substring(6, slength)), false, false);
					}
					else if (bookdesc.Equals("BREAK"))
						AddHtml(50 - dispy, 35 + (i * 25), 155, 20, "<basefont color=#6600CC>---------------------</basefont>", false, false);
					else
						if (!bookdesc.Equals("SKIP"))
							AddHtml(50 - dispy, 35 + (i * 25), 155, 20, "<i>" + bookdesc + "</i>", false, false);
				}
			}
			
			//int count = (m_RuneBooks.Count / 12) + (m_RuneBooks.Count % 12 > 0 ? 1:0);
			if (m_RuneBooks.Count > 14)
				for (int i = 0; i < m_RuneBooks.Count / 12.0; i++)
				{
					if (i == m_page)
						AddImage(9 + (i * 24), 345, 0x8B1 + i, 69);
					else
						AddButton(9 + (i * 24), 345, 0x8B1 + i, 0x8B1 + i, 50 + i, GumpButtonType.Reply, 0);
				}
		}

		private static string[] ParseString(string str, int nitems, string delimstr)
		{
			if (str == null || delimstr == null) return null;

			char[] delims = delimstr.ToCharArray();
			string[] args = null;
			str = str.Trim();
			args = str.Split(delims, nitems);
			return args;
		}

		private bool AccessAllowed(string securitystring, Mobile m)
		{
			//Console.WriteLine("AccessString={0}, {1}", securitystring, m.AccessLevel);\
			if (securitystring == null || m.AccessLevel > AccessLevel.Player || myGate)
				return true;
			if (Insensitive.Contains(securitystring, "NOREDS") && (m.Kills > 4))
				return false;
			if (Insensitive.Contains(securitystring, "REDSONLY") && (m.Kills < 5))
				return false;
			if (Insensitive.Contains(securitystring, "OWNERONLY") && (m.Account != m_MoongateLibrary.m_Owner.Account))
				return false;
			if (Insensitive.Contains(securitystring, "ALIVEONLY") && (!m.Alive))
				return false;
			if (Insensitive.Contains(securitystring, "DEADONLY") && (m.Alive))
				return false;
			if (Insensitive.Contains(securitystring, "POISONEDONLY") && (!m.Poisoned))
				return false;
			if (Insensitive.Contains(securitystring, "MALEONLY") && (m.Female))
				return false;
			if (Insensitive.Contains(securitystring, "FEMALEONLY") && (!m.Female))
				return false;
			if (Insensitive.Contains(securitystring, "MYGUILD") && (m.GuildFealty != m_MoongateLibrary.m_Owner.GuildFealty))
				return false;
			PlayerMobile pm = m as PlayerMobile;
			if (Insensitive.Contains(securitystring, "YOUNGONLY") && (!(pm.Young)))
				return false;
			if (Insensitive.Contains(securitystring, "NOYOUNG") && ((pm.Young)))
				return false;
			return true;
		}

		private void DisplayBookLocations(Runebook runebook)
		{

			if (runebook == null)
				return;

			for (int i = 0; i < runebook.Entries.Count; i++)
			{
				RunebookEntry entry = runebook.Entries[i] as RunebookEntry;
				string locationtext = entry.Description;
				if (locationtext == null || locationtext.Length < 1)
					locationtext = String.Format("({0}, {1}, {2})", entry.Location.X, entry.Location.Y, entry.Location.Z);
				string[] keywordargs = ParseString(locationtext, 15, ":");
				//Console.WriteLine("Args={0}", keywordargs.Length);
				string runesecurity = null;
				if (keywordargs != null && keywordargs.Length > 1)
				{
					locationtext = keywordargs[0];
					runesecurity = keywordargs[1];
				}
				if (locationtext.Length > 25)
					locationtext = locationtext.Substring(0, 25);
				if (AccessAllowed(runesecurity, m_Mobile))
				{
					AddRadio(210, 35 + (i * 25), 210, 211, false, m_selected * 1000 + i);
					AddLabel(235, 35 + (i * 25), MapHues[entry.Map.MapIndex], locationtext);
				}
				else
					AddLabel(235, 35 + (i * 25), 0, "--Not Accessable--");
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			int index;
			Runebook runebook;
			if (info.ButtonID == 0 || m_Mobile == null || m_MoongateLibrary == null) // Cancel
				return;
			else if (m_Mobile.Deleted || m_MoongateLibrary.Deleted || m_Mobile.Map == null)
					return;
			else if (info.ButtonID == 2 && (myGate)) // Add Runebook
			{
				m_MoongateLibrary.AddRunebook(m_Mobile);
				return;
			}
			else if (info.ButtonID == 3 || info.ButtonID == 4 && (myGate))
			{
				m_MoongateLibrary.Active = !m_MoongateLibrary.Active;
				m_MoongateLibrary.SendLibraryGump(m_Mobile, 0, 0, false);
				return;
			}
			else if (info.ButtonID == 5 && (myGate))
			{
				m_Mobile.CloseGump(typeof(MoongateConfirmDeleteGump));
				m_Mobile.SendGump(new MoongateConfirmDeleteGump(true, m_Mobile, m_MoongateLibrary, null, 0));
				return;
			}
			else if (info.ButtonID == 6 && (myGate))
			{
				m_MoongateLibrary.ReDeed(m_Mobile);
				return;
			}
			else if (info.ButtonID == 7 && (myGate))
			{
				m_MoongateLibrary.SendLibraryGump(m_Mobile, 0, 0, false);
				m_Mobile.CloseGump(typeof(MoongateLibraryHelpGump));
				m_Mobile.SendGump(new MoongateLibraryHelpGump());
				return;
			}
			else if (info.ButtonID >= 50 && info.ButtonID < 60)
			{
				m_MoongateLibrary.SendLibraryGump(m_Mobile, info.ButtonID - 50, 0, false);
				return;
			}
			else if (info.ButtonID >= 100 && info.ButtonID < 120)
			{
				m_MoongateLibrary.SendLibraryGump(m_Mobile, m_page, info.ButtonID - 100, false);
				return;
			}
			else if ((info.ButtonID >= 200 && info.ButtonID < 220) && (myGate))
			{
				m_Mobile.CloseGump(typeof(MoongateConfirmDeleteGump));
				index = m_page * 12 + info.ButtonID - 200;
				if (index < 0 || index >= m_RuneBooks.Count || m_RuneBooks[index] == null)
				{
					m_Mobile.SendMessage("Moongate Library has changed, choose again.");
					m_MoongateLibrary.RemoveDeleted();
					m_MoongateLibrary.SendLibraryGump(m_Mobile, 0, 0, true);
					return;
				}
				runebook = ((Runebook)m_RuneBooks[index]);
				m_Mobile.SendGump(new MoongateConfirmDeleteGump(false, m_Mobile, m_MoongateLibrary, runebook.Description, index));
				return;
			}
			else if ((info.ButtonID >= 300 && info.ButtonID < 320) && (myGate))
			{
				index = m_page * 12 + info.ButtonID - 300;
				if (index <= 0) // should never happen unless there's razor/button spoofing going on.
					return;
				m_MoongateLibrary.ShiftUp(m_page * 12 + info.ButtonID - 300);
				m_MoongateLibrary.SendLibraryGump(m_Mobile, m_page, (info.ButtonID - 300) == 0 ? 0 : info.ButtonID - 301, false);
				return;
			}
			else if ((info.ButtonID >= 400 && info.ButtonID < 420) && (myGate))
			{
				index = m_page * 12 + info.ButtonID - 400;
				if (index >= m_RuneBooks.Count) // should never happen unless there's razor/button spoofing going on.
					return;
				int ecount = m_RuneBooks.Count > 14 ? 14 : 12;
				int nbuttonloc = info.ButtonID - 400 == ecount ? ecount : info.ButtonID - 399;
				m_MoongateLibrary.ShiftDown(m_page * 12 + info.ButtonID - 400);
				m_MoongateLibrary.SendLibraryGump(m_Mobile, m_page, nbuttonloc, false);
				return;
			}
			if (m_MoongateLibrary.LastChanged != m_LastChanged)
			{
				m_Mobile.SendMessage("Moongate Library has changed, choose again.");
				m_MoongateLibrary.RemoveDeleted();
				m_MoongateLibrary.SendLibraryGump(m_Mobile, 0, 0, true);
				return;
			}

			int[] switches = info.Switches;

			if (switches.Length == 0)
				return;

			int switchID = switches[0];

			int selected = switchID / 1000 + m_page * 12;
			int location = switchID % 1000;

			if (selected < 0 || selected >= m_RuneBooks.Count || m_RuneBooks[selected] == null || location >= ((Runebook)m_RuneBooks[selected]).Entries.Count)
			{
				m_Mobile.SendMessage("Moongate Library has changed, choose again.");
				m_MoongateLibrary.RemoveDeleted();
				m_MoongateLibrary.SendLibraryGump(m_Mobile, 0, 0, true);
				return;
			}
			else if (((Runebook)m_RuneBooks[selected]).Deleted)
			{
				m_Mobile.SendMessage("This location is no longer available (Runebook deleted!), choose again.");
				m_MoongateLibrary.RemoveDeleted();
				m_MoongateLibrary.SendLibraryGump(m_Mobile, 0, 0, true);
				return;
			}

			runebook = ((Runebook)m_RuneBooks[selected]);
            if ((m_Mobile.AccessLevel == AccessLevel.Player) && (runebook.Description != null) && (runebook.Description.Equals("SKIP") || runebook.Description.Equals("BREAK") || Insensitive.StartsWith(runebook.Description, "TITLE,")))
			{
				m_Mobile.SendMessage("You do not have permission to access this location.");//Don't let them use format books
				return;
			}
			RunebookEntry entry = runebook.Entries[location] as RunebookEntry;
			// Validate security one more time incase they are trying to spoof gump buttons with Razor/etc...
			string[] keywordargs = ParseString(runebook.Description, 15, ":");
			//Console.WriteLine("keywordargs = {0} Runebook={1} Entry={2}", keywordargs, runebook.Description, entry.Description);
			if (keywordargs != null && keywordargs.Length > 1)
			{
				if (!AccessAllowed(keywordargs[1], m_Mobile))
				{
					m_Mobile.SendMessage("You do not have permission to access this location."); 
					return;
				}
			}
			keywordargs = ParseString(entry.Description, 15, ":");
			if (keywordargs != null && keywordargs.Length > 1)
			{
				if (!AccessAllowed(keywordargs[1], m_Mobile))
				{
					m_Mobile.SendMessage("You do not have permission to access this location.");
					return;
				}
			}
			PlayerMobile pm = m_Mobile as PlayerMobile;
			if (!m_Mobile.InRange(m_MoongateLibrary.GetWorldLocation(), 1) || m_Mobile.Map != m_MoongateLibrary.Map)
			{
				m_Mobile.SendLocalizedMessage(1019002); // You are too far away to use the gate.
			}
			else if (m_Mobile.Player && m_Mobile.Kills >= 5 && entry.Map != Map.Felucca && !m_MoongateLibrary.m_RedsAnywhere)
			{
				m_Mobile.SendLocalizedMessage(1019004); // You are not allowed to travel there.
			}
			else if (m_Mobile.Player && pm.Young && entry.Map == Map.Felucca)
			{
				m_Mobile.SendLocalizedMessage(1019004); // You are not allowed to travel there.
			}
			else if (m_Mobile.Criminal)
			{
				m_Mobile.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
			}
			else if (Server.Spells.SpellHelper.CheckCombat(m_Mobile))
			{
				m_Mobile.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
			}
			else if (m_Mobile.Spell != null)
			{
				m_Mobile.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
			}
			else if (m_Mobile.Map == entry.Map && m_Mobile.InRange(entry.Location, 1))
			{
				m_Mobile.SendLocalizedMessage(1019003); // You are already there.
			}
			else if ( m_Mobile.AccessLevel == AccessLevel.Player && Insensitive.Contains(entry.Description, "password,"))
			{
				m_Mobile.CloseGump(typeof(MoongateLibraryConfirmGump));
				m_Mobile.SendGump(new MoongateLibraryConfirmGump(m_MoongateLibrary, m_Mobile, entry.Location, entry.Map, entry.Description, 2));

			}
			else if (m_Mobile.AccessLevel == AccessLevel.Player && entry.Map == Map.Felucca && m_Mobile.Map != Map.Felucca)
			{
				m_Mobile.CloseGump(typeof(MoongateLibraryConfirmGump));
				m_Mobile.SendGump(new MoongateLibraryConfirmGump(m_MoongateLibrary, m_Mobile, entry.Location, entry.Map, entry.Description, 0));

			}
			else if (m_Mobile.AccessLevel == AccessLevel.Player && Insensitive.Contains(entry.Description, "warnplayer"))
			{
				m_Mobile.CloseGump(typeof(MoongateLibraryConfirmGump));
				m_Mobile.SendGump(new MoongateLibraryConfirmGump(m_MoongateLibrary, m_Mobile, entry.Location, entry.Map, entry.Description, 1));

			}
			else
			{
				if (m_MoongateLibrary.Cost == 0 || myGate || CollectGold(m_Mobile, m_MoongateLibrary.Cost) )
				{
					m_MoongateLibrary.AddToCollectedAmount(m_MoongateLibrary.Cost);
					BaseCreature.TeleportPets(m_Mobile, entry.Location, entry.Map);
					m_Mobile.Combatant = null;
					m_Mobile.Warmode = false;
					if (m_Mobile.AccessLevel > AccessLevel.Player)
						m_Mobile.Hidden = true;
					m_Mobile.MoveToWorld(entry.Location, entry.Map);
					Effects.PlaySound(entry.Location, entry.Map, 0x1FE);
				}
				else
				{
					m_Mobile.SendLocalizedMessage(1042205); // Not enough gold
				}
			}
		}

		private bool CollectGold (Mobile m, int amount)
		{
			if (m.Backpack.ConsumeTotal(typeof(Gold), amount))
			{
				m.SendMessage("{0} gold has been withdrawn from your backpack.", amount);
				return true;
			}
			if (m.BankBox.ConsumeTotal(typeof(Gold), amount))
			{
				m.SendMessage("{0} gold has been withdrawn from your bank box.", amount);
				return true;
			}
			return false;
		}
	}

	public class MoongateLibraryConfirmGump : Gump
	{
		private MoongateLibrary m_MoongateLibrary;
		private Mobile m_Mobile;
		private Point3D m_location;
		private Map m_map;
		private bool myGate;
		private int m_message;
		private string m_runedesc;
		private string m_passphrase;

		public MoongateLibraryConfirmGump(MoongateLibrary moongatelibrary, Mobile mobile, Point3D location, Map map, string runedesc, int message)
			: base(10, 10)
		{
			m_Mobile = mobile;
			m_location = location;
			m_map = map;
			m_MoongateLibrary = moongatelibrary;
			myGate = m_MoongateLibrary.Owner(m_Mobile);
			m_message = message;
			m_runedesc = runedesc;

			Effects.PlaySound(m_Mobile.Location, m_Mobile.Map, 0x20E);
			AddPage(0);

			AddBackground(0, 0, 360, 155, 5054);
			AddBackground(10, 10, 340, 135, 3000);

			switch (message)
			{
				case 1:
					{
						AddLabel(20, 30, 44, "Your destination may be dangerous, are you sure?");
						AddHtml(20, 50, 380, 60, @"Press Continue to proceed or Cancel to abort.", false, false);
						break;
					}
				case 2:
					{
						AddLabel(20, 20, 44, "This destination requires a password:");
						AddImageTiled(80, 60, 175, 20, 5058);
						AddTextEntry(85, 60, 190, 19, 1150, 10, m_passphrase);
						break;
					}
				default:
					{
						AddLabel(20, 30, 34, "You have chosen a Felucca destination, are you sure?");
						AddHtml(20, 50, 380, 60, @"Press Continue to proceed or Cancel to abort.", false, false);
						break;
					}
			}

			AddButton(60, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(95, 100, 290, 40, 1011011, false, false); // CONTINUE
			AddButton(220, 100, 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(255, 100, 290, 20, 1011012, false, false); // CANCEL
		}

		private static string[] ParseString(string str, int nitems, string delimstr)
		{
			if (str == null || delimstr == null) return null;

			char[] delims = delimstr.ToCharArray();
			string[] args = null;
			str = str.Trim();
			args = str.Split(delims, nitems);
			return args;
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				if (m_message == 2)
				{
					string[] actionwordargs;
					actionwordargs = ParseString(m_runedesc, 15, ",");
					string runepw = actionwordargs[actionwordargs.Length - 1];
					TextRelay tr = info.GetTextEntry(10);
					if (actionwordargs.Length == 1 || tr == null || tr.Text == null || runepw == null || tr.Text.Trim() != runepw.Trim())
					{
						m_Mobile.SendMessage("You are denied access to this location.");
						return;
					}
				}
				if (m_MoongateLibrary.Cost == 0 || myGate || CollectGold(m_Mobile, m_MoongateLibrary.Cost))
				{
					m_MoongateLibrary.AddToCollectedAmount(m_MoongateLibrary.Cost);
					BaseCreature.TeleportPets(m_Mobile, m_location, m_map);
					m_Mobile.Combatant = null;
					m_Mobile.Warmode = false;
					if (m_Mobile.AccessLevel > AccessLevel.Player)
						m_Mobile.Hidden = true;
					m_Mobile.MoveToWorld(m_location, m_map);
					Effects.PlaySound(m_location, m_map, 0x1FE);
				}
				else
				{
					m_Mobile.SendLocalizedMessage(1042205); // Not enough Gold
				}
			}
		}

		private bool CollectGold(Mobile m, int amount)
		{
			if (m.Backpack.ConsumeTotal(typeof(Gold), amount))
			{
				m.SendMessage("{0} gold has been withdrawn from your backpack.", amount);
				return true;
			}
			if (m.BankBox.ConsumeTotal(typeof(Gold), amount))
			{
				m.SendMessage("{0} gold has been withdrawn from your bank box.", amount);
				return true;
			}
			return false;
		}
	}

	public class MoongateConfirmDeleteGump : Gump
	{
		private bool m_all;
		private Mobile m_From;
		private MoongateLibrary m_Gate;
		private string m_rdesc;
		private int m_index;

		public MoongateConfirmDeleteGump(bool all, Mobile from, MoongateLibrary gate, string rdesc, int index)
			: base(Core.AOS ? 110 : 20, Core.AOS ? 100 : 30)
		{
			m_all = all;
			m_From = from;
			m_Gate = gate;
			m_rdesc = rdesc;
			m_index = index;
			Closable = false;
			int width = 250;
			int height = 125;

			AddPage(0);
			AddBackground(0, 0, width, height, 5054);

			if (m_all)
				AddHtml(10, 25, width - 10, height - 45, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 6684723, "Are you sure you want to delete All runebooks?"), false, false);
			else
			{
				AddHtml(10, 15, width - 10, height - 45, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 3407616, "Are you sure you want to delete this runebook?"), false, false);
				AddHtml(10, 60, width - 10, height - 15, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 13434675, m_rdesc), false, false);
			}
			AddButton(10, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, height - 30, 170, 20, 1011036, 32767, false, false); // OKAY

			AddButton(10 + ((width - 20) / 2), height - 30, 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45 + ((width - 20) / 2), height - 30, 170, 20, 1011012, 32767, false, false); // CANCEL
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				if (m_all)
					m_Gate.RemoveAll();
				else
					m_Gate.RemoveEntry(m_index);
			}
			m_Gate.SendLibraryGump(m_From, 0, 0, false);
		}
	}

	public class MoongateLibraryHelpGump : Gump
	{
		public MoongateLibraryHelpGump()
			: base(Core.AOS ? 110 : 20, Core.AOS ? 100 : 30)
		{

			Closable = true;
			//Sizable = true;
			int width = 400;
			int height = 300;

			AddPage(0);
			AddBackground(0, 0, width, height, 5054);
			string text = "<basefont color=#007fff><u><center>Moongate Library Instructions</center></u>" +
			"<basefont color=white>This moongate library allows you store up to 96 runebooks in one moongate, giving you the ability to have " +
			"over 1500 locations in one gate! Also, depending on how you name a runebook " +
			"or recall rune dropped in the book allows you to add additional conditions to the location as to whether " +
			"a certain player may use that book or location. There are also special keywords for runebooks allowing you " +
			"to do some simple formatting of your moongate library gump.<br>" +
			"<basefont color=red><u><center>Runebook Only Keywords</center></u>" +
			"<basefont color=black size=-3>(Note: Using one of these keywords basically makes this book a formatting holder, causing the book's " +
			"locations to be inaccessable to players.)<br>" +
			"<basefont color=white size=4>SKIP - Display a blank line<br>" +
			"BREAK - Display a ---- line<br>" +
			"TITLE,{title} - Display a {title} line<br><basefont color=black size=3>example: TITLE,Mage Shop's<br>" +
			"<basefont color=green size=4><u><center>Runebook & Location Keywords</center></u>" +
			"<basefont color=white>" +
			"NOREDS - No murders allowed<br>" +
			"REDSONLY - Only murders can use<br>" +
			"OWNERONLY - Only the owner can use this location<br>" +
			"ALIVEONLY - Must be alive to use<br>" +
			"DEADONLY - Must be a ghost<br>" +
			"POSIONEDONLY - Must be posioned<br>" +
			"MALEONLY - Men Only!<br>" +
			"FEMALEONLY - Females Only!<br>" +
			"MYGUILD - Only member's of the gates owner's guild can use<br>" +
			"YOUNGONLY - Only young players<br>" +
			"NOYOUNG - No young players allowed<br>" +
			"<basefont color=yellow size=4><u><center>Location Only Keywords</center></u>" +
			"<basefont color=white>" +
			"WARNPLAYER - Alway warn a player before transporting<br>" +
			"PASSWORD - Require a password to use<br><basefont color=black size=-3>example: PASSWORD,MySecrEtPlace <basefont color=red>(note, this MUST be the last keyword on the rune)<br>" +
			"</basefont>";
			AddHtml(15, 25, width - 30, height - 70, text, true, true);
			AddButton(width/2 - 30, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(width/2, height - 30, 170, 20, 1011036, 32767, false, false); // OKAY
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
				return;
		}
	
	}
}


namespace Server.Items
{
	public class MoongateLibraryDeed : Item
	{
		public override int LabelNumber { get { return 1041116; } } // a deed for a holiday tree
		private bool m_Active;
		private ArrayList m_RuneBooks = new ArrayList();

		// DecayTimer Stuff
		private bool m_Decays = true;
		//private DateTime m_DecayEnd;
		private TimeSpan m_DecayTime = TimeSpan.FromDays(90);
		//private bool m_DecayRunning = false;
		private TimeSpan m_TimeLeft;
		private int m_Cost = 25;
		private long m_AmountCollected;

		[CommandProperty(AccessLevel.Counselor)]
		public bool Active
		{
			get { return m_Active; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool GateDecays
		{
			get { return m_Decays; }
			set
			{
				m_Decays = value;
				if (m_Decays)
				{
					if (m_TimeLeft == TimeSpan.FromSeconds(0))
						m_TimeLeft = m_DecayTime;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Cost
		{
			get { return m_Cost; }
			set
			{
				if (value < 0)
					m_Cost = 0;
				else if (value > 5000)
					m_Cost = 5000;
				else
					m_Cost = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long Collected
		{
			get { return m_AmountCollected; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan DecayTimer1
		{
			get { return m_DecayTime; }
			set { m_DecayTime = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan TimeLeft
		{
			get { return m_TimeLeft; }
			set { m_TimeLeft = value; }
		}

		[Constructable]
		public MoongateLibraryDeed(bool active, ArrayList runebooks, bool decays, TimeSpan decaytimer, TimeSpan timeleft, int cost, long amountcollected)
			: base(0x14F0)
		{
			Hue = 58;
			Weight = 1.0;
			Name = "Moongate Library Deed";
			LootType = LootType.Blessed;
			m_Active = active;
			m_RuneBooks = runebooks;
			m_Decays = decays;
			m_DecayTime = decaytimer;
			m_TimeLeft = timeleft;
			m_Cost = cost;
			m_AmountCollected = amountcollected;
		}

		[Constructable]
		public MoongateLibraryDeed()
			: base(0x14F0)
		{
			Hue = 58;
			Weight = 1.0;
			Name = "Moongate Library Deed";
			LootType = LootType.Blessed;
		}

		public MoongateLibraryDeed(Serial serial)
			: base(serial)
		{
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			if (m_Decays)
				list.Add(1060658, "Expire Time\t{0}", m_TimeLeft);
			else
				list.Add(1060658, "Expire Time\tInfinate");
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); 

			writer.Write(m_Cost); // Version 1
			writer.Write(m_AmountCollected);

			writer.Write(m_Active); // Version 0
			writer.Write(m_Decays);
			writer.Write(m_DecayTime);
			if (m_RuneBooks != null && m_RuneBooks.Count > 0)
			{
				writer.Write(true);
				writer.WriteItemList(m_RuneBooks);
			}
			else
				writer.Write(false);
			if (m_Decays)
				writer.Write(m_TimeLeft);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			switch (version)
			{
				case 1:
					{
						m_Cost = reader.ReadInt();
						m_AmountCollected = reader.ReadLong();
						goto case 0;
					}

				case 0:
					{
						m_Active = reader.ReadBool();
						m_Decays = reader.ReadBool();
						m_DecayTime = reader.ReadTimeSpan();
						if (reader.ReadBool())
							m_RuneBooks = reader.ReadItemList();
						if (m_Decays)
							m_TimeLeft = reader.ReadTimeSpan();
						LootType = LootType.Blessed;
						break;
					}
			}
		}

		public bool ValidatePlacement(Mobile from, Point3D loc)
		{
			if (from.AccessLevel >= AccessLevel.GameMaster)
				return true;

			if (!from.InRange(this.GetWorldLocation(), 1))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return false;
			}

			Map map = from.Map;

			if (map == null)
				return false;

			BaseHouse house = BaseHouse.FindHouseAt(loc, map, 20);

			if (house == null || !house.IsFriend(from))
			{
				from.SendMessage("The Moongate Library can only be placed in your house.");
				return false;
			}

			if (!map.CanFit(loc, 20))
			{
				from.SendLocalizedMessage(500269); // You cannot build that there.
				return false;
			}

			return true;
		}

		public void BeginPlace(Mobile from)
		{
			from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Placement_OnTarget));
		}

		public void Placement_OnTarget(Mobile from, object targeted)
		{
			IPoint3D p = targeted as IPoint3D;

			if (p == null)
				return;

			Point3D loc = new Point3D(p);

			if (p is StaticTarget)
				loc.Z -= TileData.ItemTable[((StaticTarget)p).ItemID & 0x3FFF].CalcHeight; /* NOTE: OSI does not properly normalize Z positioning here.
																							* A side affect is that you can only place on floors (due to the CanFit call).
																							* That functionality may be desired. And so, it's included in this script.																					*/

			if (ValidatePlacement(from, loc))
				EndPlace(from, loc);
		}

		public void EndPlace(Mobile from, Point3D loc)
		{
			this.Delete();
			new MoongateLibrary(from, loc, m_Active, m_RuneBooks, m_Decays, m_DecayTime, m_TimeLeft, m_Cost, m_AmountCollected);
		}

		public override void OnDoubleClick(Mobile from)
		{
			BeginPlace(from);
		}
	}
}