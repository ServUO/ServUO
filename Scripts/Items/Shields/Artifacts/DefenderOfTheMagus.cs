using System;

namespace Server.Items
{
    public class DefenderOfTheMagus : MetalShield
    {
        [Constructable]
        public DefenderOfTheMagus() 
        {
            this.Name = ("Defender Of The Magus");
		
            this.Hue = 590;
			
            this.Attributes.SpellChanneling = 1;
            this.Attributes.DefendChance = 10;				
            this.Attributes.CastRecovery = 1;
            //TODO: Random Resonance, Random Resistance
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