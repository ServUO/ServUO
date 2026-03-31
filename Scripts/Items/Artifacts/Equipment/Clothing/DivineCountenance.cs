using System;

namespace Server.Items
{
    public class DivineCountenance : HornedTribalMask
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DivineCountenance()
        {
            Hue = 0x482;
            Attributes.BonusInt = 8;
            Attributes.RegenMana = 2;
            Attributes.ReflectPhysical = 15;
            Attributes.LowerManaCost = 8;
            Attributes.DefendChance = 15;
            Attributes.LowerRegCost = 15;
            Attributes.SpellDamage = 15;
            Attributes.BonusMana = 8;
            SAAbsorptionAttributes.CastingFocus = 5;
        }

        public DivineCountenance(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061289;
            }
        }// Divine Countenance
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
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
                return 18;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 18;
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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Resistances.Physical = 0;
                        this.Resistances.Fire = 0;
                        this.Resistances.Cold = 0;
                        this.Resistances.Energy = 0;
                        break;
                    }
            }
        }
    }
}