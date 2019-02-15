using System;
using System.Collections.Generic;
using System.Linq;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class RepairBenchComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties { get { return true; } }

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
        public SecureLevel Level { get; set; }

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

        public override BaseAddonDeed Deed { get { return new RepairBenchDeed(Tools); } }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if ((from.InRange(c.Location, 3)))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null)
                {
                    if (CheckAccessible(from, this))
                    {
                        from.CloseGump(typeof(RepairBenchGump));
                        from.SendGump(new RepairBenchGump(this));
                    }
                    else
                    {
                        from.SendLocalizedMessage(1061637); // You are not allowed to access this.
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
            writer.Write((int)0);

            writer.Write(Tools == null ? 0 : Tools.Count);

            if (Tools != null)
            {
                Tools.ForEach(x =>
                {
                    writer.Write((int)x.Skill);
                    writer.Write((int)x.SkillValue);
                    writer.Write((int)x.Charges);
                });
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

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

    public class RepairBenchDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1158860; } } // Repair Bench

        public override BaseAddon Addon { get { return new RepairBenchAddon(_Direction, Tools); } }

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
                list.Add(1158899, String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", value[0], value[1], value[2], value[3], value[4], value[5], value[6])); // Tinkering: ~1_CHARGES~<br>Blacksmithing: ~2_CHARGES~<br>Carpentry: ~3_CHARGES~<br>Tailoring: ~4_CHARGES~<br>Fletching: ~5_CHARGES~<br>Masonry: ~6_CHARGES~<br>Glassblowing: ~7_CHARGES~
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
            writer.Write((int)0);

            writer.Write((bool)m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
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
        private RepairBenchAddon m_Addon;
        private RepairSkillType m_Skill;

        public ConfirmRemoveGump(RepairBenchAddon addon, RepairSkillType skill)
            : base(340, 340)
        {
            m_Addon = addon;
            m_Skill = skill;

            AddPage(0);

            AddBackground(0, 0, 291, 113, 0x13BE);
            AddImageTiled(5, 5, 280, 80, 0xA40);
            AddHtmlLocalized(9, 9, 272, 80, 1158874, String.Format("#{0}", addon.Tools.Find(x => x.Skill == skill).Cliloc), 0x7FFF, false, false); // Are you sure you wish to remove all the ~1_SKILL~ charges from the bench? This action will delete all existing charges and will not refund any deeds.

            AddButton(5, 87, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 89, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL

            AddButton(160, 87, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 89, 120, 20, 1006044, 0x7FFF, false, false); // OK
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int index = info.ButtonID;

            switch (index)
            {
                case 0: { break; }
                case 1:
                    {
                        var tool = m_Addon.Tools.Find(x => x.Skill == m_Skill);

                        tool.SkillValue = 0;
                        tool.Charges = 0;
                        break;
                    }
            }
        }
    }

    public class RepairBenchGump : Gump
    {
        private RepairBenchAddon m_Addon;

        public RepairBenchGump(RepairBenchAddon addon)
            : base(100, 100)
        {
            m_Addon = addon;

            AddPage(0);

            AddBackground(0, 0, 370, 470, 0x6DB);

            AddImage(82, 0, 0x6E4);
            AddHtmlLocalized(10, 10, 350, 18, 1114513, "#1158860", 0x0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            AddHtmlLocalized(70, 57, 120, 20, 1114513, "#1158878", 0x7FE0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddHtmlLocalized(213, 57, 190, 20, 1158879, 0x7FE0, false, false); // Charges

            AddItem(20, 80, 0x1EB8);
            AddTooltip(1044097);
            AddButton(70, 97, 0x15E1, 0x15E5, 2001, GumpButtonType.Reply, 0);
            AddLabel(113, 97, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Tinkering).SkillValue));
            AddLabel(218, 97, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Tinkering).Charges));
            AddButton(318, 97, 0x2716, 0x2716, 8601, GumpButtonType.Reply, 0);

            AddItem(20, 125, 0x0FB4);
            AddTooltip(1044067);
            AddButton(70, 137, 0x15E1, 0x15E5, 2002, GumpButtonType.Reply, 0);
            AddLabel(113, 137, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Smithing).SkillValue));
            AddLabel(218, 137, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Smithing).Charges));
            AddButton(318, 137, 0x2716, 0x2716, 8602, GumpButtonType.Reply, 0);

            AddItem(20, 170, 0x1034);
            AddTooltip(1044071);
            AddButton(70, 177, 0x15E1, 0x15E5, 2003, GumpButtonType.Reply, 0);
            AddLabel(113, 177, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Carpentry).SkillValue));
            AddLabel(218, 177, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Carpentry).Charges));
            AddButton(318, 177, 0x2716, 0x2716, 8603, GumpButtonType.Reply, 0);

            AddItem(20, 215, 0x0F9D);
            AddTooltip(1044094);
            AddButton(70, 217, 0x15E1, 0x15E5, 2006, GumpButtonType.Reply, 0);
            AddLabel(113, 217, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Tailoring).SkillValue));
            AddLabel(218, 217, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Tailoring).Charges));
            AddButton(318, 217, 0x2716, 0x2716, 8606, GumpButtonType.Reply, 0);

            AddItem(20, 260, 0x12B3);
            AddTooltip(1072392);
            AddButton(70, 257, 0x15E1, 0x15E5, 2010, GumpButtonType.Reply, 0);
            AddLabel(113, 257, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Masonry).SkillValue));
            AddLabel(218, 257, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Masonry).Charges));
            AddButton(318, 257, 0x2716, 0x2716, 8610, GumpButtonType.Reply, 0);

            AddItem(20, 305, 0x182D);
            AddTooltip(1072393);
            AddButton(70, 297, 0x15E1, 0x15E5, 2011, GumpButtonType.Reply, 0);
            AddLabel(113, 297, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Glassblowing).SkillValue));
            AddLabel(218, 297, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Glassblowing).Charges));
            AddButton(318, 297, 0x2716, 0x2716, 8611, GumpButtonType.Reply, 0);

            AddItem(20, 350, 0x1022);
            AddTooltip(1015156);
            AddButton(70, 337, 0x15E1, 0x15E5, 2009, GumpButtonType.Reply, 0);
            AddLabel(113, 337, 0x5F, String.Format("{0:F1}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Fletching).SkillValue));
            AddLabel(218, 337, 0x5F, String.Format("{0}", m_Addon.Tools.Find(x => x.Skill == RepairSkillType.Fletching).Charges));
            AddButton(318, 337, 0x2716, 0x2716, 8609, GumpButtonType.Reply, 0);

            AddButton(70, 407, 0x15E1, 0x15E5, 100, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 407, 200, 30, 1153100, 0x7FFF, false, false); // Add Charges
        }

        private class InternalTarget : Target
        {
            private RepairBenchAddon m_Addon;

            public InternalTarget(Mobile from, RepairBenchAddon addon)
                : base(-1, false, TargetFlags.None)
            {
                m_Addon = addon;
            }            

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Addon == null && m_Addon.Deleted)
                    return;

                if (targeted is RepairDeed)
                {
                    RepairDeed deed = (RepairDeed)targeted;

                    if (!m_Addon.CheckAccessible(from, m_Addon))
                    {
                        from.SendLocalizedMessage(1061637); // You are not allowed to access this.
                    }
                    else if (m_Addon.Tools.Any(x => x.Skill == deed.RepairSkill && x.SkillValue != 0 && x.SkillValue != deed.SkillLevel))
                    {
                        from.SendLocalizedMessage(1158866); // The repair bench contains deeds that do not match the skill of the deed you are trying to add.

                        from.CloseGump(typeof(RepairBenchGump));
                        from.SendGump(new RepairBenchGump(m_Addon));
                    }
                    else
                    {
                        var tool = m_Addon.Tools.Find(x => x.Skill == deed.RepairSkill);

                        tool.SkillValue = deed.SkillLevel;
                        tool.Charges++;

                        from.CloseGump(typeof(RepairBenchGump));
                        from.SendGump(new RepairBenchGump(m_Addon));

                        from.Target = new InternalTarget(from, m_Addon);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1158865); // That is not a valid repair contract or container.
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (m_Addon != null && !m_Addon.Deleted)
                {
                    from.CloseGump(typeof(RepairBenchGump));
                    from.SendGump(new RepairBenchGump(m_Addon));
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            int index = info.ButtonID;

            switch (index)
            {
                case 0:
                    {
                        break;
                    }
                case 2001:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Tinkering).System, m_Addon);

                        break;
                    }
                case 2002:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Smithing).System, m_Addon);

                        break;
                    }
                case 2003:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Carpentry).System, m_Addon);

                        break;
                    }
                case 2006:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Tailoring).System, m_Addon);

                        break;
                    }
                case 2010:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Masonry).System, m_Addon);

                        break;
                    }
                case 2011:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Glassblowing).System, m_Addon);

                        break;
                    }
                case 2009:
                    {
                        Repair.Do(from, RepairSkillInfo.GetInfo(RepairSkillType.Fletching).System, m_Addon);

                        break;
                    }
                case 8601:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Tinkering));

                        break;
                    }
                case 8602:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Smithing));

                        break;
                    }
                case 8603:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Carpentry));

                        break;
                    }
                case 8606:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Tailoring));

                        break;
                    }
                case 8610:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Masonry));

                        break;
                    }
                case 8611:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Glassblowing));

                        break;
                    }
                case 8609:
                    {
                        from.CloseGump(typeof(ConfirmRemoveGump));
                        from.SendGump(new ConfirmRemoveGump(m_Addon, RepairSkillType.Fletching));

                        break;
                    }
                case 100:
                    {                        
                        from.SendLocalizedMessage(1158871); // Which repair deed or container of repair deeds do you wish to add to the repair bench?
                        from.Target = new InternalTarget(from, m_Addon);

                        break;
                    }
            }                    
        }
    }
}
