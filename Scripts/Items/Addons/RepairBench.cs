using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class RepairBenchComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties => true;

        public RepairBenchComponent(int id)
            : base(id, 1158860) // Repair Bench
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, Addon, list);
        }

        public RepairBenchComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RepairBenchAddon : BaseAddon, ISecurable
    {
        public List<RepairBenchDefinition> Tools;

        public static RepairBenchDefinition[] Definitions = new RepairBenchDefinition[]
        {
            new RepairBenchDefinition(DefTinkering.CraftSystem, RepairSkillType.Tinkering, 1044097, 0, 0),
            new RepairBenchDefinition(DefBlacksmithy.CraftSystem, RepairSkillType.Smithing, 1044067, 0, 0),
            new RepairBenchDefinition(DefCarpentry.CraftSystem, RepairSkillType.Carpentry, 1044071, 0, 0),
            new RepairBenchDefinition(DefTailoring.CraftSystem, RepairSkillType.Tailoring, 1044094, 0, 0),
            new RepairBenchDefinition(DefMasonry.CraftSystem, RepairSkillType.Masonry, 1072392, 0, 0),
            new RepairBenchDefinition(DefGlassblowing.CraftSystem, RepairSkillType.Glassblowing, 1072393, 0, 0),
            new RepairBenchDefinition(DefBowFletching.CraftSystem, RepairSkillType.Fletching, 1015156, 0, 0)
        };

        public static RepairBenchDefinition GetInfo(RepairSkillType type)
        {
            return Definitions.ToList().Find(x => x.Skill == type);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile User { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Using
        {
            get
            {
                if (User != null)
                {
                    if (User.InRange(Location, 2))
                    {
                        return true;
                    }

                    User = null;
                }

                return false;
            }
        }

        public override bool HandlesOnMovement => User != null;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m == User && !m.InRange(GetWorldLocation(), 2))
            {
                User = null;

                m.CloseGump(typeof(ConfirmRemoveGump));
                m.CloseGump(typeof(RepairBenchGump));
                Target.Cancel(m);
            }
        }

        [Constructable]
        public RepairBenchAddon(DirectionType type, List<RepairBenchDefinition> tools)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new RepairBenchComponent(0xA278), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new RepairBenchComponent(0xA27F), 0, 0, 0);
                    break;
            }

            if (tools == null)
            {
                Tools = new List<RepairBenchDefinition>();

                Definitions.ToList().ForEach(x =>
                {
                    Tools.Add(x);
                });
            }
            else
            {
                Tools = tools;
            }
        }

        public RepairBenchAddon(Serial serial)
            : base(serial)
        {
        }

        public void AccessibleFailMessage(Mobile from)
        {
            Components.FirstOrDefault().SendLocalizedMessageTo(from, 1061637); // You are not allowed to access this.
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                RepairBenchDeed deed = new RepairBenchDeed(Tools)
                {
                    IsRewardItem = IsRewardItem
                };

                return deed;
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if ((from.InRange(c.Location, 2)))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null)
                {
                    if (CheckAccessible(from, this))
                    {
                        if (from.HasGump(typeof(RepairBenchGump)))
                            return;

                        if (!Using || from == User)
                        {
                            User = from;
                            from.CloseGump(typeof(RepairBenchGump));
                            from.SendGump(new RepairBenchGump(from, this));
                        }
                        else
                        {
                            from.SendLocalizedMessage(500291); // Someone else is using that.
                        }
                    }
                    else
                    {
                        AccessibleFailMessage(from);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(500325); // I am too far away to do that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)Level);

            writer.Write(IsRewardItem);

            writer.Write(Tools == null ? 0 : Tools.Count);

            if (Tools != null)
            {
                Tools.ForEach(x =>
                {
                    writer.Write((int)x.Skill);
                    writer.Write((int)x.SkillValue);
                    writer.Write(x.Charges);
                });
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version != 0)
                Level = (SecureLevel)reader.ReadInt();

            IsRewardItem = reader.ReadBool();

            Tools = new List<RepairBenchDefinition>();

            int toolcount = reader.ReadInt();

            for (int x = 0; x < toolcount; x++)
            {
                RepairSkillType skill = (RepairSkillType)reader.ReadInt();
                int skillvalue = reader.ReadInt();
                int charge = reader.ReadInt();

                Tools.Add(new RepairBenchDefinition(GetInfo(skill).System, skill, GetInfo(skill).Cliloc, skillvalue, charge));
            }
        }
    }

    public class RepairBenchDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        public override int LabelNumber => 1158860;  // Repair Bench

        public override BaseAddon Addon
        {
            get
            {
                RepairBenchAddon addon = new RepairBenchAddon(_Direction, Tools)
                {
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }

        private DirectionType _Direction;

        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }

        public List<RepairBenchDefinition> Tools;

        [Constructable]
        public RepairBenchDeed()
            : this(null)
        {
        }

        [Constructable]
        public RepairBenchDeed(List<RepairBenchDefinition> tools)
            : base()
        {
            Tools = tools;
            LootType = LootType.Blessed;
        }

        public RepairBenchDeed(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076221); // 5th Year Veteran Reward

            if (Tools != null)
            {
                int[] value = Tools.Select(x => x.Charges).ToArray();
                list.Add(1158899, string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", value[0], value[1], value[2], value[3], value[4], value[5], value[6])); // Tinkering: ~1_CHARGES~<br>Blacksmithing: ~2_CHARGES~<br>Carpentry: ~3_CHARGES~<br>Tailoring: ~4_CHARGES~<br>Fletching: ~5_CHARGES~<br>Masonry: ~6_CHARGES~<br>Glassblowing: ~7_CHARGES~
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_IsRewardItem);

            writer.Write(Tools == null ? 0 : Tools.Count);

            if (Tools != null)
            {
                Tools.ForEach(x =>
                {
                    writer.Write((int)x.Skill);
                    writer.Write((int)x.SkillValue);
                    writer.Write(x.Charges);
                });
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();

            int toolcount = reader.ReadInt();

            if (toolcount != 0)
            {
                Tools = new List<RepairBenchDefinition>();
            }

            for (int x = 0; x < toolcount; x++)
            {
                RepairSkillType skill = (RepairSkillType)reader.ReadInt();
                int skillvalue = reader.ReadInt();
                int charge = reader.ReadInt();

                Tools.Add(new RepairBenchDefinition(RepairBenchAddon.GetInfo(skill).System, skill, RepairBenchAddon.GetInfo(skill).Cliloc, skillvalue, charge));
            }
        }
    }

    public class RepairBenchDefinition
    {
        public CraftSystem System { get; set; }
        public RepairSkillType Skill { get; set; }
        public double SkillValue { get; set; }
        public int Charges { get; set; }
        public int Cliloc { get; set; }

        public RepairBenchDefinition(CraftSystem system, RepairSkillType skill, int cliloc, double value, int charges)
        {
            System = system;
            Skill = skill;
            Cliloc = cliloc;
            SkillValue = value;
            Charges = charges;
        }
    }

    public class ConfirmRemoveGump : Gump
    {
        private readonly RepairBenchAddon m_Addon;
        private readonly RepairSkillType m_Skill;

        public ConfirmRemoveGump(RepairBenchAddon addon, RepairSkillType skill)
            : base(340, 340)
        {
            m_Addon = addon;
            m_Skill = skill;

            AddPage(0);

            AddBackground(0, 0, 291, 113, 0x13BE);
            AddImageTiled(5, 5, 280, 80, 0xA40);
            AddHtmlLocalized(9, 9, 272, 80, 1158874, string.Format("#{0}", addon.Tools.Find(x => x.Skill == skill).Cliloc), 0x7FFF, false, false); // Are you sure you wish to remove all the ~1_SKILL~ charges from the bench? This action will delete all existing charges and will not refund any deeds.

            AddButton(5, 87, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 89, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL

            AddButton(160, 87, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 89, 120, 20, 1006044, 0x7FFF, false, false); // OK
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Addon == null || m_Addon.Deleted)
                return;

            Mobile m = sender.Mobile;
            int index = info.ButtonID;

            switch (index)
            {
                case 0: { m_Addon.User = null; break; }
                case 1:
                    {
                        RepairBenchDefinition tool = m_Addon.Tools.Find(x => x.Skill == m_Skill);

                        tool.SkillValue = 0;
                        tool.Charges = 0;

                        m.SendLocalizedMessage(1158873, string.Format("#{0}", tool.Cliloc)); // You clear all the ~1_SKILL~ charges from the bench.

                        m.SendGump(new RepairBenchGump(m, m_Addon));
                        break;
                    }
            }
        }
    }

    public class RepairBenchGump : Gump
    {
        private readonly RepairBenchAddon m_Addon;
        private Timer m_Timer;

        public RepairBenchGump(Mobile from, RepairBenchAddon addon)
            : base(100, 100)
        {
            m_Addon = addon;

            StopTimer(from);

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerStateCallback(CloseGump), from);

            AddPage(0);

            AddBackground(0, 0, 370, 470, 0x6DB);

            AddImage(82, 0, 0x6E4);
            AddHtmlLocalized(10, 10, 350, 18, 1114513, "#1158860", 0x0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            AddHtmlLocalized(70, 57, 120, 20, 1114513, "#1158878", 0x7FE0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddHtmlLocalized(213, 57, 190, 20, 1158879, 0x7FE0, false, false); // Charges

            AddItem(20, 80, 0x1EB8);
            AddTooltip(1044097);
            AddButton(70, 97, 0x15E1, 0x15E5, 12, GumpButtonType.Reply, 0);
            AddLabel(113, 97, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Tinkering)));
            AddLabel(218, 97, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Tinkering)));
            AddButton(318, 97, 0x2716, 0x2716, 22, GumpButtonType.Reply, 0);

            AddItem(20, 125, 0x0FB4);
            AddTooltip(1044067);
            AddButton(70, 137, 0x15E1, 0x15E5, 10, GumpButtonType.Reply, 0);
            AddLabel(113, 137, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Smithing)));
            AddLabel(218, 137, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Smithing)));
            AddButton(318, 137, 0x2716, 0x2716, 20, GumpButtonType.Reply, 0);

            AddItem(20, 170, 0x1034);
            AddTooltip(1044071);
            AddButton(70, 177, 0x15E1, 0x15E5, 13, GumpButtonType.Reply, 0);
            AddLabel(113, 177, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Carpentry)));
            AddLabel(218, 177, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Carpentry)));
            AddButton(318, 177, 0x2716, 0x2716, 23, GumpButtonType.Reply, 0);

            AddItem(20, 215, 0x0F9D);
            AddTooltip(1044094);
            AddButton(70, 217, 0x15E1, 0x15E5, 11, GumpButtonType.Reply, 0);
            AddLabel(113, 217, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Tailoring)));
            AddLabel(218, 217, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Tailoring)));
            AddButton(318, 217, 0x2716, 0x2716, 21, GumpButtonType.Reply, 0);

            AddItem(20, 260, 0x12B3);
            AddTooltip(1072392);
            AddButton(70, 257, 0x15E1, 0x15E5, 15, GumpButtonType.Reply, 0);
            AddLabel(113, 257, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Masonry)));
            AddLabel(218, 257, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Masonry)));
            AddButton(318, 257, 0x2716, 0x2716, 25, GumpButtonType.Reply, 0);

            AddItem(20, 305, 0x182D);
            AddTooltip(1072393);
            AddButton(70, 297, 0x15E1, 0x15E5, 16, GumpButtonType.Reply, 0);
            AddLabel(113, 297, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Glassblowing)));
            AddLabel(218, 297, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Glassblowing)));
            AddButton(318, 297, 0x2716, 0x2716, 26, GumpButtonType.Reply, 0);

            AddItem(20, 350, 0x1022);
            AddTooltip(1015156);
            AddButton(70, 337, 0x15E1, 0x15E5, 14, GumpButtonType.Reply, 0);
            AddLabel(113, 337, 0x5F, string.Format("{0:F1}", GetSkillValue(RepairSkillType.Fletching)));
            AddLabel(218, 337, 0x5F, string.Format("{0}", GetCharges(RepairSkillType.Fletching)));
            AddButton(318, 337, 0x2716, 0x2716, 24, GumpButtonType.Reply, 0);

            AddButton(70, 407, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 407, 200, 30, 1153100, 0x7FFF, false, false); // Add Charges
        }

        public void CloseGump(object state)
        {
            Mobile from = state as Mobile;

            StopTimer(from);

            m_Addon.User = null;

            if (from != null && !from.Deleted)
            {
                from.CloseGump(typeof(RepairBenchGump));
            }
        }

        public void StopTimer(Mobile from)
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        private double GetSkillValue(RepairSkillType skill)
        {
            return m_Addon.Tools.Find(x => x.Skill == skill).SkillValue;
        }

        private int GetCharges(RepairSkillType skill)
        {
            return m_Addon.Tools.Find(x => x.Skill == skill).Charges;
        }

        public class InternalTarget : Target
        {
            private readonly RepairBenchAddon m_Addon;
            private readonly RepairBenchGump m_Gump;

            public InternalTarget(Mobile from, RepairBenchGump g, RepairBenchAddon addon)
                : base(-1, false, TargetFlags.None)
            {
                m_Addon = addon;
                m_Gump = g;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Addon == null || m_Addon.Deleted || (targeted is Item && !from.InRange(((Item)targeted).GetWorldLocation(), 2)))
                {
                    return;
                }

                if (!m_Addon.CheckAccessible(from, m_Addon))
                {
                    m_Addon.User = null;
                    m_Addon.AccessibleFailMessage(from);
                    return;
                }

                if (targeted is RepairDeed)
                {
                    RepairDeed deed = (RepairDeed)targeted;

                    if (m_Addon.Tools.Any(x => x.Skill == deed.RepairSkill && x.Charges >= 500))
                    {
                        from.SendLocalizedMessage(1158778); // This would exceed the maximum charges allowed on this magic item.
                        from.Target = new InternalTarget(from, m_Gump, m_Addon);
                    }
                    else if (m_Addon.Tools.Any(x => x.Skill == deed.RepairSkill && x.Charges != 0 && x.SkillValue != deed.SkillLevel))
                    {
                        from.SendLocalizedMessage(1158866); // The repair bench contains deeds that do not match the skill of the deed you are trying to add.
                        from.Target = new InternalTarget(from, m_Gump, m_Addon);
                    }
                    else
                    {
                        RepairBenchDefinition tool = m_Addon.Tools.Find(x => x.Skill == deed.RepairSkill);

                        tool.SkillValue = deed.SkillLevel;
                        tool.Charges++;

                        deed.Delete();

                        from.Target = new InternalTarget(from, m_Gump, m_Addon);
                    }
                }
                else if (targeted is Container)
                {
                    Container c = targeted as Container;

                    for (int i = c.Items.Count - 1; i >= 0; --i)
                    {
                        if (i < c.Items.Count && c.Items[i] is RepairDeed)
                        {
                            RepairDeed deed = (RepairDeed)c.Items[i];

                            if (m_Addon.Tools.Any(x => x.Skill == deed.RepairSkill && x.Charges >= 500))
                            {
                                from.SendLocalizedMessage(1158778); // This would exceed the maximum charges allowed on this magic item.
                            }
                            else if (m_Addon.Tools.Any(x => x.Skill == deed.RepairSkill && x.SkillValue == deed.SkillLevel))
                            {
                                RepairBenchDefinition tool = m_Addon.Tools.Find(x => x.Skill == deed.RepairSkill);

                                tool.SkillValue = deed.SkillLevel;
                                tool.Charges++;

                                deed.Delete();
                            }
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1158865); // That is not a valid repair contract or container.
                }

                m_Gump.StopTimer(from);
                from.CloseGump(typeof(RepairBenchGump));
                from.SendGump(new RepairBenchGump(from, m_Addon));
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (m_Addon != null && !m_Addon.Deleted && from.InRange(m_Addon.GetWorldLocation(), 2))
                {
                    m_Gump.StopTimer(from);
                    from.CloseGump(typeof(RepairBenchGump));
                    from.SendGump(new RepairBenchGump(from, m_Addon));
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            int index = info.ButtonID;

            if (index == 0)
            {
                m_Addon.User = null;
            }
            else if (index == 1)
            {
                StopTimer(from);
                from.SendLocalizedMessage(1158871); // Which repair deed or container of repair deeds do you wish to add to the repair bench?
                from.Target = new InternalTarget(from, this, m_Addon);
            }
            else if (index >= 10 && index < 20)
            {
                StopTimer(from);
                int skillindex = index - 10;
                Repair.Do(from, RepairSkillInfo.GetInfo((RepairSkillType)skillindex).System, m_Addon);
            }
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(m_Addon);

                if (house != null && house.IsOwner(from))
                {
                    StopTimer(from);
                    int skillindex = index - 20;
                    from.SendGump(new ConfirmRemoveGump(m_Addon, (RepairSkillType)skillindex));
                }
                else
                {
                    from.SendLocalizedMessage(1005213); // You can't do that
                    m_Addon.User = null;
                }
            }
        }
    }
}
