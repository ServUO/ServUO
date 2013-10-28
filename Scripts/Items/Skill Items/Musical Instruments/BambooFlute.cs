using System;

namespace Server.Items
{
    public class BambooFlute : BaseInstrument
    {
        [Constructable]
        public BambooFlute()
            : base(0x2805, 0x504, 0x503)
        {
            this.Weight = 2.0;
        }

        public BambooFlute(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 3.0)
                this.Weight = 2.0;
        }
    }
}