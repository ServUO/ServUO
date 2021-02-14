using System;

namespace Server.Items
{
    public class IDWand : BaseWand
    {
        [Constructable]
        public IDWand()
            : base(WandEffect.Identification, 25, 175)
        {
        }

        public IDWand(Serial serial)
            : base(serial)
        {
        }

        public override TimeSpan GetUseDelay => TimeSpan.Zero;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        public override bool OnWandTarget(Mobile from, object o)
        {
            if (o is BaseWeapon weapon)
                weapon.Identified = true;
            else if (o is BaseArmor armor)
                armor.Identified = true;

            return o is Item;
        }
    }
}
