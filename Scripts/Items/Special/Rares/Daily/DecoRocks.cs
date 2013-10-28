using System;

namespace Server.Items
{
    public class DecoRocks : Item
    {
        [Constructable]
        public DecoRocks()
            : base(0x1367)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoRocks(Serial serial)
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