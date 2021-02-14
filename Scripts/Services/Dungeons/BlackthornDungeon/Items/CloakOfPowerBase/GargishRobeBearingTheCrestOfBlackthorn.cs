namespace Server.Items
{
    public class GargishRobeBearingTheCrestOfBlackthorn4 : GargishRobe
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishRobeBearingTheCrestOfBlackthorn4()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusStr = 2;
            Attributes.BonusDex = 2;
            Attributes.BonusInt = 2;
            Hue = 2107;
        }

        public GargishRobeBearingTheCrestOfBlackthorn4(Serial serial)
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
