using System.Collections.Generic;
using System;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Targeting;
using Server.Gumps;
using Server.Misc;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseShipCannon : Container, IShipCannon
    {
        private int m_Hits;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hits { get { return m_Hits; } set { m_Hits = value; InvalidateDamageState(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Charged { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Loaded { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Primed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AmmunitionType AmmoType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShipPosition Position { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DamageLevel DamageState { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing { get { return GetFacing(); } }

        public virtual bool HitMultipleMobs { get { return false; } }

        public abstract ShipCannonDeed GetDeed { get; }
        public abstract int Range { get; }
        public abstract CannonPower Power { get; }

        public virtual int MaxHits { get { return 100; } }
        public virtual TimeSpan ActionTime { get { return TimeSpan.FromSeconds(1.5); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanLight { get { return Loaded && Charged && Primed; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Empty { get { return !Loaded && !Charged && !Primed && Items.Count == 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Durability { get { return ((double)m_Hits / (double)MaxHits) * 100.0; } }

        public override bool ForceShowProperties { get { return true; } }
        public override int DefaultGumpID { get { return 0x9CE7; } }
        public override bool DisplaysContent { get { return false; } }
        public override int DefaultMaxWeight { get { return 300; } }

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

        public void TryCharge(Mobile m)
        {
            if (Charged)
            {
                TryLoad(m);
            }
            else
            {
                AddAction(m, 1149644); // Charging started.
                Timer.DelayCall(ActionTime, () =>
                {
                    var charge = FindItemByType<PowderCharge>();

                    if (charge != null)
                    {
                        if (m.InRange(Location, 2))
                        {
                            charge.Consume();
                            Charged = true;
                            AddAction(m, 1149646); // Charging Finished
                            DoAreaMessage(1116061, 10, m); //~1_NAME~ finishes charging the cannon.
                            InvalidateProperties();

                            TryLoad(m);
                            ResendGump(m);
                        }
                        else
                        {
                            DoAreaMessage(1116056, 10, m); //~1_NAME~ cancels the effort of charging the cannon and retrieves the powder charge.
                            AddAction(m, 1149645); // Charging Canceled
                            ResendGump(m);
                        }
                    }
                    else
                    {
                        m.SendLocalizedMessage(1116014); // The magazine does not have a powder charge to charge this cannon with.

                        AddAction(m, 1149665); // Need Powder Charge
                        ResendGump(m);
                    }
                });
            }
        }

        public void TryLoad(Mobile m)
        {
            if (Loaded)
            {
                TryPrime(m);
            }
            else
            {
                AddAction(m, 1149647); // loading started
                AmmoType = AmmunitionType.Empty;

                Timer.DelayCall(ActionTime, () =>
                {
                    ICannonAmmo ammo = null;

                    var cannon = FindItemByType<Cannonball>();
                    var grapeshot = FindItemByType<Grapeshot>();

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
                        if (m.InRange(Location, 2))
                        {
                            Loaded = true;
                            AmmoType = ammo.AmmoType;
                            ((Item)ammo).Consume();
                            InvalidateProperties();
                            AddAction(m, 1149649); // Loading Finished

                            TryPrime(m);
                            ResendGump(m);
                        }
                        else
                        {
                            DoAreaMessage(1116057, 10, m); //~1_NAME~ cancels the effort of loading the cannon and retrieves the cannonball.
                            AddAction(m, 1149648); // Loading canceled.
                            ResendGump(m);
                        }
                    }
                    else
                    {
                        m.SendLocalizedMessage(1158933); // The magazine does not have ammo to load this cannon with.
                        AddAction(m, 1158933); // Need ammo.
                        ResendGump(m);
                    }
                });
            }
        }

        public void TryPrime(Mobile m)
        {
            if (!Primed)
            {
                Timer.DelayCall(ActionTime, () =>
                {
                    AddAction(m, 1149650); // priming started
                    var fuse = FindItemByType<FuseCord>();

                    if (fuse != null)
                    {
                        if (m.InRange(Location, 2))
                        {
                            fuse.Consume();
                            Primed = true;
                            InvalidateProperties();

                            DoAreaMessage(1116064, 10, m); //~1_NAME~ finishes priming the cannon. It is ready to be fired!
                            AddAction(m, 1149652); // Ready to fire.
                            ResendGump(m);
                        }
                        else
                        {
                            DoAreaMessage(1116059, 10, m); //~1_NAME~ cancels the effort of priming the cannon and retrieves the cannon fuse.
                            AddAction(m, 1149651); // priming canceled
                            ResendGump(m);
                        }
                    }
                    else
                    {
                        AddAction(m, 1149661); // You need fuse.
                        ResendGump(m);
                    }
                });
            }
        }

        public void Unload(Mobile m)
        {
            if (Primed)
            {
                m.AddToBackpack(new FuseCord());
                AddAction(m, 1149686); // Fuse removed.
                Primed = false;
            }

            if (Loaded)
            {
                Item item;

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
                    DoAreaMessage(AmmoType == AmmunitionType.Grapeshot ? 1116067 : 1116066, 10, m); //~1_NAME~ carefully removes the powder charge from the cannon.
                    AddAction(m, 1149685); // Ammunition removed.
                    m.AddToBackpack(item);
                }

                AmmoType = AmmunitionType.Empty;
                Loaded = false;
            }

            if (Charged)
            {
                m.AddToBackpack(new PowderCharge());
                AddAction(m, 1149684); // Powder charge removed.
                Charged = false;
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

        private Type[] _Types =
        {
            typeof(Cannonball), typeof(Grapeshot), typeof(PowderCharge), typeof(FuseCord)
        };

        public override void OnDoubleClick(Mobile from)
        {
            if (Galleon.GetSecurityLevel(from) >= SecurityLevel.Crewman)
            {
                base.OnDoubleClick(from);

                if (!Viewing.Contains(from))
                {
                    Viewing.Add(from);
                }

                AddAction(from, from.InRange(Location, 2) ? 1149653 : 1149654); // You are now operating the cannon. : You are too far away.
                ResendGump(from, TimeSpan.FromMilliseconds(500));
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

            IPooledEnumerable eable = this.GetMobilesInRange(6);

            foreach (Mobile mob in eable)
            {
                if (mob != from && mob is PlayerMobile && mob.InLOS(this))
                {
                    if (from != null)
                        mob.SendLocalizedMessage(cliloc, from.Name);
                    else
                        mob.SendLocalizedMessage(cliloc);
                }
            }

            eable.Free();
        }

        public void TryLightFuse(Mobile from)
        {
            if (from == null)
                return;

            Container pack = from.Backpack;

            if(pack != null)
            {
                Item[] items = pack.FindItemsByType(typeof(Matches));

                if (items != null)
                {
                    foreach(Item item in items)
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

            AddAction(from, 1149669); //Need a lighted match.
        }

        public void LightFuse(Mobile from)
        {
            if (!CheckRegion(from))
                return;

            DoAreaMessage(1116080, 10, from);
            AddAction(from, 1149683); //The fuse is lit!
            Effects.PlaySound(this.Location, this.Map, 0x666);
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), Shoot, from);
        }

        public bool CheckRegion(Mobile from)
        {
            Region r = Region.Find(from.Location, from.Map);

            if (r is Server.Regions.GuardedRegion && !((Server.Regions.GuardedRegion)r).IsDisabled())
            {
                from.SendMessage("You are forbidden from discharging cannons within the town limits.");
                return false;
            }

            return true;
        }

        public virtual void Shoot(object cannoneer)
        {
            AmmoInfo ammo = AmmoInfo.GetAmmoInfo(AmmoType);

            if (ammo == null)
                return;

            Mobile shooter = null;
            
            if(cannoneer is Mobile)
                shooter = (Mobile)cannoneer;

            if(shooter != null && shooter.Player)
                m_Hits -= Utility.RandomMinMax(0, 4);

            DoShootEffects();
            AddAction(shooter, 1149691); //Fired successfully.

            int xOffset = 0; int yOffset = 0;
            int currentRange = 0;
            Point3D pnt = this.Location;
            Map map = this.Map;
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

                TimeSpan delay = TimeSpan.FromSeconds((double)currentRange / 10.0);

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

                                BaseGalleon g = FindValidBoatTarget(newPoint, map, ammo);

                                if (g != null && g != Galleon && g.IsEnemy(Galleon))
                                    list.Add(g);

                                damageables.AddRange(FindDamageables(shooter, newPoint, map, false, false, false, true, true));
                            }

                            foreach (var m in damageables)
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
                                else if (toHit is BaseGalleon)
                                {
                                    Timer.DelayCall(delay, new TimerStateCallback(OnShipHit), new object[] { (BaseGalleon)toHit, newPoint, ammo, shooter });
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
                            List<Mobile> mobiles = new List<Mobile>();

                            for (int i = -lateralOffset; i <= lateralOffset; i++)
                            {
                                if (xOffset == 0)
                                    newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                                else
                                    newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                                mobiles.AddRange(FindDamageables(shooter, newPoint, map, true, true, true, true, false).OfType<Mobile>());
                            }

                            if (mobiles.Count > 0)
                            {
                                Timer.DelayCall(delay, new TimerStateCallback(OnMobileHit), new object[] { mobiles, newPoint, ammo, shooter });
                                hit = true;
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

        private BaseGalleon FindValidBoatTarget(Point3D newPoint, Map map, AmmoInfo info)
        {
            BaseGalleon galleon = BaseGalleon.FindGalleonAt(newPoint, map);

            if (galleon != null && info.RequiresSurface)
            {
                int d = galleon is BritannianShip ? 3 : 2; 
                switch (galleon.Facing)
                {
                    case Direction.North:
                    case Direction.South:
                        if (newPoint.X <= galleon.X - d || newPoint.X >= galleon.X + d)
                            return null;
                        break;
                    case Direction.East:
                    case Direction.West:
                        if (newPoint.Y <= galleon.Y - d || newPoint.Y >= galleon.Y + d)
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
                        return galleon;
                    }
                }

                return null;
            }

            return galleon;
        }

        public void DoShootEffects()
        {
            Point3D p = this.Location;
            Map map = this.Map;

            p.Z -= 3;

            switch (Facing)
            {
                case Direction.North: p.Y--; break;
                case Direction.East:  p.X++; break;
                case Direction.South: p.Y++; break;
                case Direction.West:  p.X--; break;
            }

            Effects.SendLocationEffect(p, map, 14120, 15, 10);
            Effects.PlaySound(p, map, 0x664);
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
                DoAreaMessage(1116297, 5, null); //The ship cannon has been destroyed!
                Delete();

                if (from != null && from.InRange(this.Location, 5))
                    from.SendLocalizedMessage(1116297); //The ship cannon has been destroyed!
            }

            InvalidateProperties();
        }

        public void ClearCannon()
        {
            Charged = false;
            Loaded = false;
            AmmoType = AmmunitionType.Empty;
            Primed = false;

            InvalidateProperties();
        }

        public virtual void OnShipHit(object obj)
        {
            object[] list = (object[])obj;
            BaseGalleon target = list[0] as BaseGalleon;
            Point3D pnt = (Point3D)list[1];
            AmmoInfo ammoInfo = list[2] as AmmoInfo;
            Mobile shooter = list[3] as Mobile;

            if (target != null && Galleon != null)
            {
                int damage = (int)(ammoInfo.GetDamage(this) * Galleon.CannonDamageMod);
                target.OnTakenDamage(shooter, damage);

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

                Effects.SendLocationEffect(hit, target.Map, Utility.RandomBool() ? 14000 : 14013, 15, 10);
                Effects.PlaySound(hit, target.Map, 0x207);

                Mobile victim = target.Owner;

                if (victim != null && target.Contains(victim) && shooter.CanBeHarmful(victim, false))
                {
                    shooter.DoHarmful(victim);
                }
                else
                {
                    List<Mobile> candidates = new List<Mobile>();
                    SecurityLevel highest = SecurityLevel.Passenger;

                    foreach (var mob in target.GetMobilesOnBoard().OfType<PlayerMobile>().Where(pm => shooter.CanBeHarmful(pm, false)))
                    {
                        if (Galleon.GetSecurityLevel(mob) > highest)
                        {
                            candidates.Insert(0, mob);
                        }
                        else
                        {
                            candidates.Add(mob);
                        }
                    }

                    if (candidates.Count > 0)
                    {
                        shooter.DoHarmful(candidates[0]);
                    }
                    else if (victim != null && shooter.IsHarmfulCriminal(victim))
                    {
                        shooter.CriminalAction(false);
                    }

                    ColUtility.Free(candidates);
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

        public virtual void OnMobileHit(object obj)
        {
            object[] objects = (object[])obj;
            var toHit = objects[0] as Mobile;
            var pnt = (Point3D)objects[1];
            var info = objects[2] as AmmoInfo;
            var shooter = objects[3] as Mobile;

            int damage = (int)(Utility.RandomMinMax(info.MinDamage, info.MaxDamage) * Galleon.CannonDamageMod);

            if (info == null)
                return;

            if (toHit != null)
            {
                //only cannonballs will get the damage bonus
                //if (toHit is BaseSeaChampion && info.AmmoType != AmmunitionType.Empty && info.AmmoType == AmmunitionType.Cannonball)
                 //   damage *= 75;

                shooter.DoHarmful(toHit);
                AOS.Damage(toHit, shooter, damage, info.PhysicalDamage, info.FireDamage, info.ColdDamage, info.PoisonDamage, info.EnergyDamage);
                Effects.SendLocationEffect(toHit.Location, toHit.Map, Utility.RandomBool() ? 14000 : 14013, 15, 10);
                Effects.PlaySound(toHit.Location, toHit.Map, 0x207);
            }
        }

        public virtual void OnDamageableItemHit(object obj)
        {
            object[] objects = (object[])obj;
            var toHit = objects[0] as DamageableItem;
            var pnt = (Point3D)objects[1];
            var info = objects[2] as AmmoInfo;
            var shooter = objects[3] as Mobile;

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

                if (seacreature && mob is BaseSeaChampion)
                    list.Add(mob);
            }

            eable.Free();
            return list;
        }

        public void TryRepairCannon(Mobile from)
        {
			Container pack = from.Backpack;
            Container hold = Galleon.GalleonHold;
			
			if(pack == null)
				return;

            //double ingotsNeeded = 36 * (100 - Durability);
            double ingotsNeeded = 36 * (int)DamageState;

            ingotsNeeded -= ((double)from.Skills[SkillName.Blacksmith].Value / 200.0) * ingotsNeeded;

            double min = ingotsNeeded / 10;
            double ingots1 = pack.GetAmount(typeof(IronIngot));
            double ingots2 = hold != null ? hold.GetAmount(typeof(IronIngot)) : 0;
            double ingots = ingots1 + ingots2;
            double ingotsUsed, percRepaired;

			if(ingots < min)
            {
				from.SendLocalizedMessage(1116603, ((int)min).ToString()); //You need a minimum of ~1_METAL~ iron ingots to repair this cannon.
                return;
            }
					
			if(ingots >= ingotsNeeded)
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
			if(m_Hits > MaxHits) m_Hits = MaxHits;
			InvalidateDamageState();
					
			percRepaired += Durability;
			if(percRepaired > 100) percRepaired = 100;
				
			from.SendLocalizedMessage(1116605, String.Format("{0}\t{1}", ((int)temp).ToString(), ((int)percRepaired).ToString())); //You make repairs to the cannon using ~1_METAL~ ingots. The cannon is now ~2_DMGPCT~% repaired.
        }

        public void ResendGump(Mobile from)
        {
            ResendGump(from, TimeSpan.Zero);
        }

        public void ResendGump(Mobile from, TimeSpan delay)
        {
            if (!Viewing.Contains(from))
            {
                Viewing.Add(from);
            }

            foreach (var pm in Viewing.OfType<PlayerMobile>())
            {
                var gump = BaseGump.GetGump<ShipCannonGump>(pm, g => g.Cannon == this);

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

        public Dictionary<Mobile, List<int>> Actions { get { return m_Actions; } }
        private Dictionary<Mobile, List<int>> m_Actions = new Dictionary<Mobile, List<int>>();

        public void AddAction(Mobile from, int action)
        {
            if (from == null)
                return;

            if (!m_Actions.ContainsKey(from))
                m_Actions[from] = new List<int>();

            var list = m_Actions[from];

            list.Insert(0, action);

            while (list.Count > 3)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1116026, Charged ? "#1116031" : "#1116032"); // Charged: ~1_VALUE~
            list.Add(1116027, String.Format("{0}", AmmoInfo.GetAmmoName(this).ToString())); // Ammo: ~1_VALUE~
            list.Add(1116028, Primed ? "#1116031" : "#1116032"); //Primed: ~1_VALUE~
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
                : base(1116072, 2)
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
                if (!Cannon.Empty)
                {
                    From.SendLocalizedMessage(1116321); //The ship cannon and magazine must be fully unloaded before it can be dismantled.
                }
                else if (Cannon.DamageState != DamageLevel.Pristine)
                {
                    From.SendLocalizedMessage(1116322); //The ship cannon must be fully repaired before it can be dismantled.
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

            var list = new List<PlayerMobile>(Viewing.OfType<PlayerMobile>());
            
            foreach (var pm in list)
            {
                var gump = BaseGump.GetGump<ShipCannonGump>(pm, g => g.Cannon == this);

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
            writer.Write(0);

            writer.Write(Galleon);

            writer.Write(Charged);
            writer.Write(Loaded);
            writer.Write(Primed);
            writer.Write((int)AmmoType);
            writer.Write((int)Position);

            writer.Write(m_Hits);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Galleon = reader.ReadItem() as BaseGalleon;

            Charged = reader.ReadBool();
            Loaded = reader.ReadBool();
            Primed = reader.ReadBool();
            AmmoType = (AmmunitionType)reader.ReadInt();
            Position = (ShipPosition)reader.ReadInt();

            m_Hits = reader.ReadInt();

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

                AddHtmlLocalized(115, 53, 115, 18, Cannon.Charged ? 1149631 : 1149632, Cannon.Charged ? 0x1FE7 : 0x7CE7, false, false); // Charged / Not Charged
                AddHtmlLocalized(115, 71, 115, 18, 1114057, Cannon.Loaded ? AmmoInfo.GetAmmoName(Cannon).ToString() : "#1149636", Cannon.Loaded ? 0x1FE7 : 0x7CE7, false, false); // Cannonball / Not Loaded
                AddHtmlLocalized(115, 89, 115, 18, Cannon.Primed ? 1149640 : 1149639, Cannon.Primed ? 0x1FE7 : 0x7CE7, false, false); // Primed / No Fuse

                if (Cannon.Actions.ContainsKey(User))
                {
                    var actual = 0;
                    var list = Cannon.Actions[User];

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        AddHtmlLocalized(10, 112 + (actual * 18), 230, 18, Cannon.Actions[User][i], actual == list.Count - 1 ? 0x7FE7 : 0x3DEF, false, false);
                        actual++;
                    }
                }
            }

            public override void OnResponse(RelayInfo info)
            {
                if (info.ButtonID == 0)
                {
                    if (Cannon.Viewing.Contains(User))
                    {
                        Cannon.Viewing.Remove(User);
                    }

                    return;
                }
                if (!User.InRange(Cannon.Location, 2))
                {
                    Cannon.AddAction(User, 1149654); // You are too far away.
                }
                else
                {
                    switch (info.ButtonID)
                    {
                        case 1:
                            if (!Cannon.Charged)
                            {
                                Cannon.TryCharge(User);
                            }
                            else if (!Cannon.Loaded)
                            {
                                Cannon.TryLoad(User);
                            }
                            else if (!Cannon.Primed)
                            {
                                Cannon.TryPrime(User);
                            }
                            break;
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
        public override int Range { get { return 10; } }
        public override ShipCannonDeed GetDeed { get { return new CulverinDeed(); } }
        public override CannonPower Power { get { return CannonPower.Light; } }

        public Culverin(BaseGalleon g) : base(g)
        {
        }

        public Culverin(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Carronade : BaseShipCannon
    {
        public override int Range { get { return 10; } }
        public override ShipCannonDeed GetDeed { get { return new CarronadeDeed(); } }
        public override CannonPower Power { get { return CannonPower.Heavy; } }

        public override TimeSpan ActionTime { get { return TimeSpan.FromSeconds(2.0); } }

        public Carronade(BaseGalleon g) : base(g)
        {
        }

        public Carronade(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Blundercannon : BaseShipCannon
    {
        public override int LabelNumber { get { return 1158942; } } // Blundercannon

        public override int Range { get { return 12; } }
        public override ShipCannonDeed GetDeed { get { return new BlundercannonDeed(); } }
        public override CannonPower Power { get { return CannonPower.Massive; } }

        public override TimeSpan ActionTime { get { return TimeSpan.FromSeconds(2.0); } }

        public Blundercannon(BaseGalleon g) : base(g)
        {
        }

        public Blundercannon(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
