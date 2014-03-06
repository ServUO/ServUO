using System;

namespace Server.Items
{
    public class DragonHideShield : GargishKiteShield
    {
        [Constructable]
        public DragonHideShield()
            : base()
        {
			this.Name = "Dragon Hide Shield";
			
			this.Hue = 44;
		
            this.AbsorptionAttributes.EaterFire = 20;
            this.Attributes.RegenHits = 2;
            this.Attributes.DefendChance = 10;
        }

        public DragonHideShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 0;
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
                return -4;
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
		
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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