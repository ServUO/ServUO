namespace Server.Items
{
    public class GrapeBunch : Food
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1022513;

        [Constructable]
        public GrapeBunch() : base(1, 3354)
        {
            Weight = 1.0;
            FillFactor = 1;
            Stackable = false;
        }

        public GrapeBunch(Serial serial)
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