using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Gumps
{
    public class RunicReforgingGump : BaseQuestGump
    {
        [Flags]
        public enum ReforgingOption
        {
            None                = 0x00000000,
            Powerful            = 0x00000001,
            Structural          = 0x00000002,
            Fortified           = 0x00000004,
            Fundamental         = 0x00000008,
            Integral            = 0x00000010,
            GrandArtifice       = 0x00000020,
            InspiredArtifice    = 0x00000040,
            ExaltedArtifice     = 0x00000080,
            SublimeArtifice     = 0x00000100,
        }

        public static readonly int Orange = 31137;

        private BaseRunicTool m_Tool;
        private Item m_ToReforge;
        private ReforgingOption m_Options;
        private ReforgedPrefix m_Prefix;
        private ReforgedSuffix m_Suffix;

        public RunicReforgingGump(Mobile from, Item toReforge, BaseRunicTool tool) : this(from, toReforge, tool, ReforgingOption.None, ReforgedPrefix.None, ReforgedSuffix.None)
        {
        }

        public RunicReforgingGump(Mobile from, Item toReforge, BaseRunicTool tool, ReforgingOption options, ReforgedPrefix prefix, ReforgedSuffix suffix)
            : base(25, 25)
        {
            m_Tool = tool;
            m_ToReforge = toReforge;
            m_Options = options;
            m_Prefix = prefix;
            m_Suffix = suffix;

            AddBackground(0, 0, 374, 444, 83);

            AddHtmlObject(120, 13, 200, 20, 1151952, DarkGreen, false, false); // Runic Crafting Options

            int buttonHue = White;
            int buttonID = 0x4005;
            int y = 40;
            int idx = 0;

            foreach (int i in Enum.GetValues(typeof(ReforgingOption)))
            {
                if (i == 0x00000000)
                    continue;

                ReforgingOption option = (ReforgingOption)i;

                if ((m_Options & option) != 0)
                {
                    if (CanReforge(from, option) && HasMetPrerequisite(option))
                    {
                        buttonHue = LightGreen;
                        buttonID = 4006;
                    }
                    else
                    {
                        buttonHue = Orange;
                        buttonID = 4006;
                    }
                }
                else
                {
                    if (CanReforge(from, option) && HasMetPrerequisite(option))
                    {
                        buttonHue = White;
                        buttonID = 4005;
                    }
                    else
                    {
                        buttonHue = Orange;
                        buttonID = 4006;
                    }
                }

                if(HasMetPrerequisite(option) && CanReforge(from, option))
                    AddButton(15, y, buttonID, buttonID, i + 100, GumpButtonType.Reply, 0);

                AddHtmlObject(55, y, 250, 20, GetCliloc(option), buttonHue, false, false);

                y += 25;
                idx++;
            }

            int totalCharges = GetTotalCharges();

            if ((m_Options & ReforgingOption.InspiredArtifice) != 0)
            {
                AddButton(15, 305, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlObject(55, 305, 250, 20, 1152087, White, false, false);
                AddHtmlObject(190, 305, 250, 20, RunicReforging.GetName((int)m_Prefix), LightGreen, false, false);
            }

            if ((m_Options & ReforgingOption.SublimeArtifice) != 0)
            {
                AddButton(15, 330, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlObject(55, 330, 250, 20, 1152088, White, false, false);
                AddHtmlObject(190, 330, 250, 20, RunicReforging.GetName((int)m_Suffix), LightGreen, false, false);
            }

            AddHtmlObject(30, 360, 200, 20, 1152078, LightGreen, false, false); // CHARGES NEEDED:
            AddHtmlObject(160, 360, 50, 20, totalCharges.ToString(), LightGreen, false, false);

            AddHtmlObject(45, 380, 200, 20, 1152077, LightGreen, false, false); // TOOL CHARGES:
            AddHtmlObject(160, 380, 50, 20, m_Tool.UsesRemaining.ToString(), LightGreen, false, false);

            AddButton(15, 410, 4017, 4018, 0, GumpButtonType.Reply, 0);
            AddHtmlObject(55, 410, 200, 20, "CLOSE", White, false, false);

            AddButton(330, 360, 4014, 4016, 3, GumpButtonType.Reply, 0);
            AddHtmlObject(230, 360, 200, 20, 1152080, White, false, false); // REFORGE ITEM

            AddButton(330, 410, 4011, 4013, 4, GumpButtonType.Reply, 0);
            AddHtmlObject(290, 410, 100, 20, "HELP", White, false, false);
        }

        private bool HasMetPrerequisite(ReforgingOption option)
        {
            switch (option)
            {
                case ReforgingOption.None: return true;
                case ReforgingOption.Powerful: return true;
                case ReforgingOption.Structural: return true;
                case ReforgingOption.Fortified: return (m_Options & ReforgingOption.Structural) != 0;
                case ReforgingOption.Fundamental: return true;
                case ReforgingOption.Integral: return (m_Options & ReforgingOption.Fundamental) != 0;
                case ReforgingOption.GrandArtifice: return true;
                case ReforgingOption.InspiredArtifice: return (m_Options & ReforgingOption.GrandArtifice) != 0;
                case ReforgingOption.ExaltedArtifice: return (m_Options & ReforgingOption.GrandArtifice) != 0;
                case ReforgingOption.SublimeArtifice: return (m_Options & ReforgingOption.ExaltedArtifice) != 0;
            }
            return true;
        }

        private bool CanReforge(Mobile from, ReforgingOption option)
        {
            double skill = from.Skills[SkillName.Imbuing].Value;

            switch (option)
            {
                default:
                case ReforgingOption.None:
                case ReforgingOption.Powerful: return skill >= 65.0;
                case ReforgingOption.Structural:
                case ReforgingOption.Fortified: return skill >= 85.0;
                case ReforgingOption.Fundamental:
                case ReforgingOption.Integral: return skill >= 97.0;
                case ReforgingOption.GrandArtifice:
                case ReforgingOption.InspiredArtifice: return skill >= 100.1;
                case ReforgingOption.ExaltedArtifice:
                case ReforgingOption.SublimeArtifice: return skill >= 110.1;
            }
        }

        private int GetCliloc(ReforgingOption option)
        {
            switch (option)
            {
                default:
                case ReforgingOption.None: return 0;
                case ReforgingOption.Powerful: return 1151954;
                case ReforgingOption.Structural: return 1151955;
                case ReforgingOption.Fortified: return 1151956;
                case ReforgingOption.Fundamental: return 1151957;
                case ReforgingOption.Integral: return 1151958;
                case ReforgingOption.GrandArtifice: return 1151961;
                case ReforgingOption.InspiredArtifice: return 1151962;
                case ReforgingOption.ExaltedArtifice: return 1151963;
                case ReforgingOption.SublimeArtifice: return 1151964;
            }
        }

        private int GetTotalCharges()
        {
            int count = 1;

            foreach (int i in Enum.GetValues(typeof(ReforgingOption)))
            {
                if ((m_Options & (ReforgingOption)i) != 0)
                    count++;
            }

            return Math.Min(10, count);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: break;
                case 1: // prefix
                    from.SendGump(new ItemNameGump(m_ToReforge, m_Tool, m_Options, m_Prefix, m_Suffix, true));
                    break;
                case 2: // suffix
                    from.SendGump(new ItemNameGump(m_ToReforge, m_Tool, m_Options, m_Prefix, m_Suffix, false));
                    break;
                case 3: // Reforge Item
                    {
                        int totalCharges = GetTotalCharges();

                        if (m_Tool.UsesRemaining >= totalCharges)
                        {
                            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Tool.Resource);

                            if (resInfo == null)
                                return;

                            CraftAttributeInfo attrs = resInfo.AttributeInfo;
                            int budget = GetBudget();
                            int powerMod = GetPowerMod();

                            int min = 10;
                            int max = 40;

                            if (attrs != null)
                            {
                                min = attrs.RunicMinIntensity;
                                max = attrs.RunicMaxIntensity;
                            }

                            if (min < 10) min = 10;
                            if (max > 100) max = 100;

                            if (m_Prefix == ReforgedPrefix.None && (m_Options & ReforgingOption.GrandArtifice) != 0)
                            {
                                m_Prefix = RunicReforging.ChooseRandomPrefix(m_ToReforge);
                                budget = Math.Min(800, budget + 25);
                            }

                            if (m_Suffix == ReforgedSuffix.None && (m_Options & ReforgingOption.ExaltedArtifice) != 0)
                            {
                                m_Suffix = RunicReforging.ChooseRandomSuffix(m_ToReforge, m_Prefix);
                                budget = Math.Min(800, budget + 25);
                            }

                            int maxprops = Math.Min(5, (budget / 110) + 1);
                            if (maxprops == 5 && 0.10 > Utility.RandomDouble())
                                maxprops = 6;
                            
							RunicReforging.ApplyReforgedProperties(m_ToReforge, m_Prefix, m_Suffix, true, budget, min, max, maxprops, powerMod, 0);

                            OnAfterReforged(m_ToReforge);
                            from.SendLocalizedMessage(1152286); // You re-forge the item!
                            m_Tool.UsesRemaining -= totalCharges;

                            if (m_Tool != null && m_Tool.CraftSystem != null)
                                m_Tool.CraftSystem.PlayCraftEffect(from);

                            if (m_Tool.UsesRemaining <= 0)
                            {
                                m_Tool.Delete();
                                from.SendLocalizedMessage(1044038); // You have worn out your tool!
                            }
                        }

                        break;
                    }
                case 4:
                    from.SendGump(new ReforgingHelpGump());
                    break;
                default: // Option
                    {
                        int index = info.ButtonID - 100;

                        if (index > (int)ReforgingOption.None && index <= (int)ReforgingOption.SublimeArtifice)
                        {
                            ReforgingOption option = (ReforgingOption)index;

                            if (HasMetPrerequisite(option))
                            {
                                if ((m_Options & option) == 0)
                                    m_Options |= option;
                                else
                                    m_Options ^= option;
                            }
                        }

                        from.SendGump(new RunicReforgingGump(from, m_ToReforge, m_Tool, m_Options, m_Prefix, m_Suffix));
                        break;
                    }
            }
        }

        private int GetPowerMod()
        {
            if ((m_Options & ReforgingOption.Fundamental) != 0)
                return 0;

            if ((m_Options & ReforgingOption.Structural) != 0)
                return 10;

            if ((m_Options & ReforgingOption.Powerful) != 0)
                return 20;

            return 30;
        }

        private int GetBudget()
        {
            int budget = 250;

            switch (m_Tool.Resource)
            {
                default:
                    break;
                case CraftResource.ShadowIron:
                case CraftResource.Copper:
                case CraftResource.SpinedLeather:
                case CraftResource.AshWood:
                    budget = 400; break;
                case CraftResource.Bronze:
                case CraftResource.Gold:
                case CraftResource.HornedLeather:
                case CraftResource.YewWood:
                    budget = 550; break;
                case CraftResource.Agapite:
                case CraftResource.Verite: 
                    budget = 700; break;
                case CraftResource.Valorite:
                case CraftResource.BarbedLeather:
                case CraftResource.Heartwood:
                    budget = 800; break;
            }
            
            return budget;
        }

        public void OnAfterReforged(Item item)
        {
            AosAttributes attr = null;
            int durability = 0;

            if (item is BaseWeapon)
                attr = ((BaseWeapon)item).Attributes;

            else if (item is BaseArmor)
                attr = ((BaseArmor)item).Attributes;

            if (attr != null && (m_Options & ReforgingOption.Structural) != 0)
            {
                attr.Brittle = 1;

                if ((m_Options & ReforgingOption.Fortified) != 0)
                    durability = 150;
            }

            if ((m_Options & ReforgingOption.Fundamental) != 0)
            {
                RunicReforging.SetBlockRepair(item);
                durability = (m_Options & ReforgingOption.Integral) != 0 ? 255 : 200;
            }

            if (durability > 0 && item is IDurability)
            {
                ((IDurability)item).MaxHitPoints = durability;
                ((IDurability)item).HitPoints = durability;
            }
        }

        public class ItemNameGump : BaseQuestGump
        {
            private BaseRunicTool m_Tool;
            private Item m_ToReforge;
            private ReforgingOption m_Options;
            private ReforgedPrefix m_Prefix;
            private ReforgedSuffix m_Suffix;
            private bool m_IsPrefix;

            public ItemNameGump(Item toreforge, BaseRunicTool tool, ReforgingOption options, ReforgedPrefix prefix, ReforgedSuffix suffix, bool isprefix)
                : base(25, 25)
            {
                m_Tool = tool;
                m_ToReforge = toreforge;
                m_Options = options;
                m_Prefix = prefix;
                m_Suffix = suffix;
                m_IsPrefix = isprefix;

                AddBackground(0, 0, 376, 445, 83);

                AddHtmlObject(72, 10, 250, 100, 1152089, DarkGreen, false, false);

                int buttonID = 4005;
                int buttonHue = White;
                int y = 50;

                foreach (int i in Enum.GetValues(typeof(ReforgedPrefix)))
                {
                    if ((isprefix && prefix == (ReforgedPrefix)i) || (!isprefix && suffix == (ReforgedSuffix)i))
                    {
                        buttonID = 4006;
                        buttonHue = LightGreen;
                    }
                    else
                    {
                        buttonID = 4005;
                        buttonHue = White;
                    }

                    if(HasSelection(i, toreforge, tool))
                    {
                        AddButton(15, y, buttonID, buttonID, 100 + i, GumpButtonType.Reply, 0);
                        AddHtmlObject(55, y, 250, 20, RunicReforging.GetName(i), buttonHue, false, false);
                    }

                    y += 25;
                }

                AddHtmlObject(55, 410, 100, 20, "CLOSE", White, false, false);
                AddButton(15, 410, 4017, 4019, 0, GumpButtonType.Reply, 0);
            }

            private bool HasSelection(int index, Item toreforge, BaseRunicTool tool)
            {
                // No Vampire prefix/suffix for non-weapons
                if (index == 6 && !(toreforge is BaseWeapon))
                    return false;

                if (index != 0 && (index == (int)m_Prefix || index == (int)m_Suffix))
                    return false;

                return true;
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;

                if (info.ButtonID == 0)
                    return;

                int index = info.ButtonID - 100;

                if (index >= 0 && index <= 12 && HasSelection(index, m_ToReforge, m_Tool))
                {
                    if (m_IsPrefix)
                    {
                        m_Prefix = (ReforgedPrefix)index;
                    }
                    else
                    {
                        m_Suffix = (ReforgedSuffix)index;
                    }
                }

                from.SendGump(new RunicReforgingGump(from, m_ToReforge, m_Tool, m_Options, m_Prefix, m_Suffix));
            }
        }

        private class ReforgingHelpGump : BaseQuestGump
        {
            public ReforgingHelpGump()
                : base(25, 25)
            {
                AddBackground(0, 0, 384, 451, 83);

                AddHtmlObject(125, 13, 250, 20, 1151966, DarkGreen, false, false);
                AddHtmlObject(10, 40, 364, 370, 1151965, 0xFFE0, true, true);

                AddButton(10, 415, 4017, 4019, 0, GumpButtonType.Reply, 0);
                AddHtmlObject(50, 418, 100, 20, "CLOSE", White, false, false);
            }
        }
    }
}
