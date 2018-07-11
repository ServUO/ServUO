#region Header
// **********
// ServUO - CustomHairstylist.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Items;
using Server.Network;
#endregion

namespace Server.Mobiles
{
	public class CustomHairstylist : BaseVendor
	{
		public static readonly object From = new object();
		public static readonly object Vendor = new object();
		public static readonly object Price = new object();

		private static readonly HairstylistBuyInfo[] m_SellList = new[]
		{
			new HairstylistBuyInfo(
				1018357,
				50000,
				false,
				typeof(ChangeHairstyleGump),
				new[] {From, Vendor, Price, false, ChangeHairstyleEntry.HairEntries}),
			new HairstylistBuyInfo(
				1018358,
				50000,
				true,
				typeof(ChangeHairstyleGump),
				new[] {From, Vendor, Price, true, ChangeHairstyleEntry.BeardEntries}),
			new HairstylistBuyInfo(
				1018359,
				50,
				false,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, true, true, ChangeHairHueEntry.RegularEntries}),
			new HairstylistBuyInfo(
				1018360,
				500000,
				false,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, true, true, ChangeHairHueEntry.BrightEntries}),
			new HairstylistBuyInfo(
				1018361,
				30000,
				false,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, true, false, ChangeHairHueEntry.RegularEntries}),
			new HairstylistBuyInfo(
				1018362,
				30000,
				true,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, false, true, ChangeHairHueEntry.RegularEntries}),
			new HairstylistBuyInfo(
				1018363,
				500000,
				false,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, true, false, ChangeHairHueEntry.BrightEntries}),
			new HairstylistBuyInfo(
				1018364,
				500000,
				true,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, false, true, ChangeHairHueEntry.BrightEntries})
		};

        private static readonly HairstylistBuyInfo[] m_SellListElf = new[]
        {
            new HairstylistBuyInfo(
				1018357,
				50000,
				false,
				typeof(ChangeHairstyleGump),
				new[] {From, Vendor, Price, false, ChangeHairstyleEntry.HairEntriesElf}),
			new HairstylistBuyInfo(
				1018359,
				50,
				false,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, true, true, ChangeHairHueEntry.RegularEntries}),
			new HairstylistBuyInfo(
				1018360,
				500000,
				false,
				typeof(ChangeHairHueGump),
				new[] {From, Vendor, Price, true, true, ChangeHairHueEntry.BrightEntries})
        };

		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

		[Constructable]
		public CustomHairstylist()
			: base("the hairstylist")
		{ }

		public CustomHairstylist(Serial serial)
			: base(serial)
		{ }

		public override bool ClickTitle { get { return false; } }
		public override bool IsActiveBuyer { get { return false; } }
		public override bool IsActiveSeller { get { return true; } }
		public override VendorShoeType ShoeType { get { return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals; } }
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
		{
			return false;
		}

		public override void VendorBuy(Mobile from)
		{
            if (from.Race == Race.Human)
            {
                from.SendGump(new HairstylistBuyGump(from, this, m_SellList));
            }
            else if (from.Race == Race.Elf)
            {
                from.SendGump(new HairstylistBuyGump(from, this, m_SellListElf));
            }
		}

		public override int GetHairHue()
		{
			return Utility.RandomBrightHue();
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem(new Robe(Utility.RandomPinkHue()));
		}

        public override bool CheckVendorAccess(Mobile from)
        {
            if (from.Race == Race.Gargoyle)
                return false;

            return base.CheckVendorAccess(from);
        }

		public override void InitSBInfo()
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class HairstylistBuyInfo
	{
		private readonly int m_Title;
		private readonly string m_TitleString;
		private readonly int m_Price;
		private readonly bool m_FacialHair;
		private readonly Type m_GumpType;
		private readonly object[] m_GumpArgs;

		public HairstylistBuyInfo(int title, int price, bool facialHair, Type gumpType, object[] args)
		{
			m_Title = title;
			m_Price = price;
			m_FacialHair = facialHair;
			m_GumpType = gumpType;
			m_GumpArgs = args;
		}

		public HairstylistBuyInfo(string title, int price, bool facialHair, Type gumpType, object[] args)
		{
			m_TitleString = title;
			m_Price = price;
			m_FacialHair = facialHair;
			m_GumpType = gumpType;
			m_GumpArgs = args;
		}

		public int Title { get { return m_Title; } }
		public string TitleString { get { return m_TitleString; } }
		public int Price { get { return m_Price; } }
		public bool FacialHair { get { return m_FacialHair; } }
		public Type GumpType { get { return m_GumpType; } }
		public object[] GumpArgs { get { return m_GumpArgs; } }
	}

	public class HairstylistBuyGump : Gump
	{
		private readonly Mobile m_From;
		private readonly Mobile m_Vendor;
		private readonly HairstylistBuyInfo[] m_SellList;

		public HairstylistBuyGump(Mobile from, Mobile vendor, HairstylistBuyInfo[] sellList)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_SellList = sellList;

			from.CloseGump(typeof(HairstylistBuyGump));
			from.CloseGump(typeof(ChangeHairHueGump));
			from.CloseGump(typeof(ChangeHairstyleGump));

			bool isFemale = (from.Female || from.Body.IsFemale);

			int balance = Banker.GetBalance(from);
			int canAfford = 0;

			for (int i = 0; i < sellList.Length; ++i)
			{
				if (balance >= sellList[i].Price && (!sellList[i].FacialHair || !isFemale))
				{
					++canAfford;
				}
			}

			AddPage(0);

			AddBackground(50, 10, 450, 100 + (canAfford * 25), 2600);

			AddHtmlLocalized(100, 40, 350, 20, 1018356, false, false); // Choose your hairstyle change:

			int index = 0;

			for (int i = 0; i < sellList.Length; ++i)
			{
				if (balance >= sellList[i].Price && (!sellList[i].FacialHair || !isFemale))
				{
					if (sellList[i].TitleString != null)
					{
						AddHtml(140, 75 + (index * 25), 300, 20, sellList[i].TitleString, false, false);
					}
					else
					{
						AddHtmlLocalized(140, 75 + (index * 25), 300, 20, sellList[i].Title, false, false);
					}

					AddButton(100, 75 + (index++ * 25), 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			int index = info.ButtonID - 1;

			if (index >= 0 && index < m_SellList.Length)
			{
				HairstylistBuyInfo buyInfo = m_SellList[index];

				int balance = Banker.GetBalance(m_From);

				bool isFemale = (m_From.Female || m_From.Body.IsFemale);

				if (buyInfo.FacialHair && isFemale)
				{
					// You cannot place facial hair on a woman!
					m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1010639, m_From.NetState);
				}
				else if (balance >= buyInfo.Price)
				{
					try
					{
						var origArgs = buyInfo.GumpArgs;
						var args = new object[origArgs.Length];

						for (int i = 0; i < args.Length; ++i)
						{
							if (origArgs[i] == CustomHairstylist.Price)
							{
								args[i] = m_SellList[index].Price;
							}
							else if (origArgs[i] == CustomHairstylist.From)
							{
								args[i] = m_From;
							}
							else if (origArgs[i] == CustomHairstylist.Vendor)
							{
								args[i] = m_Vendor;
							}
							else
							{
								args[i] = origArgs[i];
							}
						}

						Gump g = Activator.CreateInstance(buyInfo.GumpType, args) as Gump;

						m_From.SendGump(g);
					}
					catch
					{ }
				}
				else
				{
					// You cannot afford my services for that style.
					m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
				}
			}
		}
	}

	public class ChangeHairHueEntry
	{
		public static readonly ChangeHairHueEntry[] BrightEntries = new[]
		{
			new ChangeHairHueEntry("*****", 12, 10), new ChangeHairHueEntry("*****", 32, 5),
			new ChangeHairHueEntry("*****", 38, 8), new ChangeHairHueEntry("*****", 54, 3),
			new ChangeHairHueEntry("*****", 62, 10), new ChangeHairHueEntry("*****", 81, 2),
			new ChangeHairHueEntry("*****", 89, 2), new ChangeHairHueEntry("*****", 1153, 2)
		};

		public static readonly ChangeHairHueEntry[] RegularEntries = new[]
		{
			new ChangeHairHueEntry("*****", 1602, 26), new ChangeHairHueEntry("*****", 1628, 27),
			new ChangeHairHueEntry("*****", 1502, 32), new ChangeHairHueEntry("*****", 1302, 32),
			new ChangeHairHueEntry("*****", 1402, 32), new ChangeHairHueEntry("*****", 1202, 24),
			new ChangeHairHueEntry("*****", 2402, 29), new ChangeHairHueEntry("*****", 2213, 6),
			new ChangeHairHueEntry("*****", 1102, 8), new ChangeHairHueEntry("*****", 1110, 8),
			new ChangeHairHueEntry("*****", 1118, 16), new ChangeHairHueEntry("*****", 1134, 16)
		};

		private readonly string m_Name;
		private readonly int[] m_Hues;

		public ChangeHairHueEntry(string name, int[] hues)
		{
			m_Name = name;
			m_Hues = hues;
		}

		public ChangeHairHueEntry(string name, int start, int count)
		{
			m_Name = name;

			m_Hues = new int[count];

			for (int i = 0; i < count; ++i)
			{
				m_Hues[i] = start + i;
			}
		}

		public string Name { get { return m_Name; } }
		public int[] Hues { get { return m_Hues; } }
	}

	public class ChangeHairHueGump : Gump
	{
		private readonly Mobile m_From;
		private readonly Mobile m_Vendor;
		private readonly int m_Price;
		private readonly bool m_Hair;
		private readonly bool m_FacialHair;
		private readonly ChangeHairHueEntry[] m_Entries;

		public ChangeHairHueGump(
			Mobile from, Mobile vendor, int price, bool hair, bool facialHair, ChangeHairHueEntry[] entries)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_Price = price;
			m_Hair = hair;
			m_FacialHair = facialHair;
			m_Entries = entries;

			from.CloseGump(typeof(HairstylistBuyGump));
			from.CloseGump(typeof(ChangeHairHueGump));
			from.CloseGump(typeof(ChangeHairstyleGump));

			AddPage(0);

			AddBackground(100, 10, 350, 370, 2600);
			AddBackground(120, 54, 110, 270, 5100);

			AddHtmlLocalized(155, 25, 240, 30, 1011013, false, false); // <center>Hair Color Selection Menu</center>

			AddHtmlLocalized(150, 330, 220, 35, 1011014, false, false); // Dye my hair this color!
			AddButton(380, 330, 4005, 4007, 1, GumpButtonType.Reply, 0);

			for (int i = 0; i < entries.Length; ++i)
			{
				ChangeHairHueEntry entry = entries[i];

				AddLabel(130, 59 + (i * 22), entry.Hues[0] - 1, entry.Name);
				AddButton(207, 60 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, 1 + i);
			}

			for (int i = 0; i < entries.Length; ++i)
			{
				ChangeHairHueEntry entry = entries[i];
				var hues = entry.Hues;
				string name = entry.Name;

				AddPage(1 + i);

				for (int j = 0; j < hues.Length; ++j)
				{
					AddLabel(278 + ((j / 16) * 80), 52 + ((j % 16) * 17), hues[j] - 1, name);
					AddRadio(260 + ((j / 16) * 80), 52 + ((j % 16) * 17), 210, 211, false, (j * entries.Length) + i);
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 1)
			{
				var switches = info.Switches;

				if (switches.Length > 0)
				{
					int index = switches[0] % m_Entries.Length;
					int offset = switches[0] / m_Entries.Length;

					if (index >= 0 && index < m_Entries.Length)
					{
						if (offset >= 0 && offset < m_Entries[index].Hues.Length)
						{
							if (m_Hair && m_From.HairItemID > 0 || m_FacialHair && m_From.FacialHairItemID > 0)
							{
								if (m_Price > 0 && !Banker.Withdraw(m_From, m_Price))
								{
                                    if(m_Vendor != null)
									    m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
										// You cannot afford my services for that style.

									return;
								}

								int hue = m_Entries[index].Hues[offset];

								if (m_Hair)
								{
									m_From.HairHue = hue;
								}

								if (m_FacialHair)
								{
									m_From.FacialHairHue = hue;
								}
							}
							else
							{
                                if (m_Vendor != null)
                                {
                                    m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502623, m_From.NetState); // You have no hair to dye and you cannot use this.
                                }
                                else
                                {
                                    m_From.SendLocalizedMessage(502623);
                                }
							}
						}
					}
				}
				else
				{
                    if (m_Vendor != null)
                    {
                        m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState); // You decide not to change your hairstyle.
                    }
                    else
                    {
                        m_From.SendLocalizedMessage(1013009);
                    }
				}
			}
			else
			{
				// You decide not to change your hairstyle.
				m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
			}
		}
	}

	public class ChangeHairstyleEntry
	{
		public static readonly ChangeHairstyleEntry[] HairEntries = new[]
		{
			new ChangeHairstyleEntry(50700, 70 - 137, 20 - 60, 0x203B),
			new ChangeHairstyleEntry(60710, 193 - 260, 18 - 60, 0x2045),
			new ChangeHairstyleEntry(50703, 316 - 383, 25 - 60, 0x2044),
			new ChangeHairstyleEntry(60701, 70 - 137, 75 - 125, 0x203C),
			new ChangeHairstyleEntry(60900, 193 - 260, 85 - 125, 0x2047),
			new ChangeHairstyleEntry(60713, 320 - 383, 85 - 125, 0x204A),
			new ChangeHairstyleEntry(60702, 70 - 137, 140 - 190, 0x203D),
			new ChangeHairstyleEntry(1836, 173 - 260, 128 - 190, 0x2049),
			new ChangeHairstyleEntry(60901, 315 - 383, 150 - 190, 0x2048), new ChangeHairstyleEntry(0, 0, 0, 0)
		};

		public static readonly ChangeHairstyleEntry[] BeardEntries = new[]
		{
			new ChangeHairstyleEntry(50800, 120 - 187, 30 - 80, 0x2040),
			new ChangeHairstyleEntry(50904, 243 - 310, 33 - 80, 0x204B),
			new ChangeHairstyleEntry(50906, 120 - 187, 100 - 150, 0x204D),
			new ChangeHairstyleEntry(50801, 243 - 310, 95 - 150, 0x203E),
			new ChangeHairstyleEntry(50802, 120 - 187, 173 - 220, 0x203F),
			new ChangeHairstyleEntry(50905, 243 - 310, 165 - 220, 0x204C),
			new ChangeHairstyleEntry(50808, 120 - 187, 242 - 290, 0x2041), new ChangeHairstyleEntry(0, 0, 0, 0)
		};

        public static readonly ChangeHairstyleEntry[] HairEntriesElf = new ChangeHairstyleEntry[]
        {
			new ChangeHairstyleEntry( 0xEDF5, 0xC6E5, 70 - 137,   20 -  60,  0x2FC0, 0x2FC0 ),
			new ChangeHairstyleEntry( 0xEDF6, 0xC6E6, 198 - 260,  18 -  60,  0x2FC1, 0x2FC1 ),
			new ChangeHairstyleEntry( 0xEDF7, 0xC6E7, 316 - 383,  20 -  60,  0x2FC2, 0x2FC2 ),
			new ChangeHairstyleEntry( 0xEDDC, 0xC6CC, 70 - 137,   80 - 125,  0x2FCE, 0x2FCE ),
			new ChangeHairstyleEntry( 0xEDDD, 0xC6CD, 193 - 260,  85 - 125,  0x2FCF, 0x2FCF ),
			new ChangeHairstyleEntry( 0xEDDF, 0xC6CF, 320 - 383,  85 - 125,  0x2FD1, 0x2FD1 ),
			new ChangeHairstyleEntry( 0xEDDA, 0xC6E4, 70 - 137,   147 - 190, 0x2FCC, 0x2FBF ),
			new ChangeHairstyleEntry( 0xEDDE, 0xC6CB, 196 - 260,  142 - 190, 0x2FD0, 0x2FCD ),
            new ChangeHairstyleEntry( -1, -1, -1, -1 ),
			new ChangeHairstyleEntry( 0, 0, 0, 0 )
        };

        public static readonly ChangeHairstyleEntry[] HairEntriesGargoyle = new ChangeHairstyleEntry[]
        {
			new ChangeHairstyleEntry( 0x7A0, 0x76C, 47 - 137,   12 -  60,  0x4261, 0x4258  ),
			new ChangeHairstyleEntry( 0x7A1, 0x76D, 170 - 260,  12 -  60,  0x4262, 0x4259 ),
			new ChangeHairstyleEntry( 0x79E, 0x773, 295 - 383,  12 -  60,  0x4273, 0x425A ),
			new ChangeHairstyleEntry( 0x7A2, 0x76E, 50 - 137,   68 - 125,  0x4274, 0x425B ),
			new ChangeHairstyleEntry( 0x79F, 0x774, 172 - 260,  70 - 125,  0x4275, 0x425C ),
			new ChangeHairstyleEntry( 0x77C, 0x775, 295 - 383,  81 - 125,  0x42AA, 0x425D ),
			new ChangeHairstyleEntry( 0x77D, 0x776, 47 - 137,   142 - 190, 0x42AB, 0x425E ),
			new ChangeHairstyleEntry( 0x77E, 0x777, 172 - 260,  142 - 190, 0x42B1, 0x425F ),
            new ChangeHairstyleEntry( -1, -1, -1, -1 ),
			new ChangeHairstyleEntry( 0, 0, 0, 0 )
        };

        public static readonly ChangeHairstyleEntry[] BeardEntriesGargoyle = new ChangeHairstyleEntry[]
		{
			new ChangeHairstyleEntry( 0xC5E9, 120 - 187,  30 -  80, 0x42AD ),
			new ChangeHairstyleEntry( 0x770,  220 - 310,  23 -  80, 0x42AE ),
			new ChangeHairstyleEntry( 0xC5DA, 120 - 187, 100 - 150, 0x42AF ),
			new ChangeHairstyleEntry( 0xC5D7, 243 - 310,  95 - 150, 0x42B0 ),
			new ChangeHairstyleEntry( 0, 0, 0, 0 )
		};

        private int m_ItemID_Male;
        private int m_ItemID_Female;
        private int m_GumpID_Male;
        private int m_GumpID_Female;
        private int m_X, m_Y;

        public int ItemID_Male { get { return m_ItemID_Male; } }
        public int ItemID_Female { get { return m_ItemID_Female; } }
        public int GumpID_Male { get { return m_GumpID_Male; } }
        public int GumpID_Female { get { return m_GumpID_Female; } }
        public int X { get { return m_X; } }
        public int Y { get { return m_Y; } }

        public ChangeHairstyleEntry(int gumpID, int x, int y, int itemID)
            : this(gumpID, gumpID, x, y, itemID, itemID)
        {
        }

        public ChangeHairstyleEntry(int gumpID_Female, int gumpID_Male, int x, int y, int itemID_Female, int itemID_Male)
        {
            m_GumpID_Male = gumpID_Male;
            m_GumpID_Female = gumpID_Female;
            m_X = x;
            m_Y = y;
            m_ItemID_Male = itemID_Male;
            m_ItemID_Female = itemID_Female;
        }
	}

	public class ChangeHairstyleGump : Gump
	{
		private readonly Mobile m_From;
		private readonly Mobile m_Vendor;
		private readonly int m_Price;
		private readonly bool m_FacialHair;
		private readonly ChangeHairstyleEntry[] m_Entries;

        public bool m_Female;
        public GenderChangeToken m_Token;

        public ChangeHairstyleGump(Mobile from, Mobile vendor, int price, bool facialHair, ChangeHairstyleEntry[] entries)
            : this(from, vendor, price, facialHair, entries, null)
        {
        }

        public ChangeHairstyleGump(Mobile from, Mobile vendor, int price, bool facialHair, ChangeHairstyleEntry[] entries, GenderChangeToken token)
            : this(from.Female, from, vendor, price, facialHair, entries, token)
        {
        }

		public ChangeHairstyleGump(bool female, Mobile from, Mobile vendor, int price, bool facialHair, ChangeHairstyleEntry[] entries, GenderChangeToken token)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_Price = price;
			m_FacialHair = facialHair;
			m_Entries = entries;
            m_Female = female;

            m_Token = token;

			from.CloseGump(typeof(HairstylistBuyGump));
			from.CloseGump(typeof(ChangeHairHueGump));
			from.CloseGump(typeof(ChangeHairstyleGump));

			int tableWidth = (m_FacialHair ? 2 : 3);
			int tableHeight = ((entries.Length + tableWidth - (m_FacialHair ? 1 : 2)) / tableWidth);
			int offsetWidth = 123;
			int offsetHeight = (m_FacialHair ? 70 : 65);

			AddPage(0);

			AddBackground(0, 0, 81 + (tableWidth * offsetWidth), 145 + (tableHeight * offsetHeight), 2600);

			AddButton(45, 90 + (tableHeight * offsetHeight), 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(77, 90 + (tableHeight * offsetHeight), 90, 35, 1006044, false, false); // Ok

			AddButton(
				90 + (tableWidth * offsetWidth) - 180, 85 + (tableHeight * offsetHeight), 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(
				90 + (tableWidth * offsetWidth) - 148, 85 + (tableHeight * offsetHeight), 90, 35, 1006045, false, false); // Cancel

			if (!facialHair)
			{
				AddHtmlLocalized(50, 15, 350, 20, 1018353, false, false); // <center>New Hairstyle</center>
			}
			else
			{
				AddHtmlLocalized(55, 15, 200, 20, 1018354, false, false); // <center>New Beard</center>
			}

			for (int i = 0; i < entries.Length; ++i)
			{
				int xTable = i % tableWidth;
				int yTable = i / tableWidth;
                int gumpID = female ? entries[i].GumpID_Female : entries[i].GumpID_Male;

                if (gumpID == -1)
                    continue;

                if (gumpID != 0)
				{
					AddRadio(40 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 208, 209, false, i);
					AddBackground(87 + (xTable * offsetWidth), 50 + (yTable * offsetHeight), 50, 50, 2620);
					AddImage(
                        87 + (xTable * offsetWidth) + entries[i].X, 50 + (yTable * offsetHeight) + entries[i].Y, gumpID);
				}
				else if (!facialHair)
				{
					AddRadio(40 + ((xTable) * offsetWidth), 240, 208, 209, false, i);
					AddHtmlLocalized(60 + ((xTable) * offsetWidth), 240, 200, 40, 1011064, false, false); // Bald
				}
				else
				{
					AddRadio(40 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 208, 209, false, i);
					//AddHtmlLocalized(60 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 85, 35, 1011064, false, false); // Bald
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
            if (!m_FacialHair || !m_Female)
            {
                if (info.ButtonID == 1)
                {
                    var switches = info.Switches;

                    if (switches.Length > 0)
                    {
                        int index = switches[0];
                        bool female = m_Female;

                        if (index >= 0 && index < m_Entries.Length)
                        {
                            ChangeHairstyleEntry entry = m_Entries[index];

                            if (m_From is PlayerMobile)
                            {
                                ((PlayerMobile)m_From).SetHairMods(-1, -1);
                            }

                            int hairID = m_From.HairItemID;
                            int facialHairID = m_From.FacialHairItemID;
                            int itemID = female ? entry.ItemID_Female : entry.ItemID_Male;

                            if (itemID == 0)
                            {
                                bool invalid = m_FacialHair ? (facialHairID == 0) : (hairID == 0);

                                if (!invalid)
                                {
                                    if (m_Token != null)
                                    {
                                        m_Token.OnChangeHairstyle(m_From, m_FacialHair, 0);
                                        return;
                                    }

                                    if (Banker.Withdraw(m_From, m_Price, true))
                                    {
                                        if (m_FacialHair)
                                        {
                                            m_From.FacialHairItemID = 0;
                                        }
                                        else
                                        {
                                            m_From.HairItemID = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (m_Vendor != null)
                                        {
                                            m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
                                            // You cannot afford my services for that style.
                                        }
                                        else
                                        {
                                            m_From.SendLocalizedMessage(1042293);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                bool invalid = m_FacialHair ? facialHairID > 0 && facialHairID == itemID : hairID > 0 && hairID == itemID;

                                if (!invalid)
                                {
                                    if (m_Price <= 0 || Banker.Withdraw(m_From, m_Price))
                                    {
                                        if (m_Token != null)
                                        {
                                            m_Token.OnChangeHairstyle(m_From, m_FacialHair, itemID);
                                            return;
                                        }

                                        if (m_FacialHair)
                                        {
                                            m_From.FacialHairItemID = itemID;

                                            if (itemID != 0)
                                            {
                                                m_From.FacialHairHue = m_From.HairHue;
                                            }
                                        }
                                        else
                                        {
                                            m_From.HairItemID = itemID;
                                        }
                                    }
                                    else
                                    {
                                        if (m_Vendor != null)
                                        {
                                            m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
                                            // You cannot afford my services for that style.
                                        }
                                        else
                                        {
                                            m_From.SendLocalizedMessage(1042293);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_Vendor != null)
                        {
                            // You decide not to change your hairstyle.
                            m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
                        }
                        else
                        {
                            m_From.SendLocalizedMessage(1013009); // You decide not to change your hairstyle. 
                        }
                    }
                }
                else
                {
                    if (m_Vendor != null)
                    {
                        // You decide not to change your hairstyle.
                        m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
                    }
                    else
                    {
                        m_From.SendLocalizedMessage(1013009); // You decide not to change your hairstyle. 
                    }
                }
            }

            if (m_Token != null)
                m_Token.OnFailedHairstyle(m_From, m_FacialHair);
		}
	}
}