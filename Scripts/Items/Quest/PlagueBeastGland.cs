using System;

namespace Server.Items
{
    public class PlagueBeastGland : Item
    {
        [Constructable]
        public PlagueBeastGland()
            : base(0x1CEF)
        {
            this.Name = "A Healthy Gland";
            this.Weight = 1.0;
            this.Hue = 0x6;
        }

        public PlagueBeastGland(Serial serial)
            : base(serial)
        {
        }

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