using System;

namespace Server.Items
{
    public class AxeOfTheHeavens : DoubleAxe
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AxeOfTheHeavens()
        {
            this.Hue = 0x4D5;
            this.WeaponAttributes.HitLightning = 50;
            this.Attributes.AttackChance = 15;
            this.Attributes.DefendChance = 15;
            this.Attributes.WeaponDamage = 50;
        }

        public AxeOfTheHeavens(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061106;
            }
        }// Axe of the Heavens
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}