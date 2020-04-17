using Server.Commands;
using Server.Commands.Generic;

namespace Server.Items
{
    public class ToggleItem : Item
    {
        private int m_InactiveItemID;
        private int m_ActiveItemID;
        private bool m_PlayersCanToggle;
        [Constructable]
        public ToggleItem(int inactiveItemID, int activeItemID)
            : this(inactiveItemID, activeItemID, false)
        {
        }

        [Constructable]
        public ToggleItem(int inactiveItemID, int activeItemID, bool playersCanToggle)
            : base(inactiveItemID)
        {
            Movable = false;

            m_InactiveItemID = inactiveItemID;
            m_ActiveItemID = activeItemID;
            m_PlayersCanToggle = playersCanToggle;
        }

        public ToggleItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int InactiveItemID
        {
            get
            {
                return m_InactiveItemID;
            }
            set
            {
                m_InactiveItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveItemID
        {
            get
            {
                return m_ActiveItemID;
            }
            set
            {
                m_ActiveItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayersCanToggle
        {
            get
            {
                return m_PlayersCanToggle;
            }
            set
            {
                m_PlayersCanToggle = value;
            }
        }
        public static void Initialize()
        {
            TargetCommands.Register(new ToggleCommand());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                Toggle();
            }
            else if (m_PlayersCanToggle)
            {
                if (from.InRange(GetWorldLocation(), 1))
                    Toggle();
                else
                    from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        public void Toggle()
        {
            ItemID = (ItemID == m_ActiveItemID) ? m_InactiveItemID : m_ActiveItemID;
            Visible = (ItemID != 0x1);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_InactiveItemID);
            writer.Write(m_ActiveItemID);
            writer.Write(m_PlayersCanToggle);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_InactiveItemID = reader.ReadInt();
            m_ActiveItemID = reader.ReadInt();
            m_PlayersCanToggle = reader.ReadBool();
        }

        public class ToggleCommand : BaseCommand
        {
            public ToggleCommand()
            {
                AccessLevel = AccessLevel.GameMaster;
                Supports = CommandSupport.AllItems;
                Commands = new string[] { "Toggle" };
                ObjectTypes = ObjectTypes.Items;
                Usage = "Toggle";
                Description = "Toggles a targeted ToggleItem.";
            }

            public override void Execute(CommandEventArgs e, object obj)
            {
                if (obj is ToggleItem)
                {
                    ((ToggleItem)obj).Toggle();
                    AddResponse("The item has been toggled.");
                }
                else
                {
                    LogFailure("That is not a ToggleItem.");
                }
            }
        }
    }
}