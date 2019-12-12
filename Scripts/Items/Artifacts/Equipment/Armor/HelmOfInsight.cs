using System;

namespace Server.Items
{
    public class HelmOfInsight : PlateHelm
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HelmOfInsight()
        {
            Hue = 0x554;
            Attributes.BonusInt = 8;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 8;
        }

        public HelmOfInsight(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061096;
            }
        }// Helm of Insight
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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
                this.EnergyBonus = 0;
        }
    }
}