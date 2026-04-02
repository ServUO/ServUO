using System;

namespace Server.Items
{
    public class MaritimeGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MaritimeGlasses()
        {
            Attributes.Luck = 250;
            Attributes.NightSight = 1;
            Attributes.ReflectPhysical = 20;
            Attributes.LowerRegCost = 15;
            SkillBonuses.SetValues(0, SkillName.Fishing, 15.0);
            Hue = 0x581;
        }

        public MaritimeGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073364;
            }
        }//Maritime Reading Glasses
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
                return 15;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 30;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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
                this.Hue = 0x581;
        }
    }
}