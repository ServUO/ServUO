using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Engines.Auction;

namespace Server.AccountVault
{
    public class AuctionVault : Item, IAuctionItem
    {
        private Auction _Auction;

        [CommandProperty(AccessLevel.GameMaster)]
        public Auction Auction
        {
            get { return _Auction ?? (_Auction = new Auction(null, this)); }
            set { _Auction = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index { get; set; }

        public AuctionVault(int index)
            : base(0x9C19)
        {
            Movable = false;

            AddVault(this);
            Index = index;
        }

        public bool CheckAuctionItem(Item item)
        {
            if (_Auction == null || !_Auction.OnGoing || _Auction.AuctionItem == null)
                return false;

            if (_Auction.AuctionItem == item)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnAuctionTray()
        {
            if (_Auction.AuctionItem != null)
            {
                _Auction.AuctionItem.Delete();
                _Auction.AuctionItem = null;
                _Auction.Description = null;
            }
        }

        public void ClaimPrize(Mobile m)
        {
            var item = Auction.AuctionItem;

            if (item != null)
            {
                var name = Auction.AuctionItemName();
                var vaultCont = item as AccountVaultContainer;

                if (vaultCont != null)
                {
                    if (vaultCont.TryClaim(m))
                    {
                        Auction.Reset();
                        m.SendLocalizedMessage(1152339, name); // A reward of ~1_ITEM~ has been placed in your backpack.
                    }
                    else
                    {
                        m.SendLocalizedMessage(1158079); // You cannot currently unpack this vault or claim this item, as doing so would overload you!
                    }
                }
                else if (m.Backpack != null && m.Backpack.TryDropItem(m, item, false))
                {
                    m.SendLocalizedMessage(1152339, name); // A reward of ~1_ITEM~ has been placed in your backpack.
                    item.Movable = true;

                    Auction.Reset();
                }
                else
                {
                    m.SendLocalizedMessage(1158079); // You cannot currently unpack this vault or claim this item, as doing so would overload you!
                }
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1158004, Index.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500332); // I am too far away to do that.
            }
            else
            {
                var pm = from as PlayerMobile;

                if (pm != null && _Auction != null)
                {
                    if (_Auction.InClaimPeriod)
                    {
                        if (_Auction.HighestBid != null && pm == Auction.HighestBid.Mobile)
                        {
                            _Auction.ClaimPrize(pm);
                        }
                        else if (!pm.HasGump(typeof(AuctionBidGump)))
                        {
                            pm.SendGump(new AuctionBidGump(pm, this));
                        }
                    }
                    else if (Auction.OnGoing && !pm.HasGump(typeof(AuctionBidGump)))
                    {
                        pm.SendGump(new AuctionBidGump(pm, this));
                    }
                }
            }
        }

        public override void Delete()
        {
            base.Delete();

            RemoveVault(this);
        }

        public AuctionVault(Serial serial)
           : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Index);

            if (Auction != null)
            {
                writer.Write(1);
                Auction.Serialize(writer);
            }
            else
                writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Index = reader.ReadInt();

            if (reader.ReadInt() == 1)
                Auction = new Auction(this, reader);

            AddVault(this);
        }

        #region static members
        public static List<AuctionVault> Vaults { get; set; } = new List<AuctionVault>();

        public static void Initialize()
        {
            if (Vaults.Count == 0)
            {
                var map = Siege.SiegeShard ? Map.Felucca : Map.Trammel;

                for (int i = 0; i < Locs.Length; i++)
                {
                    var vault = new AuctionVault(i + 1);
                    vault.MoveToWorld(Locs[i], map);
                }
            }
        }

        private static Point3D[] Locs = new[]
        {
            new Point3D(1465, 1720, 0), new Point3D(1465, 1722, 0), new Point3D(1465, 1724, 0), new Point3D(1465, 1726, 0),
            new Point3D(1467, 1720, 0), new Point3D(1467, 1722, 0), new Point3D(1467, 1724, 0), new Point3D(1467, 1726, 0),
            new Point3D(1469, 1720, 0), new Point3D(1469, 1722, 0), new Point3D(1469, 1724, 0), new Point3D(1469, 1726, 0)
        };

        public static void AddVault(AuctionVault vault)
        {
            if (!Vaults.Contains(vault))
            {
                Vaults.Add(vault);
            }
        }

        public static void RemoveVault(AuctionVault vault)
        {
            if (Vaults.Contains(vault))
            {
                Vaults.Remove(vault);
            }
        }

        public static AuctionVault GetFirstAvailable()
        {
            foreach (var vault in Vaults.OrderBy(v => v.Index))
            {
                if (vault.Auction == null || !vault.Auction.OnGoing)
                {
                    return vault;
                }
            }

            return null;
        }
        #endregion
    }
}
