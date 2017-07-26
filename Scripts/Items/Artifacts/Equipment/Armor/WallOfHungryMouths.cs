using System;

namespace Server.Items
{
    public class WallofHungryMouths : HeaterShield
    {
	public override bool IsArtifact { get { return true; } }
	
        [Constructable]
        public WallofHungryMouths()
        {
            this.Weight = 8.0;
            this.Hue = 1034;

            this.AbsorptionAttributes.EaterEnergy = 20;
            this.AbsorptionAttributes.EaterPoison = 20;
            this.AbsorptionAttributes.EaterCold = 20;
            this.AbsorptionAttributes.EaterFire = 20;
        }

        public WallofHungryMouths(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113722;
            }
        }// Wall of Hungry Mouths
        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
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
        public override int AosStrReq
        {
            get
            {
                return 90;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 23;
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
