using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Globalization;
using Server.Network;
using Server.Gumps;
using Server.SkillHandlers;

namespace Server.Engines.VendorSearhing
{
    public class VendorSearchGump : Gump
    {
        public PlayerMobile User { get; set; }
        public SearchCriteria Criteria { get; set; }
        public int ChosenCategory { get; set; }
        public Map SetMap { get; set; }

        public static int LabelColor = 0x00FFFF;
        public static int CriteriaColor = 0xF5DEB3;
        public static int TextColor = 1150;

        public VendorSearchGump(PlayerMobile pm, int cat = -1) : base(10, 10)
        {
            User = pm;

            Criteria = VendorSearch.GetContext(pm);
            ChosenCategory = cat;

            if (Criteria == null)
                Criteria = VendorSearch.AddNewContext(pm);

            AddGumpLayout();
        }

        public void AddGumpLayout(bool nofind = false, bool nocrit = false)
        {
            AddBackground(0, 0, 780, 570, 30546);
            AddBackground(10, 45, 250, 22, 9350);

            AddHtmlLocalized(10, 25, 150, 20, 1154510, C32216(LabelColor), false, false); // Item Name
            AddHtmlLocalized(0, 5, 780, 20, 1154645, "#1154508", C32216(LabelColor), false, false); // Vendor Search Query
            AddHtmlLocalized(520, 25, 150, 20, 1154546, C32216(LabelColor), false, false); // Selected Search Criteria

            AddTextEntry(12, 45, 240, 25, TextColor, 0, Criteria.SearchName);

            for (int i = 0; i < VendorSearch.Categories.Count; i++)
            {
                if (i == 0)
                    AddHtmlLocalized(50, 75 + (i * 22), 215, 20, VendorSearch.Categories[i].Label, String.Format("{0}\t{1}", Criteria.MinPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")), Criteria.MaxPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), C32216(LabelColor), false, false);
                else
                    AddHtmlLocalized(50, 75 + (i * 22), 215, 20, VendorSearch.Categories[i].Label, C32216(LabelColor), false, false);

                AddButton(10, 75 + (i * 22), 30533, 30533, i + 100, GumpButtonType.Reply, 0);
            }

            if (ChosenCategory >= 0 && ChosenCategory < VendorSearch.Categories.Count)
            {
                if (ChosenCategory == 0)
                {
                    AddHtmlLocalized(270, 25, 150, 20, 1154532, C32216(LabelColor), false, false); // Minimum Price
                    AddBackground(270, 45, 250, 22, 9350);
                    AddTextEntry(272, 45, 245, 20, TextColor, 1, Criteria.MinPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

                    AddHtmlLocalized(270, 75, 150, 20, 1154533, C32216(LabelColor), false, false); // Maximum Price
                    AddBackground(270, 95, 250, 22, 9350);
                    AddTextEntry(272, 95, 245, 20, TextColor, 2, Criteria.MaxPrice.ToString("N0", CultureInfo.GetCultureInfo("en-US")));

                    AddButton(270, 125, 4011, 4012, 1, GumpButtonType.Reply, 0);
                }
                else if (ChosenCategory == VendorSearch.Categories.Count - 1)
                {
                    AddHtmlLocalized(270, 25, 150, 20, VendorSearch.Categories[ChosenCategory].Label, C32216(LabelColor), false, false); // Sort Results

                    AddHtmlLocalized(310, 45, 150, 20, 1154696, C32216(LabelColor), false, false); // Price: Low to High
                    AddButton(270, 45, 30533, 30533, 2, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(310, 67, 150, 20, 1154697, C32216(LabelColor), false, false); // Price: High to Low
                    AddButton(270, 67, 30533, 30533, 3, GumpButtonType.Reply, 0);
                }
                else if (ChosenCategory >= 0 && ChosenCategory < VendorSearch.Categories.Count)
                {
                    AddHtmlLocalized(270, 25, 150, 20, VendorSearch.Categories[ChosenCategory].Label, C32216(LabelColor), false, false);

                    for (int index = 0; index < VendorSearch.Categories[ChosenCategory].Objects.Count; index++)
                    {
                        Tuple<object, int> data = VendorSearch.Categories[ChosenCategory].Objects[index];

                        AddHtmlLocalized(310, 45 + (index * 22), 215, 20, data.Item2, C32216(LabelColor), false, false);
                        AddButton(270, 45 + (index * 22), 30533, 30533, 200 + index, GumpButtonType.Reply, 0);

                        if (VendorSearch.HasValue(data.Item1, VendorSearch.Categories[ChosenCategory]))
                        {
                            int value = Criteria.GetValueForDetails(data.Item1);

                            AddBackground(475, 45 + (index * 22), 40, 22, 9350);
                            AddTextEntry(477, 45 + (index * 22), 38, 25, TextColor, 400 + index, value > 0 ? value.ToString() : "");
                        }
                    }
                }
            }

            int y = 45;

            if (!String.IsNullOrEmpty(Criteria.SearchName))
            {
                AddButton(520, 45, 4017, 4019, 7, GumpButtonType.Reply, 0);
                AddTooltip(1154694); // Remove Selected Search Criteria

                AddHtmlLocalized(560, 45, 215, 20, 1154510, C32216(CriteriaColor), false, false);

                y += 22;
            }

            for (int i = 0; i < Criteria.Details.Count; i++)
            {
                AddButton(520, y + (i * 22), 4017, 4019, 300 + i, GumpButtonType.Reply, 0);
                AddTooltip(1154694); // Remove Selected Search Criteria

                AddHtmlLocalized(560, y + (i * 22), 215, 20, Criteria.Details[i].Label, C32216(CriteriaColor), false, false);

                if (Criteria.Details[i].Value > 0)
                {
                    AddHtml(720, y + (i * 22), 60, 20, String.Format("<basefont color=#F5DEB3>{0}", FormatValue(Criteria.Details[i].Attribute, Criteria.Details[i].Value)), false, false);
                }
            }

            if (Criteria.SortBy != SortBy.None)
            {
                AddButton(520, 45 + (Criteria.Details.Count * 22), 4017, 4019, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(560, 45 + (Criteria.Details.Count * 22), 215, 20, Criteria.SortBy == SortBy.LowToHigh ? 1154696 : 1154697, C32216(CriteriaColor), false, false);
            }

            AddHtmlLocalized(600, 518, 130, 20, 1114514, "#1154588", C32216(LabelColor), false, false); // Clear Search Criteria
            AddButton(740, 518, 30533, 30533, 5, GumpButtonType.Reply, 0);

            AddHtmlLocalized(600, 540, 130, 20, 1114514, "#1154641", C32216(LabelColor), false, false); // Search
            AddButton(740, 540, 30534, 30534, 6, GumpButtonType.Reply, 0);

            AddHtmlLocalized(50, 540, 150, 20, 3000091, C32216(LabelColor), false, false); // Cancel
            AddButton(10, 540, 30533, 30535, 0, GumpButtonType.Reply, 0);

            if (nofind)
                AddHtmlLocalized(125, 540, 400, 16, 1154587, C32216(0xFF0000), false, false); // No items matched your search.
            else if (nocrit)
                AddHtmlLocalized(125, 540, 400, 16, 1154586, C32216(0xFF0000), false, false); // Please select some criteria to search for.
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID != 0)
            {
                TextRelay searchname = info.GetTextEntry(0);

                if (searchname != null && !String.IsNullOrEmpty(searchname.Text))
                {
                    string text = searchname.Text.Trim();

                    if (Criteria.SearchName == null || text.ToLower() != Criteria.SearchName.ToLower())
                        Criteria.SearchName = searchname.Text;
                }
            }

            switch (info.ButtonID)
            {
                case 0: break;
                case 1: // Set Min/Max price
                    TextRelay tr1 = info.GetTextEntry(1);
                    TextRelay tr2 = info.GetTextEntry(2);

                    if (tr1 != null && tr1.Text != null)
                    {
                        string text = tr1.Text.Trim();
                        int min = 0;

                        if (int.TryParse(text, out min))
                        {
                            Criteria.MinPrice = min;
                        }
                    }

                    if (tr2 != null && tr2.Text != null)
                    {
                        string text = tr2.Text.Trim();
                        int max = 0;

                        if (int.TryParse(text, out max))
                        {
                            Criteria.MaxPrice = max;
                        }
                    }
                    Refresh();
                    break;
                case 2: // Low to High
                    Criteria.SortBy = SortBy.LowToHigh;
                    Refresh();
                    break;
                case 3: // High to Low
                    Criteria.SortBy = SortBy.HighToLow;
                    Refresh();
                    break;
                case 4: // Nothing, resend gump
                    Criteria.SortBy = SortBy.None;
                    Refresh();
                    break;
                case 5: // Clear Criteria
                    Criteria.Reset();
                    Refresh();
                    break;
                case 6: // Search
                    User.CloseGump(typeof(SearchResultsGump));

                    if (Criteria.IsEmpty)
                    {
                        Refresh(false, true);
                    }
                    else
                    {
                        List<VendorItem> list = VendorSearch.DoSearch(User, Criteria);

                        if (list == null || list.Count == 0)
                            Refresh(true);
                        else
                        {
                            Refresh();
                            User.SendGump(new SearchResultsGump(User, list));
                        }
                    }
                    break;
                case 7: // remove item name
                    Criteria.SearchName = null;
                    Refresh();
                    break;
                default:
                    if (info.ButtonID >= 100 && info.ButtonID < 200)
                    {
                        ChosenCategory = info.ButtonID - 100;
                        Refresh();
                    }
                    else if (info.ButtonID >= 200 && info.ButtonID < 300 && ChosenCategory >= 0 && ChosenCategory < VendorSearch.Categories.Count)
                    {
                        int index = info.ButtonID - 200;

                        SearchCategory category = VendorSearch.Categories[ChosenCategory];
                        Tuple<object, int> data = category.Objects[index];
                        object o = data.Item1;
                        int value = 0;

                        TextRelay valuetext = info.GetTextEntry(index + 400);

                        if (valuetext != null)
                            value = Math.Max(o is AosAttribute && (AosAttribute)o == AosAttribute.CastSpeed ? -1 : 0, Utility.ToInt32(valuetext.Text));

                        Criteria.TryAddDetails(o, data.Item2, value, category.Category);
                        Refresh();
                    }
                    else if (info.ButtonID >= 300 && info.ButtonID - 300 >= 0 && info.ButtonID - 300 < Criteria.Details.Count)
                    {
                        SearchDetail toRemove = Criteria.Details[info.ButtonID - 300];

                        if (toRemove.Category == Category.Equipment)
                            Criteria.SearchType = Layer.Invalid;

                        Criteria.Details.Remove(toRemove);
                        Refresh();
                    }
                    break;
            }
        }

        private string FormatValue(object attribute, int value)
        {
            int mod = Imbuing.GetMod(attribute);

            if (!Imbuing.Table.ContainsKey(mod))
                return value.ToString();

            if (mod == 41)
                return String.Format("-{0}", value.ToString());
            else if (Imbuing.Table[mod].MaxIntensity <= 8 || mod == 21 || mod == 17)
                return value.ToString();

            return String.Format("{0}%", value.ToString());
        }

        public void Refresh(bool nofind = false, bool nocrit = false)
        {
            Entries.Clear();
            Entries.TrimExcess();
            AddGumpLayout(nofind, nocrit);
            User.CloseGump(this.GetType());
            User.SendGump(this, false);
        }

        public static int C16232(int c16)
        {
            c16 &= 0x7FFF;

            int r = (((c16 >> 10) & 0x1F) << 3);
            int g = (((c16 >> 05) & 0x1F) << 3);
            int b = (((c16 >> 00) & 0x1F) << 3);

            return (r << 16) | (g << 8) | (b << 0);
        }

        public static int C16216(int c16)
        {
            return c16 & 0x7FFF;
        }

        public static int C32216(int c32)
        {
            c32 &= 0xFFFFFF;

            int r = (((c32 >> 16) & 0xFF) >> 3);
            int g = (((c32 >> 08) & 0xFF) >> 3);
            int b = (((c32 >> 00) & 0xFF) >> 3);

            return (r << 10) | (g << 5) | (b << 0);
        }

        protected string Color(string color, string str)
        {
            return String.Format("<basefont color={0}>{1}", color, str);
        }

        protected string ColorAndCenter(string color, string str)
        {
            return String.Format("<basefont color={0}><center>{1}</center>", color, str);
        }
    }

    public class SearchResultsGump : Gump
    {
        public int PerPage = 5;
        public int TextColor { get { return VendorSearchGump.C32216(0xF5DEB3); } }

        public PlayerMobile User { get; set; }
        public List<VendorItem> Items { get; set; }
        public int Index { get; set; }

        public SearchResultsGump(PlayerMobile pm, List<VendorItem> items) : base(50, 50)
        {
            User = pm;
            Items = items;
            Index = 0;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddBackground(0, 0, 500, 550, 30536);

            AddHtmlLocalized(0, 40, 500, 20, 1154645, "#1154509", TextColor, false, false); // Vendor Search Results

            AddHtmlLocalized(200, 70, 90, 20, 3000377, TextColor, false, false); // Price
            AddHtmlLocalized(290, 70, 100, 20, 3005088, TextColor, false, false); // Facet
            AddHtmlLocalized(360, 70, 100, 20, 1154642, TextColor, false, false); // Create Map

            if (Index < 0) Index = Items.Count - 1;
            if(Index >= Items.Count) Index = 0;

            int start = Index;
            int index = 0;

            for (int i = start; i < start + PerPage && i < Items.Count; i++)
            {
                VendorItem item = Items[i];
                Rectangle2D bounds = ItemBounds.Table[item.Item.ItemID];
                int y = 100 + (index * 75);

                AddImage(50, y, 2328);
                AddItem(90 - bounds.Width / 2 - bounds.X, (30 - bounds.Height / 2 - bounds.Y) + y, item.Item.ItemID, item.Item.Hue);

                ObjectPropertyList opl = new ObjectPropertyList(item.Item);
                item.Item.GetProperties(opl);

                if (User.NetState != null)
                    User.NetState.Send(opl);

                AddImage(50, y, 2328);
                AddItemProperty(item.Item.Serial);

                AddItem(90 - bounds.Width / 2 - bounds.X, (30 - bounds.Height / 2 - bounds.Y) + y, item.Item.ItemID, item.Item.Hue);

                AddHtml(200, y + 5, 88, 20, String.Format("<basefont color=#F5DEB3>{0}", item.Price == -1 ? "0" : item.FormattedPrice), false, false);
                AddHtml(290, y + 5, 70, 20, String.Format("<basefont color=#F5DEB3>{0}", item.Item.Map.ToString()), false, false);
                AddButton(370, y + 5, 30533, 30533, 100 + i, GumpButtonType.Reply, 0);

                index++;
            }

            if (Index + PerPage < Items.Count)
            {
                AddButton(430, 480, 30533, 30533, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(345, 480, 100, 20, 1044045, VendorSearchGump.C32216(VendorSearchGump.LabelColor), false, false); // NEXT PAGE
            }

            if (Index >= PerPage)
            {
                AddButton(50, 480, 30533, 30533, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(90, 480, 100, 20, 1044044, VendorSearchGump.C32216(VendorSearchGump.LabelColor), false, false); // PREV PAGE
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                default: // Buy Map
                    VendorItem item = Items[info.ButtonID - 100];
                    PlayerVendor vendor = item.Item.RootParentEntity as PlayerVendor;

                    if(vendor != null)
                    {
                        if (_GivenTo == null)
                            _GivenTo = new Dictionary<VendorItem, List<PlayerMobile>>();

                        if (!_GivenTo.ContainsKey(item))
                            _GivenTo[item] = new List<PlayerMobile>();

                        if (!_GivenTo[item].Contains(User))
                        {
                            VendorSearchMap map = new VendorSearchMap(vendor, item.Item);

                            if (User.Backpack == null || !User.Backpack.TryDropItem(User, map, false))
                                map.Delete();
                            else
                            {
                                User.SendLocalizedMessage(1154690); // The vendor map has been placed in your backpack.
                                _GivenTo[item].Add(User);
                            }
                        }
                    }
                    else
                        User.SendLocalizedMessage(1154700); // Item no longer for sale.
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

        public void Refresh()
        {
            Entries.Clear();
            Entries.TrimExcess();
            AddGumpLayout();
            User.CloseGump(this.GetType());
            User.SendGump(this, false);
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

        public static Dictionary<VendorItem, List<PlayerMobile>> _GivenTo;
    }

    public class ConfirmTeleportGump : Gump
    {
        public static int Cost = 1000;

        public VendorSearchMap VendorMap { get; set; }
        public Mobile User { get; set; }

        public ConfirmTeleportGump(VendorSearchMap map, Mobile pm)
            : base(50, 50)
        {
            VendorMap = map;
            User = pm;

            AddBackground(0, 0, 500, 300, 30546);

            if (VendorMap.Vendor != null && VendorMap.SetLocation != Point3D.Zero)
                AddHtmlLocalized(40, 50, 420, 200, 1154637, String.Format("{0}\t{1}", VendorMap.GetCoords(), VendorMap.Vendor.Map.ToString()), 0xFFFF, false, false); // Please select 'Accept' if you would like to return to ~1_loc~ (~2_facet~).  This map will be deleted after use.
            else
                AddHtmlLocalized(40, 50, 420, 200, 1154635, String.Format("{0}\t{1}\t{2}", Cost.ToString(), VendorMap.Vendor.Name, VendorMap.TimeRemaining.ToString()), 0xFFFF, false, false); // Please select 'Accept' if you would like to pay ~1_cost~ gold to teleport to vendor ~2_name~.  For this price you will also be able to teleport back to this location within the next ~3_minutes~ minutes.

            AddButton(40, 260, 30535, 30535, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(90, 260, 150, 20, 1153439, 0xFFFF, false, false);

            AddButton(433, 260, 30534, 30534, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(300, 260, 120, 20, 1114514, "#1073996", 0xFFFF, false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                default: break;
                case 1:
                    if (VendorMap.SetLocation != Point3D.Zero)
                    {
                        User.Frozen = true;

                        DoRecall(VendorMap.SetLocation, VendorMap.SetMap);
                        User.PublicOverheadMessage(MessageType.Spell, User.SpeechHue, true, "Kal Ort Por", false);
                    }
                    else if (!VendorMap.CheckVendor())
                    {
                        User.SendLocalizedMessage(1154643); // That item is no longer for sale.
                    }
                    else if (Banker.Withdraw(User, Cost, true))
                    {
                        User.Frozen = true;

                        VendorMap.SetLocation = User.Location;
                        VendorMap.SetMap = User.Map;

                        User.PublicOverheadMessage(MessageType.Spell, User.SpeechHue, true, "Kal Ort Por", false);
                        DoRecall(VendorMap.GetVendorLocation(), VendorMap.GetVendorMap());
                    }
                    else
                        User.SendLocalizedMessage(1019022); // You do not have enough gold.
                    break;
            }
        }

        private void DoRecall(Point3D loc, Map map)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
            {
                User.Frozen = false;

                if (VendorMap.CheckVendor())
                {
                    User.PlaySound(0x1FC);
                    User.MoveToWorld(loc, map);
                    User.PlaySound(0x1FC);

                    if (loc == VendorMap.SetLocation && !VendorMap.Deleted)
                        VendorMap.Delete();
                }
                else
                    User.SendLocalizedMessage(1154700); // Item no longer for sale.
            });
        }
    }
}