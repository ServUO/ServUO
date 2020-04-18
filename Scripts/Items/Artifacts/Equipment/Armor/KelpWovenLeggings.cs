namespace Server.Items
{
    public class KelpWovenLeggings : LeatherLegs
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1149960;

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 13;
        public override int BaseColdResistance => 12;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 14;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public KelpWovenLeggings()
        {
            Hue = 1155;
            AbsorptionAttributes.CastingFocus = 4;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 8;
            Attributes.LowerRegCost = 15;
        }

        public KelpWovenLeggings(Serial serial)
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