using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using Server.Misc;
using System.Text;

namespace Server.ACC.YS
{
    public class YardGump : Gump
    {
        int m_SelectedID;
        int m_ItemPrice = 0;
        int m_PlayerGold = 0;
        YardShovel m_Shovel;
        //New registry
        string[] m_Categories;
        YardGumpCategory m_CurrentCategory;
        int m_CurrentPage;

        public YardGump(Mobile owner, YardShovel shovel, string currentCategory, int currentPage, int itemID, int price)
            : base(shovel.XStart, shovel.YStart)
        {
            string Title = "Yard & Garden System - " + currentCategory;
            m_SelectedID = itemID;
            m_ItemPrice = price;
            m_Shovel = shovel;
            m_CurrentPage = currentPage;
            if (currentCategory != null && YardRegistry.Categories.ContainsKey(currentCategory))
            {
                m_CurrentCategory = YardRegistry.Categories[currentCategory];
            }
            m_Shovel.Category = currentCategory;
            m_Shovel.Page = currentPage;

            ComputeGold(owner);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            //Page 0
            AddPage(0);

            AddBackground(59, 55, 300, 300, 3600);						//MainGround
            AddBackground(34, 0, 350, 50, 3600);						//TitleGround
            AddBackground(385, 209, 150, 200, 3600);					//PicGround

            AddBackground(59, 358, 300, 50, 3600);						//PriceGround
            AddBackground(372, 93, 165, 50, 3600);						//PlaceGround
            AddBackground(372, 143, 165, 50, 3600);					//GoldGround

            AddButton(472, 102, 2642, 2643, (int)Buttons.Place, GumpButtonType.Reply, 0);
            AddLabel(398, 109, 197, "PLACE");
            AddItem(455, 98, 6022);										//LPGrass
            AddItem(489, 98, 6024);										//RPGrass

            AddLabel(116, 375, 37, @"Price : ");
            AddLabel(166, 375, 37, String.Format("{0:0,0} Gold", m_ItemPrice));
            AddLabel(387, 160, 48, String.Format("Gold : {0:0,0}", m_PlayerGold));

            AddItem(337, 110, 6019);										//TGrass
            AddItem(337, 155, 6019);										//BGrass
            AddItem(510, 183, 6024);										//FGrass
            AddItem(328, 190, 3317);										//TLog1
            AddItem(348, 195, 3318);										//TLog2
            AddItem(371, 221, 3319);										//TLog3
            AddItem(339, 354, 3316);										//BLog1
            AddItem(362, 338, 3315);										//BLog2

            AddItem(0, 8, 3497);											//LTree
            AddItem(330, 8, 3497);											//RTree
            AddItem(334, 266, 3312);										//RBVine
            AddItem(334, 192, 3312);										//RMVine
            AddItem(334, 118, 3312);										//RTVine
            AddItem(39, 266, 3308);										//LBVine
            AddItem(39, 192, 3308);										//LMVine
            AddItem(39, 118, 3308);										//LTVine

            AddItem(35, 325, 3310);										//LPVine
            AddItem(307, 325, 3314);										//RPVine
            AddButton(490, 365, 22124, 22125, (int)Buttons.Settings, GumpButtonType.Reply, 0);

            AddBackground(538, 0, 165, 409, 3600);
            AddLabel(586, 16, 68, "Categories");

            int categoryID = 0;
            m_Categories = new string[YardRegistry.Categories.Keys.Count];
            foreach (string categoryName in YardRegistry.Categories.Keys)
            {
                if (categoryName == currentCategory)
                {
                    AddButton(557, 45 + (25 * categoryID), 2361, 2360, 8851 + categoryID, GumpButtonType.Reply, 0);
                }
                else
                {
                    AddButton(557, 45 + (25 * categoryID), 2360, 2361, 8851 + categoryID, GumpButtonType.Reply, 0);
                }
                AddLabel(570, 42 + (25 * categoryID), 69, categoryName);
                m_Categories[categoryID] = categoryName;
                categoryID++;
            }

            if (m_CurrentCategory != null)
            {
                int i = 0;
                foreach (YardGumpEntry entry in m_CurrentCategory.Pages[m_CurrentPage].Values)
                {
                    entry.AppendToGump(this, 107 + (i >= 12 ? 143 : 0), 95 + (i >= 12 ? 20 * (i - 12) : 20 * i));
                    i++;
                }
            }
            else
            {
                string welcome = String.Format("{0}  {1} {2} spaces in front, {3} spaces to the left, {4} spaces to the right and {5} spaces behind the house.  {6}",
                    "<basefont color=#99AA22>Welcome to the Yard & Garden System!",
                    "Here you can purchase items for your yard.  These items can be placed while standing in your house,",
                    YardSettings.Front, YardSettings.Left, YardSettings.Right, YardSettings.Back,
                    "(Can also be placed inside)  Select the catagory to the right and design away!</basefont>");
                AddHtml(86, 96, 246, 258, welcome, false, false);
            }

            if (m_CurrentCategory != null && m_CurrentCategory.Pages.Count > m_CurrentPage + 1)
            {
                AddButton(295, 74, 9903, 9904, (int)Buttons.Next, GumpButtonType.Reply, 0);
            }

            if (m_CurrentCategory != null && m_CurrentPage > 0)
            {
                AddButton(109, 74, 9909, 9910, (int)Buttons.Prev, GumpButtonType.Reply, 0);
            }

            if (m_SelectedID != 0)
            {
                AddItem(410, 235, m_SelectedID);
            }

            AddLabel(80, 16, 68, Title);
        }

        public enum Buttons
        {
            Exit,
            Settings = -1,
            Place = -2,
            Next = -3,
            Prev = -4,
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            if (info.ButtonID == 0)
            {
                return;
            }
            else if (info.ButtonID == (int)Buttons.Settings)
            {
                from.SendGump(new YGSettingsGump(m_Shovel, from));
            }
            else if (info.ButtonID == (int)Buttons.Next)
            {
                if (m_CurrentCategory != null && YardRegistry.Categories[m_CurrentCategory.Name].Pages.Count > m_CurrentPage + 1)
                {
                    from.SendGump(new YardGump(from, m_Shovel, m_CurrentCategory.Name, m_CurrentPage + 1, m_SelectedID, m_ItemPrice));
                }
                else
                {
                    from.SendGump(new YardGump(from, m_Shovel, "", 0, m_SelectedID, m_ItemPrice));
                }
            }
            else if (info.ButtonID == (int)Buttons.Prev)
            {
                if (m_CurrentCategory != null && m_CurrentPage > 0)
                {
                    from.SendGump(new YardGump(from, m_Shovel, m_CurrentCategory.Name, m_CurrentPage - 1, m_SelectedID, m_ItemPrice));
                }
                else
                {
                    from.SendGump(new YardGump(from, m_Shovel, "", 0, m_SelectedID, m_ItemPrice));
                }
            }
            else if (info.ButtonID == (int)Buttons.Place)
            {
                if (m_SelectedID > 0)
                {
                    from.SendMessage("Please choose where to place the item");
                    from.Target = new YardTarget(m_Shovel, from, m_SelectedID, m_ItemPrice, m_CurrentCategory.Name, m_CurrentPage);
                }
            }
            else if (info.ButtonID >= 8851 && info.ButtonID <= 8859)
            {
                //Change categories
                if (m_Categories != null && m_Categories.Length > info.ButtonID - 8851)
                {
                    if (m_CurrentCategory != null)
                    {
                        from.SendGump(new YardGump(from, m_Shovel,
                                                   m_Categories[info.ButtonID - 8851] == m_CurrentCategory.Name ? "" : m_Categories[info.ButtonID - 8851], 
                                                   0, m_SelectedID, m_ItemPrice));                        
                    }
                    else
                    {
                        from.SendGump(new YardGump(from, m_Shovel, m_Categories[info.ButtonID - 8851], 0, m_SelectedID, m_ItemPrice));
                    }
                }
                else
                {
                    from.SendGump(new YardGump(from, m_Shovel, "", 0, m_SelectedID, m_ItemPrice));
                }
            }
            else
            {
                m_SelectedID = info.ButtonID;
                if (m_CurrentCategory != null)
                {
                    YardGumpEntry entry = m_CurrentCategory.GetEntry(m_SelectedID);
                    if (entry != null)
                    {
                        m_ItemPrice = entry.Price;
                    }

                    from.SendGump(new YardGump(from, m_Shovel, m_CurrentCategory.Name, m_CurrentPage, m_SelectedID, m_ItemPrice));
                }
            }
        }

        public void ComputeGold(Mobile from)
        {
            int goldInPack = 0;
            int goldInBank = 0;
            foreach (Gold gold in from.Backpack.FindItemsByType<Gold>(true))
            {
                goldInPack += gold.Amount;
            }

            foreach (Gold gold in from.BankBox.FindItemsByType<Gold>(true))
            {
                goldInBank += gold.Amount;
            }

            m_PlayerGold = goldInPack + goldInBank;
        }
    }
}
