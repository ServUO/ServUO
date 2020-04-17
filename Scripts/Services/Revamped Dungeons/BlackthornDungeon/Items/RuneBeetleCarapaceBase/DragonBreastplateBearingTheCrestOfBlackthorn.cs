namespace Server.Items
{
    public class DragonBreastplateBearingTheCrestOfBlackthorn : DragonChest
    {
        public override bool IsArtifact => true;

        [Constructable]
        public DragonBreastplateBearingTheCrestOfBlackthorn()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Hue = 1773;
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 3;
            Attributes.LowerManaCost = 15;
            ArmorAttributes.LowerStatReq = 100;
            ArmorAttributes.MageArmor = 1;
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 14;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 14;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public DragonBreastplateBearingTheCrestOfBlackthorn(Serial serial)
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