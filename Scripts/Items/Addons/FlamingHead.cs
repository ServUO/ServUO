using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class FlamingHead : StoneFaceTrapNoDamage, IAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public FlamingHead()
            : this(StoneFaceTrapType.NorthWall)
        {
        }

        [Constructable]
        public FlamingHead(StoneFaceTrapType type)
            : base()
        {
            LootType = LootType.Blessed;
            Movable = false;
            Type = type;
        }

        public FlamingHead(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041266;// Flaming Head

        public override bool ForceShowProperties => true;

        public Item Deed
        {
            get
            {
                FlamingHeadDeed deed = new FlamingHeadDeed
                {
                    IsRewardItem = m_IsRewardItem
                };

                return deed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076218); // 2nd Year Veteran Reward
        }

        void IChopable.OnChop(Mobile user)
        {
            OnDoubleClick(user);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.IsOwner(from))
                {
                    from.CloseGump(typeof(RewardDemolitionGump));
                    from.SendGump(new RewardDemolitionGump(this, 1018329)); // Do you wish to re-deed this skull?
                }
                else
                    from.SendLocalizedMessage(1018328); // You can only re-deed a skull if you placed it or you are the owner of the house.
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (map == null || !map.CanFit(p.X, p.Y, p.Z, ItemData.Height))
                return false;

            if (Type == StoneFaceTrapType.NorthWestWall)
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map) && BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // north and west wall
            else if (Type == StoneFaceTrapType.NorthWall)
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // north wall
            else if (Type == StoneFaceTrapType.WestWall)
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // west wall

            return false;
        }
    }

    public class FlamingHeadDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public FlamingHeadDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public FlamingHeadDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041050;// a flaming head deed
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076218); // 2nd Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsOwner(from))
                {
                    from.SendLocalizedMessage(1042264); // Where would you like to place this head?
                    from.Target = new InternalTarget(this);
                }
                else
                    from.SendLocalizedMessage(502115); // You must be in your house to do this.
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.          	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        private class InternalTarget : Target
        {
            private readonly FlamingHeadDeed m_Head;
            public InternalTarget(FlamingHeadDeed head)
                : base(-1, true, TargetFlags.None)
            {
                m_Head = head;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Head == null || m_Head.Deleted)
                    return;

                if (m_Head.IsChildOf(from.Backpack))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);

                    if (house != null && house.IsOwner(from))
                    {
                        IPoint3D p = targeted as IPoint3D;
                        Map map = from.Map;

                        if (p == null || map == null)
                            return;

                        Point3D p3d = new Point3D(p);
                        ItemData id = TileData.ItemTable[0x10F5];

                        house = BaseHouse.FindHouseAt(p3d, map, id.Height);

                        if (house != null && house.IsOwner(from))
                        {
                            if (map.CanFit(p3d, id.Height))
                            {
                                bool north = BaseAddon.IsWall(p3d.X, p3d.Y - 1, p3d.Z, map);
                                bool west = BaseAddon.IsWall(p3d.X - 1, p3d.Y, p3d.Z, map);

                                FlamingHead head = null;

                                if (north && west)
                                    head = new FlamingHead(StoneFaceTrapType.NorthWestWall);
                                else if (north)
                                    head = new FlamingHead(StoneFaceTrapType.NorthWall);
                                else if (west)
                                    head = new FlamingHead(StoneFaceTrapType.WestWall);

                                if (north || west)
                                {
                                    house.Addons[head] = from;

                                    head.IsRewardItem = m_Head.IsRewardItem;
                                    head.MoveToWorld(p3d, map);

                                    m_Head.Delete();
                                }
                                else
                                    from.SendLocalizedMessage(1042266); // The head must be placed next to a wall.
                            }
                            else
                                from.SendLocalizedMessage(1042266); // The head must be placed next to a wall.
                        }
                        else
                            from.SendLocalizedMessage(1042036); // That location is not in your house.			
                    }
                    else
                        from.SendLocalizedMessage(502115); // You must be in your house to do this.
                }
                else
                    from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.     
            }
        }
    }
}
