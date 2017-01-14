using System;

namespace Server.Items
{
    public class VoidInfusedKilt : GargishPlateKilt
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VoidInfusedKilt()
            : base()
        {
            this.Name = ("Void Infused Kilt");
		
            this.Hue = 2124;
			
            this.Attributes.AttackChance = 5;			
            this.Attributes.BonusStr = 5;	
            this.Attributes.BonusDex = 5;
            this.Attributes.RegenMana = 1;
            this.Attributes.RegenStam = 1;
            this.AbsorptionAttributes.EaterDamage = 10;
			this.StrRequirement = 80;
        }

        public VoidInfusedKilt(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 13;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 9;
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
                    this.ItemID = 0x030B;
                else
                    this.ItemID = 0x030C;
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