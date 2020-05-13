namespace Server.Items
{
    public class GargishSashBearingTheCrestOfBlackthorn : GargishSash
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishSashBearingTheCrestOfBlackthorn()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusInt = 5;
            Attributes.RegenMana = 2;
            Attributes.LowerRegCost = 10;
            StrRequirement = 10;
            Hue = 0xe8;
        }

        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        public GargishSashBearingTheCrestOfBlackthorn(Serial serial)
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