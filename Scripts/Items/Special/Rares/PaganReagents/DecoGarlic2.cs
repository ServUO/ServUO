using System;

namespace Server.Items
{
    public class DecoGarlic2 : Item
    {
        [Constructable]
        public DecoGarlic2()
            : base(0x18E2)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoGarlic2(Serial serial)
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