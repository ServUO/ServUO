using System;

namespace Server.Items
{
    public class DecoRoseOfTrinsic3 : Item
    {
        [Constructable]
        public DecoRoseOfTrinsic3()
            : base(0x234B)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoRoseOfTrinsic3(Serial serial)
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