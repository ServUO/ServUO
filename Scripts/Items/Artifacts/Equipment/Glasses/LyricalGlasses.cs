using System;

namespace Server.Items
{
    public class LyricalGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LyricalGlasses()
        {
            this.WeaponAttributes.HitLowerDefend = 20;
            this.Attributes.NightSight = 1;
            this.Attributes.ReflectPhysical = 15;

            this.Hue = 0x47F;
        }

        public LyricalGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073382;
            }
        }//Lyrical Reading Glasses
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
                this.Hue = 0x47F;
        }
    }
}