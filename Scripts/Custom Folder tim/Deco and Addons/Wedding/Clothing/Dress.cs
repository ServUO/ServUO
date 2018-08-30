using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    [Flipable(40687, 40688)]
    public class WeddingDress : BaseOuterTorso
    {
        [Constructable]
        public WeddingDress()
            : this(0)
        {
        }

        [Constructable]
        public WeddingDress(int hue)
            : base(40687, hue)
        {
            Name = "Wedding Dress";
			LootType = LootType.Blessed;
			this.Weight = 2.0;
        }

        public override bool AllowMaleWearer
        {
            get
            {
                return false;
            }
        }

        public WeddingDress(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}