using System;

namespace Server.Items
{
    public class OrcChieftainHelm : OrcHelm
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public OrcChieftainHelm()
        {
            this.Hue = 0x2a3;

            this.Attributes.Luck = 100;
            this.Attributes.RegenHits = 3;

            if (Utility.RandomBool())
                this.Attributes.BonusHits = 30;
            else
                this.Attributes.AttackChance = 30;
        }

        public OrcChieftainHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094924;
            }
        }// Orc Chieftain Helm [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 23;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 1;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 23;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
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

            if (version < 1 && this.Hue == 0x3f) /* Pigmented? */
            {
                this.Hue = 0x2a3;
            }
        }
    }
}