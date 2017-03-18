using Server;
using System;

namespace Server.Items
{
    public class WandOfThunderingGlory : BaseWand
    {
        public override int LabelNumber { get { return 1116623; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public WandOfThunderingGlory() : base(WandEffect.None, 0, 0)
        {
            WeaponAttributes.HitLightning = 40;
            Attributes.AttackChance = 5;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 50;
            Attributes.SpellDamage = 10;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = nrgy = pois = direct = 0;
            chaos = 100;
        }

        public WandOfThunderingGlory(Serial serial)
            : base(serial)
        {
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