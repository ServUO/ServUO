using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Multis
{
    public interface IGalleonFixture
    {
        BaseGalleon Galleon { get; set; }
    }

    public enum SecurityLevel
    {
        NA,
        Denied,
        Passenger,
        Crewman,
        Officer,
        Captain
    }

    public enum PartyAccess
    {
        Never,
        LeaderOnly,
        MemberOnly
    }

    public abstract class BaseGalleon : BaseBoat
    {
        private SecurityEntry m_SecurityEntry;

        public List<Item> Fixtures { get; set; } = new List<Item>();
        public List<Item> Cannons { get; set; }
        public Dictionary<Item, DeckItem> Addons { get; set; }

        private Dictionary<Item, Item> _InternalCannon;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile GalleonPilot { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityEntry SecurityEntry
        {
            get
            {
                if (m_SecurityEntry == null)
                    m_SecurityEntry = new SecurityEntry(this);
                return m_SecurityEntry;
            }
            set
            {
                m_SecurityEntry = value;
                m_SecurityEntry.Galleon = this;
            }
        }

        private ShipWheel _Wheel;
        private GalleonHold _Hold;
        private BindingPole _Pole;

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipWheel Wheel => _Wheel ?? (_Wheel = Fixtures.FirstOrDefault(f => f.GetType() == typeof(ShipWheel)) as ShipWheel);

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold GalleonHold => _Hold ?? (_Hold = Fixtures.FirstOrDefault(f => f.GetType() == typeof(GalleonHold)) as GalleonHold);

        [CommandProperty(AccessLevel.GameMaster)]
        public BindingPole Pole => _Pole ?? (_Pole = Fixtures.FirstOrDefault(f => f.GetType() == typeof(BindingPole)) as BindingPole);

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile CapturedCaptain { get; set; }

        public override int LabelNumber => 1035980;  // mast

        public override bool IsClassicBoat => false;

        public virtual int MaxCannons => 0;
        public virtual int WheelDistance => 0;
        public virtual int CaptiveOffset => 0;
        public virtual int MaxAddons => 0;
        public virtual double CannonDamageMod => 1.0;

        public abstract int[] CannonTileIDs { get; }
        public abstract int[] HoldIDs { get; }
        public abstract int[] HoldItemIDs { get; }
        public abstract int[] WheelItemIDs { get; }

        public BaseGalleon(Direction direction) : base(direction, false)
        {
            m_BaseBoatHue = 0;
            AddFixtures(true);

            AddGalleonPilot(direction);
            Timer.DelayCall(TimeSpan.FromSeconds(2), MarkRunes);
        }

        private void AddFixtures(bool fromConstruct)
        {
            MultiComponentList mcl = MultiData.GetComponents(ItemID);

            foreach (MultiTileEntry mte in mcl.List.Where(e => e.m_Flags == TileFlag.None || e.m_Flags == TileFlag.Generic))
            {
                ushort itemID = mte.m_ItemID;
                short x = mte.m_OffsetX;
                short y = mte.m_OffsetY;
                short z = mte.m_OffsetZ;

                if (itemID == 0x14F8 || itemID == 0x14FA)
                {
                    AddMooringLine(itemID, x, y, z);
                }
                else if (IsMainHold(itemID))
                {
                    if (!fromConstruct)
                        continue;

                    AddMainHold(itemID, x, y, z);
                }
                else if (IsHold(itemID))
                {
                    AddHoldItem(itemID, x, y, z);
                }
                else if (IsWheel(itemID))
                {
                    if (!fromConstruct)
                        continue;

                    AddWheel(itemID, x, y, z);
                }
                else if (IsWeaponPad(itemID))
                {
                    AddWeaponPad(itemID, x, y, z);
                }
                else
                {
                    AddFillerItem(itemID, x, y, z);
                }
            }

            if (!fromConstruct && Hue != 0)
            {
                PaintComponents();
            }
        }

        protected void AddMooringLine(int id, int x, int y, int z)
        {
            MooringLine line = new MooringLine(this);
            AddFixture(line);

            line.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        protected void AddMainHold(int id, int x, int y, int z)
        {
            GalleonHold hold = new GalleonHold(this, id);
            AddFixture(hold);

            hold.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        protected void AddHoldItem(int id, int x, int y, int z)
        {
            HoldItem hold = new HoldItem(this, id);
            AddFixture(hold);

            hold.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        protected void AddWheel(int id, int x, int y, int z)
        {
            ShipWheel wheel = new ShipWheel(this, id);
            AddFixture(wheel);

            wheel.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        protected void AddWeaponPad(int id, int x, int y, int z)
        {
            WeaponPad pad = new WeaponPad(id);
            AddFixture(pad);

            pad.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        protected void AddFillerItem(int id, int x, int y, int z)
        {
            DeckItem filler = new DeckItem(id);
            AddFixture(filler);

            filler.MoveToWorld(new Point3D(X + x, Y + y, Z + z), Map);
        }

        public void AddFixture(Item item)
        {
            if (item != null && !Fixtures.Contains(item))
            {
                Fixtures.Add(item);
            }
        }

        public void RemoveFixture(Item item)
        {
            if (Fixtures != null && Fixtures.Contains(item))
            {
                Fixtures.Remove(item);
            }
        }

        protected void AddCannon(Item item)
        {
            if (Cannons == null)
            {
                Cannons = new List<Item>();
            }

            Cannons.Add(item);
        }

        private bool IsMainHold(int id)
        {
            return HoldItemIDs.Any(listID => listID == id);
        }

        private bool IsHold(int id)
        {
            return HoldIDs.Any(listID => listID == id);
        }

        private bool IsWheel(int id)
        {
            return WheelItemIDs.Any(listID => listID == id);
        }

        private bool IsWeaponPad(int id)
        {
            return CannonTileIDs.Any(listID => listID == id);
        }

        public void AddGalleonPilot(Direction direction)
        {
            int dir = GetValueForDirection(Facing);

            GalleonPilot pilot = new GalleonPilot(this)
            {
                Direction = direction
            };

            TillerMan = pilot;
            GalleonPilot = pilot;

            switch (direction)
            {
                default:
                case Direction.North:
                    pilot.Location = new Point3D(X, Y + TillerManDistance, Z + ZSurface);
                    break;
                case Direction.South:
                    pilot.Location = new Point3D(X, Y - TillerManDistance, Z + ZSurface);
                    break;
                case Direction.East:
                    pilot.Location = new Point3D(X - TillerManDistance, Y, Z + ZSurface);
                    break;
                case Direction.West:
                    pilot.Location = new Point3D(X + TillerManDistance, Y, Z + ZSurface);
                    break;
            }
        }

        public override void OnPlacement(Mobile from)
        {
            base.OnPlacement(from);

            if (GalleonPilot == null)
                return;

            switch (Facing)
            {
                default:
                case Direction.North:
                    GalleonPilot.Location = new Point3D(X, Y + TillerManDistance, Z + ZSurface);
                    break;
                case Direction.South:
                    GalleonPilot.Location = new Point3D(X, Y - TillerManDistance, Z + ZSurface);
                    break;
                case Direction.East:
                    GalleonPilot.Location = new Point3D(X - TillerManDistance, Y, Z + ZSurface);
                    break;
                case Direction.West:
                    GalleonPilot.Location = new Point3D(X + TillerManDistance, Y, Z + ZSurface);
                    break;
            }

            if (_InternalCannon != null)
            {
                foreach (KeyValuePair<Item, Item> kvp in _InternalCannon)
                {
                    Point3D p = new Point3D(kvp.Value.X, kvp.Value.Y, kvp.Value.Z + TileData.ItemTable[kvp.Value.ItemID & TileData.MaxItemValue].CalcHeight);

                    kvp.Key.MoveToWorld(p, kvp.Value.Map);
                }

                UpdateCannonIDs();

                _InternalCannon.Clear();
                _InternalCannon = null;
            }
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            return Fixtures.Any(f => f.X == x && f.Y == y);
        }

        public override bool IsExcludedTile(StaticTile tile)
        {
            return IsMastTile(tile);
        }

        public override bool IsExcludedTile(StaticTile[] tiles)
        {
            foreach (StaticTile tile in tiles)
            {
                if (!IsMastTile(tile))
                    return false;
            }
            return true;
        }

        public bool IsMastTile(StaticTile tile)
        {
            int id = tile.ID;

            if ((id >= 30150 && id <= 30193) || (id >= 30650 && id <= 30693))
                return true;

            if ((id >= 30650 && id <= 30693) || (id >= 31150 && id <= 31193))
                return true;

            if ((id >= 31150 && id <= 31193) || (id >= 31650 && id <= 31693))
                return true;

            if ((id >= 31840 && id <= 31883) || (id >= 32040 && id <= 32083))
                return true;

            if ((id >= 32240 && id <= 32283) || (id >= 32440 && id <= 32483))
                return true;

            if ((id >= 32640 && id <= 32683) || (id >= 32840 && id <= 32883))
                return true;

            if ((id >= 33040 && id <= 33083) || (id >= 33240 && id <= 33283))
                return true;

            if ((id >= 23720 && id <= 23740) || (id >= 23742 && id <= 23892))
                return true;

            if ((id >= 23894 && id <= 23902) || (id >= 23904 && id <= 23935))
                return true;

            if (id >= 25256 && id <= 25471)
                return true;

            return false;
        }

        public override bool CheckAddon(Item item)
        {
            if (Addons == null)
            {
                return false;
            }

            if (Addons.ContainsKey(item))
            {
                return true;
            }

            BaseAddon addon;

            if (item is AddonComponent)
            {
                addon = ((AddonComponent)item).Addon;
            }
            else
            {
                addon = item as BaseAddon;
            }

            return addon != null && Addons.ContainsKey(addon);
        }

        public override bool CanMoveOver(IEntity entity)
        {
            if (entity.Z <= Z && entity is Item && !((Item)entity).ItemData.Impassable && ((Item)entity).ItemData.Height < ZSurface / 2)
                return true;

            return base.CanMoveOver(entity);
        }

        public bool TryMarkRune(RecallRune rune, Mobile from)
        {
            RecallRune newRune = new RecallRune();
            newRune.SetGalleon(this);

            Container c = rune.Parent as Container;

            if (c != null)
                c.AddItem(newRune);
            else
                newRune.MoveToWorld(from.Location, from.Map);

            rune.Delete();
            return true;
        }

        public void MarkRunes()
        {
            Direction d = Facing;
            string name = Name;

            if (Owner == null || !(Owner is PlayerMobile))
                return;

            RecallRune rune = new RecallRune();
            rune.SetGalleon(this);
            DistributeRune(rune, false);

            rune = new RecallRune();
            rune.SetGalleon(this);
            DistributeRune(rune, true);
        }

        public override Point3D GetMarkedLocation()
        {
            Point3D pnt = Point3D.Zero;
            int z = ZSurface;

            switch (Facing)
            {
                case Direction.North:
                    {
                        pnt = new Point3D(X, Y + RuneOffset, z);
                        break;
                    }
                case Direction.South:
                    {
                        pnt = new Point3D(X, Y - RuneOffset, z);
                        break;
                    }
                case Direction.East:
                    {
                        pnt = new Point3D(X - RuneOffset, Y, z);
                        break;
                    }
                case Direction.West:
                    {
                        pnt = new Point3D(X + RuneOffset, Y, z);
                        break;
                    }
            }
            return pnt;
        }

        public void DistributeRune(Item rune, bool bankbox)
        {
            if (Owner != null)
            {
                if (bankbox)
                {
                    if (!Owner.BankBox.TryDropItem(Owner, rune, false))
                    {
                        GalleonHold.DropItem(rune);
                        Owner.SendLocalizedMessage(1149579); //A rune to your ship could not be created in your bank box. It has been placed in the ship's cargo hold instead.
                    }
                    else
                    {
                        Owner.SendLocalizedMessage(1149581); //A recall rune for your new ship has been placed in your bank box.
                    }
                }
                else
                {
                    if (Owner.Backpack == null || !Owner.Backpack.TryDropItem(Owner, rune, false))
                    {
                        GalleonHold.DropItem(rune);
                        Owner.SendLocalizedMessage(1149577); //A recall rune for your new ship could not be created in your backpack. It has been placed in the ship hold instead.
                    }
                    else
                    {
                        Owner.SendLocalizedMessage(1149581); //A recall rune for your new ship has been placed in your bank box.
                    }
                }
            }
            else
                rune.Delete();
        }

        public SecurityLevel GetSecurityLevel(Mobile from)
        {
            if (m_SecurityEntry == null)
                m_SecurityEntry = new SecurityEntry(this);

            if (from.AccessLevel > AccessLevel.Player || IsOwner(from))
                return SecurityLevel.Captain;

            return m_SecurityEntry.GetEffectiveLevel(from);
        }

        public bool IsPublic()
        {
            if (m_SecurityEntry == null)
                m_SecurityEntry = new SecurityEntry(this);

            return m_SecurityEntry.IsPublic;
        }

        public override bool CanCommand(Mobile m)
        {
            return GetSecurityLevel(m) >= SecurityLevel.Crewman;
        }

        public override bool HasAccess(Mobile from)
        {
            if (Owner == null || (Scuttled && IsEnemy(from))/* || (Owner is BaseCreature && !Owner.Alive)*/)
                return true;

            return GetSecurityLevel(from) > SecurityLevel.Denied;
        }

        public void InvalidateGalleon()
        {
            switch (Facing)
            {
                case Direction.North: ItemID = NorthID; break;
                case Direction.South: ItemID = SouthID; break;
                case Direction.East: ItemID = EastID; break;
                case Direction.West: ItemID = WestID; break;
            }

            SetFacingComponents(Facing, Facing, true);
        }

        public void AutoAddCannons(Mobile captain)
        {
            bool heavy = Utility.RandomBool();

            foreach (WeaponPad pad in Fixtures.OfType<WeaponPad>())
            {
                if (pad.Map != Map.Internal && !pad.Deleted)
                {
                    IShipCannon cannon;

                    if (heavy)
                    {
                        cannon = new Carronade(this);
                    }
                    else
                    {
                        cannon = new Culverin(this);
                    }

                    if (!TryAddCannon(captain, pad.Location, cannon, null))
                    {
                        cannon.Delete();
                    }
                }
            }
        }

        public bool TryAddCannon(Mobile from, Point3D pnt, ShipCannonDeed deed, bool force = false)
        {
            if (!IsNearLandOrDocks(this) && !force)
            {
                if (from != null)
                {
                    from.SendLocalizedMessage(1116076); // The ship must be near shore or a sea market to deploy this weapon.
                }
            }
            else
            {
                IShipCannon cannon;

                switch (deed.CannonType)
                {
                    default:
                    case CannonPower.Pumpkin:
                        cannon = new PumpkinCannon(this);
                        break;
                    case CannonPower.Light:
                        cannon = new Culverin(this);
                        break;
                    case CannonPower.Heavy:
                        cannon = new Carronade(this);
                        break;
                    case CannonPower.Massive:
                        cannon = new Blundercannon(this);
                        break;
                }

                return TryAddCannon(from, pnt, cannon, deed);
            }

            return false;
        }

        public bool TryAddCannon(Mobile from, Point3D pnt, IShipCannon cannon, ShipCannonDeed deed)
        {
            if (cannon == null || !(cannon is Item))
            {
                return false;
            }

            if (IsValidCannonSpot(ref pnt, from))
            {
                ((Item)cannon).MoveToWorld(pnt, Map);
                AddCannon((Item)cannon);
                UpdateCannonID((Item)cannon);
                cannon.Position = GetCannonPosition(pnt);

                if (from != null)
                {
                    cannon.DoAreaMessage(1116074, 10, from); //~1_NAME~ deploys a ship cannon.
                }

                if (from != null && from.NetState != null)
                {
                    Timer.DelayCall(() =>
                    {
                        from.ClearScreen();
                        from.SendEverything();
                    });
                }

                if (deed != null && (from == null || from.AccessLevel == AccessLevel.Player))
                {
                    deed.Delete();
                }

                return true;
            }

            cannon.Delete();
            return false;
        }

        public void RemoveCannon(Item cannon)
        {
            if (Cannons != null && Cannons.Contains(cannon))
            {
                Cannons.Remove(cannon);
            }
        }

        public bool IsValidCannonSpot(ref Point3D pnt, Mobile from)
        {
            if (Map == null || Map == Map.Internal)
                return false;

            //Lets see if a cannon exists here
            if (Cannons != null)
            {
                foreach (Item cannon in Cannons)
                {
                    if (cannon.X == pnt.X && cannon.Y == pnt.Y)
                    {
                        if (from != null)
                            from.SendLocalizedMessage(1116075); //There is already a weapon deployed here.

                        return false;
                    }
                }
            }

            //Now we can check for a valid cannon tile ID
            foreach (WeaponPad pad in Fixtures.OfType<WeaponPad>())
            {
                if (pad.X == pnt.X && pad.Y == pnt.Y)
                {
                    pnt.Z = pad.Z + TileData.ItemTable[pad.ItemID & TileData.MaxItemValue].CalcHeight;
                    IPooledEnumerable eable = Map.GetMobilesInRange(pnt, 0);

                    //Lets check for mobiles
                    foreach (Mobile mob in eable)
                    {
                        if (!mob.Hidden && mob.AccessLevel == AccessLevel.Player)
                        {
                            if (from != null)
                                from.SendMessage("The weapon pad must be clear of obstructions to place a cannon.");

                            eable.Free();
                            return false;
                        }
                    }

                    eable.Free();
                    return true;
                }
            }

            if (from != null)
                from.SendLocalizedMessage(1116626); //You must use this on a ship weapon pad.

            return false;
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            foreach (Item fixture in Fixtures)
            {
                fixture.Location = new Point3D(X + (fixture.X - old.X), Y + (fixture.Y - old.Y), Z + (fixture.Z - old.Z));
            }

            if (Addons != null)
            {
                foreach (Item addon in Addons.Keys)
                {
                    addon.Location = new Point3D(X + (addon.X - old.X), Y + (addon.Y - old.Y), Z + (addon.Z - old.Z));
                }
            }

            if (Cannons != null)
            {
                foreach (Item cannon in Cannons)
                {
                    cannon.Location = new Point3D(X + (cannon.X - old.X), Y + (cannon.Y - old.Y), Z + (cannon.Z - old.Z));
                }
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            foreach (Item fixture in Fixtures)
            {
                fixture.Map = Map;
            }

            if (Addons != null)
            {
                foreach (Item addon in Addons.Keys)
                {
                    addon.Map = Map;
                }
            }

            if (Cannons != null)
            {
                foreach (Item cannon in Cannons)
                {
                    cannon.Map = Map;
                }
            }
        }

        public override TimeSpan GetMovementInterval(bool fast, out int clientSpeed)
        {
            if (DamageTaken < DamageLevel.Heavily)
                return base.GetMovementInterval(fast, out clientSpeed);

            if (fast)
            {
                clientSpeed = 0x3;
                return FastDriftInterval;
            }

            clientSpeed = 0x2;
            return SlowDriftInterval;
        }

        public override bool IsComponentItem(IEntity entity)
        {
            if (!(entity is Item))
                return false;

            Item item = (Item)entity;

            if (Fixtures.Contains(item))
            {
                return true;
            }

            if (Addons != null)
            {
                if (item is BaseAddon && Addons.ContainsKey(item))
                {
                    return true;
                }
                else if (item is AddonComponent && ((AddonComponent)item).Addon != null && Addons.ContainsKey(((AddonComponent)item).Addon))
                {
                    return true;
                }
            }

            if (Cannons != null && Cannons.Contains(item))
            {
                return true;
            }

            return base.IsComponentItem(entity);
        }

        public override void OnAfterDelete()
        {
            List<Item> list = new List<Item>(Fixtures.Where(f => !f.Deleted));

            foreach (Item fixture in list)
            {
                fixture.Delete();
            }

            if (Cannons != null)
            {
                list = new List<Item>(Cannons);

                foreach (Item cannon in list)
                {
                    cannon.Delete();
                }

                ColUtility.Free(Cannons);
            }

            if (Addons != null)
            {
                list = new List<Item>(Addons.Keys.Where(a => a != null && !a.Deleted));

                foreach (Item addon in list)
                {
                    addon.Delete();
                }

                Addons.Clear();
            }

            ColUtility.Free(list);
            ColUtility.Free(Fixtures);

            if (CapturedCaptain != null)
            {
                CapturedCaptain.Kill();
            }

            base.OnAfterDelete();
        }

        public override DryDockResult CheckDryDock(Mobile from, Mobile dockmaster)
        {
            if (this is BaseGalleon && GalleonHold.Items.Count > 0)
                return DryDockResult.Hold;

            Container pack = from.Backpack;

            if (dockmaster != null && pack != null && pack.GetAmount(typeof(Gold)) < DockMaster.DryDockAmount && Banker.GetBalance(from) < DockMaster.DryDockAmount)
                return DryDockResult.NotEnoughGold;

            if (DamageTaken != DamageLevel.Pristine)
                return DryDockResult.Damaged;

            if (Cannons != null && Cannons.Count > 0)
            {
                foreach (IShipCannon cannon in Cannons.OfType<IShipCannon>())
                {
                    if (cannon == null)
                        continue;

                    if (cannon.AmmoType != AmmunitionType.Empty)
                        return DryDockResult.Cannon;
                }
            }

            return base.CheckDryDock(from, dockmaster);
        }

        public override void OnDryDock(Mobile from)
        {
            if (Cannons != null)
            {
                if (_InternalCannon == null)
                    _InternalCannon = new Dictionary<Item, Item>();

                Cannons.ForEach(c =>
                {
                    Item pad = Fixtures.OfType<WeaponPad>().FirstOrDefault(p => p.X == c.X && p.Y == c.Y);

                    if (pad != null)
                        _InternalCannon[c] = pad;
                });
            }

            base.OnDryDock(from);
        }

        public override void SetFacingComponents(Direction newDirection, Direction oldDirection, bool ignoreLastDirection)
        {
            if (oldDirection == newDirection && !ignoreLastDirection)
                return;

            MultiComponentList mcl = MultiData.GetComponents(ItemID);

            foreach (MultiTileEntry mte in mcl.List.Where(e => e.m_Flags == TileFlag.None))
            {
                foreach (Item fixture in Fixtures.Where(f => f.X - X == mte.m_OffsetX && f.Y - Y == mte.m_OffsetY && f.Z - Z == mte.m_OffsetZ))
                {
                    fixture.ItemID = mte.m_ItemID;
                }
            }

            if (Addons != null)
            {
                foreach (Item addon in Addons.Keys)
                {
                    DeckItem tile = Addons[addon];

                    addon.MoveToWorld(new Point3D(tile.X, tile.Y, tile.Z + tile.ItemData.Height), Map);
                }
            }

            UpdateCannonIDs();
        }

        public override IEnumerable<IEntity> GetComponents()
        {
            foreach (Item fixture in Fixtures)
            {
                yield return fixture;
            }

            if (GalleonPilot != null)
            {
                yield return GalleonPilot;
            }
        }

        public int GetValueForDirection(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.South: return 0;
                case Direction.West: return 1;
                case Direction.North: return 2; ;
                case Direction.East: return 3;
            }
        }

        public void UpdateCannonIDs()
        {
            if (Cannons != null)
            {
                foreach (Item cannon in Cannons)
                {
                    UpdateCannonID(cannon);
                }
            }
        }

        public void UpdateCannonID(Item cannon)
        {
            if (cannon == null)
                return;

            int type = cannon is PumpkinCannon ? 3 : cannon is Blundercannon ? 2 : cannon is LightShipCannon || cannon is Culverin ? 0 : 1;

            switch (Facing)
            {
                default:
                case Direction.South:
                case Direction.North:
                    {
                        if (cannon.X == X)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Facing)][type];
                        else if (cannon.X < X)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.West)][type];
                        else
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.East)][type];
                        break;
                    }
                case Direction.West:
                case Direction.East:
                    {
                        if (cannon.Y == Y)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Facing)][type];
                        else if (cannon.Y < Y)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.North)][type];
                        else
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.South)][type];
                        break;
                    }
            }
        }

        public static int[][] CannonIDs => m_CannonIDs;
        private static readonly int[][] m_CannonIDs = new int[][]
        { 
                      //Light  Heavy, Blunder, Pumpkin
            new int[] { 16918, 16922, 41664, 41979 }, //South
            new int[] { 16919, 16923, 41665, 41980 }, //West
            new int[] { 16920, 16924, 41666, 41981 }, //North
            new int[] { 16921, 16925, 41667, 41982 }, //East
        };

        public virtual ShipPosition GetCannonPosition(Point3D pnt)
        {
            return ShipPosition.Bow;
        }

        #region Painting
        private int m_BaseBoatHue;
        private int m_BasePaintHue;
        private int m_PaintCoats;
        private DateTime m_NextPaintDecay;

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseBoatHue
        {
            get { return m_BaseBoatHue; }
            set
            {
                m_BaseBoatHue = value;

                if (m_PaintCoats == 0)
                {
                    Hue = m_BaseBoatHue;
                    PaintComponents();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasPaint => m_BasePaintHue != 0;

        private static readonly TimeSpan DecayPeriod = TimeSpan.FromDays(14);
        private static readonly int MaxPaintCoats = 4;

        public bool TryPermanentPaintBoat(Mobile from, int hue)
        {
            if (m_PaintCoats > 0)
                from.SendLocalizedMessage(1149826); //You cannot apply this paint to your ship while it has other paints on it. Please use paint remover to restore your ship to its permanent base color and then try again.
            else
            {
                m_BaseBoatHue = hue;
                Hue = hue;

                from.SendLocalizedMessage(1116775); //You apply the permanent paint to your ship.  This is now the base color of your ship.
                PaintComponents();
                return true;
            }
            return false;
        }

        public bool TryPaintBoat(Mobile from, int baseHue)
        {
            if (m_PaintCoats > 0 && m_BasePaintHue != baseHue)
                from.SendLocalizedMessage(1149826); //You cannot apply this paint to your ship while it has other paints on it. Please use paint remover to restore your ship to its permanent base color and then try again.
            else
            {
                if (Hue == m_BaseBoatHue)
                    m_BasePaintHue = baseHue;

                if (m_PaintCoats < MaxPaintCoats)
                {
                    m_PaintCoats++;
                    m_NextPaintDecay = DateTime.UtcNow + DecayPeriod;

                    if (baseHue == 1900 || baseHue == 2213)
                        Hue = baseHue += m_PaintCoats;
                    else
                        Hue = baseHue -= m_PaintCoats;

                    PaintComponents();
                    from.SendLocalizedMessage(1116771); //You apply a fresh coat of paint to your ship.
                    return true;
                }
                else
                    from.SendLocalizedMessage(1116774); //You have reached this paint color's maximum intensity.
            }
            return false;
        }

        public void PaintComponents()
        {
            foreach (Item fixture in Fixtures.Where(f =>
                f.GetType() != typeof(MooringLine) &&
                f.GetType() != typeof(ShipWheel)))
            {
                fixture.Hue = Hue;
            }

            if (SecureContainer != null)
                SecureContainer.Hue = Hue;
        }

        public void CheckPaintDecay()
        {
            if (m_BasePaintHue != m_BaseBoatHue && m_NextPaintDecay <= DateTime.UtcNow)
            {
                m_PaintCoats--;
                m_NextPaintDecay = DateTime.UtcNow + DecayPeriod;

                if (m_PaintCoats <= 0)
                    RemovePaint();
                else
                {
                    if (m_BasePaintHue == 1900 || m_BasePaintHue == 2213)
                        Hue = m_BasePaintHue -= m_PaintCoats;
                    else
                        Hue = m_BasePaintHue += m_PaintCoats;
                }

                PaintComponents();
            }
        }

        public bool RemovePaint()
        {
            m_BasePaintHue = 0;
            m_PaintCoats = 0;

            if (Hue == m_BaseBoatHue)
                return false;

            Hue = m_BaseBoatHue;
            PaintComponents();
            return true;
        }
        #endregion

        #region Static Methods
        public static BaseGalleon FindGalleonAt(IPoint2D pnt, Map map)
        {
            BaseBoat boat = FindBoatAt(pnt, map);

            if (boat is BaseGalleon)
                return boat as BaseGalleon;

            return null;
        }

        public static bool CheckForBoat(IPoint3D p, Mobile caster)
        {
            BaseBoat boat = FindBoatAt(caster, caster.Map);
            BaseGalleon galleon = FindGalleonAt(p, caster.Map);

            if (galleon == null || caster.AccessLevel > AccessLevel.Player)
                return false;

            if (galleon.Scuttled || galleon.GetSecurityLevel(caster) >= SecurityLevel.Crewman)
                return false;

            return true;
        }

        /*public static void CloseHold(Mobile from)
        {
            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);

            if (boat != null && boat is BaseGalleon)
            {
                GalleonHold hold = ((BaseGalleon)boat).GalleonHold;

                if (hold != null && hold.Viewers != null && hold.Viewers.Count > 0 && hold.Viewers.Contains(from))
                    hold.Close(from);
            }
        }*/

        public static bool IsNearLandOrDocks(BaseBoat boat)
        {
            return IsNearLand(boat) || IsNearDocks(boat);
        }

        public static bool IsNearLand(BaseBoat boat)
        {
            return IsNearLand(boat, 12);
        }

        public static bool IsNearLand(BaseBoat boat, int range)
        {
            if (boat == null)
                return false;

            Map map = boat.Map;

            for (int x = boat.X - range; x <= boat.X + range; x++)
            {
                for (int y = boat.Y - range; y <= boat.Y + range; y++)
                {
                    LandTile lt = map.Tiles.GetLandTile(x, y);

                    TileFlag landFlags = TileData.LandTable[lt.ID & TileData.MaxLandValue].Flags;

                    if ((landFlags & TileFlag.Impassable) == 0)
                        return true;
                }
            }
            return false;
        }

        public static bool IsNearDocks(BaseBoat boat)
        {
            return IsNearDocks(boat, 12);
        }

        public static bool IsNearDocks(BaseBoat boat, int range)
        {
            if (boat == null)
                return false;

            Map map = boat.Map;

            for (int x = boat.X - range; x <= boat.X + range; x++)
            {
                for (int y = boat.Y - range; y <= boat.Y + range; y++)
                {
                    StaticTile[] staticTiles = map.Tiles.GetStaticTiles(x, y, true);

                    for (int i = 0; i < staticTiles.Length; i++)
                    {
                        ItemData id = TileData.ItemTable[staticTiles[i].ID & TileData.MaxItemValue];

                        if (id.Name != null && (id.Name.ToLower() == "wooden plank" || id.Name.ToLower() == "pier"))
                            return true;
                    }
                }
            }
            return false;
        }
        #endregion

        public Point3D GetRecallOffset()
        {
            switch (Facing)
            {
                default:
                case Direction.North: return new Point3D(X, Y + RuneOffset, GalleonPilot.Z);
                case Direction.South: return new Point3D(X, Y - RuneOffset, GalleonPilot.Z);
                case Direction.East: return new Point3D(X + RuneOffset, Y, GalleonPilot.Z);
                case Direction.West: return new Point3D(X - RuneOffset, Y, GalleonPilot.Z);
            }
        }

        #region Addons
        public static int[] ShipAddonTiles => m_ShipAddonTiles;
        private static readonly int[] m_ShipAddonTiles =
            {23664, 23665, 23718, 23719, 23610, 23611, 23556, 23557, 23664, 23665, 23718, 23719, 23610, 23611, 23556, 23557};

        public bool CanAddAddon(Point3D p)
        {
            if ((Addons != null && Addons.Count >= MaxAddons) || Map == null || Map == Map.Internal)
                return false;

            IPooledEnumerable eable = Map.GetItemsInRange(p, 0);

            foreach (DeckItem item in eable.OfType<DeckItem>())
            {
                if (m_ShipAddonTiles.Any(id => id == item.ItemID) && (Addons == null || !Addons.ContainsValue(item)))
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public void AddAddon(Item addon)
        {
            if (Addons == null)
            {
                Addons = new Dictionary<Item, DeckItem>();
            }

            IPooledEnumerable eable = Map.GetItemsInRange(addon.Location, 0);

            foreach (DeckItem item in eable.OfType<DeckItem>())
            {
                if (m_ShipAddonTiles.Any(id => id == item.ItemID))
                {
                    Addons[addon] = item;
                }
            }

            eable.Free();
        }

        public void RemoveAddon(Item item)
        {
            if (Addons.ContainsKey(item))
            {
                Addons.Remove(item);
            }
        }

        public void OnChop(BaseAddon addon, Mobile from)
        {
            if (addon == null || from == null || !Contains(addon) || !Contains(from))
                return;

            Effects.PlaySound(addon.GetWorldLocation(), addon.Map, 0x3B3);
            from.SendLocalizedMessage(500461); // You destroy the item.

            int hue = 0;

            if (addon.RetainDeedHue)
            {
                for (int i = 0; hue == 0 && i < addon.Components.Count; ++i)
                {
                    AddonComponent c = addon.Components[i];

                    if (c.Hue != 0)
                        hue = c.Hue;
                }
            }

            addon.Delete();
            RemoveAddon(addon);
            BaseAddonDeed deed = addon.Deed;

            if (deed != null)
            {
                deed.Resource = addon.Resource;

                if (addon.RetainDeedHue)
                    deed.Hue = hue;

                from.AddToBackpack(deed);
            }

        }
        #endregion

        public BaseGalleon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(7);

            writer.Write(_InternalCannon == null ? 0 : _InternalCannon.Count);

            if (_InternalCannon != null)
            {
                foreach (KeyValuePair<Item, Item> kvp in _InternalCannon)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }

            writer.WriteItemList(Fixtures, true);

            writer.Write(CapturedCaptain);

            writer.Write(m_BaseBoatHue);

            writer.Write(GalleonPilot);

            writer.Write(m_BasePaintHue);
            writer.Write(m_PaintCoats);
            writer.Write(m_NextPaintDecay);

            SecurityEntry.Serialize(writer);

            writer.Write(Cannons != null ? Cannons.Count : 0);

            if (Cannons != null)
            {
                for (int i = 0; i < Cannons.Count; i++)
                    writer.Write(Cannons[i]);
            }

            writer.Write(Addons != null ? Addons.Count : 0);

            if (Addons != null)
            {
                foreach (KeyValuePair<Item, DeckItem> kvp in Addons)
                {
                    writer.Write(kvp.Key);
                    writer.WriteItem(kvp.Value);
                }
            }

            Timer.DelayCall(TimeSpan.FromMinutes(1), CheckPaintDecay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 7:
                    int c = reader.ReadInt();

                    if (c > 0)
                    {
                        if (_InternalCannon == null)
                        {
                            _InternalCannon = new Dictionary<Item, Item>();
                        }

                        for (int i = 0; i < c; i++)
                        {
                            Item cannon = reader.ReadItem();
                            Item pad = reader.ReadItem();

                            if (cannon != null && pad != null)
                            {
                                _InternalCannon[cannon] = pad;
                            }
                        }
                    }

                    goto case 6;
                case 6:
                    Fixtures = reader.ReadStrongItemList();
                    goto case 5;
                case 5:
                case 4:
                case 3:
                case 2:
                    if (version < 6)
                    {
                        Item pole = reader.ReadItem();
                        AddFixture(pole);
                    }

                    CapturedCaptain = reader.ReadMobile();
                    goto case 1;
                case 1:
                    m_BaseBoatHue = reader.ReadInt();
                    goto case 0;
                case 0:
                    GalleonPilot = reader.ReadMobile();

                    if (version < 5)
                    {
                        m_Hits = reader.ReadInt();
                    }

                    #region Version 5
                    if (version < 6)
                    {
                        Item wheel = reader.ReadItem();
                        Item hold = reader.ReadItem();

                        AddFixture(wheel);
                        AddFixture(hold);
                    }
                    #endregion

                    if (version < 3)
                    {
                        reader.ReadItem();
                        reader.ReadItem();
                    }

                    if (version < 5)
                    {
                        m_DamageTaken = (DamageLevel)reader.ReadInt();
                    }

                    m_BasePaintHue = reader.ReadInt();
                    m_PaintCoats = reader.ReadInt();
                    m_NextPaintDecay = reader.ReadDateTime();

                    m_SecurityEntry = new SecurityEntry(this, reader);
                    int count;

                    #region Version 5
                    if (version < 6)
                    {
                        count = reader.ReadInt();
                        List<Item> pads = new List<Item>();

                        for (int i = 0; i < count; i++)
                        {
                            Item weaponPad = reader.ReadItem();
                            pads.Add(weaponPad);
                        }

                        Timer.DelayCall(() => pads.ForEach(p => p.Delete()));
                    }
                    #endregion

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Item cannon = reader.ReadItem();
                        if (cannon != null && !cannon.Deleted)
                            AddCannon(cannon);
                    }

                    #region Version 5
                    if (version < 6)
                    {
                        count = reader.ReadInt();
                        List<Item> list = new List<Item>();

                        for (int i = 0; i < count; i++)
                        {
                            Item filler = reader.ReadItem();
                            list.Add(filler);
                        }

                        count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Item line = reader.ReadItem();
                            list.Add(line);
                        }

                        count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Item hItem = reader.ReadItem();
                            list.Add(hItem);
                        }

                        Timer.DelayCall(() => list.ForEach(i => i.Delete()));
                    }
                    #endregion

                    count = reader.ReadInt();

                    #region Version 5
                    if (version < 6)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Item addon = reader.ReadItem();

                            if (addon != null && !addon.Deleted)
                                Timer.DelayCall(() => AddAddon(addon));
                        }
                    }
                    #endregion
                    else
                    {
                        if (count > 0 && Addons == null)
                        {
                            Addons = new Dictionary<Item, DeckItem>();
                        }

                        for (int i = 0; i < count; i++)
                        {
                            Item addon = reader.ReadItem();
                            DeckItem tile = reader.ReadItem<DeckItem>();

                            if (addon != null)
                            {
                                Addons[addon] = tile;
                            }
                        }
                    }

                    #region Version 5
                    if (version < 6)
                    {
                        count = reader.ReadInt();
                        List<Item> list = new List<Item>();

                        for (int i = 0; i < count; i++)
                        {
                            Item atile = reader.ReadItem();
                            list.Add(atile);
                        }

                        Timer.DelayCall(() => list.ForEach(i => i.Delete()));
                    }
                    #endregion
                    break;
            }

            if (version < 6)
            {
                AddFixtures(false);
            }
            else
            {
                foreach (IGalleonFixture fixture in Fixtures.OfType<IGalleonFixture>())
                {
                    fixture.Galleon = this;
                }
            }
        }
    }

    [PropertyObject]
    public class SecurityEntry
    {
        private readonly SecurityLevel DefaultImpliedAccessLevel = SecurityLevel.Passenger;
        private readonly Dictionary<Mobile, SecurityLevel> m_Manifest;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public PartyAccess PartyAccess { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultPublicAccess { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultPartyAccess { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultGuildAccess { get; set; }

        public Dictionary<Mobile, SecurityLevel> Manifest => m_Manifest;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPublic => DefaultPublicAccess != SecurityLevel.Denied;

        public SecurityEntry(BaseGalleon galleon)
        {
            Galleon = galleon;
            m_Manifest = new Dictionary<Mobile, SecurityLevel>();

            AddToManifest(Galleon.Owner, SecurityLevel.Captain);
            SetToDefault();
        }

        public void AddToManifest(Mobile from, SecurityLevel access)
        {
            if (from == null || m_Manifest == null)
                return;

            m_Manifest[from] = access;
        }

        public void RemoveFromAccessList(Mobile from)
        {
            if (!m_Manifest.ContainsKey(from))
                return;

            m_Manifest.Remove(from);
        }

        public bool HasImpliedAccess(Account acct, Mobile from)
        {
            for (int i = 0; i < acct.Length; ++i)
            {
                Mobile m = acct[i];

                if (m == from)
                    continue;

                if (GetEffectiveLevel(m, false) > SecurityLevel.Denied)
                    return true;
            }

            return false;
        }

        public SecurityLevel GetImpliedAccess(Mobile from)
        {
            if (from == null)
                return SecurityLevel.Denied;

            Account acct = from.Account as Account;

            if (acct != null)
            {
                if (HasImpliedAccess(acct, from))
                    return DefaultImpliedAccessLevel;
            }

            return SecurityLevel.Denied;
        }

        public SecurityLevel GetEffectiveLevel(Mobile from)
        {
            return GetEffectiveLevel(from, true);
        }

        public SecurityLevel GetEffectiveLevel(Mobile from, bool checkImplied)
        {
            if (from == null)
                return SecurityLevel.Denied;

            //Owner is always a captain!
            if (from == Galleon.Owner)
                return SecurityLevel.Captain;

            SecurityLevel highest = SecurityLevel.Denied;

            if (m_Manifest.ContainsKey(from))
            {
                if (m_Manifest[from] == highest) //denied
                    return highest;

                highest = m_Manifest[from];
            }

            if (highest < DefaultPublicAccess)
                highest = DefaultPublicAccess;

            if (IsInParty(from) && highest < DefaultPartyAccess)
                highest = DefaultPartyAccess;

            if (IsInGuild(from) && highest < DefaultGuildAccess)
                highest = DefaultGuildAccess;

            if (checkImplied && highest == SecurityLevel.Denied)
                highest = GetImpliedAccess(from);

            return highest;
        }

        public bool IsInParty(Mobile from)
        {
            if (from == null || Galleon == null || Galleon.Owner == null)
                return false;

            Party fromParty = Party.Get(from);
            Party ownerParty = Party.Get(Galleon.Owner);

            if (fromParty == null || ownerParty == null)
                return false;

            if (fromParty == ownerParty)
            {
                switch (PartyAccess)
                {
                    case PartyAccess.Never: return false;
                    case PartyAccess.LeaderOnly: return ownerParty.Leader == Galleon.Owner;
                    case PartyAccess.MemberOnly: return true;
                }
            }
            return false;
        }

        public bool IsInGuild(Mobile from)
        {
            if (from == null || Galleon == null || Galleon.Owner == null)
                return false;

            Guild fromGuild = from.Guild as Guild;
            Guild ownerGuild = Galleon.Owner.Guild as Guild;

            return fromGuild != null && ownerGuild != null && fromGuild == ownerGuild;
        }

        public void SetToDefault()
        {
            PartyAccess = PartyAccess.Never;
            DefaultPublicAccess = SecurityLevel.NA;
            DefaultPartyAccess = SecurityLevel.NA;
            DefaultGuildAccess = SecurityLevel.NA;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)PartyAccess);
            writer.Write((int)DefaultPublicAccess);
            writer.Write((int)DefaultPartyAccess);
            writer.Write((int)DefaultGuildAccess);

            writer.Write(m_Manifest.Count);
            foreach (KeyValuePair<Mobile, SecurityLevel> kvp in m_Manifest)
            {
                writer.Write(kvp.Key);
                writer.Write((int)kvp.Value);
            }
        }

        public SecurityEntry(BaseGalleon galleon, GenericReader reader)
        {
            m_Manifest = new Dictionary<Mobile, SecurityLevel>();
            Galleon = galleon;

            int version = reader.ReadInt();

            PartyAccess = (PartyAccess)reader.ReadInt();
            DefaultPublicAccess = (SecurityLevel)reader.ReadInt();
            DefaultPartyAccess = (SecurityLevel)reader.ReadInt();
            DefaultGuildAccess = (SecurityLevel)reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile();
                SecurityLevel sl = (SecurityLevel)reader.ReadInt();

                AddToManifest(mob, sl);
            }
        }

        public override string ToString()
        {
            return "...";
        }
    }

    public class ShipAccessEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Clicker;
        private readonly BaseGalleon m_Galleon;

        public ShipAccessEntry(Mobile from, Mobile clicker, BaseGalleon galleon)
            : base(1116566, 15)
        {
            m_From = from;
            m_Clicker = clicker;
            m_Galleon = galleon;
        }

        public override void OnClick()
        {
            if (m_From == null || m_Galleon == null || m_Galleon.Deleted || m_Galleon.SecurityEntry == null || m_Clicker == null)
                return;

            m_Clicker.CloseGump(typeof(BaseShipGump));

            if (!m_Galleon.SecurityEntry.Manifest.ContainsKey(m_From))
                m_Galleon.SecurityEntry.AddToManifest(m_From, SecurityLevel.NA);

            m_Clicker.SendGump(new GrantAccessGump(m_From, m_Galleon));
        }
    }
}
