using System;

namespace Server.Items
{
    public class AbscessTail : Item
    {
        [Constructable]
        public AbscessTail()
            : base(0x1A9D)
        {
            this.LootType = LootType.Blessed;
            this.Hue = 0x51D; // TODO check
        }

        public AbscessTail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074231;
            }
        }// Abscess' Tail
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