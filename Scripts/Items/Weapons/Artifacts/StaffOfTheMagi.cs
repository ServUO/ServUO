using System;

namespace Server.Items
{
    public class StaffOfTheMagi : BlackStaff
    {
        [Constructable]
        public StaffOfTheMagi()
        {
            this.Hue = 0x481;
            this.WeaponAttributes.MageWeapon = 30;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = 1;
            this.Attributes.WeaponDamage = 50;
        }

        public StaffOfTheMagi(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061600;
            }
        }// Staff of the Magi
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
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = pois = chaos = direct = 0;
            nrgy = 100;
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

            if (this.WeaponAttributes.MageWeapon == 0)
                this.WeaponAttributes.MageWeapon = 30;

            if (this.ItemID == 0xDF1)
                this.ItemID = 0xDF0;
        }
    }
}