using Server.Engines.Craft;

namespace Server.Items
{
    public class WritingDesk : CraftAddon
    {
        public override BaseAddonDeed Deed => new WritingDeskDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0);
        public override CraftSystem CraftSystem => DefInscription.CraftSystem;

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

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WritingDeskDeed : CraftAddonDeed
    {
        public override int LabelNumber => 1157989;  // Enchanted Writing Desk
        public override BaseAddon Addon => new WritingDesk(_South, UsesRemaining);

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

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}