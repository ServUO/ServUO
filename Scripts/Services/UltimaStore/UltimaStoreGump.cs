using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.UOStore
{
    public class UltimaStoreGump : BaseGump
    {
        private readonly int[][] _Offset =
        {
            new[] { 167, 74 },
            new[] { 354, 74 },
            new[] { 541, 74 },
            new[] { 167, 294 },
            new[] { 354, 294 },
            new[] { 541, 294 }
        };

        public StoreCategory Category
        {
            get
            {
                PlayerProfile profile = UltimaStore.GetProfile(User, false);

                if (profile != null)
                {
                    return profile.Category;
                }

                return PlayerProfile.DefaultCategory;
            }
        }

        public SortBy SortBy
        {
            get
            {
                PlayerProfile profile = UltimaStore.GetProfile(User, false);

                if (profile != null)
                {
                    return profile.SortBy;
                }

                return PlayerProfile.DefaultSortBy;
            }
        }

        public Dictionary<StoreEntry, int> Cart
        {
            get
            {
                PlayerProfile profile = UltimaStore.GetProfile(User, false);

                if (profile != null)
                {
                    return profile.Cart;
                }

                return null;
            }
        }

        public int Page { get; private set; }
        public string SearchText { get; private set; }
        public List<StoreEntry> StoreList { get; private set; }

        public bool Search { get; private set; }

        public UltimaStoreGump(PlayerMobile pm, StoreEntry forcedEntry = null)
            : base(pm, 100, 200)
        {
            StoreList = UltimaStore.GetList(Category, forcedEntry);

            if (forcedEntry == null)
            {
                UltimaStore.SortList(StoreList, SortBy);
            }

            pm.Frozen = true;
            pm.Hidden = true;
            pm.TempSquelched = true;
        }

        public override void OnDispose()
        {
            ColUtility.Free(StoreList);

            StoreList = null;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);
            AddImage(0, 0, 0x9C49);

            AddECHandleInput();

            AddButton(36, 97, Category == StoreCategory.Featured ? 0x9C5F : 0x9C55, 0x9C5F, 100, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 100, 125, 25, 1114513, "#1156587", 0x7FFF, false, false); // Featured

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 126, Category == StoreCategory.Character ? 0x9C5F : 0x9C55, 0x9C5F, 101, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 129, 125, 25, 1114513, "#1156588", 0x7FFF, false, false); // Character

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 155, Category == StoreCategory.Equipment ? 0x9C5F : 0x9C55, 0x9C5F, 102, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 158, 125, 25, 1114513, "#1078237", 0x7FFF, false, false); // Equipment

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 184, Category == StoreCategory.Decorations ? 0x9C5F : 0x9C55, 0x9C5F, 103, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 187, 125, 25, 1114513, "#1044501", 0x7FFF, false, false); // Decorations

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 213, Category == StoreCategory.Mounts ? 0x9C5F : 0x9C55, 0x9C5F, 104, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 216, 125, 25, 1114513, "#1154981", 0x7FFF, false, false); // Mounts

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 242, Category == StoreCategory.Misc ? 0x9C5F : 0x9C55, 0x9C5F, 105, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 245, 125, 25, 1114513, "#1011173", 0x7FFF, false, false); // Miscellaneous

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 271, 0x9C55, 0x9C5F, 106, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 274, 125, 25, 1114513, "#1156589", 0x7FFF, false, false); // Promotional Code

            AddECHandleInput();
            AddECHandleInput();

            AddButton(36, 300, 0x9C55, 0x9C5F, 107, GumpButtonType.Reply, 0);
            AddHtmlLocalized(36, 303, 125, 25, 1114513, "#1156875", 0x7FFF, false, false); // FAQ

            AddECHandleInput();

            AddImage(36, 331, 0x9C4A);

            AddHtmlLocalized(36, 334, 125, 25, 1114513, "#1044580", 0x2945, false, false); // Sort By

            AddButton(43, 360, SortBy == SortBy.Name ? 0x9C4F : 0x9C4E, SortBy == SortBy.Name ? 0x9C4F : 0x9C4E, 108, GumpButtonType.Reply, 0);
            AddHtmlLocalized(68, 360, 88, 25, 1037013, 0x6B55, false, false); // Name

            AddButton(43, 386, SortBy == SortBy.PriceLower ? 0x9C4F : 0x9C4E, SortBy == SortBy.PriceLower ? 0x9C4F : 0x9C4E, 109, GumpButtonType.Reply, 0);
            AddHtmlLocalized(68, 386, 88, 25, 1062218, 0x6B55, false, false); // Price Down
            AddImage(110, 386, 0x9C60);

            AddButton(43, 412, SortBy == SortBy.PriceHigher ? 0x9C4F : 0x9C4E, SortBy == SortBy.PriceHigher ? 0x9C4F : 0x9C4E, 110, GumpButtonType.Reply, 0);
            AddHtmlLocalized(68, 412, 88, 25, 1062218, 0x6B55, false, false); // Price Up
            AddImage(110, 412, 0x9C61);

            AddButton(43, 438, SortBy == SortBy.Newest ? 0x9C4F : 0x9C4E, SortBy == SortBy.Newest ? 0x9C4F : 0x9C4E, 111, GumpButtonType.Reply, 0);
            AddHtmlLocalized(68, 438, 88, 25, 1156590, 0x6B55, false, false); // Newest

            AddButton(43, 464, SortBy == SortBy.Oldest ? 0x9C4F : 0x9C4E, SortBy == SortBy.Oldest ? 0x9C4F : 0x9C4E, 112, GumpButtonType.Reply, 0);
            AddHtmlLocalized(68, 464, 88, 25, 1156591, 0x6B55, false, false); // Oldest

            AddECHandleInput();

            AddButton(598, 36, Category == StoreCategory.Cart ? 0x9C5E : 0x9C54, 0x9C5E, 113, GumpButtonType.Reply, 0);
            AddHtmlLocalized(628, 39, 123, 25, 1156593, String.Format("@{0}@{1}", UltimaStore.CartCount(User), Configuration.CartCapacity), 0x7FFF, false, false);

            AddECHandleInput();

            AddBackground(167, 516, 114, 22, 0x2486);
            AddTextEntry(169, 518, 110, 18, 0, 0, "", 169);

            AddECHandleInput();

            AddButton(286, 516, 0x9C52, 0x9C5C, 114, GumpButtonType.Reply, 0);
            AddHtmlLocalized(286, 519, 64, 22, 1114513, "#1154641", 0x7FFF, false, false); // Search

            AddECHandleInput();

            AddImage(36, 74, 0x9C56);
            AddLabelCropped(59, 74, 100, 14, 0x1C7, UltimaStore.GetCurrency(User).ToString("N0"));

            AddECHandleInput();

            if (!Search && Category == StoreCategory.Cart)
            {
                PlayerProfile profile = UltimaStore.GetProfile(User);

                AddImage(167, 74, 0x9C4C);

                if (profile != null && profile.Cart != null && profile.Cart.Count > 0)
                {
                    int i = 0;

                    foreach (KeyValuePair<StoreEntry, int> kvp in profile.Cart)
                    {
                        StoreEntry entry = kvp.Key;
                        int amount = kvp.Value;

                        int index = UltimaStore.Entries.IndexOf(entry);

                        if (entry.Name[0].Number > 0)
                            AddHtmlLocalized(175, 84 + (35 * i), 256, 25, entry.Name[0].Number, 0x6B55, false, false);
                        else
                            AddHtml(175, 84 + (35 * i), 256, 25, Color(C16232(0x6B55), entry.Name[0].String), false, false);

                        AddButton(431, 81 + (35 * i), 0x9C52, 0x9C5C, index + 2000, GumpButtonType.Reply, 0);

                        AddLabelCropped(457, 82 + (35 * i), 38, 22, 0x9C2, amount.ToString());
                        AddLabelCropped(531, 82 + (35 * i), 100, 14, 0x1C7, (entry.Cost * amount).ToString("N0"));

                        AddButton(653, 81 + (35 * i), 0x9C52, 0x9C5C, index + 3000, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(653, 84 + (35 * i), 64, 22, 1114513, "#1011403", 0x7FFF, false, false); // Remove

                        AddImage(175, 109 + (35 * i), 0x9C4D);

                        ++i;
                    }
                }

                AddHtmlLocalized(508, 482, 125, 25, 1156594, 0x6B55, false, false); // Subtotal:
                AddImage(588, 482, 0x9C56);
                AddLabelCropped(611, 480, 100, 14, 0x1C7, UltimaStore.GetSubTotal(Cart).ToString("N0"));

                AddECHandleInput();
                AddECHandleInput();

                AddButton(653, 516, 0x9C52, 0x9C52, 115, GumpButtonType.Reply, 0);
                AddHtmlLocalized(653, 519, 64, 22, 1114513, "#1062219", 0x7FFF, false, false); // Buy
            }
            else
            {
                if (Search)
                {
                    StoreList = UltimaStore.GetSortedList(SearchText);

                    UltimaStore.SortList(StoreList, SortBy);

                    if (StoreList.Count == 0)
                    {
                        User.SendLocalizedMessage(1154587, "", 1281); // No items matched your search.
                        return;
                    }
                }

                int listIndex = Page * 6;
                int pageIndex = 0;
                int pages = (int)Math.Ceiling((double)StoreList.Count / 6);

                for (int i = listIndex; i < StoreList.Count && pageIndex < 6; i++)
                {
                    StoreEntry entry = StoreList[i];
                    int x = _Offset[pageIndex][0];
                    int y = _Offset[pageIndex][1];

                    AddButton(x, y, 0x9C4B, 0x9C4B, i + 1000, GumpButtonType.Reply, 0);

                    if (entry.Tooltip > 0)
                    {
                        AddTooltip(entry.Tooltip);
                    }
                    else
                    {
                        Item item = UltimaStore.UltimaStoreContainer.FindDisplayItem(entry.ItemType);

                        if (item != null)
                        {
                            AddItemProperty(item);
                        }
                    }

                    if (IsFeatured(entry))
                    {
                        AddImage(x, y + 189, 0x9C58);
                    }

                    for (int j = 0; j < entry.Name.Length; j++)
                    {
                        if (entry.Name[j].Number > 0)
                            AddHtmlLocalized(x, y + (j * 14) + 4, 183, 25, 1114513, String.Format("#{0}", entry.Name[j].Number.ToString()), 0x7FFF, false, false);
                        else
                            AddHtml(x, y + (j * 14) + 4, 183, 25, ColorAndCenter("#FFFFFF", entry.Name[j].String), false, false);
                    }

                    if (entry.ItemID > 0)
                    {
                        Rectangle2D b = ItemBounds.Table[entry.ItemID];

                        AddItem((x + 91) - b.Width / 2 - b.X, (y + 108) - b.Height / 2 - b.Y, entry.ItemID, entry.Hue);
                    }
                    else
                    {
                        AddImage((x + 91) - 72, (y + 108) - 72, entry.GumpID);
                    }

                    AddImage(x + 60, y + 192, 0x9C56);
                    AddLabelCropped(x + 80, y + 190, 143, 25, 0x9C2, entry.Cost.ToString("N0"));

                    AddECHandleInput();
                    AddECHandleInput();

                    ++pageIndex;
                    ++listIndex;
                }

                if (Page + 1 < pages)
                {
                    AddButton(692, 516, 0x9C51, 0x9C5B, 116, GumpButtonType.Reply, 0);
                }

                if (Page > 0)
                {
                    AddButton(648, 516, 0x9C50, 0x9C5A, 117, GumpButtonType.Reply, 0);
                }
            }

            if (Configuration.CurrencyDisplay)
            {
                AddHtml(43, 496, 120, 16, Color("#FFFFFF", "Currency:"), false, false);
                AddHtml(43, 518, 120, 16, Color("#FFFFFF", Configuration.CurrencyName), false, false);
            }
        }

        public bool IsFeatured(StoreEntry entry)
        {
            return entry.Category == StoreCategory.Featured ||
                UltimaStore.Entries.Any(e => e.ItemType == entry.ItemType && e.Category == StoreCategory.Featured);
        }

        public static void ReleaseHidden(PlayerMobile pm)
        {
            if (pm.HasGump(typeof(UltimaStoreGump)) || pm.HasGump(typeof(NoFundsGump)) ||
                pm.HasGump(typeof(ConfirmPurchaseGump)) || pm.HasGump(typeof(ConfirmCartGump)))
            {
                return;
            }

            pm.Frozen = false;
            pm.TempSquelched = false;
            pm.SendLocalizedMessage(501235, "", 0x35); // Help request aborted.

            if (pm.AccessLevel < AccessLevel.Counselor)
            {
                pm.RevealingAction();
            }
        }

        public override void OnServerClose(NetState owner)
        {
            if (owner.Mobile is PlayerMobile)
            {
                ReleaseHidden((PlayerMobile)owner.Mobile);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (id == 0)
            {
                ReleaseHidden(User);
                return;
            }

            PlayerProfile profile = UltimaStore.GetProfile(User);

            switch (id)
            {
                // Change Category
                case 100:
                case 101:
                case 102:
                case 103:
                case 104:
                case 105:
                    {
                        Search = false;

                        StoreCategory oldCat = profile.Category;

                        profile.Category = (StoreCategory)id - 99;

                        if (oldCat != profile.Category)
                        {
                            StoreList = UltimaStore.GetList(Category);
                            Page = 0;
                        }

                        Refresh();
                        return;
                    }

                // Promo Code
                case 106:
                    {
                        Refresh();
                        SendGump(new PromoCodeGump(User, this));
                        return;
                    }

                // FAQ
                case 107:
                    {
                        if (!String.IsNullOrWhiteSpace(Configuration.Website))
                        {
                            User.LaunchBrowser(Configuration.Website);
                        }
                        else
                        {
                            User.LaunchBrowser("https://uo.com/ultima-store/");
                        }

                        Refresh();
                        return;
                    }

                // Change Sort Method
                case 108:
                case 109:
                case 110:
                case 111:
                case 112:
                    {
                        SortBy oldSort = profile.SortBy;

                        profile.SortBy = (SortBy)id - 108;

                        if (oldSort != profile.SortBy)
                        {
                            // re-orders the list
                            if (oldSort == SortBy.Newest || oldSort == SortBy.Oldest)
                            {
                                ColUtility.Free(StoreList);

                                StoreList = UltimaStore.GetList(Category);
                            }

                            UltimaStore.SortList(StoreList, profile.SortBy);

                            Page = 0;
                        }

                        Refresh();
                        return;
                    }

                // Cart View
                case 113:
                    {
                        if (profile != null)
                        {
                            profile.Category = StoreCategory.Cart;
                        }

                        Refresh();
                        return;
                    }

                // Search
                case 114:
                    {
                        TextRelay searchTxt = info.GetTextEntry(0);

                        if (searchTxt != null && !String.IsNullOrEmpty(searchTxt.Text))
                        {
                            Search = true;
                            SearchText = searchTxt.Text;
                        }
                        else
                        {
                            User.SendLocalizedMessage(1150315); // That text is unacceptable.
                        }

                        Refresh();
                        return;
                    }

                // Buy
                case 115:
                    {
                        if (UltimaStore.CartCount(User) == 0)
                        {
                            if (profile != null)
                            {
                                profile.Category = StoreCategory.Cart;
                            }

                            Refresh();
                            return;
                        }

                        int total = UltimaStore.GetSubTotal(Cart);

                        if (total <= UltimaStore.GetCurrency(User, true))
                        {
                            SendGump(new ConfirmPurchaseGump(User));
                        }
                        else
                        {
                            SendGump(new NoFundsGump(User));
                        }

                        return;
                    }

                // Next Page
                case 116:
                    {
                        ++Page;

                        Refresh();
                        return;
                    }

                // Previous Page
                case 117:
                    {
                        --Page;

                        Refresh();
                        return;
                    }
            }

            if (id < 2000) // Add To Cart
            {
                Refresh();

                StoreEntry entry = StoreList[id - 1000];

                if (Cart == null || Cart.Count < 10)
                {
                    SendGump(new ConfirmCartGump(User, this, entry));
                    return;
                }

                User.SendLocalizedMessage(1156745); // Your store cart is currently full.
            }
            else if (id < 3000) // Change Amount In Cart
            {
                Refresh();

                StoreEntry entry = UltimaStore.Entries[id - 2000];

                SendGump(new ConfirmCartGump(User, this, entry, Cart != null && Cart.ContainsKey(entry) ? Cart[entry] : 0));
                return;
            }
            else if (id < 4000) // Remove From Cart
            {
                StoreEntry entry = UltimaStore.Entries[id - 3000];

                if (profile != null)
                {
                    profile.RemoveFromCart(entry);
                }

                Refresh();
                return;
            }

            ReleaseHidden(User);
        }
    }

    public class ConfirmCartGump : BaseGump
    {
        public UltimaStoreGump Gump { get; private set; }
        public StoreEntry Entry { get; private set; }
        public int Current { get; private set; }

        public ConfirmCartGump(PlayerMobile pm, UltimaStoreGump gump, StoreEntry entry, int current = 0)
            : base(pm, gump.X + (760 / 2) - 205, gump.Y + (574 / 2) - 100)
        {
            Gump = gump;
            Entry = entry;
            Current = current;

            pm.CloseGump(typeof(ConfirmCartGump));
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 410, 200, 0x9C40);
            AddHtmlLocalized(10, 10, 400, 20, 1114513, "#1077826", 0x7FFF, false, false); // Quantity

            for (int i = 0; i < Entry.Name.Length; i++)
            {
                if (Entry.Name[i].Number > 0)
                {
                    AddHtmlLocalized(10, 60 + (i * 14), 400, 20, 1114513, String.Format("#{0}", Entry.Name[i].Number), 0x6B45, false, false);
                }
                else
                {
                    AddHtml(10, 60 + (i * 14), 400, 20, ColorAndCenter(C16232(0x6B45), Entry.Name[i].String), false, false);
                }
            }

            AddHtmlLocalized(30, 100, 200, 20, 1114514, "#1150152", 0x7FFF, false, false); // Quantity to Buy:

            AddBackground(233, 100, 50, 20, 0x2486);
            AddTextEntry(238, 100, 50, 20, 0, 0, Current > 0 ? Current.ToString() : "", 2);

            AddECHandleInput();

            AddButton(45, 150, 0x9C53, 0x9C5D, 195, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 153, 126, 25, 1114513, "#1156596", 0x7FFF, false, false); // Okay

            AddECHandleInput();
            AddECHandleInput();

            AddButton(240, 150, 0x9C53, 0x9C5D, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(240, 153, 126, 25, 1114513, "#1006045", 0x7FFF, false, false); // Cancel

            AddECHandleInput();
        }

        public override void OnServerClose(NetState owner)
        {
            if (owner.Mobile is PlayerMobile)
            {
                UltimaStoreGump.ReleaseHidden(User);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 195)
            {
                TextRelay amtText = info.GetTextEntry(0);

                if (amtText != null && !String.IsNullOrWhiteSpace(amtText.Text))
                {
                    int amount = Utility.ToInt32(amtText.Text);

                    if (amount > 0)
                    {
                        if (amount <= 10)
                        {
                            UltimaStore.GetProfile(User).SetCartAmount(Entry, amount);
                        }
                        else
                        {
                            User.SendLocalizedMessage(1150315); // That text is unacceptable.
                            //User.SendLocalizedMessage(1156836); // You can't exceed 125 items per purchase. 
                        }

                        Gump.Refresh();
                    }
                }
                else
                {
                    User.SendLocalizedMessage(1150315); // That text is unacceptable.
                }
            }

            UltimaStoreGump.ReleaseHidden(User);
        }
    }

    public class ConfirmPurchaseGump : BaseGump
    {
        public ConfirmPurchaseGump(PlayerMobile pm)
            : base(pm, 150, 150)
        {
            pm.CloseGump(typeof(ConfirmPurchaseGump));
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 410, 200, 0x9C40);
            AddHtmlLocalized(10, 10, 400, 20, 1114513, "#1156750", 0x7FFF, false, false); // Purchase Confirmation

            AddHtmlLocalized(30, 60, 350, 60, 1156749, 0x7FFF, false, false); // Are you sure you want to complete this purchase?

            AddECHandleInput();

            AddButton(45, 150, 0x9C53, 0x9C5D, 195, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 153, 126, 25, 1114513, "#1156596", 0x7FFF, false, false); // Okay

            AddECHandleInput();
            AddECHandleInput();

            AddButton(240, 150, 0x9C53, 0x9C5D, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(240, 153, 126, 25, 1114513, "#1006045", 0x7FFF, false, false); // Cancel

            AddECHandleInput();
        }

        public override void OnServerClose(NetState owner)
        {
            if (owner.Mobile is PlayerMobile)
            {
                UltimaStoreGump.ReleaseHidden(User);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 195)
            {
                UltimaStore.TryPurchase(User);
            }

            UltimaStoreGump.ReleaseHidden(User);
        }
    }

    public class NoFundsGump : BaseGump
    {
        public NoFundsGump(PlayerMobile pm)
            : base(pm, 150, 150)
        {
            pm.CloseGump(typeof(NoFundsGump));
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 410, 200, 0x9C40);
            AddHtmlLocalized(10, 10, 400, 20, 1114513, "#1156747", 0x7FFF, false, false); // Insufficient Funds

            AddHtml(30, 60, 350, 60, Color("#da0000", String.Format("This transaction cannot be completed due to insufficient funds available. Visit your shards website for more information on how to obtain {0}.", Configuration.CurrencyName)), false, false);

            AddECHandleInput();

            AddButton(45, 150, 0x9C53, 0x9C5D, 195, GumpButtonType.Reply, 0);
            AddHtml(45, 153, 126, 25, ColorAndCenter("#FFFFFF", "Information"), false, false); // Information

            AddECHandleInput();
            AddECHandleInput();

            AddButton(240, 150, 0x9C53, 0x9C5D, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(240, 153, 126, 25, 1114513, "#1006045", 0x7FFF, false, false); // Cancel

            AddECHandleInput();
        }

        public override void OnServerClose(NetState owner)
        {
            if (owner.Mobile is PlayerMobile)
            {
                UltimaStoreGump.ReleaseHidden(User);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 195)
            {
                if (!String.IsNullOrEmpty(Configuration.Website))
                {
                    User.LaunchBrowser(Configuration.Website);
                }
                else
                {
                    User.LaunchBrowser("https://uo.com/ultima-store/");
                }
            }

            UltimaStoreGump.ReleaseHidden(User);
        }
    }

    public class PromoCodeGump : BaseGump
    {
        public BaseGump Gump { get; private set; }

        public PromoCodeGump(PlayerMobile pm, BaseGump gump)
            : base(pm, 10, 10)
        {
            Gump = gump;

            pm.CloseGump(typeof(PromoCodeGump));
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 400, 340, 0x9C40);

            AddHtmlLocalized(0, 10, 400, 20, 1114513, "#1062516", 0x7FFF, false, false); // Enter Promotional Code
            AddHtmlLocalized(20, 60, 355, 160, 1062869, C32216(0xFFFF00), false, true); // Enter your promotional code EXACTLY as it was given to you (including dashes). Enter no other text in the box aside from your promotional code.

            AddECHandleInput();

            AddBackground(80, 220, 240, 22, 0x2486);
            AddTextEntry(81, 220, 239, 20, 0, 0, "");

            AddButton(40, 260, 0x9C53, 0x9C5D, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 262, 125, 25, 1114513, "#1156596", 0x7FFF, false, false);

            AddECHandleInput();
            AddECHandleInput();

            AddButton(234, 260, 0x9C53, 0x9C5D, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(234, 262, 126, 25, 1114513, "#1006045", 0x7FFF, false, false);

            AddECHandleInput();
        }

        public override void OnServerClose(NetState owner)
        {
            if (owner.Mobile is PlayerMobile)
            {
                UltimaStoreGump.ReleaseHidden(User);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                TextRelay text = info.GetTextEntry(1);

                if (text != null && !String.IsNullOrEmpty(text.Text))
                {
                    // execute code here
                }
            }
        }
    }

    public class PromoItemGump : BaseGump
    {
        public int Image { get; private set; }
        public StoreEntry Entry { get; private set; }

        public PromoItemGump(PlayerMobile pm, StoreEntry entry, int image)
            : base(pm, 150, 200)
        {
            Image = image;
            Entry = entry;
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 0x9CE1);

            AddECHandleInput();

            AddButton(76, 88, 0x9C4B, 0x9C4B, 1198, GumpButtonType.Reply, 0);

            if (Entry.Tooltip > 0)
            {
                AddTooltip(Entry.Tooltip);
            }
            else
            {
                Item item = UltimaStore.UltimaStoreContainer.FindDisplayItem(Entry.ItemType);

                if (item != null)
                {
                    AddItemProperty(item);
                }
            }

            for (int j = 0; j < Entry.Name.Length; j++)
            {
                if (Entry.Name[j].Number > 0)
                {
                    AddHtmlLocalized(76, 92 + (j * 14) + 4, 183, 25, 1114513, String.Format("#{0}", Entry.Name[j].Number.ToString()), 0x7FFF, false, false);
                }
                else
                {
                    AddHtml(76, 92 + (j * 14) + 4, 183, 25, ColorAndCenter("#FFFFFF", Entry.Name[j].String), false, false);
                }
            }

            AddImage(94, 126, Image);
            AddImage(136, 280, 0x9C56);

            AddLabelCropped(156, 278, 143, 25, 0x9C2, Entry.Cost.ToString());

            AddECHandleInput();
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1198)
            {
                PlayerProfile profile = UltimaStore.GetProfile(User, false);

                if (profile != null)
                {
                    profile.Category = StoreCategory.None;
                }

                UltimaStore.OpenStore(User, Entry);
            }
        }
    }
}
