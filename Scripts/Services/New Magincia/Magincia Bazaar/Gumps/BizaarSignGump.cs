using Server;
using System;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Prompts;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
	public class StallLeasingGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public StallLeasingGump(Mobile from, MaginciaBazaarPlot plot) 
		{
			m_Plot = plot;
			
			AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
			AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddButton(225, 90, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(265, 90, 150, 18, 1150392, OrangeColor16, false, false); // INFORMATION
			
			AddHtmlLocalized(192, 135, 150, 18, 1150534, RedColor16, false, false); // This Stall:
			AddHtmlLocalized(265, 135, 150, 18, 1150530, m_Plot.PlotDef.ID, BlueColor16, false, false); // Stall ~1_NAME 
			
			AddHtmlLocalized(158, 160, 150, 18, 1150536, RedColor16, false, false); // Current Tenant:
			
			if(m_Plot.Owner == null)
				AddHtmlLocalized(265, 160, 150, 18, 1150542, BlueColor16, false, false); // Stall is Not Occupied
			else if(from == m_Plot.Owner)
				AddHtmlLocalized(265, 160, 200, 18, 1150539, BlueColor16, false, false); // You are leasing this stall
			else
				AddHtmlLocalized(265, 160, 150, 18, 1150541, m_Plot.Owner.Name, BlueColor16, false, false); // ~1_TOKEN~
			
			AddHtmlLocalized(184, 189, 150, 18, 1150332, RedColor16, false, false); // Shop Name:
			
			if(m_Plot.ShopName != null && m_Plot.ShopName.Length > 0)
				AddHtmlLocalized(265, 185, 200, 18, 1150312, m_Plot.ShopName, BlueColor16, false, false); // "~1_NAME~"
			else
				AddHtmlLocalized(265, 185, 200, 18, 1150314, BlueColor16, false, false); // This Shop Has No Name
				
			AddHtmlLocalized(160, 210, 150, 18, 1150388, RedColor16, false, false); // Lease Duration:
			AddHtmlLocalized(265, 210, 150, 18, 1150543, ((int)MaginciaBazaar.GetLongAuctionTime.TotalDays).ToString(), BlueColor16, false, false); // ~1_DAYS~ Days

            AddButton(225, 250, 4005, 4007, 2, GumpButtonType.Reply, 0);
			AddHtmlLocalized(265, 250, 200, 18, 1150555, OrangeColor16, false, false); // SEE TOP BIDS
			
			bool isOwner = m_Plot.IsOwner(from);
			
            AddButton(225, 274, 4005, 4007, 3, GumpButtonType.Reply, 0);
			AddHtmlLocalized(265, 274, 200, 18, 1150557, isOwner ? OrangeColor16 : GrayColor16, false, false); // MY STALL LEASE

            AddButton(225, 298, 4005, 4007, 4, GumpButtonType.Reply, 0);
			AddHtmlLocalized(265, 298, 200, 18, 1150556, OrangeColor16, false, false); // MY STALL BID
			
            AddButton(225, 322, 4005, 4007, 5, GumpButtonType.Reply, 0);
			AddHtmlLocalized(265, 322, 200, 18, 1150540, isOwner ? OrangeColor16 : GrayColor16, false, false); // MY BID MATCHING
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			switch(info.ButtonID)
			{
				default:
				case 0: break;
				case 1: 
					from.SendGump(new StallLeasingGump(from, m_Plot));
					from.SendGump(new BazaarInformationGump(-1, 1150391));
					break;
				case 2: // SEE TOP BIDS
					from.SendGump(new TopBidsGump(from, m_Plot));
					break;
				case 3: // MY STALL LEASE
                    if (m_Plot.IsOwner(from))
                        from.SendGump(new MyStallLeaseGump(from, m_Plot));
                    else
                        from.SendLocalizedMessage(1150685); // You are currently viewing a stall that you are not leasing. In order to set up or modify your stall, please use that stall's sign.
					break;
				case 4: // MY STALL BID
					from.SendGump(new StallBidGump(from, m_Plot));
					break;
				case 5: // MY BID MATCHING
					if(m_Plot.IsOwner(from))
                        from.SendGump(new MatchBidGump(from, m_Plot));
					else
						from.SendLocalizedMessage(1150685); // You are currently viewing a stall that you are not leasing. In order to set up or modify your stall, please use that stall's sign.
					break;
			}
		}
	}
	
	public class MyStallLeaseGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public MyStallLeaseGump(Mobile from, MaginciaBazaarPlot plot)
		{
			m_Plot = plot;

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddHtmlLocalized(138, 135, 150, 18, 1150387, RedColor16, false, false); // Your Stall:
			AddHtmlLocalized(210, 135, 150, 18, 1150530, m_Plot.PlotDef.ID, BlueColor16, false, false); // Stall ~1_NAME 
			
			AddHtmlLocalized(101, 160, 150, 18, 1150388, RedColor16, false, false); // Lease Duration:
			AddHtmlLocalized(210, 160, 150, 18, 1150543, ((int)MaginciaBazaar.GetLongAuctionTime.TotalDays).ToString(), BlueColor16, false, false); // ~1_DAYS~ Days
			
			if(m_Plot.Merchant == null)
				AddButton(175, 220, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(210, 220, 200, 18, 1150686, m_Plot.Merchant == null ? OrangeColor16 : GrayColor16, false, false); // HIRE ANIMAL BROKER
			
			if(m_Plot.Merchant == null)
                AddButton(175, 244, 4005, 4007, 2, GumpButtonType.Reply, 0);
			AddHtmlLocalized(210, 244, 200, 18, 1150687, m_Plot.Merchant == null ? OrangeColor16 : GrayColor16, false, false); // HIRE COMMODITY BROKER
			
			if(m_Plot.Merchant != null)
                AddButton(175, 268, 4005, 4007, 3, GumpButtonType.Reply, 0);
			AddHtmlLocalized(210, 268, 200, 18, 1150688, m_Plot.Merchant != null ? OrangeColor16 : GrayColor16, false, false); // FIRE BROKER

            AddButton(175, 292, 4005, 4007, 4, GumpButtonType.Reply, 0);
			AddHtmlLocalized(210, 292, 200, 18, 1150689, OrangeColor16, false, false); // ABANDON LEASE

            AddButton(10, 490, 4014, 4016, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(m_Plot == null || !m_Plot.IsOwner(from))
				return;
			
			switch(info.ButtonID)
			{
				default:
				case 0: break;
				case 1: // HIRE ANIMAL BROKER
					if(m_Plot.Merchant != null)
						break;
					if(m_Plot.HasTempMulti())
						from.SendGump(new ConfirmAddMultiGump(from, m_Plot, false));
					else
						from.SendGump(new HireBrokerGump(from, m_Plot, false));
					break;
				case 2: // HIRE COMMODITY BROKER
					if(m_Plot.Merchant != null)
						break;
					if(m_Plot.HasTempMulti())
						from.SendGump(new ConfirmAddMultiGump(from, m_Plot, true));
					else
						from.SendGump(new HireBrokerGump(from, m_Plot, true));
					break;
				case 3: // FIRE BROKER
					if(m_Plot.Merchant != null)
						from.SendGump(new ConfirmFireBrokerGump(from, m_Plot));
					break;
				case 4: // ABANDON LEASE
					from.SendGump(new ConfirmAbandonLeaseGump(from, m_Plot));
					break;
                case 5: // MAIN MENU
                    from.SendGump(new StallLeasingGump(from, m_Plot));
					break;
			}
		}
	}
	
	public class ConfirmAbandonLeaseGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public ConfirmAbandonLeaseGump(Mobile from, MaginciaBazaarPlot plot)
		{
			m_Plot = plot;
			
			AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
			AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			/*In order to abandon your lease, your stall must be empty. To clear out 
			 *your stall, you must first retrieve all inventory, funds, and personal 
			 *items from your broker and the stall area, then choose the Fire Broker 
			 *option to release the broker. Once the stall is cleared, return to this 
			 *menu to confirm the abandonment of your lease.*/
			 
			 /*By clicking the CONFIRM button below, you will abandon your lease on 
			  *this stall and receive no refund of your lease payment. A new lease 
			  *on this stall will be auctioned within 12 hours.*/
			
			AddHtmlLocalized(10, 100, 500, 100, m_Plot.Merchant != null ? 1150693 : 1150694, RedColor16, false, false);
			
			if(m_Plot.Merchant == null)
			{
                AddHtmlLocalized(215, 350, 150, 18, 1150695, OrangeColor16, false, false); // CONFIRM
				AddButton(175, 350, 4005, 4007, 1, GumpButtonType.Reply, 0);
			}
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(m_Plot == null || !m_Plot.IsOwner(from))
				return;
				
			if(info.ButtonID == 1 && m_Plot.Merchant == null)
				m_Plot.Abandon();
		}
	}
	
	public class ConfirmFireBrokerGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		private bool m_HasInventory;
		
		public ConfirmFireBrokerGump(Mobile from, MaginciaBazaarPlot plot) 
		{
			m_Plot = plot;
			m_HasInventory = false;
			
			if(plot.Merchant is PetBroker)
			{
				PetBroker broker = (PetBroker)plot.Merchant;

                if (broker.BrokerEntries.Count > 0 || broker.BankBalance > 0)
					m_HasInventory = true;
			}
			else if (plot.Merchant is CommodityBroker)
			{
				CommodityBroker broker = (CommodityBroker)plot.Merchant;
				
				if(broker.CommodityEntries.Count > 0 || broker.BankBalance > 0)
					m_HasInventory = true;
			}
			
			AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
			AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			/*In order to fire your broker, you must empty its inventory, withdraw all 
			 *funds, and remove any of your personal items from the broker and stall. 
			 *When ready, return to this menu selection to confirm.*/
			 
			 /*This option will release your hired broker from service and vacate your 
			  *stall. You will then be able to hire a different broker. Click the 
			  *CONFIRM button below if you wish to clear out your stall.*/
			
			AddHtmlLocalized(10, 100, 500, 100, m_HasInventory ? 1150691 : 1150692, RedColor16, false, false);
			
			if(!m_HasInventory)
			{
				AddHtmlLocalized(215, 350, 150, 18, 1150695, OrangeColor16, false, false); // CONFIRM
				AddButton(175, 350, 4005, 4007, 1, GumpButtonType.Reply, 0);
			}
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(m_Plot == null || !m_Plot.IsOwner(from))
				return;
				
			if(info.ButtonID == 1 && !m_HasInventory)
				m_Plot.FireBroker();
		}
	}
	
	public class HireBrokerGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		private bool m_Commodity;
		
		public HireBrokerGump(Mobile from, MaginciaBazaarPlot plot, bool commodity)
		{
			m_Plot = plot;
			m_Commodity = commodity;

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddHtmlLocalized(191, 100, 150, 18, m_Commodity ? 1150687 : 1150686, RedColor16, false, false); // HIRE ANIMAL BROKER
			AddHtmlLocalized(10, 130, 500, 100, 1150690, RedColor16, false, false); // Choose a stall style...
			
			AddButton(175, 244, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(210, 244, 150, 18, 1150701, RedColor16, false, false); // STYLE 1

            AddButton(175, 268, 4005, 4007, 2, GumpButtonType.Reply, 0);
			AddHtmlLocalized(210, 268, 150, 18, 1150702, RedColor16, false, false);	// STYLE 2

            AddButton(175, 292, 4005, 4007, 3, GumpButtonType.Reply, 0);
			AddHtmlLocalized(210, 292, 150, 18, 1150703, RedColor16, false, false); // STYLE 3

            AddButton(10, 490, 4014, 4016, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(m_Plot == null || !m_Plot.IsOwner(from))
				return;
				
			int idx1 = m_Commodity ? 0 : 1;
			int idx2 = info.ButtonID - 1;
			
			switch(info.ButtonID)
			{
				default:
				case 0: break;
				case 1: // style1
				case 2: // style2
				case 3: // style3
					m_Plot.AddTempMulti(idx1, idx2);
					from.SendGump(new ConfirmAddMultiGump(from, m_Plot, m_Commodity));
					break;
				case 4: // MAIN MENU
					from.SendGump(new MyStallLeaseGump(from, m_Plot));
					break;
			}
		}
	}
	
	public class ConfirmAddMultiGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		private bool m_Commodity;
		
		public ConfirmAddMultiGump(Mobile from, MaginciaBazaarPlot plot, bool commodity)
		{
			m_Plot = plot;
			m_Commodity = commodity;

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddHtmlLocalized(200, 100, 150, 18, m_Commodity ? 1150687 : 1150686, RedColor16, false, false); // HIRE ANIMAL BROKER
			
			AddHtmlLocalized(10, 150, 500, 200, 1150696, RedColor16, false, false); // The selected style is now being...
			
			AddHtmlLocalized(210, 370, 150, 18, 1150697, OrangeColor16, false, false); // CHOOSE OTHER STYLE
			AddButton(175, 370, 4005, 4007, 1, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(210, 394, 150, 18, 1150695, OrangeColor16, false, false); // CONFIRM
            AddButton(175, 394, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddButton(10, 490, 4014, 4016, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			if(m_Plot == null || !m_Plot.IsOwner(from))
				return;
				
			switch(info.ButtonID)
			{
				default:
				case 0: break;
				case 1:
					from.SendGump(new HireBrokerGump(from, m_Plot, m_Commodity));
					break;
				case 2:
					if(m_Plot.HasTempMulti())
						m_Plot.ConfirmMulti(m_Commodity);
					else
					{
						from.SendGump(new HireBrokerGump(from, m_Plot, m_Commodity));
						from.SendMessage("The current preview has timed out. Preview the stall style again, and select confirm.");
					}
					break;
				case 3:
					from.SendGump(new MyStallLeaseGump(from, m_Plot));
					break;
			}
		}
	}
	
	public class StallBidGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public StallBidGump(Mobile from, MaginciaBazaarPlot plot)
		{
			m_Plot = plot;
			
			MaginciaBazaarPlot biddingPlot = MaginciaBazaar.GetBiddingPlot(from);
            int bidAmount = biddingPlot != null ? biddingPlot.GetBid(from) : MaginciaBazaar.GetNextAvailableBid(from);

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddHtmlLocalized(86, 135, 200, 18, 1150389, RedColor16, false, false); // You are bidding on:
			
			if(biddingPlot != null)
				AddHtmlLocalized(215, 135, 100, 18, 1150541, biddingPlot.PlotDef.ID, BlueColor16, false, false); // ~1_TOKEN~
			else if(MaginciaBazaar.IsBiddingNextAvailable(from))
                AddHtmlLocalized(215, 135, 100, 18, 1150538, BlueColor16, false, false); // Next Available Stall
            else
				AddHtmlLocalized(215, 135, 100, 18, 1150396, BlueColor16, false, false); // NONE

            AddHtmlLocalized(135, 160, 150, 18, 1150407, RedColor16, false, false); // Bid Amount:
            AddHtml(215, 160, 100, 18, Color(FormatAmt(bidAmount), BlueColor), false, false);

            AddButton(260, 220, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(300, 220, 150, 18, 1150566, OrangeColor16, false, false); // INSTRUCTIONS

			AddHtmlLocalized(125, 265, 200, 18, 1150560, RedColor16, false, false); // BID AMOUNT
			AddBackground(215, 265, 295, 22, 9350);
			AddTextEntry(216, 267, 295, 20, 0, 0, "");
			
			bool isOwner = m_Plot.IsOwner(from);
			
			AddHtmlLocalized(215, 304, 250, 18, 1150568, m_Plot.PlotDef.ID, isOwner ? GrayColor16 : BlueColor16, false, false); // BID ON THIS STALL (Stall ~1_STALLID~)
			if(!isOwner)
				AddButton(175, 304, 4005, 4007, 2, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(215, 328, 250, 18, 1150569, BlueColor16, false, false); // BID ON FIRST AVAILABLE
			AddButton(175, 328, 4005, 4007, 3, GumpButtonType.Reply, 0);
			
			AddHtmlLocalized(10, 360, 500, 40, 1150792, RedColor16, false, false); // Minimum bid 1,000gp. Bids are rounded down to nearest 1,000gp (1,999gp becomes 1,000gp).

            AddButton(10, 490, 4014, 4016, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			
			switch(info.ButtonID)
			{
				default:
				case 0: break;
				case 1:
					from.SendGump(new StallBidGump(from, m_Plot));
					from.SendGump(new BazaarInformationGump(0, 1150567));
					break;
				case 2: // BID ON THIS STALL
					{
						int amount = 0;
						TextRelay relay = info.TextEntries[0];
                        bool hasBiddingPlot = MaginciaBazaar.GetBiddingPlot(from) != null || MaginciaBazaar.NextAvailable.ContainsKey(from);
						
						if(!m_Plot.IsOwner(from))
						{
							try
							{
								amount = Convert.ToInt32(relay.Text);
								
								if(amount > 0)
								{
									double r = (double)amount / 1000;
									amount = (int)(Math.Floor(r) * 1000.0);
									
									if(amount < 1000)
										amount = 1000;
								}
                                else if (!hasBiddingPlot)
                                {
                                    from.SendGump(new StallBidGump(from, m_Plot));
                                    return;
                                }

                                from.SendGump(new ConfirmBidGump(from, m_Plot, m_Plot, amount, amount <= 0));
                                return;
							}
                            catch { Console.WriteLine("Error"); }

                            from.SendGump(new StallBidGump(from, m_Plot));
						}
					}
                    break;
				case 3: // BID ON FIRST AVAILABLE
					{
						int amount1 = 0;
						TextRelay relay1 = info.TextEntries[0];
                        bool hasBiddingPlot = MaginciaBazaar.GetBiddingPlot(from) != null || MaginciaBazaar.NextAvailable.ContainsKey(from);

						try
						{
							amount1 = Convert.ToInt32(relay1.Text);
							
							if(amount1 > 0)
							{
								double r1 = (double)amount1 / 1000;
								amount1 = (int)(Math.Floor(r1) * 1000.0);
								
								if(amount1 < 1000)
									amount1 = 1000;
							}
                            else if(!hasBiddingPlot)
                            {
                                    from.SendGump(new StallBidGump(from, m_Plot));
                                    return;
                            }

                            from.SendGump(new ConfirmBidGump(from, m_Plot, null, amount1, amount1 <= 0));
                            return;
						}
                        catch
                        { 
                        }

                        from.SendGump(new StallBidGump(from, m_Plot));
					}
                    break;
				case 4: // MAIN MENU
					from.SendGump(new StallLeasingGump(from, m_Plot));
					break;
			}
		}
	}
	
	public class TopBidsGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public TopBidsGump(Mobile from, MaginciaBazaarPlot plot)
		{
			m_Plot = plot;

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddHtmlLocalized(173, 100, 173, 18, 1150562, plot.PlotDef.ID, RedColor16, false, false); // <DIV ALIGN=CENTER>Viewing top bids for stall <b>~1_STALLNAME~</b>
			
			if(m_Plot.Auction == null || m_Plot.Auction.Auctioners.Count == 0)
				AddHtmlLocalized(60, 150, 300, 18, 1150563, BlueColor16, false, false); // There are currently no bids to lease this stall.
			else
			{
				List<BidEntry> list = new List<BidEntry>(m_Plot.Auction.Auctioners.Values);
				list.Sort();
				int y = 150;
				
				for(int i = 0; i < list.Count; i++)
				{
					AddHtml(60, y + (i * 20), 200, 18, Color(FormatAmt(list[i].Amount), OrangeColor), false, false);
					
					if(i >= 10)
						break;
				}
			}

            AddButton(10, 490, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			if(m_Plot == null)
				return;
				
			if(info.ButtonID == 1)
				state.Mobile.SendGump(new StallLeasingGump(state.Mobile, m_Plot));
		}
	}
	
	public class ConfirmBidGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public ConfirmBidGump(Mobile from, MaginciaBazaarPlot actualPlot, MaginciaBazaarPlot newPlot, int newBid, bool retract)
		{
			m_Plot = actualPlot;
			
			AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
			AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			MaginciaBazaarPlot oldPlot = MaginciaBazaar.GetBiddingPlot(from);
			
			bool hasbidnextavailable = MaginciaBazaar.NextAvailable.ContainsKey(from);
			bool hasbidspecific = oldPlot != null;
			bool specific = newPlot != null;

            int cliloc1 = 0; 
            int cliloc2 = 0;
            string args1 = null;
            string args2 = null;
			
			int bankBal = Banker.GetBalance(from);
            int newBankBal = bankBal;
			int oldBid = 0;
            //int dif = 0;
			
			if(hasbidspecific)
				oldBid = oldPlot.Auction.GetBidAmount(from);
			else if (hasbidnextavailable)
				oldBid = MaginciaBazaar.NextAvailable[from].Amount;
				
			if(newBid < 1000)
				newBid = 1000;
				
			bool increase = newBid > oldBid;
			
			if(retract) // Retract all bids
			{
				if(MaginciaBazaar.TryRetractBid(from))
				{
					/*You have canceled your stall bid of ~1_OLDBID~gp. The funds have been deposited 
					 *into your bank box. Your previous bank balance was ~4_OLDBAL~gp and your current
					  *bank balance is now ~5_NEWBAL~gp. */
					  
					cliloc1 = 1150576;
					args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", FormatAmt(oldBid), "", "", FormatAmt(bankBal), FormatAmt(bankBal + oldBid));
				}
				else
				{
					/*You are attempting to cancel your stall bid of ~1_OLDBID~gp. The bid cannot be 
					 *refunded to your bank account, because your bank box cannot hold the additional 
					 *funds. Your current bank balance is ~4_OLDVAL~gp.<br><br>Your bid status has not changed.*/
					cliloc1 = 1150575;
					args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(oldBid), "", "", FormatAmt(bankBal));
				}
			}
			else if (oldBid == newBid)
			{
				// Your bid amount of ~1_BID~gp has not changed.
				cliloc1 = 1150570;
				args1 = FormatAmt(newBid);
				
				if(hasbidspecific)
				{
                    if (specific)
                    {
                        if (oldPlot != newPlot)
                        {
                            oldPlot.Auction.RemoveBid(from);
                            newPlot.Auction.MakeBid(from, newBid);

                            //You were previously bidding on Stall ~1_OLDSTALL~ and now you are bidding on Stall ~2_NEWSTALL~.
                            cliloc1 = 1150582;
                            args1 = String.Format("{0}\t{1}", oldPlot.PlotDef.ID, newPlot.PlotDef.ID);

                            // You are now bidding on Stall ~1_NEWSTALL~
                            cliloc2 = 1150585;
                            args2 = newPlot.PlotDef.ID;
                        }
                        else
                        {
                            // Your bid amount of ~1_BID~gp has not changed.
                            cliloc1 = 1150570;
                            args1 = FormatAmt(newBid);

                            // You are still bidding on Stall ~2_NEWSTALL~.
                            cliloc2 = 1150583;
                            args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
                        }
                    }
                    else
                    {
                        oldPlot.Auction.RemoveBid(from);
                        MaginciaBazaar.MakeBidNextAvailable(from, newBid);

                        // You were previously bidding on Stall ~1_OLDSTALL~, and now you are bidding on the first available stall.
                        cliloc1 = 1150580;
                        args1 = oldPlot.PlotDef.ID;

                        // You are now bidding on the next available stall.
                        cliloc2 = 1150584;
                        args2 = null;
                    }
				}
				else if(hasbidnextavailable)
				{
                    if (specific)
                    {
                        newPlot.Auction.MakeBid(from, newBid);
                        MaginciaBazaar.RemoveBidNextAvailable(from);

                        //You were previously bidding on the first available stall, and now you are bidding specifically for Stall ~2_NEWSTALL~.
                        cliloc1 = 1150579;
                        args1 = String.Format("{0}\t{1}", "", newPlot.PlotDef.ID);

                        // You are now bidding on the next available stall.
                        cliloc2 = 1150584;
                        args2 = null;
                    }
                    else
                    {
                        // Your bid amount of ~1_BID~gp has not changed.
                        cliloc1 = 1150570;
                        args1 = FormatAmt(newBid);

                        // You are still bidding on the next available stall.
                        cliloc2 = 1150581;
                        args2 = null;
                    }
				}
			}
			else if(specific)
			{
				if(increase)
				{
					int dif = newBid - oldBid;
					
					if(bankBal < dif)
					{
						/*You are attempting to increase your stall bid from ~1_OLDBID~gp to 
						 *~2_NEWBID~gp. The difference of ~3_BIDCHANGE~gp cannot be withdrawn from 
						 *your bank account. Your current bank balance is ~4_CURBAL~gp.<br><br>The 
						 *status of your bid has not changed.*/
						cliloc1 = 1150571;
						args2 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(oldBid), FormatAmt(newBid), FormatAmt(dif), FormatAmt(bankBal));
						
						if(hasbidspecific)
						{
							// You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidnextavailable)
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else
						{
							cliloc2 = 1150541;
							args2 = "You have no active bids.";
						}
					}
					else if((hasbidnextavailable || hasbidspecific) && !MaginciaBazaar.TryRetractBid(from))
					{
						/*You are attempting to cancel your stall bid of ~1_OLDBID~gp. The bid cannot 
						 *be refunded to your bank account, because your bank box cannot hold the additional 
						 *funds. Your current bank balance is ~4_OLDVAL~gp.<br><br>Your bid status has not 
						 changed.*/
						cliloc1 = 1150575;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(oldBid), "", "", FormatAmt(bankBal));
						
						if(hasbidspecific)
						{
							// You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidnextavailable)
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else
						{
							cliloc2 = 1150541;
							args2 = "You have no active bids.";
						}
					}
					else
					{
						Banker.Withdraw(from, dif);
						newPlot.Auction.MakeBid(from, newBid);
						/*You have posted a new bid of ~2_NEWBID~gp. The funds have been 
						*withdrawn from your bank box. Your previous bank balance was 
						~4_OLDBAL~gp and your new bank balance is ~5_NEWBAL~gp.*/
						cliloc1 = 1150578;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", "", FormatAmt(newBid), "", FormatAmt(bankBal), FormatAmt(bankBal - dif));
						
						if(hasbidspecific && oldPlot == newPlot) 		// same plot
						{
							/*You are increasing your stall bid from ~1_OLDBID~gp to ~2_NEWBID~gp. 
							 *The difference of ~3_BIDCHANGE~gp has been withdrawn from your bank 
							  *account. Your previous bank balance was ~4_CURBAL~gp, and your new 
							  bank balance is ~5_NEWBAL~gp.*/
							cliloc1 = 1150572;
							//You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", FormatAmt(oldBid), FormatAmt(newBid), FormatAmt(dif), FormatAmt(bankBal), FormatAmt(bankBal - dif));
							args2 = String.Format("{0}\t{1}", "", newPlot.PlotDef.ID);
						}
						else if(hasbidspecific &&  oldPlot != newPlot)	// switching plots
						{
							//You were previously bidding on Stall ~1_OLDSTALL~ and now you are bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150582;
							args2 = String.Format("{0}\t{1}", oldPlot.PlotDef.ID, newPlot.PlotDef.ID);
						}
						else if (hasbidnextavailable)					// had next available
						{
							//You were previously bidding on the first available stall, and now you are bidding specifically for Stall ~2_NEWSTALL~.
							cliloc2 = 1150579;
							args2 = String.Format("{0}\t{1}", "", newPlot.PlotDef.ID);
						}
						else											// no bids before
						{
							// You are now bidding on Stall ~1_NEWSTALL~
							cliloc2 = 1150585;
							args2 = newPlot.PlotDef.ID;
						}
					}
				}
				else
				{
					int dif = oldBid - newBid;
					
					if((hasbidnextavailable || hasbidspecific) && !MaginciaBazaar.TryRetractBid(from))
					{
						/*You are attempting to cancel your stall bid of ~1_OLDBID~gp. The bid cannot 
						 *be refunded to your bank account, because your bank box cannot hold the additional 
						 *funds. Your current bank balance is ~4_OLDVAL~gp.<br><br>Your bid status has not 
						 changed.*/
						cliloc1 = 1150575;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(oldBid), "", "", FormatAmt(bankBal));
						
						if(hasbidspecific)
						{
							// You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidnextavailable)
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else
						{
							cliloc2 = 1150541;
							args2 = "You have no active bids.";
						}
					}
					else
					{
						/*You have posted a new bid of ~2_NEWBID~gp. The funds have been 
						*withdrawn from your bank box. Your previous bank balance was 
						~4_OLDBAL~gp and your new bank balance is ~5_NEWBAL~gp.*/
						cliloc1 = 1150578;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", "", FormatAmt(newBid), "", FormatAmt(bankBal), FormatAmt(bankBal - dif));

                        Banker.Withdraw(from, newBid);
						newPlot.Auction.MakeBid(from, newBid);
						
						if(hasbidspecific &&  oldPlot == newPlot) 		// winner, same plot
						{
							/*You are decreasing your stall bid from ~1_OLDBID~gp to ~2_NEWBID~gp. 
							 *The difference of ~3_BIDCHANGE~gp has been deposited into your bank 
							 *account. Your previous bank balance was ~4_CURBAL~gp, and your new 
							 *bank balance is ~5_NEWBAL~gp.*/
							cliloc1 = 1150574;
							//You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", FormatAmt(oldBid), FormatAmt(newBid), FormatAmt(dif), FormatAmt(bankBal), FormatAmt(bankBal + dif));
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidspecific &&  oldPlot != newPlot) 	// switch plots
						{
							//You were previously bidding on Stall ~1_OLDSTALL~ and now you are bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150582;
							args2 = String.Format("{0}\t{1}", oldPlot.PlotDef.ID, newPlot.PlotDef.ID);
						}
						else if (hasbidnextavailable)					// new plot
						{
							//You were previously bidding on the first available stall, and now you are bidding specifically for Stall ~2_NEWSTALL~.
							cliloc2 = 1150579;
							args2 = String.Format("{0}\t{1}", "", newPlot.PlotDef.ID);
						}
						else											
						{
							// You are now bidding on Stall ~1_NEWSTALL~
							cliloc2 = 1150585;
							args2 = String.Format(newPlot.PlotDef.ID);
						}
					}
				}
			}
			else
			{
				if(increase)
				{
					int dif = newBid - oldBid; 
					
					if(bankBal < dif)
					{
						 /*You are attempting to place a new bid of ~2_NEWBID~gp. This amount exceeds 
						  *your current bank balance of ~4_OLDBAL~gp.<br><br>Your bid status has not 
						  changed.*/
						cliloc1 = 1150577;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}", "", FormatAmt(newBid), "", FormatAmt(bankBal));
						
						if(hasbidspecific)
						{
							// You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidnextavailable)
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else
						{
							cliloc2 = 1150541;
							args2 = "You have no active bids.";
						}
					}
					else if((hasbidnextavailable || hasbidspecific) && !MaginciaBazaar.TryRetractBid(from))
					{
						/*You are attempting to cancel your stall bid of ~1_OLDBID~gp. The bid cannot 
						 *be refunded to your bank account, because your bank box cannot hold the additional 
						 *funds. Your current bank balance is ~4_OLDVAL~gp.<br><br>Your bid status has not 
						 changed.*/
						cliloc1 = 1150575;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(oldBid), "", "", FormatAmt(bankBal));
						
						if(hasbidspecific)
						{
							// You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidnextavailable)
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else
						{
							cliloc2 = 1150541;
							args2 = "You have no active bids.";
						}
					}
					else
					{
						Banker.Withdraw(from, dif);
                        MaginciaBazaar.MakeBidNextAvailable(from, newBid);
						
						/*You have posted a new bid of ~2_NEWBID~gp. The funds have been 
						*withdrawn from your bank box. Your previous bank balance was 
						*~4_OLDBAL~gp and your new bank balance is ~5_NEWBAL~gp.*/
						cliloc1 = 1150578;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", "", FormatAmt(newBid), "", FormatAmt(bankBal), FormatAmt(bankBal - newBid));
						
						if(hasbidspecific)				// Change from specific to next available
						{
							// You were previously bidding on Stall ~1_OLDSTALL~, and now you are bidding on the first available stall.
							cliloc2 = 1150580;
							args2 = oldPlot.PlotDef.ID;
						}
						else if(hasbidnextavailable)	// stays as next available
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else							// First time bid
						{
							// You are now bidding on the next available stall.
							cliloc2 = 1150584;
							args2 = null;
						}
					}
				}
				else
				{
					int dif = oldBid - newBid;
					
					if((hasbidnextavailable || hasbidspecific) && !MaginciaBazaar.TryRetractBid(from))
					{
						/*You are attempting to cancel your stall bid of ~1_OLDBID~gp. The bid cannot 
						 *be refunded to your bank account, because your bank box cannot hold the additional 
						 *funds. Your current bank balance is ~4_OLDVAL~gp.<br><br>Your bid status has not 
						 changed.*/
						cliloc1 = 1150575;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(oldBid), "", "", FormatAmt(bankBal));
						
						if(hasbidspecific)
						{
							// You are still bidding on Stall ~2_NEWSTALL~.
							cliloc2 = 1150583;
							args2 = String.Format("{0}\t{1}", "", oldPlot.PlotDef.ID);
						}
						else if(hasbidnextavailable)
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else
						{
							cliloc2 = 1150541;
							args2 = "You have no active bids.";
						}
					}
					else
					{
						/*You have posted a new bid of ~2_NEWBID~gp. The funds have been 
						*withdrawn from your bank box. Your previous bank balance was 
						*~4_OLDBAL~gp and your new bank balance is ~5_NEWBAL~gp.*/
						cliloc1 = 1150578;
						args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", "", FormatAmt(newBid), "", FormatAmt(bankBal), FormatAmt(bankBal - newBid));

                        Banker.Withdraw(from, newBid);
						MaginciaBazaar.MakeBidNextAvailable(from, newBid);
						
						if(hasbidspecific)				// Change from specific to next available
						{
							// You were previously bidding on Stall ~1_OLDSTALL~, and now you are bidding on the first available stall.
							cliloc2 = 1150580;
							args2 = oldPlot.PlotDef.ID;
						}
						else if(hasbidnextavailable)	// stays as next available
						{
							// You are still bidding on the next available stall.
							cliloc2 = 1150581;
							args2 = null;
						}
						else							// First time bid
						{
							// You are now bidding on the next available stall.
							cliloc2 = 1150584;
							args2 = null;
						}
					}
				}
			}

            //string args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", );

            if(cliloc1 > 0)
			    AddHtmlLocalized(10, 100, 500, 120, cliloc1, args1, GreenColor16, false, false);

            if (cliloc2 > 0)
            {
                if (args2 != null)
                    AddHtmlLocalized(10, 240, 500, 100, cliloc2, args2, GreenColor16, false, false);
                else
                    AddHtmlLocalized(10, 240, 500, 100, cliloc2, GreenColor16, false, false);
            }
			
			AddHtmlLocalized(50, 490, 150, 18, 1150556, BlueColor16, false, false); // MY STALL BID
			AddButton(10, 490, 4005, 4007, 1, GumpButtonType.Reply, 0);
			
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			if(info.ButtonID == 1)
                state.Mobile.SendGump(new StallBidGump(state.Mobile, m_Plot));
		}
	}
	
	public class MatchBidGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
		
		public MatchBidGump(Mobile from, MaginciaBazaarPlot plot)
		{
			m_Plot = plot;
			
			int amount = MaginciaBazaar.GetBidMatching(from);

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			AddHtmlLocalized(12, 100, 150, 18, 1150387, RedColor16, false, false); // Your Stall:
			AddHtmlLocalized(12, 120, 150, 18, 1150393, RedColor16, false, false); // Bid Match Limit:

            if (m_Plot != null)
                AddHtml(260, 100, 150, 18, Color(m_Plot.PlotDef.ID, BlueColor), false, false);
            else
                AddHtml(260, 100, 250, 18, Color("You are not leasing stall.", BlueColor), false, false);

			AddHtml(260, 120, 150, 18, Color(FormatAmt(amount), BlueColor), false, false);
			
			AddButton(175, 175, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(215, 175, 150, 18, 1150392, OrangeColor16, false, false); // INFORMATION
			
			AddHtmlLocalized(10, 230, 200, 18, 1150587, RedColor16, false, false); // CHANGE MATCH BID
			AddBackground(215, 280, 295, 22, 9350);
			AddTextEntry(216, 281, 295, 20, 0, 0, "");
			AddButton(175, 280, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddButton(10, 490, 4014, 4016, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 200, 18, 1149777, BlueColor16, false, false); // MAIN MENU
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
            Mobile from = state.Mobile;

			if(m_Plot != null && !m_Plot.IsOwner(from))
				return;

			switch(info.ButtonID)
			{
				case 0: break;
				case 1: 
					from.SendGump(new MatchBidGump(from, m_Plot));
					from.SendGump(new BazaarInformationGump(1150399, 1150398));
					break;
				case 2:
					{
						TextRelay relay = info.TextEntries[0];
						int amount = 0;
						
						try
						{
							amount = Convert.ToInt32(relay.Text);
							from.SendGump(new ConfirmMatchBidGump(from, amount, m_Plot));
						}
						catch
						{
						}
					}
                    break;
				case 3: // MAIN MENU
					if(m_Plot != null)
						from.SendGump(new StallLeasingGump(from, m_Plot));
					break;
			}
		}
	}
	
	public class ConfirmMatchBidGump : BaseBazaarGump
	{
		private MaginciaBazaarPlot m_Plot;
				
		public ConfirmMatchBidGump(Mobile from, int amount, MaginciaBazaarPlot plot)
		{
			m_Plot = plot;
			
			if(amount < 0)
				amount = 0;

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1150386, RedColor16, false, false);    // Stall Leasing
			
			int cliloc1;
			string args1;
			
			int current = MaginciaBazaar.GetBidMatching(from);
			int bankBal = Banker.GetBalance(from);
			
			if(amount > current)
			{
				int dif = amount - current;
				
				if(dif > bankBal)
				{
					/*Your attempt to increase your Match Bid from ~1_OLDBID~gp to ~2_NEWBID~gp 
					 *failed. The difference of ~3_CHANGE~gp could not be withdrawn from your bank 
					 *account balance. Your current bank balance is ~5_NEWBAL~gp.*/
					cliloc1 = 1150588;
					args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(current), FormatAmt(amount), FormatAmt(dif), FormatAmt(bankBal));
				}
				else
				{
					Banker.Withdraw(from, amount);
					MaginciaBazaar.AddToReserve(from, dif);
					/*You have increased your Match Bid from ~1_OLDBID~gp to ~2_NEWBID~gp. The 
					 *difference of ~3_CHANGE~gp has been withdrawn from your bank account balance. 
					 *Your previous bank balance was ~4_OLDBAL~gp and your current bank balance is 
					 ~5_NEWBAL~gp.*/
					cliloc1 = 1150589;
					args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", FormatAmt(current), FormatAmt(amount), FormatAmt(dif), FormatAmt(bankBal), FormatAmt(bankBal - dif));
				}
			}
			else if (amount < current)
			{
				int dif = current - amount;

				if(!Banker.Deposit(from, dif))	
				{
					/*Your attempt to decrease your Match Bid from ~1_OLDBID~gp to ~2_NEWBID~gp 
					 *failed. The difference of ~3_CHANGE~gp could not be deposited into your bank 
					 box. Your current bank balance is ~5_NEWBAL~gp.*/
					cliloc1 = 1150590;
					args1 = String.Format("{0}\t{1}\t{2}\t{3}", FormatAmt(current), FormatAmt(amount), FormatAmt(dif), FormatAmt(bankBal));
				}
				else
				{
					MaginciaBazaar.DeductReserve(from, dif);
					/*You have decreased your Match Bid from ~1_OLDBID~gp to ~2_NEWBID~gp. The 
					 *difference of ~3_CHANGE~gp has been deposited into your bank box. Your 
					 *previous bank balance was ~4_OLDBAL~gp and your current bank balance is 
					 ~5_NEWBAL~gp.*/
					cliloc1 = 1150591;
					args1 = String.Format("{0}\t{1}\t{2}\t{3}\t{4}", FormatAmt(current), FormatAmt(amount), FormatAmt(dif), FormatAmt(bankBal), FormatAmt(bankBal + dif));
				}
			}
			else
			{
				//You have not changed your match bid.
				cliloc1 = 1150592;
				args1 = null;
			}

			if(args1 != null)
				AddHtmlLocalized(10, 100, 500, 120, cliloc1, args1, GreenColor16, false, false);
			else
				AddHtmlLocalized(10, 100, 500, 120, cliloc1, GreenColor16, false, false);
			
			AddHtmlLocalized(50, 490, 150, 18, 1150540, BlueColor16, false, false); // MY BID MATCHING
			AddButton(10, 490, 4014, 4016, 1, GumpButtonType.Reply, 0);
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
            Mobile from = state.Mobile;

			if(m_Plot != null && !m_Plot.IsOwner(from))
				return;
				
			if(info.ButtonID == 1)
				state.Mobile.SendGump(new MatchBidGump(from, m_Plot));
		}
	}
	
	public class BazaarInformationGump : BaseBazaarGump
	{
        public BazaarInformationGump(int title, int message) : this(title, message, -1)
        {
        }

		public BazaarInformationGump(int title, int message, int hue)
		{
            int useHue = hue == -1 ? RedColor16 : hue;
            AddHtmlLocalized(195, 10, 150, 18, 1150385, useHue, false, false); // New Magincia Bazaar
			
			if(title > 0)
                AddHtmlLocalized(10, 50, 500, 40, title, useHue, false, false);

            AddHtmlLocalized(10, 100, 500, 350, message, useHue, false, true); 
		}
	}
	
	public class ShopRecallRuneGump : BaseBazaarGump
	{
		private PlotSign m_Sign;
		
		public ShopRecallRuneGump(Mobile from, PlotSign sign)
		{
			m_Sign = sign;

            AddHtmlLocalized(195, 5, 150, 18, 1150385, RedColor16, false, false);     // New Magincia Bazaar
            AddHtmlLocalized(217, 40, 150, 18, 1151508, RedColor16, false, false);    // Shop Recall Rune
			
			// For a charge of 100gp, you will receive a recall rune marked for your bazaar stall.
			AddHtmlLocalized(10, 120, 500, 18, 1150458, RedColor16, false, false);
			
			AddButton(175, 150, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtml(215, 150, 150, 18, Color("PURCHASE", OrangeColor), false, false);
		}
		
		public override void OnResponse(NetState state, RelayInfo info)
		{
			if(info.ButtonID == 1)
			{
				Mobile from = state.Mobile;
				
				if(from.InRange(m_Sign.Location, 2))
				{
					if(!Banker.Withdraw(from, PlotSign.RuneCost))
					{
						from.SendMessage("You must have {0} gold in your bankbox to purchase a recall rune to this plot.", PlotSign.RuneCost);
						return;
					}
					
					RecallRune rune = new RecallRune();
					rune.Target = from.Location;
					rune.TargetMap = from.Map;
					rune.Description = String.Format("Lot: {0}", m_Sign.Plot.PlotDef.ID);
					rune.Marked = true;
					
					if(from.Backpack == null || !from.Backpack.TryDropItem(from, rune, false))
					{
						from.BankBox.DropItem(rune);
                        from.SendMessage("An item has been placed in your bankbox.");
					}
					else
                        from.SendLocalizedMessage(1153657); // An item has been placed in your backpack.
				}
				else
					from.SendMessage("You must be withing 2 tiles of the stall sign to mark a rune.");
			}
		}
	}
}