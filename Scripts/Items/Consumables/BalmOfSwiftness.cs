using System;
using Server;

namespace Server.Items
{
    public class BalmOfSwiftness : BalmOrLotion
    {
        public override int LabelNumber { get { return 1094942; } } // Balm of Swiftness
        public override int ApplyMessage { get { return 1095138; } } // You apply the balm and suddenly feel more agile!

        public static bool UnderEffect(Mobile m)
        {
            return GetActiveBalmFor(m) is BalmOfSwiftness;
        }

        public override void AddBuff(Mobile m, TimeSpan duration)
        {
            m.AddStatMod(new StatMod(StatType.Dex, "[BalmOrLotion] Dex Offset", 10, duration));
        }

        public override void RemoveBuff(Mobile m)
        {
            m.RemoveStatMod("[BalmOrLotion] Dex Offset");
        }

        [Constructable]
        public BalmOfSwiftness()
            : base(0x1848)
        {
        }

        public BalmOfSwiftness(Serial serial)
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