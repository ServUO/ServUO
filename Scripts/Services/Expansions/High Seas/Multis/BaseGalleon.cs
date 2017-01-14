using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Guilds;
using System.Collections.Generic;
using Server.Accounting;
using Server.Engines.PartySystem;
using Server.ContextMenus;
using Server.Gumps;
using Server.Network;
using System.Linq;

namespace Server.Multis
{
    public enum DamageLevel
    {
        Pristine = 0,
        Slightly = 1,
        Moderately = 2,
        Heavily = 3,
        Severely = 4
    }

    public enum SecurityLevel
    {
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
        private Mobile m_GalleonPilot;
        private SecurityEntry m_SecurityEntry;
        private DamageLevel m_DamageTaken;
        private int m_Hits;
        private ShipWheel m_Wheel;
        private GalleonHold m_GalleonHold;

        private BindingPole m_Pole;
        private Mobile m_CapturedCaptain;

        private Mobile m_Captive;
        private ShippingCrate m_ShippingCrate;

        private List<Item> m_MooringLines = new List<Item>();
        private List<Item> m_Cannons = new List<Item>();
        private List<Item> m_CannonTiles = new List<Item>();
        private List<Item> m_FillerTiles = new List<Item>();
        private List<Item> m_HoldTiles = new List<Item>();
        private List<Item> m_Addons = new List<Item>();
        private List<Item> m_AddonTiles = new List<Item>();

        private Dictionary<Item, Item> _InternalCannon;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile GalleonPilot { get { return m_GalleonPilot; } }

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

        [CommandProperty(AccessLevel.GameMaster)]
        public DamageLevel DamageTaken
        { 
            get { return m_DamageTaken; }
            set 
            {
                DamageLevel oldDamage = m_DamageTaken;
                
                m_DamageTaken = value;

                if (m_DamageTaken != oldDamage)
                {
                    InvalidateGalleon();

                    if (m_GalleonPilot != null)
                    {
                        m_GalleonPilot.InvalidateProperties();

                        if (m_DamageTaken == DamageLevel.Severely)
                            m_GalleonPilot.Say(1116687); //Arr, we be scuttled!
                    }
                }
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits { get { return m_Hits; } set { m_Hits = value; ComputeDamage(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipWheel Wheel { get { return m_Wheel; } set { m_Wheel = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold GalleonHold { get { return m_GalleonHold; } set { m_GalleonHold = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BindingPole Pole { get { return m_Pole; } set { m_Pole = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile CapturedCaptain { get { return m_CapturedCaptain; } set { m_CapturedCaptain = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Durability { get { return ((double)m_Hits / (double)MaxHits) * 100.0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Scuttled { get { return Durability < ScuttleLevel; } }

        public override int LabelNumber { get { return 1035980; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxAddons { get { return m_AddonTiles.Count; } }

        public List<Item> Addons { get { return m_Addons; } }
        public List<Item> Cannons { get { return m_Cannons; } }

        public override bool IsClassicBoat { get { return false; } }
        public override TimeSpan BoatDecayDelay { get { return TimeSpan.FromDays(21); } }

        public virtual int DamageValue
        {
            get
            {
                switch (m_DamageTaken)
                {
                    default:
                    case DamageLevel.Pristine:
                    case DamageLevel.Slightly: return 0;
                    case DamageLevel.Moderately:
                    case DamageLevel.Heavily: return  1;
                    case DamageLevel.Severely: return 2;
                }
            }
        }

        public virtual int MaxCannons { get { return 0; } }
        public virtual int WheelDistance { get { return 0;} }
        public virtual int CaptiveOffset { get { return 0; } }
        public virtual double CannonDamageMod { get { return 1.0; } }
        public virtual int MaxHits { get { return 100; } }
        public virtual double ScuttleLevel { get { return 33.0; } }
        public virtual int RuneOffset { get { return 0; } }
        public virtual int ZSurface { get { return 0; } }

        public abstract void AddMooringLines(Direction d);
        public abstract void AddCannonTiles(Direction d);
        public abstract void AddHoldTiles(Direction d);
        public abstract int[][] CannonTileIDs { get; }
        public abstract int[][] FillerIDs { get; }
        public abstract int[][] HoldIDs { get; }
        public abstract int[][] HoldItemIDs { get; }
        public abstract int[][] WheelItemIDs { get; }

        public BaseGalleon(Direction direction) : base(direction, false)
        {
            m_Hits = MaxHits;
            m_DamageTaken = DamageLevel.Pristine;
            m_BaseBoatHue = 0;

            AddMooringLines(direction);
            AddCannonTiles(direction);
            AddHoldTiles(direction);

            AddGalleonPilotAndWheel(direction);
            Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerCallback(MarkRunes));
        }

        public void AddGalleonPilotAndWheel(Direction direction)
        {
            int dir = GetValueForDirection(this.Facing);

            ShipWheel wheel = new ShipWheel(this);
            wheel.ItemID = WheelItemIDs[dir][0];

            GalleonPilot pilot = new GalleonPilot(this);
            pilot.Direction = direction;

            m_Wheel = wheel;
            TillerMan = pilot;
            m_GalleonPilot = pilot;

            switch (direction)
            {
                default:
                case Direction.North:
                    pilot.Location = new Point3D(X, Y + TillerManDistance, Z + ZSurface);
                    wheel.Location = new Point3D(X, Y + WheelDistance, Z + 1);
                    break;
                case Direction.South:
                    pilot.Location = new Point3D(X, Y - TillerManDistance, Z + ZSurface);
                    wheel.Location = new Point3D(X, Y - WheelDistance, Z + 1);
                    break;
                case Direction.East:
                    pilot.Location = new Point3D(X - TillerManDistance, Y, Z + ZSurface);
                    wheel.Location = new Point3D(X - WheelDistance, Y, Z + 1);
                    break;
                case Direction.West:
                    pilot.Location = new Point3D(X + TillerManDistance, Y, Z + ZSurface);
                    wheel.Location = new Point3D(X + WheelDistance, Y, Z + 1);
                    break;
            }
        }

        public override void OnPlacement(Mobile from)
        {
            base.OnPlacement(from);

            if (m_GalleonPilot == null)
                return;

            switch (Facing)
            {
                default:
                case Direction.North:
                    m_GalleonPilot.Location = new Point3D(X, Y + TillerManDistance, Z + ZSurface);
                    break;
                case Direction.South:
                    m_GalleonPilot.Location = new Point3D(X, Y - TillerManDistance, Z + ZSurface);
                    break;
                case Direction.East:
                    m_GalleonPilot.Location = new Point3D(X - TillerManDistance, Y, Z + ZSurface);
                    break;
                case Direction.West:
                    m_GalleonPilot.Location = new Point3D(X + TillerManDistance, Y, Z + ZSurface);
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

            if (from != null && from.Backpack != null)
            {
                List<ShipRune> list = from.Backpack.FindItemsByType<ShipRune>();

                foreach (ShipRune item in list)
                    item.InvalidateProperties();
            }
        }

        public override bool Contains(int x, int y)
        {
            if (base.Contains(x, y))
                return true;

            foreach (Item item in m_MooringLines)
                if (x == item.X && y == item.Y)
                    return true;

            foreach (Item item in m_CannonTiles)
                if (x == item.X && y == item.Y)
                    return true;

            foreach (Item item in m_FillerTiles)
                if (x == item.X && y == item.Y)
                    return true;

            foreach (Item item in m_HoldTiles)
                if (x == item.X && y == item.Y)
                    return true;

            if (m_GalleonHold != null && m_GalleonHold.X == x && m_GalleonHold.Y == y)
                return true;

            return false;
        }

        public override bool IsExcludedTile(StaticTile tile)
        {
            return IsMastTile(tile);
        }

        public override bool IsExcludedTile(StaticTile[] tiles)
        {
            foreach(StaticTile tile in tiles)
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

        public override bool CanMoveOver(Item item)
        {
            if (item.Z <= this.Z && !item.ItemData.Impassable && item.ItemData.Height < ZSurface / 2)
                return true;

            return base.CanMoveOver(item);
        }

        public bool TryMarkRune(RecallRune rune, Mobile from)
        {
            ShipRune newRune = new ShipRune(this);
            Container c = rune.Parent as Container;
            newRune.Location = rune.Location;

            if (c != null)
                c.AddItem(newRune);
            else
                c.MoveToWorld(from.Location, from.Map);

            rune.Delete();
            return true;
        }

        public void MarkRunes()
        {
            Direction d = Facing;
            string name = this.Name;

            if (Owner == null || !(Owner is PlayerMobile))
                return;

            ShipRune rune1 = new ShipRune(this);
            DistributeRune(rune1, false);

            ShipRune rune2 = new ShipRune(this);
            DistributeRune(rune2, true);

            rune1.InvalidateProperties();
            rune2.InvalidateProperties();
        }

        public override Point3D GetMarkedLocation()
        {
            Point3D pnt = Point3D.Zero;
            int z = ZSurface;

            switch (this.Facing)
            {
                case Direction.North:
                    {
                        pnt = new Point3D(this.X, this.Y + RuneOffset, z);
                        break;
                    }
                case Direction.South:
                    {
                        pnt = new Point3D(this.X, this.Y - RuneOffset, z);
                        break;
                    }
                case Direction.East:
                    {
                        pnt = new Point3D(this.X - RuneOffset, this.Y, z);
                        break;
                    }
                case Direction.West:
                    {
                        pnt = new Point3D(this.X + RuneOffset, this.Y, z);
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
                    Owner.BankBox.DropItem(rune);
                else
                    Owner.AddToBackpack(rune);

                if (bankbox)
                    Owner.SendLocalizedMessage(1149581); //A recall rune for your new ship has been placed in your bank box.
                else
                    Owner.SendLocalizedMessage(1149580); //A recall rune to your ship has been placed in your backpack.
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

        public virtual bool IsEnemy(BaseGalleon galleon)
        {
            if(this.Map != null && this.Map.Rules == MapRules.FeluccaRules)
                return true;

            Mobile thisOwner = Owner;
            Mobile themOwner = galleon.Owner;

            if (thisOwner == null || themOwner == null)
                return true;

            return thisOwner.CanBeHarmful(themOwner, false);
        }

        public virtual bool IsEnemy(Mobile from)
        {
            if (this.Map != null && this.Map.Rules == MapRules.FeluccaRules)
                return true;

            Mobile thisOwner = Owner;

            if (thisOwner == null || from == null || thisOwner is BaseCreature || from is BaseCreature)
                return true;

            return from.CanBeHarmful(thisOwner, false);
        }

        public bool IsPublic()
        {
            if (m_SecurityEntry == null)
                m_SecurityEntry = new SecurityEntry(this);

            return m_SecurityEntry.IsPublic;
        }

        public bool HasAccess(Mobile from)
        {
            if(Owner == null || (Scuttled && IsEnemy(from)) || (Owner is BaseCreature && !Owner.Alive))
                return true;

            return GetSecurityLevel(from) !=  SecurityLevel.Denied;
        }

        public virtual void OnTakenDamage(int damage)
        {
            OnTakenDamage(null, damage);
        }

        public virtual void OnTakenDamage(Mobile damager, int damage)
        {
            m_Hits -= damage;

            //TODO: Damage packets?
            if (damager != null)
            {
                List<Mobile> list = GetMobilesOnBoard();

                foreach(Mobile m in list.Where(mobile => mobile is PlayerMobile && mobile.NetState != null && HasAccess(mobile)))
                {
                    m.SendMessage(33, "Your ship has recieved {0} damage from {1}.", damage, damager.Name);
                }

                list.Clear();
                list.TrimExcess();

                if (damager is PlayerMobile && damager.NetState != null)
                    damager.SendMessage(33, "You have inflicted {0} to {1}.", damage, ShipName == null ? "an unnamed ship" : ShipName);

            }

            if (m_Hits < 0)
                m_Hits = 0;
            if (m_Hits > MaxHits)
                m_Hits = MaxHits;

            ComputeDamage();
        }

        private void ComputeDamage()
        {
            if (Durability >= 100)
                this.DamageTaken = DamageLevel.Pristine;
            else if (Durability >= 75.0)
                this.DamageTaken = DamageLevel.Slightly;
            else if (Durability >= 50.0)
                this.DamageTaken = DamageLevel.Moderately;
            else if (Durability >= 25.0)
                this.DamageTaken = DamageLevel.Heavily;
            else
                this.DamageTaken = DamageLevel.Severely;
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
            foreach (Item item in m_CannonTiles)
            {
                if (item.Map != Map.Internal && !item.Deleted)
                {
                    if(heavy)
                        TryAddCannon(captain, item.Location, new HeavyShipCannon(this), null);
                    else
                        TryAddCannon(captain, item.Location, new LightShipCannon(this), null);
                }
            }
        }

        public bool TryAddCannon(Mobile from, Point3D pnt, ShipCannonDeed deed)
        {
            BaseCannon item;
            switch (deed.CannonType)
            {
                default:
                case CannonType.Light: item = new LightShipCannon(this); break;
                case CannonType.Heavy: item = new HeavyShipCannon(this); break;
            }

            return TryAddCannon(from, pnt, item, deed);
        }

        public bool TryAddCannon(Mobile from, Point3D pnt, BaseCannon cannon, ShipCannonDeed deed)
        {
            if (cannon == null)
                return false;

            if (IsValidCannonSpot(ref pnt, from))
            {
                cannon.MoveToWorld(pnt, this.Map);
                m_Cannons.Add((Item)cannon);
                UpdateCannonID(cannon);
                cannon.Position = GetCannonPosition(pnt);
                cannon.DoAreaMessage(1116074, 10, from); //~1_NAME~ deploys a ship cannon.

                if (deed != null && from.AccessLevel == AccessLevel.Player)
                    deed.Delete();

                return true;
            }
            cannon.Delete();
            return false;
        }

        public void RemoveCannon(BaseCannon cannon)
        {
            if(m_Cannons.Contains(cannon))
                m_Cannons.Remove(cannon);
        }

        public bool IsValidCannonSpot(ref Point3D pnt, Mobile from)
        {
            if (this.Map == null || this.Map == Map.Internal)
                return false;

            //Lets see if a cannon exists here
            foreach (Item cannon in m_Cannons)
            {
                if (cannon.X == pnt.X && cannon.Y == pnt.Y)
                {
                    if(from != null)
                        from.SendLocalizedMessage(1116075); //There is already a weapon deployed here.

                    return false;
                }
            }

            //Now we can check for a valid cannon tile ID
            foreach (Item item in m_CannonTiles)
            {
                if (item.X == pnt.X && item.Y == pnt.Y)
                {
                    pnt.Z = item.Z + TileData.ItemTable[item.ItemID & TileData.MaxItemValue].CalcHeight;
                    IPooledEnumerable eable = this.Map.GetMobilesInRange(pnt, 0);

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

            m_MooringLines.ForEach(item =>
            {
                item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            });

            m_Cannons.ForEach(item =>
            {
                item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            });

            m_CannonTiles.ForEach(item =>
            {
                item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            });

            m_FillerTiles.ForEach(item =>
            {
                item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            });

            m_HoldTiles.ForEach(item =>
            {
                item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            });

            //foreach (Item item in m_Addons)
            //{
            //    item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            //}

            if (m_Wheel != null)
                m_Wheel.Location = new Point3D(X + (m_Wheel.X - old.X), Y + (m_Wheel.Y - old.Y), Z + (m_Wheel.Z - old.Z));

            if(m_GalleonHold != null)
                m_GalleonHold.Location = new Point3D(X + (m_GalleonHold.X - old.X), Y + (m_GalleonHold.Y - old.Y), Z + (m_GalleonHold.Z - old.Z));
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            m_MooringLines.ForEach(item =>
            {
                item.Map = Map;
            });

            m_Cannons.ForEach(item =>
            {
                item.Map = Map;
            });

            m_CannonTiles.ForEach(item =>
            {
                item.Map = Map;
            });

            m_FillerTiles.ForEach(item =>
            {
                item.Map = Map;
            });

            m_HoldTiles.ForEach(item =>
            {
                item.Map = Map;
            });

            m_Addons.ForEach(item =>
            {
                item.Map = Map;
            });

            if (m_Wheel != null)
                m_Wheel.Map = Map;

            if (m_GalleonHold != null)
                m_GalleonHold.Map = Map;
        }

        public override TimeSpan GetMovementInterval(bool fast, bool drifting, out int clientSpeed)
        {
            if (m_DamageTaken < DamageLevel.Heavily)
                return base.GetMovementInterval(fast, drifting, out clientSpeed);

            if (fast)
            {
                clientSpeed = 0x3;
                return FastDriftInterval;
            }

            clientSpeed = 0x2;
            return SlowDriftInterval;
        }

        public override bool IsComponentItem(ISpawnable spawnable)
        {
            if (!(spawnable is Item))
                return false;

            Item item = (Item)spawnable;

            if (m_MooringLines.Contains(item) || m_Cannons.Contains(item) || m_CannonTiles.Contains(item)
                || m_FillerTiles.Contains(item) || m_HoldTiles.Contains(item) || m_Addons.Contains(item) || item == m_Wheel || item == m_GalleonHold)
                return true;

            return base.IsComponentItem(spawnable);
        }

        public override void OnAfterDelete()
        {
            if (m_Wheel != null)
                m_Wheel.Delete();

            if (m_GalleonHold != null)
                m_GalleonHold.Delete();

            for (int i = 0; i < m_MooringLines.Count; i++)
                if(m_MooringLines[i] != null && !m_MooringLines[i].Deleted)
                    m_MooringLines[i].Delete();

            if (m_Cannons != null && m_Cannons.Count > 0)
            {
                List<Item> cannons = new List<Item>(m_Cannons);

                for (int i = 0; i < cannons.Count; i++)
                    if (cannons[i] != null && !cannons[i].Deleted)
                        cannons[i].Delete();
            }

            for (int i = 0; i < m_CannonTiles.Count; i++)
                if (m_CannonTiles[i] != null && !m_CannonTiles[i].Deleted)
                    m_CannonTiles[i].Delete();

            for (int i = 0; i < m_FillerTiles.Count; i++)
                if (m_FillerTiles[i] != null && !m_FillerTiles[i].Deleted)
                    m_FillerTiles[i].Delete();

            for (int i = 0; i < m_HoldTiles.Count; i++)
                if (m_HoldTiles[i] != null && !m_HoldTiles[i].Deleted)
                    m_HoldTiles[i].Delete();

            for (int i = 0; i < m_Addons.Count; i++)
                if (m_Addons[i] != null && !m_Addons[i].Deleted)
                    m_Addons[i].Delete();

            if (m_Pole != null)
                m_Pole.Delete();

            if (m_CapturedCaptain != null)
                m_CapturedCaptain.Kill();

            base.OnAfterDelete();
        }

        public override DryDockResult CheckDryDock(Mobile from, Mobile dockmaster)
        {
            if (this is BaseGalleon && ((BaseGalleon)this).GalleonHold.Items.Count > 0)
                return DryDockResult.Hold;

            Container pack = from.Backpack;

            if (dockmaster != null && pack != null && pack.GetAmount(typeof(Gold)) < DockMaster.DryDockAmount && Banker.GetBalance(from) < DockMaster.DryDockAmount)
                return DryDockResult.NotEnoughGold;

            if (m_DamageTaken != DamageLevel.Pristine)
                return DryDockResult.Damaged;

            if (m_Cannons != null && m_Cannons.Count > 0)
            {
                foreach (Item item in m_Cannons)
                {
                    BaseCannon cannon = (BaseCannon)item;

                    if (cannon == null)
                        continue;

                    if (cannon.AmmoType != AmmoType.Empty)
                        return DryDockResult.Cannon;
                }
            }

            return base.CheckDryDock(from, dockmaster);
        }

        public override void OnDryDock(Mobile from)
        {
            if (_InternalCannon == null)
                _InternalCannon = new Dictionary<Item, Item>();

            m_Cannons.ForEach(c =>
                {
                    Item pad = m_CannonTiles.FirstOrDefault(p => p.X == c.X && p.Y == c.Y);

                    if (pad != null)
                        _InternalCannon[c] = pad;
                });

            if (from != null && from.Backpack != null)
            {
                List<ShipRune> list = from.Backpack.FindItemsByType<ShipRune>();

                foreach (ShipRune item in list)
                    item.InvalidateProperties();
            }

            base.OnDryDock(from);
        }

        public override void SetFacingComponents(Direction newDirection, Direction oldDirection, bool ignoreLastDirection)
        {
            if(oldDirection == newDirection && !ignoreLastDirection)
                return;

            int olddir = GetValueForDirection(oldDirection);
            int newdir = GetValueForDirection(newDirection);
            int dirMod = newdir + (DamageValue * 4);
            int temp = dirMod;

            if (dirMod < 0) 
                dirMod = 0;

            if (m_CannonTiles != null)
            {
                if (dirMod >= CannonTileIDs.Length)
                    temp = newdir;

                m_CannonTiles.ForEach(tile =>
                {
                    int idx = GetIndex(CannonTileIDs, tile.ItemID);
                    tile.ItemID = CannonTileIDs[temp][idx];
                });
            }

            temp = dirMod;

            if (m_FillerTiles != null)
            {
                if (dirMod >= FillerIDs.Length)
                    temp = newdir;

                m_FillerTiles.ForEach(tile =>
                {
                    int idx = GetIndex(FillerIDs, tile.ItemID);
                    tile.ItemID = FillerIDs[temp][idx];
                });
            }

            temp = dirMod;

            if (m_HoldTiles != null)
            {
                if (dirMod >= HoldIDs.Length)
                    temp = newdir;

                m_HoldTiles.ForEach(tile =>
                {
                    int idx = GetIndex(HoldIDs, tile.ItemID);
                    tile.ItemID = HoldIDs[temp][idx];
                });
            }

            temp = dirMod;

            if (m_GalleonHold != null)
            {
                if (dirMod >= HoldItemIDs.Length)
                    temp = newdir;

                m_GalleonHold.ItemID = HoldItemIDs[temp][0];
            }

            if (m_Wheel != null)
                m_Wheel.ItemID = WheelItemIDs[newdir][0];

            for (int i = 0; i < m_Addons.Count; i++)
            {
                if (i >= 0 && i < m_AddonTiles.Count)
                {
                    Item tile = m_AddonTiles[i];
                    int z = tile.Z + TileData.ItemTable[tile.ItemID & TileData.MaxItemValue].CalcHeight;

                    m_Addons[i].MoveToWorld(new Point3D(tile.X, tile.Y, z), this.Map);
                }
            }

            UpdateCannonIDs();
        }

        public int GetIndex(int[][] list, int check)
        {
            for (int i = 0; i < list.Length; i++)
            {
                int index = Array.IndexOf(list[i], check);
                if (index > -1)
                    return index;
            }
            return check;
        }

        public override List<Item> GetComponents()
        {
            List<Item> list = new List<Item>();
            list.AddRange(m_CannonTiles);
            list.AddRange(m_FillerTiles);
            list.AddRange(m_HoldTiles);
            list.AddRange(m_MooringLines);
            list.Add(m_GalleonHold);
            list.Add(m_Wheel);
            return list;
        }

        public int GetValueForDirection(Direction direction)
        {
            switch (direction)
            {
                default:
                case Direction.South: return 0;
                case Direction.West: return 1;
                case Direction.North: return 2;;
                case Direction.East: return 3;
            }
        }

        public void UpdateCannonIDs()
        {
            m_Cannons.ForEach(c => {
                UpdateCannonID(c as BaseCannon);
            });
        }

        public void UpdateCannonID(BaseCannon cannon)
        {
            if (cannon == null)
                return;

            int type = cannon is LightShipCannon ? 0 : 1;

            switch (this.Facing)
            {
                default:
                case Direction.South:
                case Direction.North:
                    {
                        if (cannon.X == X)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(this.Facing)][type];
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
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(this.Facing)][type];
                        else if (cannon.Y < Y)
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.North)][type];
                        else
                            cannon.ItemID = m_CannonIDs[GetValueForDirection(Direction.South)][type];
                        break;
                    }
            }
        }

        private int[][] m_CannonIDs = new int[][]
        { 
                      //Light  Heavy
            new int[] { 16918, 16922 }, //South
            new int[] { 16919, 16923 }, //West
            new int[] { 16920, 16924 }, //North
            new int[] { 16921, 16925 }, //East
        };

        public virtual ShipPosition GetCannonPosition(Point3D pnt)
        {
            return ShipPosition.Bow;
        }

        protected Static AddCannonTile(Static st)
        {
            st.Name = "weapon pad";
            m_CannonTiles.Add((Item)st);
            return st;
        }

        protected HoldItem AddHoldTile(HoldItem item)
        {
            item.Name = "cargo hold";
            m_HoldTiles.Add((Item)item);
            return item;
        }

        protected GalleonHold AddGalleonHold(GalleonHold hold)
        {
            hold.Name = "cargo hold";
            m_GalleonHold = hold;
            return hold;
        }

        protected Static AddFillerTile(Static item)
        {
            item.Name = "deck";
            m_FillerTiles.Add((Item)item);
            return item;
        }

        protected MooringLine AddMooringLine(MooringLine line)
        {
            m_MooringLines.Add((Item)line);
            return line;
        }

        protected void AddAddonTile(Static item)
        {
            m_AddonTiles.Add((Item)item);
        }

        #region Repairs
        private int m_PreRepairHits;
        private EmergencyRepairDamageTimer m_EmergencyRepairTimer;

        public static readonly int EmergencyRepairClothCost = 55;
        public static readonly int EmergencyRepairWoodCost = 25;
        public static readonly TimeSpan EmergencyRepairSpan = TimeSpan.FromMinutes(5);

        public bool IsUnderEmergencyRepairs()
        {
            return m_EmergencyRepairTimer != null;
        }

        public TimeSpan GetEndEmergencyRepairs()
        {
            if (m_EmergencyRepairTimer != null && m_EmergencyRepairTimer.EndRepairs > DateTime.UtcNow)
                return m_EmergencyRepairTimer.EndRepairs - DateTime.UtcNow;

            return TimeSpan.Zero;
        }

        public bool TryEmergencyRepair(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return false;

            int clothNeeded = EmergencyRepairClothCost;
            int woodNeeded = EmergencyRepairWoodCost;
            Container pack = from.Backpack;
            Container hold = m_GalleonHold;
            TimeSpan ts = EmergencyRepairSpan;

            int wood1 = pack.GetAmount(typeof(Board));
            int wood2 = pack.GetAmount(typeof(Log));
            int wood3 = 0; int wood4 = 0;

            int cloth1 = pack.GetAmount(typeof(Cloth));
            int cloth2 = pack.GetAmount(typeof(UncutCloth));
            int cloth3 = 0; int cloth4 = 0;

            if (hold != null)
            {
                wood3 = hold.GetAmount(typeof(Board));
                wood4 = hold.GetAmount(typeof(Log));
                cloth3 = hold.GetAmount(typeof(Cloth));
                cloth4 = hold.GetAmount(typeof(UncutCloth));
            }

            int totalWood = wood1 + wood2 + wood3 + wood4;
            int totalCloth = cloth1 + cloth2 + cloth3 + cloth4;

            if (totalWood >= woodNeeded && totalCloth >= clothNeeded)
            {
                int toConsume = 0;

                if (woodNeeded > 0 && wood1 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood1);
                    pack.ConsumeTotal(typeof(Board), toConsume);
                    woodNeeded -= toConsume;
                }
                if (woodNeeded > 0 && wood2 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood2);
                    pack.ConsumeTotal(typeof(Log), toConsume);
                    woodNeeded -= toConsume;
                }
                if (hold != null && woodNeeded > 0 && wood3 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood3);
                    hold.ConsumeTotal(typeof(Board), toConsume);
                    woodNeeded -= toConsume;
                }
                if (hold != null && woodNeeded > 0 && wood4 > 0)
                {
                    toConsume = Math.Min(woodNeeded, wood4);
                    hold.ConsumeTotal(typeof(Log), toConsume);
                }
                if (clothNeeded > 0 && cloth1 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth1);
                    pack.ConsumeTotal(typeof(Cloth), toConsume);
                    clothNeeded -= toConsume;
                }
                if (clothNeeded > 0 && cloth2 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth2);
                    pack.ConsumeTotal(typeof(UncutCloth), toConsume);
                    clothNeeded -= toConsume;
                }
                if (hold != null && clothNeeded > 0 && cloth3 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth3);
                    hold.ConsumeTotal(typeof(Cloth), toConsume);
                    clothNeeded -= toConsume;
                }
                if (hold != null && clothNeeded > 0 && cloth4 > 0)
                {
                    toConsume = Math.Min(clothNeeded, cloth4);
                    hold.ConsumeTotal(typeof(UncutCloth), toConsume);
                }

                from.SendLocalizedMessage(1116592, ts.TotalMinutes.ToString()); //Your ship is underway with emergency repairs holding for an estimated ~1_TIME~ more minutes.
                m_PreRepairHits = m_Hits;
                m_Hits = (int)(MaxHits * .40);
                m_EmergencyRepairTimer = new EmergencyRepairDamageTimer(this, ts);
                ComputeDamage();
                return true;
            }
            return false;
        }

        public void EndEmergencyRepairEffects()
        {
            m_EmergencyRepairTimer = null;
            m_Hits = m_PreRepairHits;
            m_PreRepairHits = 0;
            ComputeDamage();

            SendMessageToAllOnBoard(1116765);  //The emergency repairs have given out!
        }

        private static readonly double WoodPer = 17;
        private static readonly double ClothPer = 17;

        private Type[] WoodTypes = new Type[] { typeof(Board),  typeof(OakBoard), typeof(AshBoard), typeof(YewBoard), typeof(HeartwoodBoard), typeof(BloodwoodBoard), typeof(FrostwoodBoard),
                                                typeof(Log), typeof(OakLog), typeof(AshLog), typeof(YewLog), typeof(HeartwoodLog), typeof(BloodwoodLog), typeof(FrostwoodLog), };
        
        private Type[] ClothTypes = new Type[] { typeof(Cloth), typeof(UncutCloth) };

        public void TryRepairs(Mobile from)
        {
			if(from == null || from.Backpack == null)
				return;
				
			Container pack = from.Backpack;
            Container hold = m_GalleonHold;
            Container secure = SecureContainer;

            double wood = 0;
            double cloth = 0;

            for(int i = 0; i < WoodTypes.Length; i++)
            {
                Type type = WoodTypes[i];
                if (pack != null) wood += pack.GetAmount(type);
                if (hold != null) wood += hold.GetAmount(type);
                if (secure != null) wood += secure.GetAmount(type);
            }

            for (int i = 0; i < ClothTypes.Length; i++)
            {
                Type type = ClothTypes[i];
                if (pack != null) cloth += pack.GetAmount(type);
                if (hold != null) cloth += hold.GetAmount(type);
                if (secure != null) cloth += secure.GetAmount(type);
            }

			//Now, how much do they need for 100% repair
			double woodNeeded = WoodPer * (100.0 - Durability);
			double clothNeeded = ClothPer * (100.0 - Durability);
			
			//Apply skill bonus
			woodNeeded -= ((double)from.Skills[SkillName.Carpentry].Value / 200.0) * woodNeeded;
			clothNeeded -= ((double)from.Skills[SkillName.Tailoring].Value / 200.0) * clothNeeded;
			
			//get 10% of needed repairs
			double minWood = woodNeeded / 10;
			double minCloth = clothNeeded / 10;
			
			if(wood < minWood || cloth < minCloth)
			{
                from.SendLocalizedMessage(1116593, String.Format("{0}\t{1}", ((int)minCloth).ToString(), ((int)minWood).ToString())); //You need a minimum of ~1_CLOTH~ yards of cloth and ~2_WOOD~ pieces of lumber to effect repairs to this ship.
				return;
			}
			
			double percWood, percCloth, woodUsed, clothUsed;
			
			if(wood >= woodNeeded)
			{
				woodUsed = woodNeeded;
				percWood = 100;
            }
			else
			{
				woodUsed = wood;
				percWood = (wood / woodNeeded) * 100;
			}
				
			if(cloth >= clothNeeded)
			{
				clothUsed = clothNeeded;
				percCloth = 100;
			}
			else
			{
				clothUsed = cloth;
				percCloth = (cloth / clothNeeded) * 100;
			}

            if (clothUsed > woodUsed)
            {
                clothUsed = woodUsed;
                percCloth = percWood;
            }
            else if (woodUsed > clothUsed)
            {
                woodUsed = clothUsed;
                percWood = percCloth;
            }
			
			//Average out percentage
			double totalPerc = (percWood + percCloth) / 2;
            double toConsume = 0;
            double woodTemp = woodUsed;
            double clothTemp = clothUsed;

            #region Consume
            for (int i = 0; i < WoodTypes.Length; i++)
            {
                Type type = WoodTypes[i];

                if (woodUsed <= 0)
                    break;

                if (pack != null && woodUsed > 0 && pack.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(woodUsed, pack.GetAmount(type));
                    pack.ConsumeTotal(type, (int)toConsume);
                    woodUsed -= toConsume;
                }
                if (hold != null && woodUsed > 0 && hold.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(woodUsed, hold.GetAmount(type));
                    hold.ConsumeTotal(type, (int)toConsume);
                    woodUsed -= toConsume;
                }
                if (secure != null && woodUsed > 0 && secure.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(woodUsed, secure.GetAmount(type));
                    secure.ConsumeTotal(type, (int)toConsume);
                    woodUsed -= toConsume;
                }
            }

            for (int i = 0; i < ClothTypes.Length; i++)
            {
                Type type = ClothTypes[i];

                if (clothUsed <= 0)
                    break;

                if (pack != null && clothUsed > 0 && pack.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(clothUsed, pack.GetAmount(type));
                    pack.ConsumeTotal(type, (int)toConsume);
                    clothUsed -= toConsume;
                }
                if (hold != null && clothUsed > 0 && hold.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(clothUsed, hold.GetAmount(type));
                    hold.ConsumeTotal(type, (int)toConsume);
                    clothUsed -= toConsume;
                }
                if (secure != null && clothUsed > 0 && secure.GetAmount(type) > 0)
                {
                    toConsume = Math.Min(clothUsed, secure.GetAmount(type));
                    secure.ConsumeTotal(type, (int)toConsume);
                    clothUsed -= toConsume;
                }
            }
            #endregion

            m_Hits += (int)((MaxHits - m_Hits) * (totalPerc / 100));
			if(m_Hits > MaxHits) m_Hits = MaxHits;
			ComputeDamage();
			
			totalPerc += Durability;

			if(totalPerc > 100) 
                totalPerc = 100;

            if (m_EmergencyRepairTimer != null)
            {
                m_EmergencyRepairTimer.Stop();
                m_EmergencyRepairTimer = null;
            }
		
            string args = String.Format("{0}\t{1}\t{2}", ((int)clothTemp).ToString(), ((int)woodTemp).ToString(), ((int)totalPerc).ToString());
            from.SendLocalizedMessage(1116598, args); //You effect permanent repairs using ~1_CLOTH~ yards of cloth and ~2_WOOD~ pieces of lumber. The ship is now ~3_DMGPCT~% repaired.
        }

        private class EmergencyRepairDamageTimer : Timer
        {
            private BaseGalleon m_Galleon;
            private DateTime m_EndRepairs;

            public DateTime EndRepairs { get { return m_EndRepairs; } }

            public EmergencyRepairDamageTimer(BaseGalleon galleon, TimeSpan duration)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Galleon = galleon;
                m_EndRepairs = DateTime.UtcNow + duration;
                this.Start();
            }

            protected override void OnTick()
            {
                if (m_Galleon == null)
                {
                    this.Stop();
                    return;
                }

                if (m_EndRepairs < DateTime.UtcNow)
                {
                    m_Galleon.EndEmergencyRepairEffects();
                    this.Stop();
                }
            }
        }
        #endregion

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
        public bool HasPaint { get { return m_BasePaintHue != 0; } }

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
            else /*if (m_BasePaintHue == m_BaseBoatHue)*/
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
            foreach (Item item in m_CannonTiles)
                item.Hue = Hue;

            foreach (Item item in m_FillerTiles)
                item.Hue = Hue;

            foreach (Item item in m_HoldTiles)
                item.Hue = Hue;

            if (m_GalleonHold != null)
                m_GalleonHold.Hue = Hue;

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
            BaseBoat boat = BaseBoat.FindBoatAt(pnt, map);

            if (boat is BaseGalleon)
                return boat as BaseGalleon;
            return null;
        }

        public static bool HasGalleon(Mobile from)
        {
            foreach (BaseBoat boat in BaseBoat.Boats)
            {
                if (boat is BaseGalleon && boat.Owner == from && !boat.Deleted && boat.Map != Map.Internal)
                    return true;
            }
            return false;
        }

        public static bool CheckForBoat(IPoint3D p, Mobile caster)
        {
            BaseBoat boat = BaseBoat.FindBoatAt(caster, caster.Map);
            BaseGalleon galleon = BaseGalleon.FindGalleonAt(p, caster.Map);

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
                case Direction.North: return new Point3D(X, Y + RuneOffset, m_GalleonPilot.Z);
                case Direction.South: return new Point3D(X, Y - RuneOffset, m_GalleonPilot.Z);
                case Direction.East: return new Point3D(X + RuneOffset, Y, m_GalleonPilot.Z);
                case Direction.West: return new Point3D(X - RuneOffset, Y, m_GalleonPilot.Z);
            }
        }

        #region Addons
        public void AddAddon(Item item)
        {
            m_Addons.Add(item);
        }

        public void RemoveAddon(Item item)
        {
            if (m_Addons.Contains(item))
                m_Addons.Remove(item);
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
                #region Mondains Legacy
                deed.Resource = addon.Resource;
                #endregion

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
            writer.Write((int)3);

            writer.Write(m_Pole);
            writer.Write(m_CapturedCaptain);

            writer.Write(m_BaseBoatHue);

            writer.Write(m_GalleonPilot);
            writer.Write(m_Hits);
            writer.Write(m_Wheel);
            writer.Write(m_GalleonHold);
            writer.Write((int)m_DamageTaken);

            writer.Write(m_BasePaintHue);
            writer.Write(m_PaintCoats);
            writer.Write(m_NextPaintDecay);

            SecurityEntry.Serialize(writer);

            writer.Write(m_CannonTiles.Count);
            for(int i = 0; i < m_CannonTiles.Count; i++)
                writer.Write(m_CannonTiles[i]);
            
            writer.Write(m_Cannons.Count);
            for (int i = 0; i < m_Cannons.Count; i++)
                writer.Write(m_Cannons[i]);

            writer.Write(m_FillerTiles.Count);
            for (int i = 0; i < m_FillerTiles.Count; i++)
                writer.Write(m_FillerTiles[i]);

            writer.Write(m_MooringLines.Count);
            for (int i = 0; i < m_MooringLines.Count; i++)
                writer.Write(m_MooringLines[i]);

            writer.Write(m_HoldTiles.Count);
            for (int i = 0; i < m_HoldTiles.Count; i++)
                writer.Write(m_HoldTiles[i]);

            writer.Write(m_Addons.Count);
            for (int i = 0; i < m_Addons.Count; i++)
                writer.Write(m_Addons[i]);

            writer.Write(m_AddonTiles.Count);
            for (int i = 0; i < m_AddonTiles.Count; i++)
                writer.Write(m_AddonTiles[i]);

            Timer.DelayCall(TimeSpan.FromSeconds(25), new TimerCallback(CheckPaintDecay));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                case 2:
                    m_Pole = reader.ReadItem() as BindingPole;
                    m_CapturedCaptain = reader.ReadMobile();
                    goto case 1;
                case 1:
                    m_BaseBoatHue = reader.ReadInt();
                    goto case 0;
                case 0:
                    m_GalleonPilot = reader.ReadMobile();
                    m_Hits = reader.ReadInt();
                    m_Wheel = reader.ReadItem() as ShipWheel;
                    m_GalleonHold = reader.ReadItem() as GalleonHold;

                    if (version < 3)
                    {
                        reader.ReadItem();
                        reader.ReadItem();
                    }

                    m_DamageTaken = (DamageLevel)reader.ReadInt();

                    m_BasePaintHue = reader.ReadInt();
                    m_PaintCoats = reader.ReadInt();
                    m_NextPaintDecay = reader.ReadDateTime();

                    m_SecurityEntry = new SecurityEntry(this, reader);

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Static tile = reader.ReadItem() as Static;
                        if (tile != null && !tile.Deleted)
                            AddCannonTile(tile);
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Item cannon = reader.ReadItem();
                        if (cannon != null && !cannon.Deleted)
                            m_Cannons.Add(cannon);
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Static filler = reader.ReadItem() as Static;
                        if (filler != null && !filler.Deleted)
                            AddFillerTile(filler);
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        MooringLine line = reader.ReadItem() as MooringLine;
                        if (line != null && !line.Deleted)
                            AddMooringLine(line);
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        HoldItem hItem = reader.ReadItem() as HoldItem;
                        if (hItem != null && !hItem.Deleted)
                            AddHoldTile(hItem);
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Item addon = reader.ReadItem();
                        if (addon != null && !addon.Deleted)
                            AddAddon(addon);
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Static atile = reader.ReadItem() as Static;
                        if (atile != null && !atile.Deleted)
                            AddAddonTile(atile);
                    }

                    break;
            }

            if (m_Pole != null)
                m_Pole.Galleon = this;
        }
    }

    [PropertyObject]
    public class SecurityEntry
    {
        private readonly SecurityLevel DefaultImpliedAccessLevel = SecurityLevel.Passenger;

        private BaseGalleon m_Galleon;
        private PartyAccess m_PartyAccess;
        private SecurityLevel m_DefaultPublicAccess;
        private SecurityLevel m_DefaultPartyAccess;
        private SecurityLevel m_DefaultGuildAccess;
        private Dictionary<Mobile, SecurityLevel> m_Manifest;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get { return m_Galleon; } set { m_Galleon = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PartyAccess PartyAccess { get { return m_PartyAccess; } set { m_PartyAccess = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultPublicAccess { get { return m_DefaultPublicAccess; } set { m_DefaultPublicAccess = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultPartyAccess { get { return m_DefaultPartyAccess; } set { m_DefaultPartyAccess = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecurityLevel DefaultGuildAccess { get { return m_DefaultGuildAccess; } set { m_DefaultGuildAccess = value; } }
        
        public Dictionary<Mobile, SecurityLevel> Manifest { get { return m_Manifest; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPublic { get { return m_DefaultPublicAccess != SecurityLevel.Denied; } }

        public SecurityEntry(BaseGalleon galleon)
        {
            m_Galleon = galleon;
            m_Manifest = new Dictionary<Mobile, SecurityLevel>();

            AddToManifest(m_Galleon.Owner, SecurityLevel.Captain);
            SetToDefault();
        }

        public bool AddToManifest(Mobile from, SecurityLevel access)
        {
            if (from == null || m_Manifest == null)
                return false;

            //if (m_Manifest.ContainsKey(from) && m_Manifest[from] >= access)
            //    return true;

            m_Manifest[from] = access;
            return true;
        }

        public bool RemoveFromAccessList(Mobile from)
        {
            if (!m_Manifest.ContainsKey(from))
                return false;

            m_Manifest.Remove(from);
            return true;
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
            if (from == m_Galleon.Owner)
                return SecurityLevel.Captain;

            SecurityLevel highest = SecurityLevel.Denied;

            if (m_Manifest.ContainsKey(from))
            {
                if (m_Manifest[from] == highest) //denied
                    return highest;

                highest = m_Manifest[from];
            }

            if (highest < m_DefaultPublicAccess)
                highest = m_DefaultPublicAccess;

            if (IsInParty(from) && highest < m_DefaultPartyAccess)
                highest = m_DefaultPartyAccess;

            if (IsInGuild(from) && highest < m_DefaultGuildAccess)
                highest = m_DefaultGuildAccess;

            if ( checkImplied && highest == SecurityLevel.Denied)
                highest = GetImpliedAccess(from);

            return highest;
        }

        public bool IsInParty(Mobile from)
        {
            if (from == null || m_Galleon == null || m_Galleon.Owner == null)
                return false;

            Party fromParty = Party.Get(from);
            Party ownerParty = Party.Get(m_Galleon.Owner);

            if (fromParty == null || ownerParty == null)
                return false;

            if (fromParty == ownerParty)
            {
                switch (m_PartyAccess)
                {
                    case PartyAccess.Never: return false;
                    case PartyAccess.LeaderOnly: return ownerParty.Leader == m_Galleon.Owner;
                    case PartyAccess.MemberOnly: return true;
                }
            }
            return false;
        }

        public bool IsInGuild(Mobile from)
        {
            if (from == null || m_Galleon == null || m_Galleon.Owner == null)
                return false;

            Guild fromGuild = from.Guild as Guild;
            Guild ownerGuild = m_Galleon.Owner.Guild as Guild;

            return fromGuild != null && ownerGuild != null && fromGuild == ownerGuild;
        }

        public void SetToDefault()
        {
            m_PartyAccess = PartyAccess.MemberOnly;
            m_DefaultPublicAccess = SecurityLevel.Denied;
            m_DefaultPartyAccess = SecurityLevel.Crewman;
            m_DefaultGuildAccess = SecurityLevel.Officer;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write((int)m_PartyAccess);
            writer.Write((int)m_DefaultPublicAccess);
            writer.Write((int)m_DefaultPartyAccess);
            writer.Write((int)m_DefaultGuildAccess);

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
            m_Galleon = galleon;

            int version = reader.ReadInt();

            m_PartyAccess = (PartyAccess)reader.ReadInt();
            m_DefaultPublicAccess = (SecurityLevel)reader.ReadInt();
            m_DefaultPartyAccess = (SecurityLevel)reader.ReadInt();
            m_DefaultGuildAccess = (SecurityLevel)reader.ReadInt();

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

    [PropertyObject]
	public class PilotEntry
	{
		private string m_Name;
		private bool m_Female;
		private int m_SkinHue;
		private int m_HairHue;
		private int m_HairID;
		private int m_SpeechHue;

        [CommandProperty(AccessLevel.GameMaster)]
		public string Name { get { return m_Name; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public bool Female { get { return m_Female; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public int SkinHue { get { return m_SkinHue; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public int HairHue { get { return m_HairHue; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public int HairID { get { return m_HairID; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public int SpeechHue { get { return m_SpeechHue; } }
		
		public override string ToString()
		{
			return "...";
		}
		
		public PilotEntry(Mobile pilot)
		{
			m_Name = pilot.Name;
			m_Female = pilot.Female;
			m_SkinHue = pilot.Hue;
			m_HairHue = pilot.HairHue;
			m_HairID = pilot.HairItemID;
			m_SpeechHue = pilot.SpeechHue;
		}
		
		public void Serialize(GenericWriter writer)
		{
			writer.Write((int)0);
			writer.Write(m_Name);
			writer.Write(m_Female);
			writer.Write(m_SkinHue);
			writer.Write(m_HairHue);
			writer.Write(m_HairID);
			writer.Write(m_SpeechHue);
		}
		
		public PilotEntry(GenericReader reader)
		{
			int version = reader.ReadInt();
			m_Name = reader.ReadString();
			m_Female = reader.ReadBool();
			m_SkinHue = reader.ReadInt();
			m_HairHue = reader.ReadInt();
			m_HairID = reader.ReadInt();
			m_SpeechHue = reader.ReadInt();
		}
	}

    public class GrantAccessEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private Mobile m_Clicker;
        private BaseGalleon m_Galleon;

        public GrantAccessEntry(Mobile from, Mobile clicker, BaseGalleon galleon) : base(1060676, 15)
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

            m_Galleon.SecurityEntry.AddToManifest(m_From, SecurityLevel.Passenger);
            m_Clicker.SendGump(new GrantAccessGump(m_From, m_Galleon));

            m_From.SendMessage("{0} has granted you access to {1}.", m_Clicker.Name, m_Galleon.ShipName != null && m_Galleon.ShipName.Length > 0 ? m_Galleon.ShipName : "an unnamed ship");
        }
    }
}