using System;

namespace Server.Items
{
    public class NecromanticGlasses : ElvenGlasses
    {
        [Constructable]
        public NecromanticGlasses()
        {
            this.Attributes.LowerManaCost = 15;
            this.Attributes.LowerRegCost = 30;

            this.Hue = 0x22D;
        }

        public NecromanticGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073377;
            }
        }//Necromantic Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 0;
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
                return 0;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 0;
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

            if (version == 0 && this.Hue == 0)
                this.Hue = 0x22D;
        }
    }
}