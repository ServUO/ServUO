using System;
using Server;

namespace Server.Items 
{
    public class AncientShipModelOfTheHMSCape : Item
    {
        public override int LabelNumber { get { return 1063476; } }

        [Constructable]
        public AncientShipModelOfTheHMSCape()
            : base(0x14F3)
        {
            Name = "an Ancient Ship Model Of The HMS Cape";
            Hue = 0x37B;
        }

        public AncientShipModelOfTheHMSCape(Serial serial)
            : base(serial)
        {
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
