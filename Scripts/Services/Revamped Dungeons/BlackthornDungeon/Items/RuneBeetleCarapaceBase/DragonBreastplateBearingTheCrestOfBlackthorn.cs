namespace Server.Items
{
    public class DragonBreastplateBearingTheCrestOfBlackthorn : DragonChest
    {
        public override bool IsArtifact => true;

        [Constructable]
        public DragonBreastplateBearingTheCrestOfBlackthorn()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            this.Hue = 1773;
            this.Attributes.BonusMana = 10;
            this.Attributes.RegenMana = 3;
            this.Attributes.LowerManaCost = 15;
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
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