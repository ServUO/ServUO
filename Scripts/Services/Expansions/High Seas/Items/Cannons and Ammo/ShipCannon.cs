using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public enum CannonAction
    {
        None,
        Stop,
        Fail,
        Finish
    }

    public abstract class BaseShipCannon : Container, IShipCannon
    {
        private int m_Hits;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits { get { return m_Hits; } set { m_Hits = value; InvalidateDamageState(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Processing { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonAction Prepered { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonAction Charged { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonAction Loaded { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonAction Primed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AmmunitionType AmmoType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipPosition Position { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DamageLevel DamageState { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing => GetFacing();

        public virtual bool HitMultipleMobs => false;

        public abstract ShipCannonDeed GetDeed { get; }
        public abstract int Range { get; }
        public abstract CannonPower Power { get; }

        public virtual int MaxHits => 100;
        public virtual TimeSpan ActionTime => TimeSpan.FromSeconds(1.5);

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanLight => Prepered == CannonAction.Finish && Loaded == CannonAction.Finish && Charged == CannonAction.Finish && Primed == CannonAction.Finish;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Empty => !CanLight && Items.Count == 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public double Durability => (m_Hits / (double)MaxHits) * 100.0;

        public override bool ForceShowProperties => true;
        public override int DefaultGumpID => 0x9CE7;
        public override bool DisplaysContent => false;
        public override int DefaultMaxWeight => 300;

        public List<Mobile> Viewing { get; set; } = new List<Mobile>();

        public BaseShipCannon(BaseGalleon galleon)
            : base(0x2198)
        {
            Movable = false;
            Galleon = galleon;
            AmmoType = AmmunitionType.Empty;
            m_Hits = MaxHits;
            DamageState = DamageLevel.Pristine;

            LiftOverride = true;
            MaxItems = 3;
        }

        public void TryPrep(Mobile m)
        {
            Processing = true;

            m.Animate(AnimationType.Attack, 3);

            if (Prepered == CannonAction.Stop)
                AddAction(m, 1149679); // Preparation resumed.
            else
                AddAction(m, 1149641); // Preparing to fire...

            Timer.DelayCall(TimeSpan.FromSeconds(4.0), () =>
            {
                bool ramrod = m.Backpack.GetAmount(typeof(Ramrod)) >= 1;

                if (ramrod)
                {
                    if (Galleon.Contains(m))
                    {
                        Prepered = CannonAction.Finish;
                        TryCharge(m);
                        ResendGump(m);
                    }
                    else
                    {
                        Processing = false;
                        Prepered = CannonAction.Stop;
                        AddAction(m, 1149675); // Preparation stopped.
                        ResendGump(m);
                    }
                }
                else
                {
                    Processing = false;
                    AddAction(m, 1149660); // You need a ramrod.
                    m.SendLocalizedMessage(1149660); // You need a ramrod.
                    ResendGump(m);
                }
            });
        }

        public void TryCharge(Mobile m)
        {
            Processing = true;

            m.Animate(AnimationType.Attack, 3);

            if (Charged == CannonAction.Stop)
                AddAction(m, 1149680); // Charging resumed.
            else
                AddAction(m, 1149644); // Charging started.

            Timer.DelayCall(ActionTime, () =>
            {
                PowderCharge charge = FindItemByType<PowderCharge>();

                if (charge != null)
                {
                    if (Galleon.Contains(m))
                    {
                        charge.Consume();
                        Charged = CannonAction.Finish;
                        AddAction(m, 1149646); // Charging finished.
                        InvalidateProperties();

                        TryLoad(m);
                        ResendGump(m);
                    }
                    else
                    {
                        Processing = false;
                        Charged = CannonAction.Stop;
                        AddAction(m, 1149676); // Charging stopped.
                        DoAreaMessage(1116052, 10, null); // The effort to charge the cannon has paused.
                        ResendGump(m);
                    }
                }
                else
                {
                    Processing = false;
                    m.SendLocalizedMessage(1116014); // The magazine does not have a powder charge to charge this cannon with.

                    AddAction(m, 1149665); // Need powder charge.
                    ResendGump(m);
                }
            });
        }

        public void TryLoad(Mobile m)
        {
            Processing = true;

            m.Animate(AnimationType.Attack, 3);

            if (Loaded == CannonAction.Stop)
                AddAction(m, 1149681); // Loading resumed.
            else
                AddAction(m, 1149647); // Loading started.

            AmmoType = AmmunitionType.Empty;

            Timer.DelayCall(ActionTime, () =>
            {
                ICannonAmmo ammo = null;

                Cannonball cannon = FindItemByType<Cannonball>();
                Grapeshot grapeshot = FindItemByType<Grapeshot>();

                if (cannon != null)
                {
                    ammo = cannon;
                }
                else if (grapeshot != null)
                {
                    ammo = grapeshot;
                }
                else
                {
                    cannon = m.Backpack.FindItemByType<Cannonball>();
                    grapeshot = m.Backpack.FindItemByType<Grapeshot>();

                    if (cannon != null)
                    {
                        ammo = cannon;
                    }
                    else if (grapeshot != null)
                    {
                        ammo = grapeshot;
                    }
                }

                if (ammo != null && ammo is Item)
                {
                    if (Galleon.Contains(m))
                    {
                        Loaded = CannonAction.Finish;
                        AmmoType = ammo.AmmoType;
                        ((Item)ammo).Consume();
                        InvalidateProperties();
                        AddAction(m, 1149649); // Loading finished.

                        TryPrime(m);
                        ResendGump(m);
                    }
                    else
                    {
                        Processing = false;
                        Loaded = CannonAction.Stop;
                        DoAreaMessage(1116053, 10, null); // The effort to load the cannon has paused.
                        AddAction(m, 1149677); // Loading stopped.
                        ResendGump(m);
                    }
                }
                else
                {
                    Processing = false;
                    Loaded = CannonAction.Fail;
                    m.SendLocalizedMessage(1158933); // The magazine does not have ammo to load this cannon with.
                    AddAction(m, 1158933); // Need ammo.
                    ResendGump(m);
                }
            });
        }

        public void TryPrime(Mobile m)
        {
            Processing = true;

            m.Animate(AnimationType.Attack, 3);

            if (Primed == CannonAction.Stop)
                AddAction(m, 1149682); // Priming resumed.
            else
                AddAction(m, 1149650); // Priming started.

            Timer.DelayCall(ActionTime, () =>
            {
                FuseCord fuse = FindItemByType<FuseCord>();

                if (fuse != null)
                {
                    if (Galleon.Contains(m))
                    {
                        fuse.Consume();
                        Primed = CannonAction.Finish;
                        InvalidateProperties();

                        AddAction(m, 1149652); // Ready to fire.
                        ResendGump(m);
                    }
                    else
                    {
                        Primed = CannonAction.Stop;
                        DoAreaMessage(1116053, 10, null); // The effort to load the cannon has paused.
                        AddAction(m, 1149678); // Priming stopped.
                        ResendGump(m);
                    }
                }
                else
                {
                    Primed = CannonAction.Fail;
                    AddAction(m, 1149661); // You need fuse.
                    m.SendLocalizedMessage(1158939); // The magazine does not have a fuse to prime the cannon with.
                    ResendGump(m);
                }

                Processing = false;
            });
        }

        public void Unload(Mobile m)
        {
            Item item;

            if (Primed == CannonAction.Finish)
            {
                item = new FuseCord();

                if (!TryDropItem(m, item, false))
                {
                    m.AddToBackpack(item);
                }

                AddAction(m, 1149686); // Fuse removed.
                Primed = CannonAction.None;
            }

            if (Loaded == CannonAction.Finish)
            {
                switch (AmmoType)
                {
                    default: item = null; break;
                    case AmmunitionType.Grapeshot: item = new Cannonball(); break;
                    case AmmunitionType.Cannonball: item = new Cannonball(); break;
                    case AmmunitionType.FrostCannonball: item = new FrostCannonball(); break;
                    case AmmunitionType.FlameCannonball: item = new FlameCannonball(); break;
                }

                if (item != null)
                {
                    AddAction(m, 1149685); // Ammunition removed.

                    if (!TryDropItem(m, item, false))
                    {
                        m.AddToBackpack(item);
                    }
                }

                AmmoType = AmmunitionType.Empty;
                Loaded = CannonAction.None;
            }

            if (Charged == CannonAction.Finish)
            {
                item = new PowderCharge();

                if (!TryDropItem(m, item, false))
                {
                    m.AddToBackpack(item);
                }

                AddAction(m, 1149684); // Powder charge removed.
                Charged = CannonAction.None;
            }
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!CheckType(item))
            {
                if (message)
                {
                    m.SendLocalizedMessage(1074836); // The container can not hold that type of object.
                }

                return false;
            }
            else if (Items.Count == 3)
            {
                if (message)
                {
                    m.SendLocalizedMessage(1080017); // That container cannot hold more items.
                }

                return false;
            }

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        private bool CheckType(Item item)
        {
            return _Types.Any(t => t == item.GetType() || item.GetType().IsSubclassOf(t));
        }

        private readonly Type[] _Types =
        {
            typeof(Cannonball), typeof(Grapeshot), typeof(PowderCharge), typeof(FuseCord)
        };

        public override void OnDoubleClickDead(Mobile m)
        {
            OnDoubleClick(m);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon.GetSecurityLevel(from) >= SecurityLevel.Crewman)
            {
                base.OnDoubleClick(from);

                if (!Viewing.Contains(from))
                {
                    Viewing.Add(from);
                }

                AddAction(from, from.Alive && Galleon.Contains(from) && from.InRange(Location, 2) ? 1149653 : 1149654); // You are now operating the cannon. : You are too far away.
                ResendGump(from, TimeSpan.FromMilliseconds(500));
            }
            else
            {
                from.Say(1010436); // You do not have permission to do this.
            }
        }

        public Direction GetFacing()
        {
            if (BaseGalleon.CannonIDs[0].Any(id => id == ItemID))
            {
                return Direction.South;
            }
            if (BaseGalleon.CannonIDs[1].Any(id => id == ItemID))
            {
                return Direction.West;
            }
            if (BaseGalleon.CannonIDs[2].Any(id => id == ItemID))
            {
                return Direction.North;
            }

            return Direction.East;
        }

        public void DoAreaMessage(int cliloc, int range, Mobile from)
        {
            if (from == null)
                return;

            Galleon.GetEntitiesOnBoard().OfType<PlayerMobile>().Where(x => x != from && Galleon.GetSecurityLevel(x) > SecurityLevel.Denied)
                .ToList().ForEach(y =>
                {
                    if (from != null)
                        y.SendLocalizedMessage(cliloc, from.Name);
                    else
                        y.SendLocalizedMessage(cliloc);
                });
        }

        public void TryLightFuse(Mobile from)
        {
            if (from == null)
                return;

            Container pack = from.Backpack;

            if (pack != null)
            {
                Item[] items = pack.FindItemsByType(typeof(Matches));

                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        if (item is Matches && ((Matches)item).IsLight)
                        {
                            LightFuse(from);
                            return;
                        }
                    }
                }

                items = pack.FindItemsByType(typeof(Torch));

                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        if (item is Torch && ((Torch)item).Burning)
                        {
                            LightFuse(from);
                            return;
                        }
                    }
                }
            }

            Item i = from.FindItemOnLayer(Layer.TwoHanded);

            if (i != null && i is Matches && ((Matches)i).IsLight)
            {
                LightFuse(from);
                return;
            }
            else if (i != null && i is Torch && ((Torch)i).Burning)
            {
                LightFuse(from);
                return;
            }

            AddAction(from, 1149669); // Need a lighted fire source.
            from.SendLocalizedMessage(1149669); // Need a lighted fire source.
        }

        public void LightFuse(Mobile from)
        {
            AddAction(from, 1149683); // The fuse is lit!
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), Shoot, from);
        }

        public virtual void Shoot(object cannoneer)
        {
            AmmoInfo ammo = AmmoInfo.GetAmmoInfo(AmmoType);

            if (ammo == null)
                return;

            Mobile shooter = null;

            if (cannoneer is Mobile)
                shooter = (Mobile)cannoneer;

            if (shooter != null && shooter.Player)
                m_Hits -= Utility.RandomMinMax(0, 4);

            DoShootEffects();
            AddAction(shooter, 1149691); // Fired successfully.

            int xOffset = 0; int yOffset = 0;
            int currentRange = 0;
            Point3D pnt = Location;
            Map map = Map;
            Direction d = GetFacing();

            switch (d)
            {
                case Direction.North:
                    xOffset = 0; yOffset = -1; break;
                case Direction.South:
                    xOffset = 0; yOffset = 1; break;
                case Direction.West:
                    xOffset = -1; yOffset = 0; break;
                case Direction.East:
                    xOffset = 1; yOffset = 0; break;
            }

            int xo = xOffset;
            int yo = yOffset;
            int lateralOffset = 1;

            int latDist = ammo.LateralOffset;

            int range = Range;
            bool hit = false;

            while (currentRange++ <= range)
            {
                xOffset = xo;
                yOffset = yo;

                if (currentRange % latDist == 0)
                    lateralOffset++;

                TimeSpan delay = TimeSpan.FromSeconds(currentRange / 10.0);

                switch (AmmoType)
                {
                    case AmmunitionType.Empty: break;
                    case AmmunitionType.Cannonball:
                    case AmmunitionType.FrostCannonball:
                    case AmmunitionType.FlameCannonball:
                        {
                            Point3D newPoint = pnt;
                            List<IEntity> list = new List<IEntity>();
                            List<IDamageable> damageables = new List<IDamageable>();

                            for (int i = -lateralOffset; i <= lateralOffset; i++)
                            {
                                if (xOffset == 0)
                                    newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                                else
                                    newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                                BaseBoat b = FindValidBoatTarget(newPoint, map, ammo);

                                if (b != null && b != Galleon && b.IsEnemy(Galleon))
                                    list.Add(b);

                                damageables.AddRange(FindDamageables(shooter, newPoint, map, false, false, false, true, true));
                            }

                            foreach (IDamageable m in damageables)
                            {
                                list.Add(m);
                            }

                            if (list.Count > 0)
                            {
                                IEntity toHit = list[Utility.Random(list.Count)];

                                if (toHit is Mobile)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnMobileHit), new object[] { (Mobile)toHit, newPoint, ammo, shooter });
                                    hit = true;
                                }
                                else if (toHit is BaseBoat)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnShipHit), new object[] { (BaseBoat)toHit, newPoint, ammo, shooter });
                                    hit = true;
                                }
                                else if (toHit is DamageableItem)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnDamageableItemHit), new object[] { (DamageableItem)toHit, newPoint, ammo, shooter });
                                    hit = true;
                                }
                            }
                        }
                        break;
                    case AmmunitionType.Grapeshot:
                        {
                            Point3D newPoint = pnt;
                            List<IEntity> list = new List<IEntity>();
                            List<IDamageable> damageables = new List<IDamageable>();

                            for (int i = -lateralOffset; i <= lateralOffset; i++)
                            {
                                if (xOffset == 0)
                                    newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                                else
                                    newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                                BaseBoat b = FindValidBoatTarget(newPoint, map, ammo);

                                if (b != null && b != Galleon && b.IsEnemy(Galleon))
                                    list.Add(b);

                                damageables.AddRange(FindDamageables(shooter, newPoint, map, true, true, false, true, true));
                            }

                            foreach (IDamageable m in damageables)
                            {
                                list.Add(m);
                            }

                            if (list.Count > 0)
                            {
                                IEntity toHit = list[Utility.Random(list.Count)];

                                if (toHit is Mobile)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnMobileHit), new object[] { (Mobile)toHit, newPoint, ammo, shooter });
                                    hit = true;
                                }
                                else if (toHit is BaseBoat)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnShipHit), new object[] { (BaseBoat)toHit, newPoint, ammo, shooter });
                                    hit = true;
                                }
                                else if (toHit is DamageableItem)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnDamageableItemHit), new object[] { (DamageableItem)toHit, newPoint, ammo, shooter });
                                    hit = true;
                                }
                            }
                        }
                        break;
                }

                if (hit && ammo.SingleTarget)
                    break;
            }

            ClearCannon();
            InvalidateDamageState();

            if (shooter != null)
            {
                ResendGump(shooter);
            }
        }

        private BaseBoat FindValidBoatTarget(Point3D newPoint, Map map, AmmoInfo info)
        {
            BaseBoat boat = BaseBoat.FindBoatAt(newPoint, map);

            if (boat != null && info.RequiresSurface)
            {
                int d = boat is BritannianShip ? 3 : 2;
                switch (boat.Facing)
                {
                    case Direction.North:
                    case Direction.South:
                        if (newPoint.X <= boat.X - d || newPoint.X >= boat.X + d)
                            return null;
                        break;
                    case Direction.East:
                    case Direction.West:
                        if (newPoint.Y <= boat.Y - d || newPoint.Y >= boat.Y + d)
                            return null;
                        break;
                }

                StaticTile[] tiles = map.Tiles.GetStaticTiles(newPoint.X, newPoint.Y, true);

                foreach (StaticTile tile in tiles)
                {
                    ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];
                    bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                    if (!isWater && id.Surface && !id.Impassable)
                    {
                        return boat;
                    }
                }

                return null;
            }

            return boat;
        }

        public void DoShootEffects()
        {
            Point3D p = Location;
            Map map = Map;

            p.Z -= 3;

            switch (Facing)
            {
                case Direction.North: p.Y--; break;
                case Direction.East: p.X++; break;
                case Direction.South: p.Y++; break;
                case Direction.West: p.X--; break;
            }

            Effects.SendPacket(p, map, new GraphicalEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x3728, p, p, 14, 14, true, true));
            Effects.PlaySound(Location, map, 0x11C);
        }

        public void InvalidateDamageState()
        {
            InvalidateDamageState(null);
        }

        public void InvalidateDamageState(Mobile from)
        {
            if (Durability >= 100)
                DamageState = DamageLevel.Pristine;
            else if (Durability >= 75.0)
                DamageState = DamageLevel.Slightly;
            else if (Durability >= 50.0)
                DamageState = DamageLevel.Moderately;
            else if (Durability >= 25.0)
                DamageState = DamageLevel.Heavily;
            else
                DamageState = DamageLevel.Severely;

            if (Durability <= 0)
            {
                DoAreaMessage(1116297, 5, null); // The ship cannon has been destroyed!
                Delete();

                if (from != null && from.InRange(Location, 5))
                    from.SendLocalizedMessage(1116297); // The ship cannon has been destroyed!
            }

            InvalidateProperties();
        }

        public void ClearCannon()
        {
            Prepered = CannonAction.None;
            Charged = CannonAction.None;
            Loaded = CannonAction.None;
            AmmoType = AmmunitionType.Empty;
            Primed = CannonAction.None;

            InvalidateProperties();
        }

        public virtual void OnShipHit(object obj)
        {
            object[] list = (object[])obj;
            BaseBoat target = list[0] as BaseBoat;
            Point3D pnt = (Point3D)list[1];
            AmmoInfo ammoInfo = list[2] as AmmoInfo;
            Mobile shooter = list[3] as Mobile;

            if (target != null && Galleon != null)
            {
                int z = target.ZSurface;

                if (target.TillerMan != null && target.TillerMan is IEntity)
                {
                    z = ((IEntity)target.TillerMan).Z;
                }

                Direction d = Utility.GetDirection(this, pnt);
                int xOffset = 0;
                int yOffset = 0;
                Point3D hit = pnt;

                if (!ammoInfo.RequiresSurface)
                {
                    switch (d)
                    {
                        default:
                        case Direction.North:
                            xOffset = Utility.RandomMinMax(-1, 1);
                            yOffset = Utility.RandomMinMax(-2, 0);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                        case Direction.South:
                            xOffset = Utility.RandomMinMax(-1, 1);
                            yOffset = Utility.RandomMinMax(0, 2);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                        case Direction.East:
                            xOffset = Utility.RandomMinMax(0, 2);
                            yOffset = Utility.RandomMinMax(-1, 1);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                        case Direction.West:
                            xOffset = Utility.RandomMinMax(-2, 0);
                            yOffset = Utility.RandomMinMax(-1, 1);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                    }
                }

                int damage = 0;

                if (ammoInfo.AmmoType == AmmunitionType.Grapeshot)
                {
                    for (int count = 15; count > 0; count--)
                    {
                        damage = (int)(ammoInfo.GetDamage(this) * Galleon.CannonDamageMod);
                        Point3D loc = new Point3D(hit.X + Utility.RandomMinMax(0, 4), hit.Y + Utility.RandomMinMax(0, 4), hit.Z);
                        Effects.SendPacket(loc, target.Map, new GraphicalEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x36CB, loc, loc, 15, 15, true, true));
                        target.OnTakenDamage(shooter, damage);
                        MobileOnBoardDamage(shooter, loc, ammoInfo);
                    }
                }
                else
                {
                    damage = (int)(ammoInfo.GetDamage(this) * Galleon.CannonDamageMod);
                    Effects.SendPacket(hit, target.Map, new GraphicalEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x36CB, hit, hit, 15, 15, true, true));
                    target.OnTakenDamage(shooter, damage);
                }

                if (Galleon.Map != null)
                {
                    IPooledEnumerable eable = Galleon.Map.GetItemsInRange(hit, 1);

                    foreach (Item item in eable)
                    {
                        if (item is IShipCannon && !Galleon.Contains(item))
                        {
                            ((IShipCannon)item).OnDamage(damage, shooter);
                        }
                    }

                    eable.Free();
                }
            }
        }

        public void MobileOnBoardDamage(Mobile shooter, Point3D pnt, AmmoInfo info)
        {
            List<IDamageable> list = new List<IDamageable>();

            if (Map == null || Map == Map.Internal || Galleon == null)
                return;

            IPooledEnumerable eable = Map.GetObjectsInRange(pnt, 0);

            foreach (IDamageable dam in eable.OfType<IDamageable>())
            {
                Mobile mob = dam as Mobile;

                if (mob != null && (!shooter.CanBeHarmful(mob, false) || Galleon.Contains(mob)))
                    continue;

                if (mob is PlayerMobile || mob is BaseCreature)
                {
                    shooter.DoHarmful(mob);
                    AOS.Damage(mob, shooter, 35, info.PhysicalDamage, info.FireDamage, info.ColdDamage, info.PoisonDamage, info.EnergyDamage);
                }
            }

            eable.Free();
        }

        public virtual void OnMobileHit(object obj)
        {
            object[] objects = (object[])obj;
            Mobile toHit = objects[0] as Mobile;
            Point3D pnt = (Point3D)objects[1];
            AmmoInfo info = objects[2] as AmmoInfo;
            Mobile shooter = objects[3] as Mobile;

            int damage = (int)(Utility.RandomMinMax(info.MinDamage, info.MaxDamage) * Galleon.CannonDamageMod);

            if (info == null)
                return;

            if (toHit != null)
            {
                Effects.SendPacket(toHit.Location, toHit.Map, new GraphicalEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x36CB, toHit.Location, toHit.Location, 15, 15, true, true));
                shooter.DoHarmful(toHit);
                AOS.Damage(toHit, shooter, damage, info.PhysicalDamage, info.FireDamage, info.ColdDamage, info.PoisonDamage, info.EnergyDamage);
            }
        }

        public virtual void OnDamageableItemHit(object obj)
        {
            object[] objects = (object[])obj;
            DamageableItem toHit = objects[0] as DamageableItem;
            Point3D pnt = (Point3D)objects[1];
            AmmoInfo info = objects[2] as AmmoInfo;
            Mobile shooter = objects[3] as Mobile;

            if (info == null || toHit == null || toHit.Map == null)
                return;

            int damage = (int)(Utility.RandomMinMax(info.MinDamage, info.MaxDamage) * Galleon.CannonDamageMod);

            shooter.DoHarmful(toHit);
            AOS.Damage(toHit, shooter, damage, info.PhysicalDamage, info.FireDamage, info.ColdDamage, info.PoisonDamage, info.EnergyDamage);
            Effects.SendLocationEffect(new Point3D(toHit.X, toHit.Y, toHit.Z + 5), toHit.Map, Utility.RandomBool() ? 14000 : 14013, 15, 10);
            Effects.PlaySound(toHit.Location, toHit.Map, 0x207);
        }

        public List<IDamageable> FindDamageables(Mobile shooter, Point3D pnt, Map map, bool player, bool pet, bool monsters, bool seacreature, bool items)
        {
            List<IDamageable> list = new List<IDamageable>();

            if (map == null || map == Map.Internal || Galleon == null)
                return list;

            IPooledEnumerable eable = map.GetObjectsInRange(pnt, 0);

            foreach (IDamageable dam in eable.OfType<IDamageable>())
            {
                Mobile mob = dam as Mobile;

                if (mob != null && (!shooter.CanBeHarmful(mob, false) || Galleon.Contains(mob)))
                    continue;

                if (!items && dam is DamageableItem)
                    continue;

                if (items && dam is DamageableItem && ((DamageableItem)dam).CanDamage && !Galleon.Contains(dam))
                    list.Add(dam);

                if (player && mob is PlayerMobile)
                    list.Add(mob);

                if (monsters && mob is BaseCreature && !((BaseCreature)mob).Controlled && !((BaseCreature)mob).Summoned)
                    list.Add(mob);

                if (pet && mob is BaseCreature && (((BaseCreature)mob).Controlled || ((BaseCreature)mob).Summoned))
                    list.Add(mob);

                if (seacreature && (mob is BaseSeaChampion || mob is Kraken))
                    list.Add(mob);
            }

            eable.Free();
            return list;
        }

        public void TryRepairCannon(Mobile from)
        {
            Container pack = from.Backpack;
            Container hold = Galleon.GalleonHold;

            if (pack == null)
                return;

            double ingotsNeeded = 36 * (int)DamageState;

            ingotsNeeded -= (from.Skills[SkillName.Blacksmith].Value / 200.0) * ingotsNeeded;

            double min = ingotsNeeded / 10;
            double ingots1 = pack.GetAmount(typeof(IronIngot));
            double ingots2 = hold != null ? hold.GetAmount(typeof(IronIngot)) : 0;
            double ingots = ingots1 + ingots2;
            double ingotsUsed, percRepaired;

            if (ingots < min)
            {
                from.SendLocalizedMessage(1116603, ((int)min).ToString()); //You need a minimum of ~1_METAL~ iron ingots to repair this cannon.
                return;
            }

            if (ingots >= ingotsNeeded)
            {
                ingotsUsed = ingotsNeeded;
                percRepaired = 100;
            }
            else
            {
                ingotsUsed = ingots;
                percRepaired = (ingots / ingotsNeeded) * 100;
            }

            double toConsume = 0;
            double temp = ingotsUsed;

            if (ingotsUsed > 0 && ingots1 > 0)
            {
                toConsume = Math.Min(ingotsUsed, ingots1);
                pack.ConsumeTotal(typeof(IronIngot), (int)toConsume);
                ingotsUsed -= toConsume;
            }

            if (hold != null && ingotsUsed > 0 && ingots2 > 0)
            {
                toConsume = Math.Min(ingotsUsed, ingots2);
                hold.ConsumeTotal(typeof(IronIngot), (int)toConsume);
            }

            m_Hits += (int)((MaxHits - m_Hits) * (percRepaired / 100));
            if (m_Hits > MaxHits) m_Hits = MaxHits;
            InvalidateDamageState();

            percRepaired += Durability;
            if (percRepaired > 100) percRepaired = 100;

            from.SendLocalizedMessage(1116605, string.Format("{0}\t{1}", ((int)temp).ToString(), ((int)percRepaired).ToString())); //You make repairs to the cannon using ~1_METAL~ ingots. The cannon is now ~2_DMGPCT~% repaired.
        }

        public void ResendGump(Mobile from)
        {
            ResendGump(from, TimeSpan.Zero);
        }

        public void ResendGump(Mobile from, TimeSpan delay)
        {
            if (!Galleon.Contains(from))
            {
                Viewing.Remove(from);
            }

            if (!Viewing.Contains(from))
            {
                Viewing.Add(from);
            }

            foreach (PlayerMobile pm in Viewing.OfType<PlayerMobile>())
            {
                ShipCannonGump gump = BaseGump.GetGump<ShipCannonGump>(pm, g => g.Cannon == this);

                if (gump != null)
                {
                    if (delay != TimeSpan.Zero)
                    {
                        Timer.DelayCall(delay, () => gump.Refresh());
                    }
                    else
                    {
                        gump.Refresh();
                    }
                }
                else
                {
                    if (delay != TimeSpan.Zero)
                    {
                        Timer.DelayCall(delay, () => BaseGump.SendGump(new ShipCannonGump(pm, this)));
                    }
                    else
                    {
                        BaseGump.SendGump(new ShipCannonGump(pm, this));
                    }
                }
            }
        }

        public void OnDamage(int damage, Mobile from)
        {
            m_Hits -= damage;
            InvalidateDamageState(from);
        }

        public Dictionary<Mobile, List<int>> Actions => m_Actions;
        private readonly Dictionary<Mobile, List<int>> m_Actions = new Dictionary<Mobile, List<int>>();

        public void AddAction(Mobile from, int action)
        {
            if (from == null)
                return;

            if (!m_Actions.ContainsKey(from))
                m_Actions[from] = new List<int>();

            List<int> list = m_Actions[from];

            list.Insert(0, action);

            while (list.Count > 3)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1116026, Charged == CannonAction.Finish ? "#1116031" : "#1116032"); // Charged: ~1_VALUE~
            list.Add(1116027, string.Format("{0}", AmmoInfo.GetAmmoName(this).ToString())); // Ammo: ~1_VALUE~
            list.Add(1116028, Primed == CannonAction.Finish ? "#1116031" : "#1116032"); //Primed: ~1_VALUE~
            list.Add(1116580 + (int)DamageState);
            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", TotalItems, MaxItems, TotalWeight, MaxWeight);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (Galleon.GetSecurityLevel(from) >= SecurityLevel.Officer)
            {
                list.Add(new UnloadContext(this, from));
                list.Add(new DismantleContext(this, from));
                list.Add(new RepairContext(this, from));
            }
        }

        private class UnloadContext : ContextMenuEntry
        {
            public Mobile From { get; set; }
            public BaseShipCannon Cannon { get; set; }

            public UnloadContext(BaseShipCannon cannon, Mobile from)
                : base(1116072, 2) // Unload
            {
                From = from;
                Cannon = cannon;

                Enabled = cannon.CanLight;
            }

            public override void OnClick()
            {
                Cannon.Unload(From);
                Cannon.ResendGump(From);
            }
        }

        private class DismantleContext : ContextMenuEntry
        {
            public Mobile From { get; set; }
            public BaseShipCannon Cannon { get; set; }

            public DismantleContext(BaseShipCannon cannon, Mobile from)
                : base(1116069, 2)
            {
                From = from;
                Cannon = cannon;

                Enabled = cannon.Empty && Cannon.DamageState == DamageLevel.Pristine;
            }

            public override void OnClick()
            {
                ShipCannonDeed deed = Cannon.GetDeed;

                Cannon.DoAreaMessage(1116073, 10, From); //~1_NAME~ dismantles the ship cannon.
                Cannon.Delete();

                Container pack = From.Backpack;

                if (pack == null || !pack.TryDropItem(From, deed, false))
                {
                    deed.MoveToWorld(From.Location, From.Map);
                }
            }

            public override void OnClickDisabled()
            {
                if (Cannon.Galleon.GetSecurityLevel(From) != SecurityLevel.Captain)
                {
                    From.SendLocalizedMessage(1149693); // You must own the ship to do that.
                }
                else if (!Cannon.Empty)
                {
                    From.SendLocalizedMessage(1116321); // The ship cannon and magazine must be fully unloaded before it can be dismantled.
                }
                else if (Cannon.DamageState != DamageLevel.Pristine)
                {
                    From.SendLocalizedMessage(1116322); // The ship cannon must be fully repaired before it can be dismantled.
                }
            }
        }

        private class RepairContext : ContextMenuEntry
        {
            public Mobile From { get; set; }
            public BaseShipCannon Cannon { get; set; }

            public RepairContext(BaseShipCannon cannon, Mobile from)
                : base(1116602, 2)
            {
                From = from;
                Cannon = cannon;

                Enabled = Cannon.DamageState != DamageLevel.Pristine;
            }

            public override void OnClick()
            {
                Cannon.TryRepairCannon(From);
            }

            public override void OnClickDisabled()
            {
                From.SendLocalizedMessage(1116604); //The cannon is in pristine condition and does not need repairs.
            }
        }

        public override void Delete()
        {
            if (Galleon != null)
            {
                Galleon.RemoveCannon(this);
            }

            List<PlayerMobile> list = new List<PlayerMobile>(Viewing.OfType<PlayerMobile>());

            foreach (PlayerMobile pm in list)
            {
                ShipCannonGump gump = BaseGump.GetGump<ShipCannonGump>(pm, g => g.Cannon == this);

                if (gump != null)
                {
                    gump.Close();
                }
            }

            ColUtility.Free(list);
            base.Delete();
        }

        public BaseShipCannon(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)Prepered);
            writer.Write(Galleon);
            writer.Write((int)Charged);
            writer.Write((int)Loaded);
            writer.Write((int)Primed);
            writer.Write((int)AmmoType);
            writer.Write((int)Position);

            writer.Write(m_Hits);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        Prepered = (CannonAction)reader.ReadInt();
                        Galleon = reader.ReadItem() as BaseGalleon;
                        Charged = (CannonAction)reader.ReadInt();
                        Loaded = (CannonAction)reader.ReadInt();
                        Primed = (CannonAction)reader.ReadInt();
                        AmmoType = (AmmunitionType)reader.ReadInt();
                        Position = (ShipPosition)reader.ReadInt();
                        m_Hits = reader.ReadInt();

                        return;
                    }
                case 0:
                    {
                        Galleon = reader.ReadItem() as BaseGalleon;

                        if (reader.ReadBool())
                            Charged = CannonAction.Finish;

                        if (reader.ReadBool())
                            Loaded = CannonAction.Finish;

                        if (reader.ReadBool())
                            Primed = CannonAction.Finish;

                        AmmoType = (AmmunitionType)reader.ReadInt();
                        Position = (ShipPosition)reader.ReadInt();

                        m_Hits = reader.ReadInt();

                        break;
                    }
            }

            InvalidateDamageState();
        }

        public class ShipCannonGump : BaseGump
        {
            public BaseShipCannon Cannon { get; set; }

            public ShipCannonGump(PlayerMobile pm, BaseShipCannon cannon)
                : base(pm, 100, 100)
            {
                Cannon = cannon;
                TypeID = Cannon.Serial;
            }

            public override void AddGumpLayout()
            {
                AddBackground(0, 0, 250, 175, 0x6DB);
                AddHtmlLocalized(10, 10, 230, 18, 1149614 + (int)Cannon.Position, 0x3DFF, false, false);
                AddHtmlLocalized(115, 35, 70, 18, 1158934, 0x7FE7, false, false); // STATUS

                if (Cannon.CanLight)
                {
                    AddButton(10, 35, 0xFA5, 0xFA7, 8, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 35, 70, 18, 1149985, 0x7FFF, false, false); // UNLOAD

                    AddButton(10, 89, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 89, 70, 18, 1149638, 0x7FFF, false, false); // FIRE
                }
                else
                {
                    AddButton(10, 35, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 35, 70, 18, 1158890, 0x7FFF, false, false); // PREP
                }

                AddHtmlLocalized(115, 53, 115, 18, Cannon.Charged == CannonAction.Finish ? 1149631 : 1149632, Cannon.Charged == CannonAction.Finish ? 0x1FE7 : 0x7CE7, false, false); // Charged / Not Charged
                AddHtmlLocalized(115, 71, 115, 18, 1114057, Cannon.Loaded == CannonAction.Finish ? AmmoInfo.GetAmmoName(Cannon).ToString() : "#1149636", Cannon.Loaded == CannonAction.Finish ? 0x1FE7 : 0x7CE7, false, false); // Cannonball / Not Loaded
                AddHtmlLocalized(115, 89, 115, 18, Cannon.Primed == CannonAction.Finish ? 1149640 : 1149639, Cannon.Primed == CannonAction.Finish ? 0x1FE7 : 0x7CE7, false, false); // Primed / No Fuse

                if (Cannon.Actions.ContainsKey(User))
                {
                    int actual = 0;
                    List<int> list = Cannon.Actions[User];

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        AddHtmlLocalized(10, 112 + (actual * 18), 230, 18, Cannon.Actions[User][i], actual == list.Count - 1 ? 0x7FE7 : 0x3DEF, false, false);
                        actual++;
                    }
                }
            }

            public override void OnResponse(RelayInfo info)
            {
                if (info.ButtonID == 0 || !Cannon.Galleon.Contains(User))
                {
                    if (Cannon.Viewing.Contains(User))
                    {
                        Cannon.Viewing.Remove(User);
                    }

                    return;
                }

                if (User.InRange(Cannon.Location, 2) && User.Alive)
                {
                    switch (info.ButtonID)
                    {
                        case 1:
                            {
                                if (!Cannon.Processing)
                                {
                                    if (Cannon.Prepered != CannonAction.Finish)
                                    {
                                        Cannon.TryPrep(User);
                                    }
                                    else if (Cannon.Charged != CannonAction.Finish)
                                    {
                                        Cannon.TryCharge(User);
                                    }
                                    else if (Cannon.Loaded != CannonAction.Finish)
                                    {
                                        Cannon.TryLoad(User);
                                    }
                                    else if (Cannon.Primed != CannonAction.Finish)
                                    {
                                        Cannon.TryPrime(User);
                                    }
                                }
                                break;
                            }
                        case 6:
                            if (Cannon.CanLight)
                            {
                                Cannon.TryLightFuse(User);
                            }
                            break;
                        case 8:
                            Cannon.Unload(User);
                            break;
                    }
                }
                else
                {
                    Cannon.AddAction(User, 1149654); // You are too far away.
                }

                Refresh();
            }

            public override void OnServerClose(NetState owner)
            {
                base.OnServerClose(owner);

                if (owner.Mobile != null && Cannon != null && Cannon.Viewing.Contains(owner.Mobile))
                {
                    Cannon.Viewing.Remove(owner.Mobile);
                }
            }
        }
    }

    public class Culverin : BaseShipCannon
    {
        public override int Range => 10;
        public override ShipCannonDeed GetDeed => new CulverinDeed();
        public override CannonPower Power => CannonPower.Light;

        public Culverin(BaseGalleon g) : base(g)
        {
        }

        public Culverin(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Carronade : BaseShipCannon
    {
        public override int Range => 10;
        public override ShipCannonDeed GetDeed => new CarronadeDeed();
        public override CannonPower Power => CannonPower.Heavy;

        public override TimeSpan ActionTime => TimeSpan.FromSeconds(2.0);

        public Carronade(BaseGalleon g) : base(g)
        {
        }

        public Carronade(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Blundercannon : BaseShipCannon
    {
        public override int LabelNumber => 1158942;  // Blundercannon

        public override int Range => 12;
        public override ShipCannonDeed GetDeed => new BlundercannonDeed();
        public override CannonPower Power => CannonPower.Massive;

        public override TimeSpan ActionTime => TimeSpan.FromSeconds(2.0);

        public Blundercannon(BaseGalleon g) : base(g)
        {
        }

        public Blundercannon(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
