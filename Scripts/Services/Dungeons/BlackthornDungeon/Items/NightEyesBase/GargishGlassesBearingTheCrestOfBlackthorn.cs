namespace Server.Items
{
    [Flipable(0x4644, 0x4645)]
    public class GargishGlassesBearingTheCrestOfBlackthorn1 : GargishGlasses
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishGlassesBearingTheCrestOfBlackthorn1()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.NightSight = 1;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 3;
            Hue = 1233;
        }

        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GargishGlassesBearingTheCrestOfBlackthorn1(Serial serial)
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