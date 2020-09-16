using Server.Accounting;
using Server.Engines.NewMagincia;
using Server.Items;
using Server.Mobiles;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Server.Engines.Auction
{
    public interface IAuctionItem : IEntity
    {
        Auction Auction { get; set; }
        bool CheckAuctionItem(Item item);
        void OnAuctionTray();
        void ClaimPrize(Mobile m);
    }

    [PropertyObject]
    public class Auction : IDisposable
    {
        public const int DefaultDuration = 10080;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public IAuctionItem Safe { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item AuctionItem { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public long CurrentBid { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public long StartBid { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public long Buyout { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Duration { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime StartTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime EndTime => StartTime + TimeSpan.FromMinutes(Duration);

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ClaimPeriod { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OnGoing { get; set; }

        public List<BidEntry> Bids;

        [CommandProperty(AccessLevel.GameMaster)]
        public BidEntry HighestBid { get; set; }

        public bool HasBegun => StartTime != DateTime.MinValue;
        public bool InClaimPeriod => HasBegun && ClaimPeriod != DateTime.MinValue;
        public bool CanModify => !HasBegun;

        public int CurrentGoldBid => (int)(CurrentBid >= Account.CurrencyThreshold ? CurrentBid - (CurrentPlatBid * Account.CurrencyThreshold) : CurrentBid);
        public int CurrentPlatBid => (int)(CurrentBid >= Account.CurrencyThreshold ? CurrentBid / Account.CurrencyThreshold : 0);

        public int BuyoutGold => (int)(Buyout >= Account.CurrencyThreshold ? Buyout - (BuyoutPlat * Account.CurrencyThreshold) : Buyout);
        public int BuyoutPlat => (int)(Buyout >= Account.CurrencyThreshold ? Buyout / Account.CurrencyThreshold : 0);

        public List<HistoryEntry> BidHistory { get; set; }
        public List<PlayerMobile> Viewers { get; set; }

        public bool PublicAuction => Owner == null;

        public Auction(Mobile owner, IAuctionItem item)
        {
            Safe = item;
            Owner = owner;
            Bids = new List<BidEntry>();

            Duration = DefaultDuration;
        }

        ~Auction()
        {
            Dispose();
        }

        public override string ToString()
        {
            return "...";
        }

        public bool AuctionItemOnDisplay()
        {
            if (AuctionItem == null || AuctionItem.Deleted || Safe == null)
                return false;

            return !AuctionItem.Movable && AuctionItem.X == Safe.X && AuctionItem.Y == Safe.Y && AuctionItem.Z == Safe.Z + 7;
        }

        public void OnBegin()
        {
            if (Safe == null || Safe.Deleted)
                return;

            if (!OnGoing)
            {
                BidHistory = null;
            }

            StartTime = DateTime.Now;
            OnGoing = true;

            CurrentBid = StartBid;

            Auctions.Add(this);
        }

        public bool CheckModifyAuction(Mobile m, bool checkingItem = false)
        {
            if (Safe == null)
                return false;

            if (AuctionItem != null && InClaimPeriod)
            {
                if (!checkingItem)
                    m.SendLocalizedMessage(1156430); // You must wait for your auctioned item to be claimed before modifying this auction safe.
                else
                    m.SendLocalizedMessage(1156429); // You must wait for your auctioned item to be claimed before adding a new item.

                return false;
            }

            if (!CanModify)
            {
                m.SendLocalizedMessage(1156431); // You cannot modify this while an auction is in progress.
                return false;
            }

            return true;
        }

        public bool TryPlaceBid(Mobile m, long bidTotal)
        {
            if (!OnGoing || InClaimPeriod)
            {
                m.SendLocalizedMessage(1156432); // There is no active auction to complete this action.
                return false;
            }

            BidEntry entry = GetBidEntry(m);
            Account acct = m.Account as Account;
            bool firstBid = HighestBid == null;

            long highestBid = firstBid ? CurrentBid : HighestBid.CurrentBid;

            if (acct == null || Banker.GetBalance(m) < bidTotal)
            {
                m.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                return false;
            }

            if ((firstBid && bidTotal < highestBid) || (!firstBid && bidTotal <= highestBid))
            {
                m.SendLocalizedMessage(1156445); // You have been out bid.

                if (bidTotal > CurrentBid)
                {
                    CurrentBid = bidTotal;
                    AddToHistory(m, bidTotal);
                }
            }
            else
            {
                acct.WithdrawGold(bidTotal);
                entry.CurrentBid = bidTotal;
                var mobile = HighestBid != null ? HighestBid.Mobile : null;

                if (!firstBid)
                {
                    if (mobile != m)
                    {
                        DoOutBidMessage(mobile);
                    }

                    HighestBid.Refund(this, highestBid);
                }
                else
                {
                    AddToHistory(m, bidTotal);
                }

                m.SendLocalizedMessage(1156433); // Your bid has been placed.               

                AuctionMap map = new AuctionMap(Safe);

                if (m.Backpack == null || !m.Backpack.TryDropItem(m, map, false))
                {
                    map.Delete();
                }
                else
                {
                    m.SendLocalizedMessage(1156478); // The auction safe map has been placed in your backpack.
                }

                VaultLogging.NewHighBid(this, m, mobile, bidTotal);

                HighestBid = entry;
                return true;
            }

            return false;
        }

        public bool TryBuyout(Mobile m)
        {
            if (!OnGoing || InClaimPeriod || Buyout <= 0)
                return false;

            Account acct = m.Account as Account;

            if (acct != null)
            {
                if (!acct.WithdrawGold(Buyout))
                {
                    m.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                    return false;
                }
                else
                {
                    VaultLogging.Buyout(this, m, Buyout);

                    if (HighestBid != null && HighestBid.Mobile != m)
                    {
                        DoOutBidMessage(HighestBid.Mobile);

                        HighestBid.Refund(this, HighestBid.CurrentBid);
                    }

                    HighestBid = GetBidEntry(m, true);
                    HighestBid.CurrentBid = Buyout - (int)(Buyout * .05);
                    CurrentBid = Buyout;

                    EndAuction(true);
                    ClaimPrize(m);
                    return true;
                }
            }

            return false;
        }

        private void DoOutBidMessage(Mobile to)
        {
            string name = AuctionItemName();

            if (string.IsNullOrEmpty(name))
            {
                name = "the item you bid on";
            }

            NewMaginciaMessage message = new NewMaginciaMessage(null, 1156427, string.Format("{0}\t{1}\t{2}",
                                                    name,
                                                    CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                    CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
            /*  You have been out bid in an auction for ~1_ITEMNAME~. The current winning bid amount is 
             * ~2_BIDAMT~plat and ~3_BIDAMT~gp.*/
            MaginciaLottoSystem.SendMessageTo(to, message);
        }

        private void AddToHistory(Mobile m, long highBid)
        {
            if (BidHistory == null)
                BidHistory = new List<HistoryEntry>();

            BidHistory.Add(new HistoryEntry(m, highBid));
        }

        public BidEntry GetBidEntry(Mobile m, bool create = true)
        {
            BidEntry entry = Bids.FirstOrDefault(e => m == e.Mobile);

            if (entry == null && create && m is PlayerMobile)
            {
                entry = new BidEntry((PlayerMobile)m);
                Bids.Add(entry);
            }

            return entry;
        }

        public void EndAuction()
        {
            EndAuction(false);
        }

        public void EndAuction(bool buyout)
        {
            if (HighestBid != null && HighestBid.Mobile != null)
            {
                var isPublic = PublicAuction;
                var m = HighestBid.Mobile;
                string name = AuctionItemName();

                if (string.IsNullOrEmpty(name))
                {
                    name = "the item you bid on";
                }

                NewMaginciaMessage message = new NewMaginciaMessage(null, isPublic ? 1158078 : 1156426, TimeSpan.FromHours(72), string.Format("{0}\t{1}\t{2}",
                                                        name,
                                                        CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                        CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                /*  You have won an auction for ~1_ITEMNAME~! Your bid amount of ~2_BIDAMT~plat and ~3_BIDAMT~gp won the auction. 
                 *  You have 3 days from the end of the auction to claim your item or it will be lost.*/

                MaginciaLottoSystem.SendMessageTo(HighestBid.Mobile, message);

                if (!buyout)
                {
                    VaultLogging.WinAuction(this, m, CurrentBid, HighestBid.CurrentBid);
                }

                Account a = m.Account as Account;
                Account b = isPublic ? null : Owner.Account as Account;
                long dif = HighestBid.CurrentBid - CurrentBid;

                if (a != null && dif > 0)
                    a.DepositGold(dif);

                if (b != null)
                    b.DepositGold(HighestBid.CurrentBid);

                if (!isPublic)
                {
                    message = new NewMaginciaMessage(null, new TextDefinition(1156428), TimeSpan.FromHours(24), string.Format("{0}\t{1}\t{2}",
                                                            name,
                                                            CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                            CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                    /*Your auction for ~1_ITEMNAME~ has ended with a winning bid of ~2_BIDAMT~plat and ~3_BIDAMT~gp. The winning bid has 
                     *been deposited into your currency account.*/
                    MaginciaLottoSystem.SendMessageTo(Owner, message);
                }

                ClaimPeriod = DateTime.UtcNow + TimeSpan.FromDays(3);
            }
            else
            {
                TrayAuction();
            }

            CloseGumps();
        }

        private void CloseGumps()
        {
            if (Viewers != null)
                Viewers.ForEach(pm =>
                {
                    pm.CloseGump(typeof(AuctionBidGump));
                    pm.CloseGump(typeof(AuctionOwnerGump));
                });
        }

        public void HouseCollapse()
        {
            Item item = AuctionItem;

            if (Bids != null)
            {
                Bids.ForEach(bid =>
                {
                    string name = AuctionItemName();

                    if (string.IsNullOrEmpty(name))
                    {
                        name = "the item you bid on";
                    }

                    NewMaginciaMessage mes = new NewMaginciaMessage(null, new TextDefinition(1156454), string.Format("{0}\t{1}\t{2}",
                                                                CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                                CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                                name));
                    /*Your winning bid amount of ~1_BIDAMT~plat and ~2_BIDAMT~gp for ~3_ITEMNAME~ has been refunded to you due to house collapse.*/
                    MaginciaLottoSystem.SendMessageTo(bid.Mobile, mes);

                    Account a = bid.Mobile.Account as Account;

                    if (a != null)
                        a.DepositGold(bid.CurrentBid);
                });

                RemoveAuction();
            }

            if (item != null)
            {
                item.Movable = true;

                if (Owner != null)
                {
                    Owner.BankBox.DropItem(item);
                }
            }
        }

        public void ClaimPrize(Mobile m)
        {
            if (Safe != null)
            {
                Safe.ClaimPrize(m);
            }
        }

        public void EndClaimPeriod()
        {
            if (AuctionItem != null)
            {
                TrayAuction();
            }
        }

        public string AuctionItemName()
        {
            if (AuctionItem == null)
            {
                return string.Empty;
            }

            if (AuctionItem.LabelNumber != 0)
            {
                return string.Format("#{0}", AuctionItem.LabelNumber.ToString());
            }

            return AuctionItem.Name;
        }

        public void TrayAuction()
        {
            OnGoing = false;

            ClaimPeriod = DateTime.MinValue;
            StartTime = DateTime.MinValue;

            Bids = new List<BidEntry>();
            HighestBid = null;
            CurrentBid = 0;
            StartBid = 0;
            Buyout = 0;

            Duration = DefaultDuration;

            Safe.OnAuctionTray();
        }

        public void Reset()
        {
            RemoveAuction();

            Safe.Auction = new Auction(Owner, Safe);
        }

        public void RemoveAuction()
        {
            Auctions.Remove(this);
            AuctionItem = null;
            Safe.Auction = null;
            OnGoing = false;

            ClaimPeriod = DateTime.MinValue;
        }

        public void Dispose()
        {
            try
            {
                if (Bids != null)
                    ColUtility.Free(Bids);

                if (BidHistory != null)
                    ColUtility.Free(BidHistory);

                if (Viewers != null)
                    ColUtility.Free(Viewers);

                Bids = null;
                BidHistory = null;
                Viewers = null;
            }
            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
        }

        public void ResendGumps(PlayerMobile player)
        {
            if (Viewers == null)
                return;

            ColUtility.ForEach(Viewers.Where(pm => pm != player), pm =>
                {
                    AuctionBidGump g = pm.FindGump(typeof(AuctionBidGump)) as AuctionBidGump;

                    if (g == null)
                        pm.SendGump(new AuctionOwnerGump(pm, Safe));
                    else
                        g.Refresh();
                });
        }

        public void AddViewer(PlayerMobile pm)
        {
            if (Viewers == null)
                Viewers = new List<PlayerMobile>();

            if (!Viewers.Contains(pm))
                Viewers.Add(pm);
        }

        public void RemoveViewer(PlayerMobile pm)
        {
            if (Viewers == null)
                return;

            if (Viewers.Contains(pm))
                Viewers.Remove(pm);

            if (Viewers.Count == 0)
            {
                ColUtility.Free(Viewers);
                Viewers = null;
            }
        }

        public Auction(IAuctionItem safe, GenericReader reader)
        {
            Safe = safe;

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    ClaimPeriod = reader.ReadDateTime();
                    goto case 1;
                case 1:
                    Owner = reader.ReadMobile();
                    AuctionItem = reader.ReadItem();
                    CurrentBid = reader.ReadLong();
                    StartBid = reader.ReadLong();
                    Buyout = reader.ReadLong();
                    Description = reader.ReadString();
                    Duration = reader.ReadInt();

                    StartTime = reader.ReadDateTime();
                    OnGoing = reader.ReadBool();

                    Bids = new List<BidEntry>();

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        PlayerMobile m = reader.ReadMobile() as PlayerMobile;
                        BidEntry entry = new BidEntry(m, reader);

                        if (m != null)
                        {
                            Bids.Add(entry);

                            if (entry.CurrentBid > 0 && (HighestBid == null || entry.CurrentBid > HighestBid.CurrentBid))
                                HighestBid = entry;
                        }
                    }

                    count = reader.ReadInt();

                    if (count > 0)
                        BidHistory = new List<HistoryEntry>();

                    for (int i = 0; i < count; i++)
                    {
                        BidHistory.Add(new HistoryEntry(reader));
                    }

                    break;
            }

            if (HasBegun)
                Auctions.Add(this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(2);

            writer.Write(ClaimPeriod);

            writer.Write(Owner);
            writer.Write(AuctionItem);
            writer.Write(CurrentBid);
            writer.Write(StartBid);
            writer.Write(Buyout);
            writer.Write(Description);
            writer.Write(Duration);
            writer.Write(StartTime);
            writer.Write(OnGoing);

            writer.Write(Bids.Count);
            Bids.ForEach(b =>
            {
                writer.Write(b.Mobile);
                b.Serialize(writer);
            });

            writer.Write(BidHistory != null ? BidHistory.Count : 0);

            if (BidHistory != null)
                BidHistory.ForEach(e => e.Serialize(writer));
        }

        #region static memebers
        public static List<Auction> Auctions { get; set; }

        public static void Configure()
        {
            Auctions = new List<Auction>();
        }

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), () =>
            {
                Auctions.ForEach(a =>
                {
                    if (a.OnGoing && a.EndTime < DateTime.Now && !a.InClaimPeriod)
                    {
                        a.EndAuction();
                    }
                    else if (a.InClaimPeriod && DateTime.UtcNow > a.ClaimPeriod)
                    {
                        a.EndClaimPeriod();
                    }

                });
            });
        }
        #endregion
    }
}
