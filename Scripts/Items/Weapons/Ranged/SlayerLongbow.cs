using System;

namespace Server.Items
{
    public class SlayerLongbow : ElvenCompositeLongbow
    {
        [Constructable]
        public SlayerLongbow()
        {
            this.Slayer2 = (SlayerName)Utility.RandomMinMax(1, 27);
        }

        public SlayerLongbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073506;
            }
        }// slayer longbow
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}