using Server.Commands;
using Server.ContextMenus;
using Server.Engines.Auction;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Server.Engines.VendorSearching
{
    public class VendorSearch
    {
        public static string FilePath = Path.Combine("Saves/Misc", "VendorSearch.bin");
        public static StringList StringList => StringList.Localization;

        public static List<SearchItem> DoSearchAuction(Mobile m, SearchCriteria criteria)
        {
            if (criteria == null || Auction.Auction.Auctions == null || Auction.Auction.Auctions.Count == 0)
            {
                return null;
            }

            List<SearchItem> list = new List<SearchItem>();
            bool excludefel = criteria.Details.FirstOrDefault(d => d.Attribute is Misc && (Misc)d.Attribute == Misc.ExcludeFel) != null;

            foreach (Auction.Auction pv in Auction.Auction.Auctions.Where(pv => pv.AuctionItem != null &&
                                                                                pv.AuctionItem.Map != Map.Internal &&
                                                                               pv.AuctionItem.Map != null &&
                                                                               pv.OnGoing &&
                                                                               (!excludefel || pv.AuctionItem.Map != Map.Felucca)))
            {
                list.Add(new SearchItem(pv.Safe, pv.AuctionItem, (int)pv.Buyout, false));
            }

            switch (criteria.SortBy)
            {
                case SortBy.LowToHigh: list = list.OrderBy(vi => vi.Price).ToList(); break;
                case SortBy.HighToLow: list = list.OrderBy(vi => -vi.Price).ToList(); break;
            }

            return list;
        }

        public static List<SearchItem> DoSearch(Mobile m, SearchCriteria criteria)
        {
            if (criteria == null || PlayerVendor.PlayerVendors == null || PlayerVendor.PlayerVendors.Count == 0)
                return null;

            List<SearchItem> list = new List<SearchItem>();
            bool excludefel = criteria.Details.FirstOrDefault(d => d.Attribute is Misc && (Misc)d.Attribute == Misc.ExcludeFel) != null;

            foreach (PlayerVendor pv in PlayerVendor.PlayerVendors.Where(pv => pv.Map != Map.Internal &&
                                                                               pv.Map != null &&
                                                                               pv.Backpack != null &&
                                                                               pv.VendorSearch &&
                                                                               pv.Backpack.Items.Count > 0 &&
                                                                               (!excludefel || pv.Map != Map.Felucca)))
            {
                List<Item> items = GetItems(pv);

                foreach (Item item in items)
                {
                    VendorItem vendorItem = pv.GetVendorItem(item);
                    int price = 0;
                    bool isChild = false;

                    if (vendorItem != null)
                    {
                        price = vendorItem.Price;
                    }
                    else if (item.Parent is Container)
                    {
                        vendorItem = GetParentVendorItem(pv, (Container)item.Parent);

                        if (vendorItem != null)
                        {
                            isChild = true;
                            price = vendorItem.Price;
                        }
                    }

                    if (price > 0 && CheckMatch(item, price, criteria))
                    {
                        list.Add(new SearchItem(pv, item, price, isChild));
                    }
                }

                ColUtility.Free(items);
            }

            switch (criteria.SortBy)
            {
                case SortBy.LowToHigh: list = list.OrderBy(vi => vi.Price).ToList(); break;
                case SortBy.HighToLow: list = list.OrderBy(vi => -vi.Price).ToList(); break;
            }

            return list;
        }

        private static VendorItem GetParentVendorItem(PlayerVendor pv, Container parent)
        {
            VendorItem vendorItem = pv.GetVendorItem(parent);

            if (vendorItem == null)
            {
                if (parent.Parent is Container)
                {
                    return GetParentVendorItem(pv, (Container)parent.Parent);
                }
            }

            return vendorItem;
        }

        public static bool CheckMatch(Item item, int price, SearchCriteria searchCriteria)
        {
            if (item is CommodityDeed && ((CommodityDeed)item).Commodity != null)
            {
                item = ((CommodityDeed)item).Commodity;
            }

            if (searchCriteria.MinPrice > -1 && price < searchCriteria.MinPrice)
                return false;

            if (searchCriteria.MaxPrice > -1 && price > searchCriteria.MaxPrice)
                return false;

            if (!string.IsNullOrEmpty(searchCriteria.SearchName))
            {
                string name;

                if (item is CommodityDeed && ((CommodityDeed)item).Commodity is ICommodity)
                {
                    ICommodity commodity = (ICommodity)((CommodityDeed)item).Commodity;

                    if (!string.IsNullOrEmpty(commodity.Description.String))
                    {
                        name = commodity.Description.String;
                    }
                    else
                    {
                        name = StringList.GetString(commodity.Description.Number);
                    }
                }
                else
                {
                    name = GetItemName(item);
                }

                if (name == null)
                {
                    return false; // TODO? REturn null names?
                }

                if (!CheckKeyword(searchCriteria.SearchName, item) && name.ToLower().IndexOf(searchCriteria.SearchName.ToLower()) < 0)
                {
                    return false;
                }
            }

            if (searchCriteria.SearchType != Layer.Invalid && searchCriteria.SearchType != item.Layer)
            {
                return false;
            }

            if (searchCriteria.Details.Count == 0)
                return true;

            foreach (SearchDetail detail in searchCriteria.Details)
            {
                object o = detail.Attribute;
                int value = detail.Value;

                if (value == 0)
                    value = 1;

                if (o is AosAttribute)
                {
                    AosAttributes attrs = RunicReforging.GetAosAttributes(item);

                    if (attrs == null || attrs[(AosAttribute)o] < value)
                        return false;
                }
                else if (o is AosWeaponAttribute)
                {
                    AosWeaponAttributes attrs = RunicReforging.GetAosWeaponAttributes(item);

                    if ((AosWeaponAttribute)o == AosWeaponAttribute.MageWeapon)
                    {
                        if (attrs == null || attrs[(AosWeaponAttribute)o] == 0 || attrs[(AosWeaponAttribute)o] > Math.Max(0, 30 - value))
                            return false;
                    }
                    else if (attrs == null || attrs[(AosWeaponAttribute)o] < value)
                        return false;
                }
                else if (o is SAAbsorptionAttribute)
                {
                    SAAbsorptionAttributes attrs = RunicReforging.GetSAAbsorptionAttributes(item);

                    if (attrs == null || attrs[(SAAbsorptionAttribute)o] < value)
                        return false;
                }
                else if (o is AosArmorAttribute)
                {
                    AosArmorAttributes attrs = RunicReforging.GetAosArmorAttributes(item);

                    if (attrs == null || attrs[(AosArmorAttribute)o] < value)
                        return false;
                }
                else if (o is SkillName)
                {
                    if (detail.Category != Category.RequiredSkill)
                    {
                        AosSkillBonuses skillbonuses = RunicReforging.GetAosSkillBonuses(item);

                        if (skillbonuses != null)
                        {
                            bool hasSkill = false;

                            for (int i = 0; i < 5; i++)
                            {
                                SkillName check;
                                double bonus;

                                if (skillbonuses.GetValues(i, out check, out bonus) && check == (SkillName)o && bonus >= value)
                                {
                                    hasSkill = true;
                                    break;
                                }
                            }

                            if (!hasSkill)
                                return false;
                        }
                        else if (item is SpecialScroll && value >= 105)
                        {
                            if (((SpecialScroll)item).Skill != (SkillName)o || ((SpecialScroll)item).Value < value)
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (!(item is BaseWeapon) || ((BaseWeapon)item).DefSkill != (SkillName)o)
                    {
                        return false;
                    }
                }
                else if (!CheckSlayer(item, o))
                {
                    return false;
                }
                else if (o is AosElementAttribute)
                {
                    if (item is BaseWeapon)
                    {
                        BaseWeapon wep = item as BaseWeapon;

                        if (detail.Category == Category.DamageType)
                        {
                            int phys, fire, cold, pois, nrgy, chaos, direct;
                            wep.GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

                            switch ((AosElementAttribute)o)
                            {
                                case AosElementAttribute.Physical: if (phys < value) return false; break;
                                case AosElementAttribute.Fire: if (fire < value) return false; break;
                                case AosElementAttribute.Cold: if (cold < value) return false; break;
                                case AosElementAttribute.Poison: if (pois < value) return false; break;
                                case AosElementAttribute.Energy: if (nrgy < value) return false; break;
                                case AosElementAttribute.Chaos: if (chaos < value) return false; break;
                                case AosElementAttribute.Direct: if (direct < value) return false; break;
                            }
                        }
                        else
                        {
                            switch ((AosElementAttribute)o)
                            {
                                case AosElementAttribute.Physical:
                                    if (wep.WeaponAttributes.ResistPhysicalBonus < value) return false;
                                    break;
                                case AosElementAttribute.Fire:
                                    if (wep.WeaponAttributes.ResistFireBonus < value) return false;
                                    break;
                                case AosElementAttribute.Cold:
                                    if (wep.WeaponAttributes.ResistColdBonus < value) return false;
                                    break;
                                case AosElementAttribute.Poison:
                                    if (wep.WeaponAttributes.ResistPoisonBonus < value) return false;
                                    break;
                                case AosElementAttribute.Energy:
                                    if (wep.WeaponAttributes.ResistEnergyBonus < value) return false;
                                    break;
                            }
                        }
                    }
                    else if (item is BaseArmor && detail.Category == Category.Resists)
                    {
                        BaseArmor armor = item as BaseArmor;

                        switch ((AosElementAttribute)o)
                        {
                            case AosElementAttribute.Physical:
                                if (armor.PhysicalResistance < value) return false;
                                break;
                            case AosElementAttribute.Fire:
                                if (armor.FireResistance < value) return false;
                                break;
                            case AosElementAttribute.Cold:
                                if (armor.ColdResistance < value) return false;
                                break;
                            case AosElementAttribute.Poison:
                                if (armor.PoisonResistance < value) return false;
                                break;
                            case AosElementAttribute.Energy:
                                if (armor.EnergyResistance < value) return false;
                                break;
                        }
                    }
                    else if (detail.Category != Category.DamageType)
                    {
                        AosElementAttributes attrs = RunicReforging.GetElementalAttributes(item);

                        if (attrs == null || attrs[(AosElementAttribute)o] < value)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (o is Misc)
                {
                    switch ((Misc)o)
                    {
                        case Misc.ExcludeFel: break;
                        case Misc.GargoyleOnly:
                            if (!IsGargoyle(item))
                                return false;
                            break;
                        case Misc.NotGargoyleOnly:
                            if (IsGargoyle(item))
                                return false;
                            break;
                        case Misc.ElvesOnly:
                            if (!IsElf(item))
                                return false;
                            break;
                        case Misc.NotElvesOnly:
                            if (IsElf(item))
                                return false;
                            break;
                        case Misc.FactionItem:
                            return false;
                        case Misc.PromotionalToken:
                            if (!(item is PromotionalToken))
                                return false;
                            break;
                        case Misc.Cursed:
                            if (item.LootType != LootType.Cursed)
                                return false;
                            break;
                        case Misc.NotCursed:
                            if (item.LootType == LootType.Cursed)
                                return false;
                            break;
                        case Misc.CannotRepair:
                            if (CheckCanRepair(item))
                                return false;
                            break;
                        case Misc.NotCannotBeRepaired:
                            if (!CheckCanRepair(item))
                                return false;
                            break;
                        case Misc.Brittle:
                            NegativeAttributes neg2 = RunicReforging.GetNegativeAttributes(item);
                            if (neg2 == null || neg2.Brittle == 0)
                                return false;
                            break;
                        case Misc.NotBrittle:
                            NegativeAttributes neg3 = RunicReforging.GetNegativeAttributes(item);
                            if (neg3 != null && neg3.Brittle > 0)
                                return false;
                            break;
                        case Misc.Antique:
                            NegativeAttributes neg4 = RunicReforging.GetNegativeAttributes(item);
                            if (neg4 == null || neg4.Antique == 0)
                                return false;
                            break;
                        case Misc.NotAntique:
                            NegativeAttributes neg5 = RunicReforging.GetNegativeAttributes(item);
                            if (neg5 != null && neg5.Antique > 0)
                                return false;
                            break;
                    }
                }
                else if (o is string)
                {
                    string str = o as string;

                    if (str == "WeaponVelocity" && (!(item is BaseRanged) || ((BaseRanged)item).Velocity < value))
                        return false;

                    if (str == "SearingWeapon" && (!(item is BaseWeapon) || !((BaseWeapon)item).SearingWeapon))
                        return false;

                    if (str == "ArtifactRarity" && (!(item is IArtifact) || ((IArtifact)item).ArtifactRarity < value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool CheckSlayer(Item item, object o)
        {
            if (o is TalismanSlayerName && (TalismanSlayerName)o == TalismanSlayerName.Undead)
            {
                if (!(item is ISlayer) || ((((ISlayer)item).Slayer != SlayerName.Silver && ((ISlayer)item).Slayer2 != SlayerName.Silver)))
                {
                    if (!(item is BaseTalisman) || ((BaseTalisman)item).Slayer != TalismanSlayerName.Undead)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (o is SlayerName && (!(item is ISlayer) || (((ISlayer)item).Slayer != (SlayerName)o && ((ISlayer)item).Slayer2 != (SlayerName)o)))
                {
                    return false;
                }

                if (o is TalismanSlayerName && (!(item is BaseTalisman) || ((BaseTalisman)item).Slayer != (TalismanSlayerName)o))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckCanRepair(Item item)
        {
            NegativeAttributes neg = RunicReforging.GetNegativeAttributes(item);

            return neg != null && neg.NoRepair != 0;
        }

        private static bool CheckKeyword(string searchstring, Item item)
        {
            if (item is CommodityDeed && ((CommodityDeed)item).Commodity != null)
            {
                item = ((CommodityDeed)item).Commodity;
            }

            if (item is IResource)
            {
                string resName = CraftResources.GetName(((IResource)item).Resource);

                if (resName.ToLower().IndexOf(searchstring.ToLower()) >= 0)
                {
                    return true;
                }
            }

            if (item is ICommodity)
            {
                ICommodity commodity = (ICommodity)item;

                string name = commodity.Description.String;

                if (string.IsNullOrEmpty(name) && commodity.Description.Number > 0)
                {
                    name = StringList.GetString(commodity.Description.Number);
                }

                if (!string.IsNullOrEmpty(name) && name.ToLower().IndexOf(searchstring.ToLower()) >= 0)
                {
                    return true;
                }
            }

            return Keywords.ContainsKey(searchstring.ToLower()) && Keywords[searchstring.ToLower()] == item.GetType();
        }

        public static bool IsGargoyle(Item item)
        {
            return Race.Gargoyle.ValidateEquipment(item);
        }

        public static bool IsElf(Item item)
        {
            return Race.Elf.ValidateEquipment(item);
        }

        public static SearchCriteria AddNewContext(PlayerMobile pm)
        {
            SearchCriteria criteria = new SearchCriteria();

            Contexts[pm] = criteria;

            return criteria;
        }

        public static SearchCriteria GetContext(PlayerMobile pm)
        {
            if (Contexts.ContainsKey(pm))
                return Contexts[pm];

            return null;
        }

        public static Dictionary<PlayerMobile, SearchCriteria> Contexts { get; set; }
        public static List<SearchCategory> Categories { get; set; }

        public static Dictionary<string, Type> Keywords { get; set; }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(Contexts == null ? 0 : Contexts.Count(kvp => !kvp.Value.IsEmpty));

                    if (Contexts != null)
                    {
                        foreach (KeyValuePair<PlayerMobile, SearchCriteria> kvp in Contexts.Where(kvp => !kvp.Value.IsEmpty))
                        {
                            writer.Write(kvp.Key);
                            kvp.Value.Serialize(writer);
                        }
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        PlayerMobile pm = reader.ReadMobile() as PlayerMobile;
                        SearchCriteria criteria = new SearchCriteria(reader);

                        if (pm != null)
                        {
                            if (Contexts == null)
                                Contexts = new Dictionary<PlayerMobile, SearchCriteria>();

                            Contexts[pm] = criteria;
                        }
                    }
                });
        }

        public static void Initialize()
        {
            CommandSystem.Register("GetOPLString", AccessLevel.Administrator, e =>
                {
                    e.Mobile.BeginTarget(-1, false, TargetFlags.None, (m, targeted) =>
                        {
                            if (targeted is Item)
                            {
                                Console.WriteLine(GetItemName((Item)targeted));
                                e.Mobile.SendMessage(GetItemName((Item)targeted));
                            }
                        });
                });

            Categories = new List<SearchCategory>();

            if (Contexts == null)
            {
                Contexts = new Dictionary<PlayerMobile, SearchCriteria>();
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    SearchCategory price = new SearchCategory(Category.PriceRange);
                    Categories.Add(price);

                    SearchCriteriaCategory.AllCategories.ToList().ForEach(x =>
                    {
                        SearchCategory cat = new SearchCategory(x.Category);

                        x.Criteria.ToList().ForEach(y =>
                        {
                            if (y.PropCliloc != 0)
                            {
                                cat.Register(y.Object, y.Cliloc, y.PropCliloc);
                            }
                            else
                            {
                                cat.Register(y.Object, y.Cliloc);
                            }

                        });

                        Categories.Add(cat);
                    });

                    SearchCategory sort = new SearchCategory(Category.Sort);
                    Categories.Add(sort);
                });

            Keywords = new Dictionary<string, Type>();

            Keywords["power scroll"] = typeof(PowerScroll);
            Keywords["stat scroll"] = typeof(StatCapScroll);
        }

        public static string GetItemName(Item item)
        {
            if (StringList == null || item.Name != null)
                return item.Name;

            ObjectPropertyList opl = new ObjectPropertyList(item);
            item.GetProperties(opl);

            //since the object property list is based on a packet object, the property info is packed away in a packet format
            byte[] data = opl.UnderlyingStream.UnderlyingStream.ToArray();

            int index = 15; // First localization number index
            string basestring = null;

            //reset the number property
            uint number = 0;

            //if there's not enough room for another record, quit
            if (index + 4 >= data.Length)
            {
                return null;
            }

            //read number property from the packet data
            number = (uint)(data[index++] << 24 | data[index++] << 16 | data[index++] << 8 | data[index++]);

            //reset the length property
            ushort length = 0;

            //if there's not enough room for another record, quit
            if (index + 2 > data.Length)
            {
                return null;
            }

            //read length property from the packet data
            length = (ushort)(data[index++] << 8 | data[index++]);

            //determine the location of the end of the string
            int end = index + length;

            //truncate if necessary
            if (end >= data.Length)
            {
                end = data.Length - 1;
            }

            //read the string into a StringBuilder object

            StringBuilder s = new StringBuilder();
            while (index + 2 <= end + 1)
            {
                short next = (short)(data[index++] | data[index++] << 8);

                if (next == 0)
                    break;

                s.Append(Encoding.Unicode.GetString(BitConverter.GetBytes(next)));
            }

            basestring = StringList.GetString((int)number);
            string args = s.ToString();

            if (args == string.Empty)
            {
                return basestring;
            }

            string[] parms = args.Split('\t');

            try
            {
                if (parms.Length > 1)
                {
                    for (int i = 0; i < parms.Length; i++)
                    {
                        parms[i] = parms[i].Trim(' ');

                        if (parms[i].IndexOf("#") == 0)
                        {
                            parms[i] = StringList.GetString(Convert.ToInt32(parms[i].Substring(1, parms[i].Length - 1)));
                        }
                    }
                }
                else if (parms.Length == 1 && parms[0].IndexOf("#") == 0)
                {
                    parms[0] = StringList.GetString(Convert.ToInt32(args.Substring(1, parms[0].Length - 1)));
                }
            }
            catch
            {
                return null;
            }

            StringEntry entry = StringList.GetEntry((int)number);

            if (entry != null)
            {
                return entry.Format(parms);
            }

            return basestring;
        }

        private static List<Item> GetItems(PlayerVendor pv)
        {
            List<Item> list = new List<Item>();

            foreach (Item item in pv.Items)
                if (item.Movable && item != pv.Backpack && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair)
                    list.Add(item);

            if (pv.Backpack != null)
            {
                GetItems(pv.Backpack, list);
            }

            return list;
        }

        public static void GetItems(Container c, List<Item> list)
        {
            if (c == null || c.Items.Count == 0)
                return;

            foreach (Item item in c.Items)
            {
                if (item is Container && !IsSearchableContainer(item.GetType()))
                    GetItems((Container)item, list);
                else
                    list.Add(item);
            }
        }

        public static bool CanSearch(Mobile m)
        {
            Region r = m.Region;

            if (r.GetLogoutDelay(m) == TimeSpan.Zero)
                return true;

            return r is GuardedRegion && !((GuardedRegion)r).Disabled || r is HouseRegion && ((HouseRegion)r).House.IsFriend(m);
        }

        private static bool IsSearchableContainer(Type type)
        {
            return _SearchableContainers.Any(t => t == type || type.IsSubclassOf(t));
        }

        private static readonly Type[] _SearchableContainers =
        {
            typeof(BaseQuiver),         typeof(BaseResourceSatchel),
            typeof(FishBowl),           typeof(FirstAidBelt),
            typeof(Plants.SeedBox),     typeof(BaseSpecialScrollBook),
            typeof(GardenShedBarrel),   typeof(JewelryBox),
        };
    }

    public enum SortBy
    {
        LowToHigh,
        HighToLow
    }

    public enum Category
    {
        PriceRange,
        Misc,
        Equipment,
        Combat,
        Casting,
        DamageType,
        HitSpell,
        HitArea,
        Resists,
        Stats,
        Slayer1,
        Slayer2,
        Slayer3,
        RequiredSkill,
        Skill1,
        Skill2,
        Skill3,
        Skill4,
        Skill5,
        Skill6,
        Sort,
        Auction
    }

    public enum Misc
    {
        ExcludeFel,
        GargoyleOnly,
        NotGargoyleOnly,
        ElvesOnly,
        NotElvesOnly,
        FactionItem,
        PromotionalToken,
        Cursed,
        NotCursed,
        CannotRepair,
        NotCannotBeRepaired,
        Brittle,
        NotBrittle,
        Antique,
        NotAntique
    }

    public enum SpecialSearch
    {
        PowerScroll,
        StatScroll,
        Transcendence,
        Alacrity,
        UsesRemaining
    }

    public class SearchCategory
    {
        public Category Category { get; }
        public int Label => (int)Category;

        public List<Tuple<object, int, int>> Objects { get; }

        public SearchCategory(Category category)
        {
            Category = category;

            Objects = new List<Tuple<object, int, int>>();
        }

        public void Register(object o, int label)
        {
            if (Objects.FirstOrDefault(t => t.Item1 == o) == null)
            {
                Objects.Add(new Tuple<object, int, int>(o, label, 0));
            }
        }

        public void Register(object o, int label, int pcliloc)
        {
            if (Objects.FirstOrDefault(t => t.Item1 == o) == null)
            {
                Objects.Add(new Tuple<object, int, int>(o, label, pcliloc));
            }
        }
    }

    public class SearchCriteria
    {
        public Layer SearchType { get; set; }
        public string SearchName { get; set; }
        public SortBy SortBy { get; set; }
        public bool Auction { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }

        public bool EntryPrice { get; set; }

        public List<SearchDetail> Details { get; set; }

        public SearchCriteria()
        {
            Details = new List<SearchDetail>();

            MinPrice = 0;
            MaxPrice = 175000000;
            SearchType = Layer.Invalid;
        }

        public void Reset()
        {
            Details.Clear();
            Details.TrimExcess();
            Details = new List<SearchDetail>();

            MinPrice = 0;
            MaxPrice = 175000000;
            SortBy = SortBy.LowToHigh;
            Auction = false;
            SearchName = null;
            SearchType = Layer.Invalid;
            EntryPrice = false;
        }

        public int GetValueForDetails(object o)
        {
            SearchDetail detail = Details.FirstOrDefault(d => d.Attribute == o);

            return detail != null ? detail.Value : 0;
        }

        public void TryAddDetails(object o, int name, int propname, int value, Category cat)
        {
            SearchDetail d = Details.FirstOrDefault(det => det.Attribute == o);

            if (o is Layer)
            {
                SearchDetail layer = Details.FirstOrDefault(det => det.Attribute is Layer && (Layer)det.Attribute != (Layer)o);

                if (layer != null)
                {
                    Details.Remove(layer);
                }

                Details.Add(new SearchDetail(o, name, propname, value, cat));
                SearchType = (Layer)o;
            }
            else if (d == null)
            {
                d = new SearchDetail(o, name, propname, value, cat);

                Details.Add(d);
            }
            else if (d.Value != value)
            {
                d.Value = value;
            }

            /*if (d.Attribute is TalismanSlayerName && (TalismanSlayerName)d.Attribute == TalismanSlayerName.Undead)
            {
                TryAddDetails(SlayerName.Silver, name, value, cat);
            }*/
        }

        public bool IsEmpty => Details.Count == 0 && !EntryPrice && string.IsNullOrEmpty(SearchName) && SearchType == Layer.Invalid;

        public SearchCriteria(GenericReader reader)
        {
            int version = reader.ReadInt();

            Details = new List<SearchDetail>();

            if (version > 1)
                Auction = reader.ReadBool();

            if (version != 0)
                EntryPrice = reader.ReadBool();

            SearchType = (Layer)reader.ReadInt();
            SearchName = reader.ReadString();
            SortBy = (SortBy)reader.ReadInt();
            MinPrice = reader.ReadLong();
            MaxPrice = reader.ReadLong();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Details.Add(new SearchDetail(reader));
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(2);

            writer.Write(Auction);
            writer.Write(EntryPrice);
            writer.Write((int)SearchType);
            writer.Write(SearchName);
            writer.Write((int)SortBy);
            writer.Write(MinPrice);
            writer.Write(MaxPrice);

            writer.Write(Details.Count);

            for (int i = 0; i < Details.Count; i++)
            {
                Details[i].Serialize(writer);
            }
        }
    }

    public class SearchDetail
    {
        public enum AttributeID
        {
            None = 0,
            AosAttribute,
            AosArmorAttribute,
            AosWeaponAttribute,
            AosElementAttribute,
            SkillName,
            SAAbosorptionAttribute,
            ExtendedWeaponAttribute,
            NegativeAttribute,
            SlayerName,
            String,
            TalismanSlayerName,
            TalismanSkill,
            TalismanRemoval,
            Int,
        }

        public object Attribute { get; set; }
        public int Label { get; }
        public int PropLabel { get; }
        public int Value { get; set; }
        public Category Category { get; }

        public SearchDetail(object o, int label, int proplabel, int value, Category category)
        {
            Attribute = o;
            Label = label;
            PropLabel = proplabel;
            Value = value;
            Category = category;
        }

        public SearchDetail(GenericReader reader)
        {
            int version = reader.ReadInt(); // version

            if (version > 0)
            {
                PropLabel = reader.ReadInt();
            }

            ReadAttribute(reader);

            Label = reader.ReadInt();
            Value = reader.ReadInt();
            Category = (Category)reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(PropLabel);

            WriteAttribute(writer);

            writer.Write(Label);
            writer.Write(Value);
            writer.Write((int)Category);
        }

        private void WriteAttribute(GenericWriter writer)
        {
            int attrID = GetAttributeID(Attribute);
            writer.Write(attrID);

            switch (attrID)
            {
                case 0: break;
                case 1: writer.Write((int)(AosAttribute)Attribute); break;
                case 2: writer.Write((int)(AosArmorAttribute)Attribute); break;
                case 3: writer.Write((int)(AosWeaponAttribute)Attribute); break;
                case 4: writer.Write((int)(AosElementAttribute)Attribute); break;
                case 5: writer.Write((int)(SkillName)Attribute); break;
                case 6: writer.Write((int)(SAAbsorptionAttribute)Attribute); break;
                case 7: writer.Write((int)(ExtendedWeaponAttribute)Attribute); break;
                case 8: writer.Write((int)(NegativeAttribute)Attribute); break;
                case 9: writer.Write((int)(SlayerName)Attribute); break;
                case 10: writer.Write((string)Attribute); break;
                case 11: writer.Write((int)(TalismanSlayerName)Attribute); break;
                case 12: writer.Write((int)(TalismanSkill)Attribute); break;
                case 13: writer.Write((int)(TalismanRemoval)Attribute); break;
                case 14: writer.Write((int)Attribute); break;
            }
        }

        private void ReadAttribute(GenericReader reader)
        {
            switch (reader.ReadInt())
            {
                case 0: break;
                case 1: Attribute = (AosAttribute)reader.ReadInt(); break;
                case 2: Attribute = (AosArmorAttribute)reader.ReadInt(); break;
                case 3: Attribute = (AosWeaponAttribute)reader.ReadInt(); break;
                case 4: Attribute = (AosElementAttribute)reader.ReadInt(); break;
                case 5: Attribute = (SkillName)reader.ReadInt(); break;
                case 6: Attribute = (SAAbsorptionAttribute)reader.ReadInt(); break;
                case 7: Attribute = (ExtendedWeaponAttribute)reader.ReadInt(); break;
                case 8: Attribute = (NegativeAttribute)reader.ReadInt(); break;
                case 9: Attribute = (SlayerName)reader.ReadInt(); break;
                case 10: Attribute = reader.ReadString(); break;
                case 11: Attribute = (TalismanSlayerName)reader.ReadInt(); break;
                case 12: Attribute = (TalismanSkill)reader.ReadInt(); break;
                case 13: Attribute = (TalismanRemoval)reader.ReadInt(); break;
                case 14: Attribute = reader.ReadInt(); break;
            }
        }

        public static int GetAttributeID(object o)
        {
            if (o is AosAttribute)
                return (int)AttributeID.AosAttribute;

            if (o is AosArmorAttribute)
                return (int)AttributeID.AosArmorAttribute;

            if (o is AosWeaponAttribute)
                return (int)AttributeID.AosWeaponAttribute;

            if (o is AosElementAttribute)
                return (int)AttributeID.AosElementAttribute;

            if (o is SkillName)
                return (int)AttributeID.SkillName;

            if (o is SAAbsorptionAttribute)
                return (int)AttributeID.SAAbosorptionAttribute;

            if (o is ExtendedWeaponAttribute)
                return (int)AttributeID.ExtendedWeaponAttribute;

            if (o is NegativeAttribute)
                return (int)AttributeID.NegativeAttribute;

            if (o is SlayerName)
                return (int)AttributeID.SlayerName;

            if (o is TalismanSlayerName)
                return (int)AttributeID.TalismanSlayerName;

            if (o is string)
                return (int)AttributeID.String;

            if (o is TalismanSkill)
                return (int)AttributeID.TalismanSkill;

            if (o is TalismanRemoval)
                return (int)AttributeID.TalismanRemoval;

            if (o is int)
                return (int)AttributeID.Int;

            return (int)AttributeID.None;
        }

    }

    public class SearchVendors : ContextMenuEntry
    {
        public PlayerMobile Player { get; }

        public SearchVendors(PlayerMobile pm)
            : base(1154679, -1)
        {
            Player = pm;

            Enabled = VendorSearch.CanSearch(pm);
        }

        public override void OnClick()
        {
            if (VendorSearch.CanSearch(Player))
            {
                BaseGump.SendGump(new VendorSearchGump(Player));
            }
        }
    }

    public class SearchItem
    {
        public PlayerVendor Vendor { get; }
        public IAuctionItem AuctionSafe { get; }
        public Item Item { get; }
        public int Price { get; }
        public bool IsChild { get; }
        public bool IsAuction { get; }

        public Map Map => Vendor != null ? Vendor.Map : AuctionSafe != null ? AuctionSafe.Map : null;

        public SearchItem(PlayerVendor vendor, Item item, int price, bool isChild)
        {
            Vendor = vendor;
            Item = item;
            Price = price;
            IsChild = isChild;
            IsAuction = false;
        }

        public SearchItem(IAuctionItem auctionsafe, Item item, int price, bool isChild)
        {
            AuctionSafe = auctionsafe;
            Item = item;
            Price = price;
            IsChild = isChild;
            IsAuction = true;
        }
    }
}
