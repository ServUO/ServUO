using System;
using System.Collections;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;

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

            this.m_From = from;
            this.m_Book = book;
            this.m_Page = page;

            if (list == null)
            {
                list = new ArrayList(book.Entries.Count);

                for (int i = 0; i < book.Entries.Count; ++i)
                {
                    object obj = book.Entries[i];

                    if (this.CheckFilter(obj))
                        list.Add(obj);
                }
            }

            this.m_List = list;

            int index = this.GetIndexForPage(page);
            int count = this.GetCountForIndex(index);

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

            this.X = (624 - width) / 2;

            this.AddPage(0);

            this.AddBackground(10, 10, width, 439, 5054);
            this.AddImageTiled(18, 20, width - 17, 420, 2624);

            if (canPrice)
            {
                this.AddImageTiled(573, 64, 24, 352, 200);
                this.AddImageTiled(493, 64, 78, 352, 1416);
            }

            if (canDrop)
                this.AddImageTiled(24, 64, 32, 352, 1416);

            this.AddImageTiled(58, 64, 36, 352, 200);
            this.AddImageTiled(96, 64, 133, 352, 1416);
            this.AddImageTiled(231, 64, 80, 352, 200);
            this.AddImageTiled(313, 64, 100, 352, 1416);
            this.AddImageTiled(415, 64, 76, 352, 200);

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                if (!this.CheckFilter(obj))
                    continue;

                this.AddImageTiled(24, 94 + (tableIndex * 32), canPrice ? 573 : 489, 2, 2624);

                if (obj is BOBLargeEntry)
                    tableIndex += ((BOBLargeEntry)obj).Entries.Length;
                else if (obj is BOBSmallEntry)
                    ++tableIndex;
            }

            this.AddAlphaRegion(18, 20, width - 17, 420);
            this.AddImage(5, 5, 10460);
            this.AddImage(width - 15, 5, 10460);
            this.AddImage(5, 424, 10460);
            this.AddImage(width - 15, 424, 10460);

            this.AddHtmlLocalized(canPrice ? 266 : 224, 32, 200, 32, 1062220, LabelColor, false, false); // Bulk Order Book
            this.AddHtmlLocalized(63, 64, 200, 32, 1062213, LabelColor, false, false); // Type
            this.AddHtmlLocalized(147, 64, 200, 32, 1062214, LabelColor, false, false); // Item
            this.AddHtmlLocalized(246, 64, 200, 32, 1062215, LabelColor, false, false); // Quality
            this.AddHtmlLocalized(336, 64, 200, 32, 1062216, LabelColor, false, false); // Material
            this.AddHtmlLocalized(429, 64, 200, 32, 1062217, LabelColor, false, false); // Amount

            this.AddButton(35, 32, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(70, 32, 200, 32, 1062476, LabelColor, false, false); // Set Filter

            BOBFilter f = (from.UseOwnFilter ? from.BOBFilter : book.Filter);

            if (f.IsDefault)
                this.AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062475, 16927, false, false); // Using No Filter
            else if (from.UseOwnFilter)
                this.AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062451, 16927, false, false); // Using Your Filter
            else
                this.AddHtmlLocalized(canPrice ? 470 : 386, 32, 120, 32, 1062230, 16927, false, false); // Using Book Filter

            this.AddButton(375, 416, 4017, 4018, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(410, 416, 120, 20, 1011441, LabelColor, false, false); // EXIT

            if (canDrop)
                this.AddHtmlLocalized(26, 64, 50, 32, 1062212, LabelColor, false, false); // Drop

            if (canPrice)
            {
                this.AddHtmlLocalized(516, 64, 200, 32, 1062218, LabelColor, false, false); // Price

                if (canBuy)
                {
                    this.AddHtmlLocalized(576, 64, 200, 32, 1062219, LabelColor, false, false); // Buy
                }
                else
                {
                    this.AddHtmlLocalized(576, 64, 200, 32, 1062227, LabelColor, false, false); // Set

                    this.AddButton(450, 416, 4005, 4007, 4, GumpButtonType.Reply, 0);
                    this.AddHtml(485, 416, 120, 20, "<BASEFONT COLOR=#FFFFFF>Price all</FONT>", false, false);
                }
            }

            tableIndex = 0;

            if (page > 0)
            {
                this.AddButton(75, 416, 4014, 4016, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(110, 416, 150, 20, 1011067, LabelColor, false, false); // Previous page
            }

            if (this.GetIndexForPage(page + 1) < list.Count)
            {
                this.AddButton(225, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(260, 416, 150, 20, 1011066, LabelColor, false, false); // Next page
            }

            for (int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                if (!this.CheckFilter(obj))
                    continue;

                if (obj is BOBLargeEntry)
                {
                    BOBLargeEntry e = (BOBLargeEntry)obj;

                    int y = 96 + (tableIndex * 32);

                    if (canDrop)
                        this.AddButton(35, y + 2, 5602, 5606, 5 + (i * 2), GumpButtonType.Reply, 0);

                    if (canDrop || (canBuy && e.Price > 0))
                    {
                        this.AddButton(579, y + 2, 2117, 2118, 6 + (i * 2), GumpButtonType.Reply, 0);
                        this.AddLabel(495, y, 1152, e.Price.ToString());
                    }

                    this.AddHtmlLocalized(61, y, 50, 32, 1062225, LabelColor, false, false); // Large

                    for (int j = 0; j < e.Entries.Length; ++j)
                    {
                        BOBLargeSubEntry sub = e.Entries[j];

                        this.AddHtmlLocalized(103, y, 130, 32, sub.Number, LabelColor, false, false);

                        if (e.RequireExceptional)
                            this.AddHtmlLocalized(235, y, 80, 20, 1060636, LabelColor, false, false); // exceptional
                        else
                            this.AddHtmlLocalized(235, y, 80, 20, 1011542, LabelColor, false, false); // normal

                        object name = this.GetMaterialName(e.Material, e.DeedType, sub.ItemType);

                        if (name is int)
                            this.AddHtmlLocalized(316, y, 100, 20, (int)name, LabelColor, false, false);
                        else if (name is string)
                            this.AddLabel(316, y, 1152, (string)name);

                        this.AddLabel(421, y, 1152, String.Format("{0} / {1}", sub.AmountCur, e.AmountMax));

                        ++tableIndex;
                        y += 32;
                    }
                }
                else if (obj is BOBSmallEntry)
                {
                    BOBSmallEntry e = (BOBSmallEntry)obj;

                    int y = 96 + (tableIndex++ * 32);

                    if (canDrop)
                        this.AddButton(35, y + 2, 5602, 5606, 5 + (i * 2), GumpButtonType.Reply, 0);

                    if (canDrop || (canBuy && e.Price > 0))
                    {
                        this.AddButton(579, y + 2, 2117, 2118, 6 + (i * 2), GumpButtonType.Reply, 0);
                        this.AddLabel(495, y, 1152, e.Price.ToString());
                    }

                    this.AddHtmlLocalized(61, y, 50, 32, 1062224, LabelColor, false, false); // Small

                    this.AddHtmlLocalized(103, y, 130, 32, e.Number, LabelColor, false, false);

                    if (e.RequireExceptional)
                        this.AddHtmlLocalized(235, y, 80, 20, 1060636, LabelColor, false, false); // exceptional
                    else
                        this.AddHtmlLocalized(235, y, 80, 20, 1011542, LabelColor, false, false); // normal

                    object name = this.GetMaterialName(e.Material, e.DeedType, e.ItemType);

                    if (name is int)
                        this.AddHtmlLocalized(316, y, 100, 20, (int)name, LabelColor, false, false);
                    else if (name is string)
                        this.AddLabel(316, y, 1152, (string)name);

                    this.AddLabel(421, y, 1152, String.Format("{0} / {1}", e.AmountCur, e.AmountMax));
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

                return this.CheckFilter(e.Material, e.AmountMax, true, e.RequireExceptional, e.DeedType, (e.Entries.Length > 0 ? e.Entries[0].ItemType : null));
            }
            else if (obj is BOBSmallEntry)
            {
                BOBSmallEntry e = (BOBSmallEntry)obj;

                return this.CheckFilter(e.Material, e.AmountMax, false, e.RequireExceptional, e.DeedType, e.ItemType);
            }

            return false;
        }

        public bool CheckFilter(BulkMaterialType mat, int amountMax, bool isLarge, bool reqExc, BODType deedType, Type itemType)
        {
            BOBFilter f = (this.m_From.UseOwnFilter ? this.m_From.BOBFilter : this.m_Book.Filter);

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

            switch ( f.Material )
            {
                default:
                case 0:
                    return true;
                case 1:
                    return (deedType == BODType.Smith);
                case 2:
                    return (deedType == BODType.Tailor);

                case 3:
                    return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Iron);
                case 4:
                    return (mat == BulkMaterialType.DullCopper);
                case 5:
                    return (mat == BulkMaterialType.ShadowIron);
                case 6:
                    return (mat == BulkMaterialType.Copper);
                case 7:
                    return (mat == BulkMaterialType.Bronze);
                case 8:
                    return (mat == BulkMaterialType.Gold);
                case 9:
                    return (mat == BulkMaterialType.Agapite);
                case 10:
                    return (mat == BulkMaterialType.Verite);
                case 11:
                    return (mat == BulkMaterialType.Valorite);

                case 12:
                    return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Cloth);
                case 13:
                    return (mat == BulkMaterialType.None && BGTClassifier.Classify(deedType, itemType) == BulkGenericType.Leather);
                case 14:
                    return (mat == BulkMaterialType.Spined);
                case 15:
                    return (mat == BulkMaterialType.Horned);
                case 16:
                    return (mat == BulkMaterialType.Barbed);
            }
        }

        public int GetIndexForPage(int page)
        {
            int index = 0;

            while (page-- > 0)
                index += this.GetCountForIndex(index);

            return index;
        }

        public int GetCountForIndex(int index)
        {
            int slots = 0;
            int count = 0;

            ArrayList list = this.m_List;

            for (int i = index; i >= 0 && i < list.Count; ++i)
            {
                object obj = list[i];

                if (this.CheckFilter(obj))
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
            ArrayList list = this.m_List;
            int i;
            object obj;

            for (i = 0; (i < index) && (i < list.Count); i++)
            {
                obj = list[i];
                if (this.CheckFilter(obj))
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
                    if (this.CheckFilter(obj))
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
            switch ( type )
            {
                case BODType.Smith:
                    {
                        switch ( mat )
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

                        break;
                    }
                case BODType.Tailor:
                    {
                        switch ( mat )
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
            }

            return "Invalid";
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            int index = info.ButtonID;

            switch ( index )
            {
                case 0: // EXIT
                    {
                        break;
                    }
                case 1: // Set Filter
                    {
                        this.m_From.SendGump(new BOBFilterGump(this.m_From, this.m_Book));

                        break;
                    }
                case 2: // Previous page
                    {
                        if (this.m_Page > 0)
                            this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book, this.m_Page - 1, this.m_List));

                        return;
                    }
                case 3: // Next page
                    {
                        if (this.GetIndexForPage(this.m_Page + 1) < this.m_List.Count)
                            this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book, this.m_Page + 1, this.m_List));

                        break;
                    }
                case 4: // Price all
                    {
                        if (this.m_Book.IsChildOf(this.m_From.Backpack))
                        {
                            this.m_From.Prompt = new SetPricePrompt(this.m_Book, null, this.m_Page, this.m_List);
                            this.m_From.SendMessage("Type in a price for all deeds in the book:");
                        }

                        break;
                    }
                default:
                    {
                        bool canDrop = this.m_Book.IsChildOf(this.m_From.Backpack);
                        bool canPrice = canDrop || (this.m_Book.RootParent is PlayerVendor);

                        index -= 5;

                        int type = index % 2;
                        index /= 2;

                        if (index < 0 || index >= this.m_List.Count)
                            break;

                        object obj = this.m_List[index];

                        if (!this.m_Book.Entries.Contains(obj))
                        {
                            this.m_From.SendLocalizedMessage(1062382); // The deed selected is not available.
                            break;
                        }

                        if (type == 0) // Drop
                        {
                            if (this.m_Book.IsChildOf(this.m_From.Backpack))
                            {
                                Item item = this.Reconstruct(obj);

                                if (item != null)
                                {
                                    Container pack = this.m_From.Backpack;
                                    if ((pack == null) || ((pack != null) && (!pack.CheckHold(this.m_From, item, true, true, 0, item.PileWeight + item.TotalWeight))))
                                    {
                                        this.m_From.SendLocalizedMessage(503204); // You do not have room in your backpack for this
                                        this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book, this.m_Page, null));
                                    }
                                    else
                                    {
                                        if (this.m_Book.IsChildOf(this.m_From.Backpack))
                                        {
                                            int sizeOfDroppedBod;
                                            if (obj is BOBLargeEntry)
                                                sizeOfDroppedBod = ((BOBLargeEntry)obj).Entries.Length;
                                            else
                                                sizeOfDroppedBod = 1;

                                            this.m_From.AddToBackpack(item);
                                            this.m_From.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.
                                            this.m_Book.Entries.Remove(obj);
                                            this.m_Book.InvalidateProperties();
										
                                            if (this.m_Book.Entries.Count / 5 < this.m_Book.ItemCount)
                                            {
                                                this.m_Book.ItemCount--;
                                                this.m_Book.InvalidateItems();
                                            }
										
                                            if (this.m_Book.Entries.Count > 0)
                                            {
                                                this.m_Page = this.GetPageForIndex(index, sizeOfDroppedBod);
                                                this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book, this.m_Page, null));
                                            }
                                            else
                                                this.m_From.SendLocalizedMessage(1062381); // The book is empty.
                                        }
                                    }
                                }
                                else
                                {
                                    this.m_From.SendMessage("Internal error. The bulk order deed could not be reconstructed.");
                                }
                            }
                        }
                        else // Set Price | Buy
                        {
                            if (this.m_Book.IsChildOf(this.m_From.Backpack))
                            {
                                this.m_From.Prompt = new SetPricePrompt(this.m_Book, obj, this.m_Page, this.m_List);
                                this.m_From.SendLocalizedMessage(1062383); // Type in a price for the deed:
                            }
                            else if (this.m_Book.RootParent is PlayerVendor)
                            {
                                PlayerVendor pv = (PlayerVendor)this.m_Book.RootParent;
                                VendorItem vi = pv.GetVendorItem(this.m_Book);

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
                                        this.m_From.SendLocalizedMessage(1062382); // The deed selected is not available.
                                    else
                                    {
                                        if (this.m_Book.Entries.Count > 0)
                                        {
                                            this.m_Page = this.GetPageForIndex(index, sizeOfDroppedBod);
                                            this.m_From.SendGump(new BODBuyGump(this.m_From, this.m_Book, obj, this.m_Page, price));
                                        }
                                        else
                                            this.m_From.SendLocalizedMessage(1062381); // The book is emptz
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
            private readonly BulkOrderBook m_Book;
            private readonly object m_Object;
            private readonly int m_Page;
            private readonly ArrayList m_List;
            public SetPricePrompt(BulkOrderBook book, object obj, int page, ArrayList list)
            {
                this.m_Book = book;
                this.m_Object = obj;
                this.m_Page = page;
                this.m_List = list;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Object != null && !this.m_Book.Entries.Contains(this.m_Object))
                {
                    from.SendLocalizedMessage(1062382); // The deed selected is not available.
                    return;
                }

                int price = Utility.ToInt32(text);

                if (price < 0 || price > 250000000)
                {
                    from.SendLocalizedMessage(1062390); // The price you requested is outrageous!
                }
                else if (this.m_Object == null)
                {
                    for (int i = 0; i < this.m_List.Count; ++i)
                    {
                        object obj = this.m_List[i];

                        if (!this.m_Book.Entries.Contains(obj))
                            continue;

                        if (obj is BOBLargeEntry)
                            ((BOBLargeEntry)obj).Price = price;
                        else if (obj is BOBSmallEntry)
                            ((BOBSmallEntry)obj).Price = price;
                    }

                    from.SendMessage("Deed prices set.");

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, this.m_Book, this.m_Page, this.m_List));
                }
                else if (this.m_Object is BOBLargeEntry)
                {
                    ((BOBLargeEntry)this.m_Object).Price = price;

                    from.SendLocalizedMessage(1062384); // Deed price set.

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, this.m_Book, this.m_Page, this.m_List));
                }
                else if (this.m_Object is BOBSmallEntry)
                {
                    ((BOBSmallEntry)this.m_Object).Price = price;

                    from.SendLocalizedMessage(1062384); // Deed price set.

                    if (from is PlayerMobile)
                        from.SendGump(new BOBGump((PlayerMobile)from, this.m_Book, this.m_Page, this.m_List));
                }
            }
        }
    }
}