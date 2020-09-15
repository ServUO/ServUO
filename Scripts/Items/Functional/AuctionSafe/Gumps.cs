using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Globalization;

namespace Server.Engines.Auction
{
    public class BaseAuctionGump : Gump
    {
        public const int Blue = 0x1FF;
        public const int Yellow = 0x6B45;
        public const int White = 0x7FFF;
        public const int Gray = 0x4E73;
        public const string HGray = "BFBFBF";

        public const int Length = 400;
        public const int Height = 600;

        public IAuctionItem Safe { get; set; }
        public bool Owner { get; set; }
        public PlayerMobile User { get; set; }
        public Auction Auction { get; set; }

        public BaseAuctionGump(PlayerMobile p, IAuctionItem safe)
            : base(100, 100)
        {
            Safe = safe;
            Auction = safe.Auction;
            User = p;

            AddGumpLayout();
        }

        public void Refresh()
        {
            Entries.Clear();
            Entries.TrimExcess();
            AddGumpLayout();

            User.CloseGump(GetType());
            User.SendGump(this, false);
        }

        public string Color(string color, string str)
        {
            return string.Format("<basefont color=#{0}>{1}", color, str);
        }

        public virtual void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, Length, Height, 39925);

            AddHtmlLocalized(15, 25, 360, 18, 1114513, "#1156371", White, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddHtmlLocalized(15, 52, 360, 18, 1114513, Owner ? "#1150328" : "#1156442", Blue, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            AddHtmlLocalized(80, 88, 110, 22, 3000098, Yellow, false, false); // Information
            AddButton(40, 88, 4005, 4007, 100, GumpButtonType.Reply, 0);

            AddHtmlLocalized(265, 88, 110, 22, 3010004, Yellow, false, false); // History
            AddButton(225, 88, 4005, 4007, 101, GumpButtonType.Reply, 0);

            Account acct = User.Account as Account;

            AddHtmlLocalized(15, 117, 175, 18, 1114514, "#1156044", Yellow, false, false); // Total Gold:
            AddHtml(200, 117, 175, 18, Color(HGray, acct != null ? acct.TotalGold.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : "0"), false, false);

            AddHtmlLocalized(15, 137, 175, 18, 1114514, "#1156045", Yellow, false, false); // Total Platinum:
            AddHtml(200, 137, 175, 18, Color(HGray, acct != null ? acct.TotalPlat.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : "0"), false, false);

            if (Auction != null)
                Auction.AddViewer(User);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 100: Refresh(); User.SendGump(new AuctionInfoGump(User)); break;
                case 101: Refresh(); User.SendGump(new BidHistoryGump(User, Auction)); break;
            }

            if (Auction != null)
                Auction.RemoveViewer(User);
        }
    }

    public class AuctionOwnerGump : BaseAuctionGump
    {
        private long _TempBid;
        private long _TempBuyout;
        private bool _NoBid;

        public AuctionOwnerGump(PlayerMobile pm, IAuctionItem safe)
            : base(pm, safe)
        {
            Owner = true;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            _TempBid = 0;
            _TempBuyout = 0;
            _NoBid = false;

            if (Auction == null)
            {
                if (Safe.Auction != null)
                    Auction = Safe.Auction;
                else
                    Safe.Auction = Auction = new Auction(User, Safe);
            }

            int y = 166;

            // Add Auction Item
            AddHtmlLocalized(200, y, 175, 22, 1156421, Yellow, false, false); // Select New Auction Item
            AddButton(160, y, 4005, 4007, 1, GumpButtonType.Reply, 0);

            y += 24;

            // Description
            AddHtmlLocalized(15, y, 175, 110, 1114514, "#1156400", Yellow, false, false); // Description:
            AddButton(345, y, 4014, 4016, 2, GumpButtonType.Reply, 0);

            AddBackground(200, y, 140, 110, 9350);
            AddTextEntry(202, y + 2, 136, 106, 0, 1, Auction.Description, 140);

            // Display Item
            if (Auction.AuctionItem != null)
            {
                Item i = Auction.AuctionItem;
                AddImageTiledButton(102, 212, 0x918, 0x918, 0x0, GumpButtonType.Page, 0, i.ItemID, i.Hue, 23, 5);
                AddItemProperty(i.Serial);
            }

            y += 112;

            AddHtmlLocalized(15, y, 175, 18, 1114514, "#1156404", Yellow, false, false); // Time Remaining:

            if (Auction.HasBegun)
            {
                TimeSpan left = Auction.EndTime - DateTime.Now;
                int cliloc;
                double v;

                if (left.TotalSeconds < 0 || Auction.InClaimPeriod)
                {
                    AddHtmlLocalized(200, y, 175, 18, 1114513, "#1156438", Gray, false, false); // Auction Ended
                }
                else
                {
                    if (left.TotalDays >= 1)
                    {
                        cliloc = 1153091; // Lifespan: ~1_val~ days
                        v = left.TotalDays;
                    }
                    else if (left.TotalHours >= 1)
                    {
                        cliloc = 1153090; // Lifespan: ~1_val~ hours
                        v = left.TotalHours;
                    }
                    else
                    {
                        cliloc = 1153089; // Lifespan: ~1_val~ minutes
                        v = left.TotalMinutes;
                    }

                    AddHtmlLocalized(200, y, 175, 18, cliloc, ((int)v).ToString(), Gray, false, false);
                }
            }
            else
            {
                TimeSpan ts = TimeSpan.FromMinutes(Auction.Duration);

                if (ts.TotalMinutes > 60)
                {
                    AddHtmlLocalized(200, y, 175, 18, 1153091, string.Format("{0}", ts.TotalDays), Gray, false, false); // Lifespan: ~1_val~ days
                }
                else
                {
                    AddHtmlLocalized(200, y, 175, 18, 1153089, string.Format("{0}", ts.TotalMinutes), Gray, false, false); // Lifespan: ~1_val~ minutes
                }
            }

            y += 20;

            AddHtmlLocalized(200, y, 140, 20, 1114514, "#1156455", Yellow, false, false); // One Hour
            AddButton(345, y, 4014, 4016, 3, GumpButtonType.Reply, 0);

            y += 20;

            AddHtmlLocalized(200, y, 140, 20, 1114514, "#1156418", Yellow, false, false); // Three Days
            AddButton(345, y, 4014, 4016, 4, GumpButtonType.Reply, 0);

            y += 20;

            AddHtmlLocalized(Length / 2, y, 140, 20, 1114514, "#1156419", Yellow, false, false); // Five Days
            AddButton(345, y, 4014, 4016, 5, GumpButtonType.Reply, 0);

            y += 20;

            AddHtmlLocalized(200, y, 140, 20, 1114514, "#1156420", Yellow, false, false); // Seven Days
            AddButton(Length - 55, y, 4014, 4016, 6, GumpButtonType.Reply, 0);

            y += 24;

            int[] startbid = GetPlatGold(Auction.StartBid);

            // Start Bid Plat/Gold
            AddHtmlLocalized(15, y, 175, 22, 1114514, "#1156410", Yellow, false, false); // Item Starting Bid Plat:
            AddBackground(200, y, 175, 22, 9350);
            AddTextEntry(202, y, 171, 18, 0, 2, startbid[0] > 0 ? startbid[0].ToString() : "", 9);

            y += 24;

            AddHtmlLocalized(15, y, 175, 22, 1114514, "#1156411", Yellow, false, false); // Item Starting Bid Gold:
            AddBackground(200, y, 175, 22, 9350);
            AddTextEntry(202, y, 171, 18, 0, 3, startbid[1] > 0 ? startbid[1].ToString() : "", 9);

            y += 24;

            AddHtmlLocalized(200, y, 175, 22, 1156416, Yellow, false, false); // Set Starting Bids
            AddButton(160, y, 4005, 4007, 7, GumpButtonType.Reply, 0);

            y += 26;

            // Buy Now
            AddHtmlLocalized(15, y, 175, 22, 1114514, "#1156413", Yellow, false, false); // Buy Now Plat Price:
            AddBackground(200, y, 175, 22, 9350);
            AddTextEntry(202, y + 2, 171, 18, 0, 4, Auction.BuyoutPlat > 0 ? Auction.BuyoutPlat.ToString() : "", 9);

            y += 26;

            AddHtmlLocalized(15, y, 175, 22, 1114514, "#1156412", Yellow, false, false); // Buy Now Gold Price:
            AddBackground(200, y, 175, 22, 9350);
            AddTextEntry(202, y, 171, 18, 0, 5, Auction.BuyoutGold > 0 ? Auction.BuyoutGold.ToString() : "", 9);

            y += 24;

            AddHtmlLocalized(200, y, 175, 22, 1156417, Yellow, false, false); // Set Buy Now Price
            AddButton(160, y, 4005, 4007, 8, GumpButtonType.Reply, 0);

            y += 24;

            if (Auction.AuctionItemOnDisplay() && !Auction.OnGoing)
            {
                AddHtmlLocalized(200, y, 175, 22, 1156414, Yellow, false, false); // Start Auction
                AddButton(160, y, 4005, 4007, 9, GumpButtonType.Reply, 0);
            }

            if (Auction.OnGoing && Auction.HighestBid == null)
            {
                AddHtmlLocalized(200, y, 175, 22, 1156415, Yellow, false, false); // Stop Auction
                AddButton(160, y, 4005, 4007, 23, GumpButtonType.Reply, 0);
            }
        }

        public int[] GetPlatGold(long amount)
        {
            int plat = 0;
            int gold = 0;

            if (amount >= Account.CurrencyThreshold)
            {
                plat = (int)(amount / Account.CurrencyThreshold);
                gold = (int)(amount - (plat * Account.CurrencyThreshold));
            }
            else
            {
                gold = (int)amount;
            }

            return new int[] { plat, gold };
        }

        private class InternalTarget : Target
        {
            private readonly Auction Auction;
            private readonly BaseAuctionGump Gump;

            public InternalTarget(Auction auction, BaseAuctionGump g)
                : base(-1, false, TargetFlags.None)
            {
                Auction = auction;
                Gump = g;
            }

            private bool IsBadItem(Item item)
            {
                return item == null || item.Weight > 300 || (item is Container && !(item is BaseQuiver)) || item is Gold || item is BankCheck || !item.Movable || item.Items.Count > 0;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (Auction == null || Auction.Safe == null || Auction.Safe.Deleted)
                {
                    return;
                }

                if (targeted is Item)
                {
                    Item item = targeted as Item;

                    if (!IsBadItem(item))
                    {
                        if (item.IsChildOf(from.Backpack))
                        {
                            Auction.AuctionItem = item;
                            item.Movable = false;
                            item.MoveToWorld(new Point3D(Gump.Safe.X, Gump.Safe.Y, Gump.Safe.Z + 7), Gump.Safe.Map);
                            Gump.Refresh();
                        }
                        else
                        {
                            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                            from.Target = new InternalTarget(Auction, Gump);
                        }
                    }
                    else
                    {
                        from.Target = new InternalTarget(Auction, Gump);
                    }
                }
                else
                {
                    from.Target = new InternalTarget(Auction, Gump);
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                Gump.Refresh();
                from.SendLocalizedMessage(1149667); // Invalid target.
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            base.OnResponse(state, info);

            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 1:
                    {
                        if (Auction.CheckModifyAuction(User, true))
                        {
                            if (Auction.AuctionItem != null)
                            {
                                if (Auction.AuctionItem.LabelNumber != 0)
                                {
                                    from.SendLocalizedMessage(1152339, string.Format("#{0}", Auction.AuctionItem.LabelNumber)); // A reward of ~1_ITEM~ has been placed in your backpack.
                                }
                                else
                                {
                                    from.SendLocalizedMessage(1152339, Auction.AuctionItem.Name); // A reward of ~1_ITEM~ has been placed in your backpack.
                                }

                                Auction.AuctionItem.Movable = true;
                                from.AddToBackpack(Auction.AuctionItem);
                                Auction.AuctionItem = null;
                            }

                            from.Target = new InternalTarget(Auction, this);
                        }
                        else
                        {
                            Refresh();
                        }
                        break;
                    }
                case 2:
                    {
                        if (Auction.CheckModifyAuction(User))
                        {
                            TextRelay relay = info.GetTextEntry(1);
                            string str = null;

                            if (relay != null)
                                str = relay.Text;

                            if (str != null || Guilds.BaseGuildGump.CheckProfanity(str, 140))
                            {
                                Auction.Description = Utility.FixHtml(str.Trim());
                            }
                            else
                            {
                                from.SendLocalizedMessage(1150315); // That text is unacceptable.
                            }
                        }

                        Refresh();
                        break;
                    }
                case 3:
                    {
                        if (Auction.CheckModifyAuction(User))
                            Auction.Duration = 60;

                        Refresh();
                        break;
                    }
                case 4:
                    {
                        if (Auction.CheckModifyAuction(User))
                            Auction.Duration = 4320;

                        Refresh();
                        break;
                    }
                case 5:
                    {
                        if (Auction.CheckModifyAuction(User))
                            Auction.Duration = 7200;

                        Refresh();
                        break;
                    }
                case 6:
                    {
                        if (Auction.CheckModifyAuction(User))
                            Auction.Duration = 10080;

                        Refresh();
                        break;
                    }
                case 7:
                    {
                        if (Auction.CheckModifyAuction(User))
                        {
                            TextRelay relay1 = info.GetTextEntry(2);

                            string plat1 = null;
                            string gold1 = null;

                            if (relay1 != null)
                                plat1 = relay1.Text;

                            relay1 = info.GetTextEntry(3);

                            if (relay1 != null)
                                gold1 = relay1.Text;

                            long platAmnt = Utility.ToInt64(plat1);
                            long goldAmnt = Utility.ToInt64(gold1);

                            if (platAmnt >= 0 && goldAmnt >= 0)
                            {
                                _TempBid += platAmnt * Account.CurrencyThreshold;
                                _TempBid += goldAmnt;
                            }
                            else
                            {
                                from.SendLocalizedMessage(1150315); // That text is unacceptable.
                                _NoBid = true;
                            }

                            if (!_NoBid)
                            {
                                if (Auction.OnGoing && Auction.BidHistory == null)
                                {
                                    Auction.CurrentBid = _TempBid;
                                }

                                Auction.StartBid = _TempBid;
                            }
                        }

                        Refresh();
                        break;
                    }
                case 8:
                    {
                        if (Auction.CheckModifyAuction(User))
                        {
                            TextRelay relay2 = info.GetTextEntry(4);

                            string plat2 = null;
                            string gold2 = null;

                            if (relay2 != null)
                                plat2 = relay2.Text;

                            relay2 = info.GetTextEntry(5);

                            if (relay2 != null)
                                gold2 = relay2.Text;

                            long platAmnt2 = Utility.ToInt64(plat2);
                            long goldAmnt2 = Utility.ToInt64(gold2);

                            if (platAmnt2 >= 0 && goldAmnt2 >= 0)
                            {
                                _TempBuyout += platAmnt2 * Account.CurrencyThreshold;
                                _TempBuyout += goldAmnt2;
                            }
                            else
                            {
                                from.SendLocalizedMessage(1150315); // That text is unacceptable.
                            }

                            Auction.Buyout = _TempBuyout;
                        }

                        Refresh();
                        break;
                    }
                case 9:
                    {
                        if (Auction.StartBid <= 0)
                        {
                            User.SendLocalizedMessage(1156434); // You must set a starting bid.
                        }
                        else
                        {
                            Auction.OnBegin();
                        }

                        Refresh();
                        break;
                    }
                case 23:
                    {
                        if (Auction.OnGoing && Auction.HighestBid == null)
                        {
                            Auction.ClaimPrize(User);
                        }

                        break;
                    }
            }
        }
    }

    public class AuctionBidGump : BaseAuctionGump
    {
        public long TempBid { get; set; }

        public AuctionBidGump(PlayerMobile pm, IAuctionItem safe)
            : base(pm, safe)
        {
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            TempBid = 0;

            // Display Item
            if (Auction.AuctionItem != null)
            {
                Item i = Auction.AuctionItem;
                AddImageTiledButton(200, 166, 0x918, 0x918, 0x0, GumpButtonType.Page, 0, i.ItemID, i.Hue, 23, 5);
                AddItemProperty(i.Serial);
            }

            AddHtmlLocalized(15, 238, 175, 90, 1114514, "#1156400", Yellow, false, false); // Description:
            AddHtml(200, 238, 175, 90, Auction.Description, true, true);

            AddHtmlLocalized(15, 330, 175, 18, 1114514, "#1156404", Yellow, false, false); // Time Remaining:

            if (Auction.HasBegun)
            {
                TimeSpan left = Auction.EndTime - DateTime.Now;
                int cliloc;
                double v;

                if (left.TotalSeconds < 0 || Auction.InClaimPeriod)
                {
                    AddHtmlLocalized(200, 330, 175, 18, 1114513, "#1156438", Gray, false, false); // Auction Ended
                }
                else
                {
                    if (left.TotalDays >= 1)
                    {
                        cliloc = 1153091; // Lifespan: ~1_val~ days
                        v = left.TotalDays;
                    }
                    else if (left.TotalHours >= 1)
                    {
                        cliloc = 1153090; // Lifespan: ~1_val~ hours
                        v = left.TotalHours;
                    }
                    else
                    {
                        cliloc = 1153089; // Lifespan: ~1_val~ minutes
                        v = left.TotalMinutes;
                    }

                    AddHtmlLocalized(200, 330, 175, 18, cliloc, ((int)v).ToString(), Gray, false, false);
                }
            }
            else
            {
                AddHtmlLocalized(200, 330, 175, 18, 1114513, "#1156440", Gray, false, false); // Auction Pending                
            }

            AddHtmlLocalized(15, 350, 175, 18, 1114514, "#1156436", Yellow, false, false); // Current Platinum Bid:
            AddHtml(200, 350, 175, 18, Color(HGray, Auction.CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(15, 370, 175, 18, 1114514, "#1156435", Yellow, false, false); // Current Gold Bid:
            AddHtml(200, 370, 175, 18, Color(HGray, Auction.CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);

            AddHtmlLocalized(15, 392, 175, 22, 1114514, "#1156406", Yellow, false, false); // Your Current Platinum Bid:
            AddBackground(200, 392, 175, 22, 9350);
            AddTextEntry(202, 394, 171, 18, 0, 1, "", 9);

            AddHtmlLocalized(15, 418, 175, 22, 1114514, "#1156405", Yellow, false, false); // Your Current Gold Bid:
            AddBackground(200, 418, 175, 22, 9350);
            AddTextEntry(202, 420, 171, 18, 0, 2, "", 9);

            AddHtmlLocalized(200, 442, 175, 22, 1156407, Yellow, false, false); // Place Bid
            AddButton(160, 442, 4005, 4007, 1, GumpButtonType.Reply, 0);

            if (Auction.Buyout > 0 && (Auction.HighestBid == null || Auction.HighestBid != null && Auction.HighestBid.Mobile != User))
            {
                AddHtmlLocalized(15, 484, 175, 18, 1114514, "#1156413", Yellow, false, false); // Buy Now Plat Price:
                AddHtml(200, 484, 175, 18, Color(HGray, Auction.BuyoutPlat.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);

                AddHtmlLocalized(15, 502, 175, 18, 1114514, "#1156412", Yellow, false, false); // Buy Now Gold Price:
                AddHtml(200, 502, 175, 18, Color(HGray, Auction.BuyoutGold.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);

                AddHtmlLocalized(200, 520, 175, 22, 1156409, Yellow, false, false); // Buy Now
                AddButton(160, 520, 4005, 4007, 2, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            base.OnResponse(state, info);

            switch (info.ButtonID)
            {
                case 1:
                    {
                        TextRelay relay = info.GetTextEntry(1);
                        string gold = null;
                        string plat = null;

                        if (relay != null)
                            plat = relay.Text;

                        relay = info.GetTextEntry(2);

                        if (relay != null)
                            gold = relay.Text;

                        long val = Utility.ToInt64(plat);

                        if (val < 0) val = 0;

                        TempBid += val * Account.CurrencyThreshold;

                        val = Utility.ToInt64(gold);

                        if (val < 0) val = 0;

                        TempBid += val;

                        Auction.TryPlaceBid(User, TempBid);
                        Refresh();
                        Auction.ResendGumps(User);
                        break;
                    }
                case 2:
                    {
                        Auction.TryBuyout(User);
                        User.SendGump(new AuctionBidGump(User, Safe));
                        break;
                    }
            }
        }
    }

    public class AuctionInfoGump : Gump
    {
        public AuctionInfoGump(PlayerMobile pm)
            : base(100, 200)
        {
            AddPage(0);

            AddBackground(0, 0, 600, 400, 0x9BF5);
            AddHtmlLocalized(50, 10, 500, 18, 1114513, "#1156371", 0x7FFF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            /*<DIV ALIGN=CENTER>Auction Safe</DIV><DIV ALIGN=LEFT><BR>Auction Safe deeds can be obtained from the veteran reward system.<BR>An Auction
             * Safe can be placed within public houses. When placed in a home the owner will be able to set access security on which users are allowed 
             * to place bids.<br>Setting up an auction first requires you to select an item from your backpack for auction. This item cannot be gold, 
             * a container, over 399 stones in weight, and must meet <A HREF="http://uo.com/wiki/ultima-online-wiki/gameplay/npc-commercial-transactions/npcs-player-owned"> 
             * requirements for adding an item to a vendor.</A>Once the item has been added it will be placed on the auction safe to be displayed.
             * A starting bid and auction length must be set before you can start the auction and cannot be changed once the auction begins. 140 
             * characters can be used to describe your auction item which can be updated at any time. A Buy Now price can be set which will allow 
             * customers to skip a bidding war and purchase the item at listed cost, but by doing so includes an approximate 5% fee on the purchase
             * price.<br>On completion of the auction the owner account will receive their payment immediately and will be notified by in game mail 
             * of the outcome. The winning bidder will now have 3 days to retrieve their item from the auction safe or it will revert back to the 
             * owner. Once the auctioned item has been retrieved the auction safe will once again be available to start a new auction.<br><br>In order
             * to bid on an auction, players must have<A HREF="http://uo.com/wiki/ultima-online-wiki/player/money"> currency available in their 
             * account.</A> Bidders can then place a bid for the maximum amount they are willing to pay for the listed item. Funds will immediately 
             * be removed from your account if your bid is successful. If your bid is higher than the current maximum bid yours will become the current
             * winning bid. If you are out bid as the winning bid you will be notified by in game mail and your bid will be refunded to your account. 
             * On completion of the auction if you are the winning bid you will be notified that you have three days to claim your item. Upon claiming
             * your item if you have any change as a result of your maximum bid it will be refunded to you.</DIV>*/
            AddHtmlLocalized(50, 37, 500, 313, 1156441, 0x4100, true, true);
        }
    }

    public class BidHistoryGump : Gump
    {
        private readonly int Green = 0x208;
        public const string HGray = "BFBFBF";

        public Auction Auction { get; set; }

        public BidHistoryGump(PlayerMobile pm, Auction auction)
            : base(100, 200)
        {
            Auction = auction;

            AddPage(0);

            AddBackground(0, 0, 600, 400, 0x9BF5);
            AddHtmlLocalized(50, 10, 500, 18, 1114513, "#1156422", Green, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            AddHtmlLocalized(50, 46, 58, 22, 1078924, Green, false, false); // Name:
            AddHtmlLocalized(118, 46, 117, 22, 1156423, Green, false, false); // Platinum Bid:
            AddHtmlLocalized(245, 46, 117, 22, 1156424, Green, false, false); // Gold Bid:
            AddHtmlLocalized(372, 46, 176, 22, 1156425, Green, false, false); // Bid Time:

            if (Auction == null || Auction.BidHistory == null)
                return;

            int y = 70;

            for (int i = Auction.BidHistory.Count - 1; i >= 0; i--)
            {
                if (i < Auction.BidHistory.Count - 13)
                    break;

                HistoryEntry h = Auction.BidHistory[i];
                long bid = i != Auction.BidHistory.Count - 1 || h.ShowRealBid ? h.Bid : Auction.CurrentBid;

                long plat = bid >= Account.CurrencyThreshold ? bid / Account.CurrencyThreshold : 0;
                long gold = bid >= Account.CurrencyThreshold ? bid - ((bid / Account.CurrencyThreshold) * Account.CurrencyThreshold) : bid;

                AddHtml(50, y, 58, 22, Color(HGray, string.Format("{0}*****", h.Mobile != null ? h.Mobile.Name.Trim()[0].ToString() : "?")), false, false);
                AddHtml(118, y, 117, 22, Color(HGray, plat.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);
                AddHtml(245, y, 117, 22, Color(HGray, gold.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), false, false);
                AddHtml(372, y, 176, 22, Color(HGray, string.Format("{0}-{1}-{2} {3}", h.BidTime.Year, h.BidTime.Month, h.BidTime.Day, h.BidTime.ToShortTimeString())), false, false);

                y += 24;
            }
        }

        public string Color(string color, string str)
        {
            return string.Format("<basefont color=#{0}>{1}", color, str);
        }
    }
}
