using System;

namespace Server.Items
{
    public class CoilsFang : Item
    {
        [Constructable]
        public CoilsFang()
            : base(0x10E8)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 0x487;
        }

        public CoilsFang(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074229;
            }
        }// Coil's Fang
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