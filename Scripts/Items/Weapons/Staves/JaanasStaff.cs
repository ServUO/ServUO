using System;

namespace Server.Items
{
    public class JaanasStaff : GnarledStaff
    {
        [Constructable]
        public JaanasStaff()
            : base()
        {
            this.Hue = 0x58C;

            this.WeaponAttributes.MageWeapon = 20;

            this.Attributes.SpellChanneling = 1;
            this.Attributes.Luck = 220;
            this.Attributes.DefendChance = 15;
        }

        public JaanasStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1079790;
            }
        }// Jaana's Staff
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
                return 120;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 120;
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