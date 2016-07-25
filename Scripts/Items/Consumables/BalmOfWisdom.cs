using System;
using Server;

namespace Server.Items
{
    public class BalmOfWisdom : BalmOrLotion
    {
        public override int LabelNumber { get { return 1094941; } } // Balm of Wisdom
        public override int ApplyMessage { get { return 1095137; } } // You apply the balm and suddenly feel wiser!

        public static bool UnderEffect(Mobile m)
        {
            return GetActiveBalmFor(m) is BalmOfWisdom;
        }

        public override void AddBuff(Mobile m, TimeSpan duration)
        {
            m.AddStatMod(new StatMod(StatType.Int, "[BalmOrLotion] Int Offset", 10, duration));
        }

        public override void RemoveBuff(Mobile m)
        {
            m.RemoveStatMod("[BalmOrLotion] Int Offset");
        }

        [Constructable]
        public BalmOfWisdom()
            : base(0x1847)
        {
        }

        public BalmOfWisdom(Serial serial)
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