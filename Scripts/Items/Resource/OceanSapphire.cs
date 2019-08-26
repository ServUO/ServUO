using System;

namespace Server.Items
{
    public class OceanSapphire : Item, ICommodity
    {
        public override int LabelNumber { get { return 1159162; } } // ocean sapphire

        [Constructable]
        public OceanSapphire()
            : this(1)
        {
        }

        [Constructable]
        public OceanSapphire(int amount)
            : base(0xA414)
        {
            Hue = 1917;
            Stackable = true;
            Amount = amount;
        }

        public OceanSapphire(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }
        
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
