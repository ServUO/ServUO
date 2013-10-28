using System;

namespace Server.Items
{
    public class ColdForgedBlade : ElvenSpellblade
    {
        [Constructable]
        public ColdForgedBlade()
        {
            this.WeaponAttributes.HitHarm = 40;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.NightSight = 1;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 50;

            this.Hue = this.GetElementalDamageHue();
        }

        public ColdForgedBlade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072916;
            }
        }// Cold Forged Blade
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = pois = nrgy = chaos = direct = 0;
            cold = 100;
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