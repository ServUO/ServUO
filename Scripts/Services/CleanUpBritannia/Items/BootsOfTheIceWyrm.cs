namespace Server.Items
{
    public class BootsOfTheIceWyrm : Boots
    {
        public override int LabelNumber => 1151208;  // Boots of the Ice Wyrm
        public override bool IsArtifact => true;

        [Constructable]
        public BootsOfTheIceWyrm()
        {
            Hue = 0x482;
            Resistances.Cold = 2;
        }

        public BootsOfTheIceWyrm(Serial serial)
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