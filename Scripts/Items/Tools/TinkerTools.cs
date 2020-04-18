using Server.ContextMenus;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x1EB8, 0x1EB9)]
    public class TinkerTools : BaseTool
    {
        [Constructable]
        public TinkerTools()
            : base(0x1EB8)
        {
            Weight = 1.0;
        }

        [Constructable]
        public TinkerTools(int uses)
            : base(uses, 0x1EB8)
        {
            Weight = 1.0;
        }

        public TinkerTools(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefTinkering.CraftSystem;
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

        public override void GetContextMenuEntries(Mobile from, System.Collections.Generic.List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new ToggleRepairContextMenuEntry(from, this));
        }

        public class ToggleRepairContextMenuEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly BaseTool _Tool;

            public ToggleRepairContextMenuEntry(Mobile from, BaseTool tool)
                : base(1157040) // Toggle Repair Mode
            {
                _From = from;
                _Tool = tool;
            }

            public override void OnClick()
            {
                if (_Tool.RepairMode)
                {
                    _From.SendLocalizedMessage(1157042); // This tool is fully functional. 
                    _Tool.RepairMode = false;
                }
                else
                {
                    _From.SendLocalizedMessage(1157041); // This tool will only repair items in this mode.
                    _Tool.RepairMode = true;
                }
            }
        }
    }

    public class TinkersTools : BaseTool
    {
        [Constructable]
        public TinkersTools()
            : base(0x1EBC)
        {
            Weight = 1.0;
        }

        [Constructable]
        public TinkersTools(int uses)
            : base(uses, 0x1EBC)
        {
            Weight = 1.0;
        }

        public TinkersTools(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefTinkering.CraftSystem;
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

        public override void GetContextMenuEntries(Mobile from, System.Collections.Generic.List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new ToggleRepairContextMenuEntry(from, this));
        }

        public class ToggleRepairContextMenuEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly BaseTool _Tool;

            public ToggleRepairContextMenuEntry(Mobile from, BaseTool tool)
                : base(1157040) // Toggle Repair Mode
            {
                _From = from;
                _Tool = tool;
            }

            public override void OnClick()
            {
                if (_Tool.RepairMode)
                {
                    _From.SendLocalizedMessage(1157042); // This tool is fully functional. 
                    _Tool.RepairMode = false;
                }
                else
                {
                    _From.SendLocalizedMessage(1157041); // This tool will only repair items in this mode.
                    _Tool.RepairMode = true;
                }
            }
        }
    }
}
