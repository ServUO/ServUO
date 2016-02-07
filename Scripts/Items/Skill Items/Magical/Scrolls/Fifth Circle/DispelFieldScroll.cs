using System;

namespace Server.Items
{
    public class DispelFieldScroll : SpellScroll
    {
        [Constructable]
        public DispelFieldScroll()
            : this(1)
        {
        }

        [Constructable]
        public DispelFieldScroll(int amount)
            : base(33, 0x1F4E, amount)
        {
        }

        public DispelFieldScroll(Serial serial)
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