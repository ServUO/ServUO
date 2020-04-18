namespace Server.Items
{
    public class SamaritanRobe : Robe
    {
        public override bool IsArtifact => true;
        [Constructable]
        public SamaritanRobe()
        {
            Hue = 0x2a3;
        }

        public SamaritanRobe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094926;// Good Samaritan of Britannia [Replica]
        public override int BasePhysicalResistance => 5;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;
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