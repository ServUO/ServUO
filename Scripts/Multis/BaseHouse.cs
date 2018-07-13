using System;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;
using Server.ContextMenus;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis.Deeds;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using Server.Engines.Auction;
using System.Linq;

namespace Server.Multis
{
    public abstract class BaseHouse : BaseMulti
    {
		private static int m_AccountHouseLimit = Config.Get("Housing.AccountHouseLimit", 1);
        public static int AccountHouseLimit { get { return m_AccountHouseLimit; } }

        public static bool NewVendorSystem
        {
            get
            {
                return Core.AOS;
            }
        }// Is new player vendor system enabled?

        public const int MaxCoOwners = 15;
        public static int MaxFriends
        {
            get
            {
                return !Core.AOS ? 50 : 140;
            }
        }
        public static int MaxBans
        {
            get
            {
                return !Core.AOS ? 50 : 140;
            }
        }

        #region Dynamic decay system
        private DecayLevel m_CurrentStage;
        private DateTime m_NextDecayStage;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextDecayStage
        {
            get
            {
                return m_NextDecayStage;
            }
            set
            {
                m_NextDecayStage = value;
            }
        }

        public void ResetDynamicDecay()
        {
            m_CurrentStage = DecayLevel.Ageless;
            m_NextDecayStage = DateTime.MinValue;
        }

        public void SetDynamicDecay(DecayLevel level)
        {
            m_CurrentStage = level;

            if (DynamicDecay.Decays(level))
                m_NextDecayStage = DateTime.UtcNow + DynamicDecay.GetRandomDuration(level);
            else
                m_NextDecayStage = DateTime.MinValue;
        }

        #endregion

        public const bool DecayEnabled = true;

        public static void Decay_OnTick()
        {
            for (int i = 0; i < m_AllHouses.Count; ++i)
                m_AllHouses[i].CheckDecay();
        }

        private DateTime m_LastRefreshed;
        private bool m_RestrictDecay;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastRefreshed
        {
            get
            {
                return m_LastRefreshed;
            }
            set
            {
                m_LastRefreshed = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RestrictDecay
        {
            get
            {
                return m_RestrictDecay;
            }
            set
            {
                m_RestrictDecay = value;
            }
        }

        public virtual TimeSpan DecayPeriod
        {
            get
            {
                return TimeSpan.FromDays(5.0);
            }
        }

        public virtual DecayType DecayType
        {
            get
            {
                if (m_RestrictDecay || !DecayEnabled || DecayPeriod == TimeSpan.Zero)
                    return DecayType.Ageless;

                if (m_Owner == null)
                    return Core.AOS ? DecayType.Condemned : DecayType.ManualRefresh;

                Account acct = m_Owner.Account as Account;

                if (acct == null)
                    return Core.AOS ? DecayType.Condemned : DecayType.ManualRefresh;

                if (acct.AccessLevel >= AccessLevel.GameMaster)
                    return DecayType.Ageless;

                for (int i = 0; i < acct.Length; ++i)
                {
                    Mobile mob = acct[i];

                    if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster)
                        return DecayType.Ageless;
                }

                if (!Core.AOS)
                    return DecayType.ManualRefresh;

                if (acct.Inactive)
                    return DecayType.Condemned;

                List<BaseHouse> allHouses = new List<BaseHouse>();

                for (int i = 0; i < acct.Length; ++i)
                {
                    Mobile mob = acct[i];

                    if (mob != null)
                        allHouses.AddRange(GetHouses(mob));
                }

                BaseHouse newest = null;

                for (int i = 0; i < allHouses.Count; ++i)
                {
                    BaseHouse check = allHouses[i];

                    if (newest == null || IsNewer(check, newest))
                        newest = check;
                }

                if (this == newest)
                    return DecayType.AutoRefresh;

                return DecayType.ManualRefresh;
            }
        }

        public bool IsNewer(BaseHouse check, BaseHouse house)
        {
            DateTime checkTime = (check.LastTraded > check.BuiltOn ? check.LastTraded : check.BuiltOn);
            DateTime houseTime = (house.LastTraded > house.BuiltOn ? house.LastTraded : house.BuiltOn);

            return (checkTime > houseTime);
        }

        public virtual bool CanDecay
        {
            get
            {
                DecayType type = DecayType;

                return (type == DecayType.Condemned || type == DecayType.ManualRefresh);
            }
        }

        private DecayLevel m_LastDecayLevel;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual DecayLevel DecayLevel
        {
            get
            {
                DecayLevel result;

                if (!CanDecay)
                {
                    if (DynamicDecay.Enabled)
                        ResetDynamicDecay();

                    m_LastRefreshed = DateTime.UtcNow;
                    result = DecayLevel.Ageless;
                }
                else if (DynamicDecay.Enabled)
                {
                    DecayLevel stage = m_CurrentStage;

                    if (stage == DecayLevel.Ageless || (DynamicDecay.Decays(stage) && m_NextDecayStage <= DateTime.UtcNow))
                        SetDynamicDecay(++stage);

                    if (stage == DecayLevel.Collapsed && (HasRentedVendors || VendorInventories.Count > 0))
                        result = DecayLevel.DemolitionPending;
                    else
                        result = stage;
                }
                else
                {
                    result = GetOldDecayLevel();
                }

                if (result != m_LastDecayLevel)
                {
                    m_LastDecayLevel = result;

                    if (m_Sign != null && !m_Sign.GettingProperties)
                        m_Sign.InvalidateProperties();
                }

                return result;
            }
        }

        public DecayLevel GetOldDecayLevel()
        {
            TimeSpan timeAfterRefresh = DateTime.UtcNow - m_LastRefreshed;
            int percent = (int)((timeAfterRefresh.Ticks * 1000) / DecayPeriod.Ticks);

            if (percent >= 1000) // 100.0%
                return (HasRentedVendors || VendorInventories.Count > 0) ? DecayLevel.DemolitionPending : DecayLevel.Collapsed;
            else if (percent >= 950) // 95.0% - 99.9%
                return DecayLevel.IDOC;
            else if (percent >= 750) // 75.0% - 94.9%
                return DecayLevel.Greatly;
            else if (percent >= 500) // 50.0% - 74.9%
                return DecayLevel.Fairly;
            else if (percent >= 250) // 25.0% - 49.9%
                return DecayLevel.Somewhat;
            else if (percent >= 005) // 00.5% - 24.9%
                return DecayLevel.Slightly;

            return DecayLevel.LikeNew;
        }

        public virtual bool RefreshDecay()
        {
            if (DecayType == DecayType.Condemned)
                return false;

            DecayLevel oldLevel = DecayLevel;

            m_LastRefreshed = DateTime.UtcNow;

            if (DynamicDecay.Enabled)
                ResetDynamicDecay();

            if (m_Sign != null)
                m_Sign.InvalidateProperties();

            return (oldLevel > DecayLevel.LikeNew);
        }

        public virtual bool CheckDecay()
        {
            if (!Deleted && DecayLevel == DecayLevel.Collapsed)
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Decay_Sandbox));
                return true;
            }

            return false;
        }

        public virtual void KillVendors()
        {
            List<Mobile> list = new List<Mobile>(PlayerVendors);

            foreach (PlayerVendor vendor in list)
                vendor.Destroy(true);

            list = new List<Mobile>(PlayerBarkeepers);

            foreach (PlayerBarkeeper barkeeper in list)
                barkeeper.Delete();
        }

        public virtual void Decay_Sandbox()
        {
            if (Deleted)
                return;

            if (Core.ML)
                new TempNoHousingRegion(this, null);

            if (Core.SA)
            {
                Rectangle3D[] recs = m_Region.Area;
                Map map = Map;

                Timer.DelayCall(TimeSpan.FromMilliseconds(250), () => OnAfterDecay(recs, map));
            }

            KillVendors();
            Delete();
        }

        public virtual void OnAfterDecay(Rectangle3D[] recs, Map map)
        {
            if (map != null && recs.Length > 0)
            {
                int count = Utility.RandomMinMax(1, 4);

                for (int i = 0; i < count; i++)
                {
                    var rec3D = recs[Utility.Random(recs.Length)];
                    var rec2D = new Rectangle2D(rec3D.Start, rec3D.End);

                    IPooledEnumerable eable = map.GetItemsInBounds(rec2D);
                    var list = new List<Item>();

                    foreach (Item item in eable)
                    {
                        if (item.RootParent == null && item.Movable && item.LootType != LootType.Blessed)
                        {
                            list.Add(item);
                        }
                    }

                    if (list.Count > 0)
                    {
                        Item item = list[Utility.Random(list.Count)];

                        if (item != null)
                        {
                            var grubber = new Grubber();
                            grubber.MoveToWorld(item.Location, item.Map);

                            grubber.PackItem(item);
                        }
                    }

                    eable.Free();
                    ColUtility.Free(list);
                }
            }
        }

        public virtual TimeSpan RestrictedPlacingTime
        {
            get
            {
                return TimeSpan.FromHours(1.0);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double BonusStorageScalar
        {
            get
            {
                return (Core.ML ? Core.SA ? 1.4 : 1.2 : 1.0);
            }
        }

        private bool m_Public;

        private HouseRegion m_Region;
        private HouseSign m_Sign;
        private TrashBarrel m_Trash;
        private List<Item> m_Doors;

        private Mobile m_Owner;

        private List<Mobile> m_Access;
        private List<Mobile> m_Bans;
        private List<Mobile> m_CoOwners;
        private List<Mobile> m_Friends;

        private readonly List<Mobile> m_PlayerVendors = new List<Mobile>();
        private readonly List<Mobile> m_PlayerBarkeepers = new List<Mobile>();

        private Dictionary<Item, Mobile> m_LockDowns;
        private List<Item> m_VendorRentalContracts;
        private List<SecureInfo> m_Secures;

        private Dictionary<Item, Mobile> m_Addons;
        private List<Item> m_Carpets;

        private readonly List<VendorInventory> m_VendorInventories = new List<VendorInventory>();
        private readonly List<RelocatedEntity> m_RelocatedEntities = new List<RelocatedEntity>();

        private MovingCrate m_MovingCrate;
        private List<Mobile> m_InternalizedVendors;

        private int m_MaxLockDowns;
        private int m_MaxSecures;
        private int m_Price;

        private Dictionary<Mobile, DateTime> m_Visits;

        private DateTime m_BuiltOn, m_LastTraded;

        private Point3D m_RelativeBanLocation;

        private static readonly Dictionary<Mobile, List<BaseHouse>> m_Table = new Dictionary<Mobile, List<BaseHouse>>();

        public virtual bool IsAosRules
        {
            get
            {
                return Core.AOS;
            }
        }

        public virtual bool IsActive
        {
            get
            {
                return true;
            }
        }

        public virtual HousePlacementEntry GetAosEntry()
        {
            return HousePlacementEntry.Find(this);
        }

        public virtual int GetAosMaxSecures()
        {
            HousePlacementEntry hpe = GetAosEntry();

            if (hpe == null)
                return 0;

            return (int)(hpe.Storage * BonusStorageScalar);
        }

        public virtual int GetAosMaxLockdowns()
        {
            HousePlacementEntry hpe = GetAosEntry();

            if (hpe == null)
                return 0;

            return (int)(hpe.Lockdowns * BonusStorageScalar);
        }

        private Type[] _NoItemCountTable = new Type[]
        {
            typeof(Engines.Plants.SeedBox),       typeof(GardenShedAddon),
            typeof(GardenShedBarrel)
        };

        // Not Included Storage
        public virtual bool CheckStorage(Item item)
        {
            Type type = item.GetType();

            bool contains = false;

            for (int i = 0; !contains && i < _NoItemCountTable.Length; ++i)
            {
                contains = (type == _NoItemCountTable[i]);
            }

            return contains;
        }

        public virtual int GetAosCurSecures(out int fromSecures, out int fromVendors, out int fromLockdowns, out int fromMovingCrate)
        {
            /* Secured container, container counts as fromLockdowns, items count as fromSecures
             * Locked Down Container, container and items count as fromLockdowns */

            fromSecures = 0;
            fromVendors = 0;
            fromLockdowns = 0;
            fromMovingCrate = 0;

            List<SecureInfo> list = m_Secures;

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    SecureInfo si = (SecureInfo)list[i];

                    if (!CheckStorage(si.Item) && !m_LockDowns.ContainsKey(si.Item))
                    {
                        fromSecures += si.Item.TotalItems;
                    }
                }

                fromLockdowns += list.Where(x => !m_LockDowns.ContainsKey(x.Item)).Count();
            }

            fromLockdowns += GetLockdowns();

            if (!NewVendorSystem)
            {
                foreach (PlayerVendor vendor in PlayerVendors)
                {
                    if (vendor.Backpack != null)
                    {
                        fromVendors += vendor.Backpack.TotalItems;
                    }
                }
            }

            if (MovingCrate != null)
            {
                fromMovingCrate += MovingCrate.TotalItems;

                foreach (Item item in MovingCrate.Items)
                {
                    if (item is PackingBox)
                        fromMovingCrate--;
                }
            }

            return fromSecures + fromVendors + fromLockdowns + fromMovingCrate;
        }

        public bool InRange(IPoint2D from, int range)
        {
            if (Region == null)
                return false;

            foreach (Rectangle3D rect in Region.Area)
            {
                if (from.X >= rect.Start.X - range && from.Y >= rect.Start.Y - range && from.X < rect.End.X + range && from.Y < rect.End.Y + range)
                    return true;
            }

            return false;
        }

        public virtual int GetNewVendorSystemMaxVendors()
        {
            HousePlacementEntry hpe = GetAosEntry();

            if (hpe == null)
                return 0;

            return (int)(hpe.Vendors * BonusStorageScalar);
        }

        public virtual bool CanPlaceNewVendor()
        {
            if (!IsAosRules)
                return true;

            if (!NewVendorSystem)
                return CheckAosLockdowns(10);

            return ((PlayerVendors.Count + VendorRentalContracts.Count) < GetNewVendorSystemMaxVendors());
        }

        public const int MaximumBarkeepCount = 2;

        public virtual bool CanPlaceNewBarkeep()
        {
            return (PlayerBarkeepers.Count < MaximumBarkeepCount);
        }

        public static void IsThereVendor(Point3D location, Map map, out bool vendor, out bool rentalContract)
        {
            vendor = false;
            rentalContract = false;

            IPooledEnumerable eable = map.GetObjectsInRange(location, 0);

            foreach (IEntity entity in eable)
            {
                if (Math.Abs(location.Z - entity.Z) <= 16)
                {
                    if (entity is PlayerVendor || entity is PlayerBarkeeper || entity is PlayerVendorPlaceholder)
                    {
                        vendor = true;
                        break;
                    }

                    if (entity is VendorRentalContract)
                    {
                        rentalContract = true;
                        break;
                    }
                }
            }

            eable.Free();
        }

        public bool HasPersonalVendors
        {
            get
            {
                foreach (PlayerVendor vendor in PlayerVendors)
                {
                    if (!(vendor is RentedVendor))
                        return true;
                }

                return false;
            }
        }

        public bool HasRentedVendors
        {
            get
            {
                foreach (PlayerVendor vendor in PlayerVendors)
                {
                    if (vendor is RentedVendor)
                        return true;
                }

                return false;
            }
        }

        #region Mondain's Legacy
        public bool HasAddonContainers
        {
            get
            {
                foreach (Item item in Addons.Keys)
                {
                    if (item is BaseAddonContainer || item is RaisedGardenAddon || item is WallSafe)
                        return true;
                }

                return false;
            }
        }
        #endregion

        #region TOL
        public bool HasActiveAuction
        {
            get
            {
                foreach (Item item in Addons.Keys)
                {
                    if (item is AuctionSafe && ((AuctionSafe)item).Auction != null && ((AuctionSafe)item).Auction.OnGoing)
                        return true;
                }

                return false;
            }
        }
        #endregion
        
        public List<Mobile> AvailableVendorsFor(Mobile m)
        {
            List<Mobile> list = new List<Mobile>();

            foreach (PlayerVendor vendor in PlayerVendors)
            {
                if (vendor.CanInteractWith(m, false))
                    list.Add(vendor);
            }

            return list;
        }

        public bool AreThereAvailableVendorsFor(Mobile m)
        {
            foreach (PlayerVendor vendor in PlayerVendors)
            {
                if (vendor.CanInteractWith(m, false))
                    return true;
            }

            return false;
        }

        public void MoveAllToCrate()
        {
            RelocatedEntities.Clear();

            if (MovingCrate != null)
                MovingCrate.Hide();

            if (m_Trash != null)
            {
                m_Trash.Delete();
                m_Trash = null;
            }

            foreach (Item item in LockDowns.Keys)
            {
                if (!item.Deleted)
                {
                    item.IsLockedDown = false;
                    item.IsSecure = false;
                    item.Movable = true;

                    if (item.Parent == null)
                        DropToMovingCrate(item);
                }
            }

            LockDowns.Clear();

            foreach (Item item in VendorRentalContracts)
            {
                if (!item.Deleted)
                {
                    item.IsLockedDown = false;
                    item.IsSecure = false;
                    item.Movable = true;

                    if (item.Parent == null)
                        DropToMovingCrate(item);
                }
            }

            VendorRentalContracts.Clear();

            foreach (SecureInfo info in Secures)
            {
                Item item = info.Item;

                if (!item.Deleted)
                {
                    if (item is StrongBox)
                        item = ((StrongBox)item).ConvertToStandardContainer();

                    item.IsLockedDown = false;
                    item.IsSecure = false;
                    item.Movable = true;

                    if (item.Parent == null)
                        DropToMovingCrate(item);
                }
            }

            Secures.Clear();

            foreach (Item addon in Addons.Keys)
            {
                if (!addon.Deleted)
                {
                    Item deed = null;
                    bool retainDeedHue = false;	//if the items aren't hued but the deed itself is
                    int hue = 0;

                    if (addon is IAddon)
                    {
                        deed = ((IAddon)addon).Deed;

                        if (addon is BaseAddon && ((BaseAddon)addon).RetainDeedHue)	//There are things that are IAddon which aren't BaseAddon
                        {
                            BaseAddon ba = (BaseAddon)addon;
                            retainDeedHue = true;

                            for (int i = 0; hue == 0 && i < ba.Components.Count; ++i)
                            {
                                AddonComponent c = ba.Components[i];

                                if (c.Hue != 0)
                                    hue = c.Hue;
                            }
                        }
                    }

                    if (deed != null)
                    {
                        #region Mondain's Legacy
                        if (deed is BaseAddonContainerDeed && addon is BaseAddonContainer)
                        {
                            BaseAddonContainer c = (BaseAddonContainer)addon;
                            c.DropItemsToGround();

                            ((BaseAddonContainerDeed)deed).Resource = c.Resource;
                        }
                        else if (deed is BaseAddonDeed && addon is BaseAddon)
                            ((BaseAddonDeed)deed).Resource = ((BaseAddon)addon).Resource;
                        #endregion

                        addon.Delete();

                        if (retainDeedHue)
                            deed.Hue = hue;

                        DropToMovingCrate(deed);
                    }
                    else
                    {
                        DropToMovingCrate(addon);
                    }
                }
            }

            Addons.Clear();

            foreach (PlayerVendor mobile in PlayerVendors)
            {
                mobile.Return();
                mobile.Internalize();
                InternalizedVendors.Add(mobile);
            }

            foreach (Mobile mobile in PlayerBarkeepers)
            {
                mobile.Internalize();
                InternalizedVendors.Add(mobile);
            }
        }

        public Dictionary<IEntity, Mobile> GetHouseEntities()
        {
            Dictionary<IEntity, Mobile> list = new Dictionary<IEntity, Mobile>();

            if (MovingCrate != null)
                MovingCrate.Hide();

            if (m_Trash != null && m_Trash.Map != Map.Internal)
                list.Add(m_Trash, Owner);

            foreach (Item item in LockDowns.Keys)
            {
                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item, LockDowns[item]);
            }

            foreach (Item item in VendorRentalContracts)
            {
                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item, Owner);
            }

            foreach (SecureInfo info in Secures.Where(i => !LockDowns.ContainsKey(i.Item)))
            {
                Item item = info.Item;

                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item, Owner);
            }

            foreach (Item item in Addons.Keys)
            {
                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item, Owner);
            }

            foreach (PlayerVendor mobile in PlayerVendors)
            {
                mobile.Return();

                if (mobile.Map != Map.Internal)
                    list.Add(mobile, Owner);
            }

            foreach (Mobile mobile in PlayerBarkeepers)
            {
                if (mobile.Map != Map.Internal)
                    list.Add(mobile, Owner);
            }

            return list;
        }

        public void RelocateEntities()
        {
            Dictionary<IEntity, Mobile> entities = GetHouseEntities();

            foreach (IEntity entity in entities.Keys)
            {
                Point3D relLoc = new Point3D(entity.X - X, entity.Y - Y, entity.Z - Z);
                RelocatedEntity relocEntity = new RelocatedEntity(entity, relLoc, entities[entity]);

                RelocatedEntities.Add(relocEntity);

                if (entity is Item)
                    ((Item)entity).Internalize();
                else
                    ((Mobile)entity).Internalize();
            }
        }

        public void RestoreRelocatedEntities()
        {
            foreach (RelocatedEntity relocEntity in RelocatedEntities)
            {
                Point3D relLoc = relocEntity.RelativeLocation;
                Point3D location = new Point3D(relLoc.X + X, relLoc.Y + Y, relLoc.Z + Z);

                IEntity entity = relocEntity.Entity;

                if (entity is Item)
                {
                    Item item = (Item)entity;

                    if (!item.Deleted)
                    {
                        if (item is IAddon)
                        {
                            if (((IAddon)item).CouldFit(location, Map))
                            {
                                item.MoveToWorld(location, Map);
                                continue;
                            }
                        }
                        else
                        {
                            int height;
                            bool requireSurface;
                            if (item is VendorRentalContract)
                            {
                                height = 16;
                                requireSurface = true;
                            }
                            else
                            {
                                height = item.ItemData.Height;
                                requireSurface = false;
                            }

                            if (Map.CanFit(location.X, location.Y, location.Z, height, false, false, requireSurface))
                            {
                                item.MoveToWorld(location, Map);
                                continue;
                            }
                        }

                        // The item can't fit

                        if (item is TrashBarrel)
                        {
                            item.Delete(); // Trash barrels don't go to the moving crate
                        }
                        else
                        {
                            SetLockdown(relocEntity.Owner, item, false);
                            item.IsSecure = false;
                            item.Movable = true;

                            Item relocateItem = item;

                            if (item is StrongBox)
                                relocateItem = ((StrongBox)item).ConvertToStandardContainer();

                            if (item is IAddon)
                            {
                                Item deed;

                                if (item is FishTrophy)
                                {
                                    deed = ((FishTrophy)item).TrophyDeed;
                                }
                                else
                                {
                                    deed = ((IAddon)item).Deed;
                                }

                                bool retainDeedHue = false;	//if the items aren't hued but the deed itself is
                                int hue = 0;

                                if (item is BaseAddon && ((BaseAddon)item).RetainDeedHue)	//There are things that are IAddon which aren't BaseAddon
                                {
                                    BaseAddon ba = (BaseAddon)item;
                                    retainDeedHue = true;

                                    for (int i = 0; hue == 0 && i < ba.Components.Count; ++i)
                                    {
                                        AddonComponent c = ba.Components[i];

                                        if (c.Hue != 0)
                                            hue = c.Hue;
                                    }
                                }

                                #region Mondain's Legacy
                                if (deed != null)
                                {
                                    if (deed is BaseAddonContainerDeed && item is BaseAddonContainer)
                                    {
                                        BaseAddonContainer c = (BaseAddonContainer)item;
                                        c.DropItemsToGround();

                                        ((BaseAddonContainerDeed)deed).Resource = c.Resource;
                                    }
                                    else if (deed is BaseAddonDeed && item is BaseAddon)
                                        ((BaseAddonDeed)deed).Resource = ((BaseAddon)item).Resource;

                                    if (retainDeedHue)
                                        deed.Hue = hue;
                                }
                                #endregion

                                relocateItem = deed;
                                item.Delete();
                            }

                            if (relocateItem != null)
                                DropToMovingCrate(relocateItem);
                        }
                    }

                    if (m_Trash == item)
                        m_Trash = null;

                    LockDowns.Remove(item);
                    VendorRentalContracts.Remove(item);
                    Addons.Remove(item);
                    for (int i = Secures.Count - 1; i >= 0; i--)
                    {
                        if (((SecureInfo)Secures[i]).Item == item)
                            Secures.RemoveAt(i);
                    }
                }
                else
                {
                    Mobile mobile = (Mobile)entity;

                    if (!mobile.Deleted)
                    {
                        if (Map.CanFit(location, 16, false, false))
                        {
                            mobile.MoveToWorld(location, Map);
                        }
                        else
                        {
                            InternalizedVendors.Add(mobile);
                        }
                    }
                }
            }

            RelocatedEntities.Clear();
        }

        public void DropToMovingCrate(Item item)
        {
            if (MovingCrate == null)
                MovingCrate = new MovingCrate(this);

            MovingCrate.DropItem(item);
        }

        public List<Item> GetItems()
        {
            if (Map == null || Map == Map.Internal)
                return new List<Item>();

            Point2D start = new Point2D(X + Components.Min.X, Y + Components.Min.Y);
            Point2D end = new Point2D(X + Components.Max.X + 1, Y + Components.Max.Y + 1);
            Rectangle2D rect = new Rectangle2D(start, end);

            List<Item> list = new List<Item>();

            IPooledEnumerable eable = Map.GetItemsInBounds(rect);

            foreach (Item item in eable)
                if (item.Movable && IsInside(item))
                    list.Add(item);

            eable.Free();

            return list;
        }

        public List<Mobile> GetMobiles()
        {
            if (Map == null || Map == Map.Internal)
                return new List<Mobile>();

            List<Mobile> list = new List<Mobile>();

            foreach (Mobile mobile in Region.GetMobiles())
                if (IsInside(mobile))
                    list.Add(mobile);

            return list;
        }

        public virtual bool CheckAosLockdowns(int need)
        {
            return ((GetAosCurLockdowns() + need) <= GetAosMaxLockdowns());
        }

        public virtual bool CheckAosStorage(int need)
        {
            int fromSecures, fromVendors, fromLockdowns, fromMovingCrate;

            return ((GetAosCurSecures(out fromSecures, out fromVendors, out fromLockdowns, out fromMovingCrate) + need) <= GetAosMaxSecures());
        }

        public static void Configure()
        {
            Item.LockedDownFlag = 1;
            Item.SecureFlag = 2;

            Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), new TimerCallback(Decay_OnTick));
        }

        public virtual int GetAosCurLockdowns()
        {
            int v = 0;

            v += GetLockdowns();

            if (m_Secures != null)
                v += m_Secures.Where(x => !m_LockDowns.ContainsKey(x.Item)).Count();

            if (!NewVendorSystem)
                v += PlayerVendors.Count * 10;

            return v;
        }

        public static bool CheckLockedDown(Item item)
        {
            BaseHouse house = FindHouseAt(item);

            return (house != null && house.IsLockedDown(item));
        }

        public static bool CheckSecured(Item item)
        {
            BaseHouse house = FindHouseAt(item);

            return (house != null && house.IsSecure(item));
        }

        public static bool CheckLockedDownOrSecured(Item item)
        {
            BaseHouse house = FindHouseAt(item);

            return (house != null && (house.IsSecure(item) || house.IsLockedDown(item)));
        }

        public static List<BaseHouse> GetHouses(Mobile m)
        {
            List<BaseHouse> list = new List<BaseHouse>();

            if (m != null)
            {
                List<BaseHouse> exists = null;
                m_Table.TryGetValue(m, out exists);

                if (exists != null)
                {
                    for (int i = 0; i < exists.Count; ++i)
                    {
                        BaseHouse house = exists[i];

                        if (house != null && !house.Deleted && house.Owner == m)
                            list.Add(house);
                    }
                }
            }

            return list;
        }

        public static bool CheckHold(Mobile m, Container cont, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            BaseHouse house = FindHouseAt(cont);

            if (house == null || !house.IsAosRules)
                return true;

            if (house.IsSecure(cont) && !house.CheckAosStorage(1 + item.TotalItems + plusItems))
            {
                if (message)
                    m.SendLocalizedMessage(1061839); // This action would exceed the secure storage limit of the house.

                return false;
            }

            return true;
        }

        public static bool CheckAccessible(Mobile m, Item item)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = FindHouseAt(item);

            if (house == null)
                return true;

            SecureAccessResult res = house.CheckSecureAccess(m, item);

            switch ( res )
            {
                case SecureAccessResult.Insecure:
                    break;
                case SecureAccessResult.Accessible:
                    return true;
                case SecureAccessResult.Inaccessible:
                    return false;
            }

            if (house.IsLockedDown(item))
                return house.IsCoOwner(m) && (item is Container);

            return true;
        }

        public static BaseHouse FindHouseAt(Mobile m)
        {
            if (m == null || m.Deleted)
                return null;

            return FindHouseAt(m.Location, m.Map, 16);
        }

        public static BaseHouse FindHouseAt(Item item)
        {
            if (item == null || item.Deleted)
                return null;

            return FindHouseAt(item.GetWorldLocation(), item.Map, item.ItemData.Height);
        }

        public static BaseHouse FindHouseAt(Point3D loc, Map map, int height)
        {
            if (map == null || map == Map.Internal)
                return null;

            Sector sector = map.GetSector(loc);

            for (int i = 0; i < sector.Multis.Count; ++i)
            {
                BaseHouse house = sector.Multis[i] as BaseHouse;

                if (house != null && house.IsInside(loc, height))
                    return house;
            }

            return null;
        }

        public bool IsInside(Mobile m)
        {
            if (m == null || m.Deleted || m.Map != Map)
                return false;

            return IsInside(m.Location, 16);
        }

        public bool IsInside(Item item)
        {
            if (item == null || item.Deleted || item.Map != Map)
                return false;

            return IsInside(item.Location, item.ItemData.Height);
        }

        public bool CheckAccessibility(Item item, Mobile from)
        {
            SecureAccessResult res = CheckSecureAccess(from, item);

            switch ( res )
            {
                case SecureAccessResult.Insecure:
                    break;
                case SecureAccessResult.Accessible:
                    return true;
                case SecureAccessResult.Inaccessible:
                    return false;
            }

            // staff or not locked down
            if (from.AccessLevel >= AccessLevel.GameMaster || IsOwner(from) || !IsLockedDown(item))
                return true;

            // ISecurable will set its own rules
            if (item is ISecurable)
                return HasSecureAccess(from, ((ISecurable)item).Level);

            if (item.Stackable)
                return true;

            // locked down
            if (m_LockDowns.ContainsKey(item))
            {
                // non friend, but item is on friends only list
                if (!IsFriend(from) && IsInList(item, _AccessibleToFriends))
                    return false;

                // anyone can use list, house must be public or player must have access to house
                if (IsInList(item, _AccessibleToAll) && (m_Public || m_Access.Contains(from)))
                    return true;

                return false;
            }

            return true;
        }

        private bool IsInList(Item item, Type[] list)
        {
            foreach (var t in list)
            {
                if (t == item.GetType() || item.GetType().IsSubclassOf(t))
                    return true;
            }

            return false;
        }

        private Type[] _AccessibleToAll =
        {
            typeof(TenthAnniversarySculpture), typeof(RewardBrazier), typeof(VendorRentalContract), typeof(Dyes), typeof(DyeTub),
            typeof(BaseInstrument), typeof(Clock), typeof(TreasureMap), typeof(RecallRune), typeof(Dices), typeof(BaseBoard), 
            typeof(Runebook)
        };

        private Type[] _AccessibleToFriends =
        {
            typeof(PotionKeg)
        };

        public virtual bool IsInside(Point3D p, int height)
        {
            if (Deleted)
                return false;

            MultiComponentList mcl = Components;

            int x = p.X - (X + mcl.Min.X);
            int y = p.Y - (Y + mcl.Min.Y);

            if (x < 0 || x >= mcl.Width || y < 0 || y >= mcl.Height)
                return false;

            if (this is HouseFoundation && y < (mcl.Height - 1) && p.Z >= Z)
                return true;

            StaticTile[] tiles = mcl.Tiles[x][y];

            for (int j = 0; j < tiles.Length; ++j)
            {
                StaticTile tile = tiles[j];
                int id = tile.ID & TileData.MaxItemValue;
                ItemData data = TileData.ItemTable[id];

                // Slanted roofs do not count; they overhang blocking south and east sides of the multi
                if ((data.Flags & TileFlag.Roof) != 0)
                    continue;

                // Signs and signposts are not considered part of the multi
                if ((id >= 0xB95 && id <= 0xC0E) || (id >= 0xC43 && id <= 0xC44))
                    continue;

                int tileZ = tile.Z + Z;

                if (p.Z == tileZ || (p.Z + height) > tileZ)
                    return true;
            }

            if (tiles.Length == 0 && this is Castle)
                return true;

            return false;
        }

        public SecureAccessResult CheckSecureAccess(Mobile m, Item item)
        {
            if (m_Secures == null || !(item is Container))
                return SecureAccessResult.Insecure;

            for (int i = 0; i < m_Secures.Count; ++i)
            {
                SecureInfo info = (SecureInfo)m_Secures[i];

                if (info.Item == item)
                    return HasSecureAccess(m, info) ? SecureAccessResult.Accessible : SecureAccessResult.Inaccessible;
            }

            return SecureAccessResult.Insecure;
        }

        private static readonly List<BaseHouse> m_AllHouses = new List<BaseHouse>();

        public static List<BaseHouse> AllHouses
        {
            get
            {
                return m_AllHouses;
            }
        }

        public BaseHouse(int multiID, Mobile owner, int MaxLockDown, int MaxSecure)
            : base(multiID)
        {
            m_AllHouses.Add(this);

            m_LastRefreshed = DateTime.UtcNow;

            m_BuiltOn = DateTime.UtcNow;
            m_LastTraded = DateTime.MinValue;

            m_Doors = new List<Item>();
            m_LockDowns = new Dictionary<Item, Mobile>();
            m_Secures = new List<SecureInfo>();
            m_Addons = new Dictionary<Item, Mobile>();
            m_Carpets = new List<Item>();

            m_CoOwners = new List<Mobile>();
            m_Friends = new List<Mobile>();
            m_Bans = new List<Mobile>();
            m_Access = new List<Mobile>();

            m_VendorRentalContracts = new List<Item>();
            m_InternalizedVendors = new List<Mobile>();

            m_Visits = new Dictionary<Mobile, DateTime>();

            m_Owner = owner;

            m_MaxLockDowns = MaxLockDown;
            m_MaxSecures = MaxSecure;

            m_RelativeBanLocation = BaseBanLocation;

            UpdateRegion();

            if (owner != null)
            {
                List<BaseHouse> list = null;
                m_Table.TryGetValue(owner, out list);

                if (list == null)
                    m_Table[owner] = list = new List<BaseHouse>();

                list.Add(this);
            }

            Movable = false;
        }

        public BaseHouse(Serial serial)
            : base(serial)
        {
            m_AllHouses.Add(this);
        }

        public override void OnMapChange()
        {
            if (m_LockDowns == null)
                return;

            UpdateRegion();

            if (m_Sign != null && !m_Sign.Deleted)
                m_Sign.Map = Map;

            if (m_Doors != null)
            {
                foreach (Item item in m_Doors)
                    item.Map = Map;
            }

            foreach (IEntity entity in GetHouseEntities().Keys)
            {
                if (entity is Item)
                    ((Item)entity).Map = Map;
                else
                    ((Mobile)entity).Map = Map;
            }
        }

        public virtual void ChangeSignType(int itemID)
        {
            if (m_Sign != null)
                m_Sign.ItemID = itemID;
        }

        public abstract Rectangle2D[] Area { get; }
        public abstract Point3D BaseBanLocation { get; }

        public virtual void UpdateRegion()
        {
            if (m_Region != null)
                m_Region.Unregister();

            if (Map != null)
            {
                m_Region = new HouseRegion(this);
                m_Region.Register();
            }
            else
            {
                m_Region = null;
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (m_LockDowns == null)
                return;

            int x = base.Location.X - oldLocation.X;
            int y = base.Location.Y - oldLocation.Y;
            int z = base.Location.Z - oldLocation.Z;

            if (m_Sign != null && !m_Sign.Deleted)
                m_Sign.Location = new Point3D(m_Sign.X + x, m_Sign.Y + y, m_Sign.Z + z);

            UpdateRegion();

            if (m_Doors != null)
            {
                foreach (Item item in m_Doors)
                {
                    if (!item.Deleted)
                        item.Location = new Point3D(item.X + x, item.Y + y, item.Z + z);
                }
            }

            foreach (IEntity entity in GetHouseEntities().Keys)
            {
                Point3D newLocation = new Point3D(entity.X + x, entity.Y + y, entity.Z + z);

                if (entity is Item)
                    ((Item)entity).Location = newLocation;
                else
                    ((Mobile)entity).Location = newLocation;
            }
        }

        public BaseDoor AddEastDoor(int x, int y, int z)
        {
            return AddEastDoor(true, x, y, z);
        }

        public BaseDoor AddEastDoor(bool wood, int x, int y, int z)
        {
            BaseDoor door = MakeDoor(wood, DoorFacing.SouthCW);

            AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor AddSouthDoor(int x, int y, int z)
        {
            return AddSouthDoor(true, x, y, z);
        }

        public BaseDoor AddSouthDoor(bool wood, int x, int y, int z)
        {
            BaseDoor door = MakeDoor(wood, DoorFacing.WestCW);

            AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor AddEastDoor(int x, int y, int z, uint k)
        {
            return AddEastDoor(true, x, y, z, k);
        }

        public BaseDoor AddEastDoor(bool wood, int x, int y, int z, uint k)
        {
            BaseDoor door = MakeDoor(wood, DoorFacing.SouthCW);

            door.Locked = true;
            door.KeyValue = k;

            AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor AddSouthDoor(int x, int y, int z, uint k)
        {
            return AddSouthDoor(true, x, y, z, k);
        }

        public BaseDoor AddSouthDoor(bool wood, int x, int y, int z, uint k)
        {
            BaseDoor door = MakeDoor(wood, DoorFacing.WestCW);

            door.Locked = true;
            door.KeyValue = k;

            AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor[] AddSouthDoors(int x, int y, int z, uint k)
        {
            return AddSouthDoors(true, x, y, z, k);
        }

        public BaseDoor[] AddSouthDoors(bool wood, int x, int y, int z, uint k)
        {
            BaseDoor westDoor = MakeDoor(wood, DoorFacing.WestCW);
            BaseDoor eastDoor = MakeDoor(wood, DoorFacing.EastCCW);

            westDoor.Locked = true;
            eastDoor.Locked = true;

            westDoor.KeyValue = k;
            eastDoor.KeyValue = k;

            westDoor.Link = eastDoor;
            eastDoor.Link = westDoor;

            AddDoor(westDoor, x, y, z);
            AddDoor(eastDoor, x + 1, y, z);

            return new BaseDoor[2] { westDoor, eastDoor };
        }

        public uint CreateKeys(Mobile m)
        {
            uint value = Key.RandomValue();

            if (!IsAosRules)
            {
                Key packKey = new Key(KeyType.Gold);
                Key bankKey = new Key(KeyType.Gold);

                packKey.KeyValue = value;
                bankKey.KeyValue = value;

                packKey.LootType = LootType.Newbied;
                bankKey.LootType = LootType.Newbied;

                BankBox box = m.BankBox;

                if (!box.TryDropItem(m, bankKey, false))
                    bankKey.Delete();

                m.AddToBackpack(packKey);
            }

            return value;
        }

        public BaseDoor[] AddSouthDoors(int x, int y, int z)
        {
            return AddSouthDoors(true, x, y, z, false);
        }

        public BaseDoor[] AddSouthDoors(bool wood, int x, int y, int z, bool inv)
        {
            BaseDoor westDoor = MakeDoor(wood, inv ? DoorFacing.WestCCW : DoorFacing.WestCW);
            BaseDoor eastDoor = MakeDoor(wood, inv ? DoorFacing.EastCW : DoorFacing.EastCCW);

            westDoor.Link = eastDoor;
            eastDoor.Link = westDoor;

            AddDoor(westDoor, x, y, z);
            AddDoor(eastDoor, x + 1, y, z);

            return new BaseDoor[2] { westDoor, eastDoor };
        }

        public BaseDoor MakeDoor(bool wood, DoorFacing facing)
        {
            if (wood)
                return new DarkWoodHouseDoor(facing);
            else
                return new MetalHouseDoor(facing);
        }

        public void AddDoor(BaseDoor door, int xoff, int yoff, int zoff)
        {
            door.MoveToWorld(new Point3D(xoff + X, yoff + Y, zoff + Z), Map);
            m_Doors.Add(door);
        }

        public void AddTrashBarrel(Mobile from)
        {
            if (!IsActive)
                return;

            for (int i = 0; m_Doors != null && i < m_Doors.Count; ++i)
            {
                BaseDoor door = m_Doors[i] as BaseDoor;
                Point3D p = door.Location;

                if (door.Open)
                    p = new Point3D(p.X - door.Offset.X, p.Y - door.Offset.Y, p.Z - door.Offset.Z);

                if ((from.Z + 16) >= p.Z && (p.Z + 16) >= from.Z)
                {
                    if (from.InRange(p, 1))
                    {
                        from.SendLocalizedMessage(502120); // You cannot place a trash barrel near a door or near steps.
                        return;
                    }
                }
            }

            if (m_Trash == null || m_Trash.Deleted)
            {
                m_Trash = new TrashBarrel();

                m_Trash.Movable = false;
                m_Trash.MoveToWorld(from.Location, from.Map);

                from.SendLocalizedMessage(502121); /* You have a new trash barrel.
                * Three minutes after you put something in the barrel, the trash will be emptied.
                * Be forewarned, this is permanent! */
            }
            else
            {
                from.SendLocalizedMessage(502117); // You already have a trash barrel!
            }
        }

        public void SetSign(int xoff, int yoff, int zoff)
        {
            m_Sign = new HouseSign(this);
            m_Sign.MoveToWorld(new Point3D(X + xoff, Y + yoff, Z + zoff), Map);
        }

        private void SetLockdown(Mobile m, Item i, bool locked)
        {
            SetLockdown(m, i, locked, false);
        }

        private void SetLockdown(Mobile m, Item i, bool locked, bool checkContains)
        {
            if (m_LockDowns == null)
                return;

            checkContains = true; // We need to check or server will crash!

            #region Mondain's Legacy
            if (i is BaseAddonContainer)
                i.Movable = false;
            #endregion

            i.Movable = !locked;
            i.IsLockedDown = locked;

            if (i is BaseContainer)
            {
                if (!locked)
                {
                    var secure = GetSecureInfoFor(i);

                    if (secure != null)
                        m_Secures.Remove(secure);
                }
                else
                {
                    m_Secures.Add(new SecureInfo(i, SecureLevel.Owner, m, true));
                }
            }

            if (m == null)
                m = Owner;

            Timer.DelayCall(() =>
                i.PrivateOverheadMessage(MessageType.Regular, 0, locked ? 501721 : 501657, m.NetState)); // locked down! : [no longer locked down]

            if (locked)
            {
                if (i is VendorRentalContract)
                {
                    if (!VendorRentalContracts.Contains(i))
                        VendorRentalContracts.Add(i);
                }
                else
                {
                    if (!checkContains || !m_LockDowns.ContainsKey(i))
                        m_LockDowns.Add(i, m);
                }
            }
            else
            {
                VendorRentalContracts.Remove(i);
                m_LockDowns.Remove(i);

                var secure = GetSecureInfoFor(i);

                if (secure != null)
                    m_Secures.Remove(secure);
            }

            if (!locked)
                i.SetLastMoved();

            if ((i is Container) && (!locked || !(i is BaseBoard || i is Aquarium || i is FishBowl || i is BaseSpecialScrollBook)))
            {
                foreach (Item c in i.Items)
                    SetLockdown(m, c, locked, checkContains);
            }
        }

        public bool LockDown(Mobile m, Item item)
        {
            return LockDown(m, item, true);
        }

        public bool LockDown(Mobile m, Item item, bool checkIsInside)
        {
            if (!IsFriend(m) || !IsActive)
                return false;

            if ((item is BaseAddonContainer || item.Movable) && !IsSecure(item))
            {
                int amt = 1 + item.TotalItems;

                Item rootItem = item.RootParent as Item;
                Item parentItem = item.Parent as Item;

                if (checkIsInside && item.RootParent is Mobile)
                {
                    m.SendLocalizedMessage(1005525);//That is not in your house
                }
                else if (checkIsInside && !IsInside(item.GetWorldLocation(), item.ItemData.Height))
                {
                    m.SendLocalizedMessage(1005525);//That is not in your house
                }
                else if (Ethics.Ethic.IsImbued(item))
                {
                    m.SendLocalizedMessage(1005377);//You cannot lock that down
                }
                else if (IsSecure(rootItem))
                {
                    m.SendLocalizedMessage(501737); // You need not lock down items in a secure container.
                }
                else if (parentItem != null && !IsLockedDown(parentItem))
                {
                    m.SendLocalizedMessage(501736); // You must lockdown the container first!
                }
                else if (!(item is VendorRentalContract) && (IsAosRules ? (!CheckAosLockdowns(amt) || !CheckAosStorage(amt)) : (LockDownCount + amt) > m_MaxLockDowns))
                {
                    m.SendLocalizedMessage(1005379);//That would exceed the maximum lock down limit for this house
                }
                else
                {
                    SetLockdown(m, item, true);
                    return true;
                }
            }
            else if (m_LockDowns.ContainsKey(item))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1005526); //That is already locked down
                return true;
            }
            else if (item is HouseSign || item is Static)
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1005526); // This is already locked down.
            }
            else
            {
                m.SendLocalizedMessage(1005377);//You cannot lock that down
            }

            return false;
        }

        private class TransferItem : Item
        {
            private readonly BaseHouse m_House;

            public override string DefaultName
            {
                get
                {
                    return "a house transfer contract";
                }
            }

            public TransferItem(BaseHouse house)
                : base(0x14F0)
            {
                m_House = house;

                Hue = 0x480;
                Movable = false;
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                string houseName, owner, location;

                houseName = (m_House == null ? "an unnamed house" : m_House.Sign.GetName());

                Mobile houseOwner = (m_House == null ? null : m_House.Owner);

                if (houseOwner == null)
                    owner = "nobody";
                else
                    owner = houseOwner.Name;

                int xLong = 0, yLat = 0, xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                bool valid = m_House != null && Sextant.Format(m_House.Location, m_House.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth);

                if (valid)
                    location = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                else
                    location = "unknown";

                list.Add(1061112, Utility.FixHtml(houseName)); // House Name: ~1_val~
                list.Add(1061113, owner); // Owner: ~1_val~
                list.Add(1061114, location); // Location: ~1_val~
            }

            public TransferItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                Delete();
            }

            public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
            {
                if (!base.AllowSecureTrade(from, to, newOwner, accepted))
                    return false;
                else if (!accepted)
                    return true;

                if (Deleted || m_House == null || m_House.Deleted || !m_House.IsOwner(from) || !from.CheckAlive() || !to.CheckAlive())
                    return false;

                if (BaseHouse.AtAccountHouseLimit(to))
                {
                    from.SendLocalizedMessage(501388); // You cannot transfer ownership to another house owner or co-owner!
                    return false;
                }

                return m_House.CheckTransferPosition(from, to);
            }

            public override void OnSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
            {
                if (Deleted)
                    return;

                Delete();

                if (m_House == null || m_House.Deleted || !m_House.IsOwner(from) || !from.CheckAlive() || !to.CheckAlive())
                    return;

                if (!accepted)
                    return;

                from.SendLocalizedMessage(501338); // You have transferred ownership of the house.
                to.SendLocalizedMessage(501339); /* You are now the owner of this house.
                * The house's co-owner, friend, ban, and access lists have been cleared.
                * You should double-check the security settings on any doors and teleporters in the house.
                */

                m_House.RemoveKeys(from);
                m_House.Owner = to;
                m_House.Bans.Clear();
                m_House.Friends.Clear();
                m_House.CoOwners.Clear();
                m_House.ChangeLocks(to);
                m_House.LastTraded = DateTime.UtcNow;
            }
        }

        public bool CheckTransferPosition(Mobile from, Mobile to)
        {
            bool isValid = true;
            Item sign = m_Sign;
            Point3D p = (sign == null ? Point3D.Zero : sign.GetWorldLocation());

            if (from.Map != Map || to.Map != Map)
                isValid = false;
            else if (sign == null)
                isValid = false;
            else if (from.Map != sign.Map || to.Map != sign.Map)
                isValid = false;
            else if (IsInside(from))
                isValid = false;
            else if (IsInside(to))
                isValid = false;
            else if (!from.InRange(p, 2))
                isValid = false;
            else if (!to.InRange(p, 2))
                isValid = false;

            if (!isValid)
                from.SendLocalizedMessage(1062067); // In order to transfer the house, you and the recipient must both be outside the building and within two paces of the house sign.

            return isValid;
        }

        public void BeginConfirmTransfer(Mobile from, Mobile to)
        {
            if (Deleted || !from.CheckAlive() || !IsOwner(from))
                return;

            if (NewVendorSystem && HasPersonalVendors)
            {
                from.SendLocalizedMessage(1062467); // You cannot trade this house while you still have personal vendors inside.
            }
            else if (DecayLevel == DecayLevel.DemolitionPending)
            {
                from.SendLocalizedMessage(1005321); // This house has been marked for demolition, and it cannot be transferred.
            }
            else if (from == to)
            {
                from.SendLocalizedMessage(1005330); // You cannot transfer a house to yourself, silly.
            }
            else if (to.Player)
            {
                if (BaseHouse.AtAccountHouseLimit(to))
                {
                    from.SendLocalizedMessage(501388); // You cannot transfer ownership to another house owner or co-owner!
                }
                else if (CheckTransferPosition(from, to))
                {
                    from.SendLocalizedMessage(1005326); // Please wait while the other player verifies the transfer.

                    if (HasRentedVendors)
                    {
                        /* You are about to be traded a home that has active vendor contracts.
                        * While there are active vendor contracts in this house, you
                        * <strong>cannot</strong> demolish <strong>OR</strong> customize the home.
                        * When you accept this house, you also accept landlordship for every
                        * contract vendor in the house.
                        */
                        to.SendGump(new WarningGump(1060635, 30720, 1062487, 32512, 420, 280, new WarningGumpCallback(ConfirmTransfer_Callback), from));
                    }
                    else
                    {
                        to.CloseGump(typeof(Gumps.HouseTransferGump));
                        to.SendGump(new Gumps.HouseTransferGump(from, to, this));
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(501384); // Only a player can own a house!
            }
        }

        private void ConfirmTransfer_Callback(Mobile to, bool ok, object state)
        {
            Mobile from = (Mobile)state;

            if (!ok || Deleted || !from.CheckAlive() || !IsOwner(from))
                return;

            if (CheckTransferPosition(from, to))
            {
                to.CloseGump(typeof(Gumps.HouseTransferGump));
                to.SendGump(new Gumps.HouseTransferGump(from, to, this));
            }
        }

        public void EndConfirmTransfer(Mobile from, Mobile to)
        {
            if (Deleted || !from.CheckAlive() || !IsOwner(from))
                return;

            if (NewVendorSystem && HasPersonalVendors)
            {
                from.SendLocalizedMessage(1062467); // You cannot trade this house while you still have personal vendors inside.
            }
            else if (DecayLevel == DecayLevel.DemolitionPending)
            {
                from.SendLocalizedMessage(1005321); // This house has been marked for demolition, and it cannot be transferred.
            }
            else if (from == to)
            {
                from.SendLocalizedMessage(1005330); // You cannot transfer a house to yourself, silly.
            }
            else if (to.Player)
            {
                if (BaseHouse.AtAccountHouseLimit(to))
                {
                    from.SendLocalizedMessage(501388); // You cannot transfer ownership to another house owner or co-owner!
                }
                else if (CheckTransferPosition(from, to))
                {
                    NetState fromState = from.NetState, toState = to.NetState;

                    if (fromState != null && toState != null)
                    {
                        if (from.HasTrade)
                        {
                            from.SendLocalizedMessage(1062071); // You cannot trade a house while you have other trades pending.
                        }
                        else if (to.HasTrade)
                        {
                            to.SendLocalizedMessage(1062071); // You cannot trade a house while you have other trades pending.
                        }
                        else if (!to.Alive)
                        {
                            // TODO: Check if the message is correct.
                            from.SendLocalizedMessage(1062069); // You cannot transfer this house to that person.
                        }
                        else
                        {
                            Container c = fromState.AddTrade(toState);

                            c.DropItem(new TransferItem(this));
                        }
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(501384); // Only a player can own a house!
            }
        }

        public bool CheckLockdownOwnership(Mobile m, Item item)
        {
            if(item == null)
                return false;

            if(IsOwner(m))
                return true;

            if(item is Container || item.Parent is Container)
            {
                Item check = item.Parent is BaseContainer ? (Item)item.Parent : item;

                return m_Secures.FirstOrDefault(i => i.Item == check && HasSecureAccess(m, i)) != null;
            }

            return LockDowns.ContainsKey(item) && IsSameAccount(m, LockDowns[item]);
        }

        public bool IsSameAccount(Mobile one, Mobile two)
        {
            if (one == null || two == null)
                return false;

            if (one == two)
                return true;

            Account acct = one.Account as Account;

            for (int i = 0; i < acct.Length; ++i)
            {
                if (acct[i] != null && acct[i] == two)
                    return true;
            }

            return false;
        }

        public bool Release(Mobile m, Item item)
        {
            if (!IsFriend(m) || !IsActive)
                return false;

            if (IsLockedDown(item))
            {
                if (!CheckLockdownOwnership(m, item))
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1010418); // You did not lock this down, and you are not able to release this.
                }
                else if (CanRelease(m, item))
                {
                    SetLockdown(m, item, false);

                    if (item is RewardBrazier)
                        ((RewardBrazier)item).TurnOff();

                    return true;
                }

                return false;
            }
            else if (IsSecure(item))
            {
                return ReleaseSecure(m, item);
            }
            else
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1010416); // This is not locked down or secured.
            }

            return false;
        }

        public void AddSecure(Mobile m, Item item)
        {
            if (m_Secures == null || !IsCoOwner(m) || !IsActive)
                return;

            if (!IsInside(item))
            {
                m.SendLocalizedMessage(1005525); // That is not in your house
            }
            else if (IsLockedDown(item))
            {
                m.SendLocalizedMessage(1010550); // This is already locked down and cannot be secured.
            }
            else if (!(item is Container) || item is BaseSpecialScrollBook)
            {
                LockDown(m, item);
            }
            else
            {
                SecureInfo info = null;

                for (int i = 0; info == null && i < m_Secures.Count; ++i)
                    if (((SecureInfo)m_Secures[i]).Item == item)
                        info = (SecureInfo)m_Secures[i];

                if (info != null)
                {
                    m.CloseGump(typeof (SetSecureLevelGump));
                    m.SendGump(new Gumps.SetSecureLevelGump(m_Owner, info, this));
                }
                else if (item.Parent != null)
                {
                    m.SendLocalizedMessage(1010423); // You cannot secure this, place it on the ground first.
                }
                // Mondain's Legacy mod
                else if (!(item is BaseAddonContainer) && !item.Movable)
                {
                    m.SendLocalizedMessage(1010424); // You cannot secure 
                }
                else if (!IsAosRules && SecureCount >= MaxSecures)
                {
                    // The maximum number of secure items has been reached :
                    m.SendLocalizedMessage(1008142, true, MaxSecures.ToString());
                }
                else if (IsAosRules ? !CheckAosLockdowns(1) : ((LockDownCount + 125) >= MaxLockDowns))
                {
                    m.SendLocalizedMessage(1005379); // That would exceed the maximum lock down limit for this house
                }
                else if (IsAosRules && !CheckAosStorage(item.TotalItems))
                {
                    m.SendLocalizedMessage(1061839); // This action would exceed the secure storage limit of the house.
                }
                else
                {
                    info = new SecureInfo((Container)item, SecureLevel.Owner, m);

                    item.IsLockedDown = false;
                    item.IsSecure = true;

                    m_Secures.Add(info);

                    if(m_LockDowns.ContainsKey(item))
                        m_LockDowns.Remove(item);

                    item.Movable = false;

                    if (item is GardenShedAddon)
                    {
                        GardenShedBarrel ad = ((GardenShedAddon)item).SecondContainer as GardenShedBarrel;

                        SecureInfo info2 = new SecureInfo((Container)ad, SecureLevel.Owner, m);

                        ad.IsLockedDown = false;
                        ad.IsSecure = true;

                        m_Secures.Add(info2);

                        if (m_LockDowns.ContainsKey(ad))
                            m_LockDowns.Remove(ad);

                        ad.Movable = false;
                    }

                    m.CloseGump(typeof (SetSecureLevelGump));
                    m.SendGump(new Gumps.SetSecureLevelGump(m_Owner, info, this));
                }
            }
        }

        public virtual bool IsCombatRestricted(Mobile m)
        {
            if (m == null || !m.Player || m.AccessLevel >= AccessLevel.GameMaster || !IsAosRules || (m_Owner != null && m_Owner.AccessLevel >= AccessLevel.GameMaster))
                return false;

            for (int i = 0; i < m.Aggressed.Count; ++i)
            {
                AggressorInfo info = m.Aggressed[i];

                Guild attackerGuild = m.Guild as Guild;
                Guild defenderGuild = info.Defender.Guild as Guild;

                if (info.Defender.Player && info.Defender.Alive && (DateTime.UtcNow - info.LastCombatTime) < HouseRegion.CombatHeatDelay && (attackerGuild == null || defenderGuild == null || defenderGuild != attackerGuild && !defenderGuild.IsEnemy(attackerGuild)))
                    return true;
            }

            return false;
        }

        public bool HasSecureAccess(Mobile m, SecureInfo info)
        {
            if (info.Owner == m)
                return true;

            return HasSecureAccess(m, info.Level);
        }

        public bool HasSecureAccess(Mobile m, SecureLevel level)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (IsCombatRestricted(m))
                return false;

            switch (level)
            {
                case SecureLevel.Owner:
                    return IsOwner(m);
                case SecureLevel.CoOwners:
                    return IsCoOwner(m);
                case SecureLevel.Friends:
                    return IsFriend(m);
                case SecureLevel.Anyone:
                    return true;
                case SecureLevel.Guild:
                    return IsGuildMember(m) | IsOwner(m);
            }

            return false;
        }

        public SecureLevel GetSecureAccess(Mobile m)
        {
            if (IsOwner(m) || m.AccessLevel > AccessLevel.Player)
                return SecureLevel.Owner;

            if (IsCoOwner(m))
                return SecureLevel.CoOwners;

            if (IsFriend(m))
                return SecureLevel.Friends;

            if (IsGuildMember(m))
                return SecureLevel.Guild;

            return SecureLevel.Anyone;
        }

        public SecureInfo GetSecureInfoFor(Item item)
        {
            return m_Secures.FirstOrDefault(info => info.Item == item);
        }

        public SecureInfo GetSecureInfoFor(Mobile from, Item item)
        {
            return m_Secures.FirstOrDefault(info => info.Item == item && (info.Owner == from || IsOwner(from)));
        }

        public List<SecureInfo> GetSecureInfosFor(Mobile from)
        {
            return m_Secures.Where(s => s.Owner == from).ToList();
        }

        public bool ReleaseSecure(Mobile m, Item item)
        {
            if (m_Secures == null || !IsCoOwner(m) || item is StrongBox || !IsActive || !CanRelease(m, item))
                return false;

            var info = GetSecureInfoFor(item);

            if (info != null)
            {
                if ((IsOwner(m) || info.Owner == m) && HasSecureAccess(m, info.Level))
                {
                    item.IsLockedDown = false;
                    item.IsSecure = false;

                    #region Mondain's Legacy
                    if (item is BaseAddonContainer)
                        item.Movable = false;
                    else
                    #endregion
                        item.Movable = true;

                    item.SetLastMoved();
                    item.PublicOverheadMessage(Server.Network.MessageType.Label, 0x3B2, 501656);//[no longer secure]
                    m_Secures.Remove(info);

                    return true;
                }
                else
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1010418); // You did not lock this down, and you are not able to release this.
                }

                return false;
            }

            m.SendLocalizedMessage(501717); //This isn't secure...
            return false;
        }

        private bool CanRelease(Mobile from, Item item)
        {
            if (item is Container)
            {
                foreach (var i in item.Items)
                {
                    if (i is CraftableHouseItem || i is CraftableMetalHouseDoor || i is CraftableStoneHouseDoor)
                    {
                        from.SendLocalizedMessage(1010417); // You may not release this at this time.
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

        public void AddStrongBox(Mobile from)
        {
            if (!IsCoOwner(from) || !IsActive)
                return;

            if (from == Owner)
            {
                from.SendLocalizedMessage(502109); // Owners don't get a strong box
                return;
            }

            if (IsAosRules ? !CheckAosLockdowns(1) : ((LockDownCount + 1) > m_MaxLockDowns))
            {
                from.SendLocalizedMessage(1005379);//That would exceed the maximum lock down limit for this house
                return;
            }

            foreach (SecureInfo info in m_Secures)
            {
                StrongBox c = info.Item as StrongBox;

                if (c != null && !c.Deleted && c.Owner == from)
                {
                    from.SendLocalizedMessage(502112);//You already have a strong box
                    return;
                }
            }

            for (int i = 0; m_Doors != null && i < m_Doors.Count; ++i)
            {
                BaseDoor door = m_Doors[i] as BaseDoor;
                Point3D p = door.Location;

                if (door.Open)
                    p = new Point3D(p.X - door.Offset.X, p.Y - door.Offset.Y, p.Z - door.Offset.Z);

                if ((from.Z + 16) >= p.Z && (p.Z + 16) >= from.Z)
                {
                    if (from.InRange(p, 1))
                    {
                        from.SendLocalizedMessage(502113); // You cannot place a strongbox near a door or near steps.
                        return;
                    }
                }
            }

            StrongBox sb = new StrongBox(from, this);
            sb.Movable = false;
            sb.IsLockedDown = false;
            sb.IsSecure = true;
            m_Secures.Add(new SecureInfo(sb, SecureLevel.CoOwners, from));
            sb.MoveToWorld(from.Location, from.Map);
        }

        public void Kick(Mobile from, Mobile targ)
        {
            if (!IsFriend(from) || m_Friends == null)
                return;

            if (targ.IsStaff() && from.AccessLevel <= targ.AccessLevel)
            {
                from.SendLocalizedMessage(501346); // Uh oh...a bigger boot may be required!
            }
            else if (IsFriend(targ) && !Core.ML)
            {
                from.SendLocalizedMessage(501348); // You cannot eject a friend of the house!
            }
            else if (targ is PlayerVendor)
            {
                from.SendLocalizedMessage(501351); // You cannot eject a vendor.
            }
            else if (!IsInside(targ))
            {
                from.SendLocalizedMessage(501352); // You may not eject someone who is not in your house!
            }
            else if (targ is BaseCreature && ((BaseCreature)targ).NoHouseRestrictions)
            {
                from.SendLocalizedMessage(501347); // You cannot eject that from the house!
            }
            else
            {
                targ.MoveToWorld(BanLocation, Map);

                from.SendLocalizedMessage(1042840, targ.Name); // ~1_PLAYER NAME~ has been ejected from this house.
                targ.SendLocalizedMessage(501341); /* You have been ejected from this house.
                * If you persist in entering, you may be banned from the house.
                */
            }
        }

        public void RemoveAccess(Mobile from, Mobile targ)
        {
            if (!IsFriend(from) || m_Access == null)
                return;

            if (m_Access.Contains(targ))
            {
                m_Access.Remove(targ);

                if (!HasAccess(targ) && IsInside(targ))
                {
                    targ.Location = BanLocation;
                    targ.SendLocalizedMessage(1060734); // Your access to this house has been revoked.
                }

                from.SendLocalizedMessage(1050051); // The invitation has been revoked.
            }
        }

        public void RemoveBan(Mobile from, Mobile targ)
        {
            if (!IsCoOwner(from) || m_Bans == null)
                return;

            if (m_Bans.Contains(targ))
            {
                m_Bans.Remove(targ);

                from.SendLocalizedMessage(501297); // The ban is lifted.
            }
        }

        public void Ban(Mobile from, Mobile targ)
        {
            if (!IsFriend(from) || m_Bans == null)
                return;

            if (targ.IsStaff() && from.AccessLevel <= targ.AccessLevel)
            {
                from.SendLocalizedMessage(501354); // Uh oh...a bigger boot may be required.
            }
            else if (IsFriend(targ))
            {
                from.SendLocalizedMessage(501348); // You cannot eject a friend of the house!
            }
            else if (targ is PlayerVendor)
            {
                from.SendLocalizedMessage(501351); // You cannot eject a vendor.
            }
            else if (m_Bans.Count >= MaxBans)
            {
                from.SendLocalizedMessage(501355); // The ban limit for this house has been reached!
            }
            else if (IsBanned(targ))
            {
                from.SendLocalizedMessage(501356); // This person is already banned!
            }
            else if (!IsInside(targ))
            {
                from.SendLocalizedMessage(501352); // You may not eject someone who is not in your house!
            }
            else if (!Public && IsAosRules)
            {
                from.SendLocalizedMessage(1062521); // You cannot ban someone from a private house.  Revoke their access instead.
            }
            else if (targ is BaseCreature && ((BaseCreature)targ).NoHouseRestrictions)
            {
                from.SendLocalizedMessage(1062040); // You cannot ban that.
            }
            else
            {
                m_Bans.Add(targ);

                from.SendLocalizedMessage(1042839, targ.Name); // ~1_PLAYER_NAME~ has been banned from this house.
                targ.SendLocalizedMessage(501340); // You have been banned from this house.

                targ.MoveToWorld(BanLocation, Map);
            }
        }

        public void GrantAccess(Mobile from, Mobile targ)
        {
            if (!IsFriend(from) || m_Access == null)
                return;

            if (HasAccess(targ))
            {
                from.SendLocalizedMessage(1060729); // That person already has access to this house.
            }
            else if (!targ.Player)
            {
                from.SendLocalizedMessage(1060712); // That is not a player.
            }
            else if (IsBanned(targ))
            {
                from.SendLocalizedMessage(501367); // This person is banned!  Unban them first.
            }
            else
            {
                m_Access.Add(targ);

                targ.SendLocalizedMessage(1060735); // You have been granted access to this house.
            }
        }

        public void AddCoOwner(Mobile from, Mobile targ)
        {
            if (!IsOwner(from) || m_CoOwners == null || m_Friends == null)
                return;

            if (IsOwner(targ))
            {
                from.SendLocalizedMessage(501360); // This person is already the house owner!
            }
            else if (m_Friends.Contains(targ))
            {
                from.SendLocalizedMessage(501361); // This person is a friend of the house. Remove them first.
            }
            else if (!targ.Player)
            {
                from.SendLocalizedMessage(501362); // That can't be a co-owner of the house.
            }
            else if (IsBanned(targ))
            {
                from.SendLocalizedMessage(501367); // This person is banned!  Unban them first.
            }
            else if (m_CoOwners.Count >= MaxCoOwners)
            {
                from.SendLocalizedMessage(501368); // Your co-owner list is full!
            }
            else if (m_CoOwners.Contains(targ))
            {
                from.SendLocalizedMessage(501369); // This person is already on your co-owner list!
            }
            else
            {
                AddCoOwner(targ);

                targ.Delta(MobileDelta.Noto);
                targ.SendLocalizedMessage(501343); // You have been made a co-owner of this house.
            }
        }

        public void AddCoOwner(Mobile targ)
        {
            m_CoOwners.Add(targ);

            List<Mobile> remove = new List<Mobile>();
            
            foreach(Mobile m in m_CoOwners)
            {
                if (AccountHandler.CheckAccount(m, targ) && m != targ)
                    remove.Add(m);
            }

            foreach (Mobile m in remove)
                m_CoOwners.Remove(m);

            remove.Clear();

            foreach (Mobile m in m_Friends)
            {
                if (AccountHandler.CheckAccount(m, targ))
                    remove.Add(m);
            }

            foreach (Mobile m in remove)
                m_Friends.Remove(m);

            remove.Clear();
            remove.TrimExcess();
        }

        public void RemoveCoOwner(Mobile from, Mobile targ)
        {
            RemoveCoOwner(from, targ, true);
        }

        public void RemoveCoOwner(Mobile from, Mobile targ, bool fromMessage)
        {
            if (!IsOwner(from) || m_CoOwners == null)
                return;

            if (m_CoOwners.Contains(targ))
            {
                m_CoOwners.Remove(targ);

                targ.Delta(MobileDelta.Noto);

                if(fromMessage)
                    from.SendLocalizedMessage(501299); // Co-owner removed from list.

                targ.SendLocalizedMessage(501300); // You have been removed as a house co-owner.

                var infos = GetSecureInfosFor(targ);

                foreach (var info in infos)
                {
                    if (info.Item is StrongBox)
                    {
                        StrongBox c = info.Item as StrongBox;

                        c.IsLockedDown = false;
                        c.IsSecure = false;
                        c.Destroy();

                        m_Secures.Remove(info);
                    }
                    else
                    {
                        info.Owner = from;
                    }
                }
            }
        }

        public void AddFriend(Mobile from, Mobile targ)
        {
            if (!IsCoOwner(from) || m_Friends == null || m_CoOwners == null)
                return;

            if (IsOwner(targ))
            {
                from.SendLocalizedMessage(501370); // This person is already an owner of the house!
            }
            else if (m_CoOwners.Contains(targ))
            {
                from.SendLocalizedMessage(501369); // This person is already on your co-owner list!
            }
            else if (!targ.Player)
            {
                from.SendLocalizedMessage(501371); // That can't be a friend of the house.
            }
            else if (IsBanned(targ))
            {
                from.SendLocalizedMessage(501374); // This person is banned!  Unban them first.
            }
            else if (m_Friends.Count >= MaxFriends)
            {
                from.SendLocalizedMessage(501375); // Your friends list is full!
            }
            else if (m_Friends.Contains(targ))
            {
                from.SendLocalizedMessage(501376); // This person is already on your friends list!
            }
            else
            {
                m_Friends.Add(targ);

                targ.Delta(MobileDelta.Noto);
                targ.SendLocalizedMessage(501337); // You have been made a friend of this house.
            }
        }

        public void RemoveFriend(Mobile from, Mobile targ)
        {
            RemoveFriend(from, targ, true);
        }

        public void RemoveFriend(Mobile from, Mobile targ, bool fromMessage)
        {
            if (!IsCoOwner(from) || m_Friends == null)
                return;

            if (m_Friends.Contains(targ))
            {
                m_Friends.Remove(targ);

                targ.Delta(MobileDelta.Noto);

                if(fromMessage)
                    from.SendLocalizedMessage(501298); // Friend removed from list.

                targ.SendLocalizedMessage(1060751); // You are no longer a friend of this house.

                var infos = GetSecureInfosFor(targ);

                foreach (var info in infos)
                {
                    info.Owner = from;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)21); // version

            writer.WriteItemList(m_Carpets, true);

            if (!DynamicDecay.Enabled)
            {
                writer.Write((int)-1);
            }
            else
            {
                writer.Write((int)m_CurrentStage);
                writer.Write(m_NextDecayStage);
            }

            writer.Write((Point3D)m_RelativeBanLocation);

            writer.WriteItemList(m_VendorRentalContracts, true);
            writer.WriteMobileList(m_InternalizedVendors, true);

            writer.WriteEncodedInt(m_RelocatedEntities.Count);
            foreach (RelocatedEntity relEntity in m_RelocatedEntities)
            {
                writer.Write(relEntity.Owner);
                writer.Write((Point3D)relEntity.RelativeLocation);

                if ((relEntity.Entity is Item && ((Item)relEntity.Entity).Deleted) || (relEntity.Entity is Mobile && ((Mobile)relEntity.Entity).Deleted))
                    writer.Write((int)Serial.MinusOne);
                else
                    writer.Write((int)relEntity.Entity.Serial);
            }

            writer.WriteEncodedInt(m_VendorInventories.Count);
            for (int i = 0; i < m_VendorInventories.Count; i++)
            {
                VendorInventory inventory = m_VendorInventories[i];
                inventory.Serialize(writer);
            }

            writer.Write((DateTime)m_LastRefreshed);
            writer.Write((bool)m_RestrictDecay);

            writer.Write(m_Visits.Count);
            foreach (var kvp in m_Visits)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write((int)m_Price);

            writer.WriteMobileList(m_Access);

            writer.Write(m_BuiltOn);
            writer.Write(m_LastTraded);

            //writer.WriteItemList(m_Addons, true);
            writer.Write(m_Addons.Count);
            foreach (var kvp in m_Addons)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

            writer.Write(m_Secures.Count);

            for (int i = 0; i < m_Secures.Count; ++i)
                ((SecureInfo)m_Secures[i]).Serialize(writer);

            writer.Write(m_Public);

            writer.Write(m_Owner);

            writer.WriteMobileList(m_CoOwners, true);
            writer.WriteMobileList(m_Friends, true);
            writer.WriteMobileList(m_Bans, true);

            writer.Write(m_Sign);
            writer.Write(m_Trash);

            writer.WriteItemList(m_Doors, true);

            writer.Write(m_LockDowns.Count);
            ColUtility.ForEach(m_LockDowns, (key, value) =>
                {
                    writer.Write(key);
                    writer.Write(value);
                });

            writer.Write((int)m_MaxLockDowns);
            writer.Write((int)m_MaxSecures);

            // Items in locked down containers that aren't locked down themselves must decay!
            foreach(KeyValuePair<Item, Mobile> kvp in m_LockDowns)
            {
                Item item = kvp.Key;

                if (item is Container && !(item is BaseBoard || item is Aquarium || item is FishBowl))
                {
                    Container cont = (Container)item;
                    List<Item> children = cont.Items;

                    for (int j = 0; j < children.Count; ++j)
                    {
                        Item child = children[j];

                        if (child.Decays && !child.IsLockedDown && !child.IsSecure && (child.LastMoved + child.DecayTime) <= DateTime.UtcNow)
                            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(child.Delete));
                    }
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            int count;
            bool loadedDynamicDecay = false;

            m_Visits = new Dictionary<Mobile, DateTime>();

            if (version < 21)
            {
                SecureInfo.VersionInsertion = true;
            }

            switch (version)
            {
                case 21: // version 21, version insertion for secureinfo
                case 20: // version 20, Addons resulted in version 18 bug added to dictionary
                case 19: // version 19, Visit change to dictionary
                case 18: // version 18, converted addons list to dictionary
                case 17:
                    {
                        m_Carpets = reader.ReadStrongItemList();
                        goto case 16;
                    }
                case 16: // version 16, converted lockdown list to dictionary
                case 15:
                    {
                        int stage = reader.ReadInt();

                        if (stage != -1)
                        {
                            m_CurrentStage = (DecayLevel)stage;
                            m_NextDecayStage = reader.ReadDateTime();
                            loadedDynamicDecay = true;
                        }

                        goto case 14;
                    }
                case 14:
                    {
                        m_RelativeBanLocation = reader.ReadPoint3D();
                        goto case 13;
                    }
                case 13: // removed ban location serialization
                case 12:
                    {
                        m_VendorRentalContracts = reader.ReadStrongItemList();
                        m_InternalizedVendors = reader.ReadStrongMobileList();

                        int relocatedCount = reader.ReadEncodedInt();
                        for (int i = 0; i < relocatedCount; i++)
                        {
                            Mobile m;

                            if (version > 15)
                                m = reader.ReadMobile();
                            else
                                m = Owner;

                            Point3D relLocation = reader.ReadPoint3D();
                            IEntity entity = World.FindEntity(reader.ReadInt());

                            if (entity != null)
                                m_RelocatedEntities.Add(new RelocatedEntity(entity, relLocation, m));
                        }

                        int inventoryCount = reader.ReadEncodedInt();
                        for (int i = 0; i < inventoryCount; i++)
                        {
                            VendorInventory inventory = new VendorInventory(this, reader);
                            m_VendorInventories.Add(inventory);
                        }

                        goto case 11;
                    }
                case 11:
                    {
                        m_LastRefreshed = reader.ReadDateTime();
                        m_RestrictDecay = reader.ReadBool();
                        goto case 10;
                    }
                case 10: // just a signal for updates
                case 9:
                    {
                        if (version <= 18)
                        {
                            reader.ReadInt();
                        }
                        else
                        {
                            int c = reader.ReadInt();
                            for (int i = 0; i < c; i++)
                            {
                                Mobile visitor = reader.ReadMobile();
                                DateTime lastVisit = reader.ReadDateTime();

                                if (visitor != null)
                                    m_Visits[visitor] = lastVisit;
                            }
                        }
                        goto case 8;
                    }
                case 8:
                    {
                        m_Price = reader.ReadInt();
                        goto case 7;
                    }
                case 7:
                    {
                        m_Access = reader.ReadStrongMobileList();
                        goto case 6;
                    }
                case 6:
                    {
                        m_BuiltOn = reader.ReadDateTime();
                        m_LastTraded = reader.ReadDateTime();
                        goto case 5;
                    }
                case 5: // just removed fields
                case 4:
                    {
                        m_Addons = new Dictionary<Item, Mobile>();

                        if (version < 18)
                        {
                            var list = reader.ReadStrongItemList();
                            foreach (var item in list)
                            {
                                m_Addons[item] = Owner;
                            }
                        }
                        else
                        {
                            int c = reader.ReadInt();
                            for (int i = 0; i < c; i++)
                            {
                                var item = reader.ReadItem();
                                var mob = reader.ReadMobile();

                                if (item != null)
                                {
                                    m_Addons[item] = mob != null ? mob : Owner;
                                }
                            }
                        }
                        goto case 3;
                    }
                case 3:
                    {
                        count = reader.ReadInt();
                        m_Secures = new List<SecureInfo>(count);

                        for (int i = 0; i < count; ++i)
                        {
                            SecureInfo info = new SecureInfo(reader);

                            if (info.Item != null)
                            {
                                info.Item.IsSecure = info.IsLockdown ? false : true;
                                m_Secures.Add(info);
                            }
                        }

                        goto case 2;
                    }
                case 2:
                    {
                        m_Public = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        if (version < 13)
                            reader.ReadPoint3D(); // house ban location
                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 17)
                            m_Carpets = new List<Item>();

                        if (version < 14)
                            m_RelativeBanLocation = BaseBanLocation;

                        if (version < 12)
                        {
                            m_VendorRentalContracts = new List<Item>();
                            m_InternalizedVendors = new List<Mobile>();
                        }

                        if (version < 4)
                            m_Addons = new Dictionary<Item, Mobile>();

                        if (version < 7)
                            m_Access = new List<Mobile>();

                        if (version < 8)
                            m_Price = DefaultPrice;

                        m_Owner = reader.ReadMobile();

                        if (version < 5)
                        {
                            count = reader.ReadInt();

                            for (int i = 0; i < count; i++)
                                reader.ReadRect2D();
                        }

                        UpdateRegion();

                        m_CoOwners = reader.ReadStrongMobileList();
                        m_Friends = reader.ReadStrongMobileList();
                        m_Bans = reader.ReadStrongMobileList();

                        m_Sign = reader.ReadItem() as HouseSign;
                        m_Trash = reader.ReadItem() as TrashBarrel;

                        m_Doors = reader.ReadStrongItemList();

                        m_LockDowns = new Dictionary<Item, Mobile>();

                        if (version < 16)
                        {
                            List<Item> list = reader.ReadStrongItemList();

                            foreach (Item item in list)
                            {
                                item.IsLockedDown = true;
                                m_LockDowns[item] = Owner;
                            }
                        }
                        else
                        {
                            int c = reader.ReadInt();
                            for (int i = 0; i < c; i++)
                            {
                                Item item = reader.ReadItem();
                                Mobile m = reader.ReadMobile();

                                if (item != null)
                                {
                                    item.IsLockedDown = true;
                                    m_LockDowns[item] = m != null ? m : Owner;
                                }
                            }
                        }

                        for (int i = 0; i < m_VendorRentalContracts.Count; ++i)
                            ((Item)m_VendorRentalContracts[i]).IsLockedDown = true;

                        if (version < 3)
                        {
                            List<Item> items = reader.ReadStrongItemList();
                            m_Secures = new List<SecureInfo>(items.Count);

                            for (int i = 0; i < items.Count; ++i)
                            {
                                Container c = items[i] as Container;

                                if (c != null)
                                {
                                    c.IsSecure = true;
                                    m_Secures.Add(new SecureInfo(c, SecureLevel.CoOwners, Owner));
                                }
                            }
                        }

                        m_MaxLockDowns = reader.ReadInt();
                        m_MaxSecures = reader.ReadInt();

                        if ((Map == null || Map == Map.Internal) && Location == Point3D.Zero)
                            Delete();

                        if (m_Owner != null)
                        {
                            List<BaseHouse> list = null;
                            m_Table.TryGetValue(m_Owner, out list);

                            if (list == null)
                                m_Table[m_Owner] = list = new List<BaseHouse>();

                            list.Add(this);
                        }
                        break;
                    }
            }

            if (version <= 1)
                ChangeSignType(0xBD2);//private house, plain brass sign

            if (version < 10)
            {
                /* NOTE: This can exceed the house lockdown limit. It must be this way, because
                * we do not want players' items to decay without them knowing. Or not even
                * having a chance to fix it themselves.
                */
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(FixLockdowns_Sandbox));
            }

            if (version < 11)
                m_LastRefreshed = DateTime.UtcNow + TimeSpan.FromHours(24 * Utility.RandomDouble());

            if (DynamicDecay.Enabled && !loadedDynamicDecay)
            {
                DecayLevel old = GetOldDecayLevel();

                if (old == DecayLevel.DemolitionPending)
                    old = DecayLevel.Collapsed;

                SetDynamicDecay(old);
            }

            if (!CheckDecay())
            {
                if (RelocatedEntities.Count > 0)
                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RestoreRelocatedEntities));

                if (m_Owner == null && m_Friends.Count == 0 && m_CoOwners.Count == 0)
                    Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerCallback(Delete));
            }

            if (version == 19)
            {
                Timer.DelayCall(CheckUnregisteredAddons);
            }
        }

        private void CheckUnregisteredAddons()
        {
            if (Region == null || m_Addons == null)
                return;

            foreach (var item in Region.GetEnumeratedItems().Where(i => i is IAddon))
            {
                if(m_Addons.ContainsKey(item))
                    continue;

                m_Addons[item] = Owner;
            }

            foreach (var item in Region.GetEnumeratedItems().Where(i => i is AddonComponent && ((AddonComponent)i).Addon == null))
            {
                item.Delete();
            }
        }

        private void FixLockdowns_Sandbox()
        {
            Dictionary<Item, Mobile> lockDowns = new Dictionary<Item, Mobile>();

            foreach(KeyValuePair<Item, Mobile> kvp in m_LockDowns)
            {
                if (kvp.Key is Container)
                    lockDowns.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<Item, Mobile> kvp in lockDowns)
                SetLockdown(kvp.Value, kvp.Key, true, true);
        }

        public static void HandleDeletion(Mobile mob)
        {
            List<BaseHouse> houses = GetHouses(mob);

            if (houses.Count == 0)
                return;

            Account acct = mob.Account as Account;
            Mobile trans = null;

            for (int i = 0; i < acct.Length; ++i)
            {
                if (acct[i] != null && acct[i] != mob)
                    trans = acct[i];
            }

            for (int i = 0; i < houses.Count; ++i)
            {
                BaseHouse house = houses[i];

                bool canClaim = false;

                if (trans == null)
                    canClaim = (house.CoOwners.Count > 0);
                /*{
                for ( int j = 0; j < house.CoOwners.Count; ++j )
                {
                Mobile check = house.CoOwners[j] as Mobile;

                if ( check != null && !check.Deleted && !AtAccountHouseLimit( check ) )
                {
                canClaim = true;
                break;
                }
                }
                }*/

                if (trans == null && !canClaim)
                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(house.Delete));
                else
                    house.Owner = trans;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                if (m_Owner != null)
                {
                    List<BaseHouse> list = null;
                    m_Table.TryGetValue(m_Owner, out list);

                    if (list == null)
                        m_Table[m_Owner] = list = new List<BaseHouse>();

                    list.Remove(this);
                    m_Owner.Delta(MobileDelta.Noto);
                }

                m_Owner = value;

                if (m_Owner != null)
                {
                    List<BaseHouse> list = null;
                    m_Table.TryGetValue(m_Owner, out list);

                    if (list == null)
                        m_Table[m_Owner] = list = new List<BaseHouse>();

                    list.Add(this);
                    m_Owner.Delta(MobileDelta.Noto);
                }

                if (m_Sign != null)
                    m_Sign.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Dictionary<Mobile, DateTime> Visits
        {
            get
            {
                return m_Visits;
            }
            set
            {
                m_Visits = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalVisits
        { 
            get { return m_Visits.Count; } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Public
        {
            get
            {
                return m_Public;
            }
            set
            {
                if (m_Public != value)
                {
                    m_Public = value;

                    if (!m_Public) // Privatizing the house, change to brass sign
                        ChangeSignType(0xBD2);

                    if (m_Sign != null)
                        m_Sign.InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSecures
        {
            get
            {
                return m_MaxSecures;
            }
            set
            {
                m_MaxSecures = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BanLocation
        {
            get
            {
                if (m_Region != null)
                    return m_Region.GoLocation;

                Point3D rel = m_RelativeBanLocation;
                return new Point3D(X + rel.X, Y + rel.Y, Z + rel.Z);
            }
            set
            {
                RelativeBanLocation = new Point3D(value.X - X, value.Y - Y, value.Z - Z);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D RelativeBanLocation
        {
            get
            {
                return m_RelativeBanLocation;
            }
            set
            {
                m_RelativeBanLocation = value;

                if (m_Region != null)
                    m_Region.GoLocation = new Point3D(X + value.X, Y + value.Y, Z + value.Z);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLockDowns
        {
            get
            {
                return m_MaxLockDowns;
            }
            set
            {
                m_MaxLockDowns = value;
            }
        }

        public Region Region
        {
            get
            {
                return m_Region;
            }
        }
        public List<Mobile> CoOwners
        {
            get
            {
                return m_CoOwners;
            }
            set
            {
                m_CoOwners = value;
            }
        }
        public List<Mobile> Friends
        {
            get
            {
                return m_Friends;
            }
            set
            {
                m_Friends = value;
            }
        }
        public List<Mobile> Access
        {
            get
            {
                return m_Access;
            }
            set
            {
                m_Access = value;
            }
        }
        public List<Mobile> Bans
        {
            get
            {
                return m_Bans;
            }
            set
            {
                m_Bans = value;
            }
        }
        public List<Item> Doors
        {
            get
            {
                return m_Doors;
            }
            set
            {
                m_Doors = value;
            }
        }

        public int GetLockdowns()
        {
            int count = 0;

            if (m_LockDowns != null)
            {
                foreach(KeyValuePair<Item, Mobile> kvp in m_LockDowns)
                {
                    Item item = kvp.Key;

                    if (item is Server.Engines.Plants.Seed && item.Parent is Server.Engines.Plants.SeedBox)
                        continue;

                    if (item is SpecialScroll && item.Parent is BaseSpecialScrollBook)
                        continue;

                    if (!(item is Container))
                        count += item.TotalItems;

                    count++;
                }
            }

            return count;
        }

        public int LockDownCount
        {
            get
            {
                int count = 0;

                count += GetLockdowns();

                if (m_Secures != null)
                {
                    for (int i = 0; i < m_Secures.Count; ++i)
                    {
                        SecureInfo info = m_Secures[i];

                        if (info.Item.Deleted)
                            continue;
                        else if (info.Item is StrongBox)
                            count += 1;
                        else
                            count += 125;
                    }
                }

                return count;
            }
        }

        public int SecureCount
        {
            get
            {
                int count = 0;

                if (m_Secures != null)
                {
                    for (int i = 0; i < m_Secures.Count; i++)
                    {
                        SecureInfo info = m_Secures[i];

                        if (info.Item.Deleted)
                            continue;
                        else if (!(info.Item is StrongBox))
                            count += 1;
                    }
                }

                return count;
            }
        }

        public List<Item> Carpets { get { return m_Carpets; } set { m_Carpets = value; } }

        public Dictionary<Item, Mobile> Addons
        {
            get
            {
                return m_Addons;
            }
            set
            {
                m_Addons = value;
            }
        }

        public Dictionary<Item, Mobile> LockDowns
        {
            get
            {
                return m_LockDowns;
            }
        }

        public List<SecureInfo> Secures
        {
            get
            {
                return m_Secures;
            }
        }
        public HouseSign Sign
        {
            get
            {
                return m_Sign;
            }
            set
            {
                m_Sign = value;
            }
        }
        public List<Mobile> PlayerVendors
        {
            get
            {
                return m_PlayerVendors;
            }
        }
        public List<Mobile> PlayerBarkeepers
        {
            get
            {
                return m_PlayerBarkeepers;
            }
        }
        public List<Item> VendorRentalContracts
        {
            get
            {
                return m_VendorRentalContracts;
            }
        }
        public List<VendorInventory> VendorInventories
        {
            get
            {
                return m_VendorInventories;
            }
        }
        public List<RelocatedEntity> RelocatedEntities
        {
            get
            {
                return m_RelocatedEntities;
            }
        }
        public MovingCrate MovingCrate
        {
            get
            {
                return m_MovingCrate;
            }
            set
            {
                m_MovingCrate = value;
            }
        }
        public List<Mobile> InternalizedVendors
        {
            get
            {
                return m_InternalizedVendors;
            }
        }

        public DateTime BuiltOn
        {
            get
            {
                return m_BuiltOn;
            }
            set
            {
                m_BuiltOn = value;
            }
        }

        public DateTime LastTraded
        {
            get
            {
                return m_LastTraded;
            }
            set
            {
                m_LastTraded = value;
            }
        }

        public override void OnDelete()
        {
            RestoreRelocatedEntities();

            new FixColumnTimer(this).Start();

            base.OnDelete();
        }

        private class FixColumnTimer : Timer
        {
            private readonly Map m_Map;
            private readonly int m_StartX;

            private readonly int m_StartY;

            private readonly int m_EndX;

            private readonly int m_EndY;

            public FixColumnTimer(BaseMulti multi)
                : base(TimeSpan.Zero)
            {
                m_Map = multi.Map;

                MultiComponentList mcl = multi.Components;

                m_StartX = multi.X + mcl.Min.X;
                m_StartY = multi.Y + mcl.Min.Y;
                m_EndX = multi.X + mcl.Max.X;
                m_EndY = multi.Y + mcl.Max.Y;
            }

            protected override void OnTick()
            {
                if (m_Map == null)
                    return;

                for (int x = m_StartX; x <= m_EndX; ++x)
                    for (int y = m_StartY; y <= m_EndY; ++y)
                        m_Map.FixColumn(x, y);
            }
        }

        public DateTime LastVisit(Mobile m)
        {
            if (m_Visits.ContainsKey(m))
            {
                return m_Visits[m];
            }

            return DateTime.MinValue;
        }

        public void AddVisit(Mobile m)
        {
            if (m is PlayerMobile && !IsOwner(m))
            {
                m_Visits[m] = DateTime.Now;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Owner != null)
            {
                List<BaseHouse> list = null;
                m_Table.TryGetValue(m_Owner, out list);

                if (list == null)
                    m_Table[m_Owner] = list = new List<BaseHouse>();

                list.Remove(this);
            }

            CheckUnregisteredAddons();

            if (m_Region != null)
            {
                m_Region.Unregister();
                m_Region = null;
            }

            if (m_Sign != null)
                m_Sign.Delete();

            if (m_Trash != null)
                m_Trash.Delete();

            if (m_Doors != null)
            {
                for (int i = 0; i < m_Doors.Count; ++i)
                {
                    Item item = (Item)m_Doors[i];

                    if (item != null)
                        item.Delete();
                }

                m_Doors.Clear();
            }

            if (m_LockDowns != null)
            {
                foreach(KeyValuePair<Item, Mobile> kvp in m_LockDowns)
                {
                    Item item = kvp.Key;

                    if (item != null)
                    {
                        item.IsLockedDown = false;
                        item.IsSecure = false;
                        item.Movable = true;
                        item.SetLastMoved();

                        item.SendLocalizedMessage(501657, ""); // [no longer locked down]
                    }
                }

                m_LockDowns.Clear();
            }

            if (VendorRentalContracts != null)
            {
                for (int i = 0; i < VendorRentalContracts.Count; ++i)
                {
                    Item item = (Item)VendorRentalContracts[i];

                    if (item != null)
                    {
                        item.IsLockedDown = false;
                        item.IsSecure = false;
                        item.Movable = true;
                        item.SetLastMoved();

                        item.SendLocalizedMessage(501657, ""); // [no longer locked down]
                    }
                }

                VendorRentalContracts.Clear();
            }

            if (m_Secures != null)
            {
                for (int i = 0; i < m_Secures.Count; ++i)
                {
                    SecureInfo info = m_Secures[i];

                    if (info.Item is StrongBox)
                    {
                        ((StrongBox)info.Item).Destroy();
                    }
                    else
                    {
                        info.Item.IsLockedDown = false;
                        info.Item.IsSecure = false;
                        info.Item.Movable = true;
                        info.Item.SetLastMoved();

                        info.Item.SendLocalizedMessage(501718, ""); // no longer secure!
                    }
                }

                m_Secures.Clear();
            }

            if (m_Addons != null)
            {
                foreach(var kvp in m_Addons)
                {
                    Item item = kvp.Key;

                    if (item != null)
                    {
                        if (!item.Deleted && item is IAddon)
                        {
                            Item deed = ((IAddon)item).Deed;
                            bool retainDeedHue = false;	//if the items aren't hued but the deed itself is
                            int hue = 0;

                            if (item is BaseAddon && ((BaseAddon)item).RetainDeedHue)	//There are things that are IAddon which aren't BaseAddon
                            {
                                BaseAddon ba = (BaseAddon)item;
                                retainDeedHue = true;

                                for (int j = 0; hue == 0 && j < ba.Components.Count; ++j)
                                {
                                    AddonComponent c = ba.Components[j];

                                    if (c.Hue != 0)
                                        hue = c.Hue;
                                }
                            }

                            if (deed != null)
                            {
                                if (retainDeedHue)
                                    deed.Hue = hue;

                                deed.MoveToWorld(item.Location, item.Map);
                                deed.SendLocalizedMessage(501657, ""); // [no longer locked down]
                            }
                        }

                        item.Delete();
                    }
                }

                m_Addons.Clear();
            }

            if (m_Carpets != null)
            {
                for (int i = 0; i < m_Carpets.Count; ++i)
                {
                    Item carpet = (Item)m_Carpets[i];

                    if (carpet != null)
                    {
                        carpet.Movable = true;
                        carpet.SetLastMoved();
                        carpet.InvalidateProperties();
                    }
                }
            }

            List<VendorInventory> inventories = new List<VendorInventory>(VendorInventories);

            foreach (VendorInventory inventory in inventories)
                inventory.Delete();

            if (MovingCrate != null)
                MovingCrate.Delete();

            KillVendors();

            m_AllHouses.Remove(this);
        }

        public static int GetAccountHouseCount(Mobile m)
        {
            Account a = m.Account as Account;

            if (a == null)
                return 0;

            int count = 0;

            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] != null)
                {
                    count += GetHouseCount(a[i]);
                }
            }

            return count;
        }

        public static int GetHouseCount(Mobile m)
        {
            if (m == null)
                return 0;

            List<BaseHouse> list = null;
            m_Table.TryGetValue(m, out list);

            if (list == null)
                return 0;

            int count = 0;

            for (int i = 0; i < list.Count; ++i)
            {
                BaseHouse h = list[i];

                if (!h.Deleted)
                    count++;
            }

            return count;
        }

        public static bool HasHouse(Mobile m)
        {
            return GetHouseCount(m) > 0;
        }

        public static bool AtAccountHouseLimit(Mobile m)
        {
            return GetAccountHouseCount(m) >= m_AccountHouseLimit;
        }

        public static bool CheckAccountHouseLimit(Mobile m, bool message = true)
        {
            if (AtAccountHouseLimit(m))
            {
                if (message)
                {
                    if (AccountHouseLimit == 1)
                    {
                        m.SendLocalizedMessage(501271); // You already own a house, you may not place another!
                    }
                    else
                    {
                        m.SendMessage("You already own {0} houses, you may not place any more!", BaseHouse.AccountHouseLimit.ToString());
                    }
                }

                return false;
            }

            return true;
        }

        public bool IsOwner(Mobile m)
        {
            if (m == null)
                return false;

            if (m == m_Owner || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return IsAosRules && AccountHandler.CheckAccount(m, m_Owner);
        }

        public bool IsCoOwner(Mobile m)
        {
            if (m == null || m_CoOwners == null)
                return false;

            if (IsOwner(m) || m_CoOwners.Contains(m))
                return true;

            foreach (Mobile mob in m_CoOwners)
            {
                if (AccountHandler.CheckAccount(mob, m))
                    return true;
            }

            return !IsAosRules && AccountHandler.CheckAccount(m, m_Owner);
        }

        public bool IsGuildMember(Mobile m)
        {
            if (m == null || Owner == null || Owner.Guild == null)
                return false;

            return (m.Guild == Owner.Guild);
        }

        public void RemoveKeys(Mobile m)
        {
            if (m_Doors != null)
            {
                uint keyValue = 0;

                for (int i = 0; keyValue == 0 && i < m_Doors.Count; ++i)
                {
                    BaseDoor door = m_Doors[i] as BaseDoor;

                    if (door != null)
                        keyValue = door.KeyValue;
                }

                Key.RemoveKeys(m, keyValue);
            }
        }

        public void ChangeLocks(Mobile m)
        {
            uint keyValue = CreateKeys(m);

            if (m_Doors != null)
            {
                for (int i = 0; i < m_Doors.Count; ++i)
                {
                    BaseDoor door = m_Doors[i] as BaseDoor;

                    if (door != null)
                        door.KeyValue = keyValue;
                }
            }
        }

        public void RemoveLocks()
        {
            if (m_Doors != null)
            {
                for (int i = 0; i < m_Doors.Count; ++i)
                {
                    BaseDoor door = m_Doors[i] as BaseDoor;

                    if (door != null)
                    {
                        door.KeyValue = 0;
                        door.Locked = false;
                    }
                }
            }
        }

        public virtual HousePlacementEntry ConvertEntry
        {
            get
            {
                return null;
            }
        }
        public virtual int ConvertOffsetX
        {
            get
            {
                return 0;
            }
        }
        public virtual int ConvertOffsetY
        {
            get
            {
                return 0;
            }
        }
        public virtual int ConvertOffsetZ
        {
            get
            {
                return 0;
            }
        }

        public virtual int DefaultPrice
        {
            get
            {
                return 0;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get
            {
                return m_Price;
            }
            set
            {
                m_Price = value;
            }
        }

        public virtual HouseDeed GetDeed()
        {
            return null;
        }

        public bool IsFriend(Mobile m)
        {
            if (m == null || m_Friends == null)
                return false;

            return (IsCoOwner(m) || m_Friends.Contains(m));
        }

        public bool IsBanned(Mobile m)
        {
            if (m == null || m == Owner || m.IsStaff() || m_Bans == null)
                return false;

            Account theirAccount = m.Account as Account;

            for (int i = 0; i < m_Bans.Count; ++i)
            {
                Mobile c = (Mobile)m_Bans[i];

                if (c == m)
                    return true;

                Account bannedAccount = c.Account as Account;

                if (bannedAccount != null && bannedAccount == theirAccount)
                    return true;
            }

            return false;
        }

        public bool HasAccess(Mobile m)
        {
            if (m == null)
                return false;

            if (m.IsStaff() || IsFriend(m) || (m_Access != null && m_Access.Contains(m)))
                return true;

            if (m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                if (bc.NoHouseRestrictions)
                    return true;

                if (bc.Controlled || bc.Summoned)
                {
                    m = bc.ControlMaster;

                    if (m == null)
                        m = bc.SummonMaster;

                    if (m == null)
                        return false;

                    if (m.IsStaff() || IsFriend(m) || (m_Access != null && m_Access.Contains(m)))
                        return true;
                }
            }

            return false;
        }

        public new bool IsLockedDown(Item check)
        {
            if (check == null)
                return false;

            if (m_LockDowns == null)
                return false;

            return (m_LockDowns.ContainsKey(check) || VendorRentalContracts.Contains(check));
        }

        public new bool IsSecure(Item item)
        {
            if (item == null)
                return false;

            if (m_Secures == null)
                return false;

            bool contains = false;

            for (int i = 0; !contains && i < m_Secures.Count; ++i)
                contains = m_Secures[i].Item == item;

            return contains;
        }

        public virtual Guildstone FindGuildstone()
        {
            Map map = Map;

            if (map == null)
                return null;

            MultiComponentList mcl = Components;
            IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (Item item in eable)
            {
                if (item is Guildstone && Contains(item))
                {
                    eable.Free();
                    return (Guildstone)item;
                }
            }

            eable.Free();
            return null;
        }
    }

    public enum DecayType
    {
        Ageless,
        AutoRefresh,
        ManualRefresh,
        Condemned
    }

    public enum DecayLevel
    {
        Ageless,
        LikeNew,
        Slightly,
        Somewhat,
        Fairly,
        Greatly,
        IDOC,
        Collapsed,
        DemolitionPending
    }

    public enum SecureAccessResult
    {
        Insecure,
        Accessible,
        Inaccessible
    }

    public enum SecureLevel
    {
        Owner,
        CoOwners,
        Friends,
        Anyone,
        Guild
    }

    public class SecureInfo : ISecurable
    {
        private readonly Item m_Item;
        private SecureLevel m_Level;
        private Mobile m_Owner;
        private bool m_IsLockdown;

        public Item Item { get { return m_Item; } }
        public SecureLevel Level { get { return m_Level; } set { m_Level = value; } }
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }
        public bool IsLockdown { get { return m_IsLockdown; } set { m_IsLockdown = value; } }

        #region *ONLY USED IN BASEHOUSE VERSION 21*
        public static bool VersionInsertion { get; set; }
        #endregion

        public SecureInfo(Item item, SecureLevel level, Mobile owner, bool isLockdown = false)
        {
            m_Item = item;
            m_Level = level;
            m_Owner = owner;
            m_IsLockdown = isLockdown;
        }

        public SecureInfo(GenericReader reader)
        {
            int version = VersionInsertion ? 0 : reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_IsLockdown = reader.ReadBool();
                        m_Owner = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Item = reader.ReadItem();
                        m_Level = (SecureLevel)reader.ReadByte();
                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1); // version

            writer.Write(m_IsLockdown);
            writer.Write(m_Owner);

            writer.Write(m_Item);
            writer.Write((byte)m_Level);
        }
    }

    public class RelocatedEntity
    {
        private readonly IEntity m_Entity;
        private readonly Point3D m_RelativeLocation;
        private readonly Mobile m_Owner;

        public IEntity Entity
        {
            get
            {
                return m_Entity;
            }
        }

        public Point3D RelativeLocation
        {
            get
            {
                return m_RelativeLocation;
            }
        }

        public Mobile Owner
        {
            get
            {
                return m_Owner;
            }
        }

        public RelocatedEntity(IEntity entity, Point3D relativeLocation, Mobile owner)
        {
            m_Entity = entity;
            m_RelativeLocation = relativeLocation;
            m_Owner = owner;
        }
    }

    #region Targets

    public class LockdownTarget : Target
    {
        private readonly bool m_Release;
        private readonly BaseHouse m_House;

        public LockdownTarget(bool release, BaseHouse house)
            : base(12, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_Release = release;
            m_House = house;
        }

        protected override void OnTargetNotAccessible(Mobile from, object targeted)
        {
            OnTarget(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsFriend(from))
                return;

            if (targeted is Item)
            {
                Item item = targeted as Item;

                if (m_Release)
                {
                    #region Mondain's legacy
                    if (targeted is AddonContainerComponent)
                    {
                        AddonContainerComponent component = (AddonContainerComponent)targeted;

                        if (component.Addon != null)
                            m_House.Release(from, component.Addon);
                    }
                    #endregion
                    else if (item.Parent is Container)
                    {
                        from.SendLocalizedMessage(1080387); // You may not release this while it is in a container. 
                    }
                    else
                    {
                        m_House.Release(from, (Item)targeted);
                    }
                }
                else
                {
                    if (targeted is VendorRentalContract)
                    {
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1062392); // You must double click the contract in your pack to lock it down.
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501732); // I cannot lock this down!
                    }
                    else if ((Item)targeted is AddonComponent)
                    {
                        from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 501727); // You cannot lock that down!
                        from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 501732); // I cannot lock this down!
                    }
                    else
                    {
                        #region Mondain's legacy
                        if (targeted is AddonContainerComponent)
                        {
                            AddonContainerComponent component = (AddonContainerComponent)targeted;

                            if (component.Addon != null)
                                m_House.LockDown(from, component.Addon);
                        }
                        else
                        #endregion
                        {
                            m_House.LockDown(from, (Item)targeted);
                        }
                    }
                }
            }
            else if (targeted is StaticTarget)
            {
                return;
            }
            else
            {
                from.SendLocalizedMessage(1005377); //You cannot lock that down
            }
        }
    }

    public class SecureTarget : Target
    {
        private readonly bool m_Release;
        private readonly BaseHouse m_House;

        public SecureTarget(bool release, BaseHouse house)
            : base(12, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_Release = release;
            m_House = house;
        }

        protected override void OnTargetNotAccessible(Mobile from, object targeted)
        {
            OnTarget(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsCoOwner(from))
                return;

            if (targeted is Item)
            {
                if (m_Release)
                {
                    #region Mondain's legacy
                    if (targeted is AddonContainerComponent)
                    {
                        AddonContainerComponent component = (AddonContainerComponent)targeted;

                        if (component.Addon != null)
                            m_House.ReleaseSecure(from, component.Addon);
                    }
                    else
                    #endregion

                        m_House.ReleaseSecure(from, (Item)targeted);
                }
                else
                {
                    if (targeted is VendorRentalContract)
                    {
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1062392); // You must double click the contract in your pack to lock it down.
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501732); // I cannot lock this down!
                    }
                    else
                    {
                        #region Mondain's legacy
                        if (targeted is AddonContainerComponent)
                        {
                            AddonContainerComponent component = (AddonContainerComponent)targeted;

                            if (component.Addon != null)
                                m_House.AddSecure(from, component.Addon);
                        }
                        else
                        #endregion

                            m_House.AddSecure(from, (Item)targeted);
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1010424);//You cannot secure this
            }
        }
    }

    public class HouseKickTarget : Target
    {
        private readonly BaseHouse m_House;

        public HouseKickTarget(BaseHouse house)
            : base(-1, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsFriend(from))
                return;

            if (targeted is Mobile)
            {
                m_House.Kick(from, (Mobile)targeted);
            }
            else
            {
                from.SendLocalizedMessage(501347);//You cannot eject that from the house!
            }
        }
    }

    public class HouseBanTarget : Target
    {
        private readonly BaseHouse m_House;
        private readonly bool m_Banning;

        public HouseBanTarget(bool ban, BaseHouse house)
            : base(-1, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_House = house;
            m_Banning = ban;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsFriend(from))
                return;

            if (targeted is Mobile)
            {
                if (m_Banning)
                    m_House.Ban(from, (Mobile)targeted);
                else
                    m_House.RemoveBan(from, (Mobile)targeted);
            }
            else
            {
                from.SendLocalizedMessage(501347);//You cannot eject that from the house!
            }
        }
    }

    public class HouseAccessTarget : Target
    {
        private readonly BaseHouse m_House;

        public HouseAccessTarget(BaseHouse house)
            : base(-1, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsFriend(from))
                return;

            if (targeted is Mobile)
                m_House.GrantAccess(from, (Mobile)targeted);
            else
                from.SendLocalizedMessage(1060712); // That is not a player.
        }
    }

    public class CoOwnerTarget : Target
    {
        private readonly BaseHouse m_House;
        private readonly bool m_Add;

        public CoOwnerTarget(bool add, BaseHouse house)
            : base(12, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_House = house;
            m_Add = add;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsOwner(from))
                return;

            if (targeted is Mobile)
            {
                if (m_Add)
                    m_House.AddCoOwner(from, (Mobile)targeted);
                else
                    m_House.RemoveCoOwner(from, (Mobile)targeted);
            }
            else
            {
                from.SendLocalizedMessage(501362);//That can't be a coowner
            }
        }
    }

    public class HouseFriendTarget : Target
    {
        private readonly BaseHouse m_House;
        private readonly bool m_Add;

        public HouseFriendTarget(bool add, BaseHouse house)
            : base(12, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_House = house;
            m_Add = add;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || m_House.Deleted || !m_House.IsCoOwner(from))
                return;

            if (targeted is Mobile)
            {
                if (m_Add)
                    m_House.AddFriend(from, (Mobile)targeted);
                else
                    m_House.RemoveFriend(from, (Mobile)targeted);
            }
            else
            {
                from.SendLocalizedMessage(501371); // That can't be a friend
            }
        }
    }

    public class HouseOwnerTarget : Target
    {
        private readonly BaseHouse m_House;

        public HouseOwnerTarget(BaseHouse house)
            : base(12, false, TargetFlags.None)
        {
            CheckLOS = false;

            m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile)
                m_House.BeginConfirmTransfer(from, (Mobile)targeted);
            else
                from.SendLocalizedMessage(501384); // Only a player can own a house!
        }
    }

    #endregion

    public class SetSecureLevelEntry : ContextMenuEntry
    {
        private readonly Item m_Item;
        private readonly ISecurable m_Securable;

        public SetSecureLevelEntry(Item item, ISecurable securable)
            : base(6203, 6)
        {
            m_Item = item;
            m_Securable = securable;
        }

        public static ISecurable GetSecurable(Mobile from, Item item)
        {
            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null || !house.IsAosRules)
                return null;

            bool owner = house.IsOwner(from);
            ISecurable sec = null;

            if (item is ISecurable)
            {
                if (!owner)
                    return null;

                bool isOwned = house.Doors.Contains(item);

                if (!isOwned)
                    isOwned = (house is HouseFoundation && ((HouseFoundation)house).IsFixture(item));

                if (!isOwned)
                    isOwned = house.IsLockedDown(item);

                if (!isOwned)
                    isOwned = item is BaseAddon;

                if (isOwned)
                    sec = (ISecurable)item;
            }
            else
            {
                sec = house.GetSecureInfoFor(from, item);
            }

            return sec;
        }

        public static void AddTo(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            ISecurable sec = GetSecurable(from, item);

            if (sec != null)
                list.Add(new SetSecureLevelEntry(item, sec));
        }

        public override void OnClick()
        {
            ISecurable sec = GetSecurable(Owner.From, m_Item);

            if (sec != null)
            {
                Mobile owner = sec is SecureInfo ? ((SecureInfo)sec).Owner : Owner.From;

                Owner.From.CloseGump(typeof (SetSecureLevelGump));
                Owner.From.SendGump(new SetSecureLevelGump(owner, sec, BaseHouse.FindHouseAt(m_Item)));
            }
        }
    }

    public class ReleaseEntry : ContextMenuEntry
    {
        public Mobile Mobile { get; set; }
        public Item Item { get; set; }
        public BaseHouse House { get; set; }

        public ReleaseEntry(Mobile m, Item item, BaseHouse house)
            : base(1153880, 8)
        {
            Item = item;
            Mobile = m;
            House = house;

            Enabled = Mobile.Alive;
        }

        public override void OnClick()
        {
            if (Mobile.Alive && BaseHouse.FindHouseAt(Mobile) == House && House.IsOwner(Mobile))
            {
                if (Mobile.Backpack == null || !Mobile.Backpack.CheckHold(Mobile, Item, false))
                {
                    Mobile.SendLocalizedMessage(1153881); // Your pack cannot hold this
                }
                else if (House.Release(Mobile, Item))
                {
                    Mobile.Backpack.DropItem(Item);
                }
            }
            else
            {
                Mobile.SendLocalizedMessage(1153882); // You do not own that.
            }
        }
    }

    public class TempNoHousingRegion : BaseRegion
    {
        private readonly Mobile m_RegionOwner;

        public TempNoHousingRegion(BaseHouse house, Mobile regionowner)
            : base(null, house.Map, Region.DefaultPriority, house.Region.Area)
        {
            Register();

            m_RegionOwner = regionowner;

            Timer.DelayCall(house.RestrictedPlacingTime, Unregister);
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return (from == m_RegionOwner || AccountHandler.CheckAccount(from, m_RegionOwner));
        }
    }
}
