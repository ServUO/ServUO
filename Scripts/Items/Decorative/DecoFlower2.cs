using System;

namespace Server.Items
{
    public class DecoFlower2 : Item
    {
        [Constructable]
        public DecoFlower2()
            : base(0x18D9)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoFlower2(Serial serial)
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