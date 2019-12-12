using System;

namespace Server.Items
{
    public class LeggingsOfEmbers : PlateLegs
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LeggingsOfEmbers()
        {
            Hue = 0x2C;
            LootType = LootType.Blessed;
            ArmorAttributes.SelfRepair = 10;
            ArmorAttributes.MageArmor = 1;
            ArmorAttributes.LowerStatReq = 100;
        }

        public LeggingsOfEmbers(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062911;
            }
        }// Royal Leggings of Embers
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
                return 25;
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
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}