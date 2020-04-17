namespace Server.Items
{
    public class BootsOfTheCrystalHydra : Boots
    {
        public override int LabelNumber => 1151209;  // Boots of the Crystal Hydra
        public override bool IsArtifact => true;

        [Constructable]
        public BootsOfTheCrystalHydra()
        {
            Hue = 0x47E;
            Resistances.Energy = 2;
        }

        public BootsOfTheCrystalHydra(Serial serial)
            : base(serial)
        {
        }

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