using System;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Craft
{
    public class CraftGumpItem : Gump
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly CraftItem m_CraftItem;
        private readonly ITool m_Tool;

        private const int LabelHue = 0x480; // 0x384
        private const int RedLabelHue = 0x20;

        private const int LabelColor = 0x7FFF;
        private const int RedLabelColor = 0x6400;

        private const int GreyLabelColor = 0x3DEF;

        private int m_OtherCount;

        public CraftGumpItem(Mobile from, CraftSystem craftSystem, CraftItem craftItem, ITool tool)
            : base(40, 40)
        {
            m_From = from;
            m_CraftSystem = craftSystem;
            m_CraftItem = craftItem;
            m_Tool = tool;

            from.CloseGump(typeof(CraftGump));
            from.CloseGump(typeof(CraftGumpItem));

            AddPage(0);
            AddBackground(0, 0, 530, 417, 5054);
            AddImageTiled(10, 10, 510, 22, 2624);
            AddImageTiled(10, 37, 150, 148, 2624);
            AddImageTiled(165, 37, 355, 90, 2624);
            AddImageTiled(10, 190, 155, 22, 2624);
            AddImageTiled(10, 240, 150, 57, 2624);
            AddImageTiled(165, 132, 355, 80, 2624);
            AddImageTiled(10, 325, 150, 57, 2624);
            AddImageTiled(165, 217, 355, 80, 2624);
            AddImageTiled(165, 302, 355, 80, 2624);
            AddImageTiled(10, 387, 510, 22, 2624);
            AddAlphaRegion(10, 10, 510, 399);

            AddHtmlLocalized(170, 40, 150, 20, 1044053, LabelColor, false, false); // ITEM
            AddHtmlLocalized(10, 217, 150, 22, 1044055, LabelColor, false, false); // <CENTER>MATERIALS</CENTER>
            AddHtmlLocalized(10, 302, 150, 22, 1044056, LabelColor, false, false); // <CENTER>OTHER</CENTER>

            if (craftSystem.GumpTitleNumber > 0)
                AddHtmlLocalized(10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false);
            else
                AddHtml(10, 12, 510, 20, craftSystem.GumpTitleString, false, false);

            bool needsRecipe = (craftItem.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe(craftItem.Recipe));

            if (needsRecipe)
            {
                AddButton(405, 387, 4005, 4007, 0, GumpButtonType.Page, 0);
                AddHtmlLocalized(440, 390, 150, 18, 1044151, GreyLabelColor, false, false); // MAKE NOW
            }
            else
            {
                AddButton(405, 387, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(445, 390, 150, 18, 1044151, LabelColor, false, false); // MAKE NOW
            }

            #region Stygian Abyss
            AddButton(265, 387, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(300, 390, 150, 18, 1112623, LabelColor, false, false); //MAKE NUMBER

            AddButton(135, 387, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(170, 390, 150, 18, 1112624, LabelColor, false, false); //MAKE MAX
            #endregion

            AddButton(15, 387, 4014, 4016, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 390, 150, 18, 1044150, LabelColor, false, false); // BACK

            if (craftItem.NameNumber > 0)
                AddHtmlLocalized(330, 40, 180, 18, craftItem.NameNumber, LabelColor, false, false);
            else
                AddLabel(330, 40, LabelHue, craftItem.NameString);

            if (craftItem.UseAllRes)
                AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1048176, LabelColor, false, false); // Makes as many as possible at once

            DrawItem();
            DrawSkill();
            DrawResource();

            /*
            if( craftItem.RequiresSE )
            AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 310, 18, 1063363, LabelColor, false, false ); //* Requires the "Samurai Empire" expansion
            * */

            if (craftItem.RequiredExpansion != Expansion.None)
            {
                bool supportsEx = (from.NetState != null && from.NetState.SupportsExpansion(craftItem.RequiredExpansion));
                TextDefinition.AddHtmlText(this, 170, 302 + (m_OtherCount++ * 20), 310, 18, RequiredExpansionMessage(craftItem.RequiredExpansion), false, false, supportsEx ? LabelColor : RedLabelColor, supportsEx ? LabelHue : RedLabelHue);
            }

            if (craftItem.RequiredThemePack != ThemePack.None)
            {
                TextDefinition.AddHtmlText(this, 170, 302 + (m_OtherCount++ * 20), 310, 18, RequiredThemePackMessage(craftItem.RequiredThemePack), false, false, LabelColor, LabelHue);
            }

            if (needsRecipe)
                AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1073620, RedLabelColor, false, false); // You have not learned this recipe.
        }

        private TextDefinition RequiredExpansionMessage(Expansion expansion)
        {
            switch( expansion )
            {
                case Expansion.SE:
                    return 1063363; // * Requires the "Samurai Empire" expansion
                case Expansion.ML:
                    return 1072651; // * Requires the "Mondain's Legacy" expansion
                case Expansion.SA:
                    return 1094732; // * Requires the "Stygian Abyss" expansion
                case Expansion.HS:
                    return 1116296; // * Requires the "High Seas" booster
                case Expansion.TOL:
                    return 1155876; // * Requires the "Time of Legends" expansion.
                default:
                    return String.Format("* Requires the \"{0}\" expansion", ExpansionInfo.GetInfo(expansion).Name);
            }
        }

        private TextDefinition RequiredThemePackMessage(ThemePack pack)
        {
            switch (pack)
            {
                case ThemePack.Kings:
                    return 1154195; // *Requires the "King's Collection" theme pack
                case ThemePack.Rustic:
                    return 1150651; // * Requires the "Rustic" theme pack
                case ThemePack.Gothic:
                    return 1150650; // * Requires the "Gothic" theme pack
                default:
                    return String.Format("Requires the \"{0}\" theme pack.", null);
            }
        }

        private bool m_ShowExceptionalChance;

        public void DrawItem()
        {
            Type type = m_CraftItem.ItemType;
            int id = m_CraftItem.DisplayID;
            if (id == 0) id = CraftItem.ItemIDOf(type);
            Rectangle2D b = ItemBounds.Table[id];
            AddItem(90 - b.Width / 2 - b.X, 110 - b.Height / 2 - b.Y, id, m_CraftItem.ItemHue);

            if (m_CraftItem.IsMarkable(type))
            {
                AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1044059, LabelColor, false, false); // This item may hold its maker's mark
                m_ShowExceptionalChance = true;
            }
        }

        public void DrawSkill()
        {
            for (int i = 0; i < m_CraftItem.Skills.Count; i++)
            {
                CraftSkill skill = m_CraftItem.Skills.GetAt(i);
                double minSkill = skill.MinSkill, maxSkill = skill.MaxSkill;

                if (minSkill < 0)
                    minSkill = 0;

                AddHtmlLocalized(170, 132 + (i * 20), 200, 18, AosSkillBonuses.GetLabel(skill.SkillToMake), LabelColor, false, false);
                AddLabel(430, 132 + (i * 20), LabelHue, String.Format("{0:F1}", minSkill));
            }

            CraftSubResCol res = (m_CraftItem.UseSubRes2 ? m_CraftSystem.CraftSubRes2 : m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            CraftContext context = m_CraftSystem.GetContext(m_From);

            if (context != null)
                resIndex = (m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool allRequiredSkills = true;
            double chance = m_CraftItem.GetSuccessChance(m_From, resIndex > -1 ? res.GetAt(resIndex).ItemType : null, m_CraftSystem, false, ref allRequiredSkills);
            double excepChance = m_CraftItem.GetExceptionalChance(m_CraftSystem, chance, m_From);

            if (chance < 0.0)
                chance = 0.0;
            else if (chance > 1.0)
                chance = 1.0;

            AddHtmlLocalized(170, 80, 250, 18, 1044057, LabelColor, false, false); // Success Chance:
            AddLabel(430, 80, LabelHue, String.Format("{0:F1}%", chance * 100));

            if (m_ShowExceptionalChance)
            {
                if (excepChance < 0.0)
                    excepChance = 0.0;
                else if (excepChance > 1.0)
                    excepChance = 1.0;

                AddHtmlLocalized(170, 100, 250, 18, 1044058, 32767, false, false); // Exceptional Chance:
                AddLabel(430, 100, LabelHue, String.Format("{0:F1}%", excepChance * 100));
            }
        }

        private static readonly Type typeofBlankScroll = typeof(BlankScroll);
        private static readonly Type typeofSpellScroll = typeof(SpellScroll);

        public void DrawResource()
        {
            bool retainedColor = false;

            CraftContext context = m_CraftSystem.GetContext(m_From);

            CraftSubResCol res = (m_CraftItem.UseSubRes2 ? m_CraftSystem.CraftSubRes2 : m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            if (context != null)
                resIndex = (m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool cropScroll = (m_CraftItem.Resources.Count > 1) &&
                              m_CraftItem.Resources.GetAt(m_CraftItem.Resources.Count - 1).ItemType == typeofBlankScroll &&
                              typeofSpellScroll.IsAssignableFrom(m_CraftItem.ItemType);

            for (int i = 0; i < m_CraftItem.Resources.Count - (cropScroll ? 1 : 0) && i < 4; i++)
            {
                Type type;
                string nameString;
                int nameNumber;

                CraftRes craftResource = m_CraftItem.Resources.GetAt(i);

                type = craftResource.ItemType;
                nameString = craftResource.NameString;
                nameNumber = craftResource.NameNumber;
				
                // Resource Mutation
                if (type == res.ResType && resIndex > -1)
                {
                    CraftSubRes subResource = res.GetAt(resIndex);

                    type = subResource.ItemType;

                    nameString = subResource.NameString;
                    nameNumber = subResource.GenericNameNumber;

                    if (nameNumber <= 0)
                        nameNumber = subResource.NameNumber;
                }
                // ******************

                if (!retainedColor && m_CraftItem.RetainsColorFrom(m_CraftSystem, type))
                {
                    retainedColor = true;
                    AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 310, 18, 1044152, LabelColor, false, false); // * The item retains the color of this material
                    AddLabel(500, 219 + (i * 20), LabelHue, "*");
                }

                if (nameNumber > 0)
                    AddHtmlLocalized(170, 219 + (i * 20), 310, 18, nameNumber, LabelColor, false, false);
                else
                    AddLabel(170, 219 + (i * 20), LabelHue, nameString);

                AddLabel(430, 219 + (i * 20), LabelHue, craftResource.Amount.ToString());
            }

            if (m_CraftItem.NameNumber == 1041267) // runebook
            {
                AddHtmlLocalized(170, 219 + (m_CraftItem.Resources.Count * 20), 310, 18, 1044447, LabelColor, false, false);
                AddLabel(430, 219 + (m_CraftItem.Resources.Count * 20), LabelHue, "1");
            }

            if (cropScroll)
                AddHtmlLocalized(170, 302 + (m_OtherCount++ * 20), 360, 18, 1044379, LabelColor, false, false); // Inscribing scrolls also requires a blank scroll and mana.
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: // Back Button
                    {
                        CraftGump craftGump = new CraftGump(m_From, m_CraftSystem, m_Tool, null);
                        m_From.SendGump(craftGump);
                        break;
                    }
                case 1: // Make Button
                    {
                        if (m_CraftItem.TryCraft != null)
                        {
                            m_CraftItem.TryCraft(m_From, m_CraftItem, m_Tool);
                            return;
                        }
                        
                        int num = m_CraftSystem.CanCraft(m_From, m_Tool, m_CraftItem.ItemType);

                        if (num > 0)
                        {
                            m_From.SendGump(new CraftGump(m_From, m_CraftSystem, m_Tool, num));
                        }
                        else
                        {
                            Type type = null;

                            CraftContext context = m_CraftSystem.GetContext(m_From);

                            if (context != null)
                            {
                                CraftSubResCol res = (m_CraftItem.UseSubRes2 ? m_CraftSystem.CraftSubRes2 : m_CraftSystem.CraftSubRes);
                                int resIndex = (m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

                                if (resIndex > -1)
                                    type = res.GetAt(resIndex).ItemType;
                            }

                            m_CraftSystem.CreateItem(m_From, m_CraftItem.ItemType, type, m_Tool, m_CraftItem);
                        }
                        break;
                    }
                case 2: //Make Number
                    m_From.Prompt = new MakeNumberCraftPrompt(m_From, m_CraftSystem, m_CraftItem, m_Tool);
                    m_From.SendLocalizedMessage(1112576); //Please type the amount you wish to create(1 - 100): <Escape to cancel>
                    break;
                case 3: //Make Max
                    AutoCraftTimer.EndTimer(m_From);
                    new AutoCraftTimer(m_From, m_CraftSystem, m_CraftItem, m_Tool, 9999, TimeSpan.FromSeconds(m_CraftSystem.Delay * m_CraftSystem.MaxCraftEffect + 1.0), TimeSpan.FromSeconds(m_CraftSystem.Delay * m_CraftSystem.MaxCraftEffect + 1.0));
                    break;
            }
        }
    }
}
