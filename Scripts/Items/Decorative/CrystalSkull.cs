using System;

namespace Server.Items
{
    [FlipableAttribute(0x9A1A, 0x9A1B)]
    public class CrystalSkull : Item
    {
        public override int LabelNumber { get { return 1123474; } } // Crystal Skull

        [Constructable]
        public CrystalSkull()
            : base(0x9A1A)
        {          
        }

        public CrystalSkull(Serial serial)
            : base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
