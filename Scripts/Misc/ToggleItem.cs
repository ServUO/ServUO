using System;
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
            this.Movable = false;

            this.m_InactiveItemID = inactiveItemID;
            this.m_ActiveItemID = activeItemID;
            this.m_PlayersCanToggle = playersCanToggle;
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
                return this.m_InactiveItemID;
            }
            set
            {
                this.m_InactiveItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveItemID
        {
            get
            {
                return this.m_ActiveItemID;
            }
            set
            {
                this.m_ActiveItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayersCanToggle
        {
            get
            {
                return this.m_PlayersCanToggle;
            }
            set
            {
                this.m_PlayersCanToggle = value;
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
                this.Toggle();
            }
            else if (this.m_PlayersCanToggle)
            {
                if (from.InRange(this.GetWorldLocation(), 1))
                    this.Toggle();
                else
                    from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        public void Toggle()
        {
            this.ItemID = (this.ItemID == this.m_ActiveItemID) ? this.m_InactiveItemID : this.m_ActiveItemID;
            this.Visible = (this.ItemID != 0x1);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_InactiveItemID);
            writer.Write(this.m_ActiveItemID);
            writer.Write(this.m_PlayersCanToggle);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_InactiveItemID = reader.ReadInt();
            this.m_ActiveItemID = reader.ReadInt();
            this.m_PlayersCanToggle = reader.ReadBool();
        }

        public class ToggleCommand : BaseCommand
        {
            public ToggleCommand()
            {
                this.AccessLevel = AccessLevel.GameMaster;
                this.Supports = CommandSupport.AllItems;
                this.Commands = new string[] { "Toggle" };
                this.ObjectTypes = ObjectTypes.Items;
                this.Usage = "Toggle";
                this.Description = "Toggles a targeted ToggleItem.";
            }

            public override void Execute(CommandEventArgs e, object obj)
            {
                if (obj is ToggleItem)
                {
                    ((ToggleItem)obj).Toggle();
                    this.AddResponse("The item has been toggled.");
                }
                else
                {
                    this.LogFailure("That is not a ToggleItem.");
                }
            }
        }
    }
}