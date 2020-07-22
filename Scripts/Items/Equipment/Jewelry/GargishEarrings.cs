namespace Server.Items
{
    public class GargishEarrings : BaseArmor
    {
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Chainmail;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 2;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 2;
        public override int BaseEnergyResistance => 3;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 40;

        [Constructable]
        public GargishEarrings()
            : base(0x4213)
        {
            Layer = Layer.Earrings;
        }

        public override int GetDurabilityBonus()
        {
            int bonus = Quality == ItemQuality.Exceptional ? 20 : 0;

            return bonus + ArmorAttributes.DurabilityBonus;
        }

        protected override void ApplyResourceResistances(CraftResource oldResource)
        {
        }

        public GargishEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
