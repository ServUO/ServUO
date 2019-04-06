using System;
using Server;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
    public class AlchemyStation : CraftAddon
    {
        public override BaseAddonDeed Deed { get { return new AlchemyStationDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0); } }
        public override CraftSystem CraftSystem { get { return DefAlchemy.CraftSystem; } }

        [Constructable]
        public AlchemyStation(bool south, int uses)
        {
            if (south)
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 40323, 40324, 1157070, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40332, 1157070), 1, 0, 0);
            }
            else
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 40334, 40335, 1157070, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40343, 1157070), 0, -1, 0);
            }
        }

        public AlchemyStation(Serial serial)
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

    public class AlchemyStationDeed : CraftAddonDeed
    {
        public override int LabelNumber { get { return 1157070; } } // Alchemy Station
        public override BaseAddon Addon { get { return new AlchemyStation(_South, UsesRemaining); } }

        private bool _South;

        [Constructable]
        public AlchemyStationDeed() : this(0)
        {
        }

        [Constructable]
        public AlchemyStationDeed(int uses) : base(uses)
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

        public AlchemyStationDeed(Serial serial)
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