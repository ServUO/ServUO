using Server;
using System;
using Server.Mobiles;
using Server.Items;
using Server.Multis;
using Server.Gumps;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Engines.VeteranRewards;

namespace Server.Engines.Auction
{
    public class AuctionSafe : BaseAddon, ISecurable
    {
        private Auction _Auction;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Auction Auction
        {
            get { return _Auction; }
            set
            {
                if (value == null && _Auction != null)
                {
                    Item item = Auction.AuctionItem;

                    if (item != null && _Auction.Owner != null)
                    {
                        item.Movable = true;
                        _Auction.Owner.BankBox.DropItem(item);
                    }
                }

                _Auction = value;
            }
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

        public override BaseAddonDeed Deed { get { return new AuctionSafeDeed(); } }

        public AuctionSafe(Mobile from, bool south)
        {
            AddComponent(new InternalComponent(), 0, 0, 0);

            Auction = new Auction(from, this);

            Level = SecureLevel.Anyone;
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (!from.InRange(component.Location, 3))
            {
                from.SendLocalizedMessage(500332); // I am too far away to do that.
            }
            else if (house != null && from is PlayerMobile)
            {
                if (house.IsOwner(from))
                {
                    if (!from.HasGump(typeof(AuctionBidGump)))
                    {
                        from.SendGump(new AuctionOwnerGump((PlayerMobile)from, this));
                    }
                }
                else if (Auction != null)
                {
                    if (house.HasSecureAccess(from, Level))
                    {
                        if (Auction.InClaimPeriod)
                        {
                            if (Auction.HighestBid != null && from == Auction.HighestBid.Mobile)
                            {
                                Auction.ClaimPrize(from);
                            }
                            else
                            {
                                if (!from.HasGump(typeof(AuctionBidGump)))
                                {
                                    from.SendGump(new AuctionBidGump((PlayerMobile)from, this));
                                }
                            }
                        }
                        else
                        {
                            if (!from.HasGump(typeof(AuctionBidGump)))
                            {
                                from.SendGump(new AuctionBidGump((PlayerMobile)from, this));
                            }
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1156447); // This auction is private.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1156432); // There is no active auction to complete this action.
                }
            }
        }

        public override void OnChop(Mobile from)
        {
            if (Auction != null && Auction.AuctionItemOnDisplay())
                from.SendLocalizedMessage(1156452); // You can't use a bladed item on an auction safe that has an auction item or is currently active.
            else
                base.OnChop(from);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> entries)
        {
            base.GetContextMenuEntries(from, entries);
            SetSecureLevelEntry.AddTo(from, this, entries);
        }

        public override void Delete()
        {
            base.Delete();

            if (Auction != null)
            {
                Auction.HouseCollapse();
                Auction = null;
            }
        }

        public AuctionSafe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Level);

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
            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();

            if (reader.ReadInt() == 1)
                Auction = new Auction(this, reader);
        }

        [Flipable(0x9C18, 0x9C19)]
        public class InternalComponent : AddonComponent
        {
            public override bool ForceShowProperties { get { return true; } }
            public override int LabelNumber { get { return 1156371; } } // Auction Safe

            public InternalComponent()
                : base(0x9C18)
            {
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                list.Add(501643); // locked down
            }

            public InternalComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
    }

    public class AuctionSafeDeed : BaseAddonDeed, IRewardItem
    {
        public bool SouthFacing { get; set; }
        public Mobile From { get; set; }

        public override BaseAddon Addon { get { return new AuctionSafe(From, SouthFacing); } }
        public override int LabelNumber { get { return 1156371; } } // Auction Safe

        public bool IsRewardItem { get; set; }

        [Constructable]
        public AuctionSafeDeed()
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null)
            {
                if (house.Owner == from || house.IsCoOwner(from))
                {
                    if (house.Public)
                    {
                        From = from;
                        base.OnDoubleClick(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1156437); // Auction Safes can only be placed in public type houses.
                    }
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public AuctionSafeDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((bool)IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
                IsRewardItem = reader.ReadBool();

            if (LootType != LootType.Blessed)
                LootType = LootType.Blessed;
        }
    }
}
