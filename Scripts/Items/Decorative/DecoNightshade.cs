using System;

namespace Server.Items
{
    public class DecoNightshade : Item
    {
        [Constructable]
        public DecoNightshade()
            : base(0x18E7)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoNightshade(Serial serial)
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