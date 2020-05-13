namespace Server.Items
{
    public class ObiBearingTheCrestOfBlackthorn1 : Obi
    {
        public override bool IsArtifact => true;

        [Constructable]
        public ObiBearingTheCrestOfBlackthorn1()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.BonusInt = 10;
            Attributes.RegenMana = 2;
            Attributes.DefendChance = 5;
            StrRequirement = 10;
            Hue = 2527;
        }

        public ObiBearingTheCrestOfBlackthorn1(Serial serial)
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