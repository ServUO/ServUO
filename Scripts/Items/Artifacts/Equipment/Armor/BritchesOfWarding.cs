using System;

namespace Server.Items
{
    public class BritchesOfWarding : ChainLegs
	{
        public override int LabelNumber { get { return 1157345; } }// britches of warding
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BritchesOfWarding()
        {
            switch (Utility.Random(6))
            {
                case 0: this.AbsorptionAttributes.EaterKinetic = 9; break;
                case 1: this.AbsorptionAttributes.EaterFire = 9; break;
                case 2: this.AbsorptionAttributes.EaterCold = 9; break;
                case 3: this.AbsorptionAttributes.EaterPoison = 9; break;
                case 4: this.AbsorptionAttributes.EaterEnergy = 9; break;
                case 5: this.AbsorptionAttributes.EaterDamage = 9; break;
            }

            this.Attributes.BonusStam = 12;
            this.Attributes.AttackChance = 10;
            this.Attributes.LowerManaCost = 8;
        }

        public BritchesOfWarding(Serial serial)
            : base(serial)
        {
        }
        
        public override int BasePhysicalResistance { get { return 20; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 20; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int BaseEnergyResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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
        public override int LabelNumber { get { return 1157345; } }// britches of warding
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishBritchesOfWarding()
        {
            switch (Utility.Random(6))
            {
                case 0: this.AbsorptionAttributes.EaterKinetic = 9; break;
                case 1: this.AbsorptionAttributes.EaterFire = 9; break;
                case 2: this.AbsorptionAttributes.EaterCold = 9; break;
                case 3: this.AbsorptionAttributes.EaterPoison = 9; break;
                case 4: this.AbsorptionAttributes.EaterEnergy = 9; break;
                case 5: this.AbsorptionAttributes.EaterDamage = 9; break;
            }

            this.Attributes.BonusStam = 12;
            this.Attributes.AttackChance = 10;
            this.Attributes.LowerManaCost = 8;
        }

        public GargishBritchesOfWarding(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 20; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 20; } }
        public override int BasePoisonResistance { get { return 20; } }
        public override int BaseEnergyResistance { get { return 20; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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