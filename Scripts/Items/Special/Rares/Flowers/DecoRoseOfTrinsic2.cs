using System;

namespace Server.Items
{
    public class DecoRoseOfTrinsic2 : Item
    {
        [Constructable]
        public DecoRoseOfTrinsic2()
            : base(0x234D)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoRoseOfTrinsic2(Serial serial)
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