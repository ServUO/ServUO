using System;

namespace Server.Items
{
    public class InquisitorsResolution : PlateGloves
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public InquisitorsResolution()
        {
            this.Hue = 0x4F2;
            this.Attributes.CastRecovery = 3;
            this.Attributes.LowerManaCost = 8;
            this.ArmorAttributes.MageArmor = 1;
        }

        public InquisitorsResolution(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060206;
            }
        }// The Inquisitor's Resolution
        public override int ArtifactRarity
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
                return 22;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 17;
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

            if (version < 1)
            {
                this.ColdBonus = 0;
                this.EnergyBonus = 0;
            }
        }
    }
}