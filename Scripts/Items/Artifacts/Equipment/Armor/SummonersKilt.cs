using System;

namespace Server.Items
{
    public class SummonersKilt : GargishClothKilt
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SummonersKilt()
        {
            this.Name = ("Summoner's Kilt");
		
            this.Hue = 1266;
			
            this.Attributes.BonusMana = 5;
            this.Attributes.RegenMana = 2;
            this.Attributes.SpellDamage = 5;
            SAAbsorptionAttributes.CastingFocus = 2;
            this.Attributes.LowerManaCost = 8;
            this.Attributes.LowerRegCost = 10;
			this.StrRequirement = 20;
        }

        public SummonersKilt(Serial serial)
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
                return 7;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 21;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 21;
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