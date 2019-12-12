using System;

namespace Server.Items
{
    public class BrambleCoat : WoodlandChest
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BrambleCoat()
        {
            Hue = 0x1;
            ArmorAttributes.SelfRepair = 3;
            Attributes.BonusHits = 4;
            Attributes.Luck = 150;
            Attributes.ReflectPhysical = 25;
            Attributes.DefendChance = 15;
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