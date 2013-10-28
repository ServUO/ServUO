using System;

namespace Server.Items
{
    public class BladeOfTheRighteous : Longsword
    {
        [Constructable]
        public BladeOfTheRighteous()
        {
            this.Hue = 0x47E;
            //Slayer = SlayerName.DaemonDismissal;
            this.Slayer = SlayerName.Exorcism;
            this.WeaponAttributes.HitLeechHits = 50;
            this.WeaponAttributes.UseBestSkill = 1;
            this.Attributes.BonusHits = 10;
            this.Attributes.WeaponDamage = 50;
        }

        public BladeOfTheRighteous(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061107;
            }
        }// Blade of the Righteous
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

            if (this.Slayer == SlayerName.None)
                this.Slayer = SlayerName.Exorcism;
        }
    }
}