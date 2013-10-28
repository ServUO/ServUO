using System;

namespace Server.Items
{
    public class SamplesOfCorruptedWater : Item
    {
        [Constructable]
        public SamplesOfCorruptedWater()
            : base(0xEFE)
        {
            this.LootType = LootType.Blessed;
        }

        public SamplesOfCorruptedWater(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074999;
            }
        }// samples of corrupted water
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