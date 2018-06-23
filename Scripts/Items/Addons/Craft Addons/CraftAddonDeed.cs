using System;
using Server;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Multis;
using Server.Mobiles;
using System.Linq;

namespace Server.Items
{
    public abstract class CraftAddonDeed : BaseAddonDeed
    {
        public int UsesRemaining { get; set; }

        public CraftAddonDeed(int uses)
        {
            LootType = LootType.Blessed;
            UsesRemaining = uses;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, UsesRemaining.ToString());
        }

        public CraftAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            UsesRemaining = reader.ReadInt();
        }
    }
}
