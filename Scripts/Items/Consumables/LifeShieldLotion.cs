using System;
using Server;

namespace Server.Items
{
    public class LifeShieldLotion : BalmOrLotion
    {
        public override int LabelNumber { get { return 1094945; } } // Life Shield Lotion
        public override int ApplyMessage { get { return 1095143; } } // You apply the ointment and suddenly feel less vulnerable!

        public static bool UnderEffect(Mobile m)
        {
            return GetActiveBalmFor(m) is LifeShieldLotion;
        }

        [Constructable]
        public LifeShieldLotion()
            : base(0xEFC)
        {
        }

        public LifeShieldLotion(Serial serial)
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