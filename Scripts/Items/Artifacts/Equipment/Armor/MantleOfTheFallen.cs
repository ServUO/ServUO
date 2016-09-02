using System;

namespace Server.Items
{
    public class MantleOfTheFallen : GargishClothChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MantleOfTheFallen() 
        {
            this.Name = ("Mantle Of The Fallen");
		
            this.Hue = 1512;	
			this.Weight = 6;
		
            this.Attributes.LowerRegCost = 25;
            this.Attributes.BonusInt = 8;
            this.Attributes.BonusMana = 8;
            this.Attributes.RegenMana = 1;
            this.SAAbsorptionAttributes.CastingFocus = 3;
            this.Attributes.SpellDamage = 5;
			this.StrRequirement = 25;
        }

        public MantleOfTheFallen(Serial serial)
            : base(serial)
        {
        }

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
                return 8;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
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
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}