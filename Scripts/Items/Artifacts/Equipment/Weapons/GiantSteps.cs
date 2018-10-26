using System;

namespace Server.Items
{
    public class GiantSteps : GargishStoneLegs
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113537; } } // Giant Steps
	
        [Constructable]
        public GiantSteps()
            : base()
        {
            Hue = 656;				
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.WeaponDamage = 10;
        }

        public GiantSteps(Serial serial)
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
                return 4;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 12;
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