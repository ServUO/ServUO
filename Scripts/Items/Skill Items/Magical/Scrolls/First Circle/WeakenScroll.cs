using System;

namespace Server.Items
{
    public class WeakenScroll : SpellScroll
    {
        [Constructable]
        public WeakenScroll()
            : this(1)
        {
        }

        [Constructable]
        public WeakenScroll(int amount)
            : base(7, 0x1F34, amount)
        {
        }

        public WeakenScroll(Serial serial)
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