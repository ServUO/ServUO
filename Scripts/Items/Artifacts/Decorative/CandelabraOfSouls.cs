namespace Server.Items
{
    public class CandelabraOfSouls : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public CandelabraOfSouls()
            : base(0xB26)
        {
            Light = LightType.Circle225;
        }

        public CandelabraOfSouls(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063478;
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
