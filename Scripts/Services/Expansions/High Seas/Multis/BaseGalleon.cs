using System;
using System.Linq;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Items;
using Server.Guilds;
using Server.Accounting;
using Server.Engines.PartySystem;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Multis
{
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

        private List<Item> m_MooringLines = new List<Item>();
        private List<Item> m_Cannons = new List<Item>();
        private List<Item> m_CannonTiles = new List<Item>();
        private List<Item> m_FillerTiles = new List<Item>();
        private List<Item> m_HoldTiles = new List<Item>();
        private List<Item> m_Addons = new List<Item>();
        private List<Item> m_AddonTiles = new List<Item>();

        private Dictionary<Item, Item> _InternalCannon;

        public List<Item> FillerTiles { get { return m_FillerTiles; } }

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

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipWheel Wheel { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold GalleonHold { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BindingPole Pole { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile CapturedCaptain { get; set; }

        public override int LabelNumber { get { return 1035980; } } // mast

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxAddons { get { return m_AddonTiles.Count; } }

        public List<Item> Addons { get { return m_Addons; } }
        public List<Item> Cannons { get { return m_Cannons; } }

        public override bool IsClassicBoat { get { return false; } }

        public virtual int MaxCannons { get { return 0; } }
        public virtual int WheelDistance { get { return 0; } }
        public virtual int CaptiveOffset { get { return 0; } }
        public virtual double CannonDamageMod { get { return 1.0; } }

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
            m_BaseBoatHue = 0;

            AddMooringLines(direction);
            AddCannonTiles(direction);
            AddHoldTiles(direction);

            AddGalleonPilotAndWheel(direction);
            Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerCallback(MarkRunes));
        }

        public void AddGalleonPilotAndWheel(Direction direction)
        {
            int dir = GetValueForDirection(Facing);

            ShipWheel wheel = new ShipWheel(this);
            wheel.ItemID = WheelItemIDs[dir][0];

            GalleonPilot pilot = new GalleonPilot(this);
            pilot.Direction = direction;

            Wheel = wheel;
            TillerMan = pilot;
            GalleonPilot = pilot;

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

            if (GalleonHold != null && GalleonHold.X == x && GalleonHold.Y == y)
                return true;

            return false;
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
            if (m_Addons.Contains(item))
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

            return addon != null && m_Addons.Contains(addon);
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

            foreach (Item item in m_CannonTiles)
            {
                if (item.Map != Map.Internal && !item.Deleted)
                {
                    IShipCannon cannon;

                    if (heavy)
                    {
                        if (Core.EJ)
                        {
                            cannon = new Carronade(this);
                        }
                        else
                        {
                            cannon = new HeavyShipCannon(this);
                        }
                    }
                    else
                    {
                        if (Core.EJ)
                        {
                            cannon = new Culverin(this);
                        }
                        else
                        {
                            cannon = new LightShipCannon(this);
                        }
                    }

                    if (!TryAddCannon(captain, item.Location, cannon, null))
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
                        {
                            cannon = new PumpkinCannon(this);
                            break;
                        }
                    case CannonPower.Light:
                        if (Core.EJ)
                        {
                            cannon = new Culverin(this);
                        }
                        else
                        {
                            cannon = new LightShipCannon(this);
                        }
                        break;
                    case CannonPower.Heavy:
                        if (Core.EJ)
                        {
                            cannon = new Carronade(this);
                        }
                        else
                        {
                            cannon = new HeavyShipCannon(this);
                        }
                        break;
                    case CannonPower.Massive:
                        if (Core.EJ)
                        {
                            cannon = new Blundercannon(this);
                        }
                        else
                        {
                            cannon = new HeavyShipCannon(this);
                        }
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
                m_Cannons.Add((Item)cannon);
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
            if (m_Cannons.Contains(cannon))
                m_Cannons.Remove(cannon);
        }

        public bool IsValidCannonSpot(ref Point3D pnt, Mobile from)
        {
            if (Map == null || Map == Map.Internal)
                return false;

            //Lets see if a cannon exists here
            foreach (Item cannon in m_Cannons)
            {
                if (cannon.X == pnt.X && cannon.Y == pnt.Y)
                {
                    if (from != null)
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

            if (Wheel != null)
                Wheel.Location = new Point3D(X + (Wheel.X - old.X), Y + (Wheel.Y - old.Y), Z + (Wheel.Z - old.Z));

            if (GalleonHold != null)
                GalleonHold.Location = new Point3D(X + (GalleonHold.X - old.X), Y + (GalleonHold.Y - old.Y), Z + (GalleonHold.Z - old.Z));
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

            if (Wheel != null)
                Wheel.Map = Map;

            if (GalleonHold != null)
                GalleonHold.Map = Map;
        }

        public override TimeSpan GetMovementInterval(bool fast, bool drifting, out int clientSpeed)
        {
            if (DamageTaken < DamageLevel.Heavily)
                return base.GetMovementInterval(fast, drifting, out clientSpeed);

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

            if (m_MooringLines.Contains(item) || m_Cannons.Contains(item) || m_CannonTiles.Contains(item)
                || m_FillerTiles.Contains(item) || m_HoldTiles.Contains(item) || m_Addons.Contains(item) || item == Wheel || item == GalleonHold)
                return true;

            return base.IsComponentItem(entity);
        }

        public override void OnAfterDelete()
        {
            if (Wheel != null)
                Wheel.Delete();

            if (GalleonHold != null)
                GalleonHold.Delete();

            for (int i = 0; i < m_MooringLines.Count; i++)
                if (m_MooringLines[i] != null && !m_MooringLines[i].Deleted)
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

            if (Pole != null)
                Pole.Delete();

            if (CapturedCaptain != null)
                CapturedCaptain.Kill();

            base.OnAfterDelete();
        }

        public override DryDockResult CheckDryDock(Mobile from, Mobile dockmaster)
        {
            if (this is BaseGalleon && ((BaseGalleon)this).GalleonHold.Items.Count > 0)
                return DryDockResult.Hold;

            Container pack = from.Backpack;

            if (dockmaster != null && pack != null && pack.GetAmount(typeof(Gold)) < DockMaster.DryDockAmount && Banker.GetBalance(from) < DockMaster.DryDockAmount)
                return DryDockResult.NotEnoughGold;

            if (DamageTaken != DamageLevel.Pristine)
                return DryDockResult.Damaged;

            if (m_Cannons != null && m_Cannons.Count > 0)
            {
                foreach (var cannon in m_Cannons.OfType<IShipCannon>())
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
            if (_InternalCannon == null)
                _InternalCannon = new Dictionary<Item, Item>();

            m_Cannons.ForEach(c =>
            {
                Item pad = m_CannonTiles.FirstOrDefault(p => p.X == c.X && p.Y == c.Y);

                if (pad != null)
                    _InternalCannon[c] = pad;
            });

            base.OnDryDock(from);
        }

        public override void SetFacingComponents(Direction newDirection, Direction oldDirection, bool ignoreLastDirection)
        {
            if (oldDirection == newDirection && !ignoreLastDirection)
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

            if (GalleonHold != null)
            {
                if (dirMod >= HoldItemIDs.Length)
                    temp = newdir;

                GalleonHold.ItemID = HoldItemIDs[temp][0];
            }

            if (Wheel != null)
                Wheel.ItemID = WheelItemIDs[newdir][0];

            for (int i = 0; i < m_Addons.Count; i++)
            {
                if (i >= 0 && i < m_AddonTiles.Count)
                {
                    Item tile = m_AddonTiles[i];
                    int z = tile.Z + TileData.ItemTable[tile.ItemID & TileData.MaxItemValue].CalcHeight;

                    m_Addons[i].MoveToWorld(new Point3D(tile.X, tile.Y, z), Map);
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

        public override IEnumerable<IEntity> GetComponents()
        {
            foreach (var item in m_CannonTiles)
                yield return item;

            foreach (var item in m_FillerTiles)
                yield return item;

            foreach (var item in m_HoldTiles)
                yield return item;

            foreach (var item in m_MooringLines)
                yield return item;

            if (GalleonHold != null)
                yield return GalleonHold;

            if (Wheel != null)
                yield return Wheel;

            if (GalleonPilot != null)
                yield return GalleonPilot;
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
            m_Cannons.ForEach(c => {
                UpdateCannonID(c);
            });
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

        public static int[][] CannonIDs { get { return m_CannonIDs; } }
        private static int[][] m_CannonIDs = new int[][]
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
            GalleonHold = hold;
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

            if (GalleonHold != null)
                GalleonHold.Hue = Hue;

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
            writer.Write((int)5);

            writer.Write(Pole);
            writer.Write(CapturedCaptain);

            writer.Write(m_BaseBoatHue);

            writer.Write(GalleonPilot);
            writer.Write(Wheel);
            writer.Write(GalleonHold);

            writer.Write(m_BasePaintHue);
            writer.Write(m_PaintCoats);
            writer.Write(m_NextPaintDecay);

            SecurityEntry.Serialize(writer);

            writer.Write(m_CannonTiles.Count);
            for (int i = 0; i < m_CannonTiles.Count; i++)
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
                case 5:
                case 4:
                case 3:
                case 2:
                    Pole = reader.ReadItem() as BindingPole;
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

                    Wheel = reader.ReadItem() as ShipWheel;
                    GalleonHold = reader.ReadItem() as GalleonHold;

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

            if (Pole != null)
                Pole.Galleon = this;
        }
    }

    [PropertyObject]
    public class SecurityEntry
    {
        private readonly SecurityLevel DefaultImpliedAccessLevel = SecurityLevel.Passenger;
        private Dictionary<Mobile, SecurityLevel> m_Manifest;

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

        public Dictionary<Mobile, SecurityLevel> Manifest { get { return m_Manifest; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPublic { get { return DefaultPublicAccess != SecurityLevel.Denied; } }

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
            writer.Write((int)0);

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

    [PropertyObject]
    public class PilotEntry
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public string Name { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Female { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SkinHue { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HairHue { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HairID { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpeechHue { get; }

        public override string ToString()
        {
            return "...";
        }

        public PilotEntry(Mobile pilot)
        {
            Name = pilot.Name;
            Female = pilot.Female;
            SkinHue = pilot.Hue;
            HairHue = pilot.HairHue;
            HairID = pilot.HairItemID;
            SpeechHue = pilot.SpeechHue;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);
            writer.Write(Name);
            writer.Write(Female);
            writer.Write(SkinHue);
            writer.Write(HairHue);
            writer.Write(HairID);
            writer.Write(SpeechHue);
        }

        public PilotEntry(GenericReader reader)
        {
            int version = reader.ReadInt();
            Name = reader.ReadString();
            Female = reader.ReadBool();
            SkinHue = reader.ReadInt();
            HairHue = reader.ReadInt();
            HairID = reader.ReadInt();
            SpeechHue = reader.ReadInt();
        }
    }

    public class ShipAccessEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private Mobile m_Clicker;
        private BaseGalleon m_Galleon;

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
