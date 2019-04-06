using System;
using Server;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
    public class WritingDesk : CraftAddon
    {
        public override BaseAddonDeed Deed { get { return new WritingDeskDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0); } }
        public override CraftSystem CraftSystem { get { return DefInscription.CraftSystem; } }

        [Constructable]
        public WritingDesk(bool south, int uses)
        {
            if (south)
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 40938, 40939, 1124962, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40953, 1124962), 1, 0, 0);
            }
            else
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 40945, 40946, 1124962, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40952, 1124962), 0, -1, 0);
            }
        }

        public WritingDesk(Serial serial)
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

    public class WritingDeskDeed : CraftAddonDeed
    {
        public override int LabelNumber { get { return 1157989; } } // Enchanted Writing Desk
        public override BaseAddon Addon { get { return new WritingDesk(_South, UsesRemaining); } }

        private bool _South;

        [Constructable]
        public WritingDeskDeed() : this(0)
        {
        }

        [Constructable]
        public WritingDeskDeed(int uses) : base(uses)
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

        public WritingDeskDeed(Serial serial)
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