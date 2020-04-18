namespace Server.Items
{
    public class ShipsBellOfBmvArarat : BaseDecorationArtifact
    {
        public override int ArtifactRarity => 8;
        public override bool IsArtifact => true;

        [Constructable]
        public ShipsBellOfBmvArarat()
            : base(0x4C5E)
        {
            Name = "Ship's Bell Of The Bmv Ararat";
            Weight = 10.0;
            Hue = 2968; //checked
        }

        public ShipsBellOfBmvArarat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}