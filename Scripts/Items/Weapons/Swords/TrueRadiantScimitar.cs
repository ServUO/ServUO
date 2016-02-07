using System;

namespace Server.Items
{
    public class TrueRadiantScimitar : RadiantScimitar
    {
        [Constructable]
        public TrueRadiantScimitar()
        {
            this.Attributes.NightSight = 1;
        }

        public TrueRadiantScimitar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073541;
            }
        }// true radiant scimitar
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