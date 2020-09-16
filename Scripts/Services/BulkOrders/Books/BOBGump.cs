using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;
using System;
using System.Collections;

namespace Server.Engines.BulkOrders
{
    public class BOBGump : Gump
    {
        private const int LabelColor = 0x7FFF;
        private readonly PlayerMobile m_From;
        private readonly BulkOrderBook m_Book;
        private readonly ArrayList m_List;
        private int m_Page;
        public BOBGump(PlayerMobile from, BulkOrderBook book)
            : this(from, book, 0, null)
        {
        }

        public BOBGump(PlayerMobile from, BulkOrderBook book, int page, ArrayList list)
            : base(12, 24)
        {
            from.CloseGump(typeof(BOBGump));
            from.CloseGump(typeof(BOBFilterGump));

            m_From = from;
            m_Book = book;
            m_Page = page;

            if (list == null)
            {
                list = new ArrayList(book.Entries.Count);

                for (int i = 0; i < book.Entries.Count; ++i)
                {
                    object obj = book.Entries[i];

                    if (CheckFilter(obj))
                        list.Add(obj);
                }
            }

            m_List = list;

            int index = GetIndexForPage(page);
            int count = GetCountForIndex(index);

            int tableIndex = 0;

            PlayerVendor pv = book.RootParent as PlayerVendor;

            bool canDrop = book.IsChildOf(from.Backpack);
            bool canBuy = (pv != null);
            bool canPrice = (canDrop || canBuy);

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

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                if (!CheckFilter(obj))
                    continue;

                AddImageTiled(24, 94 + (tableIndex * 32), canPrice ? 573 : 489, 2, 2624);

                if (obj is BOBLargeEntry)
                    tableIndex += ((BOBLargeEntry)obj).Entries.Length;
                else if (obj is BOBSmallEntry)
                    ++tableIndex;
            }

            AddAlphaRegion(18, 20, width - 17, 420);
            AddImage(5, 5, 10460);
            AddImage(width - 15, 5, 10460);
            AddImage(5, 424, 10460);
            AddImage(width - 15, 424, 10460);

            AddHtmlLocalized(canPrice ? 266 : 224, 32, 200, 32, 1062220, LabelColor, false, false); // Bulk Order Book
            AddHtmlLocalized(63, 64, 70, 32, 1062213, LabelColor, false, false); // Type
            AddHtmlLocalized(147, 64, 70, 32, 1062214, LabelColor, false, false); // Item
            AddHtmlLocalized(246, 64, 70, 32, 1062215, LabelColor, false, false); // Quality
            AddHtmlLocalized(336, 64, 70, 32, 1062216, LabelColor, false, false); // Material
            AddHtmlLocalized(429, 64, 70, 32, 1062217, LabelColor, false, false); // Amount

            AddButton(35, 32, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(70, 32, 200, 32, 1062476, LabelColor, false, false); // Set Filter

            BOBFilter f = (from.UseOwnFilter ? from.BOBFilter : book.Filter);

            if (f.IsDefault)
                AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062475, 16927, false, false); // Using No Filter
            else if (from.UseOwnFilter)
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
                {
                    AddHtmlLocalized(576, 64, 200, 32, 1062219, LabelColor, false, false); // Buy
                }
                else
                {
                    AddHtmlLocalized(576, 64, 200, 32, 1062227, LabelColor, false, false); // Set

                    AddButton(450, 416, 4005, 4007, 4, GumpButtonType.Reply, 0);
                    AddHtml(485, 416, 120, 20, "<BASEFONT COLOR=#FFFFFF>Price all</FONT>", false, false);
                }
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
                object obj = list[i];

                if (!CheckFilter(obj))
                    continue;

                if (obj is BOBLargeEntry)
                {
                    BOBLargeEntry e = (BOBLargeEntry)obj;

                    int y = 96 + (tableIndex * 32);

                    if (canDrop)
                        AddButton(35, y + 2, 5602, 5606, 5 + (i * 2), GumpButtonType.Reply, 0);

                    if (canDrop || (canBuy && e.Price > 0))
                    {
                        AddButton(579, y + 2, 2117, 2118, 6 + (i * 2), GumpButtonType.Reply, 0);
                        AddLabel(495, y, 1152, e.Price.ToString());
                    }

                    AddHtmlLocalized(61, y, 50, 32, 1062225, LabelColor, false, false); // Large

                    for (int j = 0; j < e.Entries.Length; ++j)
                    {
                        BOBLargeSubEntry sub = e.Entries[j];

                        AddHtmlLocalized(103, y, 130, 32, sub.Number, LabelColor, false, false);

                        if (e.RequireExceptional)
                            AddHtmlLocalized(235, y, 80, 20, 1060636, LabelColor, false, false); // exceptional
                        else
                            AddHtmlLocalized(235, y, 80, 20, 1011542, LabelColor, false, false); // normal

                        object name = GetMaterialName(e.Material, e.DeedType, sub.ItemType);

                        if (name is int)
                            AddHtmlLocalized(316, y, 100, 20, (int)name, LabelColor, false, false);
                        else if (name is string)
                            AddLabel(316, y, 1152, (string)name);

                        AddLabel(421, y, 1152, string.Format("{0} / {1}", sub.AmountCur, e.AmountMax));

                        ++tableIndex;
                        y += 32;
                    }
                }
                else if (obj is BOBSmallEntry)
                {
                    BOBSmallEntry e = (BOBSmallEntry)obj;

                    int y = 96 + (tableIndex++ * 32);

                    if (canDrop)
                        AddButton(35, y + 2, 5602, 5606, 5 + (i * 2), GumpButtonType.Reply, 0);

                    if (canDrop || (canBuy && e.Price > 0))
                    {
                        AddButton(579, y + 2, 2117, 2118, 6 + (i * 2), GumpButtonType.Reply, 0);
                        AddLabel(495, y, 1152, e.Price.ToString());
                    }

                    AddHtmlLocalized(61, y, 50, 32, 1062224, LabelColor, false, false); // Small

                    AddHtmlLocalized(103, y, 130, 32, e.Number, LabelColor, false, false);

                    if (e.RequireExceptional)
                        AddHtmlLocalized(235, y, 80, 20, 1060636, LabelColor, false, false); // exceptional
                    else
                        AddHtmlLocalized(235, y, 80, 20, 1011542, LabelColor, false, false); // normal

                    object name = GetMaterialName(e.Material, e.DeedType, e.ItemType);

                    if (name is int)
                        AddHtmlLocalized(316, y, 100, 20, (int)name, LabelColor, false, false);
                    else if (name is string)
                        AddLabel(316, y, 1152, (string)name);

                    AddLabel(421, y, 1152, string.Format("{0} / {1}", e.AmountCur, e.AmountMax));
                }
            }
        }

        public Item Reconstruct(object obj)
        {
            Item item = null;

            if (obj is BOBLargeEntry)
                item = ((BOBLargeEntry)obj).Reconstruct();
            else if (obj is BOBSmallEntry)
                item = ((BOBSmallEntry)obj).Reconstruct();

            return item;
        }

        public bool CheckFilter(object obj)
        {
            if (obj is BOBLargeEntry)
            {
                BOBLargeEntry e = (BOBLargeEntry)obj;

                return CheckFilter(e.Material, e.AmountMax, true, e.RequireExceptional, e.DeedType, (e.Entries.Length > 0 ? e.Entries[0].ItemType : null));
            }
            else if (obj is BOBSmallEntry)
            {
                BOBSmallEntry e = (BOBSmallEntry)obj;

                return CheckFilter(e.Material, e.AmountMax, false, e.RequireExceptional, e.DeedType, e.ItemType);
            }

            return false;
        }

        public bool CheckFilter(BulkMaterialType mat, int amountMax, bool isLarge, bool reqExc, BODType deedType, Type itemType)
        {
            BOBFilter f = (m_From.UseOwnFilter ? m_From.BOBFilter : m_Book.Filter);

            if (f.IsDefault)
                return true;

            if (f.Quality == 1 && reqExc)
                return false;
            else if (f.Quality == 2 && !reqExc)
                return false;

            if (f.Quantity == 1 && amountMax != 10)
                return false;
            else if (f.Quantity == 2 && amountMax != 15)
                return false;
            else if (f.Quantity == 3 && amountMax != 20)
                return false;

            if (f.Type == 1 && isLarge)
                return false;
            else if (f.Type == 2 && !isLarge)
                return false;

            if (BulkOrderSystem.NewSystemEnabled)
            {
                switch (f.Material)
                {
                    default:
                    case 0:
                        return true;
                    case 1:
                        return deedType == BODType.Smith;
                    case 2:
                        return deedType == BODType.Tailor;
                    case 3:
                        return deedType == BODType.Tinkering;
                    case 4:
                        return deedType == BODType.Carpentry;
                    case 5:
                        return deedType == BODType.Fletching;
                    case 6:
                        return deedType == BODType.Alchemy;
                    case 7:
                        return deedType == BODType.Inscription;
                    case 8:
                        return deedType == BODType.Cooking;
                    case 9:
                        return (mat == BulkMaterialType.None && deedType == BODType.Smith);
                    case 10:
                        return (mat == BulkMaterialType.DullCopper && deedType == BODType.Smith);
                    case 11:
                        return (mat == BulkMaterialType.ShadowIron && deedType == BODType.Smith);
                    case 12:
                        return (mat == BulkMaterialType.Copper && deedType == BODType.Smith);
                    case 13:
                        return (mat == BulkMaterialType.Bronze && deedType == BODType.Smith);
                    case 14:
                        return (mat == BulkMaterialType.Gold && deedType == BODType.Smith);
                    case 15:
                        return (mat == BulkMaterialType.Agapite && deedType == BODType.Smith);
                    case 16:
                        return (mat == BulkMaterialType.Verite && deedType == BODType.Smith);
                    case 17:
                        return (mat == BulkMaterialType.Valorite && deedType == BODType.Smith);

                    case 18:
                        return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Cloth);
                    case 19:
                        return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Leather);
                    case 20:
                        return (mat == BulkMaterialType.Spined && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Leather);
                    case 21:
                        return (mat == BulkMaterialType.Horned && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Leather);
                    case 22:
                        return (mat == BulkMaterialType.Barbed && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Leather);

                    case 23: // Tinkering
                        return (mat == BulkMaterialType.None && deedType == BODType.Tinkering);
                    case 24:
                        return (mat == BulkMaterialType.DullCopper && deedType == BODType.Tinkering);
                    case 25:
                        return (mat == BulkMaterialType.ShadowIron && deedType == BODType.Tinkering);
                    case 26:
                        return (mat == BulkMaterialType.Copper && deedType == BODType.Tinkering);
                    case 27:
                        return (mat == BulkMaterialType.Bronze && deedType == BODType.Tinkering);
                    case 28:
                        return (mat == BulkMaterialType.Gold && deedType == BODType.Tinkering);
                    case 29:
                        return (mat == BulkMaterialType.Agapite && deedType == BODType.Tinkering);
                    case 30:
                        return (mat == BulkMaterialType.Verite && deedType == BODType.Tinkering);
                    case 31:
                        return (mat == BulkMaterialType.Valorite && deedType == BODType.Tinkering);

                    case 32: // Carpentry
                        return (mat == BulkMaterialType.None && deedType == BODType.Carpentry);
                    case 33:
                        return (mat == BulkMaterialType.OakWood && deedType == BODType.Carpentry);
                    case 34:
                        return (mat == BulkMaterialType.AshWood && deedType == BODType.Carpentry);
                    case 35:
                        return (mat == BulkMaterialType.YewWood && deedType == BODType.Carpentry);
                    case 36:
                        return (mat == BulkMaterialType.Bloodwood && deedType == BODType.Carpentry);
                    case 37:
                        return (mat == BulkMaterialType.Heartwood && deedType == BODType.Carpentry);
                    case 38:
                        return (mat == BulkMaterialType.Frostwood && deedType == BODType.Carpentry);

                    case 39: // Fletching
                        return (mat == BulkMaterialType.None && deedType == BODType.Fletching);
                    case 40:
                        return (mat == BulkMaterialType.OakWood && deedType == BODType.Fletching);
                    case 41:
                        return (mat == BulkMaterialType.AshWood && deedType == BODType.Fletching);
                    case 42:
                        return (mat == BulkMaterialType.YewWood && deedType == BODType.Fletching);
                    case 43:
                        return (mat == BulkMaterialType.Bloodwood && deedType == BODType.Fletching);
                    case 44:
                        return (mat == BulkMaterialType.Heartwood && deedType == BODType.Fletching);
                    case 45:
                        return (mat == BulkMaterialType.Frostwood && deedType == BODType.Fletching);
                }
            }
            else
            {
                switch (f.Material)
                {
                    default:
                    case 0: return true;
                    case 1: return (deedType == BODType.Smith);
                    case 2: return (deedType == BODType.Tailor);

                    case 3: return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Iron);
                    case 4: return (mat == BulkMaterialType.DullCopper);
                    case 5: return (mat == BulkMaterialType.ShadowIron);
                    case 6: return (mat == BulkMaterialType.Copper);
                    case 7: return (mat == BulkMaterialType.Bronze);
                    case 8: return (mat == BulkMaterialType.Gold);
                    case 9: return (mat == BulkMaterialType.Agapite);
                    case 10: return (mat == BulkMaterialType.Verite);
                    case 11: return (mat == BulkMaterialType.Valorite);

                    case 12: return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Cloth);
                    case 13: return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Leather);
                    case 14: return (mat == BulkMaterialType.Spined);
                    case 15: return (mat == BulkMaterialType.Horned);
                    case 16: return (mat == BulkMaterialType.Barbed);
                }
            }
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

            ArrayList list = m_List;

            for (int i = index; i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                if (CheckFilter(obj))
                {
                    int add;

                    if (obj is BOBLargeEntry)
                        add = ((BOBLargeEntry)obj).Entries.Length;
                    else
                        add = 1;

                    if ((slots + add) > 10)
                        break;

                    slots += add;
                }

                ++count;
            }

            return count;
        }

        public int GetPageForIndex(int index, int sizeDropped)
        {
            if (index <= 0)
                return 0;

            int count = 0;
            int add = 0;
            int page = 0;
            ArrayList list = m_List;
            int i;
            object obj;

            for (i = 0; (i < index) && (i < list.Count); i++)
            {
                obj = list[i];
                if (CheckFilter(obj))
                {
                    if (obj is BOBLargeEntry)
                        add = ((BOBLargeEntry)obj).Entries.Length;
                    else
                        add = 1;
                    count += add;
                    if (count > 10)
                    {
                        page++;
                        count = add;
                    }
                }
            }
            /* now we are on the page of the bod preceeding the dropped one.
            * next step: checking whether we have to remain where we are.
            * The counter i needs to be incremented as the bod to this very moment
            * has not yet been removed from m_List */
            i++;

            /* if, for instance, a big bod of size 6 has been removed, smaller bods
            * might fall back into this page. Depending on their sizes, the page eeds
            * to be adjusted accordingly. This is done now.
            */
            if (count + sizeDropped > 10)
            {
                while ((i < list.Count) && (count <= 10))
                {
                    obj = list[i];
                    if (CheckFilter(obj))
                    {
                        if (obj is BOBLargeEntry)
                            count += ((BOBLargeEntry)obj).Entries.Length;
                        else
                            count += 1;
                    }
                    i++;
                }
                if (count > 10)
                    page++;
            }
            return page;
        }

        public object GetMaterialName(BulkMaterialType mat, BODType type, Type itemType)
        {
            switch (type)
            {
                case BODType.Tinkering:
                case BODType.Smith:
                    {
                        if (type == BODType.Tinkering && mat == BulkMaterialType.None && BGTClassifier.Classify(type, itemType) == BulkGenericType.Wood)
                        {
                            return 1079435;
                        }
                        else
                        {
                            switch (mat)
                            {
                                case BulkMaterialType.None:
                                    return 1062226;
                                case BulkMaterialType.DullCopper:
                                    return 1018332;
                                case BulkMaterialType.ShadowIron:
                                    return 1018333;
                                case BulkMaterialType.Copper:
                                    return 1018334;
                                case BulkMaterialType.Bronze:
                                    return 1018335;
                                case BulkMaterialType.Gold:
                                    return 1018336;
                                case BulkMaterialType.Agapite:
                                    return 1018337;
                                case BulkMaterialType.Verite:
                                    return 1018338;
                                case BulkMaterialType.Valorite:
                                    return 1018339;
                            }
                        }

                        break;
                    }
                case BODType.Tailor:
                    {
                        switch (mat)
                        {
                            case BulkMaterialType.None:
                                {
                                    if (itemType.IsSubclassOf(typeof(BaseArmor)) || itemType.IsSubclassOf(typeof(BaseShoes)))
                                        return 1062235;

                                    return 1044286;
                                }
                            case BulkMaterialType.Spined:
                                return 1062236;
                            case BulkMaterialType.Horned:
                                return 1062237;
                            case BulkMaterialType.Barbed:
                                return 1062238;
                        }

                        break;
                    }
                case BODType.Carpentry:
                case BODType.Fletching:
                    {
                        if (mat == BulkMaterialType.None)
                            return 1079435;

                        switch (mat)
                        {
                            default:
                            case BulkMaterialType.OakWood: return 1071428;
                            case BulkMaterialType.AshWood: return 1071429;
                            case BulkMaterialType.YewWood: return 1071430;
                            case BulkMaterialType.Heartwood: return 1071432;
                            case BulkMaterialType.Bloodwood: return 1071431;
                            case BulkMaterialType.Frostwood: return 1071433;
                        }
                    }
            }

            return "";
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            int index = info.ButtonID;

            switch (index)
            {
                case 0: // EXIT
                    {
                        break;
                    }
                case 1: // Set Filter
                    {
                        m_From.SendGump(new BOBFilterGump(m_From, m_Book));

                        break;
                    }
                case 2: // Previous page
                    {
                        if (m_Page > 0)
                            m_From.SendGump(new BOBGump(m_From, m_Book, m_Page - 1, m_List));

                        return;
                    }
                case 3: // Next page
                    {
                        if (GetIndexForPage(m_Page + 1) < m_List.Count)
                            m_From.SendGump(new BOBGump(m_From, m_Book, m_Page + 1, m_List));

                        break;
                    }
                case 4: // Price all
                    {
                        if (m_Book.IsChildOf(m_From.Backpack))
                        {
                            m_From.Prompt = new SetPricePrompt(m_Book, null, m_Page, m_List);
                            m_From.SendMessage("Type in a price for all deeds in the book:");
                        }

                        break;
                    }
                default:
                    {
                        bool canDrop = m_Book.IsChildOf(m_From.Backpack);
                        bool canPrice = canDrop || (m_Book.RootParent is PlayerVendor);

                        index -= 5;

                        int type = index % 2;
                        index /= 2;

                        if (index < 0 || index >= m_List.Count)
                            break;

                        object obj = m_List[index];

                        if (!m_Book.Entries.Contains(obj))
                        {
                            m_From.SendLocalizedMessage(1062382); // The deed selected is not available.
                            break;
                        }

                        if (type == 0) // Drop
                        {
                            if (m_Book.IsChildOf(m_From.Backpack))
                            {
                                Item item = Reconstruct(obj);

                                if (item != null)
                                {
                                    Container pack = m_From.Backpack;
                                    if ((pack == null) || ((pack != null) && (!pack.CheckHold(m_From, item, true, true, 0, item.PileWeight + item.TotalWeight))))
                                    {
                                        m_From.SendLocalizedMessage(503204); // You do not have room in your backpack for this
                                        m_From.SendGump(new BOBGump(m_From, m_Book, m_Page, null));
                                    }
                                    else
                                    {
                                        if (m_Book.IsChildOf(m_From.Backpack))
                                        {
                                            int sizeOfDroppedBod;
                                            if (obj is BOBLargeEntry)
                                                sizeOfDroppedBod = ((BOBLargeEntry)obj).Entries.Length;
                                            else
                                                sizeOfDroppedBod = 1;

                                            m_From.AddToBackpack(item);
                                            m_From.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.
                                            m_Book.Entries.Remove(obj);
                                            m_Book.InvalidateProperties();

                                            if (m_Book.Entries.Count / 5 < m_Book.ItemCount)
                                            {
                                                m_Book.ItemCount--;
                                                m_Book.InvalidateItems();
                                            }

                                            if (m_Book.Entries.Count > 0)
                                            {
                                                m_Page = GetPageForIndex(index, sizeOfDroppedBod);
                                                m_From.SendGump(new BOBGump(m_From, m_Book, m_Page, null));
                                            }
                                            else
                                                m_From.SendLocalizedMessage(1062381); // The book is empty.
                                        }
                                    }
                                }
                                else
                                {
                                    m_From.SendMessage("Internal error. The bulk order deed could not be reconstructed.");
                                }
                            }
                        }
                        else // Set Price | Buy
                        {
                            if (m_Book.IsChildOf(m_From.Backpack))
                            {
                                m_From.Prompt = new SetPricePrompt(m_Book, obj, m_Page, m_List);
                                m_From.SendLocalizedMessage(1062383); // Type in a price for the deed:
                            }
                            else if (m_Book.RootParent is PlayerVendor)
                            {
                                PlayerVendor pv = (PlayerVendor)m_Book.RootParent;
                                VendorItem vi = pv.GetVendorItem(m_Book);

                                if (vi != null && !vi.IsForSale)
                                {
                                    int sizeOfDroppedBod;
                                    int price = 0;
                                    if (obj is BOBLargeEntry)
                                    {
                                        price = ((BOBLargeEntry)obj).Price;
                                        sizeOfDroppedBod = ((BOBLargeEntry)obj).Entries.Length;
                                    }
                                    else
                                    {
                                        price = ((BOBSmallEntry)obj).Price;
                                        sizeOfDroppedBod = 1;
                                    }
                                    if (price == 0)
                                        m_From.SendLocalizedMessage(1062382); // The deed selected is not available.
                                    else
                                    {
                                        if (m_Book.Entries.Count > 0)
                                        {
                                            m_Page = GetPageForIndex(index, sizeOfDroppedBod);
                                            m_From.SendGump(new BODBuyGump(m_From, m_Book, obj, m_Page, price));
                                        }
                                        else
                                            m_From.SendLocalizedMessage(1062381); // The book is emptz
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        private class SetPricePrompt : Prompt
        {
            public override int MessageCliloc => 1062383;
            private readonly BulkOrderBook m_Book;
            private readonly object m_Object;
            private readonly int m_Page;
            private readonly ArrayList m_List;
            public SetPricePrompt(BulkOrderBook book, object obj, int page, ArrayList list)
            {
                m_Book = book;
                m_Object = obj;
                m_Page = page;
                m_List = list;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Object != null && !m_Book.Entries.Contains(m_Object))
                {
                    from.SendLocalizedMessage(1062382); // The deed selected is not available.
                    return;
                }

                int price = Utility.ToInt32(text);

                if (price < 0 || price > 250000000)
                {
                    from.SendLocalizedMessage(1062390); // The price you requested is outrageous!
                }
                else if (m_Object == null)
                {
                    for (int i = 0; i < m_List.Count; ++i)
                    {
                        object obj = m_List[i];

                        if (!m_Book.Entries.Contains(obj))
                            continue;

                        if (obj is BOBLargeEntry)
                            ((BOBLargeEntry)obj).Price = price;
                        else if (obj is BOBSmallEntry)
                            ((BOBSmallEntry)obj).Price = price;
                    }

                    from.SendMessage("Deed prices set.");

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, m_Book, m_Page, m_List));
                }
                else if (m_Object is BOBLargeEntry)
                {
                    ((BOBLargeEntry)m_Object).Price = price;

                    from.SendLocalizedMessage(1062384); // Deed price set.

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, m_Book, m_Page, m_List));
                }
                else if (m_Object is BOBSmallEntry)
                {
                    ((BOBSmallEntry)m_Object).Price = price;

                    from.SendLocalizedMessage(1062384); // Deed price set.

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, m_Book, m_Page, m_List));
                }
            }
        }
    }
}