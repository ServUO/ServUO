using System;

namespace Server.Items
{
    public class FoldedSteelGlasses : ElvenGlasses
    {
        [Constructable]
        public FoldedSteelGlasses()
        {
            this.Attributes.BonusStr = 8;
            this.Attributes.NightSight = 1;
            this.Attributes.DefendChance = 15;

            this.Hue = 0x47E;
        }

        public FoldedSteelGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073380;
            }
        }//Folded Steel Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
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
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && this.Hue == 0)
                this.Hue = 0x47E;
        }
    }
}