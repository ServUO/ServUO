using System;

namespace Server.Items
{
    public class Drums : BaseInstrument
    {
        [Constructable]
        public Drums()
            : base(0xE9C, 0x38, 0x39)
        {
            this.Weight = 4.0;
        }

        public Drums(Serial serial)
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
                this.Weight = 4.0;
        }
    }
}