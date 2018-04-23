using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Customs.MessageLog;

namespace Server.Auction
{
    class ViewItemGump : BaseGridGump
    {
        public AuctionEntry bid;
        public AuctionContainer cont;
        public Backpack bag;
        public VirtualItem item;
        public ViewItemGump(AuctionEntry tobid,AuctionContainer c,Mobile ToDisplay)
            : base(50, 50)
        {
            bid = tobid;
            cont = c;

            if ( ToDisplay != null )
            {
                bag = new Backpack();
                item = new VirtualItem( bid.Item );
                //bag.Layer = Layer.Invalid;
                bag.AddItem( item );
                bag.MoveToWorld( ToDisplay.Location, ToDisplay.Map );
                bag.Z -= 10;
Timer.DelayCall(TimeSpan.FromSeconds(15), delegate { bag.Delete(); });
                bag.Movable = false;
                //ToDisplay.AddItem( bag );
                bag.DisplayTo( ToDisplay );
            }

            AddNewPage();
            AddEntryHtml(500, tobid.Owner.Name); AddNewLine();
            AddEntryHtml(500, tobid.Name); AddNewLine();
            AddEntryHtml(500, tobid.Description); AddNewLine();
            int minprice = 0;
            if (bid.Bids.Count == 0)
                minprice = bid.StartPrice;
            else
                minprice = bid.Bids[bid.Bids.Count - 1].Value + 1;

            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 1, ArrowRightWidth, ArrowRightHeight); AddEntryHtml(479, "VIEW ITEM"); AddNewLine();
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight); AddEntryHtml(59, "BID"); AddEntryText(119, 0, (minprice).ToString()); AddEntryHeader(299);

            for (int i = 1; i <= 10; i++)
            {
                if (bid.Bids.Count - i < bid.Bids.Count && bid.Bids.Count - i >=0)
                {
                    AddNewLine();
                    AddEntryLabel(100, bid.Bids[bid.Bids.Count - i].From.Name);
                    AddEntryLabel(099, bid.Bids[bid.Bids.Count - i].Value.ToString());
                    AddEntryHeader( 299 );
                }
            }
            FinishPage();
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            bag.Delete();
            item.Delete();
            if (info.ButtonID == 0)
            {
                sender.Mobile.SendGump(new AuctionGump(sender.Mobile, cont));
            }

            if (info.ButtonID == 1)
            {

                sender.Mobile.SendGump(new ViewItemGump(bid,cont,sender.Mobile));
                return;
            }

            if (info.ButtonID == 2)
            {
                if (sender.Mobile == bid.Owner && sender.Mobile.AccessLevel == AccessLevel.Player)
                {
                    sender.Mobile.SendGump(new ViewItemGump(bid, cont, sender.Mobile));
                    sender.Mobile.SendMessage(StringList.CantBidOwn);
                    return;
                }
                if (bid.StartPrice <= 0)
                {
                    sender.Mobile.Backpack.AddItem(bid.Item);
                    sender.Mobile.SendGump(new AuctionGump(sender.Mobile, cont));
                    cont.AuctionItems.Remove(bid);
                    MessageLog.Log(sender.Mobile, StringList.DonationMoved);
                    return;
                }
                int offer;
                int minprice = 0;
                if (bid.Bids.Count == 0)
                    minprice = bid.StartPrice;
                else
                    minprice = bid.Bids[bid.Bids.Count - 1].Value + 1;
                if (int.TryParse(info.GetTextEntry(0).Text, out offer) && offer < int.MaxValue)
                {
                    if (offer >= minprice)
                    {
                        if (cont.AuctionItems.Contains(bid))
                        {
                            sender.Mobile.SendMessage(StringList.BidRegist);
                            bid.Bids.Add(new Bid(sender.Mobile, offer));
                            sender.Mobile.SendGump(new AuctionGump(sender.Mobile, cont));
                        }
                        else
                        {
                            sender.Mobile.SendGump(new AuctionGump(sender.Mobile, cont));
                            sender.Mobile.SendMessage(StringList.AlreadyAuctioned);
                        }
                    }
                    else
                    {
                        sender.Mobile.SendMessage(StringList.HigherValue);
                        sender.Mobile.SendGump(this);
                    }
                }
                else
                {
                    sender.Mobile.SendMessage(StringList.InvalidValue);
                    sender.Mobile.SendGump(this);
                }
            }
        }

        private class ViewContainer : Container
        {
            public ViewContainer()
                : base(0)
            {
                ItemID = 0;
                GumpID = 60;
                Weight = 0;
            }

            public ViewContainer(Serial s) : base(s) { }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
            }
        }
    }
}
