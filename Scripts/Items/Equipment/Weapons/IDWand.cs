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

            int version = reader.ReadInt();
        }

        public override bool OnWandTarget(Mobile from, object o)
        {
            if (o is BaseWeapon)
                ((BaseWeapon)o).Identified = true;
            else if (o is BaseArmor)
                ((BaseArmor)o).Identified = true;

            return (o is Item);
        }
    }
}
