using System;

namespace Server.Items
{
    public class GrobusFur : Item
    {
        [Constructable]
        public GrobusFur()
            : base(0x11F4)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 0x455;
        }

        public GrobusFur(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074676;
            }
        }// Grobu's Fur
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