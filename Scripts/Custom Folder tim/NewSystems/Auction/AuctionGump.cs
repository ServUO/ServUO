using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Targeting;
using Server.Customs;

namespace Server.Auction
{
    class AuctionGump : BaseGridGump
    {
        int ipp = 20; //items per page
        AuctionContainer cont;
        public AuctionGump(Mobile from, AuctionContainer c) : base(50, 50)
        {

            cont = c;
            int pages = cont.AuctionItems.Count / ipp;
            if (cont.AuctionItems.Count % ipp != 0) pages++;
            if (pages <= 0) pages = 1;
            for (int p = 0; p < pages; p++)
            {
                AddNewPage();
                AddEntryHtml(650, Center(String.Format(StringList.GumpTitle, Region.Find(cont.Location, cont.Map), DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString())));
                

                AddNewLine();
                AddEntryHeader(30 - 1); //AddEntryHtml(30 - 1, Center("#"));
                AddEntryHtml(100 - 1, Center(StringList.OwnerHeader));
                AddEntryHtml(100 - 1, Center(StringList.ItemHeader));
                AddEntryHtml(100 - 1, Center(StringList.PriceHeader));
                AddEntryHtml(100 - 1, Center(StringList.LastBidHeader));
                AddEntryHtml(220, Center(StringList.EndHeader));
                #region lista
                for (int i = 0; i < ipp; i++)
                {
                    if (i + (p * ipp) < cont.AuctionItems.Count)
                    {
                        if (cont.AuctionItems[i] != null)
                        {
                            AddNewLine();
                            try
                            {
                                    AddEntryButton(30 - 1, ArrowRightID1, ArrowRightID2, (i + 1 + (p * ipp)), ArrowRightWidth, ArrowRightHeight); //AddEntryHtml(30-1, Center((i + 1 + (p * ipp)).ToString()));
                                
                                AddEntryLabel(100 - 1, cont.AuctionItems[i + (p * ipp)].Owner.Name);
                                AddEntryLabel(100 - 1, cont.AuctionItems[i + (p * ipp)].Name);

                                if (cont.AuctionItems[i + (p * ipp)].StartPrice <= 0)
                                {
                                    AddEntryLabel(100 - 1, StringList.DonationTag);
                                    AddEntryLabel(100 - 1, StringList.DashTag);
                                }
                                else
                                {
                                    List<Bid> bids = cont.AuctionItems[i + (p * ipp)].Bids;
                                    if (bids.Count == 0)
                                    {
                                        AddEntryLabel(100 - 1, cont.AuctionItems[i + (p * ipp)].StartPrice.ToString());
                                        AddEntryLabel(100 - 1, "--");
                                    }
                                    else
                                    {
                                        AddEntryLabel(100 - 1, bids[bids.Count - 1].Value.ToString());
                                        AddEntryLabel(100 - 1, bids[bids.Count - 1].From.Name);
                                    }
                                }
                                string ed = "ERROR 0x4";

                                if (cont.AuctionItems[i + (p * ipp)].EndDate.Date == DateTime.Now.Date)
                                {
                                    ed = String.Format(StringList.EndTodayTag, cont.AuctionItems[i + (p * ipp)].EndDate.ToShortTimeString());
                                }
                                else
                                {
                                    ed = cont.AuctionItems[i + (p * ipp)].EndDate.ToShortDateString() + " " + cont.AuctionItems[i + (p * ipp)].EndDate.ToShortTimeString();
                                }
                                AddEntryLabel(220, ed);
                            }
                            catch 
                            {
                                AddEntryHeader(30-1);
                                AddEntryLabel(620-1, "ERROR Code: 0x1"); //should never reach here
                            }
                        }
                    }
                    else
                    {
                        AddNewLine();
                        AddEntryHeader(30-1);
                        AddEntryHtml(620, Center(StringList.EmptyTag));
                    }
                }
                #endregion

                AddNewLine();
                AddBlankLine();

                if (p > 0)
                {
                    AddEntryPageButton(20, ArrowLeftID1, ArrowLeftID2, p, ArrowLeftWidth, ArrowLeftHeight);
                }
                else
                {
                    AddEntryHeader(20);
                }

                AddEntryHtml(608, Center(String.Format(StringList.Page, p + 1, pages)));

                if ((pages > 0) && p < pages - 1)
                {
                    AddEntryPageButton(20, ArrowRightID1, ArrowRightID2, p + 2, ArrowRightWidth, ArrowRightHeight);
                }
                else
                {
                    AddEntryHeader(20);
                }

                AddNewLine();
                AddEntryButton(20, ArrowRightID1, ArrowRightID2, 0xffff, ArrowRightWidth, ArrowRightHeight);
                AddEntryLabel(200, StringList.AuctionItem);
                AddEntryHeader(428);
                FinishPage();
            }
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0) return;
            if (info.ButtonID == 0xffff)
            {
                sender.Mobile.Target = new AddAuctionTarget(cont);
                sender.Mobile.SendMessage(StringList.SelectItem);
                return;
            }

            if (info.ButtonID - 1 >= cont.AuctionItems.Count)
            {
                sender.Mobile.SendMessage("Bad selection - ERROR: 0x2");
                sender.Mobile.SendGump(new AuctionGump(sender.Mobile, cont));
                return;
            }

            AuctionEntry tobid = cont.AuctionItems[info.ButtonID - 1];
            if (cont.AuctionItems.Contains(tobid))
            {
                ViewItemGump gump = new ViewItemGump(tobid, cont, sender.Mobile);
                sender.Mobile.SendGump(gump);
                //gump.bag.DisplayTo(sender.Mobile);
            }
            else
            {
                sender.Mobile.SendMessage("Bad Selection - ERROR: 0x3");
                sender.Mobile.SendGump(new AuctionGump(sender.Mobile, cont));
            }

        }

        private class AddAuctionTarget : Target
        {
            AuctionContainer cont;
            public AddAuctionTarget(AuctionContainer c) : base(0, false, TargetFlags.None)
            {
                cont = c;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is Item))
                {
                    from.SendMessage(StringList.BadSelection);
                    from.Target = new AddAuctionTarget(cont);
                    return;
                }

                Item i = (Item)targeted;
                if (!(i.ParentEntity == from.Backpack))
                {
                    OnCantSeeTarget(from, targeted);
                    return;
                }

                if ((i is Gold) || (i is BankCheck))
                {
                    from.SendMessage(StringList.BadSelection);
                    from.Target = new AddAuctionTarget(cont);
                    return;
                }

                if (i is Container)
                {
                    from.SendMessage(StringList.BadSelection);
                    from.Target = new AddAuctionTarget(cont);
                    return;
                }
                string[] param = { null, null, null, null };
                from.SendGump(new AddAuctionGump(from, cont, i, param));
            }

            protected override void OnCantSeeTarget(Mobile from, object targeted)
            {
                from.Target = new AddAuctionTarget(cont);
                from.SendMessage(StringList.MustBeOnPack);
            }

            protected override void OnNonlocalTarget(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                OnTargetFinish(from);
            }

            protected override void OnTargetDeleted(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetInSecureTrade(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetNotAccessible(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetOutOfLOS(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetUntargetable(Mobile from, object targeted)
            {
                OnCantSeeTarget(from, targeted);
            }
        }
    }
}
