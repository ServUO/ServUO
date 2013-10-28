using System;

namespace Server.Items
{
    public class DecoGoldIngots : Item
    {
        [Constructable]
        public DecoGoldIngots()
            : base(0x1BEA)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoGoldIngots(Serial serial)
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