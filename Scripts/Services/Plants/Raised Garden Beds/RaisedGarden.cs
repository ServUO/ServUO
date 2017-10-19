using Server;
using System;
using Server.Engines.Plants;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public enum RaisedGardenDirection
    {
        South = 1,
        East,
        Large,
        Small
    }

    [TypeAlias("Server.Items.RaisedGardenSmallAddon", "Server.Items.RaisedGardenSouthAddon", "Server.Items.RaisedGardenEastAddon", "Server.Items.RaisedGardenLargeAddon")]
    public class RaisedGardenAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RaisedGardenDeed(); } }

        [Constructable]
        public RaisedGardenAddon(RaisedGardenDirection direction)
        {
            switch (direction)
            {
                case RaisedGardenDirection.Large:
                    {
                        AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(19240), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(19235), 2, 0, 0);
                        AddComponent(new GardenAddonComponent(19237), 2, 1, 0);
                        AddComponent(new GardenAddonComponent(19239), 2, 2, 0);
                        AddComponent(new GardenAddonComponent(19242), 1, 2, 0);
                        AddComponent(new GardenAddonComponent(19238), 0, 2, 0);
                        AddComponent(new GardenAddonComponent(19236), 0, 1, 0);
                        AddComponent(new GardenAddonComponent(19241), 1, 1, 0);

                        break;
                    }
                case RaisedGardenDirection.East:
                    {
                        AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(19235), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(19237), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(19239), 1, 2, 0);
                        AddComponent(new GardenAddonComponent(19238), 0, 2, 0);
                        AddComponent(new GardenAddonComponent(19236), 0, 1, 0);

                        break;
                    }
                case RaisedGardenDirection.South:
                    {
                        AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(19240), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(19235), 2, 0, 0);
                        AddComponent(new GardenAddonComponent(19239), 2, 1, 0);
                        AddComponent(new GardenAddonComponent(19242), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(19238), 0, 1, 0);

                        break;
                    }
                case RaisedGardenDirection.Small:
                    {
                        AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(19235), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(19239), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(19238), 0, 1, 0);

                        break;
                    }
            }
        }

        public override void OnChop(Mobile from)
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp is GardenAddonComponent && ((GardenAddonComponent)comp).Plant != null)
                {
                    from.SendLocalizedMessage(1150383); // You need to remove all plants through the plant menu before destroying this.
                    return;
                }
            }

            base.OnChop(from);
        }

        public RaisedGardenAddon(Serial serial)
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
    }

    [TypeAlias("Server.Items.RaisedGardenSmallAddonDeed", "Server.Items.RaisedGardenEastAddonDeed", "Server.Items.RaisedGardenSouthAddonDeed", "Server.Items.RaisedGardenLargeAddonDeed")]
    public class RaisedGardenDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1150359; } } // Raised Garden Bed
        public override BaseAddon Addon { get { return new RaisedGardenAddon(m_Direction); } }
        public RaisedGardenDirection m_Direction;

        [Constructable]
        public RaisedGardenDeed()
        {
            LootType = LootType.Blessed;
        }

        public RaisedGardenDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1150651); // * Requires the "Rustic" theme pack
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
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

        private class InternalGump : Gump
        {
            private RaisedGardenDeed m_Deed;

            public InternalGump(RaisedGardenDeed deed) : base(60, 36)
            {
                m_Deed = deed;

                AddPage(0);

                AddBackground(0, 0, 273, 324, 0x13BE);
                AddImageTiled(10, 10, 253, 20, 0xA40);
                AddImageTiled(10, 40, 253, 244, 0xA40);
                AddImageTiled(10, 294, 253, 20, 0xA40);
                AddAlphaRegion(10, 10, 253, 304);
                AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                AddHtmlLocalized(14, 12, 273, 20, 1076170, 0x7FFF, false, false); // Choose Direction

                AddPage(1);

                AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 47, 213, 20, 1150381, 0x7FFF, false, false); // Raised Garden Bed (South)

                AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 71, 213, 20, 1150382, 0x7FFF, false, false); // Raised Garden Bed (East)

                AddButton(19, 97, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 95, 213, 20, 1150620, 0x7FFF, false, false); // Raised Garden Bed (Large)

                AddButton(19, 121, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 119, 213, 20, 1150621, 0x7FFF, false, false); // Raised Garden Bed (Small)
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Deed == null || m_Deed.Deleted || info.ButtonID == 0)
                    return;

                m_Deed.m_Direction = (RaisedGardenDirection)info.ButtonID;
                m_Deed.SendTarget(sender.Mobile);
            }
        }
    }

    public class GardenAddonComponent : AddonComponent
    {
        private PlantItem m_Plant;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantItem Plant
        {
            get
            {
                if (m_Plant != null)
                {
                    if (m_Plant.X != X || m_Plant.Y != Y || m_Plant.Map != Map || m_Plant.Deleted)
                        m_Plant = null;
                }

                return m_Plant;
            }
            set
            {
                m_Plant = value;

                if (m_Plant != null)
                {
                    if (m_Plant.X != X || m_Plant.Y != Y || m_Plant.Map != Map || m_Plant.Deleted)
                        m_Plant = null;
                }
            }
        }

        public override int LabelNumber { get { return 1150359; } } // Raised Garden Bed

        public GardenAddonComponent(int itemID) : base(itemID)
        {
        }

        public override void Delete()
        {
            base.Delete();

            if (Plant != null)
                m_Plant.Z -= 5;
        }

        public GardenAddonComponent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Plant);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Plant = reader.ReadItem() as PlantItem;

            if (m_Plant != null && m_Plant is RaisedGardenPlantItem && ((RaisedGardenPlantItem)m_Plant).Component == null)
                ((RaisedGardenPlantItem)m_Plant).Component = this;
        }
    }
}