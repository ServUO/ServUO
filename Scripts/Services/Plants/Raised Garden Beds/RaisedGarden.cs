using Server.Engines.Plants;
using Server.Gumps;

namespace Server.Items
{
    public enum GardenBedDirection
    {
        South = 1,
        East,
        Large,
        Small
    }

    public class RaisedGardenAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new RaisedGardenDeed();
        public override bool ForceShowProperties => true;

        [Constructable]
        public RaisedGardenAddon(GardenBedDirection direction)
        {
            switch (direction)
            {
                case GardenBedDirection.Large:
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
                case GardenBedDirection.East:
                    {
                        AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(19235), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(19237), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(19239), 1, 2, 0);
                        AddComponent(new GardenAddonComponent(19238), 0, 2, 0);
                        AddComponent(new GardenAddonComponent(19236), 0, 1, 0);

                        break;
                    }
                case GardenBedDirection.South:
                    {
                        AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(19240), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(19235), 2, 0, 0);
                        AddComponent(new GardenAddonComponent(19239), 2, 1, 0);
                        AddComponent(new GardenAddonComponent(19242), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(19238), 0, 1, 0);

                        break;
                    }
                case GardenBedDirection.Small:
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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RaisedGardenDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1150359;  // Raised Garden Bed

        public override BaseAddon Addon => new RaisedGardenAddon(m_Direction);
        public GardenBedDirection m_Direction;

        [Constructable]
        public RaisedGardenDeed()
        {
            LootType = LootType.Blessed;
        }

        public RaisedGardenDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, 1150381); // Garden Bed (South) 
            list.Add(2, 1150382); // Garden Bed (East)
            list.Add(3, 1150620); // Garden Bed (Large)
            list.Add(4, 1150621); // Garden Bed (Small)
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_Direction = (GardenBedDirection)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076170)); // Choose Direction
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadEncodedInt();
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

        public override int LabelNumber => Addon is RaisedGardenAddon ? 1150359 : 1159056; // Raised Garden Bed - Field Garden Bed

        public GardenAddonComponent(int itemID)
            : base(itemID)
        {
        }

        public int ZLocation()
        {
            return Addon is RaisedGardenAddon ? 5 : 1;
        }

        public override void Delete()
        {
            base.Delete();

            if (Plant != null)
                m_Plant.Z -= ZLocation();
        }

        public GardenAddonComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Plant);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Plant = reader.ReadItem() as PlantItem;

            if (m_Plant != null && m_Plant is GardenBedPlantItem && ((GardenBedPlantItem)m_Plant).Component == null)
                ((GardenBedPlantItem)m_Plant).Component = this;
        }
    }
}
