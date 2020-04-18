namespace Server.Items
{
    public class LeviathanHideBracers : LeatherArms
    {
        public override int LabelNumber => 1116619;

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 9;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 13;
        public override int BaseEnergyResistance => 14;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public LeviathanHideBracers()
        {
            Hue = 1274;

            AbsorptionAttributes.CastingFocus = 2;
            Attributes.BonusInt = 6;
            Attributes.AttackChance = 5;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;
        }

        public LeviathanHideBracers(Serial serial)
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