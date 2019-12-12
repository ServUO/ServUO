using System;

namespace Server.Items
{
    public class DragonJadeEarrings : GargishEarrings
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113720; } } // Dragon Jade Earrings

        public override int BasePhysicalResistance { get { return 9; } }
        public override int BaseFireResistance { get { return 16; } }
        public override int BaseColdResistance { get { return 5; } }
        public override int BasePoisonResistance { get { return 13; } }
        public override int BaseEnergyResistance { get { return 3; } }

        [Constructable]
        public DragonJadeEarrings()
        {
            Hue = 2129;
            Attributes.BonusDex = 5;
            Attributes.BonusStr = 5;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.LowerManaCost = 5;
	        AbsorptionAttributes.EaterFire = 10;
        }

        public DragonJadeEarrings(Serial serial)
            : base(serial)
        {
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
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
