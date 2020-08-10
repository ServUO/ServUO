using Server.Accounting;
using Server.Mobiles;
using System;

namespace Server.Engines.Auction
{
    [PropertyObject]
    public class BidEntry : IComparable<BidEntry>
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile Mobile { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public long CurrentBid { get; set; }

        //Converts to gold/plat
        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalGoldBid => (int)(CurrentBid >= Account.CurrencyThreshold ? CurrentBid - (TotalPlatBid * Account.CurrencyThreshold) : CurrentBid);

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalPlatBid => (int)(CurrentBid >= Account.CurrencyThreshold ? CurrentBid / Account.CurrencyThreshold : 0);

        public BidEntry(PlayerMobile m, long bid = 0)
        {
            Mobile = m;
            CurrentBid = bid;
        }

        public BidEntry(PlayerMobile m, GenericReader reader)
        {
            Mobile = m;

            int version = reader.ReadInt();
            CurrentBid = reader.ReadLong();
        }

        public void Refund(Auction auction, long amount)
        {
            Account a = Mobile.Account as Account;

            if (a != null)
            {
                a.DepositGold(amount);
                VaultLogging.LogRefund(auction, Mobile, amount);
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write(CurrentBid);
        }

        public int CompareTo(BidEntry entry)
        {
            if (CurrentBid > entry.CurrentBid)
                return 1;

            return 0;
        }
    }
}
