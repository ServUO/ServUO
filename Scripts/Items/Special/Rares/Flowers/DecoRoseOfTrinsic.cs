using System;

namespace Server.Items
{
    public class DecoRoseOfTrinsic : Item
    {
        [Constructable]
        public DecoRoseOfTrinsic()
            : base(0x234C)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoRoseOfTrinsic(Serial serial)
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