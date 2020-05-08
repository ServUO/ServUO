namespace Server.Items
{
    public class BurglarsBandana : Bandana
    {
        public override bool IsArtifact => true;
		public override int LabelNumber => 1063473;
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
		
        [Constructable]
        public BurglarsBandana()
        {
            Hue = Utility.RandomBool() ? 0x58C : 0x10;
            SkillBonuses.SetValues(0, SkillName.Stealing, 10.0);
            SkillBonuses.SetValues(1, SkillName.Stealth, 10.0);
            SkillBonuses.SetValues(2, SkillName.Snooping, 10.0);
            Attributes.BonusDex = 5;
        }

        public BurglarsBandana(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}