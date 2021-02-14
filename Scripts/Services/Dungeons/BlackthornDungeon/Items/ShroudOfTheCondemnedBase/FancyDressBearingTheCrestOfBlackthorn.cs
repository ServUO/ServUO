namespace Server.Items
{
    public class FancyDressBearingTheCrestOfBlackthorn1 : FancyDress
    {
        public override bool IsArtifact => true;

        [Constructable]
        public FancyDressBearingTheCrestOfBlackthorn1()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;

            Attributes.BonusHits = 3;
            Attributes.BonusInt = 5;
            Hue = 2075;
        }

        public FancyDressBearingTheCrestOfBlackthorn1(Serial serial)
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
