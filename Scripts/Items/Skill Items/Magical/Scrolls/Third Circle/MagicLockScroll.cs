using System;

namespace Server.Items
{
    public class MagicLockScroll : SpellScroll
    {
        [Constructable]
        public MagicLockScroll()
            : this(1)
        {
        }

        [Constructable]
        public MagicLockScroll(int amount)
            : base(18, 0x1F3F, amount)
        {
        }

        public MagicLockScroll(Serial serial)
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