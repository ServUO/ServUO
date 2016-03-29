using System;

namespace Server.Items
{
    public class ArchCureScroll : SpellScroll
    {
        [Constructable]
        public ArchCureScroll()
            : this(1)
        {
        }

        [Constructable]
        public ArchCureScroll(int amount)
            : base(24, 0x1F45, amount)
        {
        }

        public ArchCureScroll(Serial serial)
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