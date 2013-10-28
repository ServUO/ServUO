using System;

namespace Server.Items
{
    public class DecoGoldIngot : Item
    {
        [Constructable]
        public DecoGoldIngot()
            : base(0x1BE9)
        {
            this.Movable = true;
            this.Stackable = true;
        }

        public DecoGoldIngot(Serial serial)
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