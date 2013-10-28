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

        public override TimeSpan GetUseDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
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

            if (!Core.AOS && o is Item)
                ((Item)o).OnSingleClick(from);

            return (o is Item);
        }
    }
}