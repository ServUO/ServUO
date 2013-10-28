using System;

namespace Server.Items
{
    public class SummonWaterElementalScroll : SpellScroll
    {
        [Constructable]
        public SummonWaterElementalScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonWaterElementalScroll(int amount)
            : base(63, 0x1F6C, amount)
        {
        }

        public SummonWaterElementalScroll(Serial serial)
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