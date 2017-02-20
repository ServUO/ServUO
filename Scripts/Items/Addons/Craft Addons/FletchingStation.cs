using System;
using Server;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
    public class FletchingStation : CraftAddon
    {
        public override BaseAddonDeed Deed { get { return new FletchingStationDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0); } }
        public override CraftSystem CraftSystem { get { return DefBowFletching.CraftSystem; } }

        [Constructable]
        public FletchingStation(bool south, int uses)
        {
            if (south)
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 39982, 39983, 1124006, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40004, 1124006), -1, 0, 0);
            }
            else
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 39992, 39993, 1124006, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40003, 1124006), 1, 0, 0);
            }
        }

        public FletchingStation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FletchingStationDeed : CraftAddonDeed
    {
        public override int LabelNumber { get { return 1156370; } } // Fletching Station
        public override BaseAddon Addon { get { return new FletchingStation(_South, UsesRemaining); } }

        private bool _South;

        [Constructable]
        public FletchingStationDeed() : this(0)
        {
        }

        [Constructable]
        public FletchingStationDeed(int uses) : base(uses)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new SouthEastGump(s =>
                {
                    _South = s;
                    base.OnDoubleClick(from);
                }));
            }
        }

        public FletchingStationDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}