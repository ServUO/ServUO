using System;

namespace Server.Items
{
    public class DecoGinseng : Item
    {
        [Constructable]
        public DecoGinseng()
            : base(0x18E9)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoGinseng(Serial serial)
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