using System;

namespace Server.Items
{
    public class ThrashersTail : Item
    {
        [Constructable]
        public ThrashersTail()
            : base(0x1A9D)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 0x455;
        }

        public ThrashersTail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074230;
            }
        }// Thrasher's Tail
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