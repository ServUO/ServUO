namespace Server.Items
{
    public class ShipModelOfTheHMSCape : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ShipModelOfTheHMSCape()
            : base(0x14F3)
        {
            Hue = 0x37B;
        }

        public ShipModelOfTheHMSCape(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063476;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}