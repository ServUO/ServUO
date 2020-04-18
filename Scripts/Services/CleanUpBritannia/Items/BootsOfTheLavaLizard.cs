namespace Server.Items
{
    public class BootsOfTheLavaLizard : Boots
    {
        public override int LabelNumber => 1151207;  // Boots Of The Lava Lizard
        public override bool IsArtifact => true;

        [Constructable]
        public BootsOfTheLavaLizard()
        {
            Hue = 0x674;
            Resistances.Fire = 2;
        }

        public BootsOfTheLavaLizard(Serial serial)
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