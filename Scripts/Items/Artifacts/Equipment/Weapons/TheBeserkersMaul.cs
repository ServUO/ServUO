using System;

namespace Server.Items
{
    public class TheBeserkersMaul : Maul
    {
        [Constructable]
        public TheBeserkersMaul()
        {
            this.Hue = 0x21;
            this.Attributes.WeaponSpeed = 75;
            this.Attributes.WeaponDamage = 50;
        }

        public TheBeserkersMaul(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061108;
            }
        }// The Berserker's Maul
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