namespace Server.Items
{
    public class DupresShield : BaseShield
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DupresShield()
            : base(0x2B01)
        {
            LootType = LootType.Blessed;
            Weight = 6.0;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 1;
            SkillBonuses.SetValues(0, SkillName.Parry, 5);
        }

        public DupresShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075196;// Dupre’s Shield
        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 1;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int StrReq => 50;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}