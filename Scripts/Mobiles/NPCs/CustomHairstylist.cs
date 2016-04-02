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
			from.SendGump(new HairstylistBuyGump(from, this, m_SellList));
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
								if (!Banker.Withdraw(m_From, m_Price))
								{
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
								m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502623, m_From.NetState);
									// You have no hair to dye and you cannot use this.
							}
						}
					}
				}
				else
				{
					// You decide not to change your hairstyle.
					m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
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
			new ChangeHairstyleEntry(60708, 70 - 137, 75 - 125, 0x203C),
			new ChangeHairstyleEntry(60900, 193 - 260, 85 - 125, 0x2047),
			new ChangeHairstyleEntry(60713, 320 - 383, 85 - 125, 0x204A),
			new ChangeHairstyleEntry(60702, 70 - 137, 140 - 190, 0x203D),
			new ChangeHairstyleEntry(60707, 193 - 260, 140 - 190, 0x2049),
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

		private readonly int m_ItemID;
		private readonly int m_GumpID;
		private readonly int m_X;
		private readonly int m_Y;

		public ChangeHairstyleEntry(int gumpID, int x, int y, int itemID)
		{
			m_GumpID = gumpID;
			m_X = x;
			m_Y = y;
			m_ItemID = itemID;
		}

		public int ItemID { get { return m_ItemID; } }
		public int GumpID { get { return m_GumpID; } }
		public int X { get { return m_X; } }
		public int Y { get { return m_Y; } }
	}

	public class ChangeHairstyleGump : Gump
	{
		private readonly Mobile m_From;
		private readonly Mobile m_Vendor;
		private readonly int m_Price;
		private readonly bool m_FacialHair;
		private readonly ChangeHairstyleEntry[] m_Entries;

		public ChangeHairstyleGump(Mobile from, Mobile vendor, int price, bool facialHair, ChangeHairstyleEntry[] entries)
			: base(50, 50)
		{
			m_From = from;
			m_Vendor = vendor;
			m_Price = price;
			m_FacialHair = facialHair;
			m_Entries = entries;

			from.CloseGump(typeof(HairstylistBuyGump));
			from.CloseGump(typeof(ChangeHairHueGump));
			from.CloseGump(typeof(ChangeHairstyleGump));

			int tableWidth = (m_FacialHair ? 2 : 3);
			int tableHeight = ((entries.Length + tableWidth - (m_FacialHair ? 1 : 2)) / tableWidth);
			int offsetWidth = 123;
			int offsetHeight = (m_FacialHair ? 70 : 65);

			AddPage(0);

			AddBackground(0, 0, 81 + (tableWidth * offsetWidth), 105 + (tableHeight * offsetHeight), 2600);

			AddButton(45, 45 + (tableHeight * offsetHeight), 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(77, 45 + (tableHeight * offsetHeight), 90, 35, 1006044, false, false); // Ok

			AddButton(
				81 + (tableWidth * offsetWidth) - 180, 45 + (tableHeight * offsetHeight), 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(
				81 + (tableWidth * offsetWidth) - 148, 45 + (tableHeight * offsetHeight), 90, 35, 1006045, false, false); // Cancel

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

				if (entries[i].GumpID != 0)
				{
					AddRadio(40 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 208, 209, false, i);
					AddBackground(87 + (xTable * offsetWidth), 50 + (yTable * offsetHeight), 50, 50, 2620);
					AddImage(
						87 + (xTable * offsetWidth) + entries[i].X, 50 + (yTable * offsetHeight) + entries[i].Y, entries[i].GumpID);
				}
				else if (!facialHair)
				{
					AddRadio(40 + ((xTable + 1) * offsetWidth), 240, 208, 209, false, i);
					AddHtmlLocalized(60 + ((xTable + 1) * offsetWidth), 240, 85, 35, 1011064, false, false); // Bald
				}
				else
				{
					AddRadio(40 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 208, 209, false, i);
					AddHtmlLocalized(60 + (xTable * offsetWidth), 70 + (yTable * offsetHeight), 85, 35, 1011064, false, false); // Bald
				}
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_FacialHair && (m_From.Female || m_From.Body.IsFemale))
			{
				return;
			}

			if (m_From.Race == Race.Elf)
			{
				m_From.SendMessage("This isn't implemented for elves yet.  Sorry!");
				return;
			}

			if (info.ButtonID == 1)
			{
				var switches = info.Switches;

				if (switches.Length > 0)
				{
					int index = switches[0];

					if (index >= 0 && index < m_Entries.Length)
					{
						ChangeHairstyleEntry entry = m_Entries[index];

						if (m_From is PlayerMobile)
						{
							((PlayerMobile)m_From).SetHairMods(-1, -1);
						}

						int hairID = m_From.HairItemID;
						int facialHairID = m_From.FacialHairItemID;

						if (entry.ItemID == 0)
						{
							if (m_FacialHair ? (facialHairID == 0) : (hairID == 0))
							{
								return;
							}

							if (Banker.Withdraw(m_From, m_Price))
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
								m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
									// You cannot afford my services for that style.
							}
						}
						else
						{
							if (m_FacialHair)
							{
								if (facialHairID > 0 && facialHairID == entry.ItemID)
								{
									return;
								}
							}
							else
							{
								if (hairID > 0 && hairID == entry.ItemID)
								{
									return;
								}
							}

							if (Banker.Withdraw(m_From, m_Price))
							{
								if (m_FacialHair)
								{
									m_From.FacialHairItemID = entry.ItemID;
								}
								else
								{
									m_From.HairItemID = entry.ItemID;
								}
							}
							else
							{
								m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1042293, m_From.NetState);
									// You cannot afford my services for that style.
							}
						}
					}
				}
				else
				{
					// You decide not to change your hairstyle.
					m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
				}
			}
			else
			{
				// You decide not to change your hairstyle.
				m_Vendor.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1013009, m_From.NetState);
			}
		}
	}
}