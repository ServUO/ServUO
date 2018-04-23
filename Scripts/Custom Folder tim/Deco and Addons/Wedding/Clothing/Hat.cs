using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
    [Flipable(40697, 40698)]
    public class TopHat : BaseHat
    {
        [Constructable]
        public TopHat()
            : this(0)
        {
        }

        [Constructable]
        public TopHat(int hue)
            : base(40697, hue)
        {
            Name = "Top Hat";
			LootType = LootType.Blessed;
			this.Weight = 3.0;
        }

        public TopHat(Serial serial)
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