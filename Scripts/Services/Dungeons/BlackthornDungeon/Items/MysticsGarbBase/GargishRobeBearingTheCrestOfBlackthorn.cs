namespace Server.Items
{
    public class GargishRobeBearingTheCrestOfBlackthorn2 : GargishRobe
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishRobeBearingTheCrestOfBlackthorn2()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.LowerManaCost = 1;
            Attributes.BonusMana = 5;
            Hue = 1306;
        }

        public GargishRobeBearingTheCrestOfBlackthorn2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
