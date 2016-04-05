using System;

namespace Server.Items
{
    public class BrambleCoat : WoodlandChest
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BrambleCoat()
        {
            this.Hue = 0x1;

            this.ArmorAttributes.SelfRepair = 3;
            this.Attributes.BonusHits = 4;
            this.Attributes.Luck = 150;
            this.Attributes.ReflectPhysical = 25;
            this.Attributes.DefendChance = 15;
        }

        public BrambleCoat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072925;
            }
        }// Bramble Coat
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
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
                return 7;
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
                return 7;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}