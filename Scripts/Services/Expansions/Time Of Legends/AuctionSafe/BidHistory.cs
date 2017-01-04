using Server;
using System;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;

namespace Server.Engines.Auction
{
    public class HistoryEntry
    {
        public Mobile Mobile { get; set; }
        public long Bid { get; set; }
        public bool ShowRealBid { get; set; }
        public DateTime BidTime { get; set; }

        public HistoryEntry(Mobile m, long bid)
        {
            Mobile = m;
            Bid = bid;

            BidTime = DateTime.Now;
        }

        public HistoryEntry(GenericReader reader)
        {
            int version = reader.ReadInt();
            Mobile = reader.ReadMobile();
            Bid = reader.ReadLong();
            ShowRealBid = reader.ReadBool();
            BidTime = reader.ReadDateTime();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write(Mobile);
            writer.Write(Bid);
            writer.Write(ShowRealBid);
            writer.Write(BidTime);
        }
    }
}