using System;

namespace Server.Items
{
    public class WallOfStoneScroll : SpellScroll
    {
        [Constructable]
        public WallOfStoneScroll()
            : this(1)
        {
        }

        [Constructable]
        public WallOfStoneScroll(int amount)
            : base(23, 0x1F44, amount)
        {
        }

        public WallOfStoneScroll(Serial serial)
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