using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using System;

namespace Server.Engines.Distillation
{
    public class DistillationGump : Gump
    {
        private const int LabelHue = 0x480;
        private const int LabelColor = 0x7FFF;
        private const int FontColor = 0xFFFFFF;

        private readonly DistillationContext m_Context;
        private readonly CraftDefinition m_Def;

        public DistillationGump(Mobile from) : base(35, 35)
        {
            from.CloseGump(typeof(DistillationGump));

            m_Context = DistillationSystem.GetContext(from);

            Group group = m_Context.LastGroup;
            Liquor liquor = m_Context.LastLiquor;

            if (liquor == Liquor.None)
                liquor = DistillationSystem.GetFirstLiquor(group);

            m_Def = DistillationSystem.GetDefinition(liquor, group);

            if (m_Def == null)
                return;

            if (m_Def.Liquor != liquor)
                liquor = m_Def.Liquor;

            AddBackground(0, 0, 500, 500, 5054);
            AddImageTiled(10, 10, 480, 30, 2624);
            AddImageTiled(10, 50, 230, 120, 2624);
            AddImageTiled(10, 180, 230, 200, 2624);
            AddImageTiled(250, 50, 240, 200, 2624);
            AddImageTiled(250, 260, 240, 120, 2624);
            AddImageTiled(10, 390, 480, 60, 2624);
            AddImageTiled(10, 460, 480, 30, 2624);
            AddAlphaRegion(10, 10, 480, 480);

            AddHtmlLocalized(10, 16, 510, 20, 1150682, LabelColor, false, false); // <center>DISTILLERY MENU</center>

            AddHtmlLocalized(10, 55, 230, 20, 1150683, LabelColor, false, false); // <center>Select the group</center>

            AddButton(15, 80, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 80, 200, 20, DistillationSystem.GetLabel(Group.WheatBased), LabelColor, false, false); // WHEAT BASED

            AddButton(15, 106, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 106, 200, 20, DistillationSystem.GetLabel(Group.WaterBased), LabelColor, false, false); // WATER  BASED

            AddButton(15, 132, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 132, 200, 20, DistillationSystem.GetLabel(Group.Other), LabelColor, false, false); // OTHER

            AddHtmlLocalized(10, 184, 230, 20, 1150684, LabelColor, false, false); // <center>Select the liquor type</center>

            int y = 210;
            for (int i = 0; i < DistillationSystem.CraftDefs.Count; i++)
            {
                CraftDefinition def = DistillationSystem.CraftDefs[i];

                if (def.Group == group)
                {
                    AddButton(15, y, 4005, 4007, 1000 + i, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(55, y, 200, 20, DistillationSystem.GetLabel(def.Liquor, false), LabelColor, false, false);
                    y += 26;
                }
            }

            AddHtmlLocalized(250, 54, 240, 20, 1150735, string.Format("#{0}", DistillationSystem.GetLabel(liquor, false)), LabelColor, false, false); // <center>Ingredients of ~1_NAME~</center>

            y = 80;
            for (int i = 0; i < m_Def.Ingredients.Length; i++)
            {
                Type type = m_Def.Ingredients[i];
                int amt = m_Def.Amounts[i];
                bool strong = m_Context.MakeStrong;

                if (i == 0 && type == typeof(Yeast))
                {
                    for (int j = 0; j < amt; j++)
                    {
                        Yeast yeast = m_Context.SelectedYeast[j];

                        AddHtmlLocalized(295, y, 200, 20, yeast == null ? 1150778 : 1150779, "#1150453", LabelColor, false, false); // Yeast: ~1_VAL~
                        AddButton(255, y, 4005, 4007, 2000 + j, GumpButtonType.Reply, 0);
                        y += 26;
                    }

                    continue;
                }
                else
                {
                    int total = strong ? amt * 2 : amt;
                    int amount = DistillationTarget.GetAmount(from, type, liquor);
                    if (amount > total)
                        amount = total;
                    AddHtmlLocalized(295, y, 200, 20, 1150733, string.Format("#{0}\t{1}", m_Def.Labels[i], string.Format("{0}/{1}", amount.ToString(), total.ToString())), LabelColor, false, false); // ~1_NAME~ : ~2_NUMBER~
                }

                y += 26;
            }

            AddHtmlLocalized(250, 264, 240, 20, 1150770, LabelColor, false, false); // <center>Distillery Options</center>

            AddButton(255, 290, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(295, 290, 200, 20, m_Context.MakeStrong ? 1150730 : 1150729, LabelColor, false, false); // Double Distillation - Standard Distillation

            AddButton(255, 316, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(295, 320, 200, 20, m_Context.Mark ? 1150731 : 1150732, LabelColor, false, false); // Mark Distiller Name - Do Not Mark

            AddButton(15, 395, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 395, 200, 20, 1150733, string.Format("Label\t{0}", m_Context.Label == null ? "None" : m_Context.Label), LabelColor, false, false); // ~1_NAME~ : ~2_NUMBER~

            AddButton(15, 465, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 465, 200, 20, 1150771, LabelColor, false, false); // Execute Distillation

            AddButton(455, 465, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(400, 465, 100, 20, 1060675, LabelColor, false, false); // CLOSE
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: // Select Wheat Based
                    m_Context.LastGroup = Group.WheatBased;
                    break;
                case 2: // Select Water Based
                    m_Context.LastGroup = Group.WaterBased;
                    break;
                case 3: // Select Other
                    m_Context.LastGroup = Group.Other;
                    break;
                case 4: // Distillation Strength
                    if (m_Context.MakeStrong)
                        m_Context.MakeStrong = false;
                    else
                        m_Context.MakeStrong = true;
                    break;
                case 5: // Mark Option
                    if (m_Context.Mark)
                        m_Context.Mark = false;
                    else
                        m_Context.Mark = true;
                    break;
                case 6: // Label
                    from.Prompt = new LabelPrompt(m_Context);
                    from.SendLocalizedMessage(1150734); // Please enter the text with which you wish to label the liquor that you will distill. You many enter up to 15 characters. Leave the text area blank to remove any existing text.
                    return;
                case 7: // Execute Distillation
                    from.Target = new DistillationTarget(from, m_Context, m_Def);
                    from.SendLocalizedMessage(1150810); // Target an empty liquor barrel in your backpack. If you don't have one already, you can use the Carpentry skill to make one.
                    return;
                default:
                    {
                        if (info.ButtonID < 2000) // Select Liquor Type
                        {
                            int sel = info.ButtonID - 1000;
                            if (sel >= 0 && sel < DistillationSystem.CraftDefs.Count)
                            {
                                CraftDefinition def = DistillationSystem.CraftDefs[sel];
                                m_Context.LastLiquor = def.Liquor;
                            }
                        }
                        else				      // Select Yeast
                        {
                            int sel = info.ButtonID - 2000;

                            if (m_Def.Ingredients[0] == typeof(Yeast) && m_Def.Amounts.Length > 0)
                            {
                                int amt = m_Def.Amounts[0];

                                if (sel >= 0 && sel < amt)
                                {
                                    from.Target = new YeastSelectionTarget(m_Context, sel);
                                    from.SendLocalizedMessage(1150437); // Target the yeast in your backpack that you wish to toggle distillation item status on.  Hit <ESC> to cancel.
                                    return;
                                }
                            }
                        }
                    }
                    break;
            }

            from.SendGump(new DistillationGump(from));
        }

        private class LabelPrompt : Prompt
        {
            private readonly DistillationContext m_Context;

            public LabelPrompt(DistillationContext context)
            {
                m_Context = context;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text == null || text.Length == 0)
                    m_Context.Label = null;
                else if (text != null)
                {
                    text = text.Trim();
                    if (text.Length > 15 || !Guilds.BaseGuildGump.CheckProfanity(text))
                        from.SendMessage("That label is unacceptable. Please try again.");
                    else
                        m_Context.Label = text;
                }

                from.SendGump(new DistillationGump(from));
            }

            public override void OnCancel(Mobile from)
            {
                from.SendGump(new DistillationGump(from));
            }


        }

        public class DistillationTarget : Target
        {
            private readonly DistillationContext m_Context;
            private readonly CraftDefinition m_Def;

            public DistillationTarget(Mobile from, DistillationContext context, CraftDefinition def)
                : base(-1, false, TargetFlags.None)
            {
                m_Context = context;
                m_Def = def;
                BeginTimeout(from, TimeSpan.FromSeconds(30));
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is LiquorBarrel)
                {
                    LiquorBarrel barrel = targeted as LiquorBarrel;

                    if (barrel.IsChildOf(from.Backpack))
                    {
                        if (barrel.IsMature)
                            from.SendLocalizedMessage(1150811); // This liquor barrel already contains liquor.
                        else if (!barrel.IsEmpty)
                            from.SendLocalizedMessage(1150802); // You realize that the liquor is on the process of maturation so you leave it alone.
                        else
                        {
                            double perc = m_Context.MakeStrong ? 2 : 1;

                            if (HasTotal(from, perc) && barrel != null)
                            {
                                double cooking = from.Skills[SkillName.Cooking].Value;
                                double alchemy = from.Skills[SkillName.Alchemy].Value / 2;
                                int resist = 0;

                                for (int i = 0; i < m_Context.SelectedYeast.Length; i++)
                                {
                                    Yeast yeast = m_Context.SelectedYeast[i];

                                    if (yeast != null)
                                        resist += yeast.BacterialResistance;
                                }

                                int chance = (int)((cooking + alchemy + (resist * 12)) / 2.5);

                                if (chance > Utility.Random(100))
                                {
                                    ConsumeTotal(from, perc);
                                    m_Context.ClearYeasts();
                                    from.SendLocalizedMessage(1150772); // You succeed at your distillation attempt.

                                    Mobile marker = m_Context.Mark ? from : null;
                                    from.PlaySound(0x2D6);

                                    barrel.BeginDistillation(m_Def.Liquor, m_Def.MaturationDuration, m_Context.Label, m_Context.MakeStrong, marker);
                                }
                                else
                                {
                                    from.PlaySound(0x2D6);
                                    ConsumeTotal(from, perc / 2);
                                    from.SendLocalizedMessage(1150745); // You have failed your distillation attempt and ingredients have been lost.
                                }
                            }
                            else
                                from.SendLocalizedMessage(1150747); // You don't have enough ingredients.
                        }
                    }
                    else
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                }
                else
                    from.SendMessage("That is not a liquor barrel.");

                DistillationSystem.SendDelayedGump(from);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Timeout)
                    from.SendLocalizedMessage(1150859); // You have waited too long to make your selection, your distillation attempt has timed out.

                from.SendGump(new DistillationGump(from));
            }

            public static int GetAmount(Mobile from, Type type, Liquor liquor)
            {
                if (from == null || from.Backpack == null)
                    return 0;

                Container pack = from.Backpack;

                if (type == typeof(Pitcher))
                {
                    int amount = 0;
                    BeverageType bType = liquor == Liquor.Brandy ? BeverageType.Wine : BeverageType.Water;

                    Item[] items = pack.FindItemsByType(type);

                    foreach (Item item in items)
                    {
                        Pitcher Pitcher = item as Pitcher;

                        if (Pitcher != null && Pitcher.Content == bType && Pitcher.Quantity >= Pitcher.MaxQuantity)
                            amount++;
                    }

                    return amount;
                }
                else if (type == typeof(PewterBowlOfCorn))
                {
                    return pack.GetAmount(type) + pack.GetAmount(typeof(WoodenBowlOfCorn));
                }

                return pack.GetAmount(type);
            }

            private bool HasTotal(Mobile from, double percentage)
            {
                for (int i = 0; i < m_Def.Ingredients.Length; i++)
                {
                    Type type = m_Def.Ingredients[i];
                    int toConsume = m_Def.Amounts[i];

                    if (i == 0 && type == typeof(Yeast))
                    {
                        for (int j = 0; j < toConsume; j++)
                        {
                            Yeast yeast = m_Context.SelectedYeast[j];

                            if (yeast == null || !yeast.IsChildOf(from.Backpack))
                                return false;
                        }
                    }
                    else
                    {
                        toConsume = (int)(toConsume * percentage);

                        if (GetAmount(from, type, m_Def.Liquor) < toConsume)
                            return false;
                    }
                }

                return true;
            }

            private void ConsumeTotal(Mobile from, double percentage)
            {
                if (from == null || from.Backpack == null)
                    return;

                Container pack = from.Backpack;

                for (int i = 0; i < m_Def.Ingredients.Length; i++)
                {
                    Type type = m_Def.Ingredients[i];
                    int toConsume = m_Def.Amounts[i];

                    if (i == 0 && type == typeof(Yeast))
                    {
                        for (int j = 0; j < toConsume; j++)
                        {
                            Yeast yeast = m_Context.SelectedYeast[j];

                            if (yeast != null)
                                yeast.Consume();
                        }
                    }
                    else if (type == typeof(Pitcher))
                    {
                        toConsume = (int)(toConsume * percentage);
                        BeverageType bType = m_Def.Liquor == Liquor.Brandy ? BeverageType.Wine : BeverageType.Water;

                        Item[] items = pack.FindItemsByType(type);

                        foreach (Item item in items)
                        {
                            Pitcher Pitcher = item as Pitcher;

                            if (Pitcher != null && Pitcher.Content == bType && Pitcher.Quantity >= Pitcher.MaxQuantity)
                            {
                                Pitcher.Quantity = 0;
                                toConsume--;
                            }

                            if (toConsume <= 0)
                                break;
                        }
                    }
                    else if (type == typeof(PewterBowlOfCorn))
                    {
                        toConsume = (int)(toConsume * percentage);

                        int totalPewter = pack.GetAmount(type);
                        int totalWooden = pack.GetAmount(typeof(WoodenBowlOfCorn));

                        if (totalPewter >= toConsume)
                            pack.ConsumeTotal(type, toConsume);
                        else
                        {
                            pack.ConsumeTotal(type, totalPewter);
                            toConsume -= totalPewter;
                            pack.ConsumeTotal(typeof(WoodenBowlOfCorn), toConsume);
                        }
                    }
                    else
                    {
                        toConsume = (int)(toConsume * percentage);
                        pack.ConsumeTotal(type, toConsume);
                    }
                }
            }
        }

        private class YeastSelectionTarget : Target
        {
            private readonly int m_Index;
            private readonly DistillationContext m_Context;

            public YeastSelectionTarget(DistillationContext context, int index)
                : base(-1, false, TargetFlags.None)
            {
                m_Index = index;
                m_Context = context;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Yeast)
                {
                    Yeast yeast = targeted as Yeast;

                    if (yeast.IsChildOf(from.Backpack))
                    {
                        if (!m_Context.YeastInUse(yeast))
                        {
                            m_Context.SelectedYeast[m_Index] = yeast;
                            from.SendLocalizedMessage(1150740); // You have chosen the yeast.
                        }
                        else
                            from.SendLocalizedMessage(1150742); // You have already chosen other yeast.
                    }
                    else
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                }
                else
                    from.SendLocalizedMessage(1150741); // That is not yeast.

                from.SendGump(new DistillationGump(from));
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (m_Context.SelectedYeast[m_Index] != null)
                {
                    m_Context.SelectedYeast[m_Index] = null;
                    from.SendLocalizedMessage(1150743); // You no longer choose this yeast.
                }

                from.SendGump(new DistillationGump(from));
            }
        }
    }
}
