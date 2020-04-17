namespace Server.Items
{
    public class BritchesOfWarding : ChainLegs
    {
        public override int LabelNumber => 1157345; // britches of warding
        public override bool IsArtifact => true;

        [Constructable]
        public BritchesOfWarding()
        {
            switch (Utility.Random(6))
            {
                case 0: AbsorptionAttributes.EaterKinetic = 9; break;
                case 1: AbsorptionAttributes.EaterFire = 9; break;
                case 2: AbsorptionAttributes.EaterCold = 9; break;
                case 3: AbsorptionAttributes.EaterPoison = 9; break;
                case 4: AbsorptionAttributes.EaterEnergy = 9; break;
                case 5: AbsorptionAttributes.EaterDamage = 9; break;
            }

            Attributes.BonusStam = 12;
            Attributes.AttackChance = 10;
            Attributes.LowerManaCost = 8;
        }

        public BritchesOfWarding(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 20;
        public override int BaseFireResistance => 20;
        public override int BaseColdResistance => 20;
        public override int BasePoisonResistance => 20;
        public override int BaseEnergyResistance => 20;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GargishBritchesOfWarding : GargishPlateLegs
    {
        public override int LabelNumber => 1157345; // britches of warding
        public override bool IsArtifact => true;

        [Constructable]
        public GargishBritchesOfWarding()
        {
            switch (Utility.Random(6))
            {
                case 0: AbsorptionAttributes.EaterKinetic = 9; break;
                case 1: AbsorptionAttributes.EaterFire = 9; break;
                case 2: AbsorptionAttributes.EaterCold = 9; break;
                case 3: AbsorptionAttributes.EaterPoison = 9; break;
                case 4: AbsorptionAttributes.EaterEnergy = 9; break;
                case 5: AbsorptionAttributes.EaterDamage = 9; break;
            }

            Attributes.BonusStam = 12;
            Attributes.AttackChance = 10;
            Attributes.LowerManaCost = 8;
        }

        public GargishBritchesOfWarding(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 20;
        public override int BaseFireResistance => 20;
        public override int BaseColdResistance => 20;
        public override int BasePoisonResistance => 20;
        public override int BaseEnergyResistance => 20;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}