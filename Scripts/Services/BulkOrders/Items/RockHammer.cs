using Server.Engines.Harvest;

namespace Server.Items
{
    [Flipable(0x9E7E, 0x9E7F)]
    public class RockHammer : BaseHarvestTool
    {
        public override int LabelNumber => 1124598;

        [Constructable]
        public RockHammer()
            : this(500)
        {
        }

        [Constructable]
        public RockHammer(int uses)
            : base(uses, 0x9E7E)
        {
            Weight = 5.0;
        }

        public RockHammer(Serial serial)
            : base(serial)
        {
        }

        public override HarvestSystem HarvestSystem => Mining.System;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
