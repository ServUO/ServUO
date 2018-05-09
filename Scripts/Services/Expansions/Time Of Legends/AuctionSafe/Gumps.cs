using Server;
using System;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Gumps;
using Server.Accounting;
using Server.Targeting;
using System.Globalization;
using Server.Network;

namespace Server.Engines.Auction
{
    public class BaseAuctionGump : Gump
    {
        public const int Blue = 0x1E90FF;
        public const int Yellow = 0xFFE0;
        public const int White = 0xFFFF;
        public const int Gray = 0x696969;

        public const int Length = 450;
        public const int Height = 600;

        public AuctionSafe Safe { get; set; }
        public bool Owner { get; set; }
        public PlayerMobile User { get; set; }

        public Auction Auction { get; set; }

        public BaseAuctionGump(PlayerMobile p, AuctionSafe safe)
            : base(50, 100)
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

            User.CloseGump(this.GetType());
            User.SendGump(this, false);
        }

        public int C32216(int value)
        {
            return Server.Engines.Quests.BaseQuestGump.C32216(value);
        }

        public string Color(string color, string str)
        {
            return String.Format("<basefont color=#{0}>{1}", color, str);
        }

        public virtual void AddGumpLayout()
        {
            AddBackground(0, 0, Length, Height, 39925);

            AddHtmlLocalized(0, 20, 400, 16, 1154645, "#1156371", White, false, false); // <center>~1_val~</center> : Auction Safe
            AddHtmlLocalized(0, 50, 400, 16, 1154645, Owner ? "#1150328" : "#1156442", C32216(Blue), false, false); // OWNER MENU : BIDDER MENU

            AddHtmlLocalized(80, 80, 150, 16, 3000098, Yellow, false, false); // Information
            AddButton(40, 80, 4005, 4007, 100, GumpButtonType.Reply, 0);

            AddHtmlLocalized(280, 80, 150, 16, 3010004, Yellow, false, false); // History
            AddButton(240, 80, 4005, 4007, 101, GumpButtonType.Reply, 0);

            Account acct = User.Account as Account;

            AddHtmlLocalized(0, 110, (Length / 2) - 3, 16, 1114514, "#1156044", Yellow, false, false); // Total Gold:
            AddLabel((Length / 2) + 3, 110, 1153, acct != null ? acct.TotalGold.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : "0");

            AddHtmlLocalized(0, 130, (Length / 2) - 3, 16, 1114514, "#1156045", Yellow, false, false); // Total Platinum:
            AddLabel((Length / 2) + 3, 130, 1153, acct != null ? acct.TotalPlat.ToString("N0", CultureInfo.GetCultureInfo("en-US")) : "0");

            if (Auction != null)
                Auction.AddViewer(User);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 100: User.SendGump(new AuctionInfoGump(User)); break;
                case 101: User.SendGump(new BidHistoryGump(User, Auction)); break;
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

        public AuctionOwnerGump(PlayerMobile pm, AuctionSafe safe)
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
                    this.Auction = Safe.Auction;
                else
                    Safe.Auction = this.Auction = new Auction(User, Safe);
            }

            int y = 160;

            // Add Auction Item
            AddHtmlLocalized((Length / 2) + 3, y, 200, 16, 1156421, Yellow, false, false); // Select New Auction Item
            AddButton((Length / 2) - 33, y, 4005, 4007, 1, GumpButtonType.Reply, 0);

            y += 25;

            // Description
            AddHtmlLocalized(0, y, (Length / 2) - 3, 16, 1114514, "#1156400", Yellow, false, false); // Description:
            AddButton(Length - 60, y, 4014, 4016, 2, GumpButtonType.Reply, 0);

            AddBackground((Length / 2) + 3, y, 150, 130, 9350);
            AddTextEntry((Length / 2) + 5, y, 145, 129, 0, 1, Auction.Description);

            y += 25;

            // Display Item
            if (Auction.AuctionItem != null)
            {
                Item i = Auction.AuctionItem;
                AddImage((Length / 2) - 83, y, 2329);
                Rectangle2D b = ItemBounds.Table[i.ItemID];
                AddItem(((Length / 2) - (83 / 2)) - b.Width / 2 - b.X, (y + 60 / 2) - b.Height / 2 - b.Y, i.ItemID, i.Hue);
                AddItemProperty(i.Serial);
            }

            y = 320;

            AddHtmlLocalized(0, y, (Length / 2) - 3, 16, 1114514, "#1156404", Yellow, false, false); // Time Remaining:

            if (Auction.HasBegun)
            {
                TimeSpan left = Auction.EndTime - DateTime.Now;
                int cliloc;
                double v;

                if (left.TotalDays >= 7)
                {
                    cliloc = 1153092; // Lifespan: ~1_val~ weeks
                    v = left.TotalDays / 7;
                }
                else if (left.TotalDays >= 1)
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

                AddHtmlLocalized((Length / 2) + 3, y, 200, 16, cliloc, ((int)v).ToString(), C32216(Gray), false, false);
            }
            else
                AddHtmlLocalized((Length / 2) + 3, y, 200, 16, 1153091, Auction.Duration.ToString(), C32216(Gray), false, false);

            y += 20;

            AddHtmlLocalized(Length / 2, y, (Length / 2) - 65, 16, 1114514, "#1156418", Yellow, false, false); // Three Days;
            AddButton(Length - 60, y, 4014, 4016, 3, GumpButtonType.Reply, 0);

            y += 20;

            AddHtmlLocalized(Length / 2, y, (Length / 2) - 65, 16, 1114514, "#1156419", Yellow, false, false); // Five Days;
            AddButton(Length - 60, y, 4014, 4016, 4, GumpButtonType.Reply, 0);

            y += 20;

            AddHtmlLocalized(Length / 2, y, (Length / 2) - 65, 16, 1114514, "#1156420", Yellow, false, false); // Seven Days;
            AddButton(Length - 60, y, 4014, 4016, 5, GumpButtonType.Reply, 0);

            y += 22;
            // Start Bid Plat/Gold
            AddHtmlLocalized(0, y, (Length / 2) - 3, 16, 1114514, "#1156410", Yellow, false, false); // Item Starting Bid Plat:
            AddBackground((Length / 2) + 3, y, (Length / 2) - 68, 20, 9350);
            AddTextEntry((Length / 2) + 5, y, (Length / 2) - 65, 20, 0, 2, Auction.CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            y += 25;

            AddHtmlLocalized(0, y, (Length / 2) - 3, 16, 1114514, "#1156411", Yellow, false, false); // Item Starting Bid Gold:
            AddBackground((Length / 2) + 3, y, (Length / 2) - 68, 20, 9350);
            AddTextEntry((Length / 2) + 5, y, (Length / 2) - 63, 20, 0, 3, Auction.CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            y += 25;

            AddHtmlLocalized((Length / 2) + 3, y, Length / 2, 16, 1156416, Yellow, false, false); // Set Starting Bids
            AddButton((Length / 2) - 33, y, 4005, 4007, 6, GumpButtonType.Reply, 0);

            y += 25;

            // Buy Now
            AddHtmlLocalized(0, y, (Length / 2) - 3, 16, 1114514, "#1156413", Yellow, false, false); // Buy Now Plat Price:
            AddBackground((Length / 2) + 3, y, (Length / 2) - 68, 20, 9350);
            AddTextEntry((Length / 2) + 5, y, (Length / 2) - 63, 20, 0, 4, Auction.BuyoutPlat.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            y += 25;

            AddHtmlLocalized(0, y, (Length / 2) - 3, 16, 1114514, "#1156412", Yellow, false, false); // Buy Now Gold Price:
            AddBackground((Length / 2) + 3, y, (Length / 2) - 68, 20, 9350);
            AddTextEntry((Length / 2) + 5, y, (Length / 2) - 63, 20, 0, 5, Auction.BuyoutGold.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            y += 25;

            AddHtmlLocalized((Length / 2) + 3, y, Length / 2, 16, 1156417, Yellow, false, false); // Set Buy Now Price
            AddButton((Length / 2) - 33, y, 4005, 4007, 7, GumpButtonType.Reply, 0);

            y += 25;

            if (Auction.AuctionItemOnDisplay() && !Auction.OnGoing)
            {
                AddHtmlLocalized((Length / 2) + 3, y, 200, 16, 1156414, Yellow, false, false); // Start Auction
                AddButton((Length / 2) - 33, y, 4005, 4007, 8, GumpButtonType.Reply, 0);
            }

            if (Auction.OnGoing && Auction.HighestBid == null)
            {
                AddHtmlLocalized((Length / 2) + 3, y, 200, 16, 1114057, "Cancel Auction", Yellow, false, false);
                AddButton((Length / 2) - 33, y, 4005, 4007, 23, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            base.OnResponse(state, info);

            switch (info.ButtonID)
            {
                case 1:
                    if (Auction.CheckModifyAuction(User, true))
                    {
                        User.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
                        {
                            Item item = targeted as Item;

                            if (!IsBadItem(item))
                            {
                                if (item == Auction.AuctionItem)
                                {
                                    User.SendMessage("You have removed the item from the auction.");
                                    Auction.AuctionItem = null;
                                    item.Movable = true;
                                    User.AddToBackpack(item);
                                }
                                else if (item.IsChildOf(User.Backpack))
                                {
                                    if (Auction.AuctionItem != null)
                                    {
                                        Auction.AuctionItem.Movable = true;
                                        User.AddToBackpack(Auction.AuctionItem);
                                        Auction.AuctionItem = null;

                                        User.SendMessage("You swap out auction items.");
                                    }
                                    else
                                        User.SendMessage("You have selected an auction item.");

                                    Auction.AuctionItem = item;
                                    item.Movable = false;
                                    item.MoveToWorld(new Point3D(Safe.X, Safe.Y, Safe.Z + Safe.Components[0].ItemData.CalcHeight), Safe.Map);
                                }
                            }
                            else
                                User.SendMessage("You cannot add that to your auction.");
                            Refresh();
                        });
                    }
                    break;
                case 2:
                    TextRelay relay = info.GetTextEntry(1);
                    string str = null;

                    if (relay != null)
                        str = relay.Text;

                    if (str == null || str.Trim().Length > 150 || !Server.Guilds.BaseGuildGump.CheckProfanity(str))
                    {
                        User.SendMessage(0x22, "Invalid description.");
                    }
                    else
                    {
                        Auction.Description = Utility.FixHtml(str.Trim());
                    }
                    Refresh();
                    break;
                case 3:
                    if (Auction.CheckModifyAuction(User))
                        Auction.Duration = 3;
                    Refresh();
                    break;
                case 4:
                    if (Auction.CheckModifyAuction(User))
                        Auction.Duration = 5;
                    Refresh();
                    break;
                case 5:
                    if (Auction.CheckModifyAuction(User))
                        Auction.Duration = 7;
                    Refresh();
                    break;
                case 6:
                    TextRelay relay1 = info.GetTextEntry(2);

                    string plat1 = null;
                    string gold1 =  null;

                    if (relay1 != null)
                        plat1 = relay1.Text;

                    relay1 = info.GetTextEntry(3);

                    if (relay1 != null)
                        gold1 = relay1.Text;

                    long platAmnt = Utility.ToInt64(plat1);
                    long goldAmnt = Utility.ToInt64(gold1);

                    if (Auction.CheckModifyAuction(User) && platAmnt >= 0 && goldAmnt >= 0)
                    {
                        if (platAmnt > UInt32.MaxValue)
                        {
                            User.SendMessage("Who in Sosaria has that much gold? Try a lower amount.");
                            break;
                        }

                        _TempBid += platAmnt * Account.CurrencyThreshold;

                        if (goldAmnt > UInt32.MaxValue)
                        {
                            User.SendMessage("That bid is too high. Try converting some of your gold bid into a plat bid.");
                            return;
                        }

                        _TempBid += goldAmnt;
                    }
                    else
                        _NoBid = true;

                    if (!_NoBid)
                        Auction.CurrentBid = _TempBid;

                    Refresh();
                    break;
                case 7:
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

                    if (Auction.CheckModifyAuction(User) && platAmnt2 >= 0 && goldAmnt2 >= 0)
                    {
                        if (platAmnt2 > UInt32.MaxValue)
                        {
                            User.SendMessage("Who in Sosaria has that much gold? Try a lower amount.");
                            return;
                        }

                        _TempBuyout += platAmnt2 * Account.CurrencyThreshold;

                        if (goldAmnt2 > UInt32.MaxValue)
                        {
                            User.SendMessage("That bid is too high. Try converting some of your gold bid into a plat bid.");
                            return;
                        }

                        _TempBuyout += goldAmnt2;
                    }

                    Auction.Buyout = _TempBuyout;

                    Refresh();
                    break;
                case 8:
                    if (Auction.CurrentBid <= 0)
                        User.SendLocalizedMessage(1156434); // You must set a starting bid.
                    else
                    {
                        Auction.OnBegin();
                    }

                    Refresh();
                    break;
                case 23:
                    if (Auction.OnGoing && Auction.HighestBid == null)
                    {
                        Auction.Cancel();
                    }
                    break;
            }

        }

        private bool IsBadItem(Item item)
        {
            return item == null || item.Weight > 300 || (item is Container && !(item is BaseQuiver)) || item is Gold || item is BankCheck || !item.Movable || item.Items.Count > 0;
        }
    }

    public class AuctionBidGump : BaseAuctionGump
    {
        public long TempBid { get; set; }

        public AuctionBidGump(PlayerMobile pm, AuctionSafe safe)
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
                AddImage((Length / 2) + 3, 160, 2329);
                Rectangle2D b = ItemBounds.Table[i.ItemID];
                AddItem(((Length / 2) + (50 / 2)) + b.Width / 2 - b.X, (160 + 60 / 2) - b.Height / 2 - b.Y, i.ItemID, i.Hue);
                AddItemProperty(i.Serial);
            }

            // Description
            AddHtmlLocalized(0, 230, (Length / 2) - 3, 16, 1114514, "#1156400", Yellow, false, false); // Description:
            AddHtml((Length / 2) + 5, 230, 145, 130, Auction.Description, true, false);

            // Time
            AddHtmlLocalized(0, 370, (Length / 2) - 3, 16, 1114514, "#1156404", Yellow, false, false); // Time Remaining:

            TimeSpan left = Auction.EndTime - DateTime.Now;
            int cliloc;
            double v;

            if (left.TotalDays >= 7)
            {
                cliloc = 1153092; // Lifespan: ~1_val~ weeks
                v = left.TotalDays / 7;
            }
            else if (left.TotalDays >= 1)
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

            BidEntry entry = Auction.GetBidEntry(User, false);

            int platBid = entry == null ? 0 : entry.TotalPlatBid;
            int goldBid = entry == null ? 0 : entry.TotalGoldBid;

            AddHtmlLocalized((Length / 2) + 3, 370, 200, 16, cliloc, ((int)v).ToString(), C32216(Gray), false, false);

            AddHtmlLocalized(0, 395, (Length / 2) - 3, 16, 1114514, "#1156436", Yellow, false, false); // Current Platinum Bid:
            AddLabel((Length / 2) + 3, 395, 1149, Auction.CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            AddHtmlLocalized(0, 420, (Length / 2) - 3, 16, 1114514, "#1156435", Yellow, false, false); // Current Gold Bid:
            AddLabel((Length / 2) + 3, 420, 1149, Auction.CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            AddHtmlLocalized(0, 445, (Length / 2) - 3, 16, 1114514, "#1156406", Yellow, false, false); // Your Current Platinum Bid:
            AddBackground((Length / 2) + 3, 445, 200, 20, 9350);
            AddTextEntry((Length / 2) + 5, 445, 198, 20, 0, 1, platBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            AddHtmlLocalized(0, 470, (Length / 2) - 3, 16, 1114514, "#1156405", Yellow, false, false); // Your Current Gold Bid:
            AddBackground((Length / 2) + 3, 470, 200, 20, 9350);
            AddTextEntry((Length / 2) + 5, 470, 198, 20, 0, 2, goldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            AddHtmlLocalized((Length / 2) + 3, 495, 200, 16, 1156407, Yellow, false, false); // Place Bid
            AddButton((Length / 2) - 33, 495, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(0, 520, (Length / 2) - 3, 16, 1114514, "#1156413", Yellow, false, false); // Buy Now Plat Price:
            AddLabel((Length / 2) + 3, 520, 1149, Auction.BuyoutPlat.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            AddHtmlLocalized(0, 545, (Length / 2) - 3, 16, 1114514, "#1156412", Yellow, false, false); // Buy Now Gold Price:
            AddLabel((Length / 2) + 3, 545, 1149, Auction.BuyoutGold.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

            if (Auction.Buyout > 0)
            {
                AddHtmlLocalized((Length / 2) + 3, 570, 200, 16, 1156409, Yellow, false, false); // Buy Now
                AddButton((Length / 2) - 33, 570, 4005, 4007, 2, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            base.OnResponse(state, info);

            switch (info.ButtonID)
            {
                case 1:
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
                case 2:
                    Auction.TryBuyout(User);
                    Refresh();
                    Auction.ResendGumps(User);
                    break;
            }
        }
    }

    public class AuctionInfoGump : Gump
    {
        public AuctionInfoGump(PlayerMobile pm)
            : base(50, 100)
        {
            AddBackground(0, 0, 700, 500, 39925);
            AddHtmlLocalized(30, 30, 640, 440, 1156441, 0xFFFF, false, true);

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
        }
    }

    public class BidHistoryGump : Gump
    {
        private readonly int Green = 0x228B22;

        public Auction Auction { get; set; }

        public BidHistoryGump(PlayerMobile pm, Auction auction)
            : base(50, 100)
        {
            Auction = auction;

            AddBackground(0, 0, 700, 500, 39925);
            AddHtmlLocalized(0, 20, 700, 16, 1154645, "#1156422", Server.Engines.Quests.BaseQuestGump.C32216(Green), false, false); // History

            AddHtmlLocalized(30, 60, 120, 16, 3000075, Server.Engines.Quests.BaseQuestGump.C32216(Green), false, false); // Name
            AddHtmlLocalized(150, 60, 150, 16, 1156423, Server.Engines.Quests.BaseQuestGump.C32216(Green), false, false); // Platinum Bid
            AddHtmlLocalized(300, 60, 150, 16, 1156424, Server.Engines.Quests.BaseQuestGump.C32216(Green), false, false); // Gold Bid
            AddHtmlLocalized(450, 60, 200, 16, 1156425, Server.Engines.Quests.BaseQuestGump.C32216(Green), false, false); // Bid Time

            if (Auction == null || Auction.BidHistory == null)
                return;

            int y = 80;

            for (int i = Auction.BidHistory.Count - 1; i >= 0; i--)
            {
                if (i < Auction.BidHistory.Count - 20)
                    break;

                HistoryEntry h = Auction.BidHistory[i];
                long bid = i != Auction.BidHistory.Count - 1 || h.ShowRealBid ? h.Bid : Auction.CurrentBid;

                long plat = bid >= Account.CurrencyThreshold ? bid / Account.CurrencyThreshold : 0;
                long gold = bid >= Account.CurrencyThreshold ? bid - ((bid / Account.CurrencyThreshold) * Account.CurrencyThreshold) : bid;

                AddLabel(30, y, 1149, String.Format("{0}*****", h.Mobile != null ? h.Mobile.Name.Trim()[0].ToString() : "?"));
                AddLabel(150, y, 1149, plat.ToString("N0", CultureInfo.GetCultureInfo("en-US")));
                AddLabel(300, y, 1149, gold.ToString("N0", CultureInfo.GetCultureInfo("en-US")));
                AddLabel(450, y, 1149, String.Format("{0}-{1}-{2} {3}", h.BidTime.Year, h.BidTime.Month, h.BidTime.Day, h.BidTime.ToShortTimeString()));

                y += 20;
            }
        }

        public string Color(string color, string str)
        {
            return String.Format("<basefont color=#{0}>{1}", color, str);
        }
    }
}