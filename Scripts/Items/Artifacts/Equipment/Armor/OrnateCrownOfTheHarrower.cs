using System;

namespace Server.Items
{
    public class OrnateCrownOfTheHarrower : BoneHelm
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public OrnateCrownOfTheHarrower()
        {
            Hue = 0x4F6;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.WeaponDamage = 25;
        }

        public OrnateCrownOfTheHarrower(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061095;
            }
        }// Ornate Crown of the Harrower
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePoisonResistance
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
                if (this.Hue == 0x55A)
                    this.Hue = 0x4F6;

                this.PoisonBonus = 0;
            }
        }
    }
}