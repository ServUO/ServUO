using System;

namespace Server.Items
{
    public class SilvanisFeywoodBow : ElvenCompositeLongbow
    {
        [Constructable]
        public SilvanisFeywoodBow()
        {
            this.Hue = 0x1A;

            this.Attributes.SpellChanneling = 1;
            this.Attributes.AttackChance = 12;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 35;
        }

        public SilvanisFeywoodBow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072955;
            }
        }// Silvani's Feywood Bow
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = pois = chaos = direct = 0;
            nrgy = 100;
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