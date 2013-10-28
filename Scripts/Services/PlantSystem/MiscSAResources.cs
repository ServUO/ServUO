using System;
using Server.Engines.Plants;

namespace Server.Items
{
    public class DryReeds : Item
    {
        private PlantHue m_PlantHue;
        [Constructable]
        public DryReeds()
            : this(1)
        {
        }

        [Constructable]
        public DryReeds(int amount)
            : base(0x1BD5)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public DryReeds(Serial serial)
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
                return 1112248;
            }
        }// Dry Reeds
        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_PlantHue);
            
            if (this.Amount > 1)
                list.Add(1113275, "{0}\t{1}", this.Amount, "#" + hueInfo.Name);  // ~1_COLOR~ Softened Reeds
            else 
                list.Add(1112289, "#" + hueInfo.Name);  // ~1_COLOR~ dry reeds
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

    public class SoftenedReeds : Item
    {
        private PlantHue m_PlantHue;
        [Constructable]
        public SoftenedReeds()
            : this(1)
        {
        }

        [Constructable]
        public SoftenedReeds(int amount)
            : base(0x4006)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public SoftenedReeds(Serial serial)
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
        public bool RetainsColorFrom
        {
            get
            {
                return true;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1112246;
            }
        }// Softened Reeds
        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_PlantHue);

            if (this.Amount > 1)
                list.Add(1113323, "{0}\t{1}", this.Amount, "#" + hueInfo.Name);  // ~1_COLOR~ Softened Reeds
            else 
                list.Add(1112346, "#" + hueInfo.Name);  // ~1_COLOR~ Softened Reeds
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