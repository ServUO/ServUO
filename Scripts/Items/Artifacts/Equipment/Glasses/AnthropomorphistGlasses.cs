using System;

namespace Server.Items
{
    public class AnthropomorphistGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AnthropomorphistGlasses()
        {
            this.Attributes.BonusHits = 5;
            this.Attributes.RegenMana = 3;
            this.Attributes.ReflectPhysical = 20;

            this.Hue = 0x80;
        }

        public AnthropomorphistGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073379;
            }
        }//Anthropomorphist Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
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
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 20;
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
                this.Hue = 0x80;
        }
    }
}