using System;

namespace Server.Items
{
    public class InvisibilityScroll : SpellScroll
    {
        [Constructable]
        public InvisibilityScroll()
            : this(1)
        {
        }

        [Constructable]
        public InvisibilityScroll(int amount)
            : base(43, 0x1F58, amount)
        {
        }

        public InvisibilityScroll(Serial serial)
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