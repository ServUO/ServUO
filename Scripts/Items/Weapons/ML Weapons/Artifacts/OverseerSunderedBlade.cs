using System;

namespace Server.Items
{
    public class OverseerSunderedBlade : RadiantScimitar
    {
        [Constructable]
        public OverseerSunderedBlade()
        {
            this.ItemID = 0x2D27;
            this.Hue = 0x485;

            this.Attributes.RegenStam = 2;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 45;

            this.Hue = this.GetElementalDamageHue();
        }

        public OverseerSunderedBlade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072920;
            }
        }// Overseer Sundered Blade
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = cold = pois = nrgy = chaos = direct = 0;
            fire = 100;
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