using System;

namespace Server.Items
{
    public class SummonFireElementalScroll : SpellScroll
    {
        [Constructable]
        public SummonFireElementalScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonFireElementalScroll(int amount)
            : base(62, 0x1F6B, amount)
        {
        }

        public SummonFireElementalScroll(Serial serial)
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