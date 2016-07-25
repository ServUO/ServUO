using System;
using Server;

namespace Server.Items
{
    public class BalmOfProtection : BalmOrLotion
    {
        public override int LabelNumber { get { return 1094943; } } // Balm of Protection
        public override int ApplyMessage { get { return 1095143; } } // You apply the ointment and suddenly feel less vulnerable!

        public static bool UnderEffect(Mobile m)
        {
            return GetActiveBalmFor(m) is BalmOfProtection;
        }

        [Constructable]
        public BalmOfProtection()
            : base(0x1C18)
        {
            Hue = 0x499;
        }

        public BalmOfProtection(Serial serial)
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