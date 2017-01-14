using System;

namespace Server.Items
{
    public class ArtsGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArtsGlasses()
        {
            this.Attributes.BonusStr = 5;
            this.Attributes.BonusInt = 5;
            this.Attributes.BonusHits = 15;

            this.Hue = 0x73;
        }

        public ArtsGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073363;
            }
        }//Reading Glasses of the Arts
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
                return 8;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 4;
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
                this.Hue = 0x73;
        }
    }
}