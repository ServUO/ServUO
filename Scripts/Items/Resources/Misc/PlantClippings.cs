using System;
using Server.Engines.Plants;

namespace Server.Items
{ 
    public class PlantClippings : Item
    {
        private PlantHue m_PlantHue;
        [Constructable]
        public PlantClippings()
            : this(1)
        {
        }

        [Constructable]
        public PlantClippings(int amount)
            : base(0x4022)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public PlantClippings(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue
        {
            get
            {
                return this.m_PlantHue;
            }
            set
            {
                this.m_PlantHue = value;
                this.Hue = PlantHueInfo.GetInfo(value).Hue;
                this.InvalidateProperties();
            }
        }
        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1112131;
            }
        }// Plant Clippings
        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_PlantHue);

            if (this.Amount > 1)
                list.Add(1113274, "{0}\t{1}", this.Amount, "#" + hueInfo.Name);  // ~1_COLOR~ Softened Reeds
            else 
                list.Add(1112122, "#" + hueInfo.Name);  // ~1_COLOR~ plant clippings
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_PlantHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    this.m_PlantHue = (PlantHue)reader.ReadInt();
                    break;
            }
        }
    }
}