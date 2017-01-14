using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Engines.Craft
{
    public class CraftGump : Gump
    {
        private readonly Mobile m_From;
        private readonly CraftSystem m_CraftSystem;
        private readonly BaseTool m_Tool;

        private readonly CraftPage m_Page;

        private const int LabelHue = 0x480;
        private const int LabelColor = 0x7FFF;
        private const int FontColor = 0xFFFFFF;

        public bool Locked { get { return AutoCraftTimer.HasTimer(m_From); } }

        private enum CraftPage
        {
            None,
            PickResource,
            PickResource2
        }

        /*public CraftGump( Mobile from, CraftSystem craftSystem, BaseTool tool ): this( from, craftSystem, -1, -1, tool, null )
        {
        }*/

        public CraftGump(Mobile from, CraftSystem craftSystem, BaseTool tool, object notice)
            : this(from, craftSystem, tool, notice, CraftPage.None)
        {
        }

        private CraftGump(Mobile from, CraftSystem craftSystem, BaseTool tool, object notice, CraftPage page)
            : base(40, 40)
        {
            this.m_From = from;
            this.m_CraftSystem = craftSystem;
            this.m_Tool = tool;
            this.m_Page = page;

            CraftContext context = craftSystem.GetContext(from);

            from.CloseGump(typeof(CraftGump));
            from.CloseGump(typeof(CraftGumpItem));

            this.AddPage(0);

            AddBackground(0, 0, 530, 497, 5054);
            AddImageTiled(10, 10, 510, 22, 2624);
            AddImageTiled(10, 292, 150, 45, 2624);
            AddImageTiled(165, 292, 355, 45, 2624);
            AddImageTiled(10, 342, 510, 145, 2624);
            AddImageTiled(10, 37, 200, 250, 2624);
            AddImageTiled(215, 37, 305, 250, 2624);
            AddAlphaRegion(10, 10, 510, 477);

            if (craftSystem.GumpTitleNumber > 0)
                this.AddHtmlLocalized(10, 12, 510, 20, craftSystem.GumpTitleNumber, LabelColor, false, false);
            else
                this.AddHtml(10, 12, 510, 20, craftSystem.GumpTitleString, false, false);

            AddHtmlLocalized(10, 37, 200, 22, 1044010, LabelColor, false, false); // <CENTER>CATEGORIES</CENTER>
            AddHtmlLocalized(215, 37, 305, 22, 1044011, LabelColor, false, false); // <CENTER>SELECTIONS</CENTER>
            AddHtmlLocalized(10, 302, 150, 25, 1044012, LabelColor, false, false); // <CENTER>NOTICES</CENTER>

            this.AddButton(15, 442, 4017, 4019, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(50, 445, 150, 18, 1011441, LabelColor, false, false); // EXIT

            this.AddButton(115, 442, 4017, 4019, GetButtonID(6, 11), GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(150, 445, 150, 18, 1112698, LabelColor, false, false); // CANCEL MAKE

            // Repair option
            if (m_CraftSystem.Repair)
            {
                AddButton(270, 342, 4005, 4007, GetButtonID(6, 5), GumpButtonType.Reply, 0);
                AddHtmlLocalized(305, 345, 150, 18, 1044260, LabelColor, false, false); // REPAIR ITEM
            }
            // ****************************************

            // Mark option
            if (m_CraftSystem.MarkOption)
            {
                AddButton(270, 362, 4005, 4007, GetButtonID(6, 6), GumpButtonType.Reply, 0);
                AddHtmlLocalized(305, 365, 150, 18, 1044017 + (context == null ? 0 : (int)context.MarkOption), LabelColor, false, false); // MARK ITEM
            }
            // ****************************************

            // Enhance option
            if (m_CraftSystem.CanEnhance)
            {
                AddButton(270, 382, 4005, 4007, GetButtonID(6, 8), GumpButtonType.Reply, 0);
                AddHtmlLocalized(305, 385, 150, 18, 1061001, LabelColor, false, false); // ENHANCE ITEM
            }
            // ****************************************

            #region SA
            // Alter option
            if (Core.SA && m_CraftSystem.CanAlter)
            {
                AddButton(270, 402, 4005, 4007, GetButtonID(6, 9), GumpButtonType.Reply, 0);
                AddHtmlLocalized(304, 405, 250, 18, 1094726, LabelColor, false, false); // ALTER ITEM (Gargoyle)
            }
            // ****************************************

            // Quest item
            if (Core.SA)
            {
                AddButton(270, 422, 4005, 4007, GetButtonID(6, 10), GumpButtonType.Reply, 0);
                AddHtmlLocalized(305, 425, 150, 18, context != null && context.QuestOption == CraftQuestOption.QuestItem ? 1112534 : 1112533, LabelColor, false, false); // QUEST ITEM
            }
            // ****************************************
            #endregion

            AddButton(270, 442, 4005, 4007, GetButtonID(6, 2), GumpButtonType.Reply, 0);
            AddHtmlLocalized(305, 445, 150, 18, 1044013, LabelColor, false, false); // MAKE LAST

            #region Stygian Abyss
            int total = 1;
            int made = 0;

            if (Locked && AutoCraftTimer.AutoCraftTable.ContainsKey(m_From))
            {
                AutoCraftTimer timer = AutoCraftTimer.AutoCraftTable[m_From];

                if (timer != null)
                {
                    total = timer.Amount;
                    made = timer.Attempts;
                }
                else
                {
                    if (context != null)
                        total = context.MakeTotal;
                }
            }

            string args = String.Format("{0}\t{1}", made.ToString(), total.ToString());

            AddHtmlLocalized(270, 468, 150, 18, 1079443, args, LabelColor, false, false); //~1_DONE~/~2_TOTAL~ COMPLETED
            #endregion

            // Resmelt option
            if (m_CraftSystem.Resmelt)
            {
                AddButton(15, 342, 4005, 4007, GetButtonID(6, 1), GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 345, 150, 18, 1044259, LabelColor, false, false); // SMELT ITEM
            }
            // ****************************************

            if (notice is int && (int)notice > 0)
                this.AddHtmlLocalized(170, 295, 350, 40, (int)notice, LabelColor, false, false);
            else if (notice is string)
                this.AddHtml(170, 295, 350, 40, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", FontColor, notice), false, false);

            // If the system has more than one resource
            if (craftSystem.CraftSubRes.Init)
            {
                string nameString = craftSystem.CraftSubRes.NameString;
                int nameNumber = craftSystem.CraftSubRes.NameNumber;

                int resIndex = (context == null ? -1 : context.LastResourceIndex);

                Type resourceType = craftSystem.CraftSubRes.ResType;
                Type resourceType2 = GetAltType(resourceType);

                if (resIndex > -1)
                {
                    CraftSubRes subResource = craftSystem.CraftSubRes.GetAt(resIndex);

                    nameString = subResource.NameString;
                    nameNumber = subResource.NameNumber;
                    resourceType = subResource.ItemType;
                }

                int resourceCount = 0;

                if (from.Backpack != null)
                {
                    Item[] items = from.Backpack.FindItemsByType(resourceType, true);

                    for (int i = 0; i < items.Length; ++i)
                        resourceCount += items[i].Amount;

                    if (resourceType2 != null)
                    {
                        Item[] items2 = m_From.Backpack.FindItemsByType(resourceType2, true);

                        for (int i = 0; i < items2.Length; ++i)
                            resourceCount += items2[i].Amount;
                    }
                }

                this.AddButton(15, 382, 4005, 4007, GetButtonID(6, 0), GumpButtonType.Reply, 0);

                if (nameNumber > 0)
                {
                    if (context.DoNotColor)
                        AddLabel(50, 385, LabelHue, "*");

                    AddHtmlLocalized(50 + (context.DoNotColor ? 13 : 0), 385, 250, 18, nameNumber, resourceCount.ToString(), LabelColor, false, false);
                }
                else
                    AddLabel(50, 382, LabelHue, (context.DoNotColor ? "*" : "") + String.Format("{0} ({1} Available)", nameString, resourceCount));
            }
            // ****************************************

            // For dragon scales
            if (craftSystem.CraftSubRes2.Init)
            {
                string nameString = craftSystem.CraftSubRes2.NameString;
                int nameNumber = craftSystem.CraftSubRes2.NameNumber;

                int resIndex = (context == null ? -1 : context.LastResourceIndex2);

                Type resourceType = craftSystem.CraftSubRes2.ResType;

                if (resIndex > -1)
                {
                    CraftSubRes subResource = craftSystem.CraftSubRes2.GetAt(resIndex);

                    nameString = subResource.NameString;
                    nameNumber = subResource.NameNumber;
                    resourceType = subResource.ItemType;
                }

                int resourceCount = 0;

                if (from.Backpack != null)
                {
                    Item[] items = from.Backpack.FindItemsByType(resourceType, true);

                    for (int i = 0; i < items.Length; ++i)
                        resourceCount += items[i].Amount;
                }

                this.AddButton(15, 402, 4005, 4007, GetButtonID(6, 7), GumpButtonType.Reply, 0);

                if (nameNumber > 0)
                    AddHtmlLocalized(50, 405, 250, 18, nameNumber, resourceCount.ToString(), LabelColor, false, false);
                else
                    AddLabel(50, 405, LabelHue, String.Format("{0} ({1} Available)", nameString, resourceCount));
            }
            // ****************************************

            this.CreateGroupList();

            if (page == CraftPage.PickResource)
                this.CreateResList(false, from);
            else if (page == CraftPage.PickResource2)
                this.CreateResList(true, from);
            else if (context != null && context.LastGroupIndex > -1)
                this.CreateItemList(context.LastGroupIndex);
        }

        private Type GetAltType(Type original)
        {
            for (int i = 0; i < m_TypesTable.Length; i++)
            {
                if (original == m_TypesTable[i][0] && m_TypesTable[i].Length > 1)
                    return m_TypesTable[i][1];
            }

            return null;
        }

        private Type[][] m_TypesTable = new Type[][]
        {
            new Type[]{ typeof( Log ), typeof( Board ) },
			new Type[]{ typeof( HeartwoodLog ), typeof( HeartwoodBoard ) },
			new Type[]{ typeof( BloodwoodLog ), typeof( BloodwoodBoard ) },
			new Type[]{ typeof( FrostwoodLog ), typeof( FrostwoodBoard ) },
			new Type[]{ typeof( OakLog ), typeof( OakBoard ) },
			new Type[]{ typeof( AshLog ), typeof( AshBoard ) },
			new Type[]{ typeof( YewLog ), typeof( YewBoard ) },
			new Type[]{ typeof( Leather ), typeof( Hides ) },
			new Type[]{ typeof( SpinedLeather ), typeof( SpinedHides ) },
			new Type[]{ typeof( HornedLeather ), typeof( HornedHides ) },
			new Type[]{ typeof( BarbedLeather ), typeof( BarbedHides ) },
        };

        public void CreateResList(bool opt, Mobile from)
        {
            CraftSubResCol res = (opt ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);

            for (int i = 0; i < res.Count; ++i)
            {
                int index = i % 10;

                CraftSubRes subResource = res.GetAt(i);

                if (index == 0)
                {
                    if (i > 0)
                        this.AddButton(485, 290, 4005, 4007, 0, GumpButtonType.Page, (i / 10) + 1);

                    this.AddPage((i / 10) + 1);

                    if (i > 0)
                        this.AddButton(455, 290, 4014, 4015, 0, GumpButtonType.Page, i / 10);

                    CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

                    this.AddButton(220, 260, 4005, 4007, GetButtonID(6, 4), GumpButtonType.Reply, 0);
                    this.AddHtmlLocalized(263, 253, 200, 18, (context == null || !context.DoNotColor) ? 1061591 : 1061590, LabelColor, false, false);
                }

                int resourceCount = 0;

                if (from.Backpack != null)
                {
                    Item[] items = from.Backpack.FindItemsByType(subResource.ItemType, true);

                    for (int j = 0; j < items.Length; ++j)
                        resourceCount += items[j].Amount;
                }

                this.AddButton(220, 70 + (index * 20), 4005, 4007, GetButtonID(5, i), GumpButtonType.Reply, 0);

                if (subResource.NameNumber > 0)
                    this.AddHtmlLocalized(255, 73 + (index * 20), 250, 18, subResource.NameNumber, resourceCount.ToString(), LabelColor, false, false);
                else
                    this.AddLabel(255, 70 + (index * 20), LabelHue, String.Format("{0} ({1})", subResource.NameString, resourceCount));
            }
        }

        public void CreateMakeLastList()
        {
            CraftContext context = this.m_CraftSystem.GetContext(this.m_From);

            if (context == null)
                return;

            List<CraftItem> items = context.Items;

            if (items.Count > 0)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    int index = i % 10;

                    CraftItem craftItem = items[i];

                    if (index == 0)
                    {
                        if (i > 0)
                        {
                            this.AddButton(370, 260, 4005, 4007, 0, GumpButtonType.Page, (i / 10) + 1);
                            this.AddHtmlLocalized(405, 263, 100, 18, 1044045, LabelColor, false, false); // NEXT PAGE
                        }

                        this.AddPage((i / 10) + 1);

                        if (i > 0)
                        {
                            this.AddButton(220, 260, 4014, 4015, 0, GumpButtonType.Page, i / 10);
                            this.AddHtmlLocalized(255, 263, 100, 18, 1044044, LabelColor, false, false); // PREV PAGE
                        }
                    }

                    this.AddButton(220, 60 + (index * 20), 4005, 4007, GetButtonID(3, i), GumpButtonType.Reply, 0);

                    if (craftItem.NameNumber > 0)
                        this.AddHtmlLocalized(255, 63 + (index * 20), 220, 18, craftItem.NameNumber, LabelColor, false, false);
                    else
                        this.AddLabel(255, 60 + (index * 20), LabelHue, craftItem.NameString);

                    this.AddButton(480, 60 + (index * 20), 4011, 4012, GetButtonID(4, i), GumpButtonType.Reply, 0);
                }
            }
            else
            {
                // NOTE: This is not as OSI; it is an intentional difference
                this.AddHtmlLocalized(230, 62, 200, 22, 1044165, LabelColor, false, false); // You haven't made anything yet.
            }
        }

        public void CreateItemList(int selectedGroup)
        {
            if (selectedGroup == 501) // 501 : Last 10
            {
                this.CreateMakeLastList();
                return;
            }

            CraftGroupCol craftGroupCol = this.m_CraftSystem.CraftGroups;
            CraftGroup craftGroup = craftGroupCol.GetAt(selectedGroup);
            CraftItemCol craftItemCol = craftGroup.CraftItems;

            for (int i = 0; i < craftItemCol.Count; ++i)
            {
                int index = i % 10;

                CraftItem craftItem = craftItemCol.GetAt(i);

                if (index == 0)
                {
                    if (i > 0)
                    {
                        this.AddButton(370, 260, 4005, 4007, 0, GumpButtonType.Page, (i / 10) + 1);
                        this.AddHtmlLocalized(405, 263, 100, 18, 1044045, LabelColor, false, false); // NEXT PAGE
                    }

                    this.AddPage((i / 10) + 1);

                    if (i > 0)
                    {
                        this.AddButton(220, 260, 4014, 4015, 0, GumpButtonType.Page, i / 10);
                        this.AddHtmlLocalized(255, 263, 100, 18, 1044044, LabelColor, false, false); // PREV PAGE
                    }
                }

                this.AddButton(220, 60 + (index * 20), 4005, 4007, GetButtonID(1, i), GumpButtonType.Reply, 0);

                if (craftItem.NameNumber > 0)
                    this.AddHtmlLocalized(255, 63 + (index * 20), 220, 18, craftItem.NameNumber, LabelColor, false, false);
                else
                    this.AddLabel(255, 60 + (index * 20), LabelHue, craftItem.NameString);

                this.AddButton(480, 60 + (index * 20), 4011, 4012, GetButtonID(2, i), GumpButtonType.Reply, 0);
            }
        }

        public int CreateGroupList()
        {
            CraftGroupCol craftGroupCol = this.m_CraftSystem.CraftGroups;

            this.AddButton(15, 60, 4005, 4007, GetButtonID(6, 3), GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(50, 63, 150, 18, 1044014, LabelColor, false, false); // LAST TEN

            for (int i = 0; i < craftGroupCol.Count; i++)
            {
                CraftGroup craftGroup = craftGroupCol.GetAt(i);

                this.AddButton(15, 80 + (i * 20), 4005, 4007, GetButtonID(0, i), GumpButtonType.Reply, 0);

                if (craftGroup.NameNumber > 0)
                    this.AddHtmlLocalized(50, 83 + (i * 20), 150, 18, craftGroup.NameNumber, LabelColor, false, false);
                else
                    this.AddLabel(50, 80 + (i * 20), LabelHue, craftGroup.NameString);
            }

            return craftGroupCol.Count;
        }

        public static int GetButtonID(int type, int index)
        {
            return 1 + type + (index * 7);
        }

        public void CraftItem(CraftItem item)
        {
            int num = this.m_CraftSystem.CanCraft(this.m_From, this.m_Tool, item.ItemType);

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
                    CraftSubResCol res = (item.UseSubRes2 ? this.m_CraftSystem.CraftSubRes2 : this.m_CraftSystem.CraftSubRes);
                    int resIndex = (item.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex);

                    if (resIndex >= 0 && resIndex < res.Count)
                        type = res.GetAt(resIndex).ItemType;
                }

                this.m_CraftSystem.CreateItem(this.m_From, item.ItemType, type, this.m_Tool, item);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID <= 0)
                return; // Canceled

            int buttonID = info.ButtonID - 1;
            int type = buttonID % 7;
            int index = buttonID / 7;

            CraftSystem system = this.m_CraftSystem;
            CraftGroupCol groups = system.CraftGroups;
            CraftContext context = system.GetContext(this.m_From);

            #region Stygian Abyss
			if (Locked)
			{
				if (type == 6 && index == 11)
				{
					// Cancel Make
					AutoCraftTimer.EndTimer(m_From);
				}
				return;
			}
            #endregion

            switch ( type )
            {
                case 0: // Show group
                    {
                        if (context == null)
                            break;

                        if (index >= 0 && index < groups.Count)
                        {
                            context.LastGroupIndex = index;
                            this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, null));
                        }

                        break;
                    }
                case 1: // Create item
                    {
                        if (context == null)
                            break;

                        int groupIndex = context.LastGroupIndex;

                        if (groupIndex >= 0 && groupIndex < groups.Count)
                        {
                            CraftGroup group = groups.GetAt(groupIndex);

                            if (index >= 0 && index < group.CraftItems.Count)
                                this.CraftItem(group.CraftItems.GetAt(index));
                        }

                        break;
                    }
                case 2: // Item details
                    {
                        if (context == null)
                            break;

                        int groupIndex = context.LastGroupIndex;

                        if (groupIndex >= 0 && groupIndex < groups.Count)
                        {
                            CraftGroup group = groups.GetAt(groupIndex);

                            if (index >= 0 && index < group.CraftItems.Count)
                                this.m_From.SendGump(new CraftGumpItem(this.m_From, system, group.CraftItems.GetAt(index), this.m_Tool));
                        }

                        break;
                    }
                case 3: // Create item (last 10)
                    {
                        if (context == null)
                            break;

                        List<CraftItem> lastTen = context.Items;

                        if (index >= 0 && index < lastTen.Count)
                            this.CraftItem(lastTen[index]);

                        break;
                    }
                case 4: // Item details (last 10)
                    {
                        if (context == null)
                            break;

                        List<CraftItem> lastTen = context.Items;

                        if (index >= 0 && index < lastTen.Count)
                            this.m_From.SendGump(new CraftGumpItem(this.m_From, system, lastTen[index], this.m_Tool));

                        break;
                    }
                case 5: // Resource selected
                    {
                        if (this.m_Page == CraftPage.PickResource && index >= 0 && index < system.CraftSubRes.Count)
                        {
                            int groupIndex = (context == null ? -1 : context.LastGroupIndex);

                            CraftSubRes res = system.CraftSubRes.GetAt(index);

                            if (this.m_From.Skills[system.MainSkill].Base < res.RequiredSkill)
                            {
                                this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, res.Message));
                            }
                            else
                            {
                                if (context != null)
                                    context.LastResourceIndex = index;

                                this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, null));
                            }
                        }
                        else if (this.m_Page == CraftPage.PickResource2 && index >= 0 && index < system.CraftSubRes2.Count)
                        {
                            int groupIndex = (context == null ? -1 : context.LastGroupIndex);

                            CraftSubRes res = system.CraftSubRes2.GetAt(index);

                            if (this.m_From.Skills[system.MainSkill].Base < res.RequiredSkill)
                            {
                                this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, res.Message));
                            }
                            else
                            {
                                if (context != null)
                                    context.LastResourceIndex2 = index;

                                this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, null));
                            }
                        }

                        break;
                    }
                case 6: // Misc. buttons
                    {
                        switch ( index )
                        {
                            case 0: // Resource selection
                                {
                                    if (system.CraftSubRes.Init)
                                        this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, null, CraftPage.PickResource));

                                    break;
                                }
                            case 1: // Smelt item
                                {
                                    if (system.Resmelt)
                                        Resmelt.Do(this.m_From, system, this.m_Tool);

                                    break;
                                }
                            case 2: // Make last
                                {
                                    if (context == null)
                                        break;

                                    CraftItem item = context.LastMade;

                                    if (item != null)
                                        this.CraftItem(item);
                                    else
                                        this.m_From.SendGump(new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, 1044165, this.m_Page)); // You haven't made anything yet.

                                    break;
                                }
                            case 3: // Last 10
                                {
                                    if (context == null)
                                        break;

                                    context.LastGroupIndex = 501;
                                    this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, null));

                                    break;
                                }
                            case 4: // Toggle use resource hue
                                {
                                    if (context == null)
                                        break;

                                    context.DoNotColor = !context.DoNotColor;

                                    this.m_From.SendGump(new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, null, this.m_Page));

                                    break;
                                }
                            case 5: // Repair item
                                {
                                    if (system.Repair)
                                        Repair.Do(this.m_From, system, this.m_Tool);

                                    break;
                                }
                            case 6: // Toggle mark option
                                {
                                    if (context == null || !system.MarkOption)
                                        break;

                                    switch ( context.MarkOption )
                                    {
                                        case CraftMarkOption.MarkItem:
                                            context.MarkOption = CraftMarkOption.DoNotMark;
                                            break;
                                        case CraftMarkOption.DoNotMark:
                                            context.MarkOption = CraftMarkOption.PromptForMark;
                                            break;
                                        case CraftMarkOption.PromptForMark:
                                            context.MarkOption = CraftMarkOption.MarkItem;
                                            break;
                                    }

                                    this.m_From.SendGump(new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, null, this.m_Page));

                                    break;
                                }
                            case 7: // Resource selection 2
                                {
                                    if (system.CraftSubRes2.Init)
                                        this.m_From.SendGump(new CraftGump(this.m_From, system, this.m_Tool, null, CraftPage.PickResource2));

                                    break;
                                }
                            case 8: // Enhance item
                                {
                                    if (system.CanEnhance)
                                        Enhance.BeginTarget(this.m_From, system, this.m_Tool);

                                    break;
                                }
                            #region Stygian Abyss
                            case 9: // Alter Item (Gargoyle)
                                {
                                    if (system.CanAlter)
                                    {
                                        if (Server.SkillHandlers.Imbuing.CheckSoulForge(m_From, 1, false))
                                        {
                                            AlterItem.BeginTarget(this.m_From, system, this.m_Tool);
                                        }
                                        else
                                            m_From.SendLocalizedMessage(1111867); // You must be near a soulforge to alter an item.
                                    }
									break;
                                }
                            case 10: // Quest Item/Non Quest Item toggle
                                {
                                    //if (context == null || !system.QuestOption)
                                    //break;
                                    switch ( context.QuestOption )
                                    {
                                        case CraftQuestOption.QuestItem:
                                            context.QuestOption = CraftQuestOption.NonQuestItem;
                                            break;
                                        case CraftQuestOption.NonQuestItem:
                                            context.QuestOption = CraftQuestOption.QuestItem;
                                            break;
                                    }

                                    this.m_From.SendGump(new CraftGump(this.m_From, this.m_CraftSystem, this.m_Tool, null, this.m_Page));

                                    break;
                                }
                            case 11: // Cancel Make
                                {
                                    AutoCraftTimer.EndTimer(m_From);
                                    break;
                                }
                            #endregion
                        }
                        break;
                    }
            }
        }
    }
}