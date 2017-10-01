using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public enum AddonFacing
    {
        South = 0,
        East = 1,
    }

    public class DragonHeadAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new DragonHeadAddonDeed(); } }

        public AddonFacing Facing { get; set; }

        [Constructable]
        public DragonHeadAddon()
            : this(AddonFacing.South)
        {
        }

        [Constructable]
        public DragonHeadAddon(AddonFacing facing)
        {
            Facing = facing;

            switch (facing)
            {
                case AddonFacing.South:
                    AddComponent(new DragonHeadComponent(0x2234), 0, 0, 10);
                    break;
                case AddonFacing.East:
                    AddComponent(new DragonHeadComponent(0x2235), 0, 0, 10);
                    break;

            }
        }

        private class DragonHeadComponent : AddonComponent
        {
            public override bool NeedsWall { get { return true; } }
            public override Point3D WallPosition
            {
                get
                {
                    switch (ItemID)
                    {
                        default:
                        case 0x2234: return new Point3D(0, -1, 0);
                        case 0x2235: return new Point3D(-1, 0, 0);
                    }
                }
            }

            public DragonHeadComponent(int id)
                : base(id)
            {
            }

            public DragonHeadComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.WriteEncodedInt(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadEncodedInt();
            }
        }

        public DragonHeadAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DragonHeadAddonDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new DragonHeadAddon(Facing); } }

        private AddonFacing Facing { get; set; }

        public override int LabelNumber { get { return 1080209; } } // Dragon Head

        [Constructable]
        public DragonHeadAddonDeed()
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public DragonHeadAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)AddonFacing.South, 1080208);
            list.Add((int)AddonFacing.East, 1080207);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            Facing = (AddonFacing)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }

    // old classes
    [TypeAlias("Server.Items.DragonHead")]
    public class DragonHeadOld : Item, IAddon
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DragonHeadOld()
            : this(0x2234)
        {
        }

        [Constructable]
        public DragonHeadOld(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public DragonHeadOld(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public Item Deed
        {
            get
            {
                return new DragonHeadAddonDeed();
            }
        }
        public bool FacingEast
        {
            get
            {
                return ItemID == 0x2235;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.IsOwner(from))
                {
                    from.CloseGump(typeof(RewardDemolitionGump));
                    from.SendGump(new RewardDemolitionGump(this, 1049783)); // Do you wish to re-deed this decoration?
                }
                else
                    from.SendLocalizedMessage(1049784); // You can only re-deed this decoration if you are the house owner or originally placed the decoration.
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (map == null || !map.CanFit(p.X, p.Y, p.Z, ItemData.Height))
                return false;

            if (FacingEast)
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // west wall                
            else
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // north wall
        }
    }

    [TypeAlias("Server.Items.DragonHeadDeed")]
    public class DragonHeadDeedOld : Item
    {
        public override int LabelNumber { get { return 1028756; } } // dragon head

        [Constructable]
        public DragonHeadDeedOld()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public DragonHeadDeedOld(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.FromSeconds(10), Replace);
        }

        private void Replace()
        {
            Container c = Parent as Container;

            if (c != null)
            {
                var deed = new DragonHeadAddonDeed();
                c.DropItem(deed);
            }
            else if (Parent == null)
            {
                Server.Multis.BaseHouse house = Server.Multis.BaseHouse.FindHouseAt(this);

                var deed = new DragonHeadAddonDeed();
                deed.MoveToWorld(Location, Map);

                deed.IsLockedDown = IsLockedDown;
                deed.IsSecure = IsSecure;
                deed.Movable = Movable;

                if (house != null && house.LockDowns.ContainsKey(this))
                {
                    house.LockDowns.Remove(this);
                    house.LockDowns.Add(deed, house.Owner);
                }
                else if (house != null && house.IsSecure(this))
                {
                    house.ReleaseSecure(house.Owner, this);
                    house.AddSecure(house.Owner, deed);
                }
            }

            Delete();
        }
    }
}