namespace Server.Items
{
    public class GargishFancyBearingTheCrestOfBlackthorn1 : GargishFancyRobe
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishFancyBearingTheCrestOfBlackthorn1()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusHits = 3;
            Attributes.BonusInt = 5;
            Hue = 2075;
        }

        public GargishFancyBearingTheCrestOfBlackthorn1(Serial serial)
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
            int version = reader.ReadInt();

            if (version == 0)
            {
                MaxHitPoints = 0;
                HitPoints = 0;
            }
        }
    }
}