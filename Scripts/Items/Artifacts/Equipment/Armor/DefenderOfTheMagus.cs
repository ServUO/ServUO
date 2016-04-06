using System;

namespace Server.Items
{
    public class DefenderOfTheMagus : MetalShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DefenderOfTheMagus() 
        {
            this.Name = ("Defender Of The Magus");
		
            this.Hue = 590;
			
            this.Attributes.SpellChanneling = 1;
            this.Attributes.DefendChance = 10;				
            this.Attributes.CastRecovery = 1;
            //Random Resonance:
            switch (Utility.Random(5))
            {
                case 0:
                    this.AbsorptionAttributes.ResonanceCold = 10;
                    break;
                case 1:
                    this.AbsorptionAttributes.ResonanceFire = 10;
                    break;
                case 2:
                    this.AbsorptionAttributes.ResonanceKinetic = 10;
                    break;
                case 3:
                    this.AbsorptionAttributes.ResonancePoison = 10;
                    break;
                case 4:
                    this.AbsorptionAttributes.ResonanceEnergy = 10;
                    break;
            }
            //Random Resist:
            switch (Utility.Random(5))
            {
                case 0:
                    this.ColdBonus = 10;
                    break;
                case 1:
                    this.FireBonus = 10;
                    break;
                case 2:
                    this.PhysicalBonus = 10;
                    break;
                case 3:
                    this.PoisonBonus = 10;
                    break;
                case 4:
                    this.EnergyBonus = 10;
                    break;
            }

        }

        public DefenderOfTheMagus(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 1;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 0;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}