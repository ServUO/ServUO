using Server.Engines.Craft;

namespace Server.Items
{
    public class SmithingPress : CraftAddon
    {
        public override BaseAddonDeed Deed => new SmithingPressDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0);
        public override CraftSystem CraftSystem => DefBlacksmithy.CraftSystem;

        [Constructable]
        public SmithingPress(bool south, int uses)
        {
            if (south)
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 39592, 39553, 1123577, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(39569, 1123593), -1, 0, 0);
            }
            else
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 39593, 39561, 1123577, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(39569, 1123593), 0, 1, 0);
            }
        }

        public SmithingPress(Serial serial)
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

    public class SmithingPressDeed : CraftAddonDeed
    {
        public override int LabelNumber => 1123577;  // smithing press
        public override BaseAddon Addon => new SmithingPress(_South, UsesRemaining);

        private bool _South;

        [Constructable]
        public SmithingPressDeed() : this(0)
        {
        }

        [Constructable]
        public SmithingPressDeed(int uses) : base(uses)
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

        public SmithingPressDeed(Serial serial)
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