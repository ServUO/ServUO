using System;

namespace Server.Items
{
    public class MagicUnTrapScroll : SpellScroll
    {
        [Constructable]
        public MagicUnTrapScroll()
            : this(1)
        {
        }

        [Constructable]
        public MagicUnTrapScroll(int amount)
            : base(13, 0x1F3A, amount)
        {
        }

        public MagicUnTrapScroll(Serial serial)
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