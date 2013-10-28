using System;

namespace Server.Items
{
    public class ShipModelOfTheHMSCape : Item
    {
        [Constructable]
        public ShipModelOfTheHMSCape()
            : base(0x14F3)
        {
            this.Hue = 0x37B;
        }

        public ShipModelOfTheHMSCape(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063476;
            }
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