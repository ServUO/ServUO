using System;

namespace Server.Items
{    
    public class OrcishSchematics : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154232; } } // Schematic for an Orcish Drilling Machine
        
        [Constructable]
        public OrcishSchematics() : base(0x2258)
        {
            this.Hue = 1945;
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public OrcishSchematics(Serial serial) : base(serial)
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
}
