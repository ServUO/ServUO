using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Engines.NewMagincia
{	
	public class MaginciaPlotAuction
	{
		private Dictionary<Mobile, BidEntry> m_Auctioners = new Dictionary<Mobile, BidEntry>();
		public Dictionary<Mobile, BidEntry> Auctioners { get { return m_Auctioners; } }
		
		private MaginciaBazaarPlot m_Plot;
		private DateTime m_AuctionEnd;
		
        public MaginciaBazaarPlot Plot { get { return m_Plot; } }
		public DateTime AuctionEnd { get { return m_AuctionEnd; } }
		
		public MaginciaPlotAuction(MaginciaBazaarPlot plot) : this(plot, MaginciaBazaar.GetShortAuctionTime)
		{
		}

        public MaginciaPlotAuction(MaginciaBazaarPlot plot, TimeSpan auctionDuration)
		{
			m_Plot = plot;
			m_AuctionEnd = DateTime.UtcNow + auctionDuration;
		}
		
		public void MakeBid(Mobile bidder, int amount)
		{
			m_Auctioners[bidder] = new BidEntry(bidder, amount, BidType.Specific);
		}
		
		public bool RetractBid(Mobile from)
		{
			Account acct = from.Account as Account;

            for (int i = 0; i < acct.Length; i++)
            {
                Mobile m = acct[i];

                if (m == null)
                    continue;

                if (m_Auctioners.ContainsKey(m))
                {
                    BidEntry entry = m_Auctioners[m];

                    if (entry != null && Banker.Deposit(m, entry.Amount))
                    {
                        m_Auctioners.Remove(m);
                        return true;
                    }
                }
            }
			
			return false;
		}

        public void RemoveBid(Mobile from)
        {
            if (m_Auctioners.ContainsKey(from))
                m_Auctioners.Remove(from);
        }
		
		public int GetHighestBid()
		{
			int highest = -1;
			foreach(BidEntry entry in m_Auctioners.Values)
			{
				if(entry.Amount >= highest)
					highest = entry.Amount;
			}
			return highest;
		}
		
		public void OnTick()
		{
			if(m_AuctionEnd < DateTime.UtcNow)
				EndAuction();
		}
		
		public void EndAuction()
		{
            if (m_Plot == null)
                return;

            if (m_Plot.HasTempMulti())
                m_Plot.RemoveTempPlot();

			Mobile winner = null;
			int highest = 0;
			
			Dictionary<Mobile, BidEntry> combined = new Dictionary<Mobile, BidEntry>(m_Auctioners);
			
			//Combine auction bids with the bids for next available plot
			foreach(KeyValuePair<Mobile, BidEntry> kvp in MaginciaBazaar.NextAvailable)
				combined.Add(kvp.Key, kvp.Value);
			
			//Get highest bid
			foreach(BidEntry entry in combined.Values)
			{
				if(entry.Amount > highest)
					highest = entry.Amount;
			}
			
			// Check for owner, and if the owner has a match bad AND hasn't bidded on another plot!
			if(m_Plot.Owner != null && MaginciaBazaar.Reserve.ContainsKey(m_Plot.Owner) && MaginciaBazaar.Instance != null && !MaginciaBazaar.Instance.HasActiveBid(m_Plot.Owner))
			{
				int matching = MaginciaBazaar.GetBidMatching(m_Plot.Owner);
				
				if(matching >= highest)
				{
					MaginciaBazaar.DeductReserve(m_Plot.Owner, highest);
					int newreserve = MaginciaBazaar.GetBidMatching(m_Plot.Owner);
					winner = m_Plot.Owner;
					
					/*You extended your lease on Stall ~1_STALLNAME~ at the ~2_FACET~ New Magincia 
					 *Bazaar. You matched the top bid of ~3_BIDAMT~gp. That amount has been deducted 
					 *from your Match Bid of ~4_MATCHAMT~gp. Your Match Bid balance is now 
					 *~5_NEWMATCH~gp. You may reclaim any additional match bid funds or adjust 
					 *your match bid for next week at the bazaar.*/
					MaginciaLottoSystem.SendMessageTo(m_Plot.Owner, new NewMaginciaMessage(null, new TextDefinition(1150427), String.Format("{0}\t{1}\t{2}\t{3}\t{4}", m_Plot.PlotDef.ID, m_Plot.PlotDef.Map.ToString(), highest.ToString("###,###,###"), matching.ToString("###,###,###"), newreserve.ToString("###,###,###"))));
				}
				else
				{
					/*You lost the bid to extend your lease on Stall ~1_STALLNAME~ at the ~2_FACET~ 
					 *New Magincia Bazaar. Your match bid amount of ~3_BIDAMT~gp is held in escrow 
					 *at the Bazaar. You may obtain a full refund there at any time. Your hired 
					 *merchant, if any, has deposited your proceeds and remaining inventory at the 
					 *Warehouse in New Magincia. You must retrieve these within one week or they 
					 *will be destroyed.*/
					MaginciaLottoSystem.SendMessageTo(m_Plot.Owner, new NewMaginciaMessage(null, new TextDefinition(1150528), String.Format("{0}\t{1}", m_Plot.PlotDef.ID, m_Plot.PlotDef.Map.ToString(), matching.ToString("###,###,###"))));
				}
			}
			else if (m_Plot.Owner != null)
			{
				/*Your lease has expired on Stall ~1_STALLNAME~ at the ~2_FACET~ New Magincia Bazaar.*/
				MaginciaLottoSystem.SendMessageTo(m_Plot.Owner, new NewMaginciaMessage(null, new TextDefinition(1150430), String.Format("{0}\t{1}", m_Plot.PlotDef.ID, m_Plot.PlotDef.Map.ToString())));
			}
			
			if(winner == null)
			{
				//Get list of winners
				List<BidEntry> winners = new List<BidEntry>();
				foreach(KeyValuePair<Mobile, BidEntry> kvp in combined)
				{
					if(kvp.Value.Amount >= highest)
						winners.Add(kvp.Value);
				}
				
				// One winner!
				if(winners.Count == 1)
					winner = winners[0].Bidder;
				else
				{
					// get a list of specific type (as opposed to next available)
					List<BidEntry> specifics = new List<BidEntry>();
					foreach(BidEntry bid in winners)
					{
						if(bid.BidType == BidType.Specific)
							specifics.Add(bid);
					}
					
					// one 1 specific!
					if(specifics.Count == 1)
						winner = specifics[0].Bidder;
					else if (specifics.Count > 1)
					{
						//gets oldest specific
						BidEntry oldest = null;
						foreach(BidEntry entry in specifics)
						{
							if(oldest == null || entry.DatePlaced < oldest.DatePlaced)
								oldest = entry;
						}
						
						winner = oldest.Bidder;
					}
					else
					{
						//no specifics! gets oldest of list of winners
						BidEntry oldest = null;
						foreach(BidEntry entry in winners)
						{
							if(oldest == null || entry.DatePlaced < oldest.DatePlaced)
								oldest = entry;
						}
						
                        if(oldest != null)
						    winner = oldest.Bidder;
					}
				}
			}
			
			//Give back gold
			foreach(KeyValuePair<Mobile, BidEntry> kvp in m_Auctioners)
			{
				Mobile m = kvp.Key;
				
				if(m != winner)
				{
					if(!Banker.Deposit(m, kvp.Value.Amount, true) && m.Backpack != null)
					{
						int total = kvp.Value.Amount;
						
						while(total > 60000)
						{
							m.Backpack.DropItem(new BankCheck(60000));
							total -= 60000;
						}
						
						if(total > 0)
							m.Backpack.DropItem(new BankCheck(total));
					}
				}
			}
			//Does actual changes to plots
            if (winner != null)
                MaginciaBazaar.AwardPlot(this, winner, highest);
            else
            {
                m_Plot.Reset(); // lease expires
                m_Plot.NewAuction(MaginciaBazaar.GetShortAuctionTime);
            }
		}
		
		public int GetBidAmount(Mobile from)
		{
			if(!m_Auctioners.ContainsKey(from))
				return 0;
				
			return m_Auctioners[from].Amount;
		}

        public void ChangeAuctionTime(TimeSpan ts)
        {
            m_AuctionEnd = DateTime.UtcNow + ts;

            if (m_Plot != null && m_Plot.Sign != null)
                m_Plot.Sign.InvalidateProperties();
        }

		public MaginciaPlotAuction(GenericReader reader, MaginciaBazaarPlot plot)
		{
			int version = reader.ReadInt();
			
			m_Plot = plot;
			m_AuctionEnd = reader.ReadDateTime();
			
			int c = reader.ReadInt();
			for(int i = 0; i < c; i++)
			{
				Mobile m = reader.ReadMobile();
				BidEntry entry = new BidEntry(reader);
				
				if(m != null)
					m_Auctioners[m] = entry;
			}
		}
		
		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);
			
			writer.Write(m_AuctionEnd);
			
			writer.Write(m_Auctioners.Count);
			foreach(KeyValuePair<Mobile, BidEntry> kvp in m_Auctioners)
			{
				writer.Write(kvp.Key);
				kvp.Value.Serialize(writer);
			}
		}
	}
	
	public enum BidType
	{
		Specific, 
		NextAvailable
	}
	
	public class BidEntry : IComparable
	{
		private Mobile m_Bidder;
		private int m_Amount;
		private BidType m_BidType;
		private DateTime m_DatePlaced;
		
		public Mobile Bidder { get { return m_Bidder; } }
		public int Amount { get { return m_Amount; } }
		public BidType BidType { get { return m_BidType; } }
		public DateTime DatePlaced { get { return m_DatePlaced; } }
		
		public BidEntry(Mobile bidder, int amount, BidType type)
		{
			m_Bidder = bidder;
			m_Amount = amount;
			m_BidType = type;
			m_DatePlaced = DateTime.UtcNow;
		}
		
		public int CompareTo( object obj )
		{
			return ((BidEntry)obj).m_Amount - m_Amount;
		}
		
		public BidEntry(GenericReader reader)
		{
			int version = reader.ReadInt();
			m_Bidder = reader.ReadMobile();
			m_Amount = reader.ReadInt();
			m_BidType = (BidType)reader.ReadInt();
			m_DatePlaced = reader.ReadDateTime();
		}
		
		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);
			writer.Write(m_Bidder);
			writer.Write(m_Amount);
			writer.Write((int)m_BidType);
			writer.Write(m_DatePlaced);
		}
	}
}