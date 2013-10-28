using System;

namespace Server.Items
{
    public class RecallScroll : SpellScroll
    {
        [Constructable]
        public RecallScroll()
            : this(1)
        {
        }

        [Constructable]
        public RecallScroll(int amount)
            : base(31, 0x1F4C, amount)
        {
        }

        public RecallScroll(Serial serial)
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
}