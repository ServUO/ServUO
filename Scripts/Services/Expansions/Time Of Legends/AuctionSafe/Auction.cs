using Server;
using System;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using System.Linq;
using Server.Accounting;
using Server.Engines.NewMagincia;
using System.Globalization;
using Server.Gumps;

namespace Server.Engines.Auction
{
    [PropertyObject]
    public class Auction : IDisposable
    {
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
                        a.EndAuction();
                    else if (a.InClaimPeriod && DateTime.UtcNow > a.ClaimPeriod)
                        a.EndClaimPeriod();

                });
            });
        }

        public const int DefaultDuration = 3;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AuctionSafe Safe { get; set; }

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
        public DateTime EndTime { get { return StartTime + TimeSpan.FromDays(Duration); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ClaimPeriod { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OnGoing { get; set; }

        public List<BidEntry> Bids;

        public BidEntry HighestBid { get; set; }

        public bool HasBegun { get { return StartTime != DateTime.MinValue; } }
        public bool InClaimPeriod { get { return HasBegun && ClaimPeriod != DateTime.MinValue; } }
        public bool CanModify { get { return !HasBegun; } }

        public int CurrentGoldBid { get { return (int)(CurrentBid >= Account.CurrencyThreshold ? CurrentBid - (CurrentPlatBid * Account.CurrencyThreshold) : CurrentBid); } }
        public int CurrentPlatBid { get { return (int)(CurrentBid >= Account.CurrencyThreshold ? CurrentBid / Account.CurrencyThreshold : 0); } }

        public int BuyoutGold { get { return (int)(Buyout >= Account.CurrencyThreshold ? Buyout - (BuyoutPlat * Account.CurrencyThreshold) : Buyout); } }
        public int BuyoutPlat { get { return (int)(Buyout >= Account.CurrencyThreshold ? Buyout / Account.CurrencyThreshold : 0); } }

        public List<HistoryEntry> BidHistory { get; set; }
        public List<PlayerMobile> Viewers { get; set; }
        public static List<Auction> Auctions { get; set; }

        public Auction(Mobile owner, AuctionSafe safe)
        {
            Safe = safe;
            Owner = owner;
            Bids = new List<BidEntry>();

            Duration = DefaultDuration;
        }

        ~Auction()
        {
            Dispose();
        }

        public bool AuctionItemOnDisplay()
		{
			if(AuctionItem == null || AuctionItem.Deleted || Safe == null)
				return false;

            return !AuctionItem.Movable && AuctionItem.X == Safe.X && AuctionItem.Y == Safe.Y && AuctionItem.Z == Safe.Z + Safe.Components[0].ItemData.CalcHeight;
		}

        public void OnBegin()
        {
            if (Safe == null || Safe.Deleted)
                return;

            StartTime = DateTime.Now;
            OnGoing = true;

            Safe.Components[0].InvalidateProperties();

            Owner.SendMessage("The auction begins!");
            StartBid = CurrentBid;

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
			if(!OnGoing)
				return false;

			BidEntry entry = GetBidEntry(m);
			Account acct = m.Account as Account;
			
			if(bidTotal < entry.CurrentBid || entry == HighestBid)
			{
                m.SendMessage("You are already own the leading bid!");
				return false;
			}

			long actualBid =  bidTotal - entry.CurrentBid;

            if (acct == null || Banker.GetBalance(m) < bidTotal)
			{
				m.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
				return false;
			}
			
			long highestBid = HighestBid != null ? HighestBid.CurrentBid : CurrentBid;
			
			if(bidTotal <= highestBid)
			{
				//if(BidHistory != null && BidHistory.Count > 0)
				//	BidHistory[BidHistory.Count - 1].ShowRealBid = true;
				
				m.SendLocalizedMessage(1156445); // You have been out bid.
			}
			else
			{
                acct.WithdrawGold(bidTotal);
                entry.CurrentBid = bidTotal;

                CurrentBid = highestBid + 1;
				
				if(HighestBid != null)
				{
					string name = "Unknown Item";
					
					if(AuctionItem.Name != null)
						name = AuctionItem.Name;
					else
						name = String.Format("#{0}", AuctionItem.LabelNumber.ToString());
					
					var message = new NewMaginciaMessage(null, new TextDefinition(1156427), String.Format("{0}\t{1}\t{2}", 
															name, 
															CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")), 
															CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
					/*  You have been out bid in an auction for ~1_ITEMNAME~. The current winning bid amount is 
					 * ~2_BIDAMT~plat and ~3_BIDAMT~gp.*/
					MaginciaLottoSystem.SendMessageTo(HighestBid.Mobile, message);

                    Account a = HighestBid.Mobile.Account as Account;

                    if(a != null)
					    a.DepositGold(HighestBid.CurrentBid);

					HighestBid.CurrentBid = 0;
				}
				
				m.SendLocalizedMessage(1156433); // Your bid has been placed.
				HighestBid = entry;
				AddToHistory(m, entry.CurrentBid);
                return true;
			}

            return false;
		}

        public bool TryBuyout(Mobile m)
        {
            if (!OnGoing || Buyout <= 0)
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
                    HighestBid = GetBidEntry(m, true);
                    HighestBid.CurrentBid = Buyout - (int)((double)Buyout * .05);
                    CurrentBid = Buyout;

                    EndAuction();
                    return true;
                }
            }

            return false;
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
            if (HighestBid != null && HighestBid.Mobile != null)
            {
                Mobile m = HighestBid.Mobile;
                string name = "Unknown Item";

                if (AuctionItem.Name != null)
                    name = AuctionItem.Name;
                else
                    name = String.Format("#{0}", AuctionItem.LabelNumber.ToString());

                NewMaginciaMessage message = new NewMaginciaMessage(null, new TextDefinition(1156426), String.Format("{0}\t{1}\t{2}",
                                                        name,
                                                        CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                        CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                /*  You have won an auction for ~1_ITEMNAME~! Your bid amount of ~2_BIDAMT~plat and ~3_BIDAMT~gp won the auction. 
                 *  You have 3 days from the end of the auction to claim your item or it will be lost.*/
                MaginciaLottoSystem.SendMessageTo(HighestBid.Mobile, message);

                Account a = m.Account as Account;
                Account b = Owner.Account as Account;
                long dif = HighestBid.CurrentBid - CurrentBid;

                if (a != null && dif > 0)
                    a.DepositGold(dif);

                if (b != null)
                    b.DepositGold(HighestBid.CurrentBid);

                message = new NewMaginciaMessage(null, new TextDefinition(1156428), String.Format("{0}\t{1}\t{2}",
                                                        name,
                                                        CurrentPlatBid.ToString("N0", CultureInfo.GetCultureInfo("en-US")),
                                                        CurrentGoldBid.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                /*Your auction for ~1_ITEMNAME~ has ended with a winning bid of ~2_BIDAMT~plat and ~3_BIDAMT~gp. The winning bid has 
                 *been deposited into your currency account.*/
                MaginciaLottoSystem.SendMessageTo(Owner, message);

                ClaimPeriod = DateTime.UtcNow + TimeSpan.FromDays(3);

                if (Safe != null && Safe.Components.Count > 0)
                    Safe.Components[0].InvalidateProperties();
            }
            else
            {
                Cancel();
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

        public void ClaimPrize(Mobile m)
        {
            if (AuctionItem != null)
            {
                AuctionItem.Movable = true;

                if (m.Backpack == null || !m.Backpack.TryDropItem(m, AuctionItem, false))
                {
                    m.BankBox.DropItem(AuctionItem);
                    m.SendMessage("Your winning item has been placed in your bank box.");
                }
                else
                    m.SendMessage("Your winning item has been placed in your backpack.");

                AuctionItem = null;
            }

            RemoveAuction();
        }

        public void EndClaimPeriod()
        {
            if (AuctionItem != null)
            {
                if (Owner == null)
                    AuctionItem.Delete();
                else
                {
                    AuctionItem.Movable = true;
                    Owner.BankBox.AddItem(AuctionItem);
                }
            }

            RemoveAuction();
        }

        public void RemoveAuction()
        {
            Auctions.Remove(this);
            Safe.Auction = null;
            OnGoing = false;

            if (AuctionItemOnDisplay() && Owner != null)
            {
                AuctionItem.Movable = true;
                Owner.BankBox.DropItem(AuctionItem);
            }

            AuctionItem = null;
            ClaimPeriod = DateTime.MinValue;
        }

        public void Cancel()
        {
            if (Bids != null)
            {
                Bids.ForEach(bid =>
                {
                    string name = "Unknown Item";

                    if (AuctionItem.Name != null)
                        name = AuctionItem.Name;
                    else
                        name = String.Format("#{0}", AuctionItem.LabelNumber.ToString());

                    var mes = new NewMaginciaMessage(null, new TextDefinition(1156454), String.Format("{0}\t{1}\t{2}",
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

            if (AuctionItem != null)
            {
                AuctionItem.Movable = true;

                if (Owner != null)
                {
                    Owner.BankBox.DropItem(AuctionItem);
                }
            }
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
            catch { }
        }

        public void ResendGumps(PlayerMobile player)
        {
            if (Viewers == null)
                return;

            ColUtility.ForEach(Viewers.Where(pm => pm != player), pm =>
                {
                    AuctionBidGump g = pm.FindGump(typeof(AuctionBidGump)) as AuctionBidGump;

                    if(g == null)
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

        public Auction(AuctionSafe safe, GenericReader reader)
        {
            Safe = safe;

            int version = reader.ReadInt();

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

            if (HasBegun)
                Auctions.Add(this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

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
    }
}