using System;
using System.Collections.Generic;
using Server;

namespace Server.Auction
{
    class AuctionEntry
    {
        public Mobile Owner;
        public int StartPrice;
        public List<Bid> Bids;
        public Item Item;
        public string Name;
        public string Description;
        public DateTime EndDate;

        public AuctionEntry(Mobile _owner, Item _item, string _name, string _description, DateTime _enddate, int _startprice)
        {
            Owner = _owner;
            Bids = new List<Bid>();
            Item = _item;
            Name = _name;
            Description = _description;
            EndDate = _enddate;
            StartPrice = _startprice;
        }

        public AuctionEntry() { }
    }

    class Bid
    {
        public Mobile From;
        public int Value;
        public Bid(Mobile _from, int _value)
        {
            From = _from;
            Value = _value;
        }
    }
}
