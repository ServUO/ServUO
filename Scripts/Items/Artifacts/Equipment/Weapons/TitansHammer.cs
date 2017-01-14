using System;

namespace Server.Items
{
    public class TitansHammer : WarHammer
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TitansHammer()
        {
            this.Hue = 0x482;
            this.WeaponAttributes.HitEnergyArea = 100;
            this.Attributes.BonusStr = 15;
            this.Attributes.AttackChance = 15;
            this.Attributes.WeaponDamage = 50;
        }

        public TitansHammer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060024;
            }
        }// Titan's Hammer
        public override int ArtifactRarity
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}