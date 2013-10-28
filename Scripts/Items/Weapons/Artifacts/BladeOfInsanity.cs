using System;

namespace Server.Items
{
    public class BladeOfInsanity : Katana
    {
        [Constructable]
        public BladeOfInsanity()
        {
            this.Hue = 0x76D;
            this.WeaponAttributes.HitLeechStam = 100;
            this.Attributes.RegenStam = 2;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 50;
        }

        public BladeOfInsanity(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061088;
            }
        }// Blade of Insanity
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

            if (this.Hue == 0x44F)
                this.Hue = 0x76D;
        }
    }
}