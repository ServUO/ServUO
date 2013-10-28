using System;
using System.Collections;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class EditSkillGump : Gump
    {
        public static readonly bool OldStyle = PropsConfig.OldStyle;
        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;
        public static readonly int SetWidth = PropsConfig.SetWidth;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
        public static readonly int OffsetSize = PropsConfig.OffsetSize;
        public static readonly int EntryHeight = PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;
        private static readonly int EntryWidth = 160;
        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + (2 * (EntryHeight + OffsetSize));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly Skill m_Skill;
        private readonly SkillsGumpGroup m_Selected;
        public EditSkillGump(Mobile from, Mobile target, Skill skill, SkillsGumpGroup selected)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_From = from;
            this.m_Target = target;
            this.m_Skill = skill;
            this.m_Selected = selected;

            string initialText = this.m_Skill.Base.ToString("F1");

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BackHeight, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, skill.Name);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            x = BorderSize + OffsetSize;
            y += EntryHeight + OffsetSize;

            this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
            this.AddTextEntry(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, 0, initialText);
            x += EntryWidth + OffsetSize;

            if (SetGumpID != 0)
                this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

            this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                try
                {
                    if (this.m_From.AccessLevel >= AccessLevel.GameMaster)
                    {
                        TextRelay text = info.GetTextEntry(0);

                        if (text != null)
                        {
                            this.m_Skill.Base = Convert.ToDouble(text.Text);
                            CommandLogging.LogChangeProperty(this.m_From, this.m_Target, String.Format("{0}.Base", this.m_Skill), this.m_Skill.Base.ToString());
                        }
                    }
                    else
                    {
                        this.m_From.SendMessage("You may not change that.");
                    }

                    this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, this.m_Selected));
                }
                catch
                {
                    this.m_From.SendMessage("Bad format. ###.# expected.");
                    this.m_From.SendGump(new EditSkillGump(this.m_From, this.m_Target, this.m_Skill, this.m_Selected));
                }
            }
            else
            {
                this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, this.m_Selected));
            }
        }
    }

    public class SkillsGump : Gump
    {
        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;
        public static readonly int SetWidth = PropsConfig.SetWidth;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
        public static readonly int OffsetSize = PropsConfig.OffsetSize;
        public static readonly int EntryHeight = PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;
        public static bool OldStyle = PropsConfig.OldStyle;
        /*
        private static bool PrevLabel = OldStyle, NextLabel = OldStyle;

        private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		
        private static readonly int PrevLabelOffsetY = 0;

        private static readonly int NextLabelOffsetX = -29;
        private static readonly int NextLabelOffsetY = 0;
        * */
        private static readonly int NameWidth = 107;
        private static readonly int ValueWidth = 128;
        private static readonly int EntryCount = 15;
        private static readonly int TypeWidth = NameWidth + OffsetSize + ValueWidth;
        private static readonly int TotalWidth = OffsetSize + NameWidth + OffsetSize + ValueWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private static readonly int IndentWidth = 12;
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly SkillsGumpGroup[] m_Groups;
        private readonly SkillsGumpGroup m_Selected;
        public SkillsGump(Mobile from, Mobile target)
            : this(from, target, null)
        {
        }

        public SkillsGump(Mobile from, Mobile target, SkillsGumpGroup selected)
            : base(GumpOffsetX, GumpOffsetY)
        {
            this.m_From = from;
            this.m_Target = target;

            this.m_Groups = SkillsGumpGroup.Groups;
            this.m_Selected = selected;

            int count = this.m_Groups.Length;

            if (selected != null)
                count += selected.Skills.Length;

            int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

            if (OldStyle)
                this.AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
            else
                this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

            x += PrevWidth + OffsetSize;

            if (!OldStyle)
                this.AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, HeaderGumpID);

            x += emptyWidth + OffsetSize;

            if (!OldStyle)
                this.AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

            for (int i = 0; i < this.m_Groups.Length; ++i)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                SkillsGumpGroup group = this.m_Groups[i];

                this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

                if (group == selected)
                    this.AddButton(x + PrevOffsetX, y + PrevOffsetY, 0x15E2, 0x15E6, this.GetButtonID(0, i), GumpButtonType.Reply, 0);
                else
                    this.AddButton(x + PrevOffsetX, y + PrevOffsetY, 0x15E1, 0x15E5, this.GetButtonID(0, i), GumpButtonType.Reply, 0);

                x += PrevWidth + OffsetSize;

                x -= (OldStyle ? OffsetSize : 0);

                this.AddImageTiled(x, y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);
                this.AddLabel(x + TextOffsetX, y, TextHue, group.Name);

                x += emptyWidth + (OldStyle ? OffsetSize * 2 : 0);
                x += OffsetSize;

                if (SetGumpID != 0)
                    this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                if (group == selected)
                {
                    int indentMaskX = BorderSize;
                    int indentMaskY = y + EntryHeight + OffsetSize;

                    for (int j = 0; j < group.Skills.Length; ++j)
                    {
                        Skill sk = target.Skills[group.Skills[j]];

                        x = BorderSize + OffsetSize;
                        y += EntryHeight + OffsetSize;

                        x += OffsetSize;
                        x += IndentWidth;

                        this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

                        this.AddButton(x + PrevOffsetX, y + PrevOffsetY, 0x15E1, 0x15E5, this.GetButtonID(1, j), GumpButtonType.Reply, 0);

                        x += PrevWidth + OffsetSize;

                        x -= (OldStyle ? OffsetSize : 0);

                        this.AddImageTiled(x, y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0) - OffsetSize - IndentWidth, EntryHeight, EntryGumpID);
                        this.AddLabel(x + TextOffsetX, y, TextHue, sk == null ? "(null)" : sk.Name);

                        x += emptyWidth + (OldStyle ? OffsetSize * 2 : 0) - OffsetSize - IndentWidth;
                        x += OffsetSize;

                        if (SetGumpID != 0)
                            this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                        if (sk != null)
                        {
                            int buttonID1, buttonID2;
                            int xOffset, yOffset;

                            switch ( sk.Lock )
                            {
                                default:
                                case SkillLock.Up:
                                    buttonID1 = 0x983;
                                    buttonID2 = 0x983;
                                    xOffset = 6;
                                    yOffset = 4;
                                    break;
                                case SkillLock.Down:
                                    buttonID1 = 0x985;
                                    buttonID2 = 0x985;
                                    xOffset = 6;
                                    yOffset = 4;
                                    break;
                                case SkillLock.Locked:
                                    buttonID1 = 0x82C;
                                    buttonID2 = 0x82C;
                                    xOffset = 5;
                                    yOffset = 2;
                                    break;
                            }

                            this.AddButton(x + xOffset, y + yOffset, buttonID1, buttonID2, this.GetButtonID(2, j), GumpButtonType.Reply, 0);

                            y += 1;
                            x -= OffsetSize;
                            x -= 1;
                            x -= 50;

                            this.AddImageTiled(x, y, 50, EntryHeight - 2, OffsetGumpID);

                            x += 1;
                            y += 1;

                            this.AddImageTiled(x, y, 48, EntryHeight - 4, EntryGumpID);

                            this.AddLabelCropped(x + TextOffsetX, y - 1, 48 - TextOffsetX, EntryHeight - 3, TextHue, sk.Base.ToString("F1"));

                            y -= 2;
                        }
                    }

                    this.AddImageTiled(indentMaskX, indentMaskY, IndentWidth + OffsetSize, (group.Skills.Length * (EntryHeight + OffsetSize)) - (i < (this.m_Groups.Length - 1) ? OffsetSize : 0), BackGumpID + 4);
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int buttonID = info.ButtonID - 1;

            int index = buttonID / 3;
            int type = buttonID % 3;

            switch ( type )
            {
                case 0:
                    {
                        if (index >= 0 && index < this.m_Groups.Length)
                        {
                            SkillsGumpGroup newSelection = this.m_Groups[index];

                            if (this.m_Selected != newSelection)
                                this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, newSelection));
                            else
                                this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, null));
                        }

                        break;
                    }
                case 1:
                    {
                        if (this.m_Selected != null && index >= 0 && index < this.m_Selected.Skills.Length)
                        {
                            Skill sk = this.m_Target.Skills[this.m_Selected.Skills[index]];

                            if (sk != null)
                            {
                                if (this.m_From.AccessLevel >= AccessLevel.GameMaster)
                                {
                                    this.m_From.SendGump(new EditSkillGump(this.m_From, this.m_Target, sk, this.m_Selected));
                                }
                                else
                                {
                                    this.m_From.SendMessage("You may not change that.");
                                    this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, this.m_Selected));
                                }
                            }
                            else
                            {
                                this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, this.m_Selected));
                            }
                        }

                        break;
                    }
                case 2:
                    {
                        if (this.m_Selected != null && index >= 0 && index < this.m_Selected.Skills.Length)
                        {
                            Skill sk = this.m_Target.Skills[this.m_Selected.Skills[index]];

                            if (sk != null)
                            {
                                if (this.m_From.AccessLevel >= AccessLevel.GameMaster)
                                {
                                    switch ( sk.Lock )
                                    {
                                        case SkillLock.Up:
                                            sk.SetLockNoRelay(SkillLock.Down);
                                            sk.Update();
                                            break;
                                        case SkillLock.Down:
                                            sk.SetLockNoRelay(SkillLock.Locked);
                                            sk.Update();
                                            break;
                                        case SkillLock.Locked:
                                            sk.SetLockNoRelay(SkillLock.Up);
                                            sk.Update();
                                            break;
                                    }
                                }
                                else
                                {
                                    this.m_From.SendMessage("You may not change that.");
                                }

                                this.m_From.SendGump(new SkillsGump(this.m_From, this.m_Target, this.m_Selected));
                            }
                        }

                        break;
                    }
            }
        }

        public int GetButtonID(int type, int index)
        {
            return 1 + (index * 3) + type;
        }
    }

    public class SkillsGumpGroup
    {
        private static readonly SkillsGumpGroup[] m_Groups = new SkillsGumpGroup[]
        {
            new SkillsGumpGroup("Crafting", new SkillName[]
            {
                SkillName.Alchemy,
                SkillName.Blacksmith,
                SkillName.Cartography,
                SkillName.Carpentry,
                SkillName.Cooking,
                SkillName.Fletching,
                SkillName.Inscribe,
                SkillName.Tailoring,
                SkillName.Tinkering,
                SkillName.Imbuing
            }),
            new SkillsGumpGroup("Bardic", new SkillName[]
            {
                SkillName.Discordance,
                SkillName.Musicianship,
                SkillName.Peacemaking,
                SkillName.Provocation
            }),
            new SkillsGumpGroup("Magical", new SkillName[]
            {
                SkillName.Chivalry,
                SkillName.EvalInt,
                SkillName.Magery,
                SkillName.MagicResist,
                SkillName.Meditation,
                SkillName.Necromancy,
                SkillName.SpiritSpeak,
                SkillName.Ninjitsu,
                SkillName.Bushido,
                SkillName.Spellweaving,
                SkillName.Mysticism
            }),
            new SkillsGumpGroup("Miscellaneous", new SkillName[]
            {
                SkillName.Camping,
                SkillName.Fishing,
                SkillName.Focus,
                SkillName.Healing,
                SkillName.Herding,
                SkillName.Lockpicking,
                SkillName.Lumberjacking,
                SkillName.Mining,
                SkillName.Snooping,
                SkillName.Veterinary
            }),
            new SkillsGumpGroup("Combat Ratings", new SkillName[]
            {
                SkillName.Archery,
                SkillName.Fencing,
                SkillName.Macing,
                SkillName.Parry,
                SkillName.Swords,
                SkillName.Tactics,
                SkillName.Wrestling,
                SkillName.Throwing
            }),
            new SkillsGumpGroup("Actions", new SkillName[]
            {
                SkillName.AnimalTaming,
                SkillName.Begging,
                SkillName.DetectHidden,
                SkillName.Hiding,
                SkillName.RemoveTrap,
                SkillName.Poisoning,
                SkillName.Stealing,
                SkillName.Stealth,
                SkillName.Tracking
            }),
            new SkillsGumpGroup("Lore & Knowledge", new SkillName[]
            {
                SkillName.Anatomy,
                SkillName.AnimalLore,
                SkillName.ArmsLore,
                SkillName.Forensics,
                SkillName.ItemID,
                SkillName.TasteID
            })
        };
        private readonly string m_Name;
        private readonly SkillName[] m_Skills;
        public SkillsGumpGroup(string name, SkillName[] skills)
        {
            this.m_Name = name;
            this.m_Skills = skills;

            Array.Sort(this.m_Skills, new SkillNameComparer());
        }

        public static SkillsGumpGroup[] Groups
        {
            get
            {
                return m_Groups;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public SkillName[] Skills
        {
            get
            {
                return this.m_Skills;
            }
        }

        private class SkillNameComparer : IComparer
        {
            public SkillNameComparer()
            {
            }

            public int Compare(object x, object y)
            {
                SkillName a = (SkillName)x;
                SkillName b = (SkillName)y;

                string aName = SkillInfo.Table[(int)a].Name;
                string bName = SkillInfo.Table[(int)b].Name;

                return aName.CompareTo(bName);
            }
        }
    }
}