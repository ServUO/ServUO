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

namespace Server.Multis
{
    public abstract class BaseHouse : BaseMulti
    {
		private static int m_AccountHouseLimit = Config.Get("Housing.AccountHouseLimit", 1);

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
                return this.m_NextDecayStage;
            }
            set
            {
                this.m_NextDecayStage = value;
            }
        }

        public void ResetDynamicDecay()
        {
            this.m_CurrentStage = DecayLevel.Ageless;
            this.m_NextDecayStage = DateTime.MinValue;
        }

        public void SetDynamicDecay(DecayLevel level)
        {
            this.m_CurrentStage = level;

            if (DynamicDecay.Decays(level))
                this.m_NextDecayStage = DateTime.UtcNow + DynamicDecay.GetRandomDuration(level);
            else
                this.m_NextDecayStage = DateTime.MinValue;
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
                return this.m_LastRefreshed;
            }
            set
            {
                this.m_LastRefreshed = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RestrictDecay
        {
            get
            {
                return this.m_RestrictDecay;
            }
            set
            {
                this.m_RestrictDecay = value;
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
                if (this.m_RestrictDecay || !DecayEnabled || this.DecayPeriod == TimeSpan.Zero)
                    return DecayType.Ageless;

                if (this.m_Owner == null)
                    return Core.AOS ? DecayType.Condemned : DecayType.ManualRefresh;

                Account acct = this.m_Owner.Account as Account;

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

                    if (newest == null || this.IsNewer(check, newest))
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
                DecayType type = this.DecayType;

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

                if (!this.CanDecay)
                {
                    if (DynamicDecay.Enabled)
                        this.ResetDynamicDecay();

                    this.m_LastRefreshed = DateTime.UtcNow;
                    result = DecayLevel.Ageless;
                }
                else if (DynamicDecay.Enabled)
                {
                    DecayLevel stage = this.m_CurrentStage;

                    if (stage == DecayLevel.Ageless || (DynamicDecay.Decays(stage) && this.m_NextDecayStage <= DateTime.UtcNow))
                        this.SetDynamicDecay(++stage);

                    if (stage == DecayLevel.Collapsed && (this.HasRentedVendors || this.VendorInventories.Count > 0))
                        result = DecayLevel.DemolitionPending;
                    else
                        result = stage;
                }
                else
                {
                    result = this.GetOldDecayLevel();
                }

                if (result != this.m_LastDecayLevel)
                {
                    this.m_LastDecayLevel = result;

                    if (this.m_Sign != null && !this.m_Sign.GettingProperties)
                        this.m_Sign.InvalidateProperties();
                }

                return result;
            }
        }

        public DecayLevel GetOldDecayLevel()
        {
            TimeSpan timeAfterRefresh = DateTime.UtcNow - this.m_LastRefreshed;
            int percent = (int)((timeAfterRefresh.Ticks * 1000) / this.DecayPeriod.Ticks);

            if (percent >= 1000) // 100.0%
                return (this.HasRentedVendors || this.VendorInventories.Count > 0) ? DecayLevel.DemolitionPending : DecayLevel.Collapsed;
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
            if (this.DecayType == DecayType.Condemned)
                return false;

            DecayLevel oldLevel = this.DecayLevel;

            this.m_LastRefreshed = DateTime.UtcNow;

            if (DynamicDecay.Enabled)
                this.ResetDynamicDecay();

            if (this.m_Sign != null)
                this.m_Sign.InvalidateProperties();

            return (oldLevel > DecayLevel.LikeNew);
        }

        public virtual bool CheckDecay()
        {
            if (!this.Deleted && this.DecayLevel == DecayLevel.Collapsed)
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Decay_Sandbox));
                return true;
            }

            return false;
        }

        public virtual void KillVendors()
        {
            ArrayList list = new ArrayList(this.PlayerVendors);

            foreach (PlayerVendor vendor in list)
                vendor.Destroy(true);

            list = new ArrayList(this.PlayerBarkeepers);

            foreach (PlayerBarkeeper barkeeper in list)
                barkeeper.Delete();
        }

        public virtual void Decay_Sandbox()
        {
            if (this.Deleted)
                return;

            if (Core.ML)
                new TempNoHousingRegion(this, null);

            this.KillVendors();
            this.Delete();
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
                return (Core.ML ? 1.2 : 1.0);
            }
        }

        private bool m_Public;

        private HouseRegion m_Region;
        private HouseSign m_Sign;
        private TrashBarrel m_Trash;
        private ArrayList m_Doors;

        private Mobile m_Owner;

        private ArrayList m_Access;
        private ArrayList m_Bans;
        private ArrayList m_CoOwners;
        private ArrayList m_Friends;

        private readonly ArrayList m_PlayerVendors = new ArrayList();
        private readonly ArrayList m_PlayerBarkeepers = new ArrayList();

        private ArrayList m_LockDowns;
        private ArrayList m_VendorRentalContracts;
        private ArrayList m_Secures;

        private ArrayList m_Addons;

        private readonly ArrayList m_VendorInventories = new ArrayList();
        private readonly ArrayList m_RelocatedEntities = new ArrayList();

        private MovingCrate m_MovingCrate;
        private ArrayList m_InternalizedVendors;

        private int m_MaxLockDowns;
        private int m_MaxSecures;
        private int m_Price;

        private int m_Visits;

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
            HousePlacementEntry hpe = this.GetAosEntry();

            if (hpe == null)
                return 0;

            return (int)(hpe.Storage * this.BonusStorageScalar);
        }

        public virtual int GetAosMaxLockdowns()
        {
            HousePlacementEntry hpe = this.GetAosEntry();

            if (hpe == null)
                return 0;

            return (int)(hpe.Lockdowns * this.BonusStorageScalar);
        }

        public virtual int GetAosCurSecures(out int fromSecures, out int fromVendors, out int fromLockdowns, out int fromMovingCrate)
        {
            fromSecures = 0;
            fromVendors = 0;
            fromLockdowns = 0;
            fromMovingCrate = 0;

            ArrayList list = this.m_Secures;

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    SecureInfo si = (SecureInfo)list[i];

                    fromSecures += si.Item.TotalItems;
                }

                fromLockdowns += list.Count;
            }

            fromLockdowns += this.GetLockdowns();

            if (!NewVendorSystem)
            {
                foreach (PlayerVendor vendor in this.PlayerVendors)
                {
                    if (vendor.Backpack != null)
                    {
                        fromVendors += vendor.Backpack.TotalItems;
                    }
                }
            }

            if (this.MovingCrate != null)
            {
                fromMovingCrate += this.MovingCrate.TotalItems;

                foreach (Item item in this.MovingCrate.Items)
                {
                    if (item is PackingBox)
                        fromMovingCrate--;
                }
            }

            return fromSecures + fromVendors + fromLockdowns + fromMovingCrate;
        }

        public bool InRange(IPoint2D from, int range)
        {
            if (this.Region == null)
                return false;

            foreach (Rectangle3D rect in this.Region.Area)
            {
                if (from.X >= rect.Start.X - range && from.Y >= rect.Start.Y - range && from.X < rect.End.X + range && from.Y < rect.End.Y + range)
                    return true;
            }

            return false;
        }

        public virtual int GetNewVendorSystemMaxVendors()
        {
            HousePlacementEntry hpe = this.GetAosEntry();

            if (hpe == null)
                return 0;

            return (int)(hpe.Vendors * this.BonusStorageScalar);
        }

        public virtual bool CanPlaceNewVendor()
        {
            if (!this.IsAosRules)
                return true;

            if (!NewVendorSystem)
                return this.CheckAosLockdowns(10);

            return ((this.PlayerVendors.Count + this.VendorRentalContracts.Count) < this.GetNewVendorSystemMaxVendors());
        }

        public const int MaximumBarkeepCount = 2;

        public virtual bool CanPlaceNewBarkeep()
        {
            return (this.PlayerBarkeepers.Count < MaximumBarkeepCount);
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
                foreach (PlayerVendor vendor in this.PlayerVendors)
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
                foreach (PlayerVendor vendor in this.PlayerVendors)
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
                foreach (Item item in this.Addons)
                {
                    if (item is BaseAddonContainer)
                        return true;
                }

                return false;
            }
        }
        #endregion

        public ArrayList AvailableVendorsFor(Mobile m)
        {
            ArrayList list = new ArrayList();

            foreach (PlayerVendor vendor in this.PlayerVendors)
            {
                if (vendor.CanInteractWith(m, false))
                    list.Add(vendor);
            }

            return list;
        }

        public bool AreThereAvailableVendorsFor(Mobile m)
        {
            foreach (PlayerVendor vendor in this.PlayerVendors)
            {
                if (vendor.CanInteractWith(m, false))
                    return true;
            }

            return false;
        }

        public void MoveAllToCrate()
        {
            this.RelocatedEntities.Clear();

            if (this.MovingCrate != null)
                this.MovingCrate.Hide();

            if (this.m_Trash != null)
            {
                this.m_Trash.Delete();
                this.m_Trash = null;
            }

            foreach (Item item in this.LockDowns)
            {
                if (!item.Deleted)
                {
                    item.IsLockedDown = false;
                    item.IsSecure = false;
                    item.Movable = true;

                    if (item.Parent == null)
                        this.DropToMovingCrate(item);
                }
            }

            this.LockDowns.Clear();

            foreach (Item item in this.VendorRentalContracts)
            {
                if (!item.Deleted)
                {
                    item.IsLockedDown = false;
                    item.IsSecure = false;
                    item.Movable = true;

                    if (item.Parent == null)
                        this.DropToMovingCrate(item);
                }
            }

            this.VendorRentalContracts.Clear();

            foreach (SecureInfo info in this.Secures)
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
                        this.DropToMovingCrate(item);
                }
            }

            this.Secures.Clear();

            foreach (Item addon in this.Addons)
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

                        this.DropToMovingCrate(deed);
                    }
                    else
                    {
                        this.DropToMovingCrate(addon);
                    }
                }
            }

            this.Addons.Clear();

            foreach (PlayerVendor mobile in this.PlayerVendors)
            {
                mobile.Return();
                mobile.Internalize();
                this.InternalizedVendors.Add(mobile);
            }

            foreach (Mobile mobile in this.PlayerBarkeepers)
            {
                mobile.Internalize();
                this.InternalizedVendors.Add(mobile);
            }
        }

        public List<IEntity> GetHouseEntities()
        {
            List<IEntity> list = new List<IEntity>();

            if (this.MovingCrate != null)
                this.MovingCrate.Hide();

            if (this.m_Trash != null && this.m_Trash.Map != Map.Internal)
                list.Add(this.m_Trash);

            foreach (Item item in this.LockDowns)
            {
                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item);
            }

            foreach (Item item in this.VendorRentalContracts)
            {
                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item);
            }

            foreach (SecureInfo info in this.Secures)
            {
                Item item = info.Item;

                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item);
            }

            foreach (Item item in this.Addons)
            {
                if (item.Parent == null && item.Map != Map.Internal)
                    list.Add(item);
            }

            foreach (PlayerVendor mobile in this.PlayerVendors)
            {
                mobile.Return();

                if (mobile.Map != Map.Internal)
                    list.Add(mobile);
            }

            foreach (Mobile mobile in this.PlayerBarkeepers)
            {
                if (mobile.Map != Map.Internal)
                    list.Add(mobile);
            }

            return list;
        }

        public void RelocateEntities()
        {
            foreach (IEntity entity in this.GetHouseEntities())
            {
                Point3D relLoc = new Point3D(entity.X - this.X, entity.Y - this.Y, entity.Z - this.Z);
                RelocatedEntity relocEntity = new RelocatedEntity(entity, relLoc);

                this.RelocatedEntities.Add(relocEntity);

                if (entity is Item)
                    ((Item)entity).Internalize();
                else
                    ((Mobile)entity).Internalize();
            }
        }

        public void RestoreRelocatedEntities()
        {
            foreach (RelocatedEntity relocEntity in this.RelocatedEntities)
            {
                Point3D relLoc = relocEntity.RelativeLocation;
                Point3D location = new Point3D(relLoc.X + this.X, relLoc.Y + this.Y, relLoc.Z + this.Z);

                IEntity entity = relocEntity.Entity;
                if (entity is Item)
                {
                    Item item = (Item)entity;

                    if (!item.Deleted)
                    {
                        if (item is IAddon)
                        {
                            if (((IAddon)item).CouldFit(location, this.Map))
                            {
                                item.MoveToWorld(location, this.Map);
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

                            if (this.Map.CanFit(location.X, location.Y, location.Z, height, false, false, requireSurface))
                            {
                                item.MoveToWorld(location, this.Map);
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
                            this.SetLockdown(item, false);
                            item.IsSecure = false;
                            item.Movable = true;

                            Item relocateItem = item;

                            if (item is StrongBox)
                                relocateItem = ((StrongBox)item).ConvertToStandardContainer();

                            if (item is IAddon)
                            {
                                Item deed = ((IAddon)item).Deed;
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
                                this.DropToMovingCrate(relocateItem);
                        }
                    }

                    if (this.m_Trash == item)
                        this.m_Trash = null;

                    this.LockDowns.Remove(item);
                    this.VendorRentalContracts.Remove(item);
                    this.Addons.Remove(item);
                    for (int i = this.Secures.Count - 1; i >= 0; i--)
                    {
                        if (((SecureInfo)this.Secures[i]).Item == item)
                            this.Secures.RemoveAt(i);
                    }
                }
                else
                {
                    Mobile mobile = (Mobile)entity;

                    if (!mobile.Deleted)
                    {
                        if (this.Map.CanFit(location, 16, false, false))
                        {
                            mobile.MoveToWorld(location, this.Map);
                        }
                        else
                        {
                            this.InternalizedVendors.Add(mobile);
                        }
                    }
                }
            }

            this.RelocatedEntities.Clear();
        }

        public void DropToMovingCrate(Item item)
        {
            if (this.MovingCrate == null)
                this.MovingCrate = new MovingCrate(this);

            this.MovingCrate.DropItem(item);
        }

        public List<Item> GetItems()
        {
            if (this.Map == null || this.Map == Map.Internal)
                return new List<Item>();

            Point2D start = new Point2D(this.X + this.Components.Min.X, this.Y + this.Components.Min.Y);
            Point2D end = new Point2D(this.X + this.Components.Max.X + 1, this.Y + this.Components.Max.Y + 1);
            Rectangle2D rect = new Rectangle2D(start, end);

            List<Item> list = new List<Item>();

            IPooledEnumerable eable = this.Map.GetItemsInBounds(rect);

            foreach (Item item in eable)
                if (item.Movable && this.IsInside(item))
                    list.Add(item);

            eable.Free();

            return list;
        }

        public List<Mobile> GetMobiles()
        {
            if (this.Map == null || this.Map == Map.Internal)
                return new List<Mobile>();

            List<Mobile> list = new List<Mobile>();

            foreach (Mobile mobile in this.Region.GetMobiles())
                if (this.IsInside(mobile))
                    list.Add(mobile);

            return list;
        }

        public virtual bool CheckAosLockdowns(int need)
        {
            return ((this.GetAosCurLockdowns() + need) <= this.GetAosMaxLockdowns());
        }

        public virtual bool CheckAosStorage(int need)
        {
            int fromSecures, fromVendors, fromLockdowns, fromMovingCrate;

            return ((this.GetAosCurSecures(out fromSecures, out fromVendors, out fromLockdowns, out fromMovingCrate) + need) <= this.GetAosMaxSecures());
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

            v += this.GetLockdowns();

            if (this.m_Secures != null)
                v += this.m_Secures.Count;

            if (!NewVendorSystem)
                v += this.PlayerVendors.Count * 10;

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
            if (m == null || m.Deleted || m.Map != this.Map)
                return false;

            return this.IsInside(m.Location, 16);
        }

        public bool IsInside(Item item)
        {
            if (item == null || item.Deleted || item.Map != this.Map)
                return false;

            return this.IsInside(item.Location, item.ItemData.Height);
        }

        public bool CheckAccessibility(Item item, Mobile from)
        {
            SecureAccessResult res = this.CheckSecureAccess(from, item);

            switch ( res )
            {
                case SecureAccessResult.Insecure:
                    break;
                case SecureAccessResult.Accessible:
                    return true;
                case SecureAccessResult.Inaccessible:
                    return false;
            }

            if (!this.IsLockedDown(item))
                return true;
            else if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;
            else if (item is Runebook)
                return true;
            else if (item is ISecurable)
                return this.HasSecureAccess(from, ((ISecurable)item).Level);
            else if (item is Container)
                return this.IsCoOwner(from);
            else if (item.Stackable)
                return true;
            else if (item is BaseLight)
                return this.IsFriend(from);
            else if (item is PotionKeg)
                return this.IsFriend(from);
            else if (item is BaseBoard)
                return true;
            else if (item is Dices)
                return true;
            else if (item is RecallRune)
                return true;
            else if (item is TreasureMap)
                return true;
            else if (item is Clock)
                return true;
            else if (item is BaseInstrument)
                return true;
            else if (item is Dyes || item is DyeTub)
                return true;
            else if (item is VendorRentalContract)
                return true;
            else if (item is RewardBrazier)
                return true;

            return false;
        }

        public virtual bool IsInside(Point3D p, int height)
        {
            if (this.Deleted)
                return false;

            MultiComponentList mcl = this.Components;

            int x = p.X - (this.X + mcl.Min.X);
            int y = p.Y - (this.Y + mcl.Min.Y);

            if (x < 0 || x >= mcl.Width || y < 0 || y >= mcl.Height)
                return false;

            if (this is HouseFoundation && y < (mcl.Height - 1) && p.Z >= this.Z)
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

                int tileZ = tile.Z + this.Z;

                if (p.Z == tileZ || (p.Z + height) > tileZ)
                    return true;
            }

            return false;
        }

        public SecureAccessResult CheckSecureAccess(Mobile m, Item item)
        {
            if (this.m_Secures == null || !(item is Container))
                return SecureAccessResult.Insecure;

            for (int i = 0; i < this.m_Secures.Count; ++i)
            {
                SecureInfo info = (SecureInfo)this.m_Secures[i];

                if (info.Item == item)
                    return this.HasSecureAccess(m, info.Level) ? SecureAccessResult.Accessible : SecureAccessResult.Inaccessible;
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

            this.m_LastRefreshed = DateTime.UtcNow;

            this.m_BuiltOn = DateTime.UtcNow;
            this.m_LastTraded = DateTime.MinValue;

            this.m_Doors = new ArrayList();
            this.m_LockDowns = new ArrayList();
            this.m_Secures = new ArrayList();
            this.m_Addons = new ArrayList();

            this.m_CoOwners = new ArrayList();
            this.m_Friends = new ArrayList();
            this.m_Bans = new ArrayList();
            this.m_Access = new ArrayList();

            this.m_VendorRentalContracts = new ArrayList();
            this.m_InternalizedVendors = new ArrayList();

            this.m_Owner = owner;

            this.m_MaxLockDowns = MaxLockDown;
            this.m_MaxSecures = MaxSecure;

            this.m_RelativeBanLocation = this.BaseBanLocation;

            this.UpdateRegion();

            if (owner != null)
            {
                List<BaseHouse> list = null;
                m_Table.TryGetValue(owner, out list);

                if (list == null)
                    m_Table[owner] = list = new List<BaseHouse>();

                list.Add(this);
            }

            this.Movable = false;
        }

        public BaseHouse(Serial serial)
            : base(serial)
        {
            m_AllHouses.Add(this);
        }

        public override void OnMapChange()
        {
            if (this.m_LockDowns == null)
                return;

            this.UpdateRegion();

            if (this.m_Sign != null && !this.m_Sign.Deleted)
                this.m_Sign.Map = this.Map;

            if (this.m_Doors != null)
            {
                foreach (Item item in this.m_Doors)
                    item.Map = this.Map;
            }

            foreach (IEntity entity in this.GetHouseEntities())
            {
                if (entity is Item)
                    ((Item)entity).Map = this.Map;
                else
                    ((Mobile)entity).Map = this.Map;
            }
        }

        public virtual void ChangeSignType(int itemID)
        {
            if (this.m_Sign != null)
                this.m_Sign.ItemID = itemID;
        }

        public abstract Rectangle2D[] Area { get; }
        public abstract Point3D BaseBanLocation { get; }

        public virtual void UpdateRegion()
        {
            if (this.m_Region != null)
                this.m_Region.Unregister();

            if (this.Map != null)
            {
                this.m_Region = new HouseRegion(this);
                this.m_Region.Register();
            }
            else
            {
                this.m_Region = null;
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_LockDowns == null)
                return;

            int x = base.Location.X - oldLocation.X;
            int y = base.Location.Y - oldLocation.Y;
            int z = base.Location.Z - oldLocation.Z;

            if (this.m_Sign != null && !this.m_Sign.Deleted)
                this.m_Sign.Location = new Point3D(this.m_Sign.X + x, this.m_Sign.Y + y, this.m_Sign.Z + z);

            this.UpdateRegion();

            if (this.m_Doors != null)
            {
                foreach (Item item in this.m_Doors)
                {
                    if (!item.Deleted)
                        item.Location = new Point3D(item.X + x, item.Y + y, item.Z + z);
                }
            }

            foreach (IEntity entity in this.GetHouseEntities())
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
            return this.AddEastDoor(true, x, y, z);
        }

        public BaseDoor AddEastDoor(bool wood, int x, int y, int z)
        {
            BaseDoor door = this.MakeDoor(wood, DoorFacing.SouthCW);

            this.AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor AddSouthDoor(int x, int y, int z)
        {
            return this.AddSouthDoor(true, x, y, z);
        }

        public BaseDoor AddSouthDoor(bool wood, int x, int y, int z)
        {
            BaseDoor door = this.MakeDoor(wood, DoorFacing.WestCW);

            this.AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor AddEastDoor(int x, int y, int z, uint k)
        {
            return this.AddEastDoor(true, x, y, z, k);
        }

        public BaseDoor AddEastDoor(bool wood, int x, int y, int z, uint k)
        {
            BaseDoor door = this.MakeDoor(wood, DoorFacing.SouthCW);

            door.Locked = true;
            door.KeyValue = k;

            this.AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor AddSouthDoor(int x, int y, int z, uint k)
        {
            return this.AddSouthDoor(true, x, y, z, k);
        }

        public BaseDoor AddSouthDoor(bool wood, int x, int y, int z, uint k)
        {
            BaseDoor door = this.MakeDoor(wood, DoorFacing.WestCW);

            door.Locked = true;
            door.KeyValue = k;

            this.AddDoor(door, x, y, z);

            return door;
        }

        public BaseDoor[] AddSouthDoors(int x, int y, int z, uint k)
        {
            return this.AddSouthDoors(true, x, y, z, k);
        }

        public BaseDoor[] AddSouthDoors(bool wood, int x, int y, int z, uint k)
        {
            BaseDoor westDoor = this.MakeDoor(wood, DoorFacing.WestCW);
            BaseDoor eastDoor = this.MakeDoor(wood, DoorFacing.EastCCW);

            westDoor.Locked = true;
            eastDoor.Locked = true;

            westDoor.KeyValue = k;
            eastDoor.KeyValue = k;

            westDoor.Link = eastDoor;
            eastDoor.Link = westDoor;

            this.AddDoor(westDoor, x, y, z);
            this.AddDoor(eastDoor, x + 1, y, z);

            return new BaseDoor[2] { westDoor, eastDoor };
        }

        public uint CreateKeys(Mobile m)
        {
            uint value = Key.RandomValue();

            if (!this.IsAosRules)
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
            return this.AddSouthDoors(true, x, y, z, false);
        }

        public BaseDoor[] AddSouthDoors(bool wood, int x, int y, int z, bool inv)
        {
            BaseDoor westDoor = this.MakeDoor(wood, inv ? DoorFacing.WestCCW : DoorFacing.WestCW);
            BaseDoor eastDoor = this.MakeDoor(wood, inv ? DoorFacing.EastCW : DoorFacing.EastCCW);

            westDoor.Link = eastDoor;
            eastDoor.Link = westDoor;

            this.AddDoor(westDoor, x, y, z);
            this.AddDoor(eastDoor, x + 1, y, z);

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
            door.MoveToWorld(new Point3D(xoff + this.X, yoff + this.Y, zoff + this.Z), this.Map);
            this.m_Doors.Add(door);
        }

        public void AddTrashBarrel(Mobile from)
        {
            if (!this.IsActive)
                return;

            for (int i = 0; this.m_Doors != null && i < this.m_Doors.Count; ++i)
            {
                BaseDoor door = this.m_Doors[i] as BaseDoor;
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

            if (this.m_Trash == null || this.m_Trash.Deleted)
            {
                this.m_Trash = new TrashBarrel();

                this.m_Trash.Movable = false;
                this.m_Trash.MoveToWorld(from.Location, from.Map);

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
            this.m_Sign = new HouseSign(this);
            this.m_Sign.MoveToWorld(new Point3D(this.X + xoff, this.Y + yoff, this.Z + zoff), this.Map);
        }

        private void SetLockdown(Item i, bool locked)
        {
            this.SetLockdown(i, locked, false);
        }

        private void SetLockdown(Item i, bool locked, bool checkContains)
        {
            if (this.m_LockDowns == null)
                return;

            #region Mondain's Legacy
            if (i is BaseAddonContainer)
                i.Movable = false;
            else
            #endregion

                i.Movable = !locked;
            i.IsLockedDown = locked;

            if (locked)
            {
                if (i is VendorRentalContract)
                {
                    if (!this.VendorRentalContracts.Contains(i))
                        this.VendorRentalContracts.Add(i);
                }
                else
                {
                    if (!checkContains || !this.m_LockDowns.Contains(i))
                        this.m_LockDowns.Add(i);
                }
            }
            else
            {
                this.VendorRentalContracts.Remove(i);
                this.m_LockDowns.Remove(i);
            }

            if (!locked)
                i.SetLastMoved();

            if ((i is Container) && (!locked || !(i is BaseBoard || i is Aquarium || i is FishBowl)))
            {
                foreach (Item c in i.Items)
                    this.SetLockdown(c, locked, checkContains);
            }
        }

        public bool LockDown(Mobile m, Item item)
        {
            return this.LockDown(m, item, true);
        }

        public bool LockDown(Mobile m, Item item, bool checkIsInside)
        {
            if (!this.IsCoOwner(m) || !this.IsActive)
                return false;

            if (item is BaseAddonContainer || item.Movable && !this.IsSecure(item))
            {
                int amt = 1 + item.TotalItems;

                Item rootItem = item.RootParent as Item;
                Item parentItem = item.Parent as Item;

                if (checkIsInside && item.RootParent is Mobile)
                {
                    m.SendLocalizedMessage(1005525);//That is not in your house
                }
                else if (checkIsInside && !this.IsInside(item.GetWorldLocation(), item.ItemData.Height))
                {
                    m.SendLocalizedMessage(1005525);//That is not in your house
                }
                else if (Ethics.Ethic.IsImbued(item))
                {
                    m.SendLocalizedMessage(1005377);//You cannot lock that down
                }
                else if (this.IsSecure(rootItem))
                {
                    m.SendLocalizedMessage(501737); // You need not lock down items in a secure container.
                }
                else if (parentItem != null && !this.IsLockedDown(parentItem))
                {
                    m.SendLocalizedMessage(501736); // You must lockdown the container first!
                }
                else if (!(item is VendorRentalContract) && (this.IsAosRules ? (!this.CheckAosLockdowns(amt) || !this.CheckAosStorage(amt)) : (this.LockDownCount + amt) > this.m_MaxLockDowns))
                {
                    m.SendLocalizedMessage(1005379);//That would exceed the maximum lock down limit for this house
                }
                else
                {
                    this.SetLockdown(item, true);
                    return true;
                }
            }
            else if (this.m_LockDowns.IndexOf(item) != -1)
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
                this.m_House = house;

                this.Hue = 0x480;
                this.Movable = false;
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                string houseName, owner, location;

                houseName = (this.m_House == null ? "an unnamed house" : this.m_House.Sign.GetName());

                Mobile houseOwner = (this.m_House == null ? null : this.m_House.Owner);

                if (houseOwner == null)
                    owner = "nobody";
                else
                    owner = houseOwner.Name;

                int xLong = 0, yLat = 0, xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                bool valid = this.m_House != null && Sextant.Format(this.m_House.Location, this.m_House.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth);

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

                this.Delete();
            }

            public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
            {
                if (!base.AllowSecureTrade(from, to, newOwner, accepted))
                    return false;
                else if (!accepted)
                    return true;

                if (this.Deleted || this.m_House == null || this.m_House.Deleted || !this.m_House.IsOwner(from) || !from.CheckAlive() || !to.CheckAlive())
                    return false;

                if (BaseHouse.HasAccountHouse(to))
                {
                    from.SendLocalizedMessage(501388); // You cannot transfer ownership to another house owner or co-owner!
                    return false;
                }

                return this.m_House.CheckTransferPosition(from, to);
            }

            public override void OnSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
            {
                if (this.Deleted)
                    return;

                this.Delete();

                if (this.m_House == null || this.m_House.Deleted || !this.m_House.IsOwner(from) || !from.CheckAlive() || !to.CheckAlive())
                    return;

                if (!accepted)
                    return;

                from.SendLocalizedMessage(501338); // You have transferred ownership of the house.
                to.SendLocalizedMessage(501339); /* You are now the owner of this house.
                * The house's co-owner, friend, ban, and access lists have been cleared.
                * You should double-check the security settings on any doors and teleporters in the house.
                */

                this.m_House.RemoveKeys(from);
                this.m_House.Owner = to;
                this.m_House.Bans.Clear();
                this.m_House.Friends.Clear();
                this.m_House.CoOwners.Clear();
                this.m_House.ChangeLocks(to);
                this.m_House.LastTraded = DateTime.UtcNow;
            }
        }

        public bool CheckTransferPosition(Mobile from, Mobile to)
        {
            bool isValid = true;
            Item sign = this.m_Sign;
            Point3D p = (sign == null ? Point3D.Zero : sign.GetWorldLocation());

            if (from.Map != this.Map || to.Map != this.Map)
                isValid = false;
            else if (sign == null)
                isValid = false;
            else if (from.Map != sign.Map || to.Map != sign.Map)
                isValid = false;
            else if (this.IsInside(from))
                isValid = false;
            else if (this.IsInside(to))
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
            if (this.Deleted || !from.CheckAlive() || !this.IsOwner(from))
                return;

            if (NewVendorSystem && this.HasPersonalVendors)
            {
                from.SendLocalizedMessage(1062467); // You cannot trade this house while you still have personal vendors inside.
            }
            else if (this.DecayLevel == DecayLevel.DemolitionPending)
            {
                from.SendLocalizedMessage(1005321); // This house has been marked for demolition, and it cannot be transferred.
            }
            else if (from == to)
            {
                from.SendLocalizedMessage(1005330); // You cannot transfer a house to yourself, silly.
            }
            else if (to.Player)
            {
                if (BaseHouse.HasAccountHouse(to))
                {
                    from.SendLocalizedMessage(501388); // You cannot transfer ownership to another house owner or co-owner!
                }
                else if (this.CheckTransferPosition(from, to))
                {
                    from.SendLocalizedMessage(1005326); // Please wait while the other player verifies the transfer.

                    if (this.HasRentedVendors)
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

            if (!ok || this.Deleted || !from.CheckAlive() || !this.IsOwner(from))
                return;

            if (this.CheckTransferPosition(from, to))
            {
                to.CloseGump(typeof(Gumps.HouseTransferGump));
                to.SendGump(new Gumps.HouseTransferGump(from, to, this));
            }
        }

        public void EndConfirmTransfer(Mobile from, Mobile to)
        {
            if (this.Deleted || !from.CheckAlive() || !this.IsOwner(from))
                return;

            if (NewVendorSystem && this.HasPersonalVendors)
            {
                from.SendLocalizedMessage(1062467); // You cannot trade this house while you still have personal vendors inside.
            }
            else if (this.DecayLevel == DecayLevel.DemolitionPending)
            {
                from.SendLocalizedMessage(1005321); // This house has been marked for demolition, and it cannot be transferred.
            }
            else if (from == to)
            {
                from.SendLocalizedMessage(1005330); // You cannot transfer a house to yourself, silly.
            }
            else if (to.Player)
            {
                if (BaseHouse.HasAccountHouse(to))
                {
                    from.SendLocalizedMessage(501388); // You cannot transfer ownership to another house owner or co-owner!
                }
                else if (this.CheckTransferPosition(from, to))
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

        public void Release(Mobile m, Item item)
        {
            if (!this.IsCoOwner(m) || !this.IsActive)
                return;

            if (this.IsLockedDown(item))
            {
                item.PublicOverheadMessage(Server.Network.MessageType.Label, 0x3B2, 501657);//[no longer locked down]
                this.SetLockdown(item, false);
                //TidyItemList( m_LockDowns );

                if (item is RewardBrazier)
                    ((RewardBrazier)item).TurnOff();
            }
            else if (this.IsSecure(item))
            {
                this.ReleaseSecure(m, item);
            }
            else
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1010416); // This is not locked down or secured.
            }
        }

        public void AddSecure(Mobile m, Item item)
        {
            if (this.m_Secures == null || !this.IsOwner(m) || !this.IsActive)
                return;

            if (!this.IsInside(item))
            {
                m.SendLocalizedMessage(1005525); // That is not in your house
            }
            else if (this.IsLockedDown(item))
            {
                m.SendLocalizedMessage(1010550); // This is already locked down and cannot be secured.
            }
            else if (!(item is Container))
            {
                this.LockDown(m, item);
            }
            else
            {
                SecureInfo info = null;

                for (int i = 0; info == null && i < this.m_Secures.Count; ++i)
                    if (((SecureInfo)this.m_Secures[i]).Item == item)
                        info = (SecureInfo)this.m_Secures[i];

                if (info != null)
                {
                    m.CloseGump(typeof (SetSecureLevelGump));
                    m.SendGump(new Gumps.SetSecureLevelGump(this.m_Owner, info, this));
                }
                else if (item.Parent != null)
                {
                    m.SendLocalizedMessage(1010423); // You cannot secure this, place it on the ground first.
                }
                // Mondain's Legacy mod
                else if (!(item is BaseAddonContainer) && !item.Movable)
                {
                    m.SendLocalizedMessage(1010424); // You cannot secure this.
                }
                else if (!this.IsAosRules && this.SecureCount >= this.MaxSecures)
                {
                    // The maximum number of secure items has been reached :
                    m.SendLocalizedMessage(1008142, true, this.MaxSecures.ToString());
                }
                else if (this.IsAosRules ? !this.CheckAosLockdowns(1) : ((this.LockDownCount + 125) >= this.MaxLockDowns))
                {
                    m.SendLocalizedMessage(1005379); // That would exceed the maximum lock down limit for this house
                }
                else if (this.IsAosRules && !this.CheckAosStorage(item.TotalItems))
                {
                    m.SendLocalizedMessage(1061839); // This action would exceed the secure storage limit of the house.
                }
                else
                {
                    info = new SecureInfo((Container)item, SecureLevel.Owner);

                    item.IsLockedDown = false;
                    item.IsSecure = true;

                    this.m_Secures.Add(info);
                    this.m_LockDowns.Remove(item);
                    item.Movable = false;

                    m.CloseGump(typeof (SetSecureLevelGump));
                    m.SendGump(new Gumps.SetSecureLevelGump(this.m_Owner, info, this));
                }
            }
        }

        public virtual bool IsCombatRestricted(Mobile m)
        {
            if (m == null || !m.Player || m.AccessLevel >= AccessLevel.GameMaster || !this.IsAosRules || (this.m_Owner != null && this.m_Owner.AccessLevel >= AccessLevel.GameMaster))
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

        public bool HasSecureAccess(Mobile m, SecureLevel level)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (this.IsCombatRestricted(m))
                return false;

            switch ( level )
            {
                case SecureLevel.Owner:
                    return this.IsOwner(m);
                case SecureLevel.CoOwners:
                    return this.IsCoOwner(m);
                case SecureLevel.Friends:
                    return this.IsFriend(m);
                case SecureLevel.Anyone:
                    return true;
                case SecureLevel.Guild:
                    return this.IsGuildMember(m) | this.IsOwner(m);
//Check
            }

            return false;
        }

        public void ReleaseSecure(Mobile m, Item item)
        {
            if (this.m_Secures == null || !this.IsOwner(m) || item is StrongBox || !this.IsActive)
                return;

            for (int i = 0; i < this.m_Secures.Count; ++i)
            {
                SecureInfo info = (SecureInfo)this.m_Secures[i];

                if (info.Item == item && this.HasSecureAccess(m, info.Level))
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
                    this.m_Secures.RemoveAt(i);
                    return;
                }
            }

            m.SendLocalizedMessage(501717);//This isn't secure...
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
            if (!this.IsCoOwner(from) || !this.IsActive)
                return;

            if (from == this.Owner)
            {
                from.SendLocalizedMessage(502109); // Owners don't get a strong box
                return;
            }

            if (this.IsAosRules ? !this.CheckAosLockdowns(1) : ((this.LockDownCount + 1) > this.m_MaxLockDowns))
            {
                from.SendLocalizedMessage(1005379);//That would exceed the maximum lock down limit for this house
                return;
            }

            foreach (SecureInfo info in this.m_Secures)
            {
                Container c = info.Item;

                if (!c.Deleted && c is StrongBox && ((StrongBox)c).Owner == from)
                {
                    from.SendLocalizedMessage(502112);//You already have a strong box
                    return;
                }
            }

            for (int i = 0; this.m_Doors != null && i < this.m_Doors.Count; ++i)
            {
                BaseDoor door = this.m_Doors[i] as BaseDoor;
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
            this.m_Secures.Add(new SecureInfo(sb, SecureLevel.CoOwners));
            sb.MoveToWorld(from.Location, from.Map);
        }

        public void Kick(Mobile from, Mobile targ)
        {
            if (!this.IsFriend(from) || this.m_Friends == null)
                return;

            if (targ.IsStaff() && from.AccessLevel <= targ.AccessLevel)
            {
                from.SendLocalizedMessage(501346); // Uh oh...a bigger boot may be required!
            }
            else if (this.IsFriend(targ) && !Core.ML)
            {
                from.SendLocalizedMessage(501348); // You cannot eject a friend of the house!
            }
            else if (targ is PlayerVendor)
            {
                from.SendLocalizedMessage(501351); // You cannot eject a vendor.
            }
            else if (!this.IsInside(targ))
            {
                from.SendLocalizedMessage(501352); // You may not eject someone who is not in your house!
            }
            else if (targ is BaseCreature && ((BaseCreature)targ).NoHouseRestrictions)
            {
                from.SendLocalizedMessage(501347); // You cannot eject that from the house!
            }
            else
            {
                targ.MoveToWorld(this.BanLocation, this.Map);

                from.SendLocalizedMessage(1042840, targ.Name); // ~1_PLAYER NAME~ has been ejected from this house.
                targ.SendLocalizedMessage(501341); /* You have been ejected from this house.
                * If you persist in entering, you may be banned from the house.
                */
            }
        }

        public void RemoveAccess(Mobile from, Mobile targ)
        {
            if (!this.IsFriend(from) || this.m_Access == null)
                return;

            if (this.m_Access.Contains(targ))
            {
                this.m_Access.Remove(targ);

                if (!this.HasAccess(targ) && this.IsInside(targ))
                {
                    targ.Location = this.BanLocation;
                    targ.SendLocalizedMessage(1060734); // Your access to this house has been revoked.
                }

                from.SendLocalizedMessage(1050051); // The invitation has been revoked.
            }
        }

        public void RemoveBan(Mobile from, Mobile targ)
        {
            if (!this.IsCoOwner(from) || this.m_Bans == null)
                return;

            if (this.m_Bans.Contains(targ))
            {
                this.m_Bans.Remove(targ);

                from.SendLocalizedMessage(501297); // The ban is lifted.
            }
        }

        public void Ban(Mobile from, Mobile targ)
        {
            if (!this.IsFriend(from) || this.m_Bans == null)
                return;

            if (targ.IsStaff() && from.AccessLevel <= targ.AccessLevel)
            {
                from.SendLocalizedMessage(501354); // Uh oh...a bigger boot may be required.
            }
            else if (this.IsFriend(targ))
            {
                from.SendLocalizedMessage(501348); // You cannot eject a friend of the house!
            }
            else if (targ is PlayerVendor)
            {
                from.SendLocalizedMessage(501351); // You cannot eject a vendor.
            }
            else if (this.m_Bans.Count >= MaxBans)
            {
                from.SendLocalizedMessage(501355); // The ban limit for this house has been reached!
            }
            else if (this.IsBanned(targ))
            {
                from.SendLocalizedMessage(501356); // This person is already banned!
            }
            else if (!this.IsInside(targ))
            {
                from.SendLocalizedMessage(501352); // You may not eject someone who is not in your house!
            }
            else if (!this.Public && this.IsAosRules)
            {
                from.SendLocalizedMessage(1062521); // You cannot ban someone from a private house.  Revoke their access instead.
            }
            else if (targ is BaseCreature && ((BaseCreature)targ).NoHouseRestrictions)
            {
                from.SendLocalizedMessage(1062040); // You cannot ban that.
            }
            else
            {
                this.m_Bans.Add(targ);

                from.SendLocalizedMessage(1042839, targ.Name); // ~1_PLAYER_NAME~ has been banned from this house.
                targ.SendLocalizedMessage(501340); // You have been banned from this house.

                targ.MoveToWorld(this.BanLocation, this.Map);
            }
        }

        public void GrantAccess(Mobile from, Mobile targ)
        {
            if (!this.IsFriend(from) || this.m_Access == null)
                return;

            if (this.HasAccess(targ))
            {
                from.SendLocalizedMessage(1060729); // That person already has access to this house.
            }
            else if (!targ.Player)
            {
                from.SendLocalizedMessage(1060712); // That is not a player.
            }
            else if (this.IsBanned(targ))
            {
                from.SendLocalizedMessage(501367); // This person is banned!  Unban them first.
            }
            else
            {
                this.m_Access.Add(targ);

                targ.SendLocalizedMessage(1060735); // You have been granted access to this house.
            }
        }

        public void AddCoOwner(Mobile from, Mobile targ)
        {
            if (!this.IsOwner(from) || this.m_CoOwners == null || this.m_Friends == null)
                return;

            if (this.IsOwner(targ))
            {
                from.SendLocalizedMessage(501360); // This person is already the house owner!
            }
            else if (this.m_Friends.Contains(targ))
            {
                from.SendLocalizedMessage(501361); // This person is a friend of the house. Remove them first.
            }
            else if (!targ.Player)
            {
                from.SendLocalizedMessage(501362); // That can't be a co-owner of the house.
            }
            else if (HasAccountHouse(targ))
            {
                from.SendLocalizedMessage(501364); // That person is already a house owner.
            }
            else if (this.IsBanned(targ))
            {
                from.SendLocalizedMessage(501367); // This person is banned!  Unban them first.
            }
            else if (this.m_CoOwners.Count >= MaxCoOwners)
            {
                from.SendLocalizedMessage(501368); // Your co-owner list is full!
            }
            else if (this.m_CoOwners.Contains(targ))
            {
                from.SendLocalizedMessage(501369); // This person is already on your co-owner list!
            }
            else
            {
                this.m_CoOwners.Add(targ);

                targ.Delta(MobileDelta.Noto);
                targ.SendLocalizedMessage(501343); // You have been made a co-owner of this house.
            }
        }

        public void RemoveCoOwner(Mobile from, Mobile targ)
        {
            if (!this.IsOwner(from) || this.m_CoOwners == null)
                return;

            if (this.m_CoOwners.Contains(targ))
            {
                this.m_CoOwners.Remove(targ);

                targ.Delta(MobileDelta.Noto);

                from.SendLocalizedMessage(501299); // Co-owner removed from list.
                targ.SendLocalizedMessage(501300); // You have been removed as a house co-owner.

                foreach (SecureInfo info in this.m_Secures)
                {
                    Container c = info.Item;

                    if (c is StrongBox && ((StrongBox)c).Owner == targ)
                    {
                        c.IsLockedDown = false;
                        c.IsSecure = false;
                        this.m_Secures.Remove(info);
                        c.Destroy();
                        break;
                    }
                }
            }
        }

        public void AddFriend(Mobile from, Mobile targ)
        {
            if (!this.IsCoOwner(from) || this.m_Friends == null || this.m_CoOwners == null)
                return;

            if (this.IsOwner(targ))
            {
                from.SendLocalizedMessage(501370); // This person is already an owner of the house!
            }
            else if (this.m_CoOwners.Contains(targ))
            {
                from.SendLocalizedMessage(501369); // This person is already on your co-owner list!
            }
            else if (!targ.Player)
            {
                from.SendLocalizedMessage(501371); // That can't be a friend of the house.
            }
            else if (this.IsBanned(targ))
            {
                from.SendLocalizedMessage(501374); // This person is banned!  Unban them first.
            }
            else if (this.m_Friends.Count >= MaxFriends)
            {
                from.SendLocalizedMessage(501375); // Your friends list is full!
            }
            else if (this.m_Friends.Contains(targ))
            {
                from.SendLocalizedMessage(501376); // This person is already on your friends list!
            }
            else
            {
                this.m_Friends.Add(targ);

                targ.Delta(MobileDelta.Noto);
                targ.SendLocalizedMessage(501337); // You have been made a friend of this house.
            }
        }

        public void RemoveFriend(Mobile from, Mobile targ)
        {
            if (!this.IsCoOwner(from) || this.m_Friends == null)
                return;

            if (this.m_Friends.Contains(targ))
            {
                this.m_Friends.Remove(targ);

                targ.Delta(MobileDelta.Noto);

                from.SendLocalizedMessage(501298); // Friend removed from list.
                targ.SendLocalizedMessage(1060751); // You are no longer a friend of this house.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)15); // version

            if (!DynamicDecay.Enabled)
            {
                writer.Write((int)-1);
            }
            else
            {
                writer.Write((int)this.m_CurrentStage);
                writer.Write(this.m_NextDecayStage);
            }

            writer.Write((Point3D)this.m_RelativeBanLocation);

            writer.WriteItemList(this.m_VendorRentalContracts, true);
            writer.WriteMobileList(this.m_InternalizedVendors, true);

            writer.WriteEncodedInt(this.m_RelocatedEntities.Count);
            foreach (RelocatedEntity relEntity in this.m_RelocatedEntities)
            {
                writer.Write((Point3D)relEntity.RelativeLocation);

                if ((relEntity.Entity is Item && ((Item)relEntity.Entity).Deleted) || (relEntity.Entity is Mobile && ((Mobile)relEntity.Entity).Deleted))
                    writer.Write((int)Serial.MinusOne);
                else
                    writer.Write((int)relEntity.Entity.Serial);
            }

            writer.WriteEncodedInt(this.m_VendorInventories.Count);
            for (int i = 0; i < this.m_VendorInventories.Count; i++)
            {
                VendorInventory inventory = (VendorInventory)this.m_VendorInventories[i];
                inventory.Serialize(writer);
            }

            writer.Write((DateTime)this.m_LastRefreshed);
            writer.Write((bool)this.m_RestrictDecay);

            writer.Write((int)this.m_Visits);

            writer.Write((int)this.m_Price);

            writer.WriteMobileList(this.m_Access);

            writer.Write(this.m_BuiltOn);
            writer.Write(this.m_LastTraded);

            writer.WriteItemList(this.m_Addons, true);

            writer.Write(this.m_Secures.Count);

            for (int i = 0; i < this.m_Secures.Count; ++i)
                ((SecureInfo)this.m_Secures[i]).Serialize(writer);

            writer.Write(this.m_Public);

            //writer.Write( BanLocation );

            writer.Write(this.m_Owner);

            // Version 5 no longer serializes region coords
            /*writer.Write( (int)m_Region.Coords.Count );
            foreach( Rectangle2D rect in m_Region.Coords )
            {
            writer.Write( rect );
            }*/

            writer.WriteMobileList(this.m_CoOwners, true);
            writer.WriteMobileList(this.m_Friends, true);
            writer.WriteMobileList(this.m_Bans, true);

            writer.Write(this.m_Sign);
            writer.Write(this.m_Trash);

            writer.WriteItemList(this.m_Doors, true);
            writer.WriteItemList(this.m_LockDowns, true);
            //writer.WriteItemList( m_Secures, true );

            writer.Write((int)this.m_MaxLockDowns);
            writer.Write((int)this.m_MaxSecures);

            // Items in locked down containers that aren't locked down themselves must decay!
            for (int i = 0; i < this.m_LockDowns.Count; ++i)
            {
                Item item = (Item)this.m_LockDowns[i];

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

            switch ( version )
            {
                case 15:
                    {
                        int stage = reader.ReadInt();

                        if (stage != -1)
                        {
                            this.m_CurrentStage = (DecayLevel)stage;
                            this.m_NextDecayStage = reader.ReadDateTime();
                            loadedDynamicDecay = true;
                        }

                        goto case 14;
                    }
                case 14:
                    {
                        this.m_RelativeBanLocation = reader.ReadPoint3D();
                        goto case 13;
                    }
                case 13: // removed ban location serialization
                case 12:
                    {
                        this.m_VendorRentalContracts = reader.ReadItemList();
                        this.m_InternalizedVendors = reader.ReadMobileList();

                        int relocatedCount = reader.ReadEncodedInt();
                        for (int i = 0; i < relocatedCount; i++)
                        {
                            Point3D relLocation = reader.ReadPoint3D();
                            IEntity entity = World.FindEntity(reader.ReadInt());

                            if (entity != null)
                                this.m_RelocatedEntities.Add(new RelocatedEntity(entity, relLocation));
                        }

                        int inventoryCount = reader.ReadEncodedInt();
                        for (int i = 0; i < inventoryCount; i++)
                        {
                            VendorInventory inventory = new VendorInventory(this, reader);
                            this.m_VendorInventories.Add(inventory);
                        }

                        goto case 11;
                    }
                case 11:
                    {
                        this.m_LastRefreshed = reader.ReadDateTime();
                        this.m_RestrictDecay = reader.ReadBool();
                        goto case 10;
                    }
                case 10: // just a signal for updates
                case 9:
                    {
                        this.m_Visits = reader.ReadInt();
                        goto case 8;
                    }
                case 8:
                    {
                        this.m_Price = reader.ReadInt();
                        goto case 7;
                    }
                case 7:
                    {
                        this.m_Access = reader.ReadMobileList();
                        goto case 6;
                    }
                case 6:
                    {
                        this.m_BuiltOn = reader.ReadDateTime();
                        this.m_LastTraded = reader.ReadDateTime();
                        goto case 5;
                    }
                case 5: // just removed fields
                case 4:
                    {
                        this.m_Addons = reader.ReadItemList();
                        goto case 3;
                    }
                case 3:
                    {
                        count = reader.ReadInt();
                        this.m_Secures = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            SecureInfo info = new SecureInfo(reader);

                            if (info.Item != null)
                            {
                                info.Item.IsSecure = true;
                                this.m_Secures.Add(info);
                            }
                        }

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Public = reader.ReadBool();
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
                        if (version < 14)
                            this.m_RelativeBanLocation = this.BaseBanLocation;

                        if (version < 12)
                        {
                            this.m_VendorRentalContracts = new ArrayList();
                            this.m_InternalizedVendors = new ArrayList();
                        }

                        if (version < 4)
                            this.m_Addons = new ArrayList();

                        if (version < 7)
                            this.m_Access = new ArrayList();

                        if (version < 8)
                            this.m_Price = this.DefaultPrice;

                        this.m_Owner = reader.ReadMobile();

                        if (version < 5)
                        {
                            count = reader.ReadInt();

                            for (int i = 0; i < count; i++)
                                reader.ReadRect2D();
                        }

                        this.UpdateRegion();

                        this.m_CoOwners = reader.ReadMobileList();
                        this.m_Friends = reader.ReadMobileList();
                        this.m_Bans = reader.ReadMobileList();

                        this.m_Sign = reader.ReadItem() as HouseSign;
                        this.m_Trash = reader.ReadItem() as TrashBarrel;

                        this.m_Doors = reader.ReadItemList();
                        this.m_LockDowns = reader.ReadItemList();

                        for (int i = 0; i < this.m_LockDowns.Count; ++i)
                            ((Item)this.m_LockDowns[i]).IsLockedDown = true;

                        for (int i = 0; i < this.m_VendorRentalContracts.Count; ++i)
                            ((Item)this.m_VendorRentalContracts[i]).IsLockedDown = true;

                        if (version < 3)
                        {
                            ArrayList items = reader.ReadItemList();
                            this.m_Secures = new ArrayList(items.Count);

                            for (int i = 0; i < items.Count; ++i)
                            {
                                Container c = items[i] as Container;

                                if (c != null)
                                {
                                    c.IsSecure = true;
                                    this.m_Secures.Add(new SecureInfo(c, SecureLevel.CoOwners));
                                }
                            }
                        }

                        this.m_MaxLockDowns = reader.ReadInt();
                        this.m_MaxSecures = reader.ReadInt();

                        if ((this.Map == null || this.Map == Map.Internal) && this.Location == Point3D.Zero)
                            this.Delete();

                        if (this.m_Owner != null)
                        {
                            List<BaseHouse> list = null;
                            m_Table.TryGetValue(this.m_Owner, out list);

                            if (list == null)
                                m_Table[this.m_Owner] = list = new List<BaseHouse>();

                            list.Add(this);
                        }
                        break;
                    }
            }

            if (version <= 1)
                this.ChangeSignType(0xBD2);//private house, plain brass sign

            if (version < 10)
            {
                /* NOTE: This can exceed the house lockdown limit. It must be this way, because
                * we do not want players' items to decay without them knowing. Or not even
                * having a chance to fix it themselves.
                */
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(FixLockdowns_Sandbox));
            }

            if (version < 11)
                this.m_LastRefreshed = DateTime.UtcNow + TimeSpan.FromHours(24 * Utility.RandomDouble());

            if (DynamicDecay.Enabled && !loadedDynamicDecay)
            {
                DecayLevel old = this.GetOldDecayLevel();

                if (old == DecayLevel.DemolitionPending)
                    old = DecayLevel.Collapsed;

                this.SetDynamicDecay(old);
            }

            if (!this.CheckDecay())
            {
                if (this.RelocatedEntities.Count > 0)
                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RestoreRelocatedEntities));

                if (this.m_Owner == null && this.m_Friends.Count == 0 && this.m_CoOwners.Count == 0)
                    Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerCallback(Delete));
            }
        }

        private void FixLockdowns_Sandbox()
        {
            ArrayList lockDowns = new ArrayList();

            for (int i = 0; this.m_LockDowns != null && i < this.m_LockDowns.Count; ++i)
            {
                Item item = (Item)this.m_LockDowns[i];

                if (item is Container)
                    lockDowns.Add(item);
            }

            for (int i = 0; i < lockDowns.Count; ++i)
                this.SetLockdown((Item)lockDowns[i], true, true);
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

                if ( check != null && !check.Deleted && !HasAccountHouse( check ) )
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
                return this.m_Owner;
            }
            set
            {
                if (this.m_Owner != null)
                {
                    List<BaseHouse> list = null;
                    m_Table.TryGetValue(this.m_Owner, out list);

                    if (list == null)
                        m_Table[this.m_Owner] = list = new List<BaseHouse>();

                    list.Remove(this);
                    this.m_Owner.Delta(MobileDelta.Noto);
                }

                this.m_Owner = value;

                if (this.m_Owner != null)
                {
                    List<BaseHouse> list = null;
                    m_Table.TryGetValue(this.m_Owner, out list);

                    if (list == null)
                        m_Table[this.m_Owner] = list = new List<BaseHouse>();

                    list.Add(this);
                    this.m_Owner.Delta(MobileDelta.Noto);
                }

                if (this.m_Sign != null)
                    this.m_Sign.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Visits
        {
            get
            {
                return this.m_Visits;
            }
            set
            {
                this.m_Visits = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Public
        {
            get
            {
                return this.m_Public;
            }
            set
            {
                if (this.m_Public != value)
                {
                    this.m_Public = value;

                    if (!this.m_Public) // Privatizing the house, change to brass sign
                        this.ChangeSignType(0xBD2);

                    if (this.m_Sign != null)
                        this.m_Sign.InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSecures
        {
            get
            {
                return this.m_MaxSecures;
            }
            set
            {
                this.m_MaxSecures = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BanLocation
        {
            get
            {
                if (this.m_Region != null)
                    return this.m_Region.GoLocation;

                Point3D rel = this.m_RelativeBanLocation;
                return new Point3D(this.X + rel.X, this.Y + rel.Y, this.Z + rel.Z);
            }
            set
            {
                this.RelativeBanLocation = new Point3D(value.X - this.X, value.Y - this.Y, value.Z - this.Z);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D RelativeBanLocation
        {
            get
            {
                return this.m_RelativeBanLocation;
            }
            set
            {
                this.m_RelativeBanLocation = value;

                if (this.m_Region != null)
                    this.m_Region.GoLocation = new Point3D(this.X + value.X, this.Y + value.Y, this.Z + value.Z);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLockDowns
        {
            get
            {
                return this.m_MaxLockDowns;
            }
            set
            {
                this.m_MaxLockDowns = value;
            }
        }

        public Region Region
        {
            get
            {
                return this.m_Region;
            }
        }
        public ArrayList CoOwners
        {
            get
            {
                return this.m_CoOwners;
            }
            set
            {
                this.m_CoOwners = value;
            }
        }
        public ArrayList Friends
        {
            get
            {
                return this.m_Friends;
            }
            set
            {
                this.m_Friends = value;
            }
        }
        public ArrayList Access
        {
            get
            {
                return this.m_Access;
            }
            set
            {
                this.m_Access = value;
            }
        }
        public ArrayList Bans
        {
            get
            {
                return this.m_Bans;
            }
            set
            {
                this.m_Bans = value;
            }
        }
        public ArrayList Doors
        {
            get
            {
                return this.m_Doors;
            }
            set
            {
                this.m_Doors = value;
            }
        }

        public int GetLockdowns()
        {
            int count = 0;

            if (this.m_LockDowns != null)
            {
                for (int i = 0; i < this.m_LockDowns.Count; ++i)
                {
                    if (this.m_LockDowns[i] is Item)
                    {
                        Item item = (Item)this.m_LockDowns[i];

                        if (!(item is Container))
                            count += item.TotalItems;
                    }

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

                count += this.GetLockdowns();

                if (this.m_Secures != null)
                {
                    for (int i = 0; i < this.m_Secures.Count; ++i)
                    {
                        SecureInfo info = (SecureInfo)this.m_Secures[i];

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

                if (this.m_Secures != null)
                {
                    for (int i = 0; i < this.m_Secures.Count; i++)
                    {
                        SecureInfo info = (SecureInfo)this.m_Secures[i];

                        if (info.Item.Deleted)
                            continue;
                        else if (!(info.Item is StrongBox))
                            count += 1;
                    }
                }

                return count;
            }
        }

        public ArrayList Addons
        {
            get
            {
                return this.m_Addons;
            }
            set
            {
                this.m_Addons = value;
            }
        }
        public ArrayList LockDowns
        {
            get
            {
                return this.m_LockDowns;
            }
        }
        public ArrayList Secures
        {
            get
            {
                return this.m_Secures;
            }
        }
        public HouseSign Sign
        {
            get
            {
                return this.m_Sign;
            }
            set
            {
                this.m_Sign = value;
            }
        }
        public ArrayList PlayerVendors
        {
            get
            {
                return this.m_PlayerVendors;
            }
        }
        public ArrayList PlayerBarkeepers
        {
            get
            {
                return this.m_PlayerBarkeepers;
            }
        }
        public ArrayList VendorRentalContracts
        {
            get
            {
                return this.m_VendorRentalContracts;
            }
        }
        public ArrayList VendorInventories
        {
            get
            {
                return this.m_VendorInventories;
            }
        }
        public ArrayList RelocatedEntities
        {
            get
            {
                return this.m_RelocatedEntities;
            }
        }
        public MovingCrate MovingCrate
        {
            get
            {
                return this.m_MovingCrate;
            }
            set
            {
                this.m_MovingCrate = value;
            }
        }
        public ArrayList InternalizedVendors
        {
            get
            {
                return this.m_InternalizedVendors;
            }
        }

        public DateTime BuiltOn
        {
            get
            {
                return this.m_BuiltOn;
            }
            set
            {
                this.m_BuiltOn = value;
            }
        }

        public DateTime LastTraded
        {
            get
            {
                return this.m_LastTraded;
            }
            set
            {
                this.m_LastTraded = value;
            }
        }

        public override void OnDelete()
        {
            this.RestoreRelocatedEntities();

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
                this.m_Map = multi.Map;

                MultiComponentList mcl = multi.Components;

                this.m_StartX = multi.X + mcl.Min.X;
                this.m_StartY = multi.Y + mcl.Min.Y;
                this.m_EndX = multi.X + mcl.Max.X;
                this.m_EndY = multi.Y + mcl.Max.Y;
            }

            protected override void OnTick()
            {
                if (this.m_Map == null)
                    return;

                for (int x = this.m_StartX; x <= this.m_EndX; ++x)
                    for (int y = this.m_StartY; y <= this.m_EndY; ++y)
                        this.m_Map.FixColumn(x, y);
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Owner != null)
            {
                List<BaseHouse> list = null;
                m_Table.TryGetValue(this.m_Owner, out list);

                if (list == null)
                    m_Table[this.m_Owner] = list = new List<BaseHouse>();

                list.Remove(this);
            }

            if (this.m_Region != null)
            {
                this.m_Region.Unregister();
                this.m_Region = null;
            }

            if (this.m_Sign != null)
                this.m_Sign.Delete();

            if (this.m_Trash != null)
                this.m_Trash.Delete();

            if (this.m_Doors != null)
            {
                for (int i = 0; i < this.m_Doors.Count; ++i)
                {
                    Item item = (Item)this.m_Doors[i];

                    if (item != null)
                        item.Delete();
                }

                this.m_Doors.Clear();
            }

            if (this.m_LockDowns != null)
            {
                for (int i = 0; i < this.m_LockDowns.Count; ++i)
                {
                    Item item = (Item)this.m_LockDowns[i];

                    if (item != null)
                    {
                        item.IsLockedDown = false;
                        item.IsSecure = false;
                        item.Movable = true;
                        item.SetLastMoved();
                    }
                }

                this.m_LockDowns.Clear();
            }

            if (this.VendorRentalContracts != null)
            {
                for (int i = 0; i < this.VendorRentalContracts.Count; ++i)
                {
                    Item item = (Item)this.VendorRentalContracts[i];

                    if (item != null)
                    {
                        item.IsLockedDown = false;
                        item.IsSecure = false;
                        item.Movable = true;
                        item.SetLastMoved();
                    }
                }

                this.VendorRentalContracts.Clear();
            }

            if (this.m_Secures != null)
            {
                for (int i = 0; i < this.m_Secures.Count; ++i)
                {
                    SecureInfo info = (SecureInfo)this.m_Secures[i];

                    if (info.Item is StrongBox)
                    {
                        info.Item.Destroy();
                    }
                    else
                    {
                        info.Item.IsLockedDown = false;
                        info.Item.IsSecure = false;
                        info.Item.Movable = true;
                        info.Item.SetLastMoved();
                    }
                }

                this.m_Secures.Clear();
            }

            if (this.m_Addons != null)
            {
                for (int i = 0; i < this.m_Addons.Count; ++i)
                {
                    Item item = (Item)this.m_Addons[i];

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
                            }
                        }

                        item.Delete();
                    }
                }

                this.m_Addons.Clear();
            }

            ArrayList inventories = new ArrayList(this.VendorInventories);

            foreach (VendorInventory inventory in inventories)
                inventory.Delete();

            if (this.MovingCrate != null)
                this.MovingCrate.Delete();

            this.KillVendors();

            m_AllHouses.Remove(this);
        }

        public static bool HasHouse(Mobile m)
        {
            if (m == null)
                return false;

            List<BaseHouse> list = null;
            m_Table.TryGetValue(m, out list);

            if (list == null)
                return false;

            for (int i = 0; i < list.Count; ++i)
            {
                BaseHouse h = list[i];

                if (!h.Deleted)
                    return true;
            }

            return false;
        }

        public static bool HasAccountHouse(Mobile m)
        {
            Account a = m.Account as Account;

            if (a == null)
                return false;

			if(HasHouse(m))
			{
				return true;
			}

			int count = 0;
			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != null && HasHouse(a[i]))
				{
					++count;
				}
			}

			return count >= m_AccountHouseLimit;
        }

        public bool IsOwner(Mobile m)
        {
            if (m == null)
                return false;

            if (m == this.m_Owner || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return this.IsAosRules && AccountHandler.CheckAccount(m, this.m_Owner);
        }

        public bool IsCoOwner(Mobile m)
        {
            if (m == null || this.m_CoOwners == null)
                return false;

            if (this.IsOwner(m) || this.m_CoOwners.Contains(m))
                return true;

            return !this.IsAosRules && AccountHandler.CheckAccount(m, this.m_Owner);
        }

        public bool IsGuildMember(Mobile m)
        {
            if (m == null || this.Owner == null || this.Owner.Guild == null)
                return false;

            return (m.Guild == this.Owner.Guild);
        }

        public void RemoveKeys(Mobile m)
        {
            if (this.m_Doors != null)
            {
                uint keyValue = 0;

                for (int i = 0; keyValue == 0 && i < this.m_Doors.Count; ++i)
                {
                    BaseDoor door = this.m_Doors[i] as BaseDoor;

                    if (door != null)
                        keyValue = door.KeyValue;
                }

                Key.RemoveKeys(m, keyValue);
            }
        }

        public void ChangeLocks(Mobile m)
        {
            uint keyValue = this.CreateKeys(m);

            if (this.m_Doors != null)
            {
                for (int i = 0; i < this.m_Doors.Count; ++i)
                {
                    BaseDoor door = this.m_Doors[i] as BaseDoor;

                    if (door != null)
                        door.KeyValue = keyValue;
                }
            }
        }

        public void RemoveLocks()
        {
            if (this.m_Doors != null)
            {
                for (int i = 0; i < this.m_Doors.Count; ++i)
                {
                    BaseDoor door = this.m_Doors[i] as BaseDoor;

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
                return this.m_Price;
            }
            set
            {
                this.m_Price = value;
            }
        }

        public virtual HouseDeed GetDeed()
        {
            return null;
        }

        public bool IsFriend(Mobile m)
        {
            if (m == null || this.m_Friends == null)
                return false;

            return (this.IsCoOwner(m) || this.m_Friends.Contains(m));
        }

        public bool IsBanned(Mobile m)
        {
            if (m == null || m == this.Owner || m.IsStaff() || this.m_Bans == null)
                return false;

            Account theirAccount = m.Account as Account;

            for (int i = 0; i < this.m_Bans.Count; ++i)
            {
                Mobile c = (Mobile)this.m_Bans[i];

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

            if (m.IsStaff() || this.IsFriend(m) || (this.m_Access != null && this.m_Access.Contains(m)))
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

                    if (m.IsStaff() || this.IsFriend(m) || (this.m_Access != null && this.m_Access.Contains(m)))
                        return true;
                }
            }

            return false;
        }

        public new bool IsLockedDown(Item check)
        {
            if (check == null)
                return false;

            if (this.m_LockDowns == null)
                return false;

            return (this.m_LockDowns.Contains(check) || this.VendorRentalContracts.Contains(check));
        }

        public new bool IsSecure(Item item)
        {
            if (item == null)
                return false;

            if (this.m_Secures == null)
                return false;

            bool contains = false;

            for (int i = 0; !contains && i < this.m_Secures.Count; ++i)
                contains = (((SecureInfo)this.m_Secures[i]).Item == item);

            return contains;
        }

        public virtual Guildstone FindGuildstone()
        {
            Map map = this.Map;

            if (map == null)
                return null;

            MultiComponentList mcl = this.Components;
            IPooledEnumerable eable = map.GetItemsInBounds(new Rectangle2D(this.X + mcl.Min.X, this.Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (Item item in eable)
            {
                if (item is Guildstone && this.Contains(item))
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
        private readonly Container m_Item;
        private SecureLevel m_Level;

        public Container Item
        {
            get
            {
                return this.m_Item;
            }
        }
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }

        public SecureInfo(Container item, SecureLevel level)
        {
            this.m_Item = item;
            this.m_Level = level;
        }

        public SecureInfo(GenericReader reader)
        {
            this.m_Item = reader.ReadItem() as Container;
            this.m_Level = (SecureLevel)reader.ReadByte();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(this.m_Item);
            writer.Write((byte)this.m_Level);
        }
    }

    public class RelocatedEntity
    {
        private readonly IEntity m_Entity;
        private readonly Point3D m_RelativeLocation;

        public IEntity Entity
        {
            get
            {
                return this.m_Entity;
            }
        }

        public Point3D RelativeLocation
        {
            get
            {
                return this.m_RelativeLocation;
            }
        }

        public RelocatedEntity(IEntity entity, Point3D relativeLocation)
        {
            this.m_Entity = entity;
            this.m_RelativeLocation = relativeLocation;
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
            this.CheckLOS = false;

            this.m_Release = release;
            this.m_House = house;
        }

        protected override void OnTargetNotAccessible(Mobile from, object targeted)
        {
            this.OnTarget(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsCoOwner(from))
                return;

            if (targeted is Item)
            {
                if (this.m_Release)
                {
                    #region Mondain's legacy
                    if (targeted is AddonContainerComponent)
                    {
                        AddonContainerComponent component = (AddonContainerComponent)targeted;

                        if (component.Addon != null)
                            this.m_House.Release(from, component.Addon);
                    }
                    else
                    #endregion

                        this.m_House.Release(from, (Item)targeted);
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
                                this.m_House.LockDown(from, component.Addon);
                        }
                        else
                        #endregion

                            this.m_House.LockDown(from, (Item)targeted);
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
            this.CheckLOS = false;

            this.m_Release = release;
            this.m_House = house;
        }

        protected override void OnTargetNotAccessible(Mobile from, object targeted)
        {
            this.OnTarget(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsCoOwner(from))
                return;

            if (targeted is Item)
            {
                if (this.m_Release)
                {
                    #region Mondain's legacy
                    if (targeted is AddonContainerComponent)
                    {
                        AddonContainerComponent component = (AddonContainerComponent)targeted;

                        if (component.Addon != null)
                            this.m_House.ReleaseSecure(from, component.Addon);
                    }
                    else
                    #endregion

                        this.m_House.ReleaseSecure(from, (Item)targeted);
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
                                this.m_House.AddSecure(from, component.Addon);
                        }
                        else
                        #endregion

                            this.m_House.AddSecure(from, (Item)targeted);
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
            this.CheckLOS = false;

            this.m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsFriend(from))
                return;

            if (targeted is Mobile)
            {
                this.m_House.Kick(from, (Mobile)targeted);
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
            this.CheckLOS = false;

            this.m_House = house;
            this.m_Banning = ban;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsFriend(from))
                return;

            if (targeted is Mobile)
            {
                if (this.m_Banning)
                    this.m_House.Ban(from, (Mobile)targeted);
                else
                    this.m_House.RemoveBan(from, (Mobile)targeted);
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
            this.CheckLOS = false;

            this.m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsFriend(from))
                return;

            if (targeted is Mobile)
                this.m_House.GrantAccess(from, (Mobile)targeted);
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
            this.CheckLOS = false;

            this.m_House = house;
            this.m_Add = add;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsOwner(from))
                return;

            if (targeted is Mobile)
            {
                if (this.m_Add)
                    this.m_House.AddCoOwner(from, (Mobile)targeted);
                else
                    this.m_House.RemoveCoOwner(from, (Mobile)targeted);
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
            this.CheckLOS = false;

            this.m_House = house;
            this.m_Add = add;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!from.Alive || this.m_House.Deleted || !this.m_House.IsCoOwner(from))
                return;

            if (targeted is Mobile)
            {
                if (this.m_Add)
                    this.m_House.AddFriend(from, (Mobile)targeted);
                else
                    this.m_House.RemoveFriend(from, (Mobile)targeted);
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
            this.CheckLOS = false;

            this.m_House = house;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile)
                this.m_House.BeginConfirmTransfer(from, (Mobile)targeted);
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
            this.m_Item = item;
            this.m_Securable = securable;
        }

        public static ISecurable GetSecurable(Mobile from, Item item)
        {
            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null || !house.IsOwner(from) || !house.IsAosRules)
                return null;

            ISecurable sec = null;

            if (item is ISecurable)
            {
                bool isOwned = house.Doors.Contains(item);

                if (!isOwned)
                    isOwned = (house is HouseFoundation && ((HouseFoundation)house).IsFixture(item));

                if (!isOwned)
                    isOwned = house.IsLockedDown(item);

                if (isOwned)
                    sec = (ISecurable)item;
            }
            else
            {
                ArrayList list = house.Secures;

                for (int i = 0; sec == null && list != null && i < list.Count; ++i)
                {
                    SecureInfo si = (SecureInfo)list[i];

                    if (si.Item == item)
                        sec = si;
                }
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
            ISecurable sec = GetSecurable(this.Owner.From, this.m_Item);

            if (sec != null)
            {
                this.Owner.From.CloseGump(typeof (SetSecureLevelGump));
                this.Owner.From.SendGump(new SetSecureLevelGump(this.Owner.From, sec, BaseHouse.FindHouseAt(this.m_Item)));
            }
        }
    }

    public class TempNoHousingRegion : BaseRegion
    {
        private readonly Mobile m_RegionOwner;

        public TempNoHousingRegion(BaseHouse house, Mobile regionowner)
            : base(null, house.Map, Region.DefaultPriority, house.Region.Area)
        {
            this.Register();

            this.m_RegionOwner = regionowner;

            Timer.DelayCall(house.RestrictedPlacingTime, Unregister);
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return (from == this.m_RegionOwner || AccountHandler.CheckAccount(from, this.m_RegionOwner));
        }
    }
}