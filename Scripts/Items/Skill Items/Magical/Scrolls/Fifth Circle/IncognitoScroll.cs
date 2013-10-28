using System;

namespace Server.Items
{
    public class IncognitoScroll : SpellScroll
    {
        [Constructable]
        public IncognitoScroll()
            : this(1)
        {
        }

        [Constructable]
        public IncognitoScroll(int amount)
            : base(34, 0x1F4F, amount)
        {
        }

        public IncognitoScroll(Serial serial)
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