using Server;
using System;
using System.Collections.Generic;
using Server.Engines.Plants;

namespace Server.Items
{
    public class RaisedGardenSmallAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RaisedGardenSmallAddonDeed(); } }

        [Constructable]
        public RaisedGardenSmallAddon()
        {
            AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
            AddComponent(new GardenAddonComponent(19235), 1, 0, 0);
            AddComponent(new GardenAddonComponent(19239), 1, 1, 0);
            AddComponent(new GardenAddonComponent(19238), 0, 1, 0);
        }

        public override void OnChop( Mobile from )
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp is GardenAddonComponent && ((GardenAddonComponent)comp).Plant != null)
                {
                    from.SendMessage("You must remove any plants before you can re-deed the raised garden.");
                    return;
                }
            }

            base.OnChop(from);
        }

        public RaisedGardenSmallAddon(Serial serial)
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

    public class RaisedGardenSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RaisedGardenSouthAddonDeed(); } }

        [Constructable]
        public RaisedGardenSouthAddon()
        {
            AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
            AddComponent(new GardenAddonComponent(19240), 1, 0, 0);
            AddComponent(new GardenAddonComponent(19235), 2, 0, 0);
            AddComponent(new GardenAddonComponent(19239), 2, 1, 0);
            AddComponent(new GardenAddonComponent(19242), 1, 1, 0);
            AddComponent(new GardenAddonComponent(19238), 0, 1, 0);
        }

        public override void OnChop(Mobile from)
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp is GardenAddonComponent && ((GardenAddonComponent)comp).Plant != null)
                {
                    from.SendMessage("You must remove any plants before you can re-deed the raised garden.");
                    return;
                }
            }

            base.OnChop(from);
        }

        public RaisedGardenSouthAddon(Serial serial)
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

    public class RaisedGardenEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RaisedGardenEastAddonDeed(); } }

        [Constructable]
        public RaisedGardenEastAddon()
        {
            AddComponent(new GardenAddonComponent(19234), 0, 0, 0);
            AddComponent(new GardenAddonComponent(19235), 1, 0, 0);
            AddComponent(new GardenAddonComponent(19237), 1, 1, 0);
            AddComponent(new GardenAddonComponent(19239), 1, 2, 0);
            AddComponent(new GardenAddonComponent(19238), 0, 2, 0);
            AddComponent(new GardenAddonComponent(19236), 0, 1, 0);
        }

        public override void OnChop(Mobile from)
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp is GardenAddonComponent && ((GardenAddonComponent)comp).Plant != null)
                {
                    from.SendMessage("You must remove any plants before you can re-deed the raised garden.");
                    return;
                }
            }

            base.OnChop(from);
        }

        public RaisedGardenEastAddon(Serial serial)
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

    public class RaisedGardenLargeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new RaisedGardenLargeAddonDeed(); } }

        [Constructable]
        public RaisedGardenLargeAddon()
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
        }

        public override void OnChop(Mobile from)
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp is GardenAddonComponent && ((GardenAddonComponent)comp).Plant != null)
                {
                    from.SendMessage("You must remove any plants before you can re-deed the raised garden.");
                    return;
                }
            }

            base.OnChop(from);
        }

        public RaisedGardenLargeAddon(Serial serial)
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

    public class RaisedGardenSmallAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RaisedGardenSmallAddon(); } }
        public override int LabelNumber { get { return 1150621; } } // Raised Garden Bed (Small)

        [Constructable]
        public RaisedGardenSmallAddonDeed()
        {
        }

        public RaisedGardenSmallAddonDeed(Serial serial)
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

    public class RaisedGardenEastAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RaisedGardenEastAddon(); } }
        public override int LabelNumber { get { return 1150382; } } // Raised Garden Bed (East)

        [Constructable]
        public RaisedGardenEastAddonDeed()
        {
        }

        public RaisedGardenEastAddonDeed(Serial serial)
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

    public class RaisedGardenSouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RaisedGardenSouthAddon(); } }
        public override int LabelNumber { get { return 1150381; } } // Raised Garden Bed (South)

        [Constructable]
        public RaisedGardenSouthAddonDeed()
        {
        }

        public RaisedGardenSouthAddonDeed(Serial serial)
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

    public class RaisedGardenLargeAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RaisedGardenLargeAddon(); } }
        public override int LabelNumber { get { return 1150620; } } // Raised Garden Bed (Large)

        [Constructable]
        public RaisedGardenLargeAddonDeed()
        {
        }

        public RaisedGardenLargeAddonDeed(Serial serial)
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
                    if (m_Plant.X != this.X || m_Plant.Y != this.Y || m_Plant.Map != this.Map || m_Plant.Deleted)
                        m_Plant = null;
                }

                return m_Plant; 
            } 
            set 
            {
                m_Plant = value;

                if (m_Plant != null)
                {
                    if (m_Plant.X != this.X || m_Plant.Y != this.Y || m_Plant.Map != this.Map || m_Plant.Deleted)
                        m_Plant = null;
                }
            } 
        }

        public override int LabelNumber { get { return 1150359; } } // Raised Garden Bed

        public GardenAddonComponent( int itemID) : base( itemID )
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