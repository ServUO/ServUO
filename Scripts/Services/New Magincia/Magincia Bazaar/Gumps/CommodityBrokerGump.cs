using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
	public class CommodityBrokerGump : BaseBazaarGump
	{
		private CommodityBroker m_Broker;
		
		public CommodityBrokerGump(CommodityBroker broker, Mobile from)
		{
			m_Broker = broker;
			
			AddHtmlLocalized(205, 10, 200, 18, 1150636, RedColor16, false, false); // Commodity Broker
			
			if(m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
				AddHtml(173, 40, 173, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false);
			else
				AddHtmlLocalized(180, 40, 200, 18, 1150314, BlueColor16, false, false); // This Shop Has No Name

            AddHtml(173, 65, 173, 18, Color(FormatBrokerName(String.Format("Proprieter: {0}", broker.Name)), BlueColor), false, false);
			
			AddHtmlLocalized(215, 100, 200, 18, 1150328, GreenColor16, false, false); // OWNER MENU
			
			AddButton(150, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(190, 150, 200, 18, 1150392, OrangeColor16, false, false); // INFORMATION
			
			AddHtmlLocalized(38, 180, 200, 18, 1150199, RedColor16, false, false); // Broker Account Balance
			AddHtml(190, 180, 300, 18, FormatAmt(broker.BankBalance), false, false);
			
			int balance = Banker.GetBalance(from);
			AddHtmlLocalized(68, 200, 200, 18, 1150149, GreenColor16, false, false); // Your Bank Balance:
			AddHtml(190, 200, 200, 18, FormatAmt(balance), false, false);
			
			AddHtmlLocalized(32, 230, 200, 18, 1150329, OrangeColor16, false, false); // Broker Sales Comission
            AddHtmlLocalized(190, 230, 100, 18, 1150330, 0x000000, false, false); // 5%
			
			AddHtmlLocalized(109, 250, 200, 18, 1150331, OrangeColor16, false, false); // Weekly Fee:
			AddHtml(190, 250, 100, 18, FormatAmt(broker.GetWeeklyFee()), false, false);
			
			AddHtmlLocalized(113, 280, 200, 18, 1150332, OrangeColor16, false, false); // Shop Name:
            AddBackground(190, 280, 285, 22, 9350);
			AddTextEntry(191, 280, 285, 20, LabelHueBlue, 0, m_Broker.Plot.ShopName == null ? "" : m_Broker.Plot.ShopName);
			AddButton(480, 280, 4014, 4016, 2, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(85, 305, 150, 18, 1150195, OrangeColor16, false, false); // Withdraw Funds
            AddBackground(190, 305, 285, 22, 9350);
			AddTextEntry(191, 305, 285, 20, LabelHueBlue, 1, "");
            AddButton(480, 305, 4014, 4016, 3, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(97, 330, 150, 18, 1150196, OrangeColor16, false, false); // Deposit Funds
            AddBackground(190, 330, 285, 22, 9350);
			AddTextEntry(191, 330, 285, 20, LabelHueBlue, 2, "");
            AddButton(480, 330, 4014, 4016, 4, GumpButtonType.Reply, 0);

            AddButton(150, 365, 4005, 4007, 5, GumpButtonType.Reply, 0);
			AddHtmlLocalized(190, 365, 200, 18, 1150192, OrangeColor16, false, false); // ADD TO INVENTORY

            AddButton(150, 390, 4005, 4007, 6, GumpButtonType.Reply, 0);
			AddHtmlLocalized(190, 390, 300, 18, 1150193, OrangeColor16, false, false); // VIEW INVENTORY / REMOVE ITEMS

            AddButton(150, 415, 4005, 4007, 7, GumpButtonType.Reply, 0);
			AddHtmlLocalized(190, 415, 300, 18, 1150194, OrangeColor16, false, false); // SET PRICES AND LIMITS
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(m_Broker == null || m_Broker.Plot == null)
				return;
			
			switch(info.ButtonID)
			{
				default:
				case 0: return;
				case 1:
					from.SendGump(new CommodityBrokerGump(m_Broker, from));
					from.SendGump(new BazaarInformationGump(0, 1150637));
					return;
				case 2: // Set Shop Name
					TextRelay tr = info.TextEntries[0];
					string text = tr.Text;
						
					if(!m_Broker.Plot.TrySetShopName(from, text))
						from.SendLocalizedMessage(1150775); // Shop names are limited to 40 characters in length. Shop names must pass an obscenity filter check. The text you have entered is not valid.
					break;
				case 3: // Withdraw Funds
					TextRelay tr1 = info.TextEntries[1];
					string text1 = tr1.Text;
					int amount = 0;
					
					try
					{
						amount = Convert.ToInt32(text1);
					}
					catch{}
					
					if(amount > 0)
					{
						m_Broker.TryWithdrawFunds(from, amount);
					}
					break;
				case 4: // Deposit Funds
					TextRelay tr2 = info.TextEntries[2];
					string text2 = tr2.Text;
					int amount1 = 0;
					
					try
					{
						amount1 = Convert.ToInt32(text2);
					}
					catch{}
					
					if(amount1 > 0)
					{
						m_Broker.TryDepositFunds(from, amount1);
					}
                    break;
				case 5: // ADD TO INVENTORY
					if(m_Broker.BankBalance < 0)
					{
						from.SendGump(new BazaarInformationGump(1150623, 1150615));
						return;
					}
					
					from.Target = new InternalTarget(m_Broker);
					from.SendGump(new CommodityTargetGump(m_Broker));
					return;
				case 6: // VIEW INVENTORY / REMOVE ITEMS
					if(m_Broker.BankBalance < 0)
					{
						from.SendGump(new BazaarInformationGump(1150623, 1150615));
						return;
					}
					from.SendGump(new ViewInventoryGump(m_Broker));
					return;
				case 7: // SET PRICES AND LIMITS
					if(m_Broker.BankBalance < 0)
					{
						from.SendGump(new BazaarInformationGump(1150623, 1150615));
						return;
					}
					from.SendGump(new SetPricesAndLimitsGump(m_Broker));
					return;
			}
			
			from.SendGump(new CommodityBrokerGump(m_Broker, from));
		}
		
		public class InternalTarget : Target
		{
			private CommodityBroker m_Broker;
            private bool m_HasPickedCommodity;
			
            public InternalTarget(CommodityBroker broker) : this(broker, false)
            {
            }

			public InternalTarget(CommodityBroker broker, bool haspicked) : base(-1, false, TargetFlags.None)
			{
				m_Broker = broker;
                m_HasPickedCommodity = haspicked;
			}
			
			protected override void OnTarget(Mobile from, object targeted)
			{
                if (from.HasGump(typeof(CommodityTargetGump)))
                    from.CloseGump(typeof(CommodityTargetGump));

				if(targeted is Item && (targeted is ICommodity || targeted is CommodityDeed))
				{
					Item item = (Item)targeted;

                    if (item.IsChildOf(from.Backpack))
                    {
                        if (item is CommodityDeed && ((CommodityDeed)item).Commodity == null)
                        {
                            from.SendLocalizedMessage(1150222); // That item is not a commodity that a broker can trade.
                        }
                        else if (m_Broker.CommodityEntries.Count < CommodityBroker.MaxEntries)
                        {
                            if (!m_Broker.TryAddBrokerEntry(item, from))    // Adds new entry									// Already has an entry, adds item to inventory for selling
                                m_Broker.AddInventory(from, item);          // or adds to the stock

                            m_HasPickedCommodity = true;
                        }
                        else
                        {
                            from.SendMessage("You can only have a maximum of {0} commodities on your merchant at a time.", CommodityBroker.MaxEntries.ToString());
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                    }
				}
				else
				{
					from.SendLocalizedMessage(1150222); // That item is not a commodity that a broker can trade.
				}

                from.Target = new InternalTarget(m_Broker, m_HasPickedCommodity);
			}
			
			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
                if (from.HasGump(typeof(CommodityTargetGump)))
                    from.CloseGump(typeof(CommodityTargetGump));

                if(m_HasPickedCommodity)
                    from.SendGump(new SetPricesAndLimitsGump(m_Broker));
                else
				    from.SendGump(new CommodityBrokerGump(m_Broker, from));
			}
		}
	}

	public class CommodityTargetGump : BaseBazaarGump
	{
		private CommodityBroker m_Broker;
		
		public CommodityTargetGump(CommodityBroker broker)
		{
			m_Broker = broker;
			
			/* Target commodity items or filled commodity deeds in your backpack to add them to the 
			 * broker's inventory. These items will be retrievable, and the broker will not trade them 
			 * until you establish prices.<br><br>When done, press the [ESC] key to cancel your targeting 
			 * cursor, or click the MAIN MENU button below.*/
			 
			AddHtmlLocalized(10, 50, 500, 400, 1150209, OrangeColor16, false, false);

            AddButton(10, 490, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
            {
                if (from.Target is CommodityBrokerGump.InternalTarget)
                    Target.Cancel(from);
            }
		}
	}

	public class SetPricesAndLimitsGump : BaseBazaarGump
	{
		private CommodityBroker m_Broker;
		private int m_Index;
		private int m_Page;

        public SetPricesAndLimitsGump(CommodityBroker broker) : this(broker, -1, 0) { }
		
		public SetPricesAndLimitsGump(CommodityBroker broker, int index, int page)
		{
			m_Broker = broker;
			m_Index = index;
			m_Page = page;

            AddHtmlLocalized(205, 10, 200, 18, 1150636, RedColor16, false, false); // Commodity Broker
			
			AddButton(150, 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(190, 40, 200, 18, 1150392, OrangeColor16, false, false); // INFORMATION
		
			AddHtmlLocalized(120, 65, 100, 18, 1150140, OrangeColor16, false, false); // ITEM
            AddHtmlLocalized(160, 65, 100, 18, 1150204, OrangeColor16, false, false); // BUY AT
            AddHtmlLocalized(220, 65, 100, 18, 1150645, OrangeColor16, false, false); // BUY LMT
            AddHtmlLocalized(315, 65, 100, 18, 1150205, OrangeColor16, false, false); // SELL AT
            AddHtmlLocalized(380, 65, 100, 18, 1150646, OrangeColor16, false, false); // SELL LMT
            AddHtmlLocalized(475, 65, 100, 18, 1150647, OrangeColor16, false, false); // EDIT
			
			int y = 85;
			int perPage = 10;
			
			if(index > -1)
				m_Page = index <= 0 ? 0 : index / perPage;

			int start = page * perPage;
            int count = 1;
			
			for(int i = start; i < broker.CommodityEntries.Count && /*index <= perPage * (m_Page + 1) &&*/ count <= perPage; i++)
			{	
				int col = i == m_Index ? YellowColor : OrangeColor;
                int col16 = col == YellowColor ? YellowColor16 : OrangeColor16;
				CommodityBrokerEntry entry = broker.CommodityEntries[i];

                AddHtmlLocalized(1, y, 150, 18, 1114514, String.Format("#{0}", entry.Label), col16, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
				AddHtml(160, y, 45, 18, AlignRight(Color(FormatAmt(entry.BuyPricePer), col)), false, false);
				AddHtml(220, y, 45, 18, AlignRight(Color(FormatAmt(entry.BuyLimit), col)), false, false);
				AddHtml(315, y, 80, 18, Color(FormatAmt(entry.SellPricePer), col), false, false);
				AddHtml(380, y, 80, 18, Color(FormatAmt(entry.SellLimit), col), false, false);
				
				AddButton(475, y, 4014, 4016, 2 + i, GumpButtonType.Reply, 0);
					
				y += 22;
                count++;
			}
			
			if(m_Page > 0) // back
				AddButton(162, 321, 4014, 4016, 400, GumpButtonType.Reply, 0);
				
			if(broker.CommodityEntries.Count - start > perPage) // forward
				AddButton(390, 321, 4005, 4007, 401, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(124, 345, 100, 18, 1150204, BlueColor16, false, false); // BUY AT
			AddBackground(190, 345, 300, 18, 9350);
            AddTextEntry(191, 344, 320, 22, LabelHueBlue, 0, "");
				
			AddHtmlLocalized(114, 370, 100, 18, 1150645, BlueColor16, false, false); // BUY LMT
			AddBackground(190, 370, 300, 18, 9350);
            AddTextEntry(191, 369, 320, 22, LabelHueBlue, 1, "");
				
			AddHtmlLocalized(118, 395, 100, 18, 1150205, BlueColor16, false, false); // SELL AT
			AddBackground(190, 395, 300, 18, 9350);
            AddTextEntry(191, 394, 320, 22, LabelHueBlue, 2, "");
				
			AddHtmlLocalized(108, 420, 100, 18, 1150646, BlueColor16, false, false); // SELL LMT
			AddBackground(190, 420, 300, 18, 9350);
            AddTextEntry(191, 419, 320, 22, LabelHueBlue, 3, "");
				
			AddButton(140, 450, 4005, 4007, 500, GumpButtonType.Reply, 0); 
			AddHtmlLocalized(190, 450, 200, 18, 1150232, RedColor16, false, false); // SET PRICES
			
			AddButton(10, 495, 4014, 4016, 501, GumpButtonType.Reply, 0); 
			AddHtmlLocalized(50, 495, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(info.ButtonID == 0)
				return;
			if(info.ButtonID == 1)
			{
				from.SendGump(new SetPricesAndLimitsGump(m_Broker, m_Index, m_Page));
				from.SendGump(new BazaarInformationGump(0, 1150644));
				return;
			}
			else if (info.ButtonID == 400) // back page
			{
				from.SendGump(new SetPricesAndLimitsGump(m_Broker, -1, m_Page - 1));
				return;
			}
			else if (info.ButtonID == 401) // forward page
			{
				from.SendGump(new SetPricesAndLimitsGump(m_Broker, -1, m_Page + 1));
				return;
			}
			else if (info.ButtonID == 500) // SET PRICES
			{
				if(m_Index >= 0 && m_Index < m_Broker.CommodityEntries.Count)
				{
					CommodityBrokerEntry entry = m_Broker.CommodityEntries[m_Index];
					
					int buyAt = entry.BuyPricePer;
					int buyLmt = entry.BuyLimit;
					int sellAt = entry.SellPricePer;
					int sellLmt = entry.SellLimit;
					
					TextRelay relay1 = info.TextEntries[0];
					TextRelay relay2 = info.TextEntries[1];
					TextRelay relay3 = info.TextEntries[2];
					TextRelay relay4 = info.TextEntries[3];
                    try
                    {
                        buyAt = Convert.ToInt32(relay1.Text);
                    }
                    catch { }
                    try
                    {
                        buyLmt = Convert.ToInt32(relay2.Text);
                    }
                    catch { }
                    try
                    {
                        sellAt = Convert.ToInt32(relay3.Text);
                    }
                    catch { }
					try
					{
						sellLmt = Convert.ToInt32(relay4.Text);
					}
					catch { }
					
					if(buyLmt < 0 || buyLmt > 60000 || sellLmt < 0 || sellLmt > 60000)
						from.SendLocalizedMessage(1150776); // You have entered an invalid numeric value. Negative values are not allowed. Trade quantities are limited to 60,000 per transaction.
					else
					{
						entry.BuyPricePer = buyAt;
						entry.BuyLimit = buyLmt;
						entry.SellPricePer = sellAt;
						entry.SellLimit = sellLmt;
					}
				}
				else
					from.SendLocalizedMessage(1150642); // You did not select a commodity.
			}
			else if (info.ButtonID == 501) // Main Menu
			{
				from.SendGump(new CommodityBrokerGump(m_Broker, from));
				return;
			}
			else
			{
				int id = info.ButtonID - 2;
				
				if(id >= 0 && id < m_Broker.CommodityEntries.Count)
					m_Index = id;
			}
			
			from.SendGump(new SetPricesAndLimitsGump(m_Broker, m_Index, m_Page));
		}
	}

	public class ViewInventoryGump : BaseBazaarGump
	{
		private CommodityBroker m_Broker;
		private int m_Index;
		private int m_Page;
		
		public ViewInventoryGump(CommodityBroker broker) : this(broker, -1)
		{
		}
		
		public ViewInventoryGump(CommodityBroker broker, int index) : this(broker, index, 0)
		{
		}
		
		public ViewInventoryGump(CommodityBroker broker, int index, int page)
		{
			m_Broker = broker;
			m_Index = index;
			m_Page = page;
			
			AddHtmlLocalized(205, 10, 200, 18, 1150636, RedColor16, false, false); // Commodity Broker
            AddHtmlLocalized(10, 30, 500, 60, 1150867, OrangeColor16, false, false); // Click the button next to the commodity...
			
			AddHtmlLocalized(150, 100, 150, 18, 1150140, OrangeColor16, false, false); // ITEM
			AddHtmlLocalized(240, 100, 150, 18, 1150201, OrangeColor16, false, false); // IN STOCK
			AddHtmlLocalized(320, 100, 150, 18, 1150639, OrangeColor16, false, false); // SELECT
			AddHtmlLocalized(420, 100, 150, 18, 1150855, OrangeColor16, false, false); // DOWN
			AddHtmlLocalized(480, 100, 150, 18, 1150856, OrangeColor16, false, false); // UP
			
			int y = 125;
			int perPage = 10;
			
			if(index > -1)
				m_Page = index <= 0 ? 0 : index / perPage;

			int start = page * perPage;
			int count = 1;
			
			for(int i = start; i < broker.CommodityEntries.Count && /*index <= perPage &&*/ count <= perPage; i++)
			{
				int col = i == m_Index ? YellowColor : OrangeColor;
                int col16 = col == YellowColor ? YellowColor16 : OrangeColor16;
				CommodityBrokerEntry entry = broker.CommodityEntries[i];
				
                AddHtmlLocalized(1, y, 180, 18, 1114514, String.Format("#{0}", entry.Label), col16, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
				AddHtml(240, y, 100, 18, AlignRight(Color(FormatAmt(entry.Stock), col)), false, false);
				
				AddButton(320, y, 4014, 4016, 1000 + i, GumpButtonType.Reply, 0); // SELECT
				
				if(i > 0)
                    AddButton(480, y, 250, 251, 1500 + i, GumpButtonType.Reply, 0); // UP
					
				if(i < broker.CommodityEntries.Count - 1)
                    AddButton(420, y, 252, 253, 2000 + i, GumpButtonType.Reply, 0); // DOWN
					
				y += 22;
				count++;
			}
			
			if(m_Page > 0) // back
				AddButton(162, 350, 4014, 4016, 400, GumpButtonType.Reply, 0);
				
			if(broker.CommodityEntries.Count - start > perPage) // forward
				AddButton(390, 350, 4005, 4007, 401, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(108, 415, 150, 18, 1150202, OrangeColor16, false, false); // WITHDRAW
			AddBackground(190, 415, 285, 22, 9350);
			AddTextEntry(191, 415, 284, 20, 0, 0, "");
			AddButton(480, 415, 4014, 4016, 1, GumpButtonType.Reply, 0);

            if (index > -1)
            {
                AddHtml(50, 460, 200, 18, Color("Remove from Inventory", RedColor), false, false);
                AddButton(10, 460, 4020, 4022, 3, GumpButtonType.Reply, 0);
            }

            AddButton(10, 490, 4014, 4016, 501, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 100, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			switch(info.ButtonID)
			{
                case 0: return;
				case 1: // Withdraw
					if(m_Index >= 0 && m_Index < m_Broker.CommodityEntries.Count)
					{
						CommodityBrokerEntry entry = m_Broker.CommodityEntries[m_Index];
						int amount = 0;
						
						TextRelay relay = info.TextEntries[0];
						
						try
						{
							amount = Convert.ToInt32(relay.Text);
						}
						catch {}
						
						if(amount <= 0 || amount > entry.Stock)
							from.SendLocalizedMessage(1150215); // You have entered an invalid value, or a non-numeric value. Please try again.
						else
							m_Broker.WithdrawInventory(from, amount, entry);
					}
					else
						from.SendLocalizedMessage(1150642); // You did not select a commodity.
					break;
				case 2: // Main Menu
					from.SendGump(new CommodityBrokerGump(m_Broker, from));
					return;
				case 3: // Remove Entry
					if(m_Index >= 0 && m_Index < m_Broker.CommodityEntries.Count)
					{
						from.SendGump(new ConfirmRemoveEntryGump(m_Broker, m_Index));
						return;
					}
					else
						from.SendLocalizedMessage(1150642); // You did not select a commodity.
					break;
				case 400:
					from.SendGump(new ViewInventoryGump(m_Broker, -1, m_Page - 1));
					return;
				case 401:
					from.SendGump(new ViewInventoryGump(m_Broker, -1, m_Page + 1));
					return;
                case 501:
                    from.SendGump(new CommodityBrokerGump(m_Broker, from));
                    return;
				default:
					{
						if(info.ButtonID < 1500) // SELECT ENTRY
						{
							int id = info.ButtonID - 1000;

                            if (id >= 0 && id < m_Broker.CommodityEntries.Count)
                            {
                                from.SendGump(new ViewInventoryGump(m_Broker, id, m_Page));
                                return;
                            }
						}
						else if (info.ButtonID < 2000) // UP
						{
							int id = info.ButtonID - 1500;
							
							if(id > 0 && id < m_Broker.CommodityEntries.Count)
							{
								CommodityBrokerEntry entry = m_Broker.CommodityEntries[id];
                                int idx = m_Broker.CommodityEntries.IndexOf(entry);

                                m_Broker.CommodityEntries.Remove(entry);
                                m_Broker.CommodityEntries.Insert(idx - 1, entry);
								//m_Broker.CommodityEntries[id] = m_Broker.CommodityEntries[id-1];
								//m_Broker.CommodityEntries[id-1] = entry;
							}
						}
						else // DOWN
						{
							int id = info.ButtonID - 2000;
							
							if(id >= 0 && id < m_Broker.CommodityEntries.Count - 1)
							{
								CommodityBrokerEntry entry = m_Broker.CommodityEntries[id];
                                int idx = m_Broker.CommodityEntries.IndexOf(entry);

                                m_Broker.CommodityEntries.Remove(entry);
                                m_Broker.CommodityEntries.Insert(idx + 1, entry);
								//m_Broker.CommodityEntries[id] = m_Broker.CommodityEntries[id+1];
								//m_Broker.CommodityEntries[id+1] = entry;
							}
						}
						
						break;
					}
			}
			
			from.SendGump(new ViewInventoryGump(m_Broker, m_Index));
		}
	}

	public class CommodityInventoryGump : BaseBazaarGump
	{
		private CommodityBroker m_Broker;
		private List<CommodityBrokerEntry> m_Entries;
		private bool m_Buy;
		private int m_Page;
		private int m_Index;
		
		public CommodityInventoryGump(CommodityBroker broker) : this(broker, -1, true, 0)
		{
		}
		
		public CommodityInventoryGump(CommodityBroker broker, int index, bool buy, int page) : base(655, 520)
		{
			m_Broker = broker;
			m_Entries = broker.CommodityEntries;
			m_Buy = buy;
			m_Page = page;
			m_Index = index;
			
            AddHtmlLocalized(205, 10, 200, 18, 1150636, RedColor16, false, false); // Commodity Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
                AddHtml(173, 40, 173, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false); // "~1_NAME~"
            else
                AddHtmlLocalized(180, 40, 200, 18, 1150314, BlueColor16, false, false); // This Shop Has No Name

            AddHtml(173, 65, 173, 18, Color(FormatBrokerName(String.Format("Proprietor: {0}", broker.Name)), BlueColor), false, false);
			
			AddButton(150, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(190, 100, 150, 18, 1150392, OrangeColor16, false, false); // INFORMATION

            AddHtmlLocalized(10, 125, 75, 18, 1150656, OrangeColor16, false, false); // BUY
            AddHtmlLocalized(60, 125, 75, 18, 1150659, OrangeColor16, false, false); // PRICE
            AddHtmlLocalized(120, 125, 75, 18, 1150658, OrangeColor16, false, false); // LIMIT
            AddHtmlLocalized(330, 125, 75, 18, 1150655, OrangeColor16, false, false); // COMMODITY
            AddHtmlLocalized(490, 125, 75, 18, 1150659, OrangeColor16, false, false); // PRICE
            AddHtmlLocalized(550, 125, 75, 18, 1150658, OrangeColor16, false, false); // LIMIT
            AddHtmlLocalized(610, 125, 75, 18, 1150657, OrangeColor16, false, false); // SELL
			
			int y = 150;
			int perPage = 10;
			
			if(index > -1)
				m_Page = index <= 0 ? 0 : index / perPage;
				
			int start = page * perPage;
			int count = 0;
			
			for(int i = start; i < m_Entries.Count && /*index < perPage*/ count <= perPage; i++)
			{
				CommodityBrokerEntry entry = m_Entries[i];
				
				int buyCol = index == i && buy ? YellowColor : OrangeColor;
				int sellCol = index == i && !buy ? YellowColor : OrangeColor;
				int stock = entry.Stock;
				
				int buyLimit = entry.ActualSellLimit;
				int sellLimit = entry.ActualBuyLimit;
				string buyAmount = entry.SellPricePer > 0 ? FormatAmt(entry.SellPricePer) : "N/A";
				string sellAmount = entry.BuyPricePer > 0 ? FormatAmt(entry.BuyPricePer) : "N/A";
				
				// Sell to player
				AddHtml(60, y, 60, 18, Color(buyAmount, buyCol), false, false);
				if(entry.SellPricePer > 0 && entry.SellLimit > 0)
					AddHtml(120, y, 150, 18, Color(FormatAmt(buyLimit), buyCol), false, false);
				
				// what we're selling/buying
                AddHtmlLocalized(200, y, 215, 18, 1114514, String.Format("#{0}", entry.Label), OrangeColor16, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
				
				// buy from player
				AddHtml(490, y, 75, 18, AlignRight(Color(sellAmount, sellCol)), false, false);
				if(entry.BuyPricePer > 0 && entry.BuyLimit > 0)
					AddHtml(550, y, 150, 18, AlignRight(Color(FormatAmt(sellLimit), sellCol)), false, false);
				
				//Buttons
				if(entry.PlayerCanBuy(0))
					AddButton(10, y, 4005, 4007, 2 + i, GumpButtonType.Reply, 0);
					
				if(entry.PlayerCanSell(0))
					AddButton(610, y, 4014, 4016, 102 + i, GumpButtonType.Reply, 0);
					
				y += 22;
				count++;
			}

            if (m_Page > 0) // back
            {
                AddHtmlLocalized(100, 410, 150, 18, 1044044, OrangeColor16, false, false);
                AddButton(60, 410, 4014, 4016, 400, GumpButtonType.Reply, 0);
            }

            if (m_Entries.Count - start > perPage) // forward
            {
                AddHtmlLocalized(340, 410, 150, 18, 1044045, OrangeColor16, false, false);
                AddButton(425, 410, 4005, 4007, 401, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(240, 445, 150, 18, 1150662, BlueColor16, false, false); // QUANTITY
			AddBackground(315, 445, 325, 22, 9350);
			AddTextEntry(316, 445, 300, 20, 0, 0, "");
				
			AddButton(275, 470, 4005, 4007, 500, GumpButtonType.Reply, 0);
            AddHtmlLocalized(315, 470, 100, 18, 1077761, OrangeColor16, false, false); // TRADE
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			switch(info.ButtonID)
			{
				case 0: return;
				case 1: 
					from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, m_Page));
					from.SendGump(new BazaarInformationGump(1150655, 1150654));
					return;
				case 400:
					from.SendGump(new CommodityInventoryGump(m_Broker, -1, m_Buy, m_Page - 1));
					return;
				case 401:
					from.SendGump(new CommodityInventoryGump(m_Broker, -1, m_Buy, m_Page + 1));
					return;
				case 500: // TRADE
					{
						if(m_Index >= 0 && m_Index < m_Entries.Count)
						{
							CommodityBrokerEntry entry = m_Entries[m_Index];
							
							if(!m_Broker.CommodityEntries.Contains(entry))
							{
								from.SendLocalizedMessage(1150244); // Transaction no longer available
								break;
							}
							
							int amount = 0;
							TextRelay relay = info.TextEntries[0];
							
							try
							{
								amount = Convert.ToInt32(relay.Text);
							}
							catch {}
							
							if(amount > 0)
							{
								if(m_Buy && entry.PlayerCanBuy(amount))
									from.SendGump(new ConfirmBuyCommodityGump(m_Broker, amount, entry, true));
								else if(!m_Buy && entry.PlayerCanSell(amount))
									from.SendGump(new ConfirmBuyCommodityGump(m_Broker, amount, entry, false));
                                else
                                    from.SendLocalizedMessage(1150215); // You have entered an invalid value, or a non-numeric value. Please try again.
								return;
							}
							else
							{
								from.SendLocalizedMessage(1150215); // You have entered an invalid value, or a non-numeric value. Please try again.
							}
						}
						else
							from.SendLocalizedMessage(1150642); // You did not select a commodity.
					}
					break;
				default:
					{
						if(info.ButtonID < 102)
						{
							int id = info.ButtonID - 2;

                            if (id >= 0 && id < m_Entries.Count && m_Broker.CommodityEntries.Contains(m_Entries[id]))
                            {
                                m_Buy = true;
                                m_Index = id;
                            }
                            else
                                from.SendLocalizedMessage(1150244); // Transaction no longer available
						}
						else
						{
							int id = info.ButtonID - 102;

                            if (id >= 0 && id < m_Entries.Count && m_Broker.CommodityEntries.Contains(m_Entries[id]))
                            {
                                m_Buy = false;
                                m_Index = id;
                            }
                            else
                                from.SendLocalizedMessage(1150244); // Transaction no longer available
						}
					}
					break;
			}

            from.SendGump(new CommodityInventoryGump(m_Broker, m_Index, m_Buy, 0));
		}
	}

	public class ConfirmRemoveEntryGump : BaseConfirmGump
	{
		private CommodityBroker m_Broker;
		private CommodityBrokerEntry m_Entry;
		private int m_Index;
		
		public override string TitleString { get { return "Remove Commodity Confirmation"; } }
		public override string LabelString { get { return "Are you sure you want to remove this entry from your commodity broker? Any unused stock will be placed in your bankbox."; } } 
		
		public ConfirmRemoveEntryGump(CommodityBroker broker, int index)
		{
			m_Broker = broker;
			m_Index = index;

            if (index >= 0 && index < m_Broker.CommodityEntries.Count)
                m_Entry = m_Broker.CommodityEntries[index];
		}
		
		public override void Confirm( Mobile from )
		{
			if(m_Entry != null && m_Broker.CommodityEntries.Contains(m_Entry))
				m_Broker.RemoveEntry(from, m_Entry);
				
			from.SendGump(new ViewInventoryGump(m_Broker));
		}
		
		public override void Refuse( Mobile from )
		{
			from.SendGump(new ViewInventoryGump(m_Broker, m_Index));
		}
	}
	
	public class ConfirmBuyCommodityGump : BaseBazaarGump
	{
		private CommodityBroker m_Broker;
		private int m_Amount;
		private CommodityBrokerEntry m_Entry;
		
		public ConfirmBuyCommodityGump(CommodityBroker broker, int amount, CommodityBrokerEntry entry, bool buy)
		{
			m_Broker = broker;
			m_Amount = amount;
			m_Entry = entry;

            AddHtmlLocalized(205, 10, 200, 18, 1150636, RedColor16, false, false); // Commodity Broker

            if (m_Broker.Plot.ShopName != null && m_Broker.Plot.ShopName.Length > 0)
                AddHtml(173, 40, 173, 18, Color(FormatStallName(m_Broker.Plot.ShopName), BlueColor), false, false); // "~1_NAME~"
            else
                AddHtmlLocalized(180, 40, 200, 18, 1150314, BlueColor16, false, false); // This Shop Has No Name

            AddHtml(173, 65, 173, 18, Color(FormatBrokerName(String.Format("Proprietor: {0}", broker.Name)), BlueColor), false, false);
			
			AddHtmlLocalized(10, 100, 500, 40, 1150666, RedColor16, false, false); // Please review the details of this transaction. If you wish to make this trade, click the TRADE button below. Otherwise, click the MAIN MENU button to return to the price list.
			
			if(buy)
			{
				int cost = entry.SellPricePer * amount;

				AddHtmlLocalized(10, 150, 260, 18, 1150245, OrangeColor16, false, false); // <DIV ALIGN=RIGHT>You are purchasing:</DIV>
                AddItem(230, 210, m_Broker.GetItemID(entry));
                
                AddHtmlLocalized(110, 350, 150, 18, 1150246, OrangeColor16, false, false); // Total Cost:
				AddHtml(180, 350, 150, 18, Color(FormatAmt(cost), OrangeColor), false, false);
				
				AddHtmlLocalized(170, 380, 120, 18, 1150247, OrangeColor16, false, false); // <DIV ALIGN=RIGHT>PURCHASE</DIV>
				AddButton(180, 380, 4005, 4007, 1, GumpButtonType.Reply, 0);
			}
			else
			{
				int cost = entry.BuyPricePer * amount;

                AddHtmlLocalized(10, 150, 260, 18, 1150250, OrangeColor16, false, false); // <DIV ALIGN=RIGHT>You are selling:</DIV>
                AddItem(230, 210, m_Broker.GetItemID(entry));

                AddHtmlLocalized(50, 350, 160, 18, 1150251, OrangeColor16, false, false); // Gold you will recieve:
				AddHtml(190, 350, 150, 18, Color(FormatAmt(cost), OrangeColor), false, false);
				
				AddHtmlLocalized(200, 380, 60, 18, 1150249, OrangeColor16, false, false); // <DIV ALIGN=RIGHT>SELL</DIV>
				AddButton(190, 380, 4005, 4007, 2, GumpButtonType.Reply, 0);
			}
			
			//AddHtmlLocalized(x, y, 120, 18, 1150248, OrangeColor, false, false); // <DIV ALIGN=RIGHT>CONFIRM</DIV>
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			switch(info.ButtonID)
			{
				default:
				case 0: break;
				case 1: //BUY
					m_Broker.TryBuyCommodity(from, m_Entry, m_Amount);
					break;
				case 2: //SELL
					m_Broker.TrySellCommodity(from, m_Entry, m_Amount);
					break;
			}
		}
	}
}