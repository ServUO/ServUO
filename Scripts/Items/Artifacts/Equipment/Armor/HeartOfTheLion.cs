using System;

namespace Server.Items
{
    public class HeartOfTheLion : PlateChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HeartOfTheLion()
        {
            this.Hue = 0x501;
            this.Attributes.Luck = 95;
            this.Attributes.DefendChance = 15;
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
        }

        public HeartOfTheLion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070817;
            }
        }// Heart of the Lion
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
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
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