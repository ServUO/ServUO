using System;

namespace Server.Items
{
    [FlipableAttribute(0x2FB7, 0x3171)]
    public class ElvenQuiver : BaseQuiver
    {
        [Constructable]
        public ElvenQuiver()
            : base()
        {
            this.WeightReduction = 30;
        }

        public ElvenQuiver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1032657;
            }
        }// elven quiver
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