using System;

namespace Server.Items
{
    public class DecoGinseng2 : Item
    {
        [Constructable]
        public DecoGinseng2()
            : base(0x18EA)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoGinseng2(Serial serial)
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