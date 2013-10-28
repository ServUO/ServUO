using System;

namespace Server.Items
{
    public class DecoWyrmsHeart : Item
    {
        [Constructable]
        public DecoWyrmsHeart()
            : base(0xF91)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoWyrmsHeart(Serial serial)
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