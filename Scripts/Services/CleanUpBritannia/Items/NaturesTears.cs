using System;

namespace Server.Items
{
    public class NaturesTears : BaseInstrument
    {
        [Constructable]
        public NaturesTears()
            : base(0x0EB3)
        {
            RandomInstrument();
            this.Hue = 2075;
            this.Weight = 5;
            this.Slayer = SlayerName.Fey;
        }

        public NaturesTears(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1154373;
            }
        }// Nature's Tears
        public override int InitMinUses
        {
            get
            {
                return 450;
            }
        }
        public override int InitMaxUses
        {
            get
            {
                return 450;
            }
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
        }
    }
}