using System;

namespace Server.Items
{
    public class SoulSeeker : RadiantScimitar
    {
        [Constructable]
        public SoulSeeker()
        {
            this.Hue = 0x38C;

            this.WeaponAttributes.HitLeechStam = 40;
            this.WeaponAttributes.HitLeechMana = 40;
            this.WeaponAttributes.HitLeechHits = 40;
            this.Attributes.WeaponSpeed = 60;
            this.Slayer = SlayerName.Repond;
        }

        public SoulSeeker(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075046;
            }
        }// Soul Seeker
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
            cold = 100;

            pois = fire = phys = nrgy = chaos = direct = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}