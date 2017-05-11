using System;

namespace Server.Items
{
    public class VioletCourage : FemalePlateChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VioletCourage()
        {
            this.Hue = Utility.RandomBool() ? 0x486 : 0x490;
            this.Attributes.Luck = 95;
            this.Attributes.DefendChance = 15;
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
        }

        public VioletCourage(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063471;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 14;
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
                return 12;
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