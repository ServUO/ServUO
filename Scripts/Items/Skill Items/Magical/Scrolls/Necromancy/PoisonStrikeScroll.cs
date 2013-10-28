using System;

namespace Server.Items
{
    public class PoisonStrikeScroll : SpellScroll
    {
        [Constructable]
        public PoisonStrikeScroll()
            : this(1)
        {
        }

        [Constructable]
        public PoisonStrikeScroll(int amount)
            : base(109, 0x2269, amount)
        {
        }

        public PoisonStrikeScroll(Serial serial)
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