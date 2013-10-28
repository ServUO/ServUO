using System;

namespace Server.Items
{
    public class DecoNightshade3 : Item
    {
        [Constructable]
        public DecoNightshade3()
            : base(0x18E6)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoNightshade3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}