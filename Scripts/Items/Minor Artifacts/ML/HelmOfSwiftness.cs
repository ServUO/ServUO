using System;

namespace Server.Items
{
    public class HelmOfSwiftness : WingedHelm
    {
        [Constructable]
        public HelmOfSwiftness()
            : base()
        {
            this.Hue = 0x592;
			
            this.Attributes.BonusInt = 5;
            this.Attributes.CastSpeed = 1;
            this.Attributes.CastRecovery = 2;
            this.ArmorAttributes.MageArmor = 1;
        }

        public HelmOfSwiftness(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075037;
            }
        }// Helm of Swiftness
        public override int BasePhysicalResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 6;
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
                return 8;
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}