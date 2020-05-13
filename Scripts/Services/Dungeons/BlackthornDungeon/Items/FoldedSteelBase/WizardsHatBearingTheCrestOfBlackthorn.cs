namespace Server.Items
{
    public class WizardsHatBearingTheCrestOfBlackthorn3 : WizardsHat
    {
        public override bool IsArtifact => true;

        [Constructable]
        public WizardsHatBearingTheCrestOfBlackthorn3()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.NightSight = 1;
            Attributes.BonusStr = 8;
            Attributes.DefendChance = 15;
            StrRequirement = 45;
            Hue = 1150;
        }

        public override int BasePhysicalResistance => 20;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public WizardsHatBearingTheCrestOfBlackthorn3(Serial serial)
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