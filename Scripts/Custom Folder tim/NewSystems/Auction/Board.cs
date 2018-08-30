using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Customs;
using Server.Network;
using Server.Customs.MessageLog;

namespace Server.Auction
{
    [Flipable(0x1E5E, 0x1E5F)]
    class AuctionContainer : BaseContainer
    {
        public List<AuctionEntry> AuctionItems;

        private Timer ATimer;

        [Constructable]
        public AuctionContainer()
            : base(0x1E5E)
        {
            Name = StringList.BoardName;
            Movable = false;
            AuctionItems = new List<AuctionEntry>();
            ATimer = new AuctionTimer(this);
            ATimer.Start();
            LiftOverride = true;
            GumpID = 60;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Alive)
            {
                from.SendMessage(StringList.Dead);
                return;
            }
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendMessage(StringList.TooFar);
                return;
            }
            from.SendGump(new AuctionGump(from, this));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            list.Add(String.Format(StringList.TipName, Region.Find(Location, Map).Name));
            list.Add(String.Format(StringList.TipCount, AuctionItems.Count));
        }

        public override void OnSingleClick(Mobile from)
        {
            from.Send(new UnicodeMessage(Serial, 
                ItemID, 
                MessageType.Label, 
                0x3B2, 
                3, 
                "ENU", 
                "", 
                String.Format(StringList.TipName, Region.Find(Location, Map).Name))
                );
        }

        public override void DropItem(Item dropped)
        {
            return;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            return false;
        }

        public AuctionContainer(Serial s) : base(s) {
            ATimer = new AuctionTimer(this);
            ATimer.Start();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)AuctionItems.Count);
            for (int i = 0; i < AuctionItems.Count; i++)
            {
                writer.Write(AuctionItems[i].Owner);

                writer.Write(AuctionItems[i].Bids.Count);
                for (int j = 0; j < AuctionItems[i].Bids.Count; j++)
                {
                    writer.Write(AuctionItems[i].Bids[j].From);
                    writer.Write(AuctionItems[i].Bids[j].Value);
                }

                writer.Write(AuctionItems[i].Item);
                writer.Write(AuctionItems[i].Name);
                writer.Write(AuctionItems[i].Description);
                writer.WriteDeltaTime(AuctionItems[i].EndDate);
                writer.Write(AuctionItems[i].StartPrice);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            AuctionItems = new List<AuctionEntry>();
            switch (version)
            {
                case 0:
                    {
                        int toread = reader.ReadInt();
                        for (int i = 0; i<toread;i++)
                        {
                            AuctionEntry ae = new AuctionEntry();
                            ae.Bids = new List<Bid>();
                            ae.Owner = reader.ReadMobile();

                            int bids = reader.ReadInt();
                            for (int j = 0; j < bids; j++)
                            {
                                ae.Bids.Add(new Bid(reader.ReadMobile(), reader.ReadInt()));
                            }

                            ae.Item = reader.ReadItem();
                            ae.Name = reader.ReadString();
                            ae.Description = reader.ReadString();
                            ae.EndDate = reader.ReadDeltaTime();
                            ae.StartPrice = reader.ReadInt();
                            AuctionItems.Add(ae);
                        }
                        break;
                    }
            }
        }

        public void DoAuction()
        {
            List<AuctionEntry> toremove = new List<AuctionEntry>();
            foreach (AuctionEntry a in AuctionItems)
            {
                if (a.EndDate < DateTime.Now)
                {
                    toremove.Add(a);
                    bool sold = false;
                    if (a.Bids.Count > 0)
                    {
                        int offers = a.Bids.Count - 1;
                        bool next = true;
                        while (next)
                        {
                            if (offers < 0) break;
                            Mobile buyer = a.Bids[offers].From;
                            if ( !a.Bids[offers].From.Deleted && a.Bids[offers].From.BankBox.ConsumeTotal( typeof( Gold ), a.Bids[offers].Value ) )
                            {
                                if (!a.Owner.Deleted) a.Owner.BankBox.AddItem(new BankCheck(a.Bids[offers].Value));
                                buyer.BankBox.AddItem(a.Item);
                                a.Item.Movable = true;

                                if (!a.Owner.Deleted) MessageLog.Log(a.Owner, String.Format(StringList.EndOwnerWarn, a.Name, buyer.Name, a.Bids[offers].Value));
                                MessageLog.Log(buyer, String.Format(StringList.EndBuyerWarn, a.Name, a.Bids[offers].Value));
                                next = false;
                                sold = true;
                                break;

                            }
                            else
                            {
                                MessageLog.Log( buyer, string.Format( StringList.NoGoldWarn, a.Name ) );
                                offers--;
                            }
                        }
                    }

                    if (!sold)
                    {
                        if (!a.Owner.Deleted)
                        {
                            a.Item.Movable = true;
                            MessageLog.Log(a.Owner, string.Format(StringList.EndOwnerNoBids, a.Name));
                            a.Owner.BankBox.AddItem(a.Item);
                        }
                        else
                        {
                            a.Item.Delete();
                        }
                    }
                }
            }

            foreach (AuctionEntry a in toremove)
            {
                AuctionItems.Remove(a);
            }

            InvalidateProperties();
        }

        private class AuctionTimer : Timer
        {
            AuctionContainer cont;

            public AuctionTimer(AuctionContainer c)
                : base(TimeSpan.FromMinutes(1))
            {
                cont = c;
            }

            protected override void OnTick()
            {
                cont.DoAuction();
                Start();
            }
        }
    }
}
