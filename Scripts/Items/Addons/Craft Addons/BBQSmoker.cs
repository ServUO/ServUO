using Server.Engines.Craft;

namespace Server.Items
{
    public class BBQSmoker : CraftAddon
    {
        public override BaseAddonDeed Deed => new BBQSmokerDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0);
        public override CraftSystem CraftSystem => DefCooking.CraftSystem;

        [Constructable]
        public BBQSmoker(bool south, int uses)
        {
            if (south)
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 40344, 40345, 1157071, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40349, 1157071), -1, 0, 0);
            }
            else
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 40350, 40351, 1157071, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40355, 1157071), 0, -1, 0);
            }
        }

        public BBQSmoker(Serial serial)
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

    public class BBQSmokerDeed : CraftAddonDeed
    {
        public override int LabelNumber => 1157071;  // BBQ Smoker
        public override BaseAddon Addon => new BBQSmoker(_South, UsesRemaining);

        private bool _South;

        [Constructable]
        public BBQSmokerDeed() : this(0)
        {
        }

        [Constructable]
        public BBQSmokerDeed(int uses) : base(uses)
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

        public BBQSmokerDeed(Serial serial)
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