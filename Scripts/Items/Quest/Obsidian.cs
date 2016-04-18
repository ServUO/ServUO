using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.Quests.Collector
{
    public class Obsidian : Item
    {
        private static readonly string[] m_Names = new string[]
        {
            null,
            "an aggressive cavalier",
            "a beguiling rogue",
            "a benevolent physician",
            "a brilliant artisan",
            "a capricious adventurer",
            "a clever beggar",
            "a convincing charlatan",
            "a creative inventor",
            "a creative tinker",
            "a cunning knave",
            "a dauntless explorer",
            "a despicable ruffian",
            "an earnest malcontent",
            "an exultant animal tamer",
            "a famed adventurer",
            "a fanatical crusader",
            "a fastidious clerk",
            "a fearless hunter",
            "a festive harlequin",
            "a fidgety assassin",
            "a fierce soldier",
            "a fierce warrior",
            "a frugal magnate",
            "a glib pundit",
            "a gnomic shaman",
            "a graceful noblewoman",
            "a idiotic madman",
            "a imaginative designer",
            "an inept conjurer",
            "an innovative architect",
            "an inventive blacksmith",
            "a judicious mayor",
            "a masterful chef",
            "a masterful woodworker",
            "a melancholy clown",
            "a melodic bard",
            "a merciful guard",
            "a mirthful jester",
            "a nervous surgeon",
            "a peaceful scholar",
            "a prolific gardener",
            "a quixotic knight",
            "a regal aristocrat",
            "a resourceful smith",
            "a reticent alchemist",
            "a sanctified priest",
            "a scheming patrician",
            "a shrewd mage",
            "a singing minstrel",
            "a skilled tailor",
            "a squeamish assassin",
            "a stoic swordsman",
            "a studious scribe",
            "a thought provoking writer",
            "a treacherous scoundrel",
            "a troubled poet",
            "an unflappable wizard",
            "a valiant warrior",
            "a wayward fool"
        };
        private const int m_Partial = 2;
        private const int m_Completed = 10;
        private int m_Quantity;
        private string m_StatueName;
        [Constructable]
        public Obsidian()
            : base(0x1EA7)
        {
            this.Hue = 0x497;

            this.m_Quantity = 1;
            this.m_StatueName = "";
        }

        public Obsidian(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Quantity
        {
            get
            {
                return this.m_Quantity;
            }
            set
            {
                if (value <= 1)
                    this.m_Quantity = 1;
                else if (value >= m_Completed)
                    this.m_Quantity = m_Completed;
                else
                    this.m_Quantity = value;

                if (this.m_Quantity < m_Partial)
                    this.ItemID = 0x1EA7;
                else if (this.m_Quantity < m_Completed)
                    this.ItemID = 0x1F13;
                else
                    this.ItemID = 0x12CB;

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string StatueName
        {
            get
            {
                return this.m_StatueName;
            }
            set
            {
                this.m_StatueName = value;
                this.InvalidateProperties();
            }
        }
        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public static string RandomName(Mobile from)
        {
            int index = Utility.Random(m_Names.Length);
            if (m_Names[index] == null)
                return from.Name;
            else
                return m_Names[index];
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.m_Quantity < m_Partial)
                list.Add(1055137); // a section of an obsidian statue
            else if (this.m_Quantity < m_Completed)
                list.Add(1055138); // a partially reconstructed obsidian statue
            else
                list.Add(1055139, this.m_StatueName); // an obsidian statue of ~1_STATUE_NAME~
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.m_Quantity < m_Partial)
                this.LabelTo(from, 1055137); // a section of an obsidian statue
            else if (this.m_Quantity < m_Completed)
                this.LabelTo(from, 1055138); // a partially reconstructed obsidian statue
            else
                this.LabelTo(from, 1055139, this.m_StatueName); // an obsidian statue of ~1_STATUE_NAME~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.m_Quantity >= m_Partial && this.m_Quantity < m_Completed && this.IsChildOf(from.Backpack))
                list.Add(new DisassembleEntry(this));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Quantity < m_Completed)
            {
                if (!this.IsChildOf(from.Backpack))
                    from.Send(new MessageLocalized(this.Serial, this.ItemID, MessageType.Regular, 0x2C, 3, 500309, "", "")); // Nothing Happens.
                else
                    from.Target = new InternalTarget(this);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.WriteEncodedInt(this.m_Quantity);
            writer.Write((string)this.m_StatueName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Quantity = reader.ReadEncodedInt();
            this.m_StatueName = Utility.Intern(reader.ReadString());
        }

        private class DisassembleEntry : ContextMenuEntry
        {
            private readonly Obsidian m_Obsidian;
            public DisassembleEntry(Obsidian obsidian)
                : base(6142)
            {
                this.m_Obsidian = obsidian;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;
                if (!this.m_Obsidian.Deleted && this.m_Obsidian.Quantity >= Obsidian.m_Partial && this.m_Obsidian.Quantity < Obsidian.m_Completed && this.m_Obsidian.IsChildOf(from.Backpack) && from.CheckAlive())
                {
                    for (int i = 0; i < this.m_Obsidian.Quantity - 1; i++)
                        from.AddToBackpack(new Obsidian());

                    this.m_Obsidian.Quantity = 1;
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly Obsidian m_Obsidian;
            public InternalTarget(Obsidian obsidian)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Obsidian = obsidian;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Item targ = targeted as Item;
                if (this.m_Obsidian.Deleted || this.m_Obsidian.Quantity >= Obsidian.m_Completed || targ == null)
                    return;

                if (this.m_Obsidian.IsChildOf(from.Backpack) && targ.IsChildOf(from.Backpack) && targ is Obsidian && targ != this.m_Obsidian)
                {
                    Obsidian targObsidian = (Obsidian)targ;
                    if (targObsidian.Quantity < Obsidian.m_Completed)
                    {
                        if (targObsidian.Quantity + this.m_Obsidian.Quantity <= Obsidian.m_Completed)
                        {
                            targObsidian.Quantity += this.m_Obsidian.Quantity;
                            this.m_Obsidian.Delete();
                        }
                        else
                        {
                            int delta = Obsidian.m_Completed - targObsidian.Quantity;
                            targObsidian.Quantity += delta;
                            this.m_Obsidian.Quantity -= delta;
                        }

                        if (targObsidian.Quantity >= Obsidian.m_Completed)
                            targObsidian.StatueName = Obsidian.RandomName(from);

                        from.Send(new AsciiMessage(targObsidian.Serial, targObsidian.ItemID, MessageType.Regular, 0x59, 3, this.m_Obsidian.Name, "Something Happened."));

                        return;
                    }
                }

                from.Send(new MessageLocalized(this.m_Obsidian.Serial, this.m_Obsidian.ItemID, MessageType.Regular, 0x2C, 3, 500309, this.m_Obsidian.Name, "")); // Nothing Happens.
            }
        }
    }
}