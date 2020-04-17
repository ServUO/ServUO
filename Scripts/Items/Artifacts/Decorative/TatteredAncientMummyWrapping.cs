namespace Server.Items
{
    public class TatteredAncientMummyWrapping : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TatteredAncientMummyWrapping()
            : base(0xE21)
        {
            Hue = 0x909;
        }

        public TatteredAncientMummyWrapping(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094912;// Tattered Ancient Mummy Wrapping [Replica]
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