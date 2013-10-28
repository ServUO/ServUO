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
        private readonly BaseTool m_Tool;

        private const int LabelHue = 0x480; // 0x384
        private const int RedLabelHue = 0x20;

        private const int LabelColor = 0x7FFF;
        private const int RedLabelColor = 0x6400;

        private const int GreyLabelColor = 0x3DEF;

        private int m_OtherCount;

        public CraftGumpItem(Mobile from, CraftSystem craftSystem, CraftItem craftItem, BaseTool tool)
            : base(40, 40)
        {
            this.m_From = from;
            this.m_CraftSystem = craftSystem;
            this.m_CraftItem = craftItem;
            this.m_Tool = tool;

            from.CloseGump(typeof(CraftGump));
            from.CloseGump(typeof(CraftGumpItem));

            this.AddPage(0);
            this.AddBackground(0, 0, 530, 417, 5054);
            this.AddImageTiled(10, 10, 510, 22, 2624);
            this.AddImageTiled(10, 37, 150, 148, 2624);
            this.AddImageTiled(165, 37, 355, 90, 2624);
            this.AddImageTiled(10, 190, 155, 22, 2624);
            #region SA;
            //AddImageTiled( 10, 217, 150, 53, 2624 );
            this.AddImageTiled(165, 132, 355, 80, 2624);
            this.AddImageTiled(10, 237, 155, 60, 2624);
            //AddImageTiled( 10, 302, 150, 53, 2624 );
            this.AddImageTiled(165, 217, 355, 80, 2624);
            this.AddImageTiled(10, 322, 155, 60, 2624);
            #endregion
            this.AddImageTiled(165, 302, 355, 80, 2624);
            this.AddImageTiled(10, 387, 510, 22, 2624);
            this.AddAlphaRegion(10, 10, 510, 399);

            this.AddHtmlLocalized(170, 40, 150, 20, 1044053, LabelColor, false, false); // ITEM
            #region SA;
            //AddHtmlLocalized( 10, 192, 150, 22, 1044054, LabelColor, false, false ); // <CENTER>SKILLS</CENTER>
            this.AddHtmlLocalized(10, 215, 150, 22, 1044055, LabelColor, false, false); // <CENTER>MATERIALS</CENTER>
            this.AddHtmlLocalized(10, 300, 150, 22, 1044056, LabelColor, false, false); // <CENTER>OTHER</CENTER>
            #endregion

            if (craftSystem.GumpTitleNumber > 0)
                this.AddHtmlLocalized(10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false);
            else
                this.AddHtml(10, 12, 510, 20, craftSystem.GumpTitleString, false, false);

            this.AddButton(15, 387, 4014, 4016, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(50, 390, 150, 18, 1044150, LabelColor, false, false); // BACK

            bool needsRecipe = (craftItem.Recipe != null && from is PlayerMobile && !((PlayerMobile)from).HasRecipe(craftItem.Recipe));

            if (needsRecipe)
            {
                this.AddButton(135, 387, 4005, 4007, 0, GumpButtonType.Page, 0);
                this.AddHtmlLocalized(440, 390, 150, 18, 1044151, GreyLabelColor, false, false); // MAKE NOW
            }
            else
            {
                this.AddButton(270, 387, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(440, 390, 150, 18, 1044151, LabelColor, false, false); // MAKE NOW
            }

            if (craftItem.NameNumber > 0)
                this.AddHtmlLocalized(330, 40, 180, 18, craftItem.NameNumber, LabelColor, false, false);
            else
                this.AddLabel(330, 40, LabelHue, craftItem.NameString);

            if (craftItem.UseAllRes)
                this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 310, 18, 1048176, LabelColor, false, false); // Makes as many as possible at once

            this.DrawItem();
            this.DrawSkill();
            this.DrawResource();

            /*
            if( craftItem.RequiresSE )
            AddHtmlLocalized( 170, 302 + (m_OtherCount++ * 20), 310, 18, 1063363, LabelColor, false, false ); //* Requires the "Samurai Empire" expansion
            * */

            if (craftItem.RequiredExpansion != Expansion.None)
            {
                bool supportsEx = (from.NetState != null && from.NetState.SupportsExpansion(craftItem.RequiredExpansion));
                TextDefinition.AddHtmlText(this, 170, 302 + (this.m_OtherCount++ * 20), 310, 18, this.RequiredExpansionMessage(craftItem.RequiredExpansion), false, false, supportsEx ? LabelColor : RedLabelColor, supportsEx ? LabelHue : RedLabelHue);
            }

            if (needsRecipe)
                this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 310, 18, 1073620, RedLabelColor, false, false); // You have not learned this recipe.
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
                default:
                    return String.Format("* Requires the \"{0}\" expansion", ExpansionInfo.GetInfo(expansion).Name);
            }
        }

        private bool m_ShowExceptionalChance;

        public void DrawItem()
        {
            Type type = this.m_CraftItem.ItemType;

            this.AddItem(20, 50, CraftItem.ItemIDOf(type), this.m_CraftItem.ItemHue);

            if (this.m_CraftItem.IsMarkable(type))
            {
                this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 310, 18, 1044059, LabelColor, false, false); // This item may hold its maker's mark
                this.m_ShowExceptionalChance = true;
            }
        }

        public void DrawSkill()
        {
            for (int i = 0; i < this.m_CraftItem.Skills.Count; i++)
            {
                CraftSkill skill = this.m_CraftItem.Skills.GetAt(i);
                double minSkill = skill.MinSkill, maxSkill = skill.MaxSkill;

                if (minSkill < 0)
                    minSkill = 0;

                this.AddHtmlLocalized(170, 132 + (i * 20), 200, 18, AosSkillBonuses.GetLabel(skill.SkillToMake), LabelColor, false, false);
                this.AddLabel(430, 132 + (i * 20), LabelHue, String.Format("{0:F1}", minSkill));
            }

            CraftSubResCol res = (this.m_CraftItem.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

            if (context != null)
                resIndex = (this.m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool allRequiredSkills = true;
            double chance = this.m_CraftItem.GetSuccessChance(this.m_From, resIndex > -1 ? res.GetAt(resIndex).ItemType : null, this.m_CraftSystem, false, ref allRequiredSkills);
            double excepChance = this.m_CraftItem.GetExceptionalChance(this.m_CraftSystem, chance, this.m_From);

            if (chance < 0.0)
                chance = 0.0;
            else if (chance > 1.0)
                chance = 1.0;

            this.AddHtmlLocalized(170, 80, 250, 18, 1044057, LabelColor, false, false); // Success Chance:
            this.AddLabel(430, 80, LabelHue, String.Format("{0:F1}%", chance * 100));

            if (this.m_ShowExceptionalChance)
            {
                if (excepChance < 0.0)
                    excepChance = 0.0;
                else if (excepChance > 1.0)
                    excepChance = 1.0;

                this.AddHtmlLocalized(170, 100, 250, 18, 1044058, 32767, false, false); // Exceptional Chance:
                this.AddLabel(430, 100, LabelHue, String.Format("{0:F1}%", excepChance * 100));
            }
        }

        private static readonly Type typeofBlankScroll = typeof(BlankScroll);
        private static readonly Type typeofSpellScroll = typeof(SpellScroll);

        public void DrawResource()
        {
            bool retainedColor = false;

            CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

            CraftSubResCol res = (this.m_CraftItem.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
            int resIndex = -1;

            if (context != null)
                resIndex = (this.m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

            bool cropScroll = (this.m_CraftItem.Resources.Count > 1) &&
                              this.m_CraftItem.Resources.GetAt(this.m_CraftItem.Resources.Count - 1).ItemType == typeofBlankScroll &&
                              typeofSpellScroll.IsAssignableFrom(this.m_CraftItem.ItemType);

            for (int i = 0; i < this.m_CraftItem.Resources.Count - (cropScroll ? 1 : 0) && i < 4; i++)
            {
                Type type;
                string nameString;
                int nameNumber;

                CraftRes craftResource = this.m_CraftItem.Resources.GetAt(i);

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

                if (!retainedColor && this.m_CraftItem.RetainsColorFrom(this.m_CraftSystem, type))
                {
                    retainedColor = true;
                    this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 310, 18, 1044152, LabelColor, false, false); // * The item retains the color of this material
                    this.AddLabel(500, 219 + (i * 20), LabelHue, "*");
                }

                if (nameNumber > 0)
                    this.AddHtmlLocalized(170, 219 + (i * 20), 310, 18, nameNumber, LabelColor, false, false);
                else
                    this.AddLabel(170, 219 + (i * 20), LabelHue, nameString);

                this.AddLabel(430, 219 + (i * 20), LabelHue, craftResource.Amount.ToString());
            }

            if (this.m_CraftItem.NameNumber == 1041267) // runebook
            {
                this.AddHtmlLocalized(170, 219 + (this.m_CraftItem.Resources.Count * 20), 310, 18, 1044447, LabelColor, false, false);
                this.AddLabel(430, 219 + (this.m_CraftItem.Resources.Count * 20), LabelHue, "1");
            }

            if (cropScroll)
                this.AddHtmlLocalized(170, 302 + (this.m_OtherCount++ * 20), 360, 18, 1044379, LabelColor, false, false); // Inscribing scrolls also requires a blank scroll and mana.
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Back Button
            if (info.ButtonID == 0)
            {
                CraftGump craftGump = new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, null);
                this.m_From.SendGump(craftGump);
            }
            else // Make Button
            {
                int num = this.m_CraftSystem.CanCraft(this.m_From, this.m_Tool, this.m_CraftItem.ItemType);

                if (num > 0)
                {
                    this.m_From.SendGump(new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, num));
                }
                else
                {
                    Type type = null;

                    CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

                    if (context != null)
                    {
                        CraftSubResCol res = (this.m_CraftItem.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
                        int resIndex = (this.m_CraftItem.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

                        if (resIndex > -1)
                            type = res.GetAt(resIndex).ItemType;
                    }

                    this.m_CraftSystem.CreateItem(this.m_From, this.m_CraftItem.ItemType, type, this.m_Tool, this.m_CraftItem);
                }
            }
        }
    }
}