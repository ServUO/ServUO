using System;
using Server;

namespace Server.Items
{
    public class BalmOfStrength : BalmOrLotion
    {
        public override int LabelNumber { get { return 1094940; } } // Balm of Strength
        public override int ApplyMessage { get { return 1095136; } } // You apply the balm and suddenly feel stronger!

        public static bool UnderEffect(Mobile m)
        {
            return GetActiveBalmFor(m) is BalmOfStrength;
        }

        public override void AddBuff(Mobile m, TimeSpan duration)
        {
            m.AddStatMod(new StatMod(StatType.Str, "[BalmOrLotion] Str Offset", 10, duration));
        }

        public override void RemoveBuff(Mobile m)
        {
            m.RemoveStatMod("[BalmOrLotion] Str Offset");
        }

        [Constructable]
        public BalmOfStrength()
            : base(0xEFB)
        {
        }

        public BalmOfStrength(Serial serial)
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