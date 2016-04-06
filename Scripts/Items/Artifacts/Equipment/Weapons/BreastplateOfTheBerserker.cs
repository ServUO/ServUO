using System;

namespace Server.Items
{
    public class BreastplateOfTheBerserker : GargishPlateChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BreastplateOfTheBerserker() 
        {
            this.Name = ("Breastplate Of The Berserker");
		
            this.Hue = 1172;	
			this.Weight = 10;
		
            this.Attributes.WeaponSpeed = 10;
            this.Attributes.WeaponDamage = 15;		
            this.Attributes.LowerManaCost = 4;
            this.Attributes.BonusHits = 5;			
			this.Attributes.RegenStam = 3;
			this.StrRequirement = 95;
        }

        public BreastplateOfTheBerserker(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 16;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
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
        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (((Mobile)parent).Female)
                    this.ItemID = 0x0309;
                else
                    this.ItemID = 0x030A;
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