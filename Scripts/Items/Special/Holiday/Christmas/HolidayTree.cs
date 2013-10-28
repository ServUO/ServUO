using System;
using System.Collections;
using Server.Multis;

namespace Server.Items
{
    public enum HolidayTreeType
    {
        Classic,
        Modern
    }

    public class HolidayTree : Item, IAddon
    {
        private ArrayList m_Components;
        private Mobile m_Placer;
        public HolidayTree(Mobile from, HolidayTreeType type, Point3D loc)
            : base(1)
        {
            this.Movable = false;
            this.MoveToWorld(loc, from.Map);

            this.m_Placer = from;
            this.m_Components = new ArrayList();

            switch ( type )
            {
                case HolidayTreeType.Classic:
                    {
                        this.ItemID = 0xCD7;

                        this.AddItem(0, 0, 0, new TreeTrunk(this, 0xCD6));

                        this.AddOrnament(0, 0, 2, 0xF22);
                        this.AddOrnament(0, 0, 9, 0xF18);
                        this.AddOrnament(0, 0, 15, 0xF20);
                        this.AddOrnament(0, 0, 19, 0xF17);
                        this.AddOrnament(0, 0, 20, 0xF24);
                        this.AddOrnament(0, 0, 20, 0xF1F);
                        this.AddOrnament(0, 0, 20, 0xF19);
                        this.AddOrnament(0, 0, 21, 0xF1B);
                        this.AddOrnament(0, 0, 28, 0xF2F);
                        this.AddOrnament(0, 0, 30, 0xF23);
                        this.AddOrnament(0, 0, 32, 0xF2A);
                        this.AddOrnament(0, 0, 33, 0xF30);
                        this.AddOrnament(0, 0, 34, 0xF29);
                        this.AddOrnament(0, 1, 7, 0xF16);
                        this.AddOrnament(0, 1, 7, 0xF1E);
                        this.AddOrnament(0, 1, 12, 0xF0F);
                        this.AddOrnament(0, 1, 13, 0xF13);
                        this.AddOrnament(0, 1, 18, 0xF12);
                        this.AddOrnament(0, 1, 19, 0xF15);
                        this.AddOrnament(0, 1, 25, 0xF28);
                        this.AddOrnament(0, 1, 29, 0xF1A);
                        this.AddOrnament(0, 1, 37, 0xF2B);
                        this.AddOrnament(1, 0, 13, 0xF10);
                        this.AddOrnament(1, 0, 14, 0xF1C);
                        this.AddOrnament(1, 0, 16, 0xF14);
                        this.AddOrnament(1, 0, 17, 0xF26);
                        this.AddOrnament(1, 0, 22, 0xF27);

                        break;
                    }
                case HolidayTreeType.Modern:
                    {
                        this.ItemID = 0x1B7E;

                        this.AddOrnament(0, 0, 2, 0xF2F);
                        this.AddOrnament(0, 0, 2, 0xF20);
                        this.AddOrnament(0, 0, 2, 0xF22);
                        this.AddOrnament(0, 0, 5, 0xF30);
                        this.AddOrnament(0, 0, 5, 0xF15);
                        this.AddOrnament(0, 0, 5, 0xF1F);
                        this.AddOrnament(0, 0, 5, 0xF2B);
                        this.AddOrnament(0, 0, 6, 0xF0F);
                        this.AddOrnament(0, 0, 7, 0xF1E);
                        this.AddOrnament(0, 0, 7, 0xF24);
                        this.AddOrnament(0, 0, 8, 0xF29);
                        this.AddOrnament(0, 0, 9, 0xF18);
                        this.AddOrnament(0, 0, 14, 0xF1C);
                        this.AddOrnament(0, 0, 15, 0xF13);
                        this.AddOrnament(0, 0, 15, 0xF20);
                        this.AddOrnament(0, 0, 16, 0xF26);
                        this.AddOrnament(0, 0, 17, 0xF12);
                        this.AddOrnament(0, 0, 18, 0xF17);
                        this.AddOrnament(0, 0, 20, 0xF1B);
                        this.AddOrnament(0, 0, 23, 0xF28);
                        this.AddOrnament(0, 0, 25, 0xF18);
                        this.AddOrnament(0, 0, 25, 0xF2A);
                        this.AddOrnament(0, 1, 7, 0xF16);

                        break;
                    }
            }
        }

        public HolidayTree(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get
            {
                return this.m_Placer;
            }
            set
            {
                this.m_Placer = value;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1041117;
            }
        }// a tree for the holidays
        Item IAddon.Deed
        {
            get
            {
                return new HolidayTreeDeed();
            }
        }
        public override void OnAfterDelete()
        {
            for (int i = 0; i < this.m_Components.Count; ++i)
                ((Item)this.m_Components[i]).Delete();
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            return map.CanFit((Point3D)p, 20);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_Placer);

            writer.Write((int)this.m_Components.Count);

            for (int i = 0; i < this.m_Components.Count; ++i)
                writer.Write((Item)this.m_Components[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Placer = reader.ReadMobile();

                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadInt();

                        this.m_Components = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            Item item = reader.ReadItem();

                            if (item != null)
                                this.m_Components.Add(item);
                        }

                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.Zero, ValidatePlacement);
        }

        public void ValidatePlacement()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null)
            {
                HolidayTreeDeed deed = new HolidayTreeDeed();
                deed.MoveToWorld(this.Location, this.Map);
                this.Delete();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                if (this.m_Placer == null || from == this.m_Placer || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    from.AddToBackpack(new HolidayTreeDeed());

                    this.Delete();

                    BaseHouse house = BaseHouse.FindHouseAt(this);

                    if (house != null && house.Addons.Contains(this))
                    {
                        house.Addons.Remove(this);
                    }

                    from.SendLocalizedMessage(503393); // A deed for the tree has been placed in your backpack.
                }
                else
                {
                    from.SendLocalizedMessage(503396); // You cannot take this tree down.
                }
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private void AddOrnament(int x, int y, int z, int itemID)
        {
            this.AddItem(x + 1, y + 1, z + 11, new Ornament(itemID));
        }

        private void AddItem(int x, int y, int z, Item item)
        {
            item.MoveToWorld(new Point3D(this.Location.X + x, this.Location.Y + y, this.Location.Z + z), this.Map);

            this.m_Components.Add(item);
        }

        private class Ornament : Item
        {
            public Ornament(int itemID)
                : base(itemID)
            {
                this.Movable = false;
            }

            public Ornament(Serial serial)
                : base(serial)
            {
            }

            public override int LabelNumber
            {
                get
                {
                    return 1041118;
                }
            }// a tree ornament
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
        }

        private class TreeTrunk : Item
        {
            private HolidayTree m_Tree;
            public TreeTrunk(HolidayTree tree, int itemID)
                : base(itemID)
            {
                this.Movable = false;
                this.MoveToWorld(tree.Location, tree.Map);

                this.m_Tree = tree;
            }

            public TreeTrunk(Serial serial)
                : base(serial)
            {
            }

            public override int LabelNumber
            {
                get
                {
                    return 1041117;
                }
            }// a tree for the holidays
            public override void OnDoubleClick(Mobile from)
            {
                if (this.m_Tree != null && !this.m_Tree.Deleted)
                    this.m_Tree.OnDoubleClick(from);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(this.m_Tree);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch ( version )
                {
                    case 0:
                        {
                            this.m_Tree = reader.ReadItem() as HolidayTree;

                            if (this.m_Tree == null)
                                this.Delete();

                            break;
                        }
                }
            }
        }
    }
}