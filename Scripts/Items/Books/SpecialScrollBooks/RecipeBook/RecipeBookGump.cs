using Server.Engines.Craft;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class RecipeBookGump : Gump
    {
        private readonly RecipeBook m_Book;
        private readonly List<RecipeScrollDefinition> m_List;

        private readonly int m_Page;

        private const int LabelColor = 0xFFFFFF;

        public bool CheckFilter(RecipeScrollDefinition recipe)
        {
            RecipeScrollFilter f = m_Book.Filter;

            if (f.IsDefault)
                return true;

            if (f.Skill == 1 && recipe.Skill != RecipeSkillName.Blacksmith)
            {
                return false;
            }
            else if (f.Skill == 2 && recipe.Skill != RecipeSkillName.Tailoring)
            {
                return false;
            }
            else if (f.Skill == 3 && recipe.Skill != RecipeSkillName.Fletching)
            {
                return false;
            }
            else if (f.Skill == 4 && recipe.Skill != RecipeSkillName.Carpentry && recipe.Skill != RecipeSkillName.Masonry)
            {
                return false;
            }
            else if (f.Skill == 5 && recipe.Skill != RecipeSkillName.Inscription)
            {
                return false;
            }
            else if (f.Skill == 6 && recipe.Skill != RecipeSkillName.Cooking)
            {
                return false;
            }
            else if (f.Skill == 7 && recipe.Skill != RecipeSkillName.Alchemy)
            {
                return false;
            }
            else if (f.Skill == 8 && recipe.Skill != RecipeSkillName.Tinkering)
            {
                return false;
            }
            else if (f.Skill == 9 && recipe.Skill != RecipeSkillName.Cartography)
            {
                return false;
            }

            if (f.Expansion == 1 && recipe.Expansion != Expansion.ML)
            {
                return false;
            }
            else if (f.Expansion == 2 && recipe.Expansion != Expansion.SA)
            {
                return false;
            }
            else if (f.Expansion == 3 && recipe.Expansion != Expansion.TOL)
            {
                return false;
            }


            if (f.Amount == 1 && recipe.Amount == 0)
            {
                return false;
            }

            return true;
        }

        public int GetIndexForPage(int page)
        {
            int index = 0;

            while (page-- > 0)
                index += GetCountForIndex(index);

            return index;
        }

        public int GetCountForIndex(int index)
        {
            int slots = 0;
            int count = 0;

            List<RecipeScrollDefinition> list = m_List;

            for (int i = index; i >= 0 && i < list.Count; ++i)
            {
                RecipeScrollDefinition recipe = list[i];

                if (CheckFilter(recipe))
                {
                    int add;

                    add = 1;

                    if ((slots + add) > 10)
                        break;

                    slots += add;
                }

                ++count;
            }

            return count;
        }

        public RecipeBookGump(Mobile from, RecipeBook book)
            : this(from, book, 0, null)
        {
        }

        private class SetPricePrompt : Prompt
        {
            private readonly RecipeBook m_Book;
            private readonly RecipeScrollDefinition m_Recipe;
            private readonly int m_Page;
            private readonly List<RecipeScrollDefinition> m_List;

            public SetPricePrompt(RecipeBook book, RecipeScrollDefinition recipe, int page, List<RecipeScrollDefinition> list)
            {
                m_Book = book;
                m_Recipe = recipe;
                m_Page = page;
                m_List = list;
            }

            public override void OnResponse(Mobile from, string text)
            {
                int price = Utility.ToInt32(text);

                if (price < 0 || price > 250000000)
                {
                    from.SendLocalizedMessage(1062390);
                    m_Book.Using = false;
                }
                else
                {
                    m_Book.Recipes.Find(x => x.RecipeID == m_Recipe.RecipeID).Price = price;

                    from.SendLocalizedMessage(1158822); // Recipe price set.

                    from.SendGump(new RecipeBookGump((PlayerMobile)from, m_Book, m_Page, m_List));
                }
            }
        }

        private string GetExpansion(Expansion expansion)
        {
            switch (expansion)
            {
                default:
                case Expansion.ML:
                    return "Mondain's";
                case Expansion.SA:
                    return "Stygian";
                case Expansion.TOL:
                    return "ToL";
            }
        }

        private int GetSkillName(RecipeSkillName skill)
        {
            switch (skill)
            {
                default:
                case RecipeSkillName.Alchemy:
                    return 1002000;
                case RecipeSkillName.Blacksmith:
                    return 1150187;
                case RecipeSkillName.Carpentry:
                    return 1002054;
                case RecipeSkillName.Cartography:
                    return 1002057;
                case RecipeSkillName.Cooking:
                    return 1002063;
                case RecipeSkillName.Fletching:
                    return 1044068;
                case RecipeSkillName.Inscription:
                    return 1002090;
                case RecipeSkillName.Masonry:
                    return 1072392;
                case RecipeSkillName.Tailoring:
                    return 1150188;
                case RecipeSkillName.Tinkering:
                    return 1002162;
            }
        }

        public RecipeBookGump(Mobile from, RecipeBook book, int page, List<RecipeScrollDefinition> list)
            : base(12, 24)
        {
            from.CloseGump(typeof(RecipeBookGump));
            from.CloseGump(typeof(RecipeScrollFilterGump));

            m_Book = book;
            m_Page = page;

            if (list == null)
            {
                list = new List<RecipeScrollDefinition>();

                m_Book.Recipes.ForEach(x =>
                {
                    if (CheckFilter(x))
                    {
                        list.Add(x);
                    }
                });
            }

            m_List = list;

            int index = GetIndexForPage(page);
            int count = GetCountForIndex(index);

            int tableIndex = 0;

            PlayerVendor pv = book.RootParent as PlayerVendor;

            bool canLocked = book.IsLockedDown;
            bool canDrop = book.IsChildOf(from.Backpack);
            bool canBuy = (pv != null);
            bool canPrice = (canDrop || canBuy || canLocked);

            if (canBuy)
            {
                VendorItem vi = pv.GetVendorItem(book);

                canBuy = (vi != null && !vi.IsForSale);
            }

            int width = 600;

            if (!canPrice)
                width = 516;

            X = (624 - width) / 2;

            AddPage(0);

            AddBackground(10, 10, width, 439, 5054);
            AddImageTiled(18, 20, width - 17, 420, 2624);

            if (canPrice)
            {
                AddImageTiled(573, 64, 24, 352, 200);
                AddImageTiled(493, 64, 78, 352, 1416);
            }

            if (canDrop)
                AddImageTiled(24, 64, 32, 352, 1416);

            AddImageTiled(58, 64, 36, 352, 200);
            AddImageTiled(96, 64, 133, 352, 1416);
            AddImageTiled(231, 64, 80, 352, 200);
            AddImageTiled(313, 64, 100, 352, 1416);
            AddImageTiled(415, 64, 76, 352, 200);

            list = list.OrderBy(x => x.ID).ToList();

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                RecipeScrollDefinition recipe = list[i];

                if (!CheckFilter(recipe))
                    continue;

                AddImageTiled(24, 94 + (tableIndex * 32), canPrice ? 573 : 489, 2, 2624);

                ++tableIndex;
            }

            AddAlphaRegion(18, 20, width - 17, 420);
            AddImage(0, 0, 10460);
            AddImage(width - 15, 5, 10460);
            AddImage(0, 429, 10460);
            AddImage(width - 15, 429, 10460);

            AddHtmlLocalized(266, 32, 200, 32, 1158810, LabelColor, false, false); // Recipe Book

            AddHtmlLocalized(147, 64, 200, 32, 1062214, LabelColor, false, false); // Item
            AddHtmlLocalized(246, 64, 200, 32, 1158814, LabelColor, false, false); // Expansion
            AddHtmlLocalized(336, 64, 200, 32, 1158816, LabelColor, false, false); // Crafting
            AddHtmlLocalized(429, 64, 100, 32, 1062217, LabelColor, false, false); // Amount

            AddHtmlLocalized(70, 32, 200, 32, 1062476, LabelColor, false, false); // Set Filter
            AddButton(35, 32, 4005, 4007, 1, GumpButtonType.Reply, 0);

            RecipeScrollFilter f = book.Filter;

            if (f.IsDefault)
                AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062475, 16927, false, false); // Using No Filter
            else if (((PlayerMobile)from).UseOwnFilter)
                AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062451, 16927, false, false); // Using Your Filter
            else
                AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062230, 16927, false, false); // Using Book Filter

            AddButton(375, 416, 4017, 4018, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(410, 416, 120, 20, 1011441, LabelColor, false, false); // EXIT

            if (canDrop)
                AddHtmlLocalized(26, 64, 50, 32, 1062212, LabelColor, false, false); // Drop

            if (canPrice)
            {
                AddHtmlLocalized(516, 64, 200, 32, 1062218, LabelColor, false, false); // Price

                if (canBuy)
                    AddHtmlLocalized(576, 64, 200, 32, 1062219, LabelColor, false, false); // Buy
                else
                    AddHtmlLocalized(576, 64, 200, 32, 1062227, LabelColor, false, false); // Set
            }

            tableIndex = 0;

            if (page > 0)
            {
                AddButton(75, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(110, 416, 150, 20, 1011067, LabelColor, false, false); // Previous page
            }

            if (GetIndexForPage(page + 1) < list.Count)
            {
                AddButton(225, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(260, 416, 150, 20, 1011066, LabelColor, false, false); // Next page
            }

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                RecipeScrollDefinition recipe = list[i];

                if (!CheckFilter(recipe) || !Recipe.Recipes.ContainsKey(recipe.RecipeID))
                    continue;

                int y = 96 + (tableIndex++ * 32);

                if (recipe.Amount > 0 && (canDrop || canLocked))
                    AddButton(35, y + 2, 5602, 5606, 4 + (i * 2), GumpButtonType.Reply, 0);

                AddLabel(61, y, 0x480, string.Format("{0}", recipe.ID));
                AddHtmlLocalized(103, y, 130, 32, Recipe.Recipes[recipe.RecipeID].TextDefinition.Number, "#103221", 0xFFFFFF, false, false); // ~1_val~
                AddLabel(235, y, 0x480, GetExpansion(recipe.Expansion));
                AddHtmlLocalized(316, y, 100, 20, GetSkillName(recipe.Skill), "#104409", 0xFFFFFF, false, false); // ~1_val~
                AddLabel(421, y, 0x480, recipe.Amount.ToString());

                if (canDrop || (canBuy && recipe.Price > 0))
                {
                    AddButton(579, y + 2, 2117, 2118, 5 + (i * 2), GumpButtonType.Reply, 0);
                    AddLabel(495, y, 1152, recipe.Price.ToString("N0"));
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            int index = info.ButtonID;

            switch (index)
            {
                case 0: { m_Book.Using = false; break; }
                case 1:
                    {
                        from.SendGump(new RecipeScrollFilterGump(from, m_Book));
                        break;
                    }
                case 2:
                    {
                        if (m_Page > 0)
                            from.SendGump(new RecipeBookGump(from, m_Book, m_Page - 1, m_List));

                        return;
                    }
                case 3:
                    {
                        if (GetIndexForPage(m_Page + 1) < m_List.Count)
                            from.SendGump(new RecipeBookGump(from, m_Book, m_Page + 1, m_List));

                        break;
                    }
                default:
                    {
                        bool canDrop = m_Book.IsChildOf(from.Backpack);
                        bool canPrice = canDrop || (m_Book.RootParent is PlayerVendor);

                        index -= 4;

                        int type = index % 2;
                        index /= 2;

                        if (index < 0 || index >= m_List.Count)
                            break;

                        RecipeScrollDefinition recipe = m_List[index];

                        if (type == 0)
                        {
                            if (!m_Book.CheckAccessible(from, m_Book))
                            {
                                m_Book.SendLocalizedMessageTo(from, 1061637); // You are not allowed to access this.
                                m_Book.Using = false;
                                break;
                            }

                            if (m_Book.IsChildOf(from.Backpack) || m_Book.IsLockedDown)
                            {
                                if (recipe.Amount == 0)
                                {
                                    from.SendLocalizedMessage(1158821); // The recipe selected is not available.
                                    m_Book.Using = false;
                                    break;
                                }

                                Item item = new RecipeScroll(recipe.RecipeID);

                                if (from.AddToBackpack(item))
                                {
                                    m_Book.Recipes.ForEach(x =>
                                    {
                                        if (x.RecipeID == recipe.RecipeID)
                                            x.Amount = x.Amount - 1;
                                    });

                                    m_Book.InvalidateProperties();

                                    from.SendLocalizedMessage(1158820); // The recipe has been placed in your backpack.

                                    from.SendGump(new RecipeBookGump(from, m_Book, m_Page, null));
                                }
                                else
                                {
                                    m_Book.Using = false;
                                    item.Delete();
                                    from.SendLocalizedMessage(1158819); // There is not enough room in your backpack for the recipe.                                    
                                }
                            }
                            else
                            {
                                m_Book.Using = false;
                            }
                        }
                        else
                        {
                            if (m_Book.IsChildOf(from.Backpack))
                            {
                                from.Prompt = new SetPricePrompt(m_Book, recipe, m_Page, m_List);
                                from.SendLocalizedMessage(1062383); // Type in a price for the deed:
                                m_Book.Using = false;
                            }
                            else if (m_Book.RootParent is PlayerVendor)
                            {
                                if (recipe.Amount > 0)
                                {
                                    from.SendGump(new RecipeScrollBuyGump(from, m_Book, recipe, recipe.Price));
                                }
                                else
                                {
                                    m_Book.Using = false;
                                }
                            }
                        }

                        break;
                    }
            }
        }
    }
}
