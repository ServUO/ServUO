using System;

namespace Server.Items
{
    public class CureScroll : SpellScroll
    {
        [Constructable]
        public CureScroll()
            : this(1)
        {
        }

        [Constructable]
        public CureScroll(int amount)
            : base(10, 0x1F37, amount)
        {
        }

        public CureScroll(Serial serial)
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