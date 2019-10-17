using System;
using Server.Multis;

namespace Server.Mobiles
{
    public class CommissionPlayerVendor : PlayerVendor
    {
        public override bool IsCommission { get { return true; } }

        public CommissionPlayerVendor(Mobile owner, BaseHouse house)
           : base(owner, house)
        {
        }

        public CommissionPlayerVendor(Serial serial)
            : base(serial)
        {
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
    }
}
