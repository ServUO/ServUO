using System;

namespace Server.Items
{
    public class DecoFlower : Item
    {
        [Constructable]
        public DecoFlower()
            : base(0x18DA)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoFlower(Serial serial)
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