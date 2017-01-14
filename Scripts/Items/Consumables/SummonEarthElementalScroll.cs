using System;

namespace Server.Items
{
    public class SummonEarthElementalScroll : SpellScroll
    {
        [Constructable]
        public SummonEarthElementalScroll()
            : this(1)
        {
        }

        [Constructable]
        public SummonEarthElementalScroll(int amount)
            : base(61, 0x1F6A, amount)
        {
        }

        public SummonEarthElementalScroll(Serial serial)
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