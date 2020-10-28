using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Engines.VendorSearching
{
    public class VendorSearchGump : BaseGump
    {
        public SearchCriteria Criteria { get; }
        public Map SetMap { get; set; }

        public int Feedback { get; }

        public static int LabelColor = 0x4BBD;
        public static int CriteriaColor = 0x6B55;
        public static int TextColor = 0x9C2;
        public static int AlertColor = 0x7C00;

        public VendorSearchGump(PlayerMobile pm, int feedback = -1)
            : base(pm, 10, 10)
        {
            TypeID = 0xF3EC8;
            Feedback = feedback;
            Criteria = VendorSearch.GetContext(pm);

            if (Criteria == null)
                Criteria = VendorSearch.AddNewContext(pm);
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 780, 600, 30546);

            AddHtmlLocalized(10, 10, 760, 18, 1114513, "#1154508", LabelColor, false, false); // Vendor Search Query
            AddHtmlLocalized(522, 30, 246, 18, 1154546, LabelColor, false, false); // Selected Search Criteria

            int yOffset = 0;

            if (!string.IsNullOrEmpty(Criteria.SearchName))
            {
                AddButton(522, 50 + (yOffset * 22), 4017, 4019, 7, GumpButtonType.Reply, 0);
                AddTooltip(1154694); // Remove Selected Search Criteria
                AddHtmlLocalized(562, 50 + (yOffset * 22), 206, 20, 1154510, CriteriaColor, false, false);
                yOffset++;
            }

            if (Criteria.EntryPrice)
            {
                AddButton(522, 50 + (yOffset * 22), 4017, 4019, 8, GumpButtonType.Reply, 0);
                AddTooltip(1154694); // Remove Selected Search Criteria
                AddHtmlLocalized(562, 50 + (yOffset * 22), 206, 20, 1154512, string.Format("@{0}@{1}", Criteria.MinPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Criteria.MaxPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), CriteriaColor, false, false);
                yOffset++;
            }

            for (int i = 0; i < Criteria.Details.Count; i++)
            {
                int cliloc = Criteria.Details[i].PropLabel;

                if (cliloc > 0)
                {
                    if (Criteria.Details[i].Attribute is SkillName)
                    {
                        AddHtmlLocalized(562, 50 + (yOffset * 22), 206, 20, 1060451, string.Format("#{0}@{1}", cliloc, Criteria.Details[i].Value), CriteriaColor, false, false);
                    }
                    else
                    {
                        AddHtmlLocalized(562, 50 + (yOffset * 22), 206, 20, cliloc, string.Format("{0}", Criteria.Details[i].Value), CriteriaColor, false, false);
                    }
                }
                else
                {
                    AddHtmlLocalized(562, 50 + (yOffset * 22), 206, 20, Criteria.Details[i].Label, CriteriaColor, false, false);
                }

                AddButton(522, 50 + (yOffset * 22), 4017, 4019, 1001 + i, GumpButtonType.Reply, 0);
                AddTooltip(1154694); // Remove Selected Search Criteria

                yOffset++;
            }

            AddButton(522, 50 + (yOffset * 22), 4017, 4019, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(562, 50 + (yOffset * 22), 215, 20, Criteria.SortBy == SortBy.LowToHigh ? 1154696 : 1154697, CriteriaColor, false, false);

            yOffset++;

            AddButton(522, 50 + (yOffset * 22), 4017, 4019, 9, GumpButtonType.Reply, 0);
            AddTooltip(1154694); // Remove Selected Search Criteria
            AddHtmlLocalized(562, 50 + (yOffset * 22), 206, 20, Criteria.Auction ? 1159353 : 1159354, CriteriaColor, false, false);

            AddHtmlLocalized(10, 30, 246, 18, 1154510, LabelColor, false, false); // Item Name
            AddBackground(10, 50, 246, 22, 9350);
            AddTextEntry(12, 52, 242, 18, TextColor, 1, Criteria.SearchName, 25);

            yOffset = 0;

            SearchCriteriaCategory.AllCategories.OrderByDescending(x => x.PageID == 2).ThenByDescending(x => x.PageID == 6).ToList().ForEach(x =>
            {
                AddButton(10, 74 + (yOffset * 22), 30533, 30533, 0, GumpButtonType.Page, x.PageID);

                if (x.Category == Category.PriceRange)
                    AddHtmlLocalized(50, 75 + (yOffset * 22), 215, 20, x.Cliloc, string.Format("@{0}@{1}", Criteria.MinPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Criteria.MaxPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), LabelColor, false, false);
                else
                    AddHtmlLocalized(50, 75 + (yOffset * 22), 215, 20, x.Cliloc, LabelColor, false, false);

                yOffset++;
            });

            AddButton(10, 570, 0x7747, 0x7747, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 570, 50, 20, 1150300, LabelColor, false, false); // Cancel

            if (Feedback != -1)
            {
                AddHtmlLocalized(110, 570, 660, 20, Feedback, AlertColor, false, false);
            }

            AddButton(740, 570, 30534, 30534, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(630, 570, 100, 20, 1114514, "#1154641", LabelColor, false, false); // Search

            AddButton(740, 550, 30533, 30533, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(630, 550, 100, 20, 1114514, "#1154588", LabelColor, false, false); // Clear Search Criteria

            int buttonIdx = 50;

            SearchCriteriaCategory.AllCategories.ToList().ForEach(x =>
            {
                AddPage(x.PageID);

                if (x.Category == Category.PriceRange)
                {
                    AddHtmlLocalized(266, 30, 246, 18, 1154532, LabelColor, false, false); // Minimum Price
                    AddBackground(266, 50, 246, 22, 9350);
                    AddTextEntry(268, 52, 242, 18, TextColor, 7, Criteria.MinPrice.ToString(), 10);

                    AddHtmlLocalized(266, 74, 246, 18, 1154533, LabelColor, false, false); // Maximum Price
                    AddBackground(266, 94, 246, 22, 9350);
                    AddTextEntry(268, 96, 242, 18, TextColor, 8, Criteria.MaxPrice.ToString(), 10);

                    AddButton(266, 118, 4011, 4012, 1154512, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(306, 118, 100, 272, 1154534, LabelColor, false, false); // Add Search Criteria
                }
                else if (x.Category == Category.Sort)
                {
                    AddHtmlLocalized(266, 30, 246, 18, x.Cliloc, LabelColor, false, false); // Sort Results

                    AddHtmlLocalized(306, 50, 215, 20, 1154696, LabelColor, false, false); // Price: Low to High
                    AddButton(266, 50, 30533, 30533, 236, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(306, 72, 215, 20, 1154697, LabelColor, false, false); // Price: High to Low
                    AddButton(266, 72, 30533, 30533, 237, GumpButtonType.Reply, 0);
                }
                else if (x.Category == Category.Auction)
                {
                    AddHtmlLocalized(266, 30, 246, 18, x.Cliloc, LabelColor, false, false); // Auction Item

                    AddHtmlLocalized(306, 50, 215, 20, 1159354, LabelColor, false, false); // Non Auction Item
                    AddButton(266, 50, 30533, 30533, 238, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(306, 72, 215, 20, 1159353, LabelColor, false, false); // Auction Item
                    AddButton(266, 72, 30533, 30533, 239, GumpButtonType.Reply, 0);
                }
                else
                {
                    AddHtmlLocalized(266, 30, 246, 18, x.Cliloc, LabelColor, false, false);

                    yOffset = 0;

                    x.Criteria.ToList().ForEach(y =>
                    {
                        AddHtmlLocalized(306, 50 + (yOffset * 22), 215, 20, y.Cliloc, LabelColor, false, false);
                        AddButton(266, 50 + (yOffset * 22), 30533, 30533, buttonIdx, GumpButtonType.Reply, 0);

                        if (y.PropCliloc != 0)
                        {
                            int value = Criteria.GetValueForDetails(y.Object);

                            AddBackground(482, 50 + (yOffset * 22), 30, 20, 9350);
                            AddTextEntry(484, 50 + (yOffset * 22), 26, 16, TextColor, buttonIdx - 40, value > 0 ? value.ToString() : "", 3);
                        }

                        yOffset++;
                        buttonIdx++;
                    });
                }
            });
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID != 0)
            {
                if (!VendorSearch.CanSearch(User))
                {
                    User.SendLocalizedMessage(1154680); //Before using vendor search, you must be in a justice region or a safe log-out location (such as an inn or a house which has you on its Owner, Co-owner, or Friends list). 
                    return;
                }

                TextRelay searchname = info.GetTextEntry(1);

                if (searchname != null && !string.IsNullOrEmpty(searchname.Text))
                {
                    string text = searchname.Text.Trim();

                    if (Criteria.SearchName == null || text.ToLower() != Criteria.SearchName.ToLower())
                        Criteria.SearchName = searchname.Text;
                }
            }

            switch (info.ButtonID)
            {
                case 0: break;
                case 1: // Search
                    {
                        User.CloseGump(typeof(SearchResultsGump));

                        if (Criteria.IsEmpty)
                        {
                            SendGump(new VendorSearchGump(User, 1154586)); // Please select some criteria to search for.
                        }
                        else
                        {
                            Task<List<SearchItem>> resultsTask = FindVendorItemsAsync(User, Criteria);

                            TaskPollingTimer<List<SearchItem>> pollingTimer = new TaskPollingTimer<List<SearchItem>>(resultsTask, (results) =>
                            {
                                User.CloseGump(typeof(SearchWaitGump));

                                if (results == null || results.Count == 0)
                                {
                                    SendGump(new VendorSearchGump(User, 1154587)); // No items matched your search.                                     
                                }
                                else
                                {
                                    Refresh();
                                    SendGump(new SearchResultsGump(User, results));
                                }
                            });

                            resultsTask.Start();
                            pollingTimer.Start();

                            SendGump(new SearchWaitGump(User, pollingTimer));
                        }
                        break;
                    }
                case 2: // Clear Criteria
                    {
                        Criteria.Reset();
                        Refresh();
                        break;
                    }
                case 4: // Nothing, resend gump                    
                    Refresh();
                    break;
                case 7: // remove item name
                    Criteria.SearchName = null;
                    Refresh();
                    break;
                case 8: // remove price entry
                    Criteria.EntryPrice = false;
                    Refresh();
                    break;
                case 9: // remove auction entry
                    Refresh();
                    break;
                case 236: // Low to High
                    Criteria.SortBy = SortBy.LowToHigh;
                    Refresh();
                    break;
                case 237: // High to Low
                    Criteria.SortBy = SortBy.HighToLow;
                    Refresh();
                    break;
                case 238: // Non Auction Item
                    Criteria.Auction = false;
                    Refresh();
                    break;
                case 239: // Auction Item
                    Criteria.Auction = true;
                    Refresh();
                    break;
                case 1154512: // Set Min/Max price
                    TextRelay tr1 = info.GetTextEntry(7);
                    TextRelay tr2 = info.GetTextEntry(8);

                    if (tr1 != null && tr1.Text != null)
                    {
                        string text = tr1.Text.Trim();

                        if (int.TryParse(text, out int min))
                        {
                            Criteria.MinPrice = min;
                        }
                    }

                    if (tr2 != null && tr2.Text != null)
                    {
                        string text = tr2.Text.Trim();

                        if (int.TryParse(text, out int max))
                        {
                            Criteria.MaxPrice = max;
                        }
                    }

                    Criteria.EntryPrice = true;
                    Refresh();
                    break;
                default:
                    if (info.ButtonID > 1000)
                    {
                        SearchDetail toRemove = Criteria.Details[info.ButtonID - 1001];

                        if (toRemove.Category == Category.Equipment)
                            Criteria.SearchType = Layer.Invalid;

                        Criteria.Details.Remove(toRemove);
                        Refresh();
                    }
                    else
                    {
                        if (Criteria.Details.Count >= 20)
                        {
                            SendGump(new VendorSearchGump(User, 1154681)); // You may not add any more search criteria items.
                        }

                        var criteria = SearchCriteriaCategory.AllCategories.SelectMany(x => x.Criteria, (x, c) => new { x.Category, c.Object, c.Cliloc, c.PropCliloc }).ToList()[info.ButtonID - 50];
                        object o = criteria.Object;
                        int value = 0;

                        TextRelay valuetext = info.GetTextEntry(info.ButtonID - 40);

                        if (valuetext != null)
                            value = Math.Max(o is AosAttribute && (AosAttribute)o == AosAttribute.CastSpeed ? -1 : 0, Utility.ToInt32(valuetext.Text));

                        Criteria.TryAddDetails(o, criteria.Cliloc, criteria.PropCliloc, value, criteria.Category);
                        Refresh();
                    }
                    break;
            }
        }

        public Task<List<SearchItem>> FindVendorItemsAsync(Mobile m, SearchCriteria criteria)
        {
            return new Task<List<SearchItem>>(() =>
            {
                return criteria.Auction ? VendorSearch.DoSearchAuction(m, criteria) : VendorSearch.DoSearch(m, criteria);
            });
        }
    }

    public class SearchWaitGump : BaseGump
    {
        private readonly Timer m_PollingTimer;

        public SearchWaitGump(PlayerMobile pm, Timer waitTimer)
            : base(pm, 10, 10)
        {
            m_PollingTimer = waitTimer;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 414, 214, 0x7752);

            AddHtmlLocalized(27, 47, 380, 80, 1114513, "#1154678", 0x4E73, false, false); // <DIV ALIGN=CENTER>Please wait for your search to complete.</DIV>
        }

        public override void OnResponse(RelayInfo info)
        {
            m_PollingTimer.Stop();
        }
    }

    public class SearchResultsGump : BaseGump
    {
        public int PerPage = 5;
        public int LabelColor => 0x4BBD;
        public int TextColor => 0x6B55;

        public List<SearchItem> Items { get; }
        public int Index { get; set; }

        public SearchResultsGump(PlayerMobile pm, List<SearchItem> items)
            : base(pm, 30, 30)
        {
            Items = items;
            Index = 0;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 500, 550, 30536);

            AddHtmlLocalized(50, 50, 400, 18, 1114513, "#1154509", LabelColor, false, false); // Vendor Search Results

            AddHtmlLocalized(162, 70, 102, 18, 1114513, "#1062218", LabelColor, false, false); // Price
            AddHtmlLocalized(274, 70, 61, 18, 1114513, "#1154644", LabelColor, false, false); // Facet
            AddHtmlLocalized(345, 70, 102, 18, 1114513, "#1154642", LabelColor, false, false); // Create Map

            if (Index < 0) Index = Items.Count - 1;
            if (Index >= Items.Count) Index = 0;

            int start = Index;
            int index = 0;

            for (int i = start; i < start + PerPage && i < Items.Count; i++)
            {
                var item = Items[i].Item;
                var price = Items[i].Price;
                var map = Items[i].Map;

                Rectangle2D bounds = ItemBounds.Table[item.ItemID];
                int y = 101 + (index * 75);

                if (map == null && item.RootParentEntity is Mobile)
                    map = ((Mobile)item.RootParentEntity).Map;

                AddImageTiledButton(50, y, 0x918, 0x918, 0x0, GumpButtonType.Page, 0, item.ItemID, item.Hue, 40 - bounds.Width / 2 - bounds.X, 30 - bounds.Height / 2 - bounds.Y);
                AddItemProperty(item);

                if (Items[i].IsAuction)
                    AddHtmlLocalized(162, y, 102, 72, 1159353, 0x6B55, false, false); // Auction Item
                else
                    AddHtmlLocalized(162, y, 102, 72, Items[i].IsChild ? 1154598 : 1154645, string.Format("{0}", price <= 0 ? "0" : FormatPrice(price)), TextColor, false, false); // <center>~1_val~</center>

                if (map != null)
                    AddHtmlLocalized(274, y, 102, 72, 1114513, string.Format("{0}", map), TextColor, false, false);

                AddButton(386, y, 30533, 30533, 100 + i, GumpButtonType.Reply, 0);

                index++;
            }

            if (Index + PerPage < Items.Count)
            {
                AddButton(430, 480, 30533, 30533, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(355, 480, 70, 20, 1114514, "#1044045", LabelColor, false, false); // NEXT PAGE
            }

            if (Index >= PerPage)
            {
                AddButton(50, 480, 30533, 30533, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 480, 255, 20, 1044044, "#1044044", LabelColor, false, false); // PREV PAGE
            }
        }

        private string FormatPrice(int price)
        {
            return price.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                default: // Buy Map
                    SearchItem item = Items[info.ButtonID - 100];

                    if (item != null && (item.AuctionSafe != null && item.AuctionSafe.CheckAuctionItem(item.Item) || item.Vendor != null && item.Vendor.GetVendorItem(item.Item) != null))
                    {
                        if (_GivenTo == null)
                            _GivenTo = new Dictionary<Item, List<PlayerMobile>>();

                        if (!_GivenTo.ContainsKey(item.Item))
                            _GivenTo[item.Item] = new List<PlayerMobile>();

                        if (!_GivenTo[item.Item].Contains(User))
                        {
                            VendorSearchMap map = new VendorSearchMap(item);

                            if (User.Backpack == null || !User.Backpack.TryDropItem(User, map, false))
                                map.Delete();
                            else
                            {
                                User.SendLocalizedMessage(1154690); // The vendor map has been placed in your backpack.
                                _GivenTo[item.Item].Add(User);
                            }
                        }
                    }
                    else
                    {
                        User.SendLocalizedMessage(1154643); // That item is no longer for sale.
                    }
                    break;
                case 2: // Next Page
                    Index += PerPage;
                    Refresh();
                    break;
                case 3: // Prev Page
                    Index -= PerPage;
                    Refresh();
                    break;
            }
        }

        public static void Initialize()
        {
            Timer t = Timer.DelayCall(TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30), () =>
            {
                if (_GivenTo != null)
                {
                    _GivenTo.Clear();
                    _GivenTo = null;
                }
            });

            t.Priority = TimerPriority.OneMinute;
        }

        public static Dictionary<Item, List<PlayerMobile>> _GivenTo;
    }

    public class ConfirmTeleportGump : BaseGump
    {
        public VendorSearchMap VendorMap { get; }

        public ConfirmTeleportGump(VendorSearchMap map, PlayerMobile pm)
            : base(pm, 10, 10)
        {
            VendorMap = map;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 414, 214, 0x7752);

            if (VendorMap.SetLocation != Point3D.Zero && (VendorMap.Vendor != null || VendorMap.AuctionSafe != null))
            {
                string[] coord = VendorMap.GetCoords();
                AddHtmlLocalized(27, 47, 380, 80, 1154637, string.Format("@{0}@{1}", coord[0], coord[1]), 0x4E73, false, false); // Please select 'Accept' if you would like to return to ~1_loc~ (~2_facet~).  This map will be deleted after use.
            }
            else
                AddHtmlLocalized(27, 47, 380, 80, 1154635, string.Format("@{0}@{1}@{2}", VendorMap.TeleportCost.ToString(), VendorMap.Name()[0], VendorMap.DeleteDelayMinutes.ToString()), 0x4E73, false, false); // Please select 'Accept' if you would like to pay ~1_cost~ gold to teleport to vendor ~2_name~.  For this price you will also be able to teleport back to this location within the next ~3_minutes~ minutes.

            AddButton(7, 167, 0x7747, 0x7747, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(47, 167, 100, 40, 1150300, 0x4E73, false, false); // CANCEL

            AddButton(377, 167, 0x7746, 0x7746, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(267, 167, 100, 40, 1114514, "#1150299", 0x4E73, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                default: break;
                case 1:
                    {
                        if (Banker.GetBalance(User) < VendorMap.TeleportCost)
                        {
                            User.SendLocalizedMessage(1154672); // You cannot afford to teleport to the vendor.
                        }
                        else if (VendorMap.SetLocation == Point3D.Zero && !VendorSearch.CanSearch(User))
                        {
                            User.SendLocalizedMessage(1154680); //Before using vendor search, you must be in a justice region or a safe log-out location (such as an inn or a house which has you on its Owner, Co-owner, or Friends list). 
                        }
                        else if (VendorMap.SetLocation == Point3D.Zero && !VendorSearch.CanSearch(User))
                        {
                            User.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                        }
                        else
                        {
                            new Spells.Fourth.RecallSpell(User, VendorMap, VendorMap).Cast();
                        }

                        break;
                    }
            }
        }
    }
}
