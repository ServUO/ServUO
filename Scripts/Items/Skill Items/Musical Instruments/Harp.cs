using System;

namespace Server.Items
{
    public class Harp : BaseInstrument
    {
        [Constructable]
        public Harp()
            : base(0xEB1, 0x43, 0x44)
        {
            this.Weight = 35.0;
        }

        public Harp(Serial serial)
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
                this.Weight = 35.0;
        }
    }
}