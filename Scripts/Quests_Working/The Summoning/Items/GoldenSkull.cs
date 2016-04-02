using System;

namespace Server.Engines.Quests.Doom
{
    public class GoldenSkull : Item
    {
        [Constructable]
        public GoldenSkull()
            : base(Utility.Random(0x1AE2, 3))
        {
            this.Weight = 1.0;
            this.Hue = 0x8A5;
            this.LootType = LootType.Blessed;
        }

        public GoldenSkull(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061619;
            }
        }// a golden skull
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