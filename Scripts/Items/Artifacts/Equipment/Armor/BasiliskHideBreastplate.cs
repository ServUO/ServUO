namespace Server.Items
{
    public class BasiliskHideBreastplate : DragonChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115444;  // Basilisk Hide Breastplate

        [Constructable]
        public BasiliskHideBreastplate()
        {
            Resource = CraftResource.None;
            Hue = 1366;
            AbsorptionAttributes.EaterDamage = 10;
            Attributes.BonusDex = 5;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 1;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 5;
        }

        public BasiliskHideBreastplate(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 12;
        public override int BaseFireResistance => 14;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 11;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Resource = CraftResource.None;
                Hue = 1366;
            }
        }
    }
}