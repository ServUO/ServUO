using System;

namespace Server.Items
{
    public class LightweightShortbow : MagicalShortbow
    {
        [Constructable]
        public LightweightShortbow()
        {
            this.Balanced = true;
        }

        public LightweightShortbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073510;
            }
        }// lightweight shortbow
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