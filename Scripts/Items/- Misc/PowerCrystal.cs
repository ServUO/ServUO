using System;
using Server.Network;

namespace Server.Items
{
    public class PowerCrystal : Item
    {
        [Constructable]
        public PowerCrystal()
            : base(0x1F1C)
        {
            this.Weight = 1.0;
        }

        public PowerCrystal(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "power crystal";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 3))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else
                from.SendAsciiMessage("This looks like part of a larger contraption.");
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